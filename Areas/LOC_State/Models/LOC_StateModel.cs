using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MVC_Project.Areas.LOC_State.Models
{
    public class LOC_StateModel
    {
        public int? StateID { get; set; }

        [Required(ErrorMessage = "State Name is Required")]
        [DisplayName("State Name")]
        public string StateName { get; set; }

        [Required(ErrorMessage = "State Code is Required")]
        [DisplayName("State Code")]
        [StringLength(maximumLength: 100, MinimumLength = 1)]
        public string StateCode { get; set; }

        [Required(ErrorMessage = "Country Name is Required")]
        [DisplayName("Country Name")]
        public string? CountryName { get; set; }

        public int CountryID { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

    }
    public class LOC_StateDropDownModel
    {
        public int StateID { get; set; }
        public string? StateName { get; set; }
    }
}
