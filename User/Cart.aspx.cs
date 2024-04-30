using db_work.Admin;
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
    public partial class Cart : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;

        decimal grandTotal = 0;
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
                    getCartItems();
                }
            }

        }
        void getCartItems()
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());
            con.Open();
            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Cart");
            int usid = Convert.ToInt32(Session["userId"]);
            string queryString = $"SELECT c.product_id, p.name, i.url as imageurl, p.price, c.quantity, c.quantity as qty, p.quantity as prdqty FROM \"Carts\" c inner join \"Product\" p on p.product_id = c.product_id inner join \"ImageUrls\" i on i.imageurl_id=p.imageurl_id where c.user_id={usid}";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rCartItem.DataSource = dataSet.Tables["Cart"].DefaultView;

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                rCartItem.FooterTemplate = null;
                rCartItem.FooterTemplate = new CustomTemplate(ListItemType.Footer);
            }
            rCartItem.DataBind();

        }

        protected void rCartItem_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Utils utils = new Utils();
            if (e.CommandName == "remove")
            {
                con = new NpgsqlConnection(Connection.GetConnectionString());
                cmd = new NpgsqlCommand("Cart_Crud", con);
                cmd.Parameters.AddWithValue("@action", "DELETE");
                cmd.Parameters.AddWithValue("@productid", Convert.ToInt32(e.CommandArgument));
                cmd.Parameters.AddWithValue("@userid", Convert.ToInt32(Session["userId"]));
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    getCartItems();
                    Session["cartCount"] = utils.cartCount(Convert.ToInt32(Session["userId"]));
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Error - " + ex.Message + " ');script>");
                }
                finally
                {
                    con.Close();
                }
            }
            if (e.CommandName == "updateCart")
            {
                bool isCartUpdated = false;
                for (int i = 0; i < rCartItem.Items.Count; ++i)
                {
                    if (rCartItem.Items[i].ItemType == ListItemType.Item || rCartItem.Items[i].ItemType == ListItemType.AlternatingItem)
                    {
                        TextBox quantity = rCartItem.Items[i].FindControl("txtQuantity") as TextBox;


                        HiddenField _productId = rCartItem.Items[i].FindControl("hdnProductId") as HiddenField;
                        HiddenField _quantity = rCartItem.Items[i].FindControl("hdnQuantity") as HiddenField;
                        int quantityFromCart = Convert.ToInt32(quantity.Text);
                        int ProductId = Convert.ToInt32(_productId.Value);
                        int quantityFromDB = Convert.ToInt32(_quantity.Value);
                        bool isTrue = false;
                        int updatedQuantity = 1;
                        if (quantityFromCart > quantityFromDB)
                        {
                            updatedQuantity = quantityFromCart;
                            isTrue = true;
                        }

                        else if (quantityFromCart < quantityFromDB)
                        {
                            updatedQuantity = quantityFromCart;
                            isTrue = true;
                        }

                        if (isTrue)
                        {
                            isCartUpdated = utils.updateCartQuantity(updatedQuantity, ProductId, Convert.ToInt32(Session["userId"]));

                        }
                    }
                }
                getCartItems();
            }
            if (e.CommandName == "checkout")
            {
                bool isTrue = false;
                string pName = string.Empty;
                for (int i = 0; i < rCartItem.Items.Count; ++i)
                {
                    if (rCartItem.Items[i].ItemType == ListItemType.Item || rCartItem.Items[i].ItemType == ListItemType.AlternatingItem)
                    {
                        HiddenField _productId = rCartItem.Items[i].FindControl("hdnProductId") as HiddenField;
                        HiddenField _cartQuantity = rCartItem.Items[i].FindControl("hdnQuantity") as HiddenField;
                        HiddenField _productQuantity = rCartItem.Items[i].FindControl("hdnPrdQuantity") as HiddenField;
                        Label productName = rCartItem.Items[i].FindControl("lblName") as Label;
                        int ProductId = Convert.ToInt32(_productId.Value);
                        int cartQuantity = Convert.ToInt32(_cartQuantity.Value);
                        int productQuantity = Convert.ToInt32(_productQuantity.Value);

                        if (productQuantity > cartQuantity && productQuantity>2)
                        {
                            isTrue = true;
                        }

                        else 
                        {
                            isTrue = false;
                            pName = productName.Text.ToString();
                            break;
                        }

             
                    }
                }
                if (isTrue)
                {
                    Response.Redirect("Payment.aspx");
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Item <b>'"+ pName+"'</b> is out of stock:(";
                    lblMsg.CssClass = "alert alert-warning";
                }
            }
        }

        protected void rCartItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {


            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label totalPrice = e.Item.FindControl("lblTotalPrice") as Label;
                Label productPrice = e.Item.FindControl("lblPrice") as Label;
                TextBox quantity = e.Item.FindControl("txtQuantity") as TextBox;
                decimal calTotalPrice = Convert.ToDecimal(productPrice.Text) * Convert.ToDecimal(quantity.Text);
                totalPrice.Text = calTotalPrice.ToString();
                grandTotal += calTotalPrice;
            }
            Session["grandTotalPrice"] = grandTotal;

        }
        private sealed class CustomTemplate : ITemplate
        {
            private ListItemType ListItemType { get; set; }
            public CustomTemplate(ListItemType type)
            {
                ListItemType = type;
            }
            public void InstantiateIn(Control container)
            {
                if (ListItemType == ListItemType.Footer)
                {
                    var footer = new LiteralControl("<tr><td colspan='5'><b>Your Cart is empty.</b><a href='Product.aspx' class='badge badge-info ml-2'>Continue Shopping</a></td></tr></tbody></table>");
                    container.Controls.Add(footer);
                }
            }
        }
    }





}