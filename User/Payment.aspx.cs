using Npgsql;
using NpgsqlTypes;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace db_work.User
{
    public partial class Payment : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataReader dr, dr2;
        NpgsqlDataAdapter adapter1, adapter2;
        NpgsqlTransaction transaction;
        string _name = string.Empty;
        string _cardNo = string.Empty;
        string _cvv = string.Empty;
        string _expiryDate = string.Empty;
        string _address = string.Empty;
        string _paymentMode = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("Login.aspx");
                }

            }
        }



        protected void lbCardSubmit_Click(object sender, EventArgs e)
         {
            _name = txtName.Text.Trim();
            _cardNo = txtCardNo.Text.Trim();
            _cardNo = string.Format("************{0}", txtCardNo.Text.Trim().Substring(12, 4));
            _expiryDate = txtExpMonth.Text.Trim() + "/" + txtExpYear.Text.Trim();
            _cvv = txtCvv.Text.Trim();
            _address = txtAddress.Text.Trim();
            _paymentMode = "card";
            if (Session["userId"] != null)
            {
                OrderPayment(_name, _cardNo, _expiryDate, _cvv, _address, _paymentMode);
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

        }

        protected void lbCodSubmit_Click(object sender, EventArgs e)
        {
            _address = txtCODAddress.Text.Trim();
            _paymentMode = "cod";
            if (Session["userId"] != null)
            {
                OrderPayment(_name, _cardNo, _expiryDate, _cvv, _address, _paymentMode);

            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        void OrderPayment(string name, string cardNo, string expiryDate, string cvv, string address, string paym)
        {
            int paymentId;
            List<int> productId = new List<int>();
            List<int> quantity = new List<int>();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[7]{
                new DataColumn("order_no", typeof(string)),
                new DataColumn("product_id", typeof(int)),
                new DataColumn("quantity", typeof(int)),
                new DataColumn("user_id", typeof(int)),
                new DataColumn("status_id", typeof(int)),
                new DataColumn("payment_id", typeof(int)),
                new DataColumn("order_date", typeof(DateTime)),
            });
            con = new NpgsqlConnection(Connection.GetConnectionString());
            con.Open();
            #region Npgsql Transaction
            transaction = con.BeginTransaction();
            cmd = new NpgsqlCommand("Save_Payment", con, transaction);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@cardn", cardNo);
            cmd.Parameters.AddWithValue("@expdate", expiryDate);
            cmd.Parameters.AddWithValue("@cvv", Convert.ToInt32( cvv));
            cmd.Parameters.AddWithValue("@addr", address);
            cmd.Parameters.AddWithValue("@paym", paym);
            cmd.Parameters.AddWithValue("@insertedid", NpgsqlDbType.Bigint);
            cmd.Parameters["insertedid"].Direction = ParameterDirection.Output;
            try
            {

                cmd.ExecuteNonQuery();
                paymentId = Convert.ToInt32(cmd.Parameters["insertedid"].Value);


                int usid = Convert.ToInt32(Session["userId"]);
                #region Getting Cartt Item's
                string queryString = $"SELECT c.product_id, p.name, i.url as imageurl, " +
                    $"p.price, c.quantity, c.quantity as qty, p.quantity as prdqty FROM \"Carts\" c " +
                    $"inner join \"Product\" p on p.product_id = c.product_id inner join \"ImageUrls\" i on i.imageurl_id = p.imageurl_id where c.user_id={usid}";
                NpgsqlCommand com = new NpgsqlCommand(queryString, con, transaction);

                using (NpgsqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        quantity.Add(Convert.ToInt32(dr["quantity"]));
                        productId.Add(Convert.ToInt32(dr["product_id"]));
                        dt.Rows.Add(Utils.GetUniqueId(), Convert.ToInt32(dr["product_id"]), Convert.ToInt32(dr["quantity"]),Convert.ToInt32( Session["userId"]), 1, paymentId, Convert.ToDateTime(DateTime.Now));
                    }
                }
                //  quantity = Convert.ToInt32( dr["quantity"]);
                //    productId = Convert.ToInt32(dr["product_id"]);

                //Upate prod quant
                //   UpdateQuantity(productId, quantity, transaction, con);
                //Upate prod quant end

                //delete cart item
                //delete cart item end
                //dt.Rows.Add(Convert.ToInt32(Utils.GetUniqueId()), Convert.ToInt32(dr["product_id"]), Convert.ToInt32(dr["quantity"]), (int)Session["userId"], "Pending",
                //        paymentId, Convert.ToDateTime(DateTime.Now));
                
                //dr.Close();
                for(int i=0;i<quantity.Count;i++)
                {

                    UpdateQuantity(productId.ElementAt(i), quantity.ElementAt(i), transaction, con);
                        DeleteCartItem(productId.ElementAt(i), transaction, con);
                }

                #endregion Getting Cartt Item's

                #region Order Details
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        // Створюємо команду для виклику процедури
                        NpgsqlCommand cmd = new NpgsqlCommand("Save_orders", con, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Додаємо параметри для передачі значень рядка
                        cmd.Parameters.AddWithValue("@ordern", row["order_no"]);
                        cmd.Parameters.AddWithValue("@productid", row["product_id"]);
                        cmd.Parameters.AddWithValue("@quant", row["quantity"]);
                        cmd.Parameters.AddWithValue("@userid", row["user_id"]);
                        cmd.Parameters.AddWithValue("@stat", row["status_id"]);
                        cmd.Parameters.AddWithValue("@paymentid", row["payment_id"]);
                        cmd.Parameters.AddWithValue("@orderdate", row["order_date"]);

                        // Виконуємо команду
                        cmd.ExecuteNonQuery();
                    }
                }

                #endregion

                transaction.Commit();
                lblMsg.Visible = true;
                lblMsg.Text = "Your item ordered successful!!!";
                lblMsg.CssClass = "alert alert-success";
                Response.AddHeader("REFRESH", "1;URL=Invoice.aspx?id" + paymentId);

            }
            catch (Exception e)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "'(;</script>");
                }
            }
            #endregion Npgsql Transaction
            finally
            {
                con.Close();
            }

        }
        void UpdateQuantity(int _productId, int _quantity, NpgsqlTransaction trans, NpgsqlConnection conect)
        {
            List<int> dbQuantity = new List<int>();
            string queryString = $"SELECT * from \"Product\" where product_id={_productId}";
            NpgsqlCommand com = new NpgsqlCommand(queryString, conect, trans);
            

            try
            {
                dr2 = com.ExecuteReader();
                while(dr2.Read())
                {
                    dbQuantity.Add(Convert.ToInt32(dr2["quantity"]));
                    //dbQuantity = Convert.ToInt32(dr2["quantity"]);
                   

                    //if (dbQuantity>_quantity && dbQuantity > 2)
                    //{
                    //    dbQuantity= dbQuantity - _quantity;
                    //    cmd = new NpgsqlCommand("Product_crud", conect, trans);
                    //    cmd.Parameters.AddWithValue("@action", "QTYUPDATE");
                    //    cmd.Parameters.AddWithValue("@productid", _productId);
                    //    cmd.Parameters.AddWithValue("@quant", dbQuantity);
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    cmd.ExecuteNonQuery();
                    //}

                }
                dr2.Close();

                foreach(var item in dbQuantity)
                {
                    if (item > _quantity && item > 2)
                    {
                        int temp = item - _quantity;
                        cmd = new NpgsqlCommand("Product_crud", conect, trans);
                        cmd.Parameters.AddWithValue("@action", "QTYUPDATE");
                        cmd.Parameters.AddWithValue("@productid", _productId);
                        cmd.Parameters.AddWithValue("@quant", temp);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "'(;</script>");


            }
            finally
            {

            }

        }

        void DeleteCartItem(int _productId, NpgsqlTransaction trans, NpgsqlConnection conect)
        {
            cmd = new NpgsqlCommand("Cart_crud", conect, trans);
            cmd.Parameters.AddWithValue("@action", "DELETE");
            cmd.Parameters.AddWithValue("@productid", _productId);
            cmd.Parameters.AddWithValue("@userid", Convert.ToInt32( Session["userId"] ));
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();

            }
            catch(Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message + "'(;</script>");

            }
            finally
            {

            }
        }

    }
}