namespace RandevuPanel.Models;

public enum ContactPlatform
{
    Facebook,
    Instagram,
    Google,
    Telefon,
    WhatsApp,
    Diger
}

public enum AppointmentStatus
{
    Bekliyor,
    Geldi,
    Tamamlandi,
    IptalEdildi
}

public class Appointment
{
    public int Id { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly AppointmentTime { get; set; }
    public string VehicleBrand { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public int? VehicleYear { get; set; }
    public string? VinNumber { get; set; }
    public string ProcessDescription { get; set; } = string.Empty;
    public ContactPlatform ContactPlatform { get; set; }
    public string? CustomerName { get; set; }
    public string CustomerPhone { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Bekliyor;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; } = null!;
}
