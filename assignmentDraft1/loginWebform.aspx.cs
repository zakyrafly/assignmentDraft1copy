using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace assignmentDraft1
{
	public partial class WebForm1 : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                // Modified query to also get the user's role and other details
                string query = "SELECT UserID, Name, Role FROM Users WHERE username=@email AND Password=@pass";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pass", password);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Successful login - get user details
                    int userId = (int)reader["UserID"];
                    string userName = reader["Name"].ToString();
                    string userRole = reader["Role"].ToString();

                    // Store user information in session
                    Session["email"] = email;
                    Session["userId"] = userId;
                    Session["userName"] = userName;
                    Session["userRole"] = userRole;

                    reader.Close();

                    // Redirect based on user role
                    if (userRole.ToLower() == "teacher" || userRole.ToLower() == "lecturer")
                    {
                        Response.Redirect("TeacherDashboard.aspx");
                    }
                    else if (userRole.ToLower() == "student")
                    {
                        Response.Redirect("homeWebform.aspx");
                    }
                    else
                    {
                        // Default redirect for other roles
                        Response.Redirect("homeWebform.aspx");
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid email or password.";
                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Register.aspx");
        }

        protected void LinkButton1_Click1(object sender, EventArgs e)
        {
            Response.Redirect("Register.aspx");
        }
    }
}