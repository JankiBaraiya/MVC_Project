using Microsoft.AspNetCore.Mvc;
using MVC_Project.Areas.LOC_Country.Models;
using System.Data;
using System.Data.SqlClient;

namespace MVC_Project.Areas.LOC_Country.Controllers
{
	[Area("LOC_Country")]
	[Route("LOC_Country/{controller}/{action}/{id?}")]
	public class LOC_CountryController : Controller
	{
		private IConfiguration Configuration;

		#region con
		public LOC_CountryController(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		#endregion

		#region SelectAll
		public IActionResult CountryList()
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "PR_Country_SelectAll";
			SqlDataReader reader = command.ExecuteReader();
			DataTable table = new DataTable();
			table.Load(reader);
			connection.Close();
			return View("CountryList", table);
		}
		#endregion

		#region Delete
		public IActionResult Delete(int CountryID)
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "PR_Country_DeleteByPK";
			command.Parameters.AddWithValue("@CountryID", CountryID);
			command.ExecuteNonQuery();
			connection.Close();
			return RedirectToAction("CountryList");
		}
		#endregion

		#region AddEdit
		public IActionResult Country_AddEdit(int? CountryID)
		{
			if (CountryID != null)
			{
				string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
				SqlConnection connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand command = connection.CreateCommand();
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = "PR_Country_SelectByPK";
				command.Parameters.AddWithValue("@CountryID", CountryID);
				DataTable dt = new DataTable();
				SqlDataReader reader = command.ExecuteReader();
				dt.Load(reader);

				LOC_CountryModel lOC_CountryModel = new LOC_CountryModel();
				foreach (DataRow dr in dt.Rows)
				{
					lOC_CountryModel.CountryID = Convert.ToInt32(dr["CountryID"]);
					lOC_CountryModel.CountryName = dr["CountryName"].ToString();
					lOC_CountryModel.CountryCode = dr["CountryCode"].ToString();
				}
				return View(lOC_CountryModel);

			}
			return View();
		}
		#endregion

		#region Save


		public IActionResult Save(LOC_CountryModel model)
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;

			if (model.CountryID == 0)
			{
				command.CommandText = "PR_Country_Insert";
			}
			else
			{
				command.CommandText = "PR_Country_UpdateByPK";
				command.Parameters.AddWithValue("@CountryID", model.CountryID);
			}
			command.Parameters.AddWithValue("@CountryName", SqlDbType.VarChar).Value = model.CountryName;
			command.Parameters.AddWithValue("@CountryCode", SqlDbType.VarChar).Value = model.CountryCode;


			if (Convert.ToBoolean(command.ExecuteNonQuery()))
			{
				if (model.CountryID == null)
					TempData["Message"] = "Record Inserted Successfully";
				else
				{
					TempData["Message"] = "Record Updated Successfully";
					return RedirectToAction("CountryList");
				}
			}

			connection.Close();

			return RedirectToAction("CountryList");
		}
		#endregion

		#region Filtter

		public IActionResult Filtter()
        {
            string CountryName = HttpContext.Request.Form["CountryName"];
            string CountryCode = HttpContext.Request.Form["CountryCode"];

            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[dbo].[PR_Country_Search]";

            command.Parameters.AddWithValue("@CountryName", SqlDbType.VarChar).Value = CountryName;
            command.Parameters.AddWithValue("@CountryCode", SqlDbType.VarChar).Value = CountryCode;


            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            return View("CountryList", table);
        }
        #endregion

    }
}
