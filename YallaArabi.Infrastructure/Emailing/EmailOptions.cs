namespace YallaArabi.Infrastructure.Emailing;

public class EmailOptions
{
    public string SmtpServer { get; set; } = default!;
    public int SmtpPort { get; set; }
    public string SmtpLogin { get; set; } = default!;
    public string SmtpKey { get; set; } = default!;
    public string SenderEmail { get; set; } = default!;
    public string SenderName { get; set; } = default!;
}