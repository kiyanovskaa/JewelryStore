using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace db_work.User
{
    public partial class Profile : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    getUserDetails();
                }
            }

        }
        void getUserDetails()
        {
 
            con = new NpgsqlConnection(Connection.GetConnectionString());
            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "User");
            string uid = Session["userId"].ToString();

            string queryString = $"SELECT u.*, c.country as country FROM \"User\" u  inner join \"Countries\" c on c.country_id=u.country_id WHERE user_id = {uid}";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rUserProfile.DataSource = dataSet.Tables["User"].DefaultView;
            rUserProfile.DataBind();
   
            if (dataSet.Tables[0].Rows.Count == 1)
            {
                Session["name"] = dataSet.Tables[0].Rows[0]["user_name"].ToString();
             //   Session["username"] = dataSet.Tables[0].Rows[0]["username"].ToString();
                Session["email"] = dataSet.Tables[0].Rows[0]["email"].ToString();
                Session["createdDate"] = dataSet.Tables[0].Rows[0]["createddate"].ToString();

            }
           
        }
    }
}