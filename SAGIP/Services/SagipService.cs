using System.Security.Cryptography;
using System.Text.Json;
using SAGIP.Models;

namespace SAGIP.Services;

public class SagipService : ISagipService
{
    private readonly object _lock = new();

    private SystemSettings _settings = new();
    private readonly List<SupportContact> _contacts = new();
    private readonly List<StaffUser> _staffUsers = new();
    private readonly List<ReportRecord> _reports = new();
    private readonly List<ConsentRecord> _consents = new();
    private readonly List<CaseRecord> _cases = new();
    private readonly List<CaseNote> _notes = new();
    private readonly List<CaseStatusHistoryEntry> _statusHistory = new();
    private readonly List<AuditLogEntry> _auditLogs = new();

    private static DateTime HoursAgo(double hours) => DateTime.UtcNow.AddHours(-hours);

    public SagipService()
    {
        InitializeSeedData();
    }

    private void InitializeSeedData()
    {
        lock (_lock)
        {
            _settings = new SystemSettings
            {
                BarangayName = "Barangay VAWC Desk",
                ReceivingOfficeName = "Barangay VAWC Desk",
                DpoName = "Data Protection Officer",
                DpoContact = "dpo@vawc.gov.ph · (02) 8555-0140",
                DeskContact = "(02) 8555-0100",
                DeskHours = "Mon–Fri, 8:00 AM – 5:00 PM",
                EmergencyNumbers = "911 · PNP WCPD (02) 8555-0101 · DSWD 1343",
                QuickExitUrl = "https://www.pagasa.dost.gov.ph",
                SessionTimeoutMinutes = 15,
                AlertThresholdHours = 24,
                RetentionYears = 5,
                LockoutThreshold = 5,
                LockoutDurationMinutes = 15,
                AllowThirdPartyReports = true,
                RequireContactConsent = true,
                SmallCountSuppression = true
            };

            _contacts.AddRange(new[]
            {
                new SupportContact
                {
                    ContactId = "sc-1",
                    OfficeNameEn = "Barangay VAWC Desk",
                    OfficeNameFil = "Tanggapan ng VAWC ng Barangay",
                    CategoryEn = "Receives SAGIP reports",
                    CategoryFil = "Tumatanggap ng ulat ng SAGIP",
                    Phone = "(02) 8555-0100",
                    HoursEn = "Mon–Fri, 8:00 AM – 5:00 PM",
                    HoursFil = "Lun–Biy, 8:00 AM – 5:00 PM",
                    AddressEn = "2/F Barangay Hall, Mabini St.",
                    AddressFil = "2/F Barangay Hall, Mabini St.",
                    GuidanceEn = "Walk-ins welcome during office hours. Bring a valid ID if applying for a Barangay Protection Order.",
                    GuidanceFil = "Maaaring pumunta sa oras ng opisina. Magdala ng valid ID kung hihingi ng Barangay Protection Order.",
                    PublicationStatus = "Published",
                    DisplayOrder = 1,
                    IsActive = true,
                    ReceivesSagipReports = true,
                    UpdatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new SupportContact
                {
                    ContactId = "sc-2",
                    OfficeNameEn = "Women and Children Protection Desk",
                    OfficeNameFil = "Himpilan ng Pulisya · Women and Children Protection Desk",
                    CategoryEn = "Support office · Police assistance",
                    CategoryFil = "Katuwang na tanggapan · Pulisya",
                    Phone = "(02) 8555-0101",
                    HoursEn = "Open 24 hours",
                    HoursFil = "Bukas 24 oras",
                    AddressEn = "Municipal Police Station, Rizal Ave.",
                    AddressFil = "Himpilan ng Pulisya, Rizal Ave.",
                    GuidanceEn = "Call 911 or visit the WCPD room for immediate police blotter, rescue, or criminal complaint intake.",
                    GuidanceFil = "Tumawag sa 911 o magtungo sa WCPD para sa agarang tulong ng pulisya.",
                    PublicationStatus = "Published",
                    DisplayOrder = 2,
                    IsActive = true,
                    UpdatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new SupportContact
                {
                    ContactId = "sc-3",
                    OfficeNameEn = "Social Welfare Office (C/MSWDO)",
                    OfficeNameFil = "Tanggapan ng Kagalingang Panlipunan",
                    CategoryEn = "Support office · Shelter & counseling",
                    CategoryFil = "Katuwang na tanggapan · Kanlungan at payo",
                    Phone = "(02) 8555-0102",
                    HoursEn = "Mon–Fri, 8:00 AM – 5:00 PM",
                    HoursFil = "Lun–Biy, 8:00 AM – 5:00 PM",
                    AddressEn = "Ground Floor, Municipal Annex Building",
                    AddressFil = "Ground Floor, Municipal Annex Building",
                    GuidanceEn = "Provides psychosocial counseling, temporary shelter referral, and emergency financial assistance.",
                    GuidanceFil = "Nagbibigay ng pagpapayo, pansamantalang kanlungan, at tulong pinansyal.",
                    PublicationStatus = "Published",
                    DisplayOrder = 3,
                    IsActive = true,
                    UpdatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new SupportContact
                {
                    ContactId = "sc-4",
                    OfficeNameEn = "Health Services · Rural Health Unit",
                    OfficeNameFil = "Serbisyong Pangkalusugan · Rural Health Unit",
                    CategoryEn = "Support office · Medical care",
                    CategoryFil = "Katuwang na tanggapan · Medikal na tulong",
                    Phone = "(02) 8555-0103",
                    HoursEn = "Mon–Sat, 8:00 AM – 4:00 PM",
                    HoursFil = "Lun–Sab, 8:00 AM – 4:00 PM",
                    AddressEn = "Barangay Health Center, Acacia Lane",
                    AddressFil = "Barangay Health Center, Acacia Lane",
                    GuidanceEn = "Confidential physical examination, medico-legal documentation, and primary medical care.",
                    GuidanceFil = "Kumpidensyal na pagsusuring medikal at dokumentasyong medico-legal.",
                    PublicationStatus = "Published",
                    DisplayOrder = 4,
                    IsActive = true,
                    UpdatedAt = DateTime.UtcNow.AddDays(-15)
                }
            });

            _staffUsers.AddRange(new[]
            {
                new StaffUser
                {
                    UserId = "u-maria",
                    FullName = "Maria Santos",
                    Email = "maria.santos@barangay.gov.ph",
                    Role = SagipConstants.RoleDeskOfficer,
                    MfaMethod = "Authenticator",
                    Status = "Active",
                    FailedLoginCount = 0,
                    LastLoginAt = HoursAgo(7),
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new StaffUser
                {
                    UserId = "u-ana",
                    FullName = "Ana Reyes",
                    Email = "ana.reyes@barangay.gov.ph",
                    Role = SagipConstants.RoleDeskOfficer,
                    MfaMethod = "Email",
                    Status = "Active",
                    FailedLoginCount = 0,
                    LastLoginAt = HoursAgo(24),
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new StaffUser
                {
                    UserId = "u-elena",
                    FullName = "Elena Cruz",
                    Email = "elena.cruz@barangay.gov.ph",
                    Role = SagipConstants.RoleAdministrator,
                    MfaMethod = "Authenticator",
                    Status = "Active",
                    FailedLoginCount = 0,
                    LastLoginAt = HoursAgo(4),
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new StaffUser
                {
                    UserId = "u-josefa",
                    FullName = "Josefa Lim",
                    Email = "josefa.lim@barangay.gov.ph",
                    Role = SagipConstants.RoleDeskOfficer,
                    MfaMethod = "Authenticator",
                    Status = "Pending",
                    FailedLoginCount = 0,
                    SetupToken = "SAGIP-INVITE-0912",
                    SetupTokenExpiresAt = DateTime.UtcNow.AddDays(30),
                    RequiresEnrollment = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                }
            });

            _reports.AddRange(new[]
            {
                new ReportRecord
                {
                    ReportId = "rep-041",
                    ReferenceCode = "A9BC-71KL-22MN",
                    SubmissionToken = "tok-041",
                    ReportType = "Anonymous",
                    IsThirdParty = false,
                    VictimType = "Woman",
                    IsRA9262Pathway = true,
                    IncidentDate = "2026-11-13",
                    IncidentLocation = "Purok 3, Sampaguita St.",
                    IncidentDescription = "Physical confrontation and threats made inside the residence after an argument regarding household expenses. Reporter sought temporary shelter with a relative.",
                    OffenderRelationship = "Current spouse or partner",
                    ChildrenInvolved = "Yes",
                    SubmittedAt = HoursAgo(26)
                },
                new ReportRecord
                {
                    ReportId = "rep-042",
                    ReferenceCode = "M4PQ-88ZT-51VX",
                    SubmissionToken = "tok-042",
                    ReportType = "Identified",
                    IsThirdParty = false,
                    VictimType = "Woman",
                    IsRA9262Pathway = true,
                    IncidentDate = "2026-11-14",
                    IncidentLocation = "Sitio Maligaya",
                    IncidentDescription = "Repeated verbal threats and withholding of daily allowances over the past three weeks. Requesting guidance on securing a Barangay Protection Order.",
                    OffenderRelationship = "Current spouse or partner",
                    ChildrenInvolved = "No",
                    ReporterName = "R. Villanueva",
                    ContactType = "Email",
                    ContactValue = "r.villanueva@example.com",
                    SafeContactTime = "Weekdays 9:00 AM – 12:00 PM",
                    SubmittedAt = HoursAgo(25)
                },
                new ReportRecord
                {
                    ReportId = "rep-043",
                    ReferenceCode = "D7TR-39WN-60HJ",
                    SubmissionToken = "tok-043",
                    ReportType = "Anonymous",
                    IsThirdParty = false,
                    VictimType = "Child",
                    ChildMotherAbused = true,
                    IsRA9262Pathway = true,
                    IncidentDate = "2026-11-14",
                    IncidentLocation = "Near Mabini Elementary",
                    IncidentDescription = "Child and mother experienced escalating intimidation at home when father returned intoxicated late at night.",
                    OffenderRelationship = "Former spouse or partner",
                    ChildrenInvolved = "Yes",
                    SubmittedAt = HoursAgo(24.5)
                },
                new ReportRecord
                {
                    ReportId = "rep-044",
                    ReferenceCode = "K7QM-4XD2-9WTR",
                    SubmissionToken = "tok-044",
                    ReportType = "Identified",
                    IsThirdParty = false,
                    VictimType = "Woman",
                    IsRA9262Pathway = true,
                    IncidentDate = "2026-11-15",
                    IncidentLocation = "Block 4, Acacia Lane",
                    IncidentDescription = "Sustained psychological intimidation and phone monitoring. Partner has threatened to take away access to the family home.",
                    OffenderRelationship = "Current spouse or partner",
                    ChildrenInvolved = "Yes",
                    ReporterName = "L. Bautista",
                    ContactType = "Email",
                    ContactValue = "l.bautista@example.com",
                    SafeContactTime = "Weekdays after 6:00 PM",
                    SubmittedAt = HoursAgo(6)
                },
                new ReportRecord
                {
                    ReportId = "rep-045",
                    ReferenceCode = "P2YU-64KL-93BC",
                    SubmissionToken = "tok-045",
                    ReportType = "Identified",
                    IsThirdParty = true,
                    VictimType = "Other",
                    IsRA9262Pathway = false,
                    IncidentDate = "2026-11-14",
                    IncidentLocation = "Zone 2",
                    IncidentDescription = "Elderly household member experiencing financial control and neglect by an adult relative living in the same compound.",
                    OffenderRelationship = "Other family member",
                    ChildrenInvolved = "No",
                    ReporterName = "C. Navarro",
                    ContactType = "Phone",
                    ContactValue = "0917 555 0144",
                    SafeContactTime = "Mornings before 11:00 AM",
                    RelationshipToVictim = "Neighbor",
                    VictimName = "T. Navarro",
                    VictimContactType = "Phone",
                    VictimContactValue = "0917 555 0199",
                    VictimSafeContactTime = "Weekdays 1:00 PM – 3:00 PM",
                    SubmittedAt = HoursAgo(18)
                },
                new ReportRecord
                {
                    ReportId = "rep-046",
                    ReferenceCode = "W8NM-12FG-44RS",
                    SubmissionToken = "tok-046",
                    ReportType = "Anonymous",
                    IsThirdParty = false,
                    VictimType = "Woman",
                    IsRA9262Pathway = true,
                    IncidentDate = "2026-11-15",
                    IncidentLocation = "Purok 1",
                    IncidentDescription = "Partner forcefully took phone and withheld salary card following a dispute.",
                    OffenderRelationship = "Dating or intimate partner",
                    ChildrenInvolved = "No",
                    SubmittedAt = HoursAgo(1)
                },
                new ReportRecord
                {
                    ReportId = "rep-047",
                    ReferenceCode = "H3JK-55PL-88XZ",
                    SubmissionToken = "tok-047",
                    ReportType = "Identified",
                    IsThirdParty = false,
                    VictimType = "Woman",
                    IsRA9262Pathway = true,
                    IncidentDate = "2026-11-08",
                    IncidentLocation = "Purok 5",
                    IncidentDescription = "Follow-up report regarding compliance with Barangay Protection Order issued earlier this month.",
                    OffenderRelationship = "Former spouse or partner",
                    ChildrenInvolved = "Yes",
                    ReporterName = "J. Morales",
                    ContactType = "Phone",
                    ContactValue = "0918 555 0162",
                    SafeContactTime = "Afternoons 2:00 PM – 4:00 PM",
                    SubmittedAt = HoursAgo(140)
                },
                new ReportRecord
                {
                    ReportId = "rep-048",
                    ReferenceCode = "Z5VB-77NM-19QW",
                    SubmissionToken = "tok-048",
                    ReportType = "Identified",
                    IsThirdParty = false,
                    VictimType = "Woman",
                    IsRA9262Pathway = true,
                    IncidentDate = "2026-11-05",
                    IncidentLocation = "Purok 2",
                    IncidentDescription = "Resolved mediation and safety check completed; victim relocated safely with family support.",
                    OffenderRelationship = "Current spouse or partner",
                    ChildrenInvolved = "No",
                    ReporterName = "N. Mendoza",
                    ContactType = "Email",
                    ContactValue = "n.mendoza@example.com",
                    SafeContactTime = "Anytime weekdays",
                    SubmittedAt = HoursAgo(210)
                }
            });

            _consents.AddRange(new[]
            {
                new ConsentRecord
                {
                    ConsentId = "con-042",
                    ReportId = "rep-042",
                    ConsentType = "Contact",
                    Granted = true,
                    DecidedAt = HoursAgo(25)
                },
                new ConsentRecord
                {
                    ConsentId = "con-044",
                    ReportId = "rep-044",
                    ConsentType = "Contact",
                    Granted = true,
                    DecidedAt = HoursAgo(6)
                },
                new ConsentRecord
                {
                    ConsentId = "con-045-a",
                    ReportId = "rep-045",
                    ConsentType = "Contact",
                    Granted = true,
                    DecidedAt = HoursAgo(18)
                },
                new ConsentRecord
                {
                    ConsentId = "con-045-b",
                    ReportId = "rep-045",
                    ConsentType = "VictimContact",
                    Granted = true,
                    DecidedAt = HoursAgo(18)
                },
                new ConsentRecord
                {
                    ConsentId = "con-047",
                    ReportId = "rep-047",
                    ConsentType = "Contact",
                    Granted = true,
                    DecidedAt = HoursAgo(140)
                },
                new ConsentRecord
                {
                    ConsentId = "con-048",
                    ReportId = "rep-048",
                    ConsentType = "Contact",
                    Granted = true,
                    DecidedAt = HoursAgo(210)
                }
            });

            _cases.AddRange(new[]
            {
                new CaseRecord
                {
                    CaseId = "case-041",
                    CaseNumber = "C-026-041",
                    ReportId = "rep-041",
                    AssignedOfficerId = null,
                    Urgency = "High",
                    AbuseTypes = new List<string> { "Physical", "Psychological" },
                    Status = "Under Assessment",
                    ConflictFlaggedByIds = new List<string>(),
                    LastActivityAt = HoursAgo(26),
                    CreatedAt = HoursAgo(26)
                },
                new CaseRecord
                {
                    CaseId = "case-042",
                    CaseNumber = "C-026-042",
                    ReportId = "rep-042",
                    AssignedOfficerId = null,
                    Urgency = null,
                    AbuseTypes = new List<string>(),
                    Status = "Received",
                    ConflictFlaggedByIds = new List<string>(),
                    LastActivityAt = HoursAgo(25),
                    CreatedAt = HoursAgo(25)
                },
                new CaseRecord
                {
                    CaseId = "case-043",
                    CaseNumber = "C-026-043",
                    ReportId = "rep-043",
                    AssignedOfficerId = null,
                    Urgency = null,
                    AbuseTypes = new List<string>(),
                    Status = "Received",
                    ConflictFlaggedByIds = new List<string>(),
                    LastActivityAt = HoursAgo(24.5),
                    CreatedAt = HoursAgo(24.5)
                },
                new CaseRecord
                {
                    CaseId = "case-044",
                    CaseNumber = "C-026-044",
                    ReportId = "rep-044",
                    AssignedOfficerId = "u-maria",
                    Urgency = "Medium",
                    AbuseTypes = new List<string> { "Psychological" },
                    Status = "Under Assessment",
                    ConflictFlaggedByIds = new List<string>(),
                    LastActivityAt = HoursAgo(2),
                    CreatedAt = HoursAgo(6)
                },
                new CaseRecord
                {
                    CaseId = "case-045",
                    CaseNumber = "C-026-045",
                    ReportId = "rep-045",
                    AssignedOfficerId = "u-ana",
                    Urgency = "Low",
                    AbuseTypes = new List<string> { "Economic" },
                    Status = "Referred",
                    ReferralOffice = "Social Welfare Office (C/MSWDO)",
                    ReferralDate = "2026-11-15",
                    ReferralPurpose = "Elder financial protection & family mediation",
                    ReferralReference = "MSWDO-2026-084",
                    ConflictFlaggedByIds = new List<string>(),
                    LastActivityAt = HoursAgo(3),
                    CreatedAt = HoursAgo(18)
                },
                new CaseRecord
                {
                    CaseId = "case-046",
                    CaseNumber = "C-026-046",
                    ReportId = "rep-046",
                    AssignedOfficerId = "u-maria",
                    Urgency = null,
                    AbuseTypes = new List<string>(),
                    Status = "Received",
                    ConflictFlaggedByIds = new List<string>(),
                    LastActivityAt = HoursAgo(0.75),
                    CreatedAt = HoursAgo(1)
                },
                new CaseRecord
                {
                    CaseId = "case-047",
                    CaseNumber = "C-026-047",
                    ReportId = "rep-047",
                    AssignedOfficerId = "u-maria",
                    Urgency = "High",
                    AbuseTypes = new List<string> { "Physical" },
                    Status = "BPO Issued",
                    BpoReference = "BPO 2026-018",
                    BpoIssuedBy = "Punong Barangay",
                    BpoDateIssued = "2026-11-09",
                    BpoEffectiveUntil = "2026-11-24",
                    ConflictFlaggedByIds = new List<string>(),
                    LastActivityAt = HoursAgo(12),
                    CreatedAt = HoursAgo(140)
                },
                new CaseRecord
                {
                    CaseId = "case-048",
                    CaseNumber = "C-026-048",
                    ReportId = "rep-048",
                    AssignedOfficerId = "u-ana",
                    Urgency = "Medium",
                    AbuseTypes = new List<string> { "Psychological" },
                    Status = "Closed",
                    ClosureReason = "Resolved & Safe",
                    ConflictFlaggedByIds = new List<string>(),
                    LastActivityAt = HoursAgo(48),
                    CreatedAt = HoursAgo(210),
                    ClosedAt = HoursAgo(48)
                }
            });

            _notes.AddRange(new[]
            {
                new CaseNote
                {
                    NoteId = "note-1",
                    CaseId = "case-044",
                    AuthorId = "u-maria",
                    AuthorName = "Maria Santos",
                    NoteText = "Reviewed report narrative. Scheduled outreach call during reporter safe time (weekdays after 6:00 PM) to discuss BPO eligibility.",
                    CreatedAt = HoursAgo(2)
                },
                new CaseNote
                {
                    NoteId = "note-2",
                    CaseId = "case-045",
                    AuthorId = "u-ana",
                    AuthorName = "Ana Reyes",
                    NoteText = "Non-RA 9262 case recorded. Referred victim to Social Welfare Office (CSWDO) for elder assistance and family mediation outside SAGIP.",
                    CreatedAt = HoursAgo(3)
                },
                new CaseNote
                {
                    NoteId = "note-3",
                    CaseId = "case-047",
                    AuthorId = "u-maria",
                    AuthorName = "Maria Santos",
                    NoteText = "Punong Barangay issued Barangay Protection Order (BPO #2026-118). Status updated to BPO Issued.",
                    CreatedAt = HoursAgo(12)
                }
            });

            _statusHistory.AddRange(new[]
            {
                new CaseStatusHistoryEntry
                {
                    HistoryId = "hist-044-1",
                    CaseId = "case-044",
                    ToStatus = "Received",
                    ChangedAt = HoursAgo(6)
                },
                new CaseStatusHistoryEntry
                {
                    HistoryId = "hist-044-2",
                    CaseId = "case-044",
                    FromStatus = "Received",
                    ToStatus = "Under Assessment",
                    ChangedBy = "u-maria",
                    ChangedByName = "Maria Santos",
                    ChangedAt = HoursAgo(2)
                },
                new CaseStatusHistoryEntry
                {
                    HistoryId = "hist-045-1",
                    CaseId = "case-045",
                    ToStatus = "Received",
                    ChangedAt = HoursAgo(18)
                },
                new CaseStatusHistoryEntry
                {
                    HistoryId = "hist-045-2",
                    CaseId = "case-045",
                    FromStatus = "Received",
                    ToStatus = "Under Assessment",
                    ChangedBy = "u-ana",
                    ChangedByName = "Ana Reyes",
                    ChangedAt = HoursAgo(10)
                },
                new CaseStatusHistoryEntry
                {
                    HistoryId = "hist-045-3",
                    CaseId = "case-045",
                    FromStatus = "Under Assessment",
                    ToStatus = "Referred",
                    ChangedBy = "u-ana",
                    ChangedByName = "Ana Reyes",
                    ChangedAt = HoursAgo(3)
                }
            });

            _auditLogs.AddRange(new[]
            {
                new AuditLogEntry
                {
                    LogId = "aud-1",
                    ActorType = "Reporter",
                    Action = "Create",
                    RecordType = "Report",
                    RecordId = "C-026-046",
                    CreatedAt = HoursAgo(1)
                },
                new AuditLogEntry
                {
                    LogId = "aud-2",
                    ActorType = "User",
                    UserId = "u-maria",
                    UserName = "Maria Santos",
                    Action = "StatusChange",
                    RecordType = "Case",
                    RecordId = "C-026-044",
                    OldValue = "Received",
                    NewValue = "Under Assessment",
                    CreatedAt = HoursAgo(2)
                },
                new AuditLogEntry
                {
                    LogId = "aud-3",
                    ActorType = "User",
                    UserId = "u-ana",
                    UserName = "Ana Reyes",
                    Action = "StatusChange",
                    RecordType = "Case",
                    RecordId = "C-026-045",
                    OldValue = "Under Assessment",
                    NewValue = "Referred",
                    CreatedAt = HoursAgo(3)
                },
                new AuditLogEntry
                {
                    LogId = "aud-4",
                    ActorType = "User",
                    UserId = "u-elena",
                    UserName = "Elena Cruz",
                    Action = "Login",
                    RecordType = "Session",
                    RecordId = "u-elena",
                    CreatedAt = HoursAgo(4)
                }
            });
        }
    }

    public SystemSettings GetSettings()
    {
        lock (_lock)
        {
            return _settings;
        }
    }

    public void UpdateSettings(SystemSettings settings, string updatedBy)
    {
        lock (_lock)
        {
            _settings = settings;
            RecordAudit("User", updatedBy, updatedBy, "SettingChange", "SystemSettings", null, null, "Settings updated");
        }
    }

    public List<SupportContact> GetSupportContacts(bool activeOnly = false)
    {
        lock (_lock)
        {
            return _contacts
                .Where(c => !activeOnly || c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
        }
    }

    public SupportContact? GetSupportContact(string contactId)
    {
        lock (_lock)
        {
            return _contacts.FirstOrDefault(c => c.ContactId == contactId);
        }
    }

    public void SaveSupportContact(SupportContact contact, string updatedBy)
    {
        lock (_lock)
        {
            var existing = _contacts.FirstOrDefault(c => c.ContactId == contact.ContactId);
            contact.UpdatedAt = DateTime.UtcNow;
            if (existing != null)
            {
                var idx = _contacts.IndexOf(existing);
                _contacts[idx] = contact;
                RecordAudit("User", updatedBy, updatedBy, "Edit", "SupportContact", contact.ContactId);
            }
            else
            {
                if (string.IsNullOrEmpty(contact.ContactId))
                {
                    contact.ContactId = "sc-" + Guid.NewGuid().ToString("N")[..8];
                }
                _contacts.Add(contact);
                RecordAudit("User", updatedBy, updatedBy, "Create", "SupportContact", contact.ContactId);
            }
        }
    }

    public void DeleteSupportContact(string contactId, string updatedBy)
    {
        lock (_lock)
        {
            var item = _contacts.FirstOrDefault(c => c.ContactId == contactId);
            if (item != null)
            {
                _contacts.Remove(item);
                RecordAudit("User", updatedBy, updatedBy, "Delete", "SupportContact", contactId);
            }
        }
    }

    public List<StaffUser> GetStaffUsers()
    {
        lock (_lock)
        {
            return _staffUsers.OrderBy(u => u.FullName).ToList();
        }
    }

    public StaffUser? GetStaffUserById(string userId)
    {
        lock (_lock)
        {
            return _staffUsers.FirstOrDefault(u => u.UserId == userId);
        }
    }

    public StaffUser? GetStaffUserByEmail(string email)
    {
        lock (_lock)
        {
            return _staffUsers.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }
    }

    public (bool ok, string? error, StaffUser? user, bool requiresMfa) AuthenticateStep1(string email, string password)
    {
        lock (_lock)
        {
            var user = GetStaffUserByEmail(email);
            if (user == null)
            {
                RecordAudit("User", null, email, "LoginFailed", "Session", null, null, "User not found");
                return (false, "Invalid credentials.", null, false);
            }

            if (user.Status != "Active")
            {
                return (false, "This account is not active. Please contact the administrator.", null, false);
            }

            if (user.LockoutUntil.HasValue && user.LockoutUntil > DateTime.UtcNow)
            {
                return (false, $"Account is temporarily locked until {user.LockoutUntil.Value:HH:mm UTC}.", null, false);
            }

            // For demo purposes, any non-empty password is accepted for existing users
            if (string.IsNullOrWhiteSpace(password))
            {
                user.FailedLoginCount++;
                if (user.FailedLoginCount >= _settings.LockoutThreshold)
                {
                    user.LockoutUntil = DateTime.UtcNow.AddMinutes(_settings.LockoutDurationMinutes);
                    RecordAudit("User", user.UserId, user.FullName, "LoginFailed", "Session", user.UserId, null, "Account locked");
                    return (false, $"Account locked due to {_settings.LockoutThreshold} failed attempts.", null, false);
                }
                return (false, "Password is required.", null, false);
            }

            // Step 1 success - proceed to MFA
            return (true, null, user, true);
        }
    }

    public (bool ok, string? error, StaffUser? user) AuthenticateStep2(string userId, string code)
    {
        lock (_lock)
        {
            var user = GetStaffUserById(userId);
            if (user == null) return (false, "User session not found.", null);

            // Accept standard demo codes: "123456", "000000", or 6 digits
            if (string.IsNullOrWhiteSpace(code) || code.Trim().Length != 6)
            {
                RecordAudit("User", user.UserId, user.FullName, "LoginFailed", "Session", user.UserId, null, "Invalid MFA code");
                return (false, "Please enter a valid 6-digit verification code.", null);
            }

            user.FailedLoginCount = 0;
            user.LockoutUntil = null;
            user.LastLoginAt = DateTime.UtcNow;

            RecordAudit("User", user.UserId, user.FullName, "Login", "Session", user.UserId);
            return (true, null, user);
        }
    }

    public StaffUser? QuickLogin(string email)
    {
        lock (_lock)
        {
            var user = GetStaffUserByEmail(email);
            if (user != null && user.Status == "Active")
            {
                user.FailedLoginCount = 0;
                user.LastLoginAt = DateTime.UtcNow;
                RecordAudit("User", user.UserId, user.FullName, "Login", "Session", user.UserId, null, "Demo quick login");
                return user;
            }
            return null;
        }
    }

    public (bool ok, string? setupLink, string? error) CreateStaffAccount(string fullName, string email, string role, string mfaMethod, string createdBy)
    {
        lock (_lock)
        {
            if (GetStaffUserByEmail(email) != null)
            {
                return (false, null, "An account with this email already exists.");
            }

            var token = "SAGIP-INVITE-" + RandomNumberGenerator.GetInt32(1000, 9999);
            var newUser = new StaffUser
            {
                UserId = "u-" + Guid.NewGuid().ToString("N")[..8],
                FullName = fullName,
                Email = email,
                Role = role,
                MfaMethod = mfaMethod,
                Status = "Pending",
                SetupToken = token,
                SetupTokenExpiresAt = DateTime.UtcNow.AddDays(7),
                RequiresEnrollment = true,
                CreatedAt = DateTime.UtcNow
            };

            _staffUsers.Add(newUser);
            RecordAudit("User", createdBy, createdBy, "Create", "StaffUser", newUser.UserId, null, $"Role: {role}");
            var link = $"/Staff/Enroll?token={token}";
            return (true, link, null);
        }
    }

    public bool ToggleStaffActive(string userId, string updatedBy)
    {
        lock (_lock)
        {
            var user = GetStaffUserById(userId);
            if (user == null) return false;
            var oldVal = user.Status;
            user.Status = user.Status == "Active" ? "Inactive" : "Active";
            RecordAudit("User", updatedBy, updatedBy, "Edit", "StaffUser", user.UserId, oldVal, user.Status);
            return true;
        }
    }

    public bool RevokeStaffInvite(string userId, string updatedBy)
    {
        lock (_lock)
        {
            var user = GetStaffUserById(userId);
            if (user == null || user.Status != "Pending") return false;
            _staffUsers.Remove(user);
            RecordAudit("User", updatedBy, updatedBy, "Delete", "StaffUser", user.UserId, null, "Revoked invitation");
            return true;
        }
    }

    public (bool ok, string? link) GenerateStaffResetLink(string userId, string updatedBy)
    {
        lock (_lock)
        {
            var user = GetStaffUserById(userId);
            if (user == null) return (false, null);
            var token = "RESET-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
            user.SetupToken = token;
            user.SetupTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            RecordAudit("User", updatedBy, updatedBy, "PasswordReset", "StaffUser", user.UserId);
            return (true, $"/Staff/ResetPassword?token={token}");
        }
    }

    private static string GenerateReferenceCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        string Part(int len)
        {
            var res = new char[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }
            return new string(res);
        }
        return $"{Part(4)}-{Part(4)}-{Part(4)}";
    }

    public ReportConfirmationViewModel SubmitReport(ReportFormModel form)
    {
        lock (_lock)
        {
            // Honeypot check
            if (!string.IsNullOrEmpty(form.Honeypot))
            {
                return new ReportConfirmationViewModel
                {
                    Success = false,
                    ErrorMessage = "Invalid submission."
                };
            }

            // RA 9262 Pathway determination:
            // Under RA 9262: Women and children (or other if related to mother/woman protection)
            bool isRa9262 = form.VictimType is "Woman" or "Child" || form.ChildMotherAbused;

            var reportId = "rep-" + Guid.NewGuid().ToString("N")[..8];
            var refCode = GenerateReferenceCode();
            var caseSeq = _cases.Count + 41;
            var caseNumber = $"C-026-{caseSeq:000}";

            var attachedLogs = new List<AttachedLogSnapshot>();
            if (!string.IsNullOrEmpty(form.AttachedLogsJson) && form.LogAttachmentConsent)
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<List<AttachedLogSnapshot>>(form.AttachedLogsJson);
                    if (parsed != null) attachedLogs.AddRange(parsed);
                }
                catch { /* ignore json parse failure */ }
            }

            var report = new ReportRecord
            {
                ReportId = reportId,
                ReferenceCode = refCode,
                SubmissionToken = form.SubmissionToken,
                ReportType = form.ReportType,
                IsThirdParty = form.IsThirdParty,
                VictimType = form.VictimType,
                ChildMotherAbused = form.ChildMotherAbused,
                IsRA9262Pathway = isRa9262,
                IncidentDate = form.IncidentDate,
                IncidentLocation = form.IncidentLocation,
                IncidentDescription = form.IncidentDescription,
                OffenderRelationship = form.OffenderRelationship,
                ChildrenInvolved = form.ChildrenInvolved,
                ReporterName = form.ReportType == "Identified" ? form.ReporterName : null,
                ContactType = form.ReportType == "Identified" ? form.ContactType : null,
                ContactValue = form.ReportType == "Identified" ? form.ContactValue : null,
                SafeContactTime = form.ReportType == "Identified" ? form.SafeContactTime : null,
                VictimName = form.IsThirdParty ? form.VictimName : null,
                VictimContactType = form.IsThirdParty ? form.VictimContactType : null,
                VictimContactValue = form.IsThirdParty ? form.VictimContactValue : null,
                VictimSafeContactTime = form.IsThirdParty ? form.VictimSafeContactTime : null,
                RelationshipToVictim = form.IsThirdParty ? form.RelationshipToVictim : null,
                AttachedLogEntries = attachedLogs,
                SubmittedAt = DateTime.UtcNow
            };

            _reports.Add(report);

            // Record consents
            if (form.ReportType == "Identified" && form.ContactConsent)
            {
                _consents.Add(new ConsentRecord
                {
                    ConsentId = "con-" + Guid.NewGuid().ToString("N")[..8],
                    ReportId = reportId,
                    ConsentType = "Contact",
                    Granted = true,
                    DecidedAt = DateTime.UtcNow
                });
            }

            if (form.IsThirdParty && form.VictimContactConsent)
            {
                _consents.Add(new ConsentRecord
                {
                    ConsentId = "con-" + Guid.NewGuid().ToString("N")[..8],
                    ReportId = reportId,
                    ConsentType = "VictimContact",
                    Granted = true,
                    DecidedAt = DateTime.UtcNow
                });
            }

            if (attachedLogs.Count > 0 && form.LogAttachmentConsent)
            {
                _consents.Add(new ConsentRecord
                {
                    ConsentId = "con-" + Guid.NewGuid().ToString("N")[..8],
                    ReportId = reportId,
                    ConsentType = "LogAttachment",
                    Granted = true,
                    DecidedAt = DateTime.UtcNow
                });
            }

            // Create Case Record
            var caseRecord = new CaseRecord
            {
                CaseId = "case-" + Guid.NewGuid().ToString("N")[..8],
                CaseNumber = caseNumber,
                ReportId = reportId,
                Status = "Received",
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };
            _cases.Add(caseRecord);

            // Record Status History
            _statusHistory.Add(new CaseStatusHistoryEntry
            {
                HistoryId = "hist-" + Guid.NewGuid().ToString("N")[..8],
                CaseId = caseRecord.CaseId,
                ToStatus = "Received",
                ChangedAt = DateTime.UtcNow,
                Reason = "Initial submission"
            });

            // Record Audit
            RecordAudit("Reporter", null, null, "Create", "Report", caseNumber, null, $"Pathway: {(isRa9262 ? "RA 9262" : "General/Referral")}");

            return new ReportConfirmationViewModel
            {
                Success = true,
                ReferenceCode = refCode,
                IsRA9262Pathway = isRa9262,
                ReceivingOffice = isRa9262 ? _settings.ReceivingOfficeName : "Social Welfare Office (C/MSWDO)",
                ReportType = form.ReportType
            };
        }
    }

    public StatusLookupViewModel LookupStatus(string referenceCode)
    {
        lock (_lock)
        {
            var cleaned = referenceCode?.Trim().ToUpperInvariant() ?? "";
            var report = _reports.FirstOrDefault(r => r.ReferenceCode == cleaned);
            if (report == null)
            {
                return new StatusLookupViewModel
                {
                    ReferenceCode = cleaned,
                    Searched = true,
                    Found = false,
                    Message = "No report was found matching this reference code. Please verify the code and try again."
                };
            }

            var caseRec = _cases.FirstOrDefault(c => c.ReportId == report.ReportId);
            var consent = _consents.FirstOrDefault(c => c.ReportId == report.ReportId && c.ConsentType == "Contact");

            RecordAudit("Reporter", null, null, "View", "ReportStatus", caseRec?.CaseNumber ?? report.ReferenceCode);

            return new StatusLookupViewModel
            {
                ReferenceCode = cleaned,
                Searched = true,
                Found = true,
                Status = caseRec?.Status ?? "Received",
                ReceivingOffice = report.IsRA9262Pathway ? _settings.ReceivingOfficeName : "Social Welfare Office (C/MSWDO)",
                CanWithdrawConsent = consent != null && consent.Granted && !consent.WithdrawnAt.HasValue,
                ConsentWithdrawnAt = consent?.WithdrawnAt,
                IsIdentified = report.ReportType == "Identified",
                ContactType = report.ContactType
            };
        }
    }

    public (bool ok, DateTime? withdrawnAt, string? error) WithdrawConsent(string referenceCode)
    {
        lock (_lock)
        {
            var cleaned = referenceCode?.Trim().ToUpperInvariant() ?? "";
            var report = _reports.FirstOrDefault(r => r.ReferenceCode == cleaned);
            if (report == null) return (false, null, "Report not found.");

            var consent = _consents.FirstOrDefault(c => c.ReportId == report.ReportId && c.ConsentType == "Contact");
            if (consent == null) return (false, null, "No contact consent record found for this report.");

            consent.Granted = false;
            consent.WithdrawnAt = DateTime.UtcNow;

            var caseRec = _cases.FirstOrDefault(c => c.ReportId == report.ReportId);
            RecordAudit("Reporter", null, null, "ConsentWithdrawn", "Report", caseRec?.CaseNumber ?? cleaned, "Granted", "Withdrawn");

            return (true, consent.WithdrawnAt, null);
        }
    }

    public List<CaseRecord> GetCases(string? status = null, string? urgency = null, string? officerId = null, string? search = null)
    {
        lock (_lock)
        {
            var query = _cases.AsEnumerable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(c => c.Status == status);
            }

            if (!string.IsNullOrEmpty(urgency) && urgency != "All")
            {
                query = query.Where(c => c.Urgency == urgency);
            }

            if (!string.IsNullOrEmpty(officerId) && officerId != "All")
            {
                if (officerId == "unassigned")
                {
                    query = query.Where(c => string.IsNullOrEmpty(c.AssignedOfficerId));
                }
                else
                {
                    query = query.Where(c => c.AssignedOfficerId == officerId);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c =>
                {
                    if (c.CaseNumber.Contains(term, StringComparison.OrdinalIgnoreCase)) return true;
                    var rep = _reports.FirstOrDefault(r => r.ReportId == c.ReportId);
                    if (rep != null)
                    {
                        if (rep.ReferenceCode.Contains(term, StringComparison.OrdinalIgnoreCase)) return true;
                        if (!string.IsNullOrEmpty(rep.IncidentLocation) && rep.IncidentLocation.Contains(term, StringComparison.OrdinalIgnoreCase)) return true;
                        if (!string.IsNullOrEmpty(rep.ReporterName) && rep.ReporterName.Contains(term, StringComparison.OrdinalIgnoreCase)) return true;
                    }
                    return false;
                });
            }

            return query.OrderByDescending(c => c.LastActivityAt).ToList();
        }
    }

    public CaseRecord? GetCaseById(string caseId)
    {
        lock (_lock)
        {
            return _cases.FirstOrDefault(c => c.CaseId == caseId);
        }
    }

    public ReportRecord? GetReportById(string reportId)
    {
        lock (_lock)
        {
            return _reports.FirstOrDefault(r => r.ReportId == reportId);
        }
    }

    public List<ConsentRecord> GetConsentsByReportId(string reportId)
    {
        lock (_lock)
        {
            return _consents.Where(c => c.ReportId == reportId).ToList();
        }
    }

    public List<CaseNote> GetCaseNotes(string caseId)
    {
        lock (_lock)
        {
            return _notes.Where(n => n.CaseId == caseId).OrderByDescending(n => n.CreatedAt).ToList();
        }
    }

    public List<CaseStatusHistoryEntry> GetStatusHistory(string caseId)
    {
        lock (_lock)
        {
            return _statusHistory.Where(h => h.CaseId == caseId).OrderByDescending(h => h.ChangedAt).ToList();
        }
    }

    public List<CaseAlert> GetAlerts()
    {
        lock (_lock)
        {
            var alerts = new List<CaseAlert>();
            var threshold = DateTime.UtcNow.AddHours(-_settings.AlertThresholdHours);

            foreach (var c in _cases.Where(c => c.Status != "Closed"))
            {
                if (c.LastActivityAt < threshold)
                {
                    var hrs = Math.Round((DateTime.UtcNow - c.LastActivityAt).TotalHours, 1);
                    alerts.Add(new CaseAlert
                    {
                        CaseId = c.CaseId,
                        CaseNumber = c.CaseNumber,
                        Reason = $"No activity for {hrs}h (threshold: {_settings.AlertThresholdHours}h) · Status: {c.Status}"
                    });
                }
            }
            return alerts;
        }
    }

    public bool UpdateCaseTriage(string caseId, string urgency, List<string> abuseTypes, string officerId, string officerName)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c == null) return false;

            c.Urgency = urgency;
            c.AbuseTypes = abuseTypes ?? new List<string>();
            c.LastActivityAt = DateTime.UtcNow;

            RecordAudit("User", officerId, officerName, "Edit", "Case", c.CaseNumber, null, $"Triage: Urgency={urgency}, Types={string.Join(",", c.AbuseTypes)}");
            return true;
        }
    }

