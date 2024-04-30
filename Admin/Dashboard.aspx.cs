using db_work.User;
using Npgsql;
using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace db_work.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../User/Login.aspx");
                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
             readCountryFromFile();
            generateUsers();
            genereteOrders();


        }

        private void genereteOrders()
        {
           
            string filePath = "E:\\course_work\\db_work\\db_work\\orders.txt";
  
            try
            {
                NpgsqlConnection con = new NpgsqlConnection(Connection.GetConnectionString());
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(','); // Розділяємо рядок за допомогою коми

                        // Зчитуємо значення змінних з масиву частин
                        string orderno = parts[0].Trim();
                        int productid = int.Parse(parts[1].Trim());
                        int quant = int.Parse(parts[2].Trim());
                        int userid = int.Parse(parts[3].Trim());
                        int stat = int.Parse(parts[4].Trim());
                        int paymentid = int.Parse(parts[5].Trim());
                        string orderdate =parts[6].Trim();
                        NpgsqlCommand cmd = new NpgsqlCommand("Save_orders", con);
                        cmd.Parameters.AddWithValue("@ordern", orderno);
                        cmd.Parameters.AddWithValue("@productid", productid);
                        cmd.Parameters.AddWithValue("@quant", quant);
                        cmd.Parameters.AddWithValue("@userid", userid);
                        cmd.Parameters.AddWithValue("@stat", stat);
                        cmd.Parameters.AddWithValue("@paymentid", paymentid);
                        cmd.Parameters.Add("@orderdate", NpgsqlTypes.NpgsqlDbType.Timestamp).Value = DateTime.Parse(orderdate);
                        //k++;
                        cmd.CommandType = CommandType.StoredProcedure;
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                          //  string orderInfo = $"{orderno}, {id}, {n}, {id}, {n}, 43, 2024-04-28 22:08:01.966871";
                            //writer.WriteLine(orderInfo);
                        }
                        catch (NpgsqlException ex)
                        {
                            if (ex.Message.Contains("violates unique constraint"))
                            {
                                lblMsg.Visible = true;
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

            }
            catch (Exception ex)
            {
                // Обробка помилок
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void readCountryFromFile()
        {

            string filePath = "E:\\course_work\\db_work\\db_work\\countries.txt"; // Шлях до файлу
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    NpgsqlConnection con = new NpgsqlConnection(Connection.GetConnectionString());
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string queryString = $"insert into \"Countries\" (country) values (\'{line}\')"; // Виводимо рядок зчитаної інформації
                        NpgsqlCommand cmd = new NpgsqlCommand(queryString, con);

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (NpgsqlException ex)
                        {
                            if (ex.Message.Contains("violates unique constraint"))
                            {
                                lblMsg.Visible = true;
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
            }
            catch (IOException ex)
            {
                Console.WriteLine("An error occurred while reading the file: " + ex.Message);
            }
        }

        private void generateUsers()
        {
            NpgsqlConnection con = new NpgsqlConnection(Connection.GetConnectionString());
            string queryString = $"select country_id from \"Countries\""; // Виводимо рядок зчитаної інформації
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);





            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Country");
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            List<int> id = new List<int>();
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                id.Add(Convert.ToInt32(dataSet.Tables[0].Rows[i]["country_id"]));
            }

            int k = 0;
            for (int i = 0; i < 5000; ++i)
            {
                if (k == id.Count - 2)
                {
                    k = 0;
                }
                NpgsqlCommand cmd = new NpgsqlCommand("User_Crud", con);
                string temp = "test" + Convert.ToString(i);
                cmd.Parameters.AddWithValue("@action", "INSERT");
                cmd.Parameters.AddWithValue("@countryid", id.ElementAt(k));
                cmd.Parameters.AddWithValue("@name", "test");
                cmd.Parameters.AddWithValue("@un", temp);
                cmd.Parameters.AddWithValue("@mail", "test");
                cmd.Parameters.AddWithValue("@addr", "test");
                cmd.Parameters.AddWithValue("@passw", "test");
                k++;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (NpgsqlException ex)
                {
                    if (ex.Message.Contains("violates unique constraint"))
                    {
                        lblMsg.Visible = true;
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
    }
}

