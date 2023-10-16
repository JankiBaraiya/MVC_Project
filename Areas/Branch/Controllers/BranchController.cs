using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using MVC_Project.Areas.Branch.Models;

namespace MVC_Project.Areas.Branch.Controllers
{
	[Area("Branch")]
	[Route("Branch/{controller}/{action}/{id?}")]
	public class BranchController : Controller
	{
		private IConfiguration Configuration;

		#region Con
		public BranchController(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		#endregion

		#region SelectAll
		public IActionResult BranchList()
		{
			DataTable dt = new DataTable();
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "PR_Branch_SelectAll";
			SqlDataReader reader = command.ExecuteReader();

			if (reader.HasRows)
			{
				dt.Load(reader);
			}

			return View(dt);
			
		}
        #endregion
        #region Delete
        public IActionResult Delete(int BranchID)
        {
            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Branch_DeleteByPK";
            command.Parameters.AddWithValue("@BranchID", BranchID);
            command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("BranchList");
        }
        #endregion

        #region AddEdit
        public IActionResult Branch_AddEdit(int? BranchID)
        {
            if (BranchID != null)
            {
                string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Branch_SelectAllByPK";
                command.Parameters.AddWithValue("@BranchID", BranchID);
                DataTable dt = new DataTable();
                SqlDataReader reader = command.ExecuteReader();
                dt.Load(reader);

                BranchModel BranchModel = new BranchModel();
                foreach (DataRow dr in dt.Rows)
                {
                    BranchModel.BranchID = Convert.ToInt32(dr["BranchID"]);
                    BranchModel.BranchName = dr["BranchName"].ToString();
                    BranchModel.BranchCode = dr["BranchCode"].ToString();
                }
                return View(BranchModel);

            }
            return View();
        }
        #endregion

        #region Save


        public IActionResult Save(BranchModel model)
        {
            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (model.BranchID == null)
            {
                command.CommandText = "PR_Branch_Insert";
            }
            else
            {
                command.CommandText = "PR_Branch_UpdateByPK";
                command.Parameters.AddWithValue("@BranchID", model.BranchID);
            }
            command.Parameters.AddWithValue("@BranchName", SqlDbType.VarChar).Value = model.BranchName;
            command.Parameters.AddWithValue("@BranchCode", SqlDbType.VarChar).Value = model.BranchCode;


            if (Convert.ToBoolean(command.ExecuteNonQuery()))
            {
                if (model.BranchID == null)
                    TempData["Message"] = "Record Inserted Successfully";
                else
                {
                    TempData["Message"] = "Record Updated Successfully";
                    return RedirectToAction("BranchList");
                }
            }

            connection.Close();

            return RedirectToAction("BranchList");
        }
        #endregion


        #region Filtter
        public IActionResult Filtter()
		{
			string BranchName = HttpContext.Request.Form["BranchName"];
			string BranchCode = HttpContext.Request.Form["BranchCode"];

			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "[PR_Branch_Search]";

			command.Parameters.AddWithValue("@BranchName", BranchName);
			command.Parameters.AddWithValue("@BranchCode", BranchCode);


			SqlDataReader reader = command.ExecuteReader();
			DataTable table = new DataTable();
			table.Load(reader);
			connection.Close();
			return View("BranchList", table);
		}
		#endregion
	}
}
