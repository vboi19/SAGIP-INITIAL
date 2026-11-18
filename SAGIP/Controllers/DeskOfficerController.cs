using Microsoft.AspNetCore.Mvc;
using SAGIP.Models;
using SAGIP.Services;

namespace SAGIP.Controllers;

public class DeskOfficerController : Controller
{
    private readonly ISagipService _sagipService;
    private readonly I18nService _i18n;

    public DeskOfficerController(ISagipService sagipService, I18nService i18n)
    {
        _sagipService = sagipService;
        _i18n = i18n;
    }

    private StaffUser? GetCurrentOfficer()
    {
        var userId = HttpContext.Session.GetString("StaffUserId");
        if (string.IsNullOrEmpty(userId)) return null;

        var user = _sagipService.GetStaffUserById(userId);
        if (user == null || user.Role != SagipConstants.RoleDeskOfficer) return null;
        return user;
    }

    [HttpGet]
    public IActionResult Index(string? status, string? urgency, string? officerId, string? search)
    {
        var officer = GetCurrentOfficer();
        if (officer == null)
        {
            return RedirectToAction("Login", "Staff", new { returnUrl = Request.Path });
        }

        var cases = _sagipService.GetCases(status, urgency, officerId, search);
        var reports = cases.Select(c => _sagipService.GetReportById(c.ReportId)!).Where(r => r != null).ToList();
        var officers = _sagipService.GetStaffUsers().Where(u => u.Role == SagipConstants.RoleDeskOfficer && u.Status == "Active").ToList();
        var alerts = _sagipService.GetAlerts();

        var vm = new DeskOfficerDashboardViewModel
        {
            CurrentUser = officer,
            Settings = _sagipService.GetSettings(),
            Cases = cases,
            Reports = reports,
            Officers = officers,
            SupportContacts = _sagipService.GetSupportContacts(),
            Alerts = alerts,
            StatusFilter = status ?? "All",
            UrgencyFilter = urgency ?? "All",
            OfficerFilter = officerId ?? "All",
            SearchQuery = search ?? ""
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Details(string id)
    {
        var officer = GetCurrentOfficer();
        if (officer == null)
        {
            return RedirectToAction("Login", "Staff", new { returnUrl = Request.Path });
        }

        var caseRec = _sagipService.GetCaseById(id);
        if (caseRec == null) return NotFound();

        var report = _sagipService.GetReportById(caseRec.ReportId);
        if (report == null) return NotFound();

        _sagipService.RecordCaseView(caseRec.CaseId, officer.UserId, officer.FullName);

        var vm = new CaseDetailViewModel
        {
            CurrentUser = officer,
            Case = caseRec,
            Report = report,
            Consents = _sagipService.GetConsentsByReportId(report.ReportId),
            Notes = _sagipService.GetCaseNotes(caseRec.CaseId),
            StatusHistory = _sagipService.GetStatusHistory(caseRec.CaseId),
            AvailableOfficers = _sagipService.GetStaffUsers().Where(u => u.Role == SagipConstants.RoleDeskOfficer && u.Status == "Active").ToList(),
            SupportContacts = _sagipService.GetSupportContacts(activeOnly: true),
            Settings = _sagipService.GetSettings(),
            IsConflictFlaggedByCurrent = caseRec.ConflictFlaggedByIds.Contains(officer.UserId)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateTriage(string caseId, string urgency, List<string> abuseTypes)
    {
        var officer = GetCurrentOfficer();
        if (officer == null) return Unauthorized();

        _sagipService.UpdateCaseTriage(caseId, urgency, abuseTypes, officer.UserId, officer.FullName);
        TempData["SuccessMessage"] = "Case triage updated successfully.";
        return RedirectToAction(nameof(Details), new { id = caseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveAssessment(string caseId, string outcome, string pathway, string urgency, List<string> abuseTypes, string notes)
    {
        var officer = GetCurrentOfficer();
        if (officer == null) return Unauthorized();

        _sagipService.SaveCaseAssessment(caseId, outcome, pathway, urgency, abuseTypes, notes, officer.UserId, officer.FullName);
        TempData["SuccessMessage"] = "Case assessment recorded and status updated to Under Assessment.";
        return RedirectToAction(nameof(Details), new { id = caseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecordReferral(string caseId, string outsideOffice, string referralDate, string purpose, string? reference, string? notes)
    {
        var officer = GetCurrentOfficer();
        if (officer == null) return Unauthorized();

        _sagipService.RecordCaseReferral(caseId, outsideOffice, referralDate, purpose, reference, notes, officer.UserId, officer.FullName);
        TempData["SuccessMessage"] = $"Referral to {outsideOffice} recorded successfully.";
        return RedirectToAction(nameof(Details), new { id = caseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RecordBpo(string caseId, string bpoRef, string issuedBy, string dateIssued, string? effectiveUntil, string? notes)
    {
        var officer = GetCurrentOfficer();
        if (officer == null) return Unauthorized();

        _sagipService.RecordCaseBpo(caseId, bpoRef, issuedBy, dateIssued, effectiveUntil, notes, officer.UserId, officer.FullName);
        TempData["SuccessMessage"] = $"Barangay Protection Order ({bpoRef}) recorded successfully.";
        return RedirectToAction(nameof(Details), new { id = caseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(string caseId, string newStatus, string? reason, string? assignOfficerId)
    {
        var officer = GetCurrentOfficer();
        if (officer == null) return Unauthorized();

        _sagipService.UpdateCaseStatus(caseId, newStatus, reason, officer.UserId, officer.FullName, assignOfficerId);
        TempData["SuccessMessage"] = $"Status updated to {newStatus}.";
        return RedirectToAction(nameof(Details), new { id = caseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AssignOfficer(string caseId, string? officerId)
    {
        var officer = GetCurrentOfficer();
        if (officer == null) return Unauthorized();

        _sagipService.AssignCase(caseId, officerId, officer.UserId, officer.FullName);
        TempData["SuccessMessage"] = "Assigned officer updated.";
        return RedirectToAction(nameof(Details), new { id = caseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddNote(string caseId, string noteText)
    {
        var officer = GetCurrentOfficer();
        if (officer == null) return Unauthorized();

        if (!string.IsNullOrWhiteSpace(noteText))
        {
            _sagipService.AddCaseNote(caseId, noteText.Trim(), officer.UserId, officer.FullName);
            TempData["SuccessMessage"] = "Case note added.";
        }
        return RedirectToAction(nameof(Details), new { id = caseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult FlagConflict(string caseId)
    {
        var officer = GetCurrentOfficer();
        if (officer == null) return Unauthorized();

        _sagipService.FlagConflict(caseId, officer.UserId, officer.FullName);
        TempData["WarningMessage"] = "Conflict of interest flagged. You have recused yourself from this case.";
        return RedirectToAction(nameof(Details), new { id = caseId });
    }
}
