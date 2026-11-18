using Microsoft.AspNetCore.Mvc;
using SAGIP.Models;
using SAGIP.Services;

namespace SAGIP.Controllers;

public class ReportController : Controller
{
    private readonly ISagipService _sagipService;
    private readonly I18nService _i18n;

    public ReportController(ISagipService sagipService, I18nService i18n)
    {
        _sagipService = sagipService;
        _i18n = i18n;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.Settings = _sagipService.GetSettings();
        var model = new ReportFormModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(ReportFormModel model)
    {
        // Honeypot check
        if (!string.IsNullOrEmpty(model.Honeypot))
        {
            return RedirectToAction("Index", "Home");
        }

        if (string.IsNullOrWhiteSpace(model.IncidentDescription))
        {
            ModelState.AddModelError("IncidentDescription", _i18n.Text(
                "Please describe what happened so our desk officer can assist you.",
                "Pakilarawan ang nangyari upang matulungan ka ng aming desk officer."));
        }

        if (model.ReportType == "Identified")
        {
            if (string.IsNullOrWhiteSpace(model.ReporterName))
            {
                ModelState.AddModelError("ReporterName", _i18n.Text("Please provide your name.", "Pakibigay ang iyong pangalan."));
            }
            if (string.IsNullOrWhiteSpace(model.ContactValue))
            {
                ModelState.AddModelError("ContactValue", _i18n.Text("Please provide an email or phone number.", "Pakibigay ang email o numero ng telepono."));
            }
            if (!model.ContactConsent)
            {
                ModelState.AddModelError("ContactConsent", _i18n.Text(
                    "You must consent to being contacted, or choose Anonymous reporting instead.",
                    "Kailangan mong sumang-ayon na makontak, o pumili ng Anonymous na pag-uulat."));
            }
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Settings = _sagipService.GetSettings();
            return View(model);
        }

        var result = _sagipService.SubmitReport(model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Submission failed.");
            ViewBag.Settings = _sagipService.GetSettings();
            return View(model);
        }

        TempData["ReportRefCode"] = result.ReferenceCode;
        TempData["IsRA9262"] = result.IsRA9262Pathway;
        TempData["ReceivingOffice"] = result.ReceivingOffice;
        TempData["ReportType"] = result.ReportType;

        return RedirectToAction(nameof(Confirmation));
    }

    [HttpGet]
    public IActionResult Confirmation()
    {
        var refCode = TempData["ReportRefCode"] as string;
        if (string.IsNullOrEmpty(refCode))
        {
            return RedirectToAction("Index", "Home");
        }

        var vm = new ReportConfirmationViewModel
        {
            Success = true,
            ReferenceCode = refCode,
            IsRA9262Pathway = TempData["IsRA9262"] as bool? ?? true,
            ReceivingOffice = (TempData["ReceivingOffice"] as string) ?? _sagipService.GetSettings().ReceivingOfficeName,
            ReportType = (TempData["ReportType"] as string) ?? "Anonymous"
        };

        ViewBag.Settings = _sagipService.GetSettings();
        return View(vm);
    }
}
