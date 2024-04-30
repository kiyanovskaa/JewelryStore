using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace db_work.Admin
{
    public partial class Users : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                Session["breadCrum"] = "Users";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../User.Login.aspx");
                }
                else
                {
                    getUsers();

                }
            }

        }

        private void getUsers()
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());


            con.Open();

            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "User");
            string queryString = "select ROW_NUMBER() OVER(ORDER BY (select 1)) as srno, user_id, user_name, username, email, createddate from \"User\"";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rUser.DataSource = dataSet.Tables["User"].DefaultView;
            rUser.DataBind();



        }
        protected void rUser_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            if (e.CommandName == "delete")
            {
                con = new NpgsqlConnection(Connection.GetConnectionString());
                cmd = new NpgsqlCommand("User_Crud", con);
                cmd.Parameters.AddWithValue("@action", "DELETE");
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt32(e.CommandArgument));
                //cmd.Parameters.AddWithValue("@categoryname", txtName.Text.Trim());
                //cmd.Parameters.Add("@active", NpgsqlTypes.NpgsqlDbType.Bit).Value = cbIsActive.Checked;
                //cmd.Parameters.AddWithValue("@image", "ss");


                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "User deleted successfully!";
                    lblMsg.CssClass = "alert alert-success";
                    getUsers();
                }

                catch (Exception ex)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Error-" + ex.Message;
                    lblMsg.CssClass = "alert alert-danger";
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}