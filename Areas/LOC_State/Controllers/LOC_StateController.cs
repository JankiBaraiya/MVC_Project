using Microsoft.AspNetCore.Mvc;
using MVC_Project.Areas.LOC_State.Models;
using static MVC_Project.Areas.LOC_Country.Models.LOC_CountryModel;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace MVC_Project.Areas.LOC_State.Controllers
{
    [Area("LOC_State")]
    [Route("LOC_State/{controller}/{action}/{id?}")]
    public class LOC_StateController : Controller
    {
        private IConfiguration Configuration;

        #region con
        public LOC_StateController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion

        #region SelectAll
        public IActionResult StateList()
        {
            FillCountryDDL();
            DataTable dt = new DataTable();
            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_State_SelectAll";
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                dt.Load(reader);
            }
            return View(dt);
        }

        #endregion

        #region Delete

        public IActionResult Delete(int StateID)
        {
            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_State_DeleteByPK";
            command.Parameters.AddWithValue("@StateID", StateID);
            command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("StateList");
        }
        #endregion

        #region AddEdit
        public IActionResult State_AddEdit(int? StateID)
        {
            FillCountryDDL();
            if (StateID != null)
            {
                string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_State_SelectByPK";
                command.Parameters.AddWithValue("@StateID", StateID);
                DataTable dt = new DataTable();
                SqlDataReader reader = command.ExecuteReader();
                dt.Load(reader);
                LOC_StateModel loc_State_Model = new LOC_StateModel();
                foreach (DataRow dr in dt.Rows)
                {
                    loc_State_Model.StateName = @dr["StateName"].ToString();
                    loc_State_Model.StateCode = @dr["StateCode"].ToString();
                    loc_State_Model.CountryID = Convert.ToInt32(@dr["CountryID"]);
                }
                return View("State_AddEdit", loc_State_Model);
            }
            return View("State_AddEdit");
        }

        #endregion

        #region Save
        [HttpPost]
        public IActionResult Save(LOC_StateModel model)
        {
            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand objCmd = connection.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            if (model.StateID == null)
            {
                objCmd.CommandText = "PR_State_Insert";
            }
            else
            {
                objCmd.CommandText = "PR_State_UpdateByPK";
                objCmd.Parameters.Add("@stateID", SqlDbType.Int).Value = model.StateID;
            }
            objCmd.Parameters.Add("@StateName", SqlDbType.VarChar).Value = model.StateName;
            objCmd.Parameters.Add("@StateCode", SqlDbType.VarChar).Value = model.StateCode;
            objCmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = model.CountryID;

            objCmd.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("StateList");
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

        #region Filtter

        public IActionResult Filtter()
        {
            string StateName = HttpContext.Request.Form["StateName"];
            string StateCode = HttpContext.Request.Form["StateCode"];
            string CountryName = HttpContext.Request.Form["CountryName"];

            string connectionString = this.Configuration.GetConnectionString("myConnectionStrings");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[PR_State_Search]";

            command.Parameters.AddWithValue("@StateName", StateName);
            command.Parameters.AddWithValue("@StateCode", StateCode);
            command.Parameters.AddWithValue("@CountryName", CountryName);


            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            return View("StateList", table);
        }
        #endregion
    }
}
