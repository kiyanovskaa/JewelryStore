using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;

namespace db_work
{
    public class Connection
    {

        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        }
    }

    public class Utils
    {
        NpgsqlConnection con;
        NpgsqlDataAdapter adapter;

        NpgsqlCommand cmd;
        public static bool IsValidExtension(string fileName)
        {
            bool isValid = false;
            string[] fileExt = { ".jpg", ".png", "jpeg" };
            for(int i = 0; i<=fileExt.Length-1; i++)
            {
                if (fileName.Contains(fileExt[i]))
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;

        }

        public static string GetImageUrl(Object url)
        {
            string url1 = "";
            if(string.IsNullOrEmpty(url.ToString())||url==DBNull.Value)
            {
                url1 = "../Images/No_image.png";
            }
            else
            {
                url1 = string.Format("../{0}", url);
            }
            return url1;
        }
        public bool updateCartQuantity(int quant, int prodid, int userId)
        {
            bool isUpdated = false;
            con = new NpgsqlConnection(Connection.GetConnectionString());
            cmd = new NpgsqlCommand("Cart_Crud", con);
            cmd.Parameters.AddWithValue("@action", "UPDATE");
            cmd.Parameters.AddWithValue("@productid", prodid);
            cmd.Parameters.AddWithValue("@quant", quant);
            cmd.Parameters.AddWithValue("@userid", userId);

            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                isUpdated = true;
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write("<script>alert('Error - " + ex.Message + " ');<script>");
            }
            finally
            {
                con.Close();
            }
            return isUpdated;
        }

        public int cartCount(int userId)
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());
            con.Open();
            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Cart");
            string queryString = $"SELECT c.product_id, p.name, i.url as imageurl, " +
                $"p.price, c.quantity, c.quantity as qty, p.quantity as prdqty FROM \"Carts\" c " +
                $"inner join \"Product\" p on p.product_id = c.product_id inner join \"ImageUrls\" as i on i.imageurl_id=p.imageurl_id where c.user_id={userId}" ;
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            return dataSet.Tables["Cart"].Rows.Count;
            con.Close();
        }

        public static string GetUniqueId()
        {
            Guid guid = Guid.NewGuid();
            String uniqueUd=guid.ToString();
            return uniqueUd;
        }
    }

}