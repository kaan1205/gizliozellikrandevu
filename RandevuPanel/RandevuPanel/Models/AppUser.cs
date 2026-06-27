namespace RandevuPanel.Models;

public class AppUser
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Lead> Leads { get; set; } = new List<Lead>();
    public ICollection<ServiceNote> ServiceNotes { get; set; } = new List<ServiceNote>();
    public ICollection<CompletedOperation> CompletedOperations { get; set; } = new List<CompletedOperation>();

    public string FullName => $"{FirstName} {LastName}";
}
