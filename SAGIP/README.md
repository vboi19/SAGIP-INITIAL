# SAGIP - Safe, Anonymous Guide for Incident Protection
### ASP.NET Core 10.0 MVC Web Application

SAGIP is a trauma-informed, privacy-preserving community intake and case management system for Barangay Violence Against Women and Children (VAWC) Desks, built in accordance with Philippine Republic Act No. 9262 (Anti-Violence Against Women and Their Children Act of 2004) and Republic Act No. 10173 (Data Privacy Act of 2012).

---

## 🚀 Running the Application

```bash
cd C:\SAGIP\SAGIP
dotnet run --launch-profile http
```
The application will be live at: **`http://localhost:5108`** (or `https://localhost:7015`).

---

## 👥 Demo Staff Accounts (One-Click Login available on `/Staff/Login`)

| Name | Role | Email | Capabilities |
| :--- | :--- | :--- | :--- |
| **Maria Santos** | Desk Officer | `maria.santos@barangay.gov.ph` | Case review, legal triage, BPO issuance, referrals, case notes |
| **Ana Reyes** | Desk Officer | `ana.reyes@barangay.gov.ph` | Case triage, MSWDO/WCPD referrals, contact consent checks |
| **Elena Cruz** | Administrator | `elena.cruz@barangay.gov.ph` | Staff invitations, directory management, system configuration, audit logs *(RBAC protected from viewing victim PII)* |

---

## 🛡️ Key Features Recreated from SAGIP-INITIAL

### 1. Public & Victim Protection Portal
- **Sanctuary Landing Page (`/`)**: Supportive trauma-informed copy (*"Hindi ka nag-iisa"*, *"A place to begin. A choice that's yours"*), quick pathways, clarity process steps, and emergency calling shortcuts.
- **Bilingual Interface**: Seamless instant toggle between **English** and **Filipino (Tagalog)** across all public views and guides.
- **Safety Features & Quick Exit**:
  - Prominent **Quick Exit (ESC)** button on every page. Immediately redirects to neutral decoy site (`https://www.pagasa.dost.gov.ph`).
  - Option to securely wipe local private notes on quick exit.
  - Dismissible browser safety banner reminding users of private windows.
- **Confidential Report Intake Wizard (`/Report`)**:
  - **Step 1: Jurisdiction & Screening**: Checks Barangay jurisdiction coverage, Primary victim vs Third-party reporter, Victim Type (Woman, Child, Other - determines RA 9262 vs Referral pathway).
  - **Step 2: Incident Details**: Incident date, Purok/location, offender relationship, children in household, detailed statement of facts narrative.
  - **Step 3: Identity & Contact**: Option to remain **100% Anonymous** or submit **Identified** with explicit contact consent checkbox, safe contact windows, and option to attach local incident notebook logs.
  - **Step 4: Review & Submission**: Summary check, generation of unique 12-character Reference Code (e.g., `K7QM-4XD2-9WTR`).
- **Private Incident Notebook (`/Home/Notebook`)**:
  - Local browser-only incident documentation tool.
  - Allows saving incident dates, locations, and descriptions without sending data to servers.
  - Export entries to `.txt` or seamlessly attach to report wizard as supportive evidence.
- **Report Status Tracker (`/Status`)**:
  - Lookup report status by Reference Code.
  - Shows current status (Received, Under Assessment, Referred, BPO Issued, Closed).
  - **Consent Revocation**: Identified reporters can withdraw contact consent at any time under RA 10173.
- **Rights Guide & Protection Orders (`/Home/Rights`)**:
  - Plain-language explanation of Physical, Sexual, Psychological, and Economic abuse.
  - Detailed breakdown of **Barangay Protection Orders (BPO)** vs TPO vs PPO.
- **Emergency Directory (`/Home/Contacts`)**:
  - Directory of Barangay VAWC Desk, PNP WCPD, C/MSWDO Social Welfare, and Rural Health Unit (RHU), with clickable phone links and category filtering.

### 2. Desk Officer Workspace (`/DeskOfficer`)
- **Dashboard & KPIs**: Total Cases, Cases Under Assessment, BPOs Issued, Unassigned Cases.
- **SLA Inactivity Alerts**: Flags cases exceeding inactivity threshold (default: 24h).
- **Registry & Filtering**: Filter by Status, Urgency (High/Medium/Low), Assigned Officer, or search query.
- **Comprehensive Case Detail View (`/DeskOfficer/Details/{id}`)**:
  - Full incident narrative & attached notebook entries.
  - Reporter contact details with real-time Consent Status badge.
  - **Conflict of Interest Flagging**: Recusal button unassigns officer and locks record.
  - **Officer Action Tools**:
    1. *Triage & Assessment*: Set Urgency, multi-select Abuse Types, record outcome.
    2. *Barangay Protection Order (BPO)*: Record official BPO reference #, issuing official, date issued, and 15-day duration.
    3. *External Referral*: Formally refer case to C/MSWDO, PNP WCPD, or RHU.
  - **Internal Case Notes Timeline**: Add and review timestamped notes.
  - **Status History Audit Trail**: Tracks who transitioned case status, timestamps, and reasons.

### 3. Administrator Workspace (`/Admin`)
- **Strict Separation of Duties (RBAC)**: Enforces privacy compliance. Administrators are blocked from accessing victim PII and case files. Includes live guardrail test button that logs `AccessDenied` events.
- **Anonymized Statistics**: Aggregate charts by abuse category, status pipeline, urgency, and small-count suppression.
- **Staff User Management**: Invite new staff, toggle active/inactive status, revoke invitations, generate password reset links.
- **Support Contacts Directory**: Add, update, and manage public contact listings.
- **System Settings**: Configure barangay jurisdiction, DPO information, decoy URL, timeout limits, and retention periods.
- **System Audit Trail**: Complete immutable event log filterable by Actor, Action, and Record Type.
