using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace db_work.Admin
{
    public partial class Material : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                Session["breadCrum"] = "Material";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../User/Login.aspx");
                }
                else
                {
                    getMaterials();

                }
            }
            lblMsg.Visible = false;

        }

      
        private void getMaterials()
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());


            con.Open();

            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Material");
            string queryString = "SELECT * FROM \"Materials\" ";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rMaterial.DataSource = dataSet.Tables["Material"].DefaultView;
            rMaterial.DataBind();
        }
        protected void rMaterial_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());
            lblMsg.Visible = false;
            if (e.CommandName == "edit")
            {
                adapter = new NpgsqlDataAdapter();
                adapter.TableMappings.Add("Table", "Material");
                string sid = (e.CommandArgument).ToString();
                string queryString = $"SELECT * FROM \"Materials\"  WHERE material_id = {sid}";
                NpgsqlCommand com = new NpgsqlCommand(queryString, con);
                DataSet dataSet = new DataSet();
                adapter.SelectCommand = com;
                adapter.Fill(dataSet);


                /*cmd = new NpgsqlCommand("Category_Crud", con);
                cmd.Parameters.AddWithValue("@action", "GETBYID");
                cmd.Parameters.AddWithValue("@categoryid", Convert.ToInt32(e.CommandArgument));
                cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                cmd.Parameters.Add("@isactive", NpgsqlTypes.NpgsqlDbType.Bit).Value = cbIsActive.Checked;
                cmd.Parameters.AddWithValue("@imageurl", "ss");
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new NpgsqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);*/
                txtName.Text = dataSet.Tables[0].Rows[0]["material_name"].ToString();
               
                hdnId.Value = dataSet.Tables[0].Rows[0]["material_id"].ToString();
                btnAddOrUpdate.Text = "Update";
                LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                btn.CssClass = "badge badge-warning";
            }
            else if (e.CommandName == "delete")
            {
                cmd = new NpgsqlCommand("Material_Crud", con);
                cmd.Parameters.AddWithValue("@action", "DELETE");
                cmd.Parameters.AddWithValue("@materialid", Convert.ToInt32(e.CommandArgument));


                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "Category deleted successfully!";
                    lblMsg.CssClass = "alert alert-success";
                    getMaterials();
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

        protected void rMaterial_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }
        private void clear()
        {
            txtName.Text = string.Empty;
            hdnId.Value = "0";
            btnAddOrUpdate.Text = "Add";
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExt = string.Empty;
            bool isValidToExecute = false;
            int categoryId = Convert.ToInt32(hdnId.Value);
            int imageid = 0;
            con = new NpgsqlConnection(Connection.GetConnectionString());
            cmd = new NpgsqlCommand("Material_Crud", con);
            cmd.Parameters.AddWithValue("@action", categoryId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@materialid", categoryId);
            cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                  con.Open();
                cmd.ExecuteNonQuery();
                actionName = categoryId == 0 ? "inserted" : "updated";
                lblMsg.Visible = true;
                lblMsg.Text = "Category " + actionName + " successfully";
                lblMsg.CssClass = "alert alert-success";
                getMaterials();
                clear();

            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Error - " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
            finally
            {
                con.Close();
            }
        }
    }
}