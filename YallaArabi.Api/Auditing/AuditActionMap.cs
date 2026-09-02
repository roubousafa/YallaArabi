namespace YallaArabi.Api.Auditing;

public static class AuditActionMap
{
    private sealed record Rule(string Method, string PathPattern, string ActionAr, string? EntityType);

    private static readonly List<Rule> Rules = new()
    {
        new("POST",   "/admin/words/bulk-import",       "استيراد مفردات من ملف Excel", "Word"),
        new("POST",   "/admin/words",                   "إضافة مفردة",                 "Word"),
        new("PUT",    "/admin/words",                   "تعديل مفردة",                 "Word"),
        new("DELETE", "/admin/words",                   "حذف مفردة",                   "Word"),

        new("POST",   "/admin/levels",                  "إضافة مستوى",                 "Level"),
        new("PUT",    "/admin/levels",                  "تعديل مستوى",                 "Level"),
        new("DELETE", "/admin/levels",                  "حذف مستوى",                   "Level"),

        new("POST",   "/admin/categories",              "إضافة وحدة",                  "Category"),
        new("PUT",    "/admin/categories",              "تعديل وحدة",                  "Category"),
        new("DELETE", "/admin/categories",              "حذف وحدة",                    "Category"),

        new("PUT",    "/admin/students",                "تعديل بيانات طالب",           "Student"),
        new("DELETE", "/admin/students",                "حذف طالب",                    "Student"),
        new("POST",   "/admin/users/force-password-reset", "فرض إعادة تعيين كلمة المرور", "Student"),

        new("POST",   "/admin/team",                    "إضافة عضو فريق",              "TeamMember"),
        new("PUT",    "/admin/team",                    "تعديل عضو فريق",              "TeamMember"),
        new("DELETE", "/admin/team",                    "حذف عضو فريق",                "TeamMember"),

        new("POST",   "/admin/roles",                   "إنشاء دور",                   "Role"),
        new("PUT",    "/admin/roles",                   "تعديل دور وصلاحياته",         "Role"),
        new("DELETE", "/admin/roles",                   "حذف دور",                     "Role"),
        new("PUT",    "/admin/users",                   "إسناد دور لمستخدم",           "User"),

        new("POST",   "/admin/subscriptions/grant",     "منح اشتراك",                  "Subscription"),
        new("POST",   "/admin/subscriptions",           "إلغاء اشتراك",                "Subscription"),

        new("POST",   "/admin/notifications/send",      "إرسال إشعار",                 "Notification"),

        new("POST",   "/admin/point-rules",             "إضافة قاعدة نقاط",            "PointRule"),
        new("PUT",    "/admin/point-rules",             "تعديل قاعدة نقاط",            "PointRule"),

        new("POST",   "/admin/challenges/questions",    "إضافة سؤال لتحدٍّ",           "ChallengeQuestion"),
        new("PUT",    "/admin/challenges/questions",    "تعديل سؤال تحدٍّ",            "ChallengeQuestion"),
        new("DELETE", "/admin/challenges/questions",    "حذف سؤال تحدٍّ",              "ChallengeQuestion"),
        new("POST",   "/admin/challenges",              "إنشاء تحدٍّ",                 "Challenge"),
        new("PUT",    "/admin/challenges",              "تعديل تحدٍّ",                 "Challenge"),
        new("DELETE", "/admin/challenges",              "حذف تحدٍّ",                   "Challenge"),

        new("POST",   "/admin/uploads",                 "رفع ملف",                     "Upload"),
    };

    public static (string Action, string? EntityType) Resolve(string method, string path)
    {
        var normalizedPath = path.ToLowerInvariant();

        if (normalizedPath.Contains("/assign"))
        {
            return ("إسناد تحدٍّ لطلاب", "Challenge");
        }

        if (normalizedPath.Contains("/publish"))
        {
            return ("نشر تحدٍّ", "Challenge");
        }

        if (normalizedPath.Contains("/cancel"))
        {
            return ("إلغاء اشتراك", "Subscription");
        }

        if (normalizedPath.Contains("/role") && method == "PUT")
        {
            return ("إسناد دور لمستخدم", "User");
        }

        var match = Rules
            .Where(r => r.Method == method && normalizedPath.Contains(r.PathPattern))
            .OrderByDescending(r => r.PathPattern.Length)
            .FirstOrDefault();

        return match is null
            ? ($"{method} {path}", null)
            : (match.ActionAr, match.EntityType);
    }

    public static string? ExtractEntityId(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        for (var i = segments.Length - 1; i >= 0; i--)
        {
            if (Guid.TryParse(segments[i], out var id))
            {
                return id.ToString();
            }
        }

        return null;
    }
}