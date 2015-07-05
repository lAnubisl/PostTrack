using System.ComponentModel.DataAnnotations;

namespace Posttrack.Web.Models
{
    public class SaveTrackingModel
    {
        [Required]
        [StringLength(256)]
        [RegularExpression(@".+@.+\..+")]
        public string Email { get; set; }

        [Required]
        [UniqueTrackingNumber(ErrorMessage = "Такой номер уже зарегистрирован в системе")]
        // Validation is disabled because china started using tracking numbers like 44596888337 and WDG30865967CN
        //[RegularExpression(@"^\w{2}\d{9}\w{2}$")]
        public string Tracking { get; set; }

        [Required]
        [StringLength(256)]
        public string Description { get; set; }
    }
}