using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MVC_Project.Areas.Student.Models;
using static System.Formats.Asn1.AsnWriter;
using System.Security.Cryptography.X509Certificates;
using MVC_Project.Areas.LOC_City.Models;
using static MVC_Project.Areas.LOC_Country.Models.LOC_CountryModel;
using MVC_Project.Areas.Branch.Models;

namespace MVC_Project.Areas.Student.Controllers
{
	[Area("Student")]
	[Route("Student/{controller}/{action}/{id?}")]
	public class StudentController : Controller
	{
		private IConfiguration Configuration;

		#region Con
		public StudentController(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		#endregion

		#region SelectAll
		public IActionResult StudentList()
		{
			DataTable dt = new DataTable();
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "PR_Student_SelectAll";
			SqlDataReader reader = command.ExecuteReader();

			if (reader.HasRows)
			{
				dt.Load(reader);
			}

			return View(dt);
		}
		#endregion

		#region Delete

		public IActionResult Delete(int StudentID)
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "PR_Student_DeleteByPK";
			command.Parameters.AddWithValue("@StudentID", StudentID);
			command.ExecuteNonQuery();
			connection.Close();
			return RedirectToAction("StudentList");
		}

		#endregion
		#region Student_AddEdit
		public IActionResult Student_AddEdit(int? StudentID)
		{
			FillCityDDL();
            FillBranchDDL();

            if (StudentID != null)
			{
				string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
				SqlConnection connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand objCmd = connection.CreateCommand();
				objCmd.CommandType = CommandType.StoredProcedure;
				objCmd.CommandText = "PR_Student_SelectAllByPK";
				objCmd.Parameters.AddWithValue("@StudentID", StudentID);
				SqlDataReader reader = objCmd.ExecuteReader();
				DataTable table = new DataTable();
				table.Load(reader);

				StudentModel StudentModel = new StudentModel();
				foreach (DataRow dr in table.Rows)
				{
					StudentModel.StudentName = @dr["StudentName"].ToString();
					StudentModel.BranchID = Convert.ToInt32(@dr["BranchID"]);
					StudentModel.CityID = Convert.ToInt32(@dr["CityID"]);
					StudentModel.Email = @dr["Email"].ToString();
					StudentModel.MobileNoStudent = @dr["MobileNoStudent"].ToString();
					StudentModel.MobileNoFather = @dr["MobileNoFather"].ToString();
					StudentModel.Address = @dr["Address"].ToString();
					StudentModel.BirthDate = Convert.ToDateTime(@dr["BirthDate"]);
					StudentModel.Age = Convert.ToInt32(@dr["Age"]);
					StudentModel.IsActive = Convert.ToBoolean(@dr["IsActive"]);
					StudentModel.Gender = @dr["Gender"].ToString();
					StudentModel.Password = @dr["Password"].ToString();
				}
				return View(StudentModel);
			}

			return View();
		}
		#endregion


		#region Save

		[HttpPost]
		public IActionResult Save(StudentModel model)
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand objCmd = connection.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			if (model.StudentID == 0)
			{
                objCmd.CommandText = "PR_Student_Insert";
			}
			else
			{
                objCmd.CommandText = "PR_Student_UpdateByPK";
                objCmd.Parameters.Add("@StudentID", SqlDbType.Int).Value = model.StudentID;
            }
            objCmd.Parameters.Add("@StudentName", SqlDbType.VarChar).Value = model.StudentName;
			objCmd.Parameters.Add("@BranchID", SqlDbType.Int).Value = model.BranchID;
			objCmd.Parameters.Add("@CityID", SqlDbType.Int).Value = model.CityID;
            objCmd.Parameters.Add("@MobileNoStudent", SqlDbType.VarChar).Value = model.MobileNoStudent;
			objCmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = model.Email;
			objCmd.Parameters.Add("@MobileNoFather", SqlDbType.VarChar).Value = model.MobileNoFather;
			objCmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = model.Address;
			objCmd.Parameters.Add("@BirthDate", SqlDbType.DateTime).Value = model.BirthDate;
			objCmd.Parameters.Add("@Age", SqlDbType.Int).Value = model.Age;
			objCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = model.IsActive;
            objCmd.Parameters.Add("@Gender", SqlDbType.VarChar).Value = model.Gender;
			objCmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = model.Password;

			objCmd.ExecuteNonQuery();
			connection.Close();
			return RedirectToAction("StudentList");
		}

        #endregion


        #region FillCityDDL
        public void FillCityDDL()
        {
            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
            List<LOC_CityDropDownModel> loc_City = new List<LOC_CityDropDownModel>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand objCmd = connection.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_City_SelectForDropDown";
            SqlDataReader objSDR = objCmd.ExecuteReader();
            if (objSDR.HasRows)
            {
                while (objSDR.Read())
                {
                    LOC_CityDropDownModel City = new LOC_CityDropDownModel()
                    {
                        CityID = Convert.ToInt32(objSDR["CityID"]),
                        CityName= objSDR["CityName"].ToString()
                    };
                    loc_City.Add(City);
                }
                objSDR.Close();
            }
            connection.Close();
            ViewBag.CityList = loc_City;

        }
        #endregion

        #region FillBranchDDL
        public void FillBranchDDL()
        {
            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
            List<BranchDropDownModel> branch = new List<BranchDropDownModel>();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand objCmd = connection.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_Branch_SelectForDropDown";
            SqlDataReader objSDR = objCmd.ExecuteReader();
            if (objSDR.HasRows)
            {
                while (objSDR.Read())
                {
                    BranchDropDownModel Branch = new BranchDropDownModel()
                    {
                        BranchID = Convert.ToInt32(objSDR["BranchID"]),
                        BranchName = objSDR["BranchName"].ToString()
                    };
                    branch.Add(Branch);
                }
                objSDR.Close();
            }
            connection.Close();
            ViewBag.Branch = branch;

        }
		#endregion

		#region Filtter
		public IActionResult Filtter()
		{
			string StudentName = HttpContext.Request.Form["StudentName"];
			string BranchName = HttpContext.Request.Form["BranchName"];
			string CityName = HttpContext.Request.Form["CityName"];
			string Age = HttpContext.Request.Form["Age"];
			string Email = HttpContext.Request.Form["Email"];

			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "[PR_Student_Search]";

			command.Parameters.AddWithValue("@StudentName", StudentName);
			command.Parameters.AddWithValue("@BranchName", BranchName);
			command.Parameters.AddWithValue("@CityName", CityName);
			command.Parameters.AddWithValue("@Age", Age);
			command.Parameters.AddWithValue("@Email", Email);


			SqlDataReader reader = command.ExecuteReader();
			DataTable table = new DataTable();
			table.Load(reader);
			connection.Close();
			return View("StudentList", table);
		}
		#endregion
	}
}
