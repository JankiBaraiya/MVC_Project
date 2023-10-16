using Microsoft.AspNetCore.Mvc;
using MVC_Project.Areas.LOC_City.Models;
using MVC_Project.Areas.LOC_State.Models;
using System.Data;
using System.Data.SqlClient;
using static MVC_Project.Areas.LOC_Country.Models.LOC_CountryModel;

namespace MVC_Project.Areas.LOC_City.Controllers
{
	[Area("LOC_City")]
	[Route("LOC_City/{controller}/{action}/{id?}")]
	public class LOC_CityController : Controller
	{
		private IConfiguration Configuration;

		#region Con
		public LOC_CityController(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		#endregion

		#region SelectAll
		public IActionResult CityList()
		{
			DataTable dt = new DataTable();
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "PR_City_SelectAll";
			SqlDataReader reader = command.ExecuteReader();

			if (reader.HasRows)
			{
				dt.Load(reader);
			}

			return View(dt);
		}
		#endregion
		#region Delete
		public IActionResult Delete(int CityID)
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "PR_City_DeleteByPK";
			command.Parameters.AddWithValue("@CityID", CityID);
			command.ExecuteNonQuery();
			connection.Close();
			return RedirectToAction("CityList");
		}
		#endregion
		#region City_AddEdit
		public IActionResult City_AddEdit(int? CityID)
		{
			FillCountryDDL();
			FillStateDDL();
			if (CityID != null)
			{
				string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
				SqlConnection connection = new SqlConnection(connectionString);
				connection.Open();
				SqlCommand objCmd = connection.CreateCommand();
				objCmd.CommandType = CommandType.StoredProcedure;
				objCmd.CommandText = "PR_City_SelectByPK";
				objCmd.Parameters.Add("@CityID", SqlDbType.Int).Value = CityID;
				SqlDataReader reader = objCmd.ExecuteReader();
				DataTable table = new DataTable();
				table.Load(reader);

				LOC_CityModel lOC_CityModel = new LOC_CityModel();
				foreach (DataRow dr in table.Rows)
				{
					lOC_CityModel.CityName = @dr["CityName"].ToString();
					lOC_CityModel.Citycode = @dr["CityCode"].ToString();
					lOC_CityModel.CountryID = Convert.ToInt32(@dr["CountryID"]);
					lOC_CityModel.StateID = Convert.ToInt32(@dr["StateID"]);

				}
				return View(lOC_CityModel);
			}

			return View();
		}
		#endregion

		#region Save
		[HttpPost]
		public IActionResult Save(LOC_CityModel model)
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand objCmd = connection.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			if (model.CityID == null)
			{
				objCmd.CommandText = "PR_City_Insert";
			}
			else
			{
				objCmd.CommandText = "PR_City_UpdateByPK";
				objCmd.Parameters.Add("@CityID", SqlDbType.Int).Value = model.CityID;
			}
			objCmd.Parameters.Add("@CityName", SqlDbType.VarChar).Value = model.CityName;
			objCmd.Parameters.Add("@CityCode", SqlDbType.VarChar).Value = model.Citycode;
			objCmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = model.CountryID;
			objCmd.Parameters.Add("@StateID", SqlDbType.Int).Value = model.StateID;

			objCmd.ExecuteNonQuery();
			connection.Close();
			return RedirectToAction("CityList");
		}
		#endregion
		#region FillCountryDDL
		public void FillCountryDDL()
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			List<LOC_CountryDropDownModel> loc_Country = new List<LOC_CountryDropDownModel>();
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand objCmd = connection.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			objCmd.CommandText = "PR_Country_SelectForDropDown";
			SqlDataReader objSDR = objCmd.ExecuteReader();
			if (objSDR.HasRows)
			{
				while (objSDR.Read())
				{
					LOC_CountryDropDownModel country = new LOC_CountryDropDownModel()
					{
						CountryID = Convert.ToInt32(objSDR["CountryID"]),
						CountryName = objSDR["CountryName"].ToString()
					};
					loc_Country.Add(country);
				}
				objSDR.Close();
			}
			connection.Close();
			ViewBag.CountryList = loc_Country;

		}
		#endregion

		#region FillStateDDL
		public void FillStateDDL()
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			List<LOC_StateDropDownModel> loc_State = new List<LOC_StateDropDownModel>();
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand objCmd = connection.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			objCmd.CommandText = "PR_State_SelectForDropDown";
			SqlDataReader objSDR = objCmd.ExecuteReader();
			if (objSDR.HasRows)
			{
				while (objSDR.Read())
				{
					LOC_StateDropDownModel state = new LOC_StateDropDownModel()
					{
						StateID = Convert.ToInt32(objSDR["StateID"]),
						StateName = objSDR["StateName"].ToString()
					};
					loc_State.Add(state);
				}
				objSDR.Close();
			}
			connection.Close();
			ViewBag.StateList = loc_State;

		}
		#endregion


		#region selectStateByCountry
		[HttpPost]
		public IActionResult StateDropDownByCountryID(int CountryID)
		{
			string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			List<LOC_StateDropDownModel> loc_State = new List<LOC_StateDropDownModel>();
			SqlConnection connection = new SqlConnection(connectionString);

			//open connection and create command object.
			connection.Open();
			SqlCommand objCmd = connection.CreateCommand();
			objCmd.CommandType = CommandType.StoredProcedure;
			objCmd.CommandText = "PR_StateSelectByCountry";
			objCmd.Parameters.AddWithValue("@CountryID", CountryID);

			SqlDataReader objSDR = objCmd.ExecuteReader();
			if (objSDR.HasRows)
			{
				while (objSDR.Read())
				{
					LOC_StateDropDownModel vlst = new LOC_StateDropDownModel()
					{
						StateID = Convert.ToInt32(objSDR["StateID"]),
						StateName = objSDR["StateName"].ToString()
					};
					loc_State.Add(vlst);
				}
				objSDR.Close();
			}
			connection.Close();
			var vModel = loc_State;
			return Json(vModel);
		}

		#endregion

		#region Filtter
		public IActionResult Filtter()
		{
			string CityName = HttpContext.Request.Form["CityName"];
			string CityCode = HttpContext.Request.Form["CityCode"];
            string CountryName = HttpContext.Request.Form["CountryName"];
            string StateName = HttpContext.Request.Form["StateName"];

            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
			SqlConnection connection = new SqlConnection(connectionString);
			connection.Open();
			SqlCommand command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "[PR_City_Search]";

			command.Parameters.AddWithValue("@CityName", CityName);
			command.Parameters.AddWithValue("@CityCode", CityCode);
            command.Parameters.AddWithValue("@CountryName", CountryName);
            command.Parameters.AddWithValue("@StateName", StateName);


            SqlDataReader reader = command.ExecuteReader();
			DataTable table = new DataTable();
			table.Load(reader);
			connection.Close();
			return View("CityList", table);
		}
		#endregion
	}
}
