using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

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
        [RegularExpression(@"^\w{2}\d{9}\w{2}$")]
        public string Tracking { get; set; }

        [Required] 
        [StringLength(256)]
        public string Description { get; set; }
    }
}