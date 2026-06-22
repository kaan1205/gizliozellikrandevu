using System.ComponentModel.DataAnnotations;
using RandevuPanel.Models;

namespace RandevuPanel.ViewModels.Leads;

public class LeadCreateViewModel
{
    [Required(ErrorMessage = "Müşteri adı zorunludur.")]
    [Display(Name = "Müşteri Adı Soyadı")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Araç Markası")]
    public string? VehicleBrand { get; set; }

    [Display(Name = "Araç Modeli")]
    public string? VehicleModel { get; set; }

    [Display(Name = "Model Yılı")]
    [Range(1900, 2100, ErrorMessage = "Geçerli bir yıl giriniz.")]
    public int? VehicleYear { get; set; }

    [Required(ErrorMessage = "İletişim platformu zorunludur.")]
    [Display(Name = "Nereden Geldi")]
    public ContactPlatform ContactPlatform { get; set; }

    [Required(ErrorMessage = "İstediği işlem zorunludur.")]
    [Display(Name = "İstediği İşlem")]
    public string ProcessDescription { get; set; } = string.Empty;

    [Display(Name = "Not")]
    public string? Notes { get; set; }
}
