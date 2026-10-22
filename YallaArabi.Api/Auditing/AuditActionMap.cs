namespace YallaArabi.Api.Auditing;

public static class AuditActionMap
{
    private sealed record Rule(string Method, string PathPattern, string ActionEn, string ActionAr, string? EntityType);

    private static readonly List<Rule> Rules = new()
    {
        new("POST",   "/admin/words/bulk-import",       "Bulk import words",        "استيراد مفردات من ملف Excel", "Word"),
        new("POST",   "/admin/words",                   "Create word",              "إضافة مفردة",                 "Word"),
        new("PUT",    "/admin/words",                   "Update word",              "تعديل مفردة",                 "Word"),
        new("DELETE", "/admin/words",                   "Delete word",              "حذف مفردة",                   "Word"),

        new("POST",   "/admin/levels",                  "Create level",             "إضافة مستوى",                 "Level"),
        new("PUT",    "/admin/levels",                  "Update level",             "تعديل مستوى",                 "Level"),
        new("DELETE", "/admin/levels",                  "Delete level",             "حذف مستوى",                   "Level"),

        new("POST",   "/admin/categories",              "Create category",          "إضافة وحدة",                  "Category"),
        new("PUT",    "/admin/categories",              "Update category",          "تعديل وحدة",                  "Category"),
        new("DELETE", "/admin/categories",              "Delete category",          "حذف وحدة",                    "Category"),

        new("POST",   "/admin/students",                "Create student",           "إضافة طالب",                  "Student"),
        new("PUT",    "/admin/students",                "Update student",           "تعديل بيانات طالب",           "Student"),
        new("DELETE", "/admin/students",                "Delete student",           "حذف طالب",                    "Student"),
        new("POST",   "/admin/users/force-password-reset", "Force password reset",  "فرض إعادة تعيين كلمة المرور", "Student"),

        new("POST",   "/admin/team",                    "Create team member",       "إضافة عضو فريق",              "TeamMember"),
        new("PUT",    "/admin/team",                    "Update team member",       "تعديل عضو فريق",              "TeamMember"),
        new("DELETE", "/admin/team",                    "Delete team member",       "حذف عضو فريق",                "TeamMember"),

        new("POST",   "/admin/roles",                   "Create role",              "إنشاء دور",                   "Role"),
        new("PUT",    "/admin/roles",                   "Update role permissions",  "تعديل دور وصلاحياته",         "Role"),
        new("DELETE", "/admin/roles",                   "Delete role",              "حذف دور",                     "Role"),
        new("PUT",    "/admin/users",                   "Assign role to user",      "إسناد دور لمستخدم",           "User"),

        new("POST",   "/admin/subscriptions/grant",     "Grant subscription",       "منح اشتراك",                  "Subscription"),
        new("POST",   "/admin/subscriptions",           "Cancel subscription",      "إلغاء اشتراك",                "Subscription"),

        new("POST",   "/admin/notifications/send",      "Send notification",        "إرسال إشعار",                 "Notification"),

        new("POST",   "/admin/point-rules",             "Create point rule",        "إضافة قاعدة نقاط",            "PointRule"),
        new("PUT",    "/admin/point-rules",             "Update point rule",        "تعديل قاعدة نقاط",            "PointRule"),

        new("POST",   "/admin/challenges/questions",    "Add challenge question",   "إضافة سؤال لتحدٍّ",           "ChallengeQuestion"),
        new("PUT",    "/admin/challenges/questions",    "Update challenge question","تعديل سؤال تحدٍّ",            "ChallengeQuestion"),
        new("DELETE", "/admin/challenges/questions",    "Delete challenge question","حذف سؤال تحدٍّ",              "ChallengeQuestion"),
        new("POST",   "/admin/challenges",              "Create challenge",         "إنشاء تحدٍّ",                 "Challenge"),
        new("PUT",    "/admin/challenges",              "Update challenge",         "تعديل تحدٍّ",                 "Challenge"),
        new("DELETE", "/admin/challenges",              "Delete challenge",         "حذف تحدٍّ",                   "Challenge"),

        new("POST",   "/admin/uploads",                 "Upload file",              "رفع ملف",                     "Upload"),
                new("POST",   "/admin/lessons",                 "Create lesson",            "إضافة درس",                   "Lesson"),
        new("PUT",    "/admin/lessons",                 "Update lesson",            "تعديل درس",                   "Lesson"),
        new("DELETE", "/admin/lessons",                 "Delete lesson",            "حذف درس",                     "Lesson"),
                new("POST",   "/admin/recording-sessions/prepare", "Prepare recording session", "تحضير جلسة تسجيل", "RecordingSession"),
    };

