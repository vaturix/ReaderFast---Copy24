using Microsoft.AspNetCore.Mvc;

public class SupportController : Controller
{
    public IActionResult SalesAgreement()
    {
        return View();
    }

    public IActionResult UserAgreement()
    {
        return View();
    }

    public IActionResult PrivacyPolicy()
    {
        return View();
    }
}
