using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using ReaderFast.webui.Areas.Identity.Data;
using ReaderFast.webui.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity.UI.Services;
using ReaderFast.webui.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization.Routing;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var connectionString = builder.Configuration.GetConnectionString("ReaderFastDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ReaderFastDbContextConnection' not found.");

// Services

builder.Services.AddDbContext<ReaderFastDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddTransient<EmailSender>(); 



builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ReaderFastDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.SameSite = SameSiteMode.None;
    options.ExpireTimeSpan = TimeSpan.FromDays(5);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(); 

builder.Services.AddRazorPages();
builder.Services.AddSingleton<IdentityErrorDescriber, CustomIdentityErrorDescriber>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

// Localization Configuration
var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("tr-TR")
};
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.AddInitialRequestCultureProvider(new RouteDataRequestCultureProvider
    {
        Options = options
    });
    options.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new QueryStringRequestCultureProvider(),
    new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()

};

});


// Build Application
var app = builder.Build();
var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

// Other Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

// Run Application
app.Run();