    public static (string ActionEn, string ActionAr, string? EntityType) Resolve(string method, string path)
    {
        var normalizedPath = path.ToLowerInvariant();

        if (normalizedPath.Contains("/assign"))
        {
            return ("Assign challenge to students", "إسناد تحدٍّ لطلاب", "Challenge");
        }

        if (normalizedPath.Contains("/publish"))
        {
            return ("Publish challenge", "نشر تحدٍّ", "Challenge");
        }

        if (normalizedPath.Contains("/cancel"))
        {
            return ("Cancel subscription", "إلغاء اشتراك", "Subscription");
        }

        // Lesson content routes nest under /admin/lessons, so they need their own check before the generic lesson rules.
        if (normalizedPath.Contains("/admin/lessons/") && normalizedPath.Contains("/questions"))
        {
            return method switch
            {
                "POST" => ("Add lesson question", "إضافة سؤال لدرس", "LessonQuestion"),
                "PUT" => ("Update lesson question", "تعديل سؤال درس", "LessonQuestion"),
                _ => ("Delete lesson question", "حذف سؤال درس", "LessonQuestion")
            };
        }

        if (normalizedPath.Contains("/admin/lessons/") && normalizedPath.Contains("/words"))
        {
            return method == "POST"
                ? ("Add words to lesson", "إضافة كلمات لدرس", "Lesson")
                : ("Remove word from lesson", "إزالة كلمة من درس", "Lesson");
        }

        if (normalizedPath.Contains("/admin/ai-mode/settings"))
        {
            return ("Update AI Mode settings", "تعديل إعدادات الذكاء الاصطناعي", "AiModeSettings");
        }

        if (normalizedPath.Contains("/admin/ai-mode/entries") && normalizedPath.Contains("/image"))
        {
            return method == "DELETE"
                ? ("Remove AI result image", "حذف صورة نتيجة ذكاء اصطناعي", "AiDictionaryEntry")
                : normalizedPath.EndsWith("/image/search")
                    ? ("Find AI result image", "جلب صورة تلقائية لنتيجة ذكاء اصطناعي", "AiDictionaryEntry")
                    : ("Upload AI result image", "رفع صورة لنتيجة ذكاء اصطناعي", "AiDictionaryEntry");
        }

        if (normalizedPath.Contains("/admin/ai-mode/entries"))
        {
            return normalizedPath.EndsWith("/approve")
                ? ("Approve AI result", "اعتماد نتيجة ذكاء اصطناعي", "AiDictionaryEntry")
                : normalizedPath.EndsWith("/reject")
                    ? ("Reject AI result", "رفض نتيجة ذكاء اصطناعي", "AiDictionaryEntry")
                    : ("Edit AI result", "تعديل نتيجة ذكاء اصطناعي", "AiDictionaryEntry");
        }

        if (normalizedPath.Contains("/role") && method == "PUT")
        {
            return ("Assign role to user", "إسناد دور لمستخدم", "User");
        }

        var match = Rules
            .Where(r => r.Method == method && normalizedPath.Contains(r.PathPattern))
            .OrderByDescending(r => r.PathPattern.Length)
            .FirstOrDefault();

        return match is null
            ? ($"{method} {path}", $"{method} {path}", null)
            : (match.ActionEn, match.ActionAr, match.EntityType);
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