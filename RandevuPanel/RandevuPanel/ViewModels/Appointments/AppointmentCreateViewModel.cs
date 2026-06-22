using System.ComponentModel.DataAnnotations;
using RandevuPanel.Models;

namespace RandevuPanel.ViewModels.Appointments;

public class AppointmentCreateViewModel
{
    [Required(ErrorMessage = "Randevu tarihi zorunludur.")]
    [Display(Name = "Randevu Tarihi")]
    public DateOnly AppointmentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Randevu saati zorunludur.")]
    [Display(Name = "Randevu Saati")]
    public TimeOnly AppointmentTime { get; set; } = new TimeOnly(9, 0);

    [Required(ErrorMessage = "Araç markası zorunludur.")]
    [Display(Name = "Araç Markası")]
    public string VehicleBrand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Araç modeli zorunludur.")]
    [Display(Name = "Araç Modeli")]
    public string VehicleModel { get; set; } = string.Empty;

    [Display(Name = "Model Yılı")]
    [Range(1900, 2100, ErrorMessage = "Geçerli bir yıl giriniz.")]
    public int? VehicleYear { get; set; }

    [Display(Name = "Şase No")]
    [MaxLength(17, ErrorMessage = "Şase no en fazla 17 karakter olabilir.")]
    public string? VinNumber { get; set; }

    [Required(ErrorMessage = "Yapılacak işlem zorunludur.")]
    [Display(Name = "Yapılacak İşlem")]
    public string ProcessDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "İletişim platformu zorunludur.")]
    [Display(Name = "İletişim Platformu")]
    public ContactPlatform ContactPlatform { get; set; }

    [Display(Name = "Müşteri Adı Soyadı")]
    public string? CustomerName { get; set; }

    [Required(ErrorMessage = "Müşteri iletişim numarası zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [Display(Name = "Müşteri Telefonu")]
    public string CustomerPhone { get; set; } = string.Empty;

    [Display(Name = "Ek Not")]
    public string? Notes { get; set; }
}
