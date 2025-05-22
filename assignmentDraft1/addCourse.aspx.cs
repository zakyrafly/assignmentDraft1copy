using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace assignmentDraft1
{
    public partial class addCourse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in and is a teacher
            if (Session["userId"] == null || Session["userRole"] == null)
            {
                Response.Redirect("loginWebform.aspx");
                return;
            }

            string userRole = Session["userRole"].ToString();
            if (userRole.ToLower() != "teacher" && userRole.ToLower() != "lecturer")
            {
                Response.Redirect("loginWebform.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTeacherData();
            }
        }

        private void LoadTeacherData()
        {
            if (Session["userName"] != null)
            {
                string teacherName = Session["userName"].ToString();
                lblTeacherName.Text = teacherName;
                lblSidebarTeacherName.Text = teacherName;
                lblTeacherRole.Text = Session["userRole"].ToString();
                lblSidebarTeacherRole.Text = Session["userRole"].ToString();
            }
        }

        protected void btnAddCourse_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string courseTitle = txtCourseTitle.Text.Trim();
                string courseDescription = txtCourseDescription.Text.Trim();
                string category = ddlCategory.SelectedValue;
                int teacherId = Convert.ToInt32(Session["userId"]);

                string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    try
                    {
                        con.Open();

                        // Check if course with same title already exists for this teacher
                        string checkQuery = "SELECT COUNT(*) FROM Courses WHERE CourseName = @title AND CreatedBy = @teacherId";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                        checkCmd.Parameters.AddWithValue("@title", courseTitle);
                        checkCmd.Parameters.AddWithValue("@teacherId", teacherId);

                        int existingCourses = (int)checkCmd.ExecuteScalar();
                        if (existingCourses > 0)
                        {
                            ShowError("A course with this title already exists in your courses.");
                            return;
                        }

                        // Get next CourseID
                        string getMaxIdQuery = "SELECT ISNULL(MAX(CourseID), 0) + 1 FROM Courses";
                        SqlCommand getMaxIdCmd = new SqlCommand(getMaxIdQuery, con);
                        int newCourseId = (int)getMaxIdCmd.ExecuteScalar();

                        // Insert new course using your actual table structure
                        string insertQuery = @"INSERT INTO Courses (CourseID, CourseName, Description, Category, CreatedBy, CreatedDate, IsActive) 
                                             VALUES (@courseId, @courseName, @description, @category, @createdBy, @createdDate, 1)";

                        SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                        insertCmd.Parameters.AddWithValue("@courseId", newCourseId);
                        insertCmd.Parameters.AddWithValue("@courseName", courseTitle);
                        insertCmd.Parameters.AddWithValue("@description", courseDescription);
                        insertCmd.Parameters.AddWithValue("@category", category);
                        insertCmd.Parameters.AddWithValue("@createdBy", teacherId);
                        insertCmd.Parameters.AddWithValue("@createdDate", DateTime.Now);

                        int result = insertCmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            ShowSuccess(newCourseId);
                            ClearForm();
                        }
                        else
                        {
                            ShowError("Failed to add course. Please try again.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError("An error occurred while adding the course: " + ex.Message);
                    }
                }
            }
        }

        private void ShowSuccess(int courseId)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            lnkViewCourse.NavigateUrl = $"viewCourse.aspx?id={courseId}";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblErrorMessage.Text = message;
        }

        private void ClearForm()
        {
            txtCourseTitle.Text = "";
            txtCourseDescription.Text = "";
            ddlCategory.SelectedIndex = 0;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("TeacherDashboard.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Response.Redirect($"searchResults.aspx?q={HttpUtility.UrlEncode(searchTerm)}");
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("loginWebform.aspx");
        }
    }
}