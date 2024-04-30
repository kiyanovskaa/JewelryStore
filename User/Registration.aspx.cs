using Npgsql;
using Npgsql.Internal.Postgres;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace db_work.User
{
    public partial class Registration : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)// && Session["user_id"] != null)
                {
                    getUserDetails();
                }
                else if (Session["user_id"] != null)
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {


            string actionName = string.Empty, imagePath = string.Empty, fileExtension = string.Empty;
            bool isValidToExecute = false;
            int userId = Convert.ToInt32(Request.QueryString["id"]);
            con = new NpgsqlConnection(Connection.GetConnectionString());
            cmd = new NpgsqlCommand("User_Crud", con);
            cmd.Parameters.AddWithValue("@action", userId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@userid", userId);
            cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@un", txtUsername.Text.Trim());
            cmd.Parameters.AddWithValue("@mail", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@addr", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@passw", txtPassword.Text.Trim());
            cmd.Parameters.AddWithValue("@countryid", Convert.ToInt32(ddlCountries.SelectedValue));////////////////////////////////////////////////////////////



            isValidToExecute = true;
           


            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = userId == 0 ?
                    "registration is successful! <b><a href='Login.aspx'> Click here </a></b>to do login" :
                    "details updated successful! <b><a href='Profile.aspx'>Can check here</a> </b>";
                    lblMsg.Visible = true;
                    lblMsg.Text = "<b>" + txtUsername.Text.Trim() + "</b> " + actionName; //"Registration is successf
                    lblMsg.CssClass = "alert alert-success";
                    if (userId != 0)
                    {
                        Response.AddHeader("REFRESH", "1;URL=Profile.aspx");
                    }

                    clear();
                }
                catch (NpgsqlException ex)
                {
                    if (ex.Message.Contains("violates unique constraint"))
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "<b>" + txtUsername.Text.Trim() + "</b> username already exist, try new one..!";
                        lblMsg.CssClass = "alert alert-danger";
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Error-" + ex.Message;
                    lblMsg.CssClass = "alert alert-danger";
                }
                finally { con.Close(); }
            }
        }



        void getUserDetails()
        {
            /*
             * 
             
                 adapter = new NpgsqlDataAdapter();
                adapter.TableMappings.Add("Table", "Category");
                string sid = (e.CommandArgument).ToString();
                string queryString = $"SELECT * FROM \"Category\" WHERE category_id = {sid}";
                NpgsqlCommand com = new NpgsqlCommand(queryString, con);
                DataSet dataSet = new DataSet();
                adapter.SelectCommand = com;
                adapter.Fill(dataSet);
             */
            con = new NpgsqlConnection(Connection.GetConnectionString());
            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "User");
            string uid = Request.QueryString["id"];

            string queryString = $"SELECT * FROM \"User\" WHERE user_id = {uid}";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);

            /* cmd = new NpgsqlCommand("User_Crud", con);
             cmd.Parameters.AddWithValue("@action", "SELECT4PROFILE"); cmd.Parameters.AddWithValue("@UserId", Request.QueryString["id"]); cmd.CommandType = CommandType.StoredProcedure;
             sda = new SqlDataAdapter(cmd);
             dt = new DataTable();
             sda.Fill(dt);*/
            if (dataSet.Tables[0].Rows.Count == 1)
            {
                txtName.Text = dataSet.Tables[0].Rows[0]["user_name"].ToString();
                txtUsername.Text = dataSet.Tables[0].Rows[0]["username"].ToString(); 
                txtEmail.Text = dataSet.Tables[0].Rows[0]["email"].ToString(); 
                txtAddress.Text = dataSet.Tables[0].Rows[0]["address"].ToString();
                
                txtPassword.TextMode = TextBoxMode.SingleLine;
                txtPassword.ReadOnly = true;
                txtPassword.Text = dataSet.Tables[0].Rows[0]["password"].ToString();
               
            }
            lblHeaderMsg.Text = "Edit Profile";
            btnRegister.Text = "Update";
            lblAlreadyUser.Text = "";
        }

        private void clear()
        {
            txtName.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }
    }
}