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
    public partial class Report : System.Web.UI.Page
    {
        NpgsqlConnection con;
        NpgsqlCommand cmd;
        NpgsqlDataAdapter adapter;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["breadCrum"] = "Selling Report";
                if (Session["admin"] == null)
                {
                    Response.Redirect("../User/Login.aspx");
                }
            }
        }

        private void getReport(DateTime fromDate, DateTime toDate)
        {
            con = new NpgsqlConnection(Connection.GetConnectionString());


            con.Open();
            double grandTotal = 0;
            adapter = new NpgsqlDataAdapter();
            adapter.TableMappings.Add("Table", "Report");
            //string from=Convert.ToString(fromDate), to=Convert.ToString(toDate);
            //DateTime datefrom = DateTime.ParseExact(from, "dd.MM.yyyy H:mm:ss", null);
            //DateTime dateto = DateTime.ParseExact(to, "dd.MM.yyyy H:mm:ss", null);
            string d= "dd";
            string fromcoorect = fromDate.ToString("yyyy-MM-dd");
            string tocorrect = toDate.ToString("yyyy-MM-dd");

            string queryString = "select row_number() over(order by(select 1)) as srno, " +
                "u.user_name, u.email, sum(o.quantity) as TotalOrders, " +
                "Sum(p.price*o.quantity) as TotalPrice from \"Orders\" o " +
                "inner join \"Product\" p on p.product_id=o.product_id " +
                "inner join \"User\" u on u.user_id=o.user_id " +
                $"where  o.order_date between \'{fromcoorect}\' AND \'{tocorrect}\'" +
                " group by u.user_name, u.email";
            NpgsqlCommand com = new NpgsqlCommand(queryString, con);
            DataSet dataSet = new DataSet();
            adapter.SelectCommand = com;
            adapter.Fill(dataSet);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                foreach(DataRow dr in dataSet.Tables[0].Rows)
                {
                    grandTotal +=Convert.ToDouble( dr["TotalPrice"]);
                }
                lblTotal.Text="Sold Cost: $"+ grandTotal.ToString();
                lblTotal.CssClass = "badge badge-primary";
            }
            rReport.DataSource = dataSet.Tables["Report"].DefaultView;
            rReport.DataBind();

            con.Close();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
           DateTime fromDate=Convert.ToDateTime(txtFromDate.Text);
           DateTime toDate=Convert.ToDateTime(txtToDate.Text);
            //if(toDate > DateTime.Now){
            //    Response.Write("<script>alert('ToDate cannot be greater than current date!');<script>");
            //}
            //else
            if (fromDate > toDate)
            {
                Response.Write("<script>alert('FromDate cannot be greater than ToDate!');</script>");

            }
            else
            {
                getReport(fromDate, toDate);
            }

        }
    }
}