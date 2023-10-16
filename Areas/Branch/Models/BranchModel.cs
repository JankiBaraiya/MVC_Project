using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Areas.Branch.Models
{
	public class BranchModel
	{
		public int? BranchID { get; set; }

        [Required]
        public string BranchName { get; set; }

        [Required]
        public string BranchCode { get; set; }

		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
	}
    public class BranchDropDownModel
    {
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
    }
}
