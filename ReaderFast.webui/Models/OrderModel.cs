using System.ComponentModel.DataAnnotations;

namespace ReaderFast.webui.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string IdentityNumber { get; set; }
        public string Email { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string Cvc { get; set; }
        public bool HasAgreedToSalesAgreement { get; set; } // New Property

    }
    public class AlertMessage
    {
        public string Message { get; set; }
        public string AlertType { get; set; }
    }
}
