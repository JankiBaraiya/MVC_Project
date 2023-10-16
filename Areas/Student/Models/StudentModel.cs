using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Areas.Student.Models
{
    public class StudentModel
    {
        public int StudentID { get; set; }

        [Required]
        public string StudentName { get; set; }
        public int BranchID { get; set; }

        public string BranchName { get; set; }

        public int CityID { get; set; }
        public string CityName { get; set; }

        [Required]
        public string MobileNoStudent { get; set; }

        [Required]
        public string Email { get; set; }

        public string MobileNoFather { get; set; }

        [Required]
        public string Address { get; set; }


        public DateTime BirthDate { get; set; }

        public int Age { get; set; }


        public bool IsActive { get; set; }

        [Required]
        public string Gender { get; set; }
        public string Password { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

    }
}
