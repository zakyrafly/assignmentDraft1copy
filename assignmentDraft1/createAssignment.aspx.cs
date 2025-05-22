using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace assignmentDraft1
{
	public partial class createAssignment : System.Web.UI.Page
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
                LoadCourses();
                SetMinimumDate();
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

        private void LoadCourses()
        {
            string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
            int teacherId = Convert.ToInt32(Session["userId"]);

            using (SqlConnection con = new SqlConnection(cs))
            {
                // Adjust this query based on your actual Courses table structure
                string query = "SELECT CourseID, CourseName FROM Courses WHERE CreatedBy = @teacherId AND ISNULL(IsActive, 1) = 1 ORDER BY CourseName";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@teacherId", teacherId);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlCourse.Items.Clear();
                    ddlCourse.Items.Add(new ListItem("Select a Course", ""));

                    while (reader.Read())
                    {
                        ddlCourse.Items.Add(new ListItem(reader["CourseName"].ToString(), reader["CourseID"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Error loading courses: " + ex.Message);
                }
            }
        }

        private void SetMinimumDate()
        {
            // Set minimum date validation to current date/time
            cvDueDate.ValueToCompare = DateTime.Now.ToString("yyyy-MM-dd");
        }

        protected void btnCreateAssignment_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                CreateAssignment(false); // false = not a draft
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            CreateAssignment(true); // true = save as draft
        }

        private void CreateAssignment(bool isDraft)
        {
            try
            {
                string assignmentTitle = txtAssignmentTitle.Text.Trim();
                string description = txtDescription.Text.Trim();
                DateTime dueDate = Convert.ToDateTime(txtDueDate.Text);
                int maxPoints = Convert.ToInt32(txtMaxPoints.Text);
                string assignmentType = ddlAssignmentType.SelectedValue;
                string submissionFormat = ddlSubmissionFormat.SelectedValue;
                string instructions = txtInstructions.Text.Trim();
                int courseId = Convert.ToInt32(ddlCourse.SelectedValue);
                int teacherId = Convert.ToInt32(Session["userId"]);

                string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    // Check if assignment with same title already exists for this course
                    string checkQuery = "SELECT COUNT(*) FROM Assignments WHERE AssignmentTitle = @title AND CourseID = @courseId";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@title", assignmentTitle);
                    checkCmd.Parameters.AddWithValue("@courseId", courseId);

                    int existingAssignments = (int)checkCmd.ExecuteScalar();
                    if (existingAssignments > 0)
                    {
                        ShowError("An assignment with this title already exists for the selected course.");
                        return;
                    }

                    // Get next AssignmentID (if using manual ID assignment)
                    string getMaxIdQuery = "SELECT ISNULL(MAX(AssignmentID), 0) + 1 FROM Assignments";
                    SqlCommand getMaxIdCmd = new SqlCommand(getMaxIdQuery, con);
                    int newAssignmentId = (int)getMaxIdCmd.ExecuteScalar();

                    // Insert new assignment
                    string insertQuery = @"INSERT INTO Assignments 
                                         (AssignmentID, CourseID, AssignmentTitle, Description, DueDate, 
                                          MaxPoints, AssignmentType, SubmissionFormat, Instructions, 
                                          CreatedBy, CreatedDate, Status, IsActive) 
                                         VALUES 
                                         (@assignmentId, @courseId, @title, @description, @dueDate, 
                                          @maxPoints, @assignmentType, @submissionFormat, @instructions, 
                                          @createdBy, @createdDate, @status, 1)";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@assignmentId", newAssignmentId);
                    insertCmd.Parameters.AddWithValue("@courseId", courseId);
                    insertCmd.Parameters.AddWithValue("@title", assignmentTitle);
                    insertCmd.Parameters.AddWithValue("@description", description);
                    insertCmd.Parameters.AddWithValue("@dueDate", dueDate);
                    insertCmd.Parameters.AddWithValue("@maxPoints", maxPoints);
                    insertCmd.Parameters.AddWithValue("@assignmentType", assignmentType);
                    insertCmd.Parameters.AddWithValue("@submissionFormat", submissionFormat);
                    insertCmd.Parameters.AddWithValue("@instructions", string.IsNullOrWhiteSpace(instructions) ? (object)DBNull.Value : instructions);
                    insertCmd.Parameters.AddWithValue("@createdBy", teacherId);
                    insertCmd.Parameters.AddWithValue("@createdDate", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@status", isDraft ? "Draft" : "Active");

                    int result = insertCmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        ShowSuccess(newAssignmentId, isDraft);
                        if (!isDraft)
                        {
                            ClearForm();
                        }
                    }
                    else
                    {
                        ShowError("Failed to create assignment. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("An error occurred while creating the assignment: " + ex.Message);
            }
        }

        private void ShowSuccess(int assignmentId, bool isDraft)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;

            // Update the success message
            if (isDraft)
            {
                lblSuccessMessage.Text = "Assignment saved as draft successfully!";
            }
            else
            {
                lblSuccessMessage.Text = "Assignment created successfully!";
            }

            // Set the link URL for viewing the assignment
            lnkViewAssignment.NavigateUrl = $"viewAssignment.aspx?id={assignmentId}";
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblErrorMessage.Text = message;
        }

        private void ClearForm()
        {
            txtAssignmentTitle.Text = "";
            txtDescription.Text = "";
            txtDueDate.Text = "";
            txtMaxPoints.Text = "";
            txtInstructions.Text = "";
            ddlCourse.SelectedIndex = 0;
            ddlAssignmentType.SelectedIndex = 0;
            ddlSubmissionFormat.SelectedIndex = 0;
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