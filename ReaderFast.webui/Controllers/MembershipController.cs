using System;
using System.Security.Claims;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReaderFast.webui.Areas.Identity.Data;
using ReaderFast.webui.Data;
using ReaderFast.webui.Models;

namespace shopapp.webui.Controllers
{
    public class MembershipController : Controller
    {
        private readonly ReaderFastDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public MembershipController(UserManager<ApplicationUser> userManager, ReaderFastDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var firstProduct = _context.Products.FirstOrDefault();
            if (firstProduct != null)
            {
                ViewData["Price"] = firstProduct.Price;
                ViewData["Name"] = firstProduct.Name;
                ViewData["Category"] = firstProduct.Category;
            }

            // Bu kısımda gerektiği şekilde modelinizi doldurup view'a gönderin.
            return View(new OrderModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Checkout(OrderModel model)
        {
            // Check whether ModelState is valid
            if (!ModelState.IsValid)
            {
                // If not, return the view with the current model to show validation errors
                return View(model);
            }

            // Check if the user has agreed to the sales agreement
            if (!model.HasAgreedToSalesAgreement)
            {
                // If not, add a model error and return the view with the current model
                ModelState.AddModelError("HasAgreedToSalesAgreement", "You must agree to the Sales Agreement to proceed.");
                return View(model);
            }

            // If ModelState is valid and user has agreed to the sales agreement, proceed with processing
            var userId = _userManager.GetUserId(User);
            var payment = PaymentProcess(model, userId);

            if (payment.Status == "success")
            {
                // Create a new order object and set its properties from the model
                var order = new OrderModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    City = model.City,
                    Country = model.Country,
                    Phone = model.Phone,
                    Email = model.Email,
                    IdentityNumber = model.IdentityNumber,
                    HasAgreedToSalesAgreement = model.HasAgreedToSalesAgreement  // Set the agreement acknowledgment
                };

                // Add the order object to the Orders set and save changes to the database
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Save membership data and return the Success view
                await SaveMembershipData(userId);
                return View("Success");
            }
            else
            {
                // If payment is not successful, create an alert message and add it to TempData
                var msg = new AlertMessage()
                {
                    Message = $"{payment.ErrorMessage}",
                    AlertType = "danger"
                };
                TempData["message"] = JsonConvert.SerializeObject(msg);
                return View(model);  // Return the view with the current model
            }
        }


        private Payment PaymentProcess(OrderModel model, string userId)
        {
            string userIp = GetUserIpAddress();
            var product = _context.Products.FirstOrDefault(); // İlk ürünü alır.
            if (product == null)
            {
                throw new Exception("No product found in the database.");
            }


            Options options = new Options();
            options.ApiKey = _configuration["API:ApiKey"];
            options.SecretKey = _configuration["API:SecretKey"];
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(111111111, 999999999).ToString();
            request.Price = product.Price.ToString();
            request.PaidPrice = product.Price .ToString();
            request.Currency = Currency.USD.ToString();
            request.Installment = 1;
           
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = userId;
            buyer.Name = model.FirstName;
            buyer.Surname = model.LastName;
            buyer.GsmNumber = model.Phone;
            buyer.Email = model.Email;
            buyer.IdentityNumber = model.IdentityNumber;
          
            buyer.RegistrationAddress = model.Address;
            buyer.Ip = userIp;
            buyer.City = model.City;
            buyer.Country = model.Country;            
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = model.FirstName;
            shippingAddress.City = model.City;
            shippingAddress.Country = model.Country;
            shippingAddress.Description = model.Address;          
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = model.FirstName;
            billingAddress.City = model.City;
            billingAddress.Country = model.Country;
            billingAddress.Description = model.Address;
            
            request.BillingAddress = billingAddress;
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem firstBasketItem = new BasketItem();
            firstBasketItem.Id = product.Id.ToString();
            firstBasketItem.Name = product.Name;
            firstBasketItem.Category1 = product.Category;
            firstBasketItem.Category2 = product.Category;
            firstBasketItem.ItemType = BasketItemType.VIRTUAL.ToString();
            firstBasketItem.Price = product.Price.ToString();
            basketItems.Add(firstBasketItem);         
            request.BasketItems = basketItems;
            return Payment.Create(request, options);
        }

        private string GetUserIpAddress()
        {
            return HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                   HttpContext.Connection.RemoteIpAddress?.ToString();
        }
        private async Task RefreshUserClaims(ApplicationUser user)
        {
            // Kullanıcının mevcut claims'lerini alın
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userIdentity = new ClaimsIdentity(userClaims, "Cookie");

            // Yeni claim'leri ekleyin (eğer varsa)
            var newClaim = new Claim(ClaimTypes.Role, "PremiumUser");
            userIdentity.AddClaim(newClaim);

            // Kullanıcının kimliğini yeniden oluşturun
            var authProperties = new AuthenticationProperties();
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authProperties);
        }

        private async Task SaveMembershipData(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, "PremiumUser");

                if (!result.Succeeded)
                {
                    throw new Exception("User could not be added to the PremiumUser role.");
                }

                // PremiumUser rolü atandıktan sonra PremiumRoleAssignedDate özelliğini güncelle
                user.PremiumRoleAssignedDate = DateTime.UtcNow;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    throw new Exception("User's PremiumRoleAssignedDate could not be updated.");
                }

                // Kullanıcının claims'lerini ve kimliğini yeniden başlatın
                await RefreshUserClaims(user);
            }
        }

    }
}
