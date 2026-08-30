namespace YallaArabi.Infrastructure.Notifications;

public class FirebaseOptions
{
    public string ServiceAccountKeyPath { get; set; } = "firebase-service-account.json";
    public bool Enabled { get; set; }
}