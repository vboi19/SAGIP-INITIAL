using Microsoft.AspNetCore.Mvc;
using SAGIP.Models;
using SAGIP.Services;

namespace SAGIP.Controllers;

public class HomeController : Controller
{
    private readonly ISagipService _sagipService;
    private readonly I18nService _i18n;

    public HomeController(ISagipService sagipService, I18nService i18n)
    {
        _sagipService = sagipService;
        _i18n = i18n;
    }

    public IActionResult Index()
    {
        ViewBag.Settings = _sagipService.GetSettings();
        return View();
    }

    public IActionResult Privacy()
    {
        ViewBag.Settings = _sagipService.GetSettings();
        return View();
    }

    public IActionResult Rights()
    {
        return View();
    }

    public IActionResult Contacts(string? category, string? search)
    {
        var contacts = _sagipService.GetSupportContacts(activeOnly: true);

        if (!string.IsNullOrWhiteSpace(category) && category != "All")
        {
            contacts = contacts.Where(c =>
                c.CategoryEn.Contains(category, StringComparison.OrdinalIgnoreCase) ||
                c.CategoryFil.Contains(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            contacts = contacts.Where(c =>
                c.OfficeNameEn.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.OfficeNameFil.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                c.Phone.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                (c.AddressEn != null && c.AddressEn.Contains(s, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        ViewBag.Settings = _sagipService.GetSettings();
        ViewBag.SelectedCategory = category ?? "All";
        ViewBag.SearchQuery = search ?? "";
        return View(contacts);
    }

    public IActionResult Accessibility()
    {
        return View();
    }

    public IActionResult Notebook()
    {
        ViewBag.Settings = _sagipService.GetSettings();
        return View();
    }

    public IActionResult QuickExit()
    {
        HttpContext.Session.Clear();
        var settings = _sagipService.GetSettings();
        var decoy = !string.IsNullOrEmpty(settings.QuickExitUrl) ? settings.QuickExitUrl : "https://www.pagasa.dost.gov.ph";
        return Redirect(decoy);
    }

    [HttpPost]
    public IActionResult SetLanguage(string lang, string returnUrl = "/")
    {
        if (lang == "fil" || lang == "en")
        {
            Response.Cookies.Append("sagip_lang", lang, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });
        }

        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
