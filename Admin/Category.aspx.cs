using Microsoft.SqlServer.Server;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace db_work.Admin
{
    public partial class Category : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                Session["breadCrum"] = "Category";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../User/Login.aspx");
                }
                else
                {
                    getCategories();

                }
            }
            lblMsg.Visible = false;
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExt = string.Empty;
            bool isValidToExecute = false;
            int categoryId = Convert.ToInt32(hdnId.Value);
            int imageid = 0;
            con = new NpgsqlConnection(Connection.GetConnectionString());

            if (fuCategoryImage.HasFile)
            {
                if (Utils.IsValidExtension(fuCategoryImage.FileName))
                {
                    Guid obj = Guid.NewGuid();
                    fileExt = Path.GetExtension(fuCategoryImage.FileName);
                    imagePath = "Images/Category/" + obj.ToString() + fileExt;
                    fuCategoryImage.PostedFile.SaveAs(Server.MapPath("~/Images/Category/") + obj.ToString() + fileExt);
                    cmd = new NpgsqlCommand("INSERT_IMG", con);

                    //   cmd.Parameters.AddWithValue("@imageurl", "dddd");
                    cmd.Parameters.AddWithValue("@imgurl", imagePath);
                    cmd.Parameters.AddWithValue("@imgurlid", NpgsqlDbType.Bigint);
                    cmd.Parameters["imgurlid"].Direction = ParameterDirection.Output;


                    cmd.CommandType = CommandType.StoredProcedure;


                    isValidToExecute = true;
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        imageid = Convert.ToInt32(cmd.Parameters["imgurlid"].Value);



                    }
                    catch (Exception ex)
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Error - " + ex.Message;
                        lblMsg.CssClass = "alert alert-danger";
                    }
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Please select image";
                    lblMsg.CssClass = "alert alert-danger";
                    isValidToExecute = false;
                }
            }
            else
            {
                isValidToExecute = true;
              //  cmd.Parameters.AddWithValue("@image", imgCategory.ImageUrl);


            }

            cmd = new NpgsqlCommand("Category_Crud", con);
            cmd.Parameters.AddWithValue("@action", categoryId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@categoryid", categoryId);
            cmd.Parameters.AddWithValue("@imageurlid", imageid);

            cmd.Parameters.AddWithValue("@categoryname", txtName.Text.Trim());
            cmd.Parameters.Add("@active", NpgsqlTypes.NpgsqlDbType.Bit).Value = cbIsActive.Checked;




            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                  //  con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = categoryId == 0 ? "inserted" : "updated";
                    lblMsg.Visible = true;
                    lblMsg.Text = "Category " + actionName + " successfully";
                    lblMsg.CssClass = "alert alert-success";
                    getCategories();
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

        private void getCategories()
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());


            con.Open();

            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Category");
            string queryString = "SELECT c.*, i.url as imageurl FROM \"Category\" c inner join \"ImageUrls\" i on c.imageurl_id=i.imageurl_id";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rCategory.DataSource = dataSet.Tables["Category"].DefaultView;
            rCategory.DataBind();



        }

        private void clear()
        {
            txtName.Text = string.Empty;
            cbIsActive.Checked = false;
            hdnId.Value = "0";
            btnAddOrUpdate.Text = "Add";
            imgCategory.ImageUrl = string.Empty;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        protected void rCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());
            lblMsg.Visible = false;
            if (e.CommandName == "edit")
            {



                adapter = new NpgsqlDataAdapter();
                adapter.TableMappings.Add("Table", "Category");
                string sid = (e.CommandArgument).ToString();
                string queryString = $"SELECT c.*, i.url as imageurl FROM \"Category\" c inner join \"ImageUrls\" i on c.imageurl_id=i.imageurl_id WHERE category_id = {sid}";
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
                txtName.Text = dataSet.Tables[0].Rows[0]["name"].ToString();
                cbIsActive.Checked = Convert.ToBoolean(dataSet.Tables[0].Rows[0]["isactive"]);
                imgCategory.ImageUrl = string.IsNullOrEmpty(dataSet.Tables[0].Rows[0]["imageurl"].ToString()) ?
                     "../Images/No_image.png" : "../" + dataSet.Tables[0].Rows[0]["imageurl"].ToString();
                imgCategory.Height = 200;
                imgCategory.Width = 200;
                hdnId.Value = dataSet.Tables[0].Rows[0]["category_id"].ToString();
                btnAddOrUpdate.Text = "Update";
                LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                btn.CssClass = "badge badge-warning";
            }
            else if (e.CommandName == "delete")
            {
                cmd = new NpgsqlCommand("Category_Crud", con);
                cmd.Parameters.AddWithValue("@action", "DELETE");
                cmd.Parameters.AddWithValue("@categoryid", Convert.ToInt32(e.CommandArgument));
                cmd.Parameters.AddWithValue("@categoryname", txtName.Text.Trim());
                cmd.Parameters.Add("@active", NpgsqlTypes.NpgsqlDbType.Bit).Value = cbIsActive.Checked;


                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "Category deleted successfully!";
                    lblMsg.CssClass = "alert alert-success";
                    getCategories();
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

        protected void rCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lbl = e.Item.FindControl("lblIsActive") as Label;
                if (lbl.Text == "True")
                {
                    lbl.Text = "Active";
                    lbl.CssClass = "badge badge-success";
                }
                else
                {
                    lbl.Text = "In-Active";
                    lbl.CssClass = "badge badge-danger";
                }
            }
        }
    }
}