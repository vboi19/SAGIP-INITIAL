using Microsoft.AspNetCore.Mvc;
using SAGIP.Models;
using SAGIP.Services;

namespace SAGIP.Controllers;

public class StaffController : Controller
{
    private readonly ISagipService _sagipService;
    private readonly I18nService _i18n;

    public StaffController(ISagipService sagipService, I18nService i18n)
    {
        _sagipService = sagipService;
        _i18n = i18n;
    }

    private void SetStaffSession(StaffUser user)
    {
        HttpContext.Session.SetString("StaffUserId", user.UserId);
        HttpContext.Session.SetString("StaffUserRole", user.Role);
        HttpContext.Session.SetString("StaffUserName", user.FullName);
        HttpContext.Session.SetString("StaffUserEmail", user.Email);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        // If already logged in, redirect to respective dashboard
        var role = HttpContext.Session.GetString("StaffUserRole");
        if (role == SagipConstants.RoleDeskOfficer)
        {
            return RedirectToAction("Index", "DeskOfficer");
        }
        else if (role == SagipConstants.RoleAdministrator)
        {
            return RedirectToAction("Index", "Admin");
        }

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.StaffUsers = _sagipService.GetStaffUsers();
        ViewBag.Settings = _sagipService.GetSettings();
        return View(new StaffLoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(StaffLoginViewModel model, string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.StaffUsers = _sagipService.GetStaffUsers();
        ViewBag.Settings = _sagipService.GetSettings();

        if (model.RequiresMfa)
        {
            if (string.IsNullOrWhiteSpace(model.MfaCode))
            {
                model.ErrorMessage = "Please enter the 6-digit verification code.";
                return View(model);
            }

            var (ok, error, user) = _sagipService.AuthenticateStep2(model.PendingUserId!, model.MfaCode);
            if (!ok || user == null)
            {
                model.ErrorMessage = error ?? "Invalid MFA verification code.";
                return View(model);
            }

            SetStaffSession(user);
            return RedirectAfterLogin(user.Role, returnUrl);
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            model.ErrorMessage = "Email address is required.";
            return View(model);
        }

        var (s1Ok, s1Error, s1User, requiresMfa) = _sagipService.AuthenticateStep1(model.Email, model.Password);
        if (!s1Ok || s1User == null)
        {
            model.ErrorMessage = s1Error ?? "Authentication failed.";
            return View(model);
        }

        model.RequiresMfa = true;
        model.PendingUserId = s1User.UserId;
        model.MfaMethod = s1User.MfaMethod;
        return View(model);
    }

    [HttpGet]
    [HttpPost]
    public IActionResult QuickLogin(string email, string? returnUrl)
    {
        var user = _sagipService.QuickLogin(email);
        if (user != null)
        {
            SetStaffSession(user);
            return RedirectAfterLogin(user.Role, returnUrl);
        }

        TempData["ErrorMessage"] = "Could not quick-login as this user.";
        return RedirectToAction(nameof(Login));
    }

    private IActionResult RedirectAfterLogin(string role, string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        if (role == SagipConstants.RoleAdministrator)
        {
            return RedirectToAction("Index", "Admin");
        }
        return RedirectToAction("Index", "DeskOfficer");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var userId = HttpContext.Session.GetString("StaffUserId");
        var userName = HttpContext.Session.GetString("StaffUserName");
        if (!string.IsNullOrEmpty(userId))
        {
            _sagipService.RecordAudit("User", userId, userName, "Logout", "Session", userId);
        }

        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ResetPassword(string? token)
    {
        ViewBag.Token = token;
        ViewBag.Settings = _sagipService.GetSettings();
        return View();
    }

    [HttpGet]
    public IActionResult Enroll(string? token)
    {
        ViewBag.Token = token;
        ViewBag.Settings = _sagipService.GetSettings();
        return View();
    }
}
