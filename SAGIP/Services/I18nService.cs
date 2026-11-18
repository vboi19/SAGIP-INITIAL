namespace SAGIP.Services;

public class CopyModel
{
    public string EmergencyStrip { get; set; } = "";
    public string SupportContactsLink { get; set; } = "";
    public NavCopy Nav { get; set; } = new();
    public SafetyTipCopy SafetyTip { get; set; } = new();
    public HomeCopy Home { get; set; } = new();
    public FooterCopy Footer { get; set; } = new();
}

public class NavCopy
{
    public string Home { get; set; } = "";
    public string Rights { get; set; } = "";
    public string Contacts { get; set; } = "";
    public string Status { get; set; } = "";
    public string QuickExit { get; set; } = "";
}

public class SafetyTipCopy
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Dismiss { get; set; } = "";
    public string TipsBtn { get; set; } = "";
}

public class HomeCopy
{
    public string Kicker { get; set; } = "";
    public string Headline { get; set; } = "";
    public string Subhead { get; set; } = "";
    public string StartReport { get; set; } = "";
    public string ExploreSupport { get; set; } = "";
    public string MetaLine { get; set; } = "";
    public string SanctuaryKicker { get; set; } = "";
    public string SanctuaryTagalog { get; set; } = "";
    public string SanctuaryEnglish { get; set; } = "";
    public string SanctuaryPillars { get; set; } = "";
    public string PathwaysHeading { get; set; } = "";
    public string PathwaysSub { get; set; } = "";
    public string Card1Title { get; set; } = "";
    public string Card1Desc { get; set; } = "";
    public string Card1Cta { get; set; } = "";
    public string Card2Title { get; set; } = "";
    public string Card2Desc { get; set; } = "";
    public string Card2Cta { get; set; } = "";
    public string Card3Title { get; set; } = "";
    public string Card3Desc { get; set; } = "";
    public string Card3Cta { get; set; } = "";
    public string ClarityHeading { get; set; } = "";
    public string ClaritySub { get; set; } = "";
    public string Step1Title { get; set; } = "";
    public string Step1Desc { get; set; } = "";
    public string Step2Title { get; set; } = "";
    public string Step2Desc { get; set; } = "";
    public string Step3Title { get; set; } = "";
    public string Step3Desc { get; set; } = "";
    public string NotebookKicker { get; set; } = "";
    public string NotebookTitle { get; set; } = "";
    public string NotebookDesc { get; set; } = "";
    public string NotebookCta { get; set; } = "";
    public string NotebookFoot { get; set; } = "";
    public string KnowledgeTitle { get; set; } = "";
    public string KnowledgeSub { get; set; } = "";
    public string KnowledgeCta { get; set; } = "";
    public string K1Title { get; set; } = "";
    public string K1Desc { get; set; } = "";
    public string K2Title { get; set; } = "";
    public string K2Desc { get; set; } = "";
    public string K3Title { get; set; } = "";
    public string K3Desc { get; set; } = "";
    public string Disclaimer { get; set; } = "";
}

public class FooterCopy
{
    public string FullName { get; set; } = "";
    public string Privacy { get; set; } = "";
    public string Accessibility { get; set; } = "";
    public string StaffSignIn { get; set; } = "";
    public string BottomNote { get; set; } = "";
}

