using Microsoft.AspNetCore.Mvc;
using SAGIP.Models;
using SAGIP.Services;

namespace SAGIP.Controllers;

public class AdminController : Controller
{
    private readonly ISagipService _sagipService;
    private readonly I18nService _i18n;

    public AdminController(ISagipService sagipService, I18nService i18n)
    {
        _sagipService = sagipService;
        _i18n = i18n;
    }

    private StaffUser? GetCurrentAdmin()
    {
        var userId = HttpContext.Session.GetString("StaffUserId");
        if (string.IsNullOrEmpty(userId)) return null;

        var user = _sagipService.GetStaffUserById(userId);
        if (user == null || user.Role != SagipConstants.RoleAdministrator) return null;
        return user;
    }

    [HttpGet]
    public IActionResult Index(string tab = "stats", string? auditActor = null, string? auditAction = null, string? auditRecord = null)
    {
        var admin = GetCurrentAdmin();
        if (admin == null)
        {
            return RedirectToAction("Login", "Staff", new { returnUrl = Request.Path });
        }

        var vm = new AdminDashboardViewModel
        {
            CurrentUser = admin,
            Settings = _sagipService.GetSettings(),
            StaffUsers = _sagipService.GetStaffUsers(),
            SupportContacts = _sagipService.GetSupportContacts(activeOnly: false),
            AuditLogs = _sagipService.GetAuditLogs(auditActor, auditAction, auditRecord),
            Stats = _sagipService.GetAnonymizedStats(),
            ActiveTab = tab,
            StatusMessage = TempData["SuccessMessage"] as string,
            ErrorMessage = TempData["ErrorMessage"] as string
        };

        ViewBag.AuditActor = auditActor ?? "All";
        ViewBag.AuditAction = auditAction ?? "All";
        ViewBag.AuditRecord = auditRecord ?? "All";

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AttemptCaseAccess(string caseNumber)
    {
        var admin = GetCurrentAdmin();
        if (admin == null) return Unauthorized();

        // Enforce RBAC Separation of Duties: Administrators cannot access victim cases
        _sagipService.RecordAudit("User", admin.UserId, admin.FullName, "AccessDenied", "Case", caseNumber, null, "RBAC Policy: Administrator cannot view confidential case details or victim PII (RA 10173 & RA 9262 compliance).");

        TempData["ErrorMessage"] = $"Access Denied: As an Administrator, your role is strictly separated from case handling. You cannot access case '{caseNumber}' or victim records under Philippine Data Privacy Act (RA 10173) and RA 9262 privacy regulations. This event has been recorded in the audit log.";
        return RedirectToAction(nameof(Index), new { tab = "audit" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateStaff(string fullName, string email, string role, string mfaMethod)
    {
        var admin = GetCurrentAdmin();
        if (admin == null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
        {
            TempData["ErrorMessage"] = "Full name and email are required.";
            return RedirectToAction(nameof(Index), new { tab = "staff" });
        }

        var (ok, setupLink, error) = _sagipService.CreateStaffAccount(fullName.Trim(), email.Trim(), role, mfaMethod, admin.FullName);
        if (ok)
        {
            TempData["SuccessMessage"] = $"Staff invitation created. Setup link: {setupLink}";
        }
        else
        {
            TempData["ErrorMessage"] = error ?? "Failed to create staff account.";
        }
        return RedirectToAction(nameof(Index), new { tab = "staff" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleStaffActive(string userId)
    {
        var admin = GetCurrentAdmin();
        if (admin == null) return Unauthorized();

        if (userId == admin.UserId)
        {
            TempData["ErrorMessage"] = "You cannot deactivate your own administrative account.";
            return RedirectToAction(nameof(Index), new { tab = "staff" });
        }

        if (_sagipService.ToggleStaffActive(userId, admin.FullName))
        {
            TempData["SuccessMessage"] = "Staff user status updated.";
        }
        else
        {
            TempData["ErrorMessage"] = "Staff user not found.";
        }
        return RedirectToAction(nameof(Index), new { tab = "staff" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RevokeStaffInvite(string userId)
    {
        var admin = GetCurrentAdmin();
        if (admin == null) return Unauthorized();

        if (_sagipService.RevokeStaffInvite(userId, admin.FullName))
        {
            TempData["SuccessMessage"] = "Staff invitation revoked.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not revoke invitation.";
        }
        return RedirectToAction(nameof(Index), new { tab = "staff" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult GenerateResetLink(string userId)
    {
        var admin = GetCurrentAdmin();
        if (admin == null) return Unauthorized();

        var (ok, link) = _sagipService.GenerateStaffResetLink(userId, admin.FullName);
        if (ok)
        {
            TempData["SuccessMessage"] = $"Password reset link generated: {link}";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to generate reset link.";
        }
        return RedirectToAction(nameof(Index), new { tab = "staff" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveContact(SupportContact contact)
    {
        var admin = GetCurrentAdmin();
        if (admin == null) return Unauthorized();

        _sagipService.SaveSupportContact(contact, admin.FullName);
        TempData["SuccessMessage"] = "Support contact updated successfully.";
        return RedirectToAction(nameof(Index), new { tab = "contacts" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteContact(string contactId)
    {
        var admin = GetCurrentAdmin();
        if (admin == null) return Unauthorized();

        _sagipService.DeleteSupportContact(contactId, admin.FullName);
        TempData["SuccessMessage"] = "Support contact removed.";
        return RedirectToAction(nameof(Index), new { tab = "contacts" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateSettings(SystemSettings settings)
    {
        var admin = GetCurrentAdmin();
        if (admin == null) return Unauthorized();

        _sagipService.UpdateSettings(settings, admin.FullName);
        TempData["SuccessMessage"] = "System settings updated successfully.";
        return RedirectToAction(nameof(Index), new { tab = "settings" });
    }
}
