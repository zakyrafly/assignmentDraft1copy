using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace assignmentDraft1
{
    public partial class TeacherDashbord : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;

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
                LoadDashboardData();
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

        private void LoadDashboardData()
        {
            LoadStats();
            LoadCourses();
            LoadLessons();
            LoadStudents();
            LoadAssignments();
            LoadAnalytics();
        }

        private void LoadStats()
        {
            string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            int teacherId = Convert.ToInt32(Session["userId"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                try
                {
                    con.Open();

                    // Get teacher's course statistics
                    string statsQuery = @"
                            SELECT 
                                COUNT(DISTINCT c.CourseID) as TotalCourses,
                                COUNT(DISTINCT uc.UserID) as TotalStudents,
                                COUNT(DISTINCT a.AssignmentID) as TotalAssignments,
                                COUNT(DISTINCT m.MaterialID) as TotalMaterials
                            FROM Courses c
                            LEFT JOIN UserCourses uc ON c.CourseID = uc.CourseID
                            LEFT JOIN Assignments a ON c.CourseID = a.CourseID AND a.IsActive = 1
                            LEFT JOIN Materials m ON c.CourseID = m.CourseID AND m.IsActive = 1
                            WHERE c.CreatedBy = @teacherId AND c.IsActive = 1";

                    SqlCommand cmd = new SqlCommand(statsQuery, con);
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var stats = new List<object>
                            {
                                new { Icon = "fa-book", Value = reader["TotalCourses"].ToString(), Title = "Active Courses" },
                                new { Icon = "fa-users", Value = reader["TotalStudents"].ToString(), Title = "Total Students" },
                                new { Icon = "fa-tasks", Value = reader["TotalAssignments"].ToString(), Title = "Assignments" },
                                new { Icon = "fa-file-alt", Value = reader["TotalMaterials"].ToString(), Title = "Course Materials" }
                            };

                        rptStats.DataSource = stats;
                        rptStats.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    // Handle error - show default stats
                    var defaultStats = new List<object>
                        {
                            new { Icon = "fa-book", Value = "0", Title = "Active Courses" },
                            new { Icon = "fa-users", Value = "0", Title = "Total Students" },
                            new { Icon = "fa-tasks", Value = "0", Title = "Assignments" },
                            new { Icon = "fa-file-alt", Value = "0", Title = "Course Materials" }
                        };

                    rptStats.DataSource = defaultStats;
                    rptStats.DataBind();
                }
            }
        }

        private void LoadCourses()
        {
            string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            int teacherId = Convert.ToInt32(Session["userId"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                        SELECT 
                            c.CourseID, 
                            c.CourseName as CourseTitle, 
                            ISNULL(c.Description, 'No description available') as CourseDescription,
                            COUNT(DISTINCT uc.UserID) as TotalStudents,
                            75 as CompletionRate
                        FROM Courses c
                        LEFT JOIN UserCourses uc ON c.CourseID = uc.CourseID
                        WHERE c.CreatedBy = @teacherId AND c.IsActive = 1
                        GROUP BY c.CourseID, c.CourseName, c.Description
                        ORDER BY c.CreatedDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@teacherId", teacherId);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        rptCourses.DataSource = reader;
                        rptCourses.DataBind();
                        pnlNoCourses.Visible = false;
                    }
                    else
                    {
                        pnlNoCourses.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    pnlNoCourses.Visible = true;
                }
            }
        }

        private void LoadLessons()
        {
            string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            int teacherId = Convert.ToInt32(Session["userId"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                        SELECT 
                            c.CourseID,
                            c.CourseName as CourseTitle,
                            COUNT(m.MaterialID) as MaterialCount
                        FROM Courses c
                        LEFT JOIN Materials m ON c.CourseID = m.CourseID AND m.IsActive = 1
                        WHERE c.CreatedBy = @teacherId AND c.IsActive = 1
                        GROUP BY c.CourseID, c.CourseName
                        HAVING COUNT(m.MaterialID) > 0
                        ORDER BY c.CourseName";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@teacherId", teacherId);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        rptLessons.DataSource = reader;
                        rptLessons.DataBind();
                        pnlNoMaterials.Visible = false;
                    }
                    else
                    {
                        pnlNoMaterials.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    pnlNoMaterials.Visible = true;
                }
            }
        }

        private void LoadStudents()
        {
            string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            int teacherId = Convert.ToInt32(Session["userId"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                        SELECT TOP 10 
                            u.Name as StudentName, 
                            u.username as StudentEmail, 
                            75 as Progress
                        FROM Users u
                        INNER JOIN UserCourses uc ON u.UserID = uc.UserID
                        INNER JOIN Courses c ON uc.CourseID = c.CourseID
                        WHERE c.CreatedBy = @teacherId AND u.Role = 'Student'
                        ORDER BY u.Name";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@teacherId", teacherId);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        rptStudents.DataSource = reader;
                        rptStudents.DataBind();
                        pnlNoStudents.Visible = false;
                    }
                    else
                    {
                        pnlNoStudents.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    pnlNoStudents.Visible = true;
                }
            }
        }

        private void LoadAssignments()
        {
            string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            int teacherId = Convert.ToInt32(Session["userId"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                        SELECT 
                            a.AssignmentID,
                            a.AssignmentTitle,
                            c.CourseName as CourseTitle,
                            a.DueDate,
                            0 as SubmissionCount,
                            COUNT(DISTINCT uc.UserID) as TotalStudents,
                            a.Status
                        FROM Assignments a
                        INNER JOIN Courses c ON a.CourseID = c.CourseID
                        LEFT JOIN UserCourses uc ON c.CourseID = uc.CourseID
                        WHERE a.CreatedBy = @teacherId AND a.IsActive = 1
                        GROUP BY a.AssignmentID, a.AssignmentTitle, c.CourseName, a.DueDate, a.Status
                        ORDER BY a.DueDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@teacherId", teacherId);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        rptAssignments.DataSource = reader;
                        rptAssignments.DataBind();
                        pnlNoAssignments.Visible = false;
                    }
                    else
                    {
                        pnlNoAssignments.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    pnlNoAssignments.Visible = true;
                }
            }
        }

        private void LoadAnalytics()
        {
            string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            int teacherId = Convert.ToInt32(Session["userId"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                        SELECT 
                            c.CourseName as CourseTitle,
                            COUNT(DISTINCT uc.UserID) as TotalStudents,
                            75 as CompletionRate,
                            85 as AverageGrade,
                            45 as AvgActiveTime
                        FROM Courses c
                        LEFT JOIN UserCourses uc ON c.CourseID = uc.CourseID
                        WHERE c.CreatedBy = @teacherId AND c.IsActive = 1
                        GROUP BY c.CourseID, c.CourseName
                        ORDER BY COUNT(DISTINCT uc.UserID) DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@teacherId", teacherId);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        rptAnalytics.DataSource = reader;
                        rptAnalytics.DataBind();
                        pnlNoAnalytics.Visible = false;
                    }
                    else
                    {
                        pnlNoAnalytics.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    pnlNoAnalytics.Visible = true;
                }
            }
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

        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCourse")
            {
                int courseId = Convert.ToInt32(e.CommandArgument);

                string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = "UPDATE Courses SET IsActive = 0 WHERE CourseID = @courseId AND CreatedBy = @teacherId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@courseId", courseId);
                    cmd.Parameters.AddWithValue("@teacherId", Session["userId"]);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        LoadCourses(); // Refresh the courses list
                    }
                    catch (Exception ex)
                    {
                        // Handle error
                    }
                }
            }
        }
    }
}