using Microsoft.AspNetCore.Mvc;
using SAGIP.Models;
using SAGIP.Services;

namespace SAGIP.Controllers;

public class StatusController : Controller
{
    private readonly ISagipService _sagipService;
    private readonly I18nService _i18n;

    public StatusController(ISagipService sagipService, I18nService i18n)
    {
        _sagipService = sagipService;
        _i18n = i18n;
    }

    [HttpGet]
    public IActionResult Index(string? code)
    {
        ViewBag.Settings = _sagipService.GetSettings();
        if (string.IsNullOrWhiteSpace(code))
        {
            return View(new StatusLookupViewModel());
        }

        var result = _sagipService.LookupStatus(code);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Lookup(StatusLookupViewModel model)
    {
        ViewBag.Settings = _sagipService.GetSettings();
        if (string.IsNullOrWhiteSpace(model.ReferenceCode))
        {
            ModelState.AddModelError("ReferenceCode", _i18n.Text("Please enter a valid reference code.", "Pakipasok ang wastong reference code."));
            return View("Index", model);
        }

        var result = _sagipService.LookupStatus(model.ReferenceCode);
        return View("Index", result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult WithdrawConsent(string referenceCode)
    {
        var result = _sagipService.WithdrawConsent(referenceCode);
        if (result.ok)
        {
            TempData["SuccessMessage"] = _i18n.Text(
                "Your contact consent has been successfully withdrawn. Our desk officer will not reach out to your provided contact details.",
                "Matagumpay na binawi ang iyong pahintulot sa pakikipag-ugnayan. Hindi na makikipag-ugnayan ang aming desk officer sa iyong mga detalye.");
        }
        else
        {
            TempData["ErrorMessage"] = result.error ?? "Failed to withdraw consent.";
        }
        return RedirectToAction(nameof(Index), new { code = referenceCode });
    }
}
