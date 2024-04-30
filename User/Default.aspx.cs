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
    public partial class Default : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                getCategories();
            }
        }
    
        private void getCategories()
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());


            con.Open();

            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Category");
            string queryString = "SELECT c.*, i.url as imageurl FROM \"Category\" c inner join \"ImageUrls\" i on c.imageurl_id=i.imageurl_id where isactive=\'1\'";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            rCategory.DataSource = dataSet.Tables["Category"].DefaultView;
            rCategory.DataBind();



        }
    }
}