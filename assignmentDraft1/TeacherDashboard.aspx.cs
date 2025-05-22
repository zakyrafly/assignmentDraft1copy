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
    public partial class TeacherDashboard : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["email"] == null)
                {
                    // Not logged in, redirect to login page
                    Response.Redirect("loginWebform.aspx");
                }
                else
                {
                    string email = Session["email"].ToString();
                    string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        string query = "SELECT Name, Role, UserID FROM Users WHERE Username = @Email";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Email", email);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            // Check if user is a teacher
                            if (reader["Role"].ToString().ToLower() != "teacher")
                            {
                                // Not a teacher, redirect to appropriate page
                                Response.Redirect("homeWebform.aspx");
                                return;
                            }

                            // Set teacher information
                            lblTeacherName.Text = reader["Name"].ToString();
                            lblTeacherRole.Text = reader["Role"].ToString();
                            lblSidebarTeacherName.Text = reader["Name"].ToString();
                            lblSidebarTeacherRole.Text = reader["Role"].ToString();

                            // Store UserID for later use
                            int userId = Convert.ToInt32(reader["UserID"]);
                            Session["UserID"] = userId;

                            // Close the reader before other commands
                            reader.Close();

                            // Load all dashboard data
                            LoadDashboardStats(userId);
                            LoadCourses(userId);
                            LoadLessons(userId);
                            LoadStudents(userId);

                            // These will need the new tables
                            // LoadAssignments(userId);
                            // LoadAnalytics(userId);
                        }
                        else
                        {
                            // User not found in database
                            Response.Redirect("loginWebform.aspx");
                        }
                    }
                }
            }
        }

        private void LoadDashboardStats(int teacherId)
        {
            try
            {
                List<DashboardStat> stats = new List<DashboardStat>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Count active courses
                    string coursesQuery = @"
                        SELECT COUNT(DISTINCT c.CourseID) 
                        FROM Courses c
                        JOIN Modules m ON c.CourseID = m.CourseID
                        WHERE m.LecturerID = @TeacherID";
                    SqlCommand coursesCmd = new SqlCommand(coursesQuery, conn);
                    coursesCmd.Parameters.AddWithValue("@TeacherID", teacherId);
                    int courseCount = Convert.ToInt32(coursesCmd.ExecuteScalar());
                    stats.Add(new DashboardStat { Icon = "fa-book", Value = courseCount.ToString(), Title = "Active Courses" });

                    // Count enrolled students
                    string studentsQuery = @"
                        SELECT COUNT(DISTINCT uc.UserID) 
                        FROM UserCourses uc
                        JOIN Courses c ON uc.CourseID = c.CourseID
                        JOIN Modules m ON c.CourseID = m.CourseID
                        JOIN Users u ON uc.UserID = u.UserID
                        WHERE m.LecturerID = @TeacherID
                        AND u.Role = 'student'";
                    SqlCommand studentsCmd = new SqlCommand(studentsQuery, conn);
                    studentsCmd.Parameters.AddWithValue("@TeacherID", teacherId);
                    int studentCount = Convert.ToInt32(studentsCmd.ExecuteScalar());
                    stats.Add(new DashboardStat { Icon = "fa-users", Value = studentCount.ToString(), Title = "Total Students" });

                    // Count active assignments - Use if you have the Assignments table
                    // Otherwise use placeholder value
                    try
                    {
                        string assignmentsQuery = @"
                            SELECT COUNT(*) 
                            FROM Assignments a
                            JOIN Courses c ON a.CourseID = c.CourseID
                            JOIN Modules m ON c.CourseID = m.CourseID
                            WHERE m.LecturerID = @TeacherID 
                            AND a.DueDate >= GETDATE()";
                        SqlCommand assignmentsCmd = new SqlCommand(assignmentsQuery, conn);
                        assignmentsCmd.Parameters.AddWithValue("@TeacherID", teacherId);
                        object assignmentResult = assignmentsCmd.ExecuteScalar();
                        int assignmentCount = assignmentResult != null ? Convert.ToInt32(assignmentResult) : 0;
                        stats.Add(new DashboardStat { Icon = "fa-tasks", Value = assignmentCount.ToString(), Title = "Active Assignments" });
                    }
                    catch
                    {
                        // Assignments table might not exist yet
                        stats.Add(new DashboardStat { Icon = "fa-tasks", Value = "0", Title = "Active Assignments" });
                    }

                    // For demonstration - use placeholder for completion rate or calculate if you have the data
                    stats.Add(new DashboardStat { Icon = "fa-check-circle", Value = "75", Title = "Completion Rate" });
                }

                // Bind stats to repeater
                rptStats.DataSource = stats;
                rptStats.DataBind();
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error loading dashboard stats: " + ex.Message);
            }
        }

        private void LoadCourses(int teacherId)
        {
            try
            {
                List<Course> courses = new List<Course>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT DISTINCT
                            c.CourseID, 
                            c.CourseName AS CourseTitle, 
                            c.Description AS CourseDescription,
                            (SELECT COUNT(*) FROM UserCourses uc WHERE uc.CourseID = c.CourseID) AS TotalStudents,
                            75 AS CompletionRate  -- Placeholder, calculate based on your requirements
                        FROM 
                            Courses c
                            JOIN Modules m ON c.CourseID = m.CourseID
                        WHERE 
                            m.LecturerID = @TeacherID
                        ORDER BY 
                            c.CourseName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseID = Convert.ToInt32(reader["CourseID"]),
                            CourseTitle = reader["CourseTitle"].ToString(),
                            CourseDescription = reader["CourseDescription"].ToString(),
                            TotalStudents = Convert.ToInt32(reader["TotalStudents"]),
                            CompletionRate = Convert.ToInt32(reader["CompletionRate"])
                        });
                    }
                    reader.Close();
                }

                // Display empty state if no courses
                if (courses.Count == 0)
                {
                    pnlNoCourses.Visible = true;
                }
                else
                {
                    // Bind courses to repeater
                    rptCourses.DataSource = courses;
                    rptCourses.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error loading courses: " + ex.Message);
            }
        }

        private void LoadLessons(int teacherId)
        {
            try
            {
                List<CourseWithMaterials> coursesWithMaterials = new List<CourseWithMaterials>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string courseQuery = @"
                        SELECT DISTINCT c.CourseID, c.CourseName as CourseTitle 
                        FROM Courses c
                        JOIN Modules m ON c.CourseID = m.CourseID
                        WHERE m.LecturerID = @TeacherID 
                        ORDER BY c.CourseName";

                    SqlCommand courseCmd = new SqlCommand(courseQuery, conn);
                    courseCmd.Parameters.AddWithValue("@TeacherID", teacherId);

                    conn.Open();
                    SqlDataReader courseReader = courseCmd.ExecuteReader();

                    while (courseReader.Read())
                    {
                        int courseId = Convert.ToInt32(courseReader["CourseID"]);
                        string courseTitle = courseReader["CourseTitle"].ToString();

                        // Create course with empty materials list
                        CourseWithMaterials courseWithMaterials = new CourseWithMaterials
                        {
                            CourseID = courseId,
                            CourseTitle = courseTitle,
                            Materials = new List<Material>()
                        };

                        coursesWithMaterials.Add(courseWithMaterials);
                    }
                    courseReader.Close();

                    // Now get lessons for each course (adapting to your Lessons table)
                    foreach (var course in coursesWithMaterials)
                    {
                        string materialQuery = @"
                            SELECT 
                                l.LessonID as MaterialID, 
                                l.Title as FileName, 
                                l.ContentType as FileType, 
                                l.ContentURL as FilePath,
                                l.TextContent
                            FROM 
                                Lessons l
                            JOIN
                                Modules m ON l.ModuleID = m.ModuleID
                            WHERE 
                                m.CourseID = @CourseID
                            ORDER BY 
                                m.ModuleOrder, l.Title";

                        SqlCommand materialCmd = new SqlCommand(materialQuery, conn);
                        materialCmd.Parameters.AddWithValue("@CourseID", course.CourseID);

                        SqlDataReader materialReader = materialCmd.ExecuteReader();
                        while (materialReader.Read())
                        {
                            course.Materials.Add(new Material
                            {
                                MaterialID = Convert.ToInt32(materialReader["MaterialID"]),
                                FileName = materialReader["FileName"].ToString(),
                                FileType = materialReader["FileType"].ToString(),
                                FilePath = materialReader["FilePath"].ToString()
                            });
                        }
                        materialReader.Close();
                    }
                }

                // Display empty state if no materials
                if (coursesWithMaterials.Count == 0 || coursesWithMaterials.All(c => c.Materials.Count == 0))
                {
                    pnlNoMaterials.Visible = true;
                }
                else
                {
                    // Bind courses with materials to repeater
                    rptLessons.DataSource = coursesWithMaterials;
                    rptLessons.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error loading lessons: " + ex.Message);
            }
        }

        private void LoadStudents(int teacherId)
        {
            try
            {
                List<Student> students = new List<Student>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT TOP 5
                            u.UserID AS StudentID,
                            u.Name AS StudentName,
                            u.ContactInfo AS StudentEmail,
                            50 AS Progress  -- Placeholder, calculate based on your requirements
                        FROM 
                            Users u
                            INNER JOIN UserCourses uc ON u.UserID = uc.UserID
                            INNER JOIN Courses c ON uc.CourseID = c.CourseID
                            INNER JOIN Modules m ON c.CourseID = m.CourseID
                        WHERE 
                            m.LecturerID = @TeacherID
                            AND u.Role = 'student'
                        GROUP BY 
                            u.UserID, u.Name, u.ContactInfo
                        ORDER BY 
                            u.Name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            StudentID = Convert.ToInt32(reader["StudentID"]),
                            StudentName = reader["StudentName"].ToString(),
                            StudentEmail = reader["StudentEmail"].ToString(),
                            Progress = Convert.ToInt32(reader["Progress"])
                        });
                    }
                    reader.Close();
                }

                // Display empty state if no students
                if (students.Count == 0)
                {
                    pnlNoStudents.Visible = true;
                }
                else
                {
                    // Bind students to repeater
                    rptStudents.DataSource = students;
                    rptStudents.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error loading students: " + ex.Message);
            }
        }

        private void LoadAssignments(int teacherId)
        {
            try
            {
                List<Assignment> assignments = new List<Assignment>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // This is a placeholder query - adjust it based on your actual database structure
                    // If the Assignments table doesn't exist yet, you'll need to modify this
                    string query = @"
                        SELECT TOP 6
                            a.AssignmentID,
                            a.Title AS AssignmentTitle,
                            c.CourseName AS CourseTitle,
                            a.DueDate,
                            (SELECT COUNT(*) FROM Submissions s WHERE s.AssignmentID = a.AssignmentID) AS SubmissionCount,
                            (SELECT COUNT(*) FROM UserCourses uc WHERE uc.CourseID = c.CourseID) AS TotalStudents,
                            CASE 
                                WHEN a.DueDate < GETDATE() AND EXISTS (SELECT 1 FROM Submissions s WHERE s.AssignmentID = a.AssignmentID AND s.Grade IS NOT NULL) THEN 'Graded'
                                WHEN a.DueDate < GETDATE() THEN 'Pending Review'
                                ELSE 'Active'
                            END AS Status
                        FROM 
                            Assignments a
                            INNER JOIN Courses c ON a.CourseID = c.CourseID
                            INNER JOIN Modules m ON c.CourseID = m.CourseID
                        WHERE 
                            m.LecturerID = @TeacherID
                        ORDER BY 
                            a.DueDate";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        assignments.Add(new Assignment
                        {
                            AssignmentID = Convert.ToInt32(reader["AssignmentID"]),
                            AssignmentTitle = reader["AssignmentTitle"].ToString(),
                            CourseTitle = reader["CourseTitle"].ToString(),
                            DueDate = Convert.ToDateTime(reader["DueDate"]),
                            SubmissionCount = Convert.ToInt32(reader["SubmissionCount"]),
                            TotalStudents = Convert.ToInt32(reader["TotalStudents"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                    reader.Close();
                }

                // Display empty state if no assignments
                if (assignments.Count == 0)
                {
                    pnlNoAssignments.Visible = true;
                }
                else
                {
                    // Bind assignments to repeater
                    rptAssignments.DataSource = assignments;
                    rptAssignments.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Assignments table might not exist yet
                pnlNoAssignments.Visible = true;
                // Log error
                System.Diagnostics.Debug.WriteLine("Error loading assignments: " + ex.Message);
            }
        }

        private void LoadAnalytics(int teacherId)
        {
            try
            {
                List<CourseAnalytics> analytics = new List<CourseAnalytics>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // This is a placeholder query - modify for your actual database
                    // If some tables don't exist yet, this will need adjustment
                    string query = @"
                        SELECT 
                            c.CourseID,
                            c.CourseName AS CourseTitle,
                            COUNT(DISTINCT uc.UserID) AS TotalStudents,
                            75 AS CompletionRate, -- Placeholder
                            85 AS AverageGrade,   -- Placeholder
                            45 AS AvgActiveTime   -- Placeholder
                        FROM 
                            Courses c
                            JOIN Modules m ON c.CourseID = m.CourseID
                            LEFT JOIN UserCourses uc ON c.CourseID = uc.CourseID
                        WHERE 
                            m.LecturerID = @TeacherID
                        GROUP BY 
                            c.CourseID, c.CourseName
                        ORDER BY 
                            COUNT(DISTINCT uc.UserID) DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        analytics.Add(new CourseAnalytics
                        {
                            CourseID = Convert.ToInt32(reader["CourseID"]),
                            CourseTitle = reader["CourseTitle"].ToString(),
                            TotalStudents = Convert.ToInt32(reader["TotalStudents"]),
                            CompletionRate = Convert.ToInt32(reader["CompletionRate"]),
                            AverageGrade = Convert.ToInt32(reader["AverageGrade"]),
                            AvgActiveTime = Convert.ToInt32(reader["AvgActiveTime"])
                        });
                    }
                    reader.Close();
                }

                // Display empty state if no analytics
                if (analytics.Count == 0)
                {
                    pnlNoAnalytics.Visible = true;
                }
                else
                {
                    // Bind analytics to repeater
                    rptAnalytics.DataSource = analytics;
                    rptAnalytics.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Analytics might depend on tables that don't exist yet
                pnlNoAnalytics.Visible = true;
                // Log error
                System.Diagnostics.Debug.WriteLine("Error loading analytics: " + ex.Message);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Get search term
            string searchTerm = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Redirect to search results page with the search term
                Response.Redirect($"search.aspx?q={HttpUtility.UrlEncode(searchTerm)}");
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            // Clear session and redirect to login page
            Session.Clear();
            Response.Redirect("loginWebform.aspx");
        }

        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCourse")
            {
                int courseId = Convert.ToInt32(e.CommandArgument);
                DeleteCourse(courseId);

                // Reload course data after deletion
                int teacherId = Convert.ToInt32(Session["UserID"]);
                LoadCourses(teacherId);
                LoadDashboardStats(teacherId);
            }
        }

        private void DeleteCourse(int courseId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // This assumes your Courses table has an IsActive field
                    // If not, you'll need to adjust this query
                    string query = "UPDATE Courses SET IsActive = 0 WHERE CourseID = @CourseID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CourseID", courseId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error deleting course: " + ex.Message);
            }
        }

        // Helper method to get file icon based on file type
        protected string GetFileIcon(string fileType)
        {
            switch (fileType.ToLower())
            {
                case "pdf":
                    return "fa-file-pdf";
                case "doc":
                case "docx":
                    return "fa-file-word";
                case "xls":
                case "xlsx":
                    return "fa-file-excel";
                case "ppt":
                case "pptx":
                    return "fa-file-powerpoint";
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                    return "fa-file-image";
                case "mp4":
                case "avi":
                case "mov":
                    return "fa-file-video";
                case "mp3":
                case "wav":
                    return "fa-file-audio";
                case "zip":
                case "rar":
                    return "fa-file-archive";
                case "cs":
                case "js":
                case "html":
                case "css":
                case "php":
                    return "fa-file-code";
                default:
                    return "fa-file";
            }
        }

        // Helper method to get initials from name
        protected string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "";

            string[] nameParts = name.Split(' ');
            if (nameParts.Length >= 2)
            {
                return (nameParts[0][0].ToString() + nameParts[nameParts.Length - 1][0].ToString()).ToUpper();
            }
            else if (nameParts.Length == 1 && nameParts[0].Length > 0)
            {
                return nameParts[0][0].ToString().ToUpper();
            }

            return "";
        }

        // Helper method to get status color
        protected string GetStatusColor(string status)
        {
            switch (status)
            {
                case "Active":
                    return "var(--primary)";
                case "Pending Review":
                    return "var(--warning)";
                case "Graded":
                    return "var(--success)";
                default:
                    return "var(--text)";
            }
        }

        // Helper method to get action button class
        protected string GetActionBtnClass(string status)
        {
            switch (status)
            {
                case "Active":
                    return "btn btn-outline";
                case "Pending Review":
                    return "btn btn-success";
                case "Graded":
                    return "btn btn-outline";
                default:
                    return "btn";
            }
        }

        // Helper method to get action button text
        protected string GetActionText(string status)
        {
            switch (status)
            {
                case "Active":
                    return "View Assignment";
                case "Pending Review":
                    return "Grade Submissions";
                case "Graded":
                    return "View Grades";
                default:
                    return "View";
            }
        }

        // Helper method to get action URL
        protected string GetActionUrl(string assignmentId, string status)
        {
            switch (status)
            {
                case "Active":
                    return $"viewAssignment.aspx?id={assignmentId}";
                case "Pending Review":
                    return $"gradeSubmissions.aspx?id={assignmentId}";
                case "Graded":
                    return $"viewGrades.aspx?id={assignmentId}";
                default:
                    return $"viewAssignment.aspx?id={assignmentId}";
            }
        }
    }

    // Class to represent dashboard statistics
    public class DashboardStat
    {
        public string Icon { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
    }

    // Class to represent course data
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDescription { get; set; }
        public int TotalStudents { get; set; }
        public int CompletionRate { get; set; }
    }

    // Class to represent course with materials
    public class CourseWithMaterials
    {
        public int CourseID { get; set; }
        public string CourseTitle { get; set; }
        public List<Material> Materials { get; set; }
    }

    // Class to represent material data
    public class Material
    {
        public int MaterialID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
    }

    // Class to represent student data
    public class Student
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public int Progress { get; set; }
    }

    // Class to represent assignment data
    public class Assignment
    {
        public int AssignmentID { get; set; }
        public string AssignmentTitle { get; set; }
        public string CourseTitle { get; set; }
        public DateTime DueDate { get; set; }
        public int SubmissionCount { get; set; }
        public int TotalStudents { get; set; }
        public string Status { get; set; }
    }

    // Class to represent course analytics data
    public class CourseAnalytics
    {
        public int CourseID { get; set; }
        public string CourseTitle { get; set; }
        public int TotalStudents { get; set; }
        public int CompletionRate { get; set; }
        public int AverageGrade { get; set; }
        public int AvgActiveTime { get; set; }
    }
}