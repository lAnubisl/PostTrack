using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Posttrack.Web.Models
{
    public class SaveTrackingModel
    {
	    private string tracking;
	    private string email;
	    private string description;

	    [Required]
	    [RegularExpression(@".+@.+\..+")]
	    public string Email
	    {
		    get { return email;}
		    set { email = value.ToLower(CultureInfo.InvariantCulture); }
	    }

	    [Required]
	    [UniqueTrackingNumber(ErrorMessage = "Такой номер уже зарегистрирован в системе")]
	    public string Tracking
	    {
		    get { return tracking; }
			set { tracking = value.ToUpper(CultureInfo.InvariantCulture); }
	    }

	    [Required]
	    public string Description
	    {
		    get { return description; }
			set { description = value.Trim(); }
	    }
    }
}