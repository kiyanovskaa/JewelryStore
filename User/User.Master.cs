using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace db_work.User
{
    public partial class User : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.Url.AbsoluteUri.ToString().Contains("Default.aspx"))
            {
              //  form1.Attributes.Add("class", "sub_page");
            }
            else
            {
              //  form1.Attributes.Remove("class");
                //Control sliderUserControl = (Control)Page.LoadControl("SliderUserControl.ascx");
               // pnlSliderUC.Controls.Add(sliderUserControl);
            }
            if (Session["userId"] != null)
            {
                
                Utils utils = new Utils();
                lblLoginOrLogout.Text = "Logout";
                Session["cartCount"] = utils.cartCount(Convert.ToInt32(Session["userId"]));


            }
            else
            {
                lblLoginOrLogout.Text = "Login";
                Session["cartCount"] = "0";
               // Session["cartCount"]  = utils.cartCount(Convert.ToInt32(Session["userId"]));
            }
        }

        protected void lblLoginOrLogout_Click(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                Session.Abandon();
                Response.Redirect("Login.aspx");
            }
        }

        protected void lblRegisterOrProfile_Click(object sender, EventArgs e)
        {
            if (Session["userId"] != null)
            {
                lblRegisterOrProfile.ToolTip = "User Profile";
                Response.Redirect("Profile.aspx");
            }
            else
            {
                lblRegisterOrProfile.ToolTip = "User Registration";
                Response.Redirect("Registration.aspx");
            }
        }
    }
}