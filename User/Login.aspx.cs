using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace db_work.User
{
    public partial class Login : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userid"] != null)
            {
                Response.Redirect("Default.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim() == "Admin" && txtPassword.Text.Trim() == "123")
            {
                Session["admin"] = txtUsername.Text.Trim();
                Response.Redirect("../Admin/Dashboard.aspx");
            }
            else
            {
                con = new NpgsqlConnection(Connection.GetConnectionString());


                con.Open();

                adapter = new NpgsqlDataAdapter();
                adapter.TableMappings.Add("Table", "User");
                string name=txtUsername.Text.Trim();
                string password=txtPassword.Text.Trim();
                string queryString = $"SELECT * FROM \"User\" where user_name=\'{name}\' and password=\'{password}\'";
                NpgsqlCommand com = new NpgsqlCommand(queryString, con);
                DataSet dataSet = new DataSet();
                adapter.SelectCommand = com;
               // if()
                adapter.Fill(dataSet);

                if (dataSet.Tables[0].Rows.Count == 1)
                {
                    Session["username"]=txtUsername.Text.Trim();
                    Session["userid"] = dataSet.Tables[0].Rows[0]["user_id"];
                    Response.Redirect("Default.aspx");
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Invalid Credentials";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
        }
    }
}