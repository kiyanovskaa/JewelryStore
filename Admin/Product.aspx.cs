using db_work.User;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace db_work.Admin
{
    public partial class Product : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Product";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../User.Login.aspx");
                }
                else
                {
                    getProducts();

                }
            }
            lblMsg.Visible = false;
        }


        private void getProducts()
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());


            con.Open();

            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Product");
            string queryString = "SELECT p.*, c.name as CategoryName, m.material_name as MaterialName,  i.url as imageurl FROM \"Product\" p inner join \"Category\" c on c.category_id=p.category_id  inner join \"ImageUrls\" i  on p.imageurl_id=i.imageurl_id inner join \"Materials\" m on m.material_id=p.material_id";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rProduct.DataSource = dataSet.Tables["Product"].DefaultView;
            rProduct.DataBind();


        }
        private void clear()
        {
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtPrice.Text = string.Empty;
            ddlCategories.ClearSelection();
            cbIsActive.Checked = false;
            hdnId.Value = "0";
            btnAddOrUpdate.Text = "Add";
            imgProduct.ImageUrl = string.Empty;
        }


        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        protected void rProduct_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());
            lblMsg.Visible = false;
            if (e.CommandName == "edit")
            {

                adapter = new NpgsqlDataAdapter();
                adapter.TableMappings.Add("Table", "Product");
                string sid = (e.CommandArgument).ToString();
                string queryString = $"SELECT p.*, i.url as imageurl  FROM \"Product\" p  inner join \"ImageUrls\" i  on p.imageurl_id=i.imageurl_id WHERE product_id = {sid}";
                NpgsqlCommand com = new NpgsqlCommand(queryString, con);
                DataSet dataSet = new DataSet();
                adapter.SelectCommand = com;
                adapter.Fill(dataSet);


                /*cmd = new NpgsqlCommand("Category_Crud", con);
                cmd.Parameters.AddWithValue("@action", "GETBYID");
                cmd.Parameters.AddWithValue("@categoryid", Convert.ToInt32(e.CommandArgument));
                cmd.Parameters.AddWithValue("@name", txtNameProductText.Trim());
                cmd.Parameters.Add("@isactive", NpgsqlTypes.NpgsqlDbType.Bit).Value = cbIsActive.Checked;
                cmd.Parameters.AddWithValue("@imageurl", "ss");
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new NpgsqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);*/

                txtName.Text = dataSet.Tables[0].Rows[0]["name"].ToString();
                txtDescription.Text = dataSet.Tables[0].Rows[0]["description"].ToString();
                txtPrice.Text = dataSet.Tables[0].Rows[0]["price"].ToString();
                txtQuantity.Text = dataSet.Tables[0].Rows[0]["quantity"].ToString();
                ddlCategories.SelectedValue = dataSet.Tables[0].Rows[0]["category_id"].ToString();
                ddlMaterial.SelectedValue = dataSet.Tables[0].Rows[0]["material_id"].ToString();
                cbIsActive.Checked = Convert.ToBoolean(dataSet.Tables[0].Rows[0]["isactive"]);
                imgProduct.ImageUrl = string.IsNullOrEmpty(dataSet.Tables[0].Rows[0]["imageurl"].ToString()) ?
                     "../Images/No_image.png" : "../" + dataSet.Tables[0].Rows[0]["imageurl"].ToString();
                imgProduct.Height = 200;
                imgProduct.Width = 200;
                hdnId.Value = dataSet.Tables[0].Rows[0]["product_id"].ToString();
                btnAddOrUpdate.Text = "Update";
                LinkButton btn = e.Item.FindControl("lnkEdit") as LinkButton;
                btn.CssClass = "badge badge-warning";
            }
            else if (e.CommandName == "delete")
            {
                cmd = new NpgsqlCommand("Product_Crud", con);
                cmd.Parameters.AddWithValue("@action", "DELETE");
                cmd.Parameters.AddWithValue("@productid", Convert.ToInt32(e.CommandArgument));
                cmd.Parameters.AddWithValue("@productname", "dd");
                cmd.Parameters.AddWithValue("@descr", "ll");
                cmd.Parameters.AddWithValue("@pricep", 4.5);
              


                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lblMsg.Visible = true;
                    lblMsg.Text = "Category deleted successfully!";
                    lblMsg.CssClass = "alert alert-success";
                    getProducts();
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

        protected void rProduct_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblIsActive = e.Item.FindControl("lblIsActive") as Label;
                Label lblQuantity = e.Item.FindControl("lblQuantity") as Label;
                if (lblIsActive.Text == "True")
                {
                    lblIsActive.Text = "Active";
                    lblIsActive.CssClass = "badge badge-success";
                }
                else
                {
                    lblIsActive.Text = "In-Active";
                    lblIsActive.CssClass = "badge badge-danger";
                }
                if (Convert.ToInt32(lblQuantity.Text) <= 5)
                {
                    lblQuantity.CssClass = "badge badge-danger";
                    lblQuantity.ToolTip = "Item about to be 'Out of stock'!";
                }
            }
        }

        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExt = string.Empty;
            bool isValidToExecute = false;
            int productId = Convert.ToInt32(hdnId.Value);
            int imageid = 0;
            con = new NpgsqlConnection(Connection.GetConnectionString());
            con.Open();
            if (fuProductImage.HasFile)
            {
                if (Utils.IsValidExtension(fuProductImage.FileName))
                {
                    Guid obj = Guid.NewGuid();
                    fileExt = Path.GetExtension(fuProductImage.FileName);
                    imagePath = "Images/Product/" + obj.ToString() + fileExt;
                    fuProductImage.PostedFile.SaveAs(Server.MapPath("~/Images/Product/") + obj.ToString() + fileExt);
                    // cmd.Parameters.Add("@imageurl", NpgsqlTypes.NpgsqlDbType.Text).Value = imagePath;
                    cmd = new NpgsqlCommand("INSERT_IMG", con);

                    //   cmd.Parameters.AddWithValue("@imageurl", "dddd");
                    cmd.Parameters.AddWithValue("@imgurl", imagePath);
                    cmd.Parameters.AddWithValue("@imgurlid", NpgsqlDbType.Bigint);
                    cmd.Parameters["imgurlid"].Direction = ParameterDirection.Output;


                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {

                        cmd.ExecuteNonQuery();
                        imageid = Convert.ToInt32(cmd.Parameters["imgurlid"].Value);



                    }
                    catch (Exception ex)
                    {
                        lblMsg.Visible = true;
                        lblMsg.Text = "Error - " + ex.Message;
                        lblMsg.CssClass = "alert alert-danger";
                    }
                    isValidToExecute = true;
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
                // cmd.Parameters.AddWithValue("@image", imgProduct.ImageUrl);



            }

            cmd = new NpgsqlCommand("Product_Crud", con);
            cmd.Parameters.AddWithValue("@action", productId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@productid", Convert.ToInt32(productId));
            cmd.Parameters.AddWithValue("@productname", txtName.Text.Trim());
            cmd.Parameters.AddWithValue("@descr", txtDescription.Text.Trim());
            cmd.Parameters.AddWithValue("@pricep", Convert.ToDouble(txtPrice.Text.Trim()));
            cmd.Parameters.AddWithValue("@quant", Convert.ToInt32(txtQuantity.Text.Trim()));
            cmd.Parameters.AddWithValue("@imageurlid", imageid);
            cmd.Parameters.AddWithValue("@categoryid", Convert.ToInt32(ddlCategories.SelectedValue));////////////////////////////////////////////////////////////
            cmd.Parameters.AddWithValue("@materialid", Convert.ToInt32(ddlMaterial.SelectedValue));////////////////////////////////////////////////////////////
            cmd.Parameters.Add("@active", NpgsqlTypes.NpgsqlDbType.Bit).Value = cbIsActive.Checked;



            if (isValidToExecute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {

                    // con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = productId == 0 ? "inserted" : "updated";
                    lblMsg.Visible = true;
                    lblMsg.Text = "Product " + actionName + " successfully";
                    lblMsg.CssClass = "alert alert-success";
                    getProducts();
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
}