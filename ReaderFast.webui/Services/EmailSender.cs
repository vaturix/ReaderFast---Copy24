using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using ReaderFast.webui.Services; // IViewRenderService'in bulunduğu namespace'i ekleyin

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _client;
    private readonly IViewRenderService _viewRenderService; // ViewRenderService için eklenen değişken

    // Constructor'a IViewRenderService enjeksiyonu ekleyin
    public EmailSender(IConfiguration configuration, IViewRenderService viewRenderService)
    {
        _client = new SmtpClient
        {
            Host = configuration["Email:Smtp:Host"],
            Port = int.Parse(configuration["Email:Smtp:Port"]),
            EnableSsl = bool.Parse(configuration["Email:Smtp:EnableSsl"]),
            Credentials = new NetworkCredential(
                configuration["Email:Smtp:Username"],
                configuration["Email:Smtp:Password"])
        };
        _viewRenderService = viewRenderService; // Servisi sınıf değişkenine atama
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MailMessage("your-email@example.com", email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };

        return _client.SendMailAsync(message);
    }

    // .cshtml şablonlarını kullanarak e-posta göndermek için yeni bir metod
    public async Task SendEmailFromTemplateAsync(string email, string subject, string viewName, object model)
    {
        var emailBody = await _viewRenderService.RenderToStringAsync(viewName, model);
        await SendEmailAsync(email, subject, emailBody);
    }
}