public class I18nService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public static readonly CopyModel En = new()
    {
        EmergencyStrip = "In immediate danger? Call 911. SAGIP is not an emergency service.",
        SupportContactsLink = "Support contacts",
        Nav = new NavCopy
        {
            Home = "Home",
            Rights = "Your rights",
            Contacts = "Support contacts",
            Status = "Check status",
            QuickExit = "Quick Exit"
        },
        SafetyTip = new SafetyTipCopy
        {
            Title = "A quick safety reminder",
            Body = "Quick Exit leaves this site right away, but it does not erase your browser history. If someone checks your device, consider using a private window—note that private incident log entries are removed when a private window closes.",
            Dismiss = "Dismiss reminder",
            TipsBtn = "Browser safety tips"
        },
        Home = new HomeCopy
        {
            Kicker = "Private support · Barangay VAWC Desk",
            Headline = "A place to begin. A choice that’s yours.",
            Subhead = "Reach your barangay VAWC desk, understand your rights under RA 9262, and take the next step—only when you are ready.",
            StartReport = "Start a report",
            ExploreSupport = "Explore support",
            MetaLine = "No account needed · Anonymous reporting available · Aligned with RA 10173",
            SanctuaryKicker = "Take a breath. You are here.",
            SanctuaryTagalog = "Hindi ka nag-iisa.",
            SanctuaryEnglish = "You are not alone.",
            SanctuaryPillars = "Your pace · Your voice · Your choice",
            PathwaysHeading = "What would help you today?",
            PathwaysSub = "There is no single right next step.",
            Card1Title = "Share what happened",
            Card1Desc = "Submit a report privately. Choose whether to stay anonymous or share contact details.",
            Card1Cta = "Start a report",
            Card2Title = "Check your report",
            Card2Desc = "Use your reference code to see the current status and receiving office.",
            Card2Cta = "Check status",
            Card3Title = "Understand your options",
            Card3Desc = "Learn about abuse, RA 9262, and Barangay Protection Orders for yourself or someone else.",
            Card3Cta = "Read the guide",
            ClarityHeading = "A little clarity. One step at a time.",
            ClaritySub = "You stay in control of what you share.",
            Step1Title = "Choose your privacy",
            Step1Desc = "Report anonymously without identity details, or share your details for safe contact.",
            Step2Title = "Reach your local desk",
            Step2Desc = "Reports within the service area go directly to your barangay VAWC desk.",
            Step3Title = "Keep your reference code",
            Step3Desc = "Return anytime to check the status. A lost code cannot be recovered.",
            NotebookKicker = "Just for you on this device",
            NotebookTitle = "Not ready to report? Start with a note.",
            NotebookDesc = "Keep a private incident log, encrypted in your browser with your own passphrase. Nothing is shared unless you attach it to an identified report with consent.",
            NotebookCta = "Open private notebook",
            NotebookFoot = "Only this browser · Only this device",
            KnowledgeTitle = "Knowledge can be a first step.",
            KnowledgeSub = "Plain-language guides to help you decide what comes next.",
            KnowledgeCta = "Explore your rights",
            K1Title = "Recognizing abuse",
            K1Desc = "Physical, sexual, psychological, and economic abuse explained clearly.",
            K2Title = "Your rights under RA 9262",
            K2Desc = "Understand the Anti-Violence Against Women and Their Children Act.",
            K3Title = "Protection orders, explained",
            K3Desc = "Learn what a Barangay Protection Order is and where to ask for help.",
            Disclaimer = "This is general information, not legal advice."
        },
        Footer = new FooterCopy
        {
            FullName = "Safe, Anonymous Guide for Incident Protection",
            Privacy = "Privacy notice",
            Accessibility = "Accessibility",
            StaffSignIn = "Staff sign in",
            BottomNote = "You decide what to share. Use Quick Exit at any time."
        }
    };

    public static readonly CopyModel Fil = new()
    {
        EmergencyStrip = "Nasa agarang panganib? Tumawag sa 911. Ang SAGIP ay hindi serbisyong pang-emerhensya.",
        SupportContactsLink = "Mga tanggapang tutulong",
        Nav = new NavCopy
        {
            Home = "Tahanan",
            Rights = "Iyong mga karapatan",
            Contacts = "Mga tulong",
            Status = "Suriin ang ulat",
            QuickExit = "Mabilisang Labas"
        },
        SafetyTip = new SafetyTipCopy
        {
            Title = "Maikling paalala para sa iyong kaligtasan",
            Body = "Ang Mabilisang Labas ay agad na aalis sa site na ito, ngunit hindi nito nabubura ang history ng iyong browser. Kung may sumusuri ng iyong device, gumamit ng private window—tandaan na mawawala ang private notebook kapag isinara ito.",
            Dismiss = "Itago ang paalala",
            TipsBtn = "Gabay sa kaligtasan sa browser"
        },
        Home = new HomeCopy
        {
            Kicker = "Pribadong suporta · Barangay VAWC Desk",
            Headline = "Isang ligtas na simula. Desisyon na nasa iyo.",
            Subhead = "Makipag-ugnayan sa VAWC desk ng iyong barangay, alamin ang iyong mga karapatan sa ilalim ng RA 9262, at humakbang lamang kapag handa ka na.",
            StartReport = "Magsumite ng ulat",
            ExploreSupport = "Maghanap ng tulong",
            MetaLine = "Hindi kailangan ng account · Puwedeng anonymous · Sumusunod sa RA 10173",
            SanctuaryKicker = "Huminga nang malalim. Narito ka na.",
            SanctuaryTagalog = "Hindi ka nag-iisa.",
            SanctuaryEnglish = "May kaagapay ka.",
            SanctuaryPillars = "Sa iyong bilis · Iyong boses · Iyong pasya",
            PathwaysHeading = "Ano ang makakatulong sa iyo ngayon?",
            PathwaysSub = "Walang iisang tamang unang hakbang.",
            Card1Title = "Ibahagi ang nangyari",
            Card1Desc = "Magsumite ng pribadong ulat. Piliin kung mananatiling anonymous o magbibigay ng detalye.",
            Card1Cta = "Magsumite ng ulat",
            Card2Title = "Suriin ang iyong ulat",
            Card2Desc = "Gamitin ang reference code upang makita ang kasalukuyang kalagayan at tanggapan.",
            Card2Cta = "Suriin ang status",
            Card3Title = "Unawain ang iyong opsyon",
            Card3Desc = "Alamin ang tungkol sa pang-aabuso, RA 9262, at Barangay Protection Order.",
            Card3Cta = "Basahin ang gabay",
            ClarityHeading = "Gabay nang hakbang-hakbang.",
            ClaritySub = "Ikaw ang may kontrol sa kung ano ang ibabahagi.",
            Step1Title = "Piliin ang iyong privacy",
            Step1Desc = "Mag-ulat nang anonymous o magbahagi ng detalye para sa ligtas na pakikipag-ugnayan.",
            Step2Title = "Makakarating sa lokal na desk",
            Step2Desc = "Ang mga ulat sa sakop na barangay ay direktang pupunta sa VAWC desk.",
            Step3Title = "Itago ang reference code",
            Step3Desc = "Bumalik anumang oras upang tingnan ang status. Hindi na mababawi ang nawalang code.",
            NotebookKicker = "Para lamang sa iyo sa device na ito",
            NotebookTitle = "Hindi pa handang mag-ulat? Magsimula sa tala.",
            NotebookDesc = "Magpanatili ng pribadong talaan ng mga pangyayari, naka-encrypt sa iyong browser gamit ang sariling passphrase.",
            NotebookCta = "Buksan ang pribadong talaan",
            NotebookFoot = "Sa browser at device lamang na ito",
            KnowledgeTitle = "Ang kaalaman ay unang hakbang.",
            KnowledgeSub = "Payak na gabay upang matulungan kang magpasya.",
            KnowledgeCta = "Alamin ang karapatan",
            K1Title = "Pagkilala sa pang-aabuso",
            K1Desc = "Pisikal, sekswal, sikolohikal, at ekonomikong pang-aabuso.",
            K2Title = "Karapatan sa ilalim ng RA 9262",
            K2Desc = "Unawain ang Anti-Violence Against Women and Their Children Act.",
            K3Title = "Paliwanag sa Protection Order",
            K3Desc = "Alamin kung ano ang Barangay Protection Order at paano humingi ng tulong.",
            Disclaimer = "Ito ay pangkalahatang impormasyon lamang, hindi legal na payo."
        },
        Footer = new FooterCopy
        {
            FullName = "Safe, Anonymous Guide for Incident Protection",
            Privacy = "Abiso sa Privacy",
            Accessibility = "Accessibility",
            StaffSignIn = "Pag-sign in ng kawani",
            BottomNote = "Ikaw ang magpapasya kung ano ang ibabahagi. Gamitin ang Mabilisang Labas anumang oras."
        }
    };

    public I18nService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CurrentLanguage
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Request.Cookies.TryGetValue("sagip_lang", out var lang) && (lang == "fil" || lang == "en"))
            {
                return lang;
            }
            return "en";
        }
    }

    public bool IsFilipino => CurrentLanguage == "fil";

    public CopyModel T => IsFilipino ? Fil : En;

    public string Text(string en, string fil) => IsFilipino ? fil : en;
}
