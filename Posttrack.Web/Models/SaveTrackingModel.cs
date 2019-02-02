using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Posttrack.Web.Models
{
    public class SaveTrackingModel
    {
        private string _tracking;
        private string _email;
        private string _description;

        [Required]
        [RegularExpression(@".+@.+\..+")]
        public string Email
        {
            get { return _email; }
#pragma warning disable CA1308 // Normalize strings to uppercase
            set { _email = value.ToLowerInvariant(); }
#pragma warning restore CA1308 // Normalize strings to uppercase
        }

        [Required]
        [UniqueTrackingNumber(ErrorMessage = "Такой номер уже зарегистрирован в системе")]
        public string Tracking
        {
            get { return _tracking; }
            set { _tracking = value.ToUpperInvariant(); }
        }

        [Required]
        public string Description
        {
            get { return _description; }
            set { _description = value.Trim(); }
        }
    }
}