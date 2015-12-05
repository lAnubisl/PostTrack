using System.ComponentModel.DataAnnotations;

namespace Posttrack.Web.Models
{
    public class SaveTrackingModel
    {
        [Required]
        [RegularExpression(@".+@.+\..+")]
        public string Email { get; set; }

        [Required]
        [UniqueTrackingNumber(ErrorMessage = "Такой номер уже зарегистрирован в системе")]
        public string Tracking { get; set; }

        [Required]
        public string Description { get; set; }
    }
}