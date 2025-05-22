using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace assignmentDraft1
{
	public partial class uploadMaterial : System.Web.UI.Page
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

                // Check if courseId is passed in query string
                string courseId = Request.QueryString["courseId"];
                if (!string.IsNullOrEmpty(courseId))
                {
                    ddlCourse.SelectedValue = courseId;
                }
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

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && fileUpload.HasFile)
            {
                try
                {
                    // Validate file
                    if (!IsValidFile(fileUpload))
                    {
                        return;
                    }

                    string materialTitle = txtMaterialTitle.Text.Trim();
                    string description = txtDescription.Text.Trim();
                    string materialType = ddlMaterialType.SelectedValue;
                    int courseId = Convert.ToInt32(ddlCourse.SelectedValue);
                    int teacherId = Convert.ToInt32(Session["userId"]);

                    // Create upload directory if it doesn't exist
                    string uploadDir = Server.MapPath("~/uploads/materials/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // Generate unique filename using C# 7.3 compatible syntax
                    string fileExtension = Path.GetExtension(fileUpload.FileName);
                    string guidString = Guid.NewGuid().ToString("N");
                    string shortGuid = guidString.Substring(0, 8); // Instead of [..8]
                    string fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{shortGuid}{fileExtension}";
                    string filePath = Path.Combine(uploadDir, fileName);
                    string relativePath = $"~/uploads/materials/{fileName}";

                    // Save file
                    fileUpload.SaveAs(filePath);

                    // Save to database
                    string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        con.Open();

                        // Get next MaterialID (if using manual ID assignment)
                        string getMaxIdQuery = "SELECT ISNULL(MAX(MaterialID), 0) + 1 FROM Materials";
                        SqlCommand getMaxIdCmd = new SqlCommand(getMaxIdQuery, con);
                        int newMaterialId = (int)getMaxIdCmd.ExecuteScalar();

                        // Insert material record
                        string insertQuery = @"INSERT INTO Materials 
                                             (MaterialID, CourseID, MaterialTitle, Description, MaterialType, 
                                              FileName, FilePath, FileSize, UploadedBy, UploadDate, IsActive) 
                                             VALUES 
                                             (@materialId, @courseId, @title, @description, @type, 
                                              @fileName, @filePath, @fileSize, @uploadedBy, @uploadDate, 1)";

                        SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                        insertCmd.Parameters.AddWithValue("@materialId", newMaterialId);
                        insertCmd.Parameters.AddWithValue("@courseId", courseId);
                        insertCmd.Parameters.AddWithValue("@title", materialTitle);
                        insertCmd.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(description) ? (object)DBNull.Value : description);
                        insertCmd.Parameters.AddWithValue("@type", materialType);
                        insertCmd.Parameters.AddWithValue("@fileName", fileUpload.FileName);
                        insertCmd.Parameters.AddWithValue("@filePath", relativePath);
                        insertCmd.Parameters.AddWithValue("@fileSize", fileUpload.FileBytes.Length);
                        insertCmd.Parameters.AddWithValue("@uploadedBy", teacherId);
                        insertCmd.Parameters.AddWithValue("@uploadDate", DateTime.Now);

                        int result = insertCmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            ShowSuccess();
                            ClearForm();
                        }
                        else
                        {
                            // Delete uploaded file if database insert fails
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            ShowError("Failed to save material information. Please try again.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("An error occurred while uploading the material: " + ex.Message);
                }
            }
            else if (!fileUpload.HasFile)
            {
                ShowError("Please select a file to upload.");
            }
        }

        private bool IsValidFile(FileUpload fileUpload)
        {
            // Check file size (50MB limit)
            int maxFileSize = 50 * 1024 * 1024; // 50MB in bytes
            if (fileUpload.FileBytes.Length > maxFileSize)
            {
                ShowError("File size cannot exceed 50MB.");
                return false;
            }

            // Check file extension
            string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
            string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".mp4", ".mp3", ".zip", ".rar", ".txt", ".jpg", ".jpeg", ".png", ".gif" };

            bool isValidExtension = false;
            foreach (string ext in allowedExtensions)
            {
                if (fileExtension == ext)
                {
                    isValidExtension = true;
                    break;
                }
            }

            if (!isValidExtension)
            {
                ShowError("Invalid file format. Please upload a supported file type.");
                return false;
            }

            return true;
        }

        private void ShowSuccess()
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            lblErrorMessage.Text = message;
        }

        private void ClearForm()
        {
            txtMaterialTitle.Text = "";
            txtDescription.Text = "";
            ddlMaterialType.SelectedIndex = 0;
            ddlCourse.SelectedIndex = 0;
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