namespace YallaArabi.Domain.Common;

public static class Permissions
{
    public const string SuperAdminRoleName = "Super Admin";

    public static class Dashboard
    {
        public const string View = "dashboard.view";
    }

    public static class Students
    {
        public const string View = "students.view";
        public const string Create = "students.create";
        public const string Edit = "students.edit";
        public const string Delete = "students.delete";
        public const string ForcePasswordReset = "students.force_password_reset";
    }

    public static class Words
    {
        public const string View = "words.view";
        public const string Create = "words.create";
        public const string Edit = "words.edit";
        public const string Delete = "words.delete";
        public const string Import = "words.import";
    }

    public static class Levels
    {
        public const string View = "levels.view";
        public const string Create = "levels.create";
        public const string Edit = "levels.edit";
        public const string Delete = "levels.delete";
    }

    public static class Categories
    {
        public const string View = "categories.view";
        public const string Create = "categories.create";
        public const string Edit = "categories.edit";
        public const string Delete = "categories.delete";
    }

    public static class Challenges
    {
        public const string View = "challenges.view";
        public const string Create = "challenges.create";
        public const string Edit = "challenges.edit";
        public const string Delete = "challenges.delete";
        public const string Publish = "challenges.publish";
        public const string Assign = "challenges.assign";
        public const string ViewParticipants = "challenges.view_participants";
    }

    public static class Notifications
    {
        public const string View = "notifications.view";
        public const string Send = "notifications.send";
    }

    public static class Subscriptions
    {
        public const string View = "subscriptions.view";
        public const string Grant = "subscriptions.grant";
        public const string Cancel = "subscriptions.cancel";
    }

    public static class PointRules
    {
        public const string View = "point_rules.view";
        public const string Create = "point_rules.create";
        public const string Edit = "point_rules.edit";
    }

    public static class Uploads
    {
        public const string Upload = "uploads.upload";
    }

    public static class Users
    {
        public const string View = "users.view";
        public const string Create = "users.create";
        public const string Edit = "users.edit";
        public const string Delete = "users.delete";
    }

    public static class Roles
    {
        public const string View = "roles.view";
        public const string Create = "roles.create";
        public const string Edit = "roles.edit";
        public const string Delete = "roles.delete";
        public const string Assign = "roles.assign";
    }

    public static class AuditLogs
    {
        public const string View = "audit_logs.view";
        public const string Export = "audit_logs.export";
    }

    public static class Reports
    {
        public const string View = "reports.view";
        public const string Export = "reports.export";
    }

    public static class Recordings
    {
        public const string View = "recordings.view";
        public const string Manage = "recordings.manage";
    }

    public static class Lessons
    {
        public const string View = "lessons.view";
        public const string Create = "lessons.create";
        public const string Edit = "lessons.edit";
        public const string Delete = "lessons.delete";
    }

    public static IReadOnlyList<PermissionDefinition> All { get; } = new List<PermissionDefinition>
    {
        new(Dashboard.View, "Dashboard", "عرض لوحة المعلومات", "View dashboard"),

        new(Students.View, "Students", "عرض الطلاب", "View students"),
        new(Students.Create, "Students", "إضافة طالب", "Create student"),
        new(Students.Edit, "Students", "تعديل طالب", "Edit student"),
        new(Students.Delete, "Students", "حذف طالب", "Delete student"),
        new(Students.ForcePasswordReset, "Students", "طرد وإعادة تعيين كلمة المرور", "Force password reset"),

        new(Words.View, "Dictionary", "عرض المفردات", "View words"),
        new(Words.Create, "Dictionary", "إضافة مفردة", "Create word"),
        new(Words.Edit, "Dictionary", "تعديل مفردة", "Edit word"),
        new(Words.Delete, "Dictionary", "حذف مفردة", "Delete word"),
        new(Words.Import, "Dictionary", "استيراد من Excel", "Bulk import words"),

        new(Levels.View, "Levels", "عرض المستويات", "View levels"),
        new(Levels.Create, "Levels", "إضافة مستوى", "Create level"),
        new(Levels.Edit, "Levels", "تعديل مستوى", "Edit level"),
        new(Levels.Delete, "Levels", "حذف مستوى", "Delete level"),

        new(Categories.View, "Content", "عرض الوحدات", "View categories"),
        new(Categories.Create, "Content", "إضافة وحدة", "Create category"),
        new(Categories.Edit, "Content", "تعديل وحدة", "Edit category"),
        new(Categories.Delete, "Content", "حذف وحدة", "Delete category"),

        new(Challenges.View, "Challenges", "عرض التحديات", "View challenges"),
        new(Challenges.Create, "Challenges", "إنشاء تحدٍّ", "Create challenge"),
        new(Challenges.Edit, "Challenges", "تعديل تحدٍّ", "Edit challenge"),
        new(Challenges.Delete, "Challenges", "حذف تحدٍّ", "Delete challenge"),
        new(Challenges.Publish, "Challenges", "نشر تحدٍّ", "Publish challenge"),
        new(Challenges.Assign, "Challenges", "إسناد تحدٍّ", "Assign challenge"),
        new(Challenges.ViewParticipants, "Challenges", "عرض المشاركين", "View participants"),

        new(Notifications.View, "Notifications", "عرض الإشعارات", "View notifications"),
        new(Notifications.Send, "Notifications", "إرسال إشعار", "Send notification"),

        new(Subscriptions.View, "Subscriptions", "عرض الاشتراكات", "View subscriptions"),
        new(Subscriptions.Grant, "Subscriptions", "منح اشتراك", "Grant subscription"),
        new(Subscriptions.Cancel, "Subscriptions", "إلغاء اشتراك", "Cancel subscription"),

        new(PointRules.View, "Point Rules", "عرض قواعد النقاط", "View point rules"),
        new(PointRules.Create, "Point Rules", "إضافة قاعدة", "Create point rule"),
        new(PointRules.Edit, "Point Rules", "تعديل قاعدة", "Edit point rule"),

        new(Uploads.Upload, "Uploads", "رفع الملفات", "Upload files"),

        new(Users.View, "Team", "عرض المستخدمين", "View users"),
        new(Users.Create, "Team", "إضافة مستخدم", "Create user"),
        new(Users.Edit, "Team", "تعديل مستخدم", "Edit user"),
        new(Users.Delete, "Team", "حذف مستخدم", "Delete user"),

        new(Roles.View, "Roles", "عرض الأدوار", "View roles"),
        new(Roles.Create, "Roles", "إنشاء دور", "Create role"),
        new(Roles.Edit, "Roles", "تعديل دور", "Edit role"),
        new(Roles.Delete, "Roles", "حذف دور", "Delete role"),
        new(Roles.Assign, "Roles", "إسناد دور", "Assign role"),

        new(AuditLogs.View, "Audit Log", "عرض سجل التدقيق", "View audit log"),
        new(AuditLogs.Export, "Audit Log", "تصدير سجل التدقيق", "Export audit log"),

         new(Reports.View, "Reports", "عرض التقارير", "View reports"),
        new(Reports.Export, "Reports", "تصدير التقارير", "Export reports"),

                new(Lessons.View, "Content", "عرض الدروس", "View lessons"),
        new(Lessons.Create, "Content", "إضافة درس", "Create lesson"),
        new(Lessons.Edit, "Content", "تعديل درس", "Edit lesson"),
        new(Lessons.Delete, "Content", "حذف درس", "Delete lesson"),

                new(Recordings.View, "Recordings", "عرض جلسات التسجيل", "View recording sessions"),
        new(Recordings.Manage, "Recordings", "إدارة جلسات التسجيل", "Manage recording sessions"),
    };
}

public sealed record PermissionDefinition(string Key, string Group, string NameAr, string NameEn);