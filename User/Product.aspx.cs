using db_work.Admin;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace db_work.User
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
                getCategories();
                getProducts();
            }
        }
        private void getProducts()
        {
            //con = new NpgsqlConnection(Connection.GetConnectionString());
            //con.Open();
            //cmd = new NpgsqlCommand("product_crud", con);
            //cmd.Parameters.Add(new NpgsqlParameter("@action", NpgsqlDbType.Varchar)).Value = "SELECT";

            //cmd.Parameters.Add(new NpgsqlParameter("@productid", NpgsqlDbType.Integer)).Value = DBNull.Value;
            //cmd.Parameters["@productid"].Direction = ParameterDirection.InputOutput;

            //cmd.Parameters.Add(new NpgsqlParameter("@productname", NpgsqlDbType.Varchar)).Value = DBNull.Value;
            //cmd.Parameters["@productname"].Direction = ParameterDirection.InputOutput;

            //cmd.Parameters.Add(new NpgsqlParameter("@descr", NpgsqlDbType.Varchar)).Value = DBNull.Value;
            //cmd.Parameters["@descr"].Direction = ParameterDirection.InputOutput;

            //cmd.Parameters.Add(new NpgsqlParameter("@pricep", NpgsqlDbType.Double)).Value = DBNull.Value;
            //cmd.Parameters["@pricep"].Direction = ParameterDirection.InputOutput;

            //cmd.Parameters.Add(new NpgsqlParameter("@quant", NpgsqlDbType.Integer)).Value = DBNull.Value;
            //cmd.Parameters["@quant"].Direction = ParameterDirection.InputOutput;

            //cmd.Parameters.Add(new NpgsqlParameter("@image", NpgsqlDbType.Varchar)).Value = DBNull.Value;
            //cmd.Parameters["@image"].Direction = ParameterDirection.InputOutput;

            //cmd.Parameters.Add(new NpgsqlParameter("@categoryid", NpgsqlDbType.Integer)).Value = DBNull.Value;
            //cmd.Parameters["@categoryid"].Direction = ParameterDirection.InputOutput;

            //cmd.Parameters.Add(new NpgsqlParameter("@active", NpgsqlDbType.Bit)).Value = DBNull.Value;
            //cmd.Parameters["@active"].Direction = ParameterDirection.InputOutput;

            //cmd.Parameters.Add(new NpgsqlParameter("@categoryname", NpgsqlDbType.Varchar)).Value = DBNull.Value;
            //cmd.Parameters["@categoryname"].Direction = ParameterDirection.InputOutput;

            //cmd.CommandType = CommandType.StoredProcedure;
            //try
            //{
            //    con.Open();
            //    // Виконання команди
            //    using (NpgsqlDataReader reader = cmd.ExecuteReader())
            //    {
            //        // Створення DataTable
            //        DataTable dataTable = new DataTable("Product");

            //        // Додавання колонок до DataTable
            //        dataTable.Columns.Add("productid", typeof(int));
            //        dataTable.Columns.Add("productname", typeof(string));
            //        dataTable.Columns.Add("descr", typeof(string));
            //        dataTable.Columns.Add("pricep", typeof(double));
            //        dataTable.Columns.Add("quant", typeof(int));
            //        dataTable.Columns.Add("image", typeof(string));
            //        dataTable.Columns.Add("categoryid", typeof(int));
            //        dataTable.Columns.Add("active", typeof(bool));
            //        dataTable.Columns.Add("categoryname", typeof(string));

            //        // Зчитування результатів і додавання рядків до DataTable
            //        while (reader.Read())
            //        {
            //            DataRow newRow = dataTable.NewRow();
            //            newRow["productid"] = reader["productid"];
            //            newRow["productname"] = reader["productname"].ToString();
            //            newRow["descr"] = reader["descr"].ToString();
            //            newRow["pricep"] = reader["pricep"];
            //            newRow["quant"] = reader["quant"];
            //            newRow["image"] = reader["image"].ToString();
            //            newRow["categoryid"] = reader["categoryid"];
            //            // Перетворення значення BitArray на Boolean
            //            bool isActive = ((BitArray)reader["active"])[0];

            //            // Присвоєння перетвореного значення в стовпець "active"
            //            newRow["active"] = isActive;

            //            newRow["categoryname"] = reader["categoryname"].ToString();
            //            dataTable.Rows.Add(newRow);
            //        }

            //        // Створення DataSet та додавання DataTable
            //        DataSet dataSet = new DataSet();
            //        dataSet.Tables.Add(dataTable);

            //        // Додавання DataTable до DataSet
            //      //  dataSet.Tables.Add(dataTable);
            //        adapter.Fill(dataSet);
            //        rProducts.DataSource = dataSet.Tables["Product"].DefaultView;
            //        rProducts.DataBind();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Response.Write("<script>alert('Error - " + ex.Message + " ');<script>");
            //}
            //finally
            //{
            //    con.Close();
            //}




            //
            con = new NpgsqlConnection(Connection.GetConnectionString());
            con.Open();
            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Product");
            string queryString = "SELECT p.*,  c.name as CategoryName, i.url as imageurl FROM \"Product\" p inner join \"Category\" c on c.category_id=p.category_id inner join \"ImageUrls\" i  on p.imageurl_id=i.imageurl_id where c.isactive= \'1\' and p.isactive=\'1\'";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rProducts.DataSource = dataSet.Tables["Product"].DefaultView;
            rProducts.DataBind();
            con.Close();
        }

        private void getCategories()
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());


            con.Open();

            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Category");
            string queryString = "SELECT * FROM \"Category\" where isactive=\'1\'";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rCategory.DataSource = dataSet.Tables["Category"].DefaultView;
            rCategory.DataBind();

            con.Close();


        }

        protected void rProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
              if (e.CommandName == "view")
            {
                TextBox1.Text = "dddddddddddddddddddddddddddd";
            }

            if (Session["userId"] != null)
            {
                if (e.CommandName == "addToCart")
                {
                    bool isCartItemUpdated = false;
                    int i = isItemExistInCart(Convert.ToInt32(e.CommandArgument));
                    if (i == 0)
                    {
                        con = new NpgsqlConnection(Connection.GetConnectionString());
                        cmd = new NpgsqlCommand("Cart_Crud", con);
                        cmd.Parameters.AddWithValue("@action", "INSERT");
                        cmd.Parameters.AddWithValue("@productid", Convert.ToInt32(e.CommandArgument));
                        cmd.Parameters.AddWithValue("@quant", 1);
                        cmd.Parameters.AddWithValue("@userid", Convert.ToInt32(Session["userId"]));

                        cmd.CommandType = CommandType.StoredProcedure;
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Response.Write("<script>alert('Error - " + ex.Message + " ');<script>");
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                    else
                    {
                        //add existing item into cart
                        Utils util = new Utils();
                        isCartItemUpdated = util.updateCartQuantity(i + 1, Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["userId"]));

                    }
                    lblMsg.Visible = true;
                    lblMsg.Text = "Item added successfully in your cart!";
                    lblMsg.CssClass = "alert alert-success";
                    Response.AddHeader("REFRESH", "1;URL=Cart.aspx");
                }
               
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        int isItemExistInCart(int prodId)
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());
            con.Open();
            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Product");
            int usid = Convert.ToInt32(Session["userId"]);
            string queryString = $"SELECT *  FROM \"Carts\" where user_id={usid} and product_id={prodId}";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            int quat = 0;
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                quat = Convert.ToInt32(dataSet.Tables[0].Rows[0]["quantity"]);
            }
            return quat;
        }

       

        protected void rCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if(e.CommandName== "getProd")
            {
                int i = Convert.ToInt32(e.CommandArgument);
                con = new NpgsqlConnection(Connection.GetConnectionString());
                con.Open();
                adapter = new NpgsqlDataAdapter();
                adapter.TableMappings.Add("Table", "Product");
                string queryString = $"SELECT p.*,  c.name as CategoryName, i.url as imageurl FROM \"Product\" p inner join \"Category\" c on c.category_id=p.category_id inner join \"ImageUrls\" i  on p.imageurl_id=i.imageurl_id where c.isactive= \'1\' and p.isactive=\'1\' and p.category_id={i}";
                NpgsqlCommand com = new NpgsqlCommand(queryString, con);
                DataSet dataSet = new DataSet();
                adapter.SelectCommand = com;
                adapter.Fill(dataSet);
                rProducts.DataSource = dataSet.Tables["Product"].DefaultView;
                rProducts.DataBind();
                con.Close();

            }
        }

        protected void lbGetByCategory_Click(object sender, EventArgs e)
        {

        }
    }
}