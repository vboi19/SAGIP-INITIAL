using SAGIP.Models;

namespace SAGIP.Services;

public interface ISagipService
{
    // Settings
    SystemSettings GetSettings();
    void UpdateSettings(SystemSettings settings, string updatedBy);

    // Support Contacts
    List<SupportContact> GetSupportContacts(bool activeOnly = false);
    SupportContact? GetSupportContact(string contactId);
    void SaveSupportContact(SupportContact contact, string updatedBy);
    void DeleteSupportContact(string contactId, string updatedBy);

    // Staff & Auth
    List<StaffUser> GetStaffUsers();
    StaffUser? GetStaffUserById(string userId);
    StaffUser? GetStaffUserByEmail(string email);
    (bool ok, string? error, StaffUser? user, bool requiresMfa) AuthenticateStep1(string email, string password);
    (bool ok, string? error, StaffUser? user) AuthenticateStep2(string userId, string code);
    StaffUser? QuickLogin(string email);
    (bool ok, string? setupLink, string? error) CreateStaffAccount(string fullName, string email, string role, string mfaMethod, string createdBy);
    bool ToggleStaffActive(string userId, string updatedBy);
    bool RevokeStaffInvite(string userId, string updatedBy);
    (bool ok, string? link) GenerateStaffResetLink(string userId, string updatedBy);

    // Reporting & Status
    ReportConfirmationViewModel SubmitReport(ReportFormModel form);
    StatusLookupViewModel LookupStatus(string referenceCode);
    (bool ok, DateTime? withdrawnAt, string? error) WithdrawConsent(string referenceCode);

    // Cases & Desk Officer Actions
    List<CaseRecord> GetCases(string? status = null, string? urgency = null, string? officerId = null, string? search = null);
    CaseRecord? GetCaseById(string caseId);
    ReportRecord? GetReportById(string reportId);
    List<ConsentRecord> GetConsentsByReportId(string reportId);
    List<CaseNote> GetCaseNotes(string caseId);
    List<CaseStatusHistoryEntry> GetStatusHistory(string caseId);
    List<CaseAlert> GetAlerts();

    bool UpdateCaseTriage(string caseId, string urgency, List<string> abuseTypes, string officerId, string officerName);
    bool SaveCaseAssessment(string caseId, string outcome, string pathway, string urgency, List<string> abuseTypes, string notes, string officerId, string officerName);
    bool RecordCaseReferral(string caseId, string outsideOffice, string referralDate, string purpose, string? reference, string? notes, string officerId, string officerName);
    bool RecordCaseBpo(string caseId, string bpoRef, string issuedBy, string dateIssued, string? effectiveUntil, string? notes, string officerId, string officerName);
    bool UpdateCaseStatus(string caseId, string newStatus, string? reason, string officerId, string officerName, string? assignOfficerId = null);
    bool AssignCase(string caseId, string? officerId, string currentOfficerId, string currentOfficerName);
    bool AddCaseNote(string caseId, string noteText, string authorId, string authorName);
    bool FlagConflict(string caseId, string officerId, string officerName);
    void RecordCaseView(string caseId, string officerId, string officerName);

    // Admin & Audit
    List<AuditLogEntry> GetAuditLogs(string? actorType = null, string? action = null, string? recordType = null);
    void RecordAudit(string actorType, string? userId, string? userName, string action, string recordType, string? recordId = null, string? oldValue = null, string? newValue = null);
    AnonymizedStatsViewModel GetAnonymizedStats();
}
