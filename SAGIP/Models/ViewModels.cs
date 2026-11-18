namespace SAGIP.Models;

public class ReportFormModel
{
    // Step 1: Safety & Screening
    public string? Honeypot { get; set; }
    public string SubmissionToken { get; set; } = Guid.NewGuid().ToString("N");
    public string VictimType { get; set; } = "Woman"; // Woman | Child | Other
    public bool ChildMotherAbused { get; set; }
    public bool WithinBarangay { get; set; } = true;
    public bool IsThirdParty { get; set; }
    public string? RelationshipToVictim { get; set; }

    // Step 2: Incident Details
    public string? IncidentDate { get; set; }
    public string? IncidentLocation { get; set; }
    public string IncidentDescription { get; set; } = string.Empty;
    public string? OffenderRelationship { get; set; }
    public string? ChildrenInvolved { get; set; } = "No"; // Yes | No | Skip

    // Step 3: Identity & Contact
    public string ReportType { get; set; } = "Anonymous"; // Anonymous | Identified
    public string? ReporterName { get; set; }
    public string? ContactType { get; set; } = "Email"; // Email | Phone
    public string? ContactValue { get; set; }
    public string? SafeContactTime { get; set; }
    public bool ContactConsent { get; set; }

    // Third-party victim contact
    public string? VictimName { get; set; }
    public string? VictimContactType { get; set; } = "Phone";
    public string? VictimContactValue { get; set; }
    public string? VictimSafeContactTime { get; set; }
    public bool VictimContactConsent { get; set; }

    // Attached notebook logs (JSON string or list)
    public string? AttachedLogsJson { get; set; }
    public bool LogAttachmentConsent { get; set; }
}

public class ReportConfirmationViewModel
{
    public bool Success { get; set; }
    public string? ReferenceCode { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsRA9262Pathway { get; set; } = true;
    public string ReceivingOffice { get; set; } = "Barangay VAWC Desk";
    public string ReportType { get; set; } = "Anonymous";
}

public class StatusLookupViewModel
{
    public string ReferenceCode { get; set; } = string.Empty;
    public bool Searched { get; set; }
    public bool Found { get; set; }
    public string? Status { get; set; }
    public string? ReceivingOffice { get; set; }
    public bool CanWithdrawConsent { get; set; }
    public DateTime? ConsentWithdrawnAt { get; set; }
    public bool IsIdentified { get; set; }
    public string? ContactType { get; set; }
    public string? Message { get; set; }
}

public class StaffLoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? MfaCode { get; set; }
    public bool RequiresMfa { get; set; }
    public string? MfaMethod { get; set; }
    public string? ErrorMessage { get; set; }
    public string? PendingUserId { get; set; }
}

public class DeskOfficerDashboardViewModel
{
    public StaffUser CurrentUser { get; set; } = null!;
    public SystemSettings Settings { get; set; } = null!;
    public List<CaseRecord> Cases { get; set; } = new();
    public List<ReportRecord> Reports { get; set; } = new();
    public List<StaffUser> Officers { get; set; } = new();
    public List<StaffUser> StaffUsers => Officers;
    public List<SupportContact> SupportContacts { get; set; } = new();
    public List<CaseAlert> Alerts { get; set; } = new();

    // Filters
    public string? StatusFilter { get; set; }
    public string? UrgencyFilter { get; set; }
    public string? OfficerFilter { get; set; }
    public string? SearchQuery { get; set; }

    // Stats
    public int TotalCases => Cases.Count;
    public int UnderAssessmentCount => Cases.Count(c => c.Status == "Under Assessment");
    public int BpoIssuedCount => Cases.Count(c => c.Status == "BPO Issued");
    public int UnassignedCount => Cases.Count(c => string.IsNullOrEmpty(c.AssignedOfficerId));
}

public class CaseDetailViewModel
{
    public StaffUser CurrentUser { get; set; } = null!;
    public CaseRecord Case { get; set; } = null!;
    public ReportRecord Report { get; set; } = null!;
    public List<ConsentRecord> Consents { get; set; } = new();
    public List<CaseNote> Notes { get; set; } = new();
    public List<CaseStatusHistoryEntry> StatusHistory { get; set; } = new();
    public List<StaffUser> AvailableOfficers { get; set; } = new();
    public List<SupportContact> SupportContacts { get; set; } = new();
    public SystemSettings Settings { get; set; } = null!;
    public bool IsConflictFlaggedByCurrent { get; set; }
}

public class AdminDashboardViewModel
{
    public StaffUser CurrentUser { get; set; } = null!;
    public SystemSettings Settings { get; set; } = null!;
    public List<StaffUser> StaffUsers { get; set; } = new();
    public List<SupportContact> SupportContacts { get; set; } = new();
    public List<AuditLogEntry> AuditLogs { get; set; } = new();
    public AnonymizedStatsViewModel Stats { get; set; } = new();
    public string ActiveTab { get; set; } = "stats"; // stats | staff | contacts | settings | audit
    public string? StatusMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AnonymizedStatsViewModel
{
    public int TotalCases { get; set; }
    public int OpenCases { get; set; }
    public int ClosedCases { get; set; }
    public int BpoIssuedCount { get; set; }
    public int ReferredCount { get; set; }
    public Dictionary<string, int> CasesByAbuseType { get; set; } = new();
    public Dictionary<string, int> CasesByStatus { get; set; } = new();
    public Dictionary<string, int> CasesByUrgency { get; set; } = new();
    public Dictionary<string, int> CasesByVictimType { get; set; } = new();
    public int AnonymousCount { get; set; }
    public int IdentifiedCount { get; set; }
}
