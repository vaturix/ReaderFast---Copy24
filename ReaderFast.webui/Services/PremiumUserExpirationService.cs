using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReaderFast.webui.Areas.Identity.Data; // ApplicationUser modelinizin bulunduğu namespace.

public class PremiumUserExpirationService : BackgroundService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public PremiumUserExpirationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("PremiumUser") && user.PremiumRoleAssignedDate.HasValue && user.PremiumRoleAssignedDate.Value.AddYears(1) < DateTime.UtcNow)
                {
                    await _userManager.RemoveFromRoleAsync(user, "PremiumUser");
                }
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Run once a day
        }
    }

}