    public bool SaveCaseAssessment(string caseId, string outcome, string pathway, string urgency, List<string> abuseTypes, string notes, string officerId, string officerName)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c == null) return false;

            c.AssessmentOutcome = outcome;
            c.EligibilityPathway = pathway;
            c.Urgency = urgency;
            c.AbuseTypes = abuseTypes ?? new List<string>();
            c.AssessmentNotes = notes;
            c.LastActivityAt = DateTime.UtcNow;

            if (c.Status == "Received")
            {
                var oldStatus = c.Status;
                c.Status = "Under Assessment";
                _statusHistory.Add(new CaseStatusHistoryEntry
                {
                    HistoryId = "hist-" + Guid.NewGuid().ToString("N")[..8],
                    CaseId = c.CaseId,
                    FromStatus = oldStatus,
                    ToStatus = c.Status,
                    ChangedBy = officerId,
                    ChangedByName = officerName,
                    ChangedAt = DateTime.UtcNow,
                    Reason = "Assessment conducted"
                });
            }

            RecordAudit("User", officerId, officerName, "Edit", "CaseAssessment", c.CaseNumber, null, $"Outcome: {outcome}");
            return true;
        }
    }

    public bool RecordCaseReferral(string caseId, string outsideOffice, string referralDate, string purpose, string? reference, string? notes, string officerId, string officerName)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c == null) return false;

            c.ReferralOffice = outsideOffice;
            c.ReferralDate = referralDate;
            c.ReferralPurpose = purpose;
            c.ReferralReference = reference;
            c.ReferralNotes = notes;
            c.LastActivityAt = DateTime.UtcNow;

            var oldStatus = c.Status;
            c.Status = "Referred";

            _statusHistory.Add(new CaseStatusHistoryEntry
            {
                HistoryId = "hist-" + Guid.NewGuid().ToString("N")[..8],
                CaseId = c.CaseId,
                FromStatus = oldStatus,
                ToStatus = "Referred",
                ChangedBy = officerId,
                ChangedByName = officerName,
                ChangedAt = DateTime.UtcNow,
                Reason = $"Referred to {outsideOffice}"
            });

            RecordAudit("User", officerId, officerName, "StatusChange", "Case", c.CaseNumber, oldStatus, "Referred");
            return true;
        }
    }

    public bool RecordCaseBpo(string caseId, string bpoRef, string issuedBy, string dateIssued, string? effectiveUntil, string? notes, string officerId, string officerName)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c == null) return false;

            c.BpoReference = bpoRef;
            c.BpoIssuedBy = issuedBy;
            c.BpoDateIssued = dateIssued;
            c.BpoEffectiveUntil = effectiveUntil;
            c.BpoNotes = notes;
            c.LastActivityAt = DateTime.UtcNow;

            var oldStatus = c.Status;
            c.Status = "BPO Issued";

            _statusHistory.Add(new CaseStatusHistoryEntry
            {
                HistoryId = "hist-" + Guid.NewGuid().ToString("N")[..8],
                CaseId = c.CaseId,
                FromStatus = oldStatus,
                ToStatus = "BPO Issued",
                ChangedBy = officerId,
                ChangedByName = officerName,
                ChangedAt = DateTime.UtcNow,
                Reason = $"Barangay Protection Order {bpoRef} issued by {issuedBy}"
            });

            RecordAudit("User", officerId, officerName, "StatusChange", "Case", c.CaseNumber, oldStatus, "BPO Issued");
            return true;
        }
    }

    public bool UpdateCaseStatus(string caseId, string newStatus, string? reason, string officerId, string officerName, string? assignOfficerId = null)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c == null) return false;

            var oldStatus = c.Status;
            c.Status = newStatus;
            c.LastActivityAt = DateTime.UtcNow;

            if (newStatus == "Closed")
            {
                c.ClosedAt = DateTime.UtcNow;
                c.ClosureReason = reason ?? "Resolved";
            }

            if (!string.IsNullOrEmpty(assignOfficerId))
            {
                c.AssignedOfficerId = assignOfficerId;
            }

            _statusHistory.Add(new CaseStatusHistoryEntry
            {
                HistoryId = "hist-" + Guid.NewGuid().ToString("N")[..8],
                CaseId = c.CaseId,
                FromStatus = oldStatus,
                ToStatus = newStatus,
                ChangedBy = officerId,
                ChangedByName = officerName,
                ChangedAt = DateTime.UtcNow,
                Reason = reason
            });

            RecordAudit("User", officerId, officerName, "StatusChange", "Case", c.CaseNumber, oldStatus, newStatus);
            return true;
        }
    }

    public bool AssignCase(string caseId, string? officerId, string currentOfficerId, string currentOfficerName)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c == null) return false;

            var oldOfficer = c.AssignedOfficerId;
            c.AssignedOfficerId = string.IsNullOrEmpty(officerId) ? null : officerId;
            c.LastActivityAt = DateTime.UtcNow;

            var targetOfficer = string.IsNullOrEmpty(officerId) ? "Unassigned" : (GetStaffUserById(officerId)?.FullName ?? officerId);
            RecordAudit("User", currentOfficerId, currentOfficerName, "Edit", "CaseAssignment", c.CaseNumber, oldOfficer, targetOfficer);
            return true;
        }
    }

    public bool AddCaseNote(string caseId, string noteText, string authorId, string authorName)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c == null) return false;

            var note = new CaseNote
            {
                NoteId = "note-" + Guid.NewGuid().ToString("N")[..8],
                CaseId = caseId,
                AuthorId = authorId,
                AuthorName = authorName,
                NoteText = noteText,
                CreatedAt = DateTime.UtcNow
            };
            _notes.Add(note);
            c.LastActivityAt = DateTime.UtcNow;

            RecordAudit("User", authorId, authorName, "Create", "CaseNote", c.CaseNumber);
            return true;
        }
    }

    public bool FlagConflict(string caseId, string officerId, string officerName)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c == null) return false;

            if (!c.ConflictFlaggedByIds.Contains(officerId))
            {
                c.ConflictFlaggedByIds.Add(officerId);
            }

            if (c.AssignedOfficerId == officerId)
            {
                c.AssignedOfficerId = null;
            }
            c.LastActivityAt = DateTime.UtcNow;

            RecordAudit("User", officerId, officerName, "Edit", "CaseConflict", c.CaseNumber, null, "Conflict of interest flagged");
            return true;
        }
    }

    public void RecordCaseView(string caseId, string officerId, string officerName)
    {
        lock (_lock)
        {
            var c = GetCaseById(caseId);
            if (c != null)
            {
                RecordAudit("User", officerId, officerName, "View", "Case", c.CaseNumber);
            }
        }
    }

    public List<AuditLogEntry> GetAuditLogs(string? actorType = null, string? action = null, string? recordType = null)
    {
        lock (_lock)
        {
            var query = _auditLogs.AsEnumerable();
            if (!string.IsNullOrEmpty(actorType) && actorType != "All") query = query.Where(a => a.ActorType == actorType);
            if (!string.IsNullOrEmpty(action) && action != "All") query = query.Where(a => a.Action == action);
            if (!string.IsNullOrEmpty(recordType) && recordType != "All") query = query.Where(a => a.RecordType == recordType);
            return query.OrderByDescending(a => a.CreatedAt).ToList();
        }
    }

    public void RecordAudit(string actorType, string? userId, string? userName, string action, string recordType, string? recordId = null, string? oldValue = null, string? newValue = null)
    {
        var entry = new AuditLogEntry
        {
            LogId = "aud-" + Guid.NewGuid().ToString("N")[..8],
            ActorType = actorType,
            UserId = userId,
            UserName = userName,
            Action = action,
            RecordType = recordType,
            RecordId = recordId,
            OldValue = oldValue,
            NewValue = newValue,
            CreatedAt = DateTime.UtcNow
        };
        _auditLogs.Add(entry);
    }

    public AnonymizedStatsViewModel GetAnonymizedStats()
    {
        lock (_lock)
        {
            var stats = new AnonymizedStatsViewModel
            {
                TotalCases = _cases.Count,
                OpenCases = _cases.Count(c => c.Status != "Closed"),
                ClosedCases = _cases.Count(c => c.Status == "Closed"),
                BpoIssuedCount = _cases.Count(c => c.Status == "BPO Issued"),
                ReferredCount = _cases.Count(c => c.Status == "Referred"),
                AnonymousCount = _reports.Count(r => r.ReportType == "Anonymous"),
                IdentifiedCount = _reports.Count(r => r.ReportType == "Identified")
            };

            foreach (var status in new[] { "Received", "Under Assessment", "Referred", "BPO Issued", "Closed" })
            {
                stats.CasesByStatus[status] = _cases.Count(c => c.Status == status);
            }

            foreach (var urgency in new[] { "High", "Medium", "Low", "Unassessed" })
            {
                if (urgency == "Unassessed")
                    stats.CasesByUrgency[urgency] = _cases.Count(c => string.IsNullOrEmpty(c.Urgency));
                else
                    stats.CasesByUrgency[urgency] = _cases.Count(c => c.Urgency == urgency);
            }

            foreach (var abuse in new[] { "Physical", "Sexual", "Psychological", "Economic" })
            {
                stats.CasesByAbuseType[abuse] = _cases.Count(c => c.AbuseTypes.Contains(abuse));
            }

            foreach (var victim in new[] { "Woman", "Child", "Other" })
            {
                stats.CasesByVictimType[victim] = _reports.Count(r => r.VictimType == victim);
            }

            return stats;
        }
    }
}
