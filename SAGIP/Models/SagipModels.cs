namespace SAGIP.Models;

public static class SagipConstants
{
    public const string RoleDeskOfficer = "DeskOfficer";
    public const string RoleAdministrator = "Administrator";
}

public class IncidentLogEntry
{
    public string EntryId { get; set; } = string.Empty;
    public string EntryDate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Place { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AttachedLogSnapshot
{
    public string EntryId { get; set; } = string.Empty;
    public string EntryDate { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Place { get; set; }
}

public class ReportRecord
{
    public string ReportId { get; set; } = string.Empty;
    public string ReferenceCode { get; set; } = string.Empty;
    public string SubmissionToken { get; set; } = string.Empty;
    public string ReportType { get; set; } = "Anonymous"; // Anonymous | Identified
    public bool IsThirdParty { get; set; }
    public string VictimType { get; set; } = "Woman"; // Woman | Child | Other
    public bool ChildMotherAbused { get; set; }
    public bool IsRA9262Pathway { get; set; } = true;
    public string? IncidentDate { get; set; }
    public string IncidentDescription { get; set; } = string.Empty;
    public string? IncidentLocation { get; set; }
    public string? OffenderRelationship { get; set; }
    public string? ChildrenInvolved { get; set; } // Yes | No | Skip
    public string? ReporterName { get; set; }
    public string? ContactType { get; set; } // Email | Phone
    public string? ContactValue { get; set; }
    public string? SafeContactTime { get; set; }
    public string? VictimName { get; set; }
    public string? VictimContactType { get; set; } // Email | Phone
    public string? VictimContactValue { get; set; }
    public string? VictimSafeContactTime { get; set; }
    public string? RelationshipToVictim { get; set; }
    public List<AttachedLogSnapshot> AttachedLogEntries { get; set; } = new();
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}

public class ConsentRecord
{
    public string ConsentId { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
    public string ConsentType { get; set; } = "Contact"; // Contact | LogAttachment | VictimContact
    public bool Granted { get; set; } = true;
    public DateTime DecidedAt { get; set; } = DateTime.UtcNow;
    public DateTime? WithdrawnAt { get; set; }
    public string WordingVersion { get; set; } = "v1.0";
}

public class CaseNote
{
    public string NoteId { get; set; } = string.Empty;
    public string CaseId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string NoteText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CaseStatusHistoryEntry
{
    public string HistoryId { get; set; } = string.Empty;
    public string CaseId { get; set; } = string.Empty;
    public string? FromStatus { get; set; }
    public string ToStatus { get; set; } = "Received";
    public string? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }
}

public class CaseRecord
{
    public string CaseId { get; set; } = string.Empty;
    public string CaseNumber { get; set; } = string.Empty; // e.g., C-026-041
    public string ReportId { get; set; } = string.Empty;
    public string? AssignedOfficerId { get; set; }
    public string? Urgency { get; set; } // Low | Medium | High
    public List<string> AbuseTypes { get; set; } = new(); // Physical, Sexual, Psychological, Economic
    public string Status { get; set; } = "Received"; // Received | Under Assessment | Referred | BPO Issued | Closed
    public string? ClosureReason { get; set; }
    public string? AssessmentOutcome { get; set; }
    public string? EligibilityPathway { get; set; }
    public string? AssessmentNotes { get; set; }
    public string? ReferralOffice { get; set; }
    public string? ReferralDate { get; set; }
    public string? ReferralPurpose { get; set; }
    public string? ReferralReference { get; set; }
    public string? ReferralNotes { get; set; }
    public string? BpoReference { get; set; }
    public string? BpoIssuedBy { get; set; }
    public string? BpoDateIssued { get; set; }
    public string? BpoEffectiveUntil { get; set; }
    public string? BpoNotes { get; set; }
    public List<string> ConflictFlaggedByIds { get; set; } = new();
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
}

public class StaffUser
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "DeskOfficer"; // DeskOfficer | Administrator
    public string MfaMethod { get; set; } = "Authenticator"; // Authenticator | Email
    public string Status { get; set; } = "Active"; // Active | Pending | Inactive
    public int FailedLoginCount { get; set; }
    public DateTime? LockoutUntil { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? SetupToken { get; set; }
    public DateTime? SetupTokenExpiresAt { get; set; }
    public bool RequiresEnrollment { get; set; }
}

public class AuditLogEntry
{
    public string LogId { get; set; } = string.Empty;
    public string ActorType { get; set; } = "System"; // User | Reporter | System
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = "View"; // View, Create, Edit, Delete, StatusChange, Login, LoginFailed, Logout, PasswordReset, ConsentWithdrawn, SettingChange, AccessDenied
    public string RecordType { get; set; } = string.Empty;
    public string? RecordId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SupportContact
{
    public string ContactId { get; set; } = string.Empty;
    public string OfficeNameEn { get; set; } = string.Empty;
    public string OfficeNameFil { get; set; } = string.Empty;
    public string CategoryEn { get; set; } = string.Empty;
    public string CategoryFil { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string HoursEn { get; set; } = string.Empty;
    public string HoursFil { get; set; } = string.Empty;
    public string? AddressEn { get; set; }
    public string? AddressFil { get; set; }
    public string? GuidanceEn { get; set; }
    public string? GuidanceFil { get; set; }
    public string PublicationStatus { get; set; } = "Published"; // Published | Draft
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool ReceivesSagipReports { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string Name => !string.IsNullOrEmpty(OfficeNameEn) ? OfficeNameEn : OfficeNameFil;
    public string Category => !string.IsNullOrEmpty(CategoryEn) ? CategoryEn : CategoryFil;
    public string Address => !string.IsNullOrEmpty(AddressEn) ? AddressEn : (AddressFil ?? "");
    public string Email { get; set; } = "";
    public string OperatingHours => !string.IsNullOrEmpty(HoursEn) ? HoursEn : HoursFil;
}

public class SystemSettings
{
    public string BarangayName { get; set; } = "Barangay VAWC Desk";
    public string ReceivingOfficeName { get; set; } = "Barangay VAWC Desk";
    public string DpoName { get; set; } = "Data Protection Officer";
    public string DpoContact { get; set; } = "dpo@vawc.gov.ph · (02) 8555-0140";
    public string DeskContact { get; set; } = "(02) 8555-0100";
    public string DeskHours { get; set; } = "Mon–Fri, 8:00 AM – 5:00 PM";
    public string EmergencyNumbers { get; set; } = "911 · PNP WCPD (02) 8555-0101 · DSWD 1343";
    public string QuickExitUrl { get; set; } = "https://www.pagasa.dost.gov.ph";
    public string SafeExitUrl { get => QuickExitUrl; set => QuickExitUrl = value; }
    public string ServiceArea { get; set; } = "Barangay Service Area";
    public int SessionTimeoutMinutes { get; set; } = 15;
    public int AlertThresholdHours { get; set; } = 24;
    public int RetentionYears { get; set; } = 5;
    public int LockoutThreshold { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 15;
    public bool AllowThirdPartyReports { get; set; } = true;
    public bool RequireContactConsent { get; set; } = true;
    public bool SmallCountSuppression { get; set; } = true;
}

public class CaseAlert
{
    public string CaseId { get; set; } = string.Empty;
    public string CaseNumber { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
