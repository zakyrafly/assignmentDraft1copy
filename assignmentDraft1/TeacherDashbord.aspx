<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TeacherDashbord.aspx.cs" Inherits="assignmentDraft1.TeacherDashbord" %>

<script runat="server">
    // Helper method to get file icon based on file type
    protected string GetFileIcon(string fileType)
    {
        switch (fileType.ToLower())
        {
            case "pdf": return "fa-file-pdf";
            case "doc": case "docx": return "fa-file-word";
            case "xls": case "xlsx": return "fa-file-excel";
            case "ppt": case "pptx": return "fa-file-powerpoint";
            case "jpg": case "jpeg": case "png": case "gif": return "fa-file-image";
            case "mp4": case "avi": case "mov": return "fa-file-video";
            case "mp3": case "wav": return "fa-file-audio";
            case "zip": case "rar": return "fa-file-archive";
            case "cs": case "js": case "html": case "css": case "php": return "fa-file-code";
            default: return "fa-file";
        }
    }

    // Helper method to get initials from name
    protected string GetInitials(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";
        string[] nameParts = name.Split(' ');
        if (nameParts.Length >= 2)
            return (nameParts[0][0].ToString() + nameParts[nameParts.Length - 1][0].ToString()).ToUpper();
        else if (nameParts.Length == 1 && nameParts[0].Length > 0)
            return nameParts[0][0].ToString().ToUpper();
        return "";
    }

    // Helper method to get status color
    protected string GetStatusColor(string status)
    {
        switch (status)
        {
            case "Active": return "var(--primary)";
            case "Pending Review": return "var(--warning)";
            case "Graded": return "var(--success)";
            default: return "var(--text)";
        }
    }

    // Helper method to get action button class
    protected string GetActionBtnClass(string status)
    {
        switch (status)
        {
            case "Active": return "btn btn-outline";
            case "Pending Review": return "btn btn-success";
            case "Graded": return "btn btn-outline";
            default: return "btn";
        }
    }

    // Helper method to get action button text
    protected string GetActionText(string status)
    {
        switch (status)
        {
            case "Active": return "View Assignment";
            case "Pending Review": return "Grade Submissions";
            case "Graded": return "View Grades";
            default: return "View";
        }
    }

    // Helper method to get action URL
    protected string GetActionUrl(string assignmentId, string status)
    {
        switch (status)
        {
            case "Active": return $"viewAssignment.aspx?id={assignmentId}";
            case "Pending Review": return $"gradeSubmissions.aspx?id={assignmentId}";
            case "Graded": return $"viewGrades.aspx?id={assignmentId}";
            default: return $"viewAssignment.aspx?id={assignmentId}";
        }
    }
</script>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Teacher Dashboard - Bulb</title>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.1.2/css/all.min.css">
  <link rel="stylesheet" href="css/style.css">
</head>
<body>
  <form id="form1" runat="server">
    <header class="header">
      <section class="flex">
        <a href="TeacherDashboard.aspx" class="logo">Bulb</a>
        
        <div class="search-form">
          <asp:TextBox ID="txtSearch" runat="server" placeholder="Search courses, students..." Required="true"></asp:TextBox>
          <asp:Button ID="btnSearch" runat="server" CssClass="fas fa-search" OnClick="btnSearch_Click" />
        </div>
        
        <div class="icons">
          <div id="menu-btn" class="fas fa-bars"></div>
          <div id="search-btn" class="fas fa-search"></div>
          <div id="user-btn" class="fas fa-user"></div>
          <div id="toggle-btn" class="fas fa-sun"></div>
        </div>

        <div class="profile">
          <img src="images/pic-1.jpg" class="image" alt="Teacher Profile">
          <h3 class="name"><asp:Label ID="lblTeacherName" runat="server" Text="Teacher Name"></asp:Label></h3>
          <p class="role"><asp:Label ID="lblTeacherRole" runat="server" Text="teacher"></asp:Label></p>
          <a href="profile.aspx" class="btn">View Profile</a>
          <div class="flex-btn">
            <asp:LinkButton ID="lnkLogout" runat="server" CssClass="option-btn" OnClick="lnkLogout_Click">Logout</asp:LinkButton>
          </div>
        </div>
      </section>
    </header>
    <div class="side-bar">
      <div id="close-btn">
        <i class="fas fa-times"></i>
      </div>

      <div class="profile">
        <img src="images/pic-1.jpg" class="image" alt="Teacher Profile">
        <h3 class="name"><asp:Label ID="lblSidebarTeacherName" runat="server" Text="Teacher Name"></asp:Label></h3>
        <p class="role"><asp:Label ID="lblSidebarTeacherRole" runat="server" Text="teacher"></asp:Label></p>
        <a href="profile.aspx" class="btn">View Profile</a>
      </div>
      
      <nav class="navbar">
        <a href="TeacherDashboard.aspx" class="active"><i class="fas fa-home"></i><span>Dashboard</span></a>
        <a href="addCourse.aspx"><i class="fas fa-plus"></i><span>Add Course</span></a>
        <a href="manageStudents.aspx"><i class="fas fa-users"></i><span>Students</span></a>
        <a href="assignments.aspx"><i class="fas fa-tasks"></i><span>Assignments</span></a>
        <a href="analytics.aspx"><i class="fas fa-chart-line"></i><span>Analytics</span></a>
        <a href="settings.aspx"><i class="fas fa-cog"></i><span>Settings</span></a>
        <a href="logout.aspx"><i class="fas fa-sign-out-alt"></i><span>Logout</span></a>
      </nav>
    </div>
    <div class="main-content">
      <div class="section">
        <div class="card-grid">
          <asp:Repeater ID="rptStats" runat="server">
            <ItemTemplate>
              <div class="stats-card">
                <div class="stats-icon">
                  <i class="fas <%# Eval("Icon") %>"></i>
                </div>
                <div class="stats-info">
                  <h3><%# Eval("Value") %></h3>
                  <p><%# Eval("Title") %></p>
                </div>
              </div>
            </ItemTemplate>
          </asp:Repeater>
        </div>
      </div>
      
      <!-- Course Management -->
      <div class="section">
        <div class="section-header">
          <h2 class="section-title">Course Management</h2>
          <a href="addCourse.aspx" class="btn inline-btn"><i class="fas fa-plus"></i> Add New Course</a>
        </div>
        
        <!-- Empty state message when no courses are available -->
        <asp:Panel ID="pnlNoCourses" runat="server" CssClass="empty-state" Visible="false">
          <i class="fas fa-book"></i>
          <h3>No courses yet</h3>
          <p>Start by creating your first course</p>
          <a href="addCourse.aspx" class="btn">Add Course</a>
        </asp:Panel>
        
        <div class="card-grid">
          <asp:Repeater ID="rptCourses" runat="server" OnItemCommand="rptCourses_ItemCommand">
            <ItemTemplate>
              <div class="card">
                <div class="card-header">
                  <h3 class="card-title"><%# Eval("CourseTitle") %></h3>
                </div>
                <div class="card-body">
                  <p class="card-text"><%# Eval("CourseDescription") %></p>
                  <div class="progress-bar">
                    <div class="fill" style="width: <%# Eval("CompletionRate") %>%"></div>
                  </div>
                  <div style="display: flex; justify-content: space-between; font-size: 0.85rem;">
                    <span><%# Eval("TotalStudents") %> Students</span>
                    <span><%# Eval("CompletionRate") %>% Completion</span>
                  </div>
                </div>
                <div class="card-footer">
                  <asp:HyperLink ID="lnkEdit" runat="server" CssClass="btn btn-outline" 
                                 NavigateUrl='<%# "editCourse.aspx?id=" + Eval("CourseID") %>'>Edit</asp:HyperLink>
                  <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger" 
                                  CommandName="DeleteCourse" CommandArgument='<%# Eval("CourseID") %>'
                                  OnClientClick="return confirm('Are you sure you want to delete this course?');">
                    Delete
                  </asp:LinkButton>
                </div>
              </div>
            </ItemTemplate>
          </asp:Repeater>
        </div>
      </div>
      
      <!-- Lessons & Materials -->
      <div class="section">
        <div class="section-header">
          <h2 class="section-title">Lessons & Materials</h2>
          <a href="uploadMaterial.aspx" class="btn inline-btn"><i class="fas fa-upload"></i> Upload Materials</a>
        </div>
        
        <!-- Empty state message when no materials are available -->
        <asp:Panel ID="pnlNoMaterials" runat="server" CssClass="empty-state" Visible="false">
          <i class="fas fa-file-alt"></i>
          <h3>No course materials yet</h3>
          <p>Start by uploading materials to your courses</p>
          <a href="uploadMaterial.aspx" class="btn">Upload Materials</a>
        </asp:Panel>
        
        <div class="card-grid">
          <asp:Repeater ID="rptLessons" runat="server">
            <ItemTemplate>
              <div class="card">
                <div class="card-header">
                  <h3 class="card-title"><%# Eval("CourseTitle") %></h3>
                </div>
                <div class="card-body">
                  <asp:Repeater ID="rptMaterials" runat="server" DataSource='<%# Eval("Materials") %>'>
                    <HeaderTemplate>
                      <ul style="list-style: none; padding: 0;">
                    </HeaderTemplate>
                    <ItemTemplate>
                      <li style="display: flex; align-items: center; margin-bottom: 0.75rem;">
                        <i class="fas <%# GetFileIcon(Eval("FileType").ToString()) %>" style="margin-right: 0.75rem;"></i>
                        <span><%# Eval("FileName") %></span>
                        <a href='<%# Eval("FilePath") %>' style="margin-left: auto; color: var(--primary);">
                          <i class="fas fa-download"></i>
                        </a>
                      </li>
                    </ItemTemplate>
                    <FooterTemplate>
                      </ul>
                    </FooterTemplate>
                  </asp:Repeater>
                </div>
                <div class="card-footer">
                  <a href='uploadMaterial.aspx?courseId=<%# Eval("CourseID") %>' class="btn btn-outline">
                    <i class="fas fa-plus"></i> Add Material
                  </a>
                </div>
              </div>
            </ItemTemplate>
          </asp:Repeater>
        </div>
      </div>
      
      <!-- Student Enrollments -->
      <div class="section">
        <div class="section-header">
          <h2 class="section-title">Student Enrollments</h2>
          <a href="manageStudents.aspx" class="btn inline-btn"><i class="fas fa-users"></i> View All Students</a>
        </div>
        
        <!-- Empty state message when no students are enrolled -->
        <asp:Panel ID="pnlNoStudents" runat="server" CssClass="empty-state" Visible="false">
          <i class="fas fa-user-graduate"></i>
          <h3>No enrolled students yet</h3>
          <p>Students will appear here once they enroll in your courses</p>
        </asp:Panel>
        
        <div class="card">
          <div class="card-body">
            <asp:Repeater ID="rptStudents" runat="server">
              <HeaderTemplate>
                <ul class="student-list">
              </HeaderTemplate>
              <ItemTemplate>
                <li class="student-item">
                  <div class="student-avatar"><%# GetInitials(Eval("StudentName").ToString()) %></div>
                  <div class="student-details">
                    <h4 class="student-name"><%# Eval("StudentName") %></h4>
                    <p class="student-email"><%# Eval("StudentEmail") %></p>
                  </div>
                  <div class="student-progress"><%# Eval("Progress") %>%</div>
                </li>
              </ItemTemplate>
              <FooterTemplate>
                </ul>
              </FooterTemplate>
            </asp:Repeater>
          </div>
        </div>
      </div>
      
      <!-- Assignments & Grading -->
      <div class="section">
        <div class="section-header">
          <h2 class="section-title">Assignments & Grading</h2>
          <a href="createAssignment.aspx" class="btn inline-btn"><i class="fas fa-plus"></i> Create Assignment</a>
        </div>
        
        <!-- Empty state message when no assignments exist -->
        <asp:Panel ID="pnlNoAssignments" runat="server" CssClass="empty-state" Visible="false">
          <i class="fas fa-tasks"></i>
          <h3>No assignments yet</h3>
          <p>Create assignments for your students to complete</p>
          <a href="createAssignment.aspx" class="btn">Create Assignment</a>
        </asp:Panel>
        
        <div class="card-grid">
          <asp:Repeater ID="rptAssignments" runat="server">
            <ItemTemplate>
              <div class="card">
                <div class="card-header">
                  <h3 class="card-title"><%# Eval("AssignmentTitle") %></h3>
                </div>
                <div class="card-body">
                  <p><strong>Course:</strong> <%# Eval("CourseTitle") %></p>
                  <p><strong>Due Date:</strong> <%# Eval("DueDate", "{0:MMM dd, yyyy}") %></p>
                  <p><strong>Submissions:</strong> <%# Eval("SubmissionCount") %>/<%# Eval("TotalStudents") %> students</p>
                  <p><strong>Status:</strong> 
                    <span style='color: <%# GetStatusColor(Eval("Status").ToString()) %>;'>
                      <%# Eval("Status") %>
                    </span>
                  </p>
                </div>
                <div class="card-footer">
                  <asp:HyperLink ID="lnkGrade" runat="server" CssClass='<%# GetActionBtnClass(Eval("Status").ToString()) %>'
                                NavigateUrl='<%# GetActionUrl(Eval("AssignmentID").ToString(), Eval("Status").ToString()) %>'>
                    <%# GetActionText(Eval("Status").ToString()) %>
                  </asp:HyperLink>
                </div>
              </div>
            </ItemTemplate>
          </asp:Repeater>
        </div>
      </div>
      
      <!-- Course Analytics -->
      <div class="section">
        <div class="section-header">
          <h2 class="section-title">Course Analytics</h2>
          <a href="analytics.aspx" class="btn inline-btn"><i class="fas fa-chart-line"></i> View Detailed Analytics</a>
        </div>
        
        <!-- Empty state message when no analytics data is available -->
        <asp:Panel ID="pnlNoAnalytics" runat="server" CssClass="empty-state" Visible="false">
          <i class="fas fa-chart-pie"></i>
          <h3>No analytics data yet</h3>
          <p>Analytics will be available once students start engaging with your courses</p>
        </asp:Panel>
        
        <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 1.5rem;">
          <asp:Repeater ID="rptAnalytics" runat="server">
            <ItemTemplate>
              <div class="card">
                <div class="card-header">
                  <h3 class="card-title"><%# Eval("CourseTitle") %></h3>
                </div>
                <div class="card-body" style="padding: 1.5rem;">
                  <div style="display: flex; justify-content: space-between; margin-bottom: 1rem;">
                    <div>
                      <p style="margin-bottom: 0.5rem; color: var(--secondary);">Total Students</p>
                      <h3><%# Eval("TotalStudents") %></h3>
                    </div>
                    <div>
                      <p style="margin-bottom: 0.5rem; color: var(--secondary);">Completion Rate</p>
                      <h3><%# Eval("CompletionRate") %>%</h3>
                    </div>
                  </div>
                  <div style="display: flex; justify-content: space-between;">
                    <div>
                      <p style="margin-bottom: 0.5rem; color: var(--secondary);">Average Grade</p>
                      <h3><%# Eval("AverageGrade") %>/100</h3>
                    </div>
                    <div>
                      <p style="margin-bottom: 0.5rem; color: var(--secondary);">Active Time</p>
                      <h3><%# Eval("AvgActiveTime") %> min/day</h3>
                    </div>
                  </div>
                </div>
              </div>
            </ItemTemplate>
          </asp:Repeater>
        </div>
      </div>
    </div>
    
    <footer class="footer">
      &copy; copyright @ 2025 by <span><a href="https://github.com/stanlynilam" target="_blank" style="color: var(--primary); text-decoration: none;">Stanley Nilam</a></span> | all rights reserved!
    </footer>
  </form>

  <script src="js/script.js"></script>

  <script>
    // Show/hide sidebar on mobile
    document.querySelector('#menu-btn').addEventListener('click', function() {
      document.querySelector('.side-bar').classList.toggle('active');
    });

    // Close sidebar on mobile
    document.querySelector('#close-btn').addEventListener('click', function() {
      document.querySelector('.side-bar').classList.remove('active');
    });

    // Toggle dark/light mode
    document.querySelector('#toggle-btn').addEventListener('click', function() {
      document.body.classList.toggle('dark-mode');
      this.classList.toggle('fa-sun');
      this.classList.toggle('fa-moon');
    });

    // Show/hide user profile
    document.querySelector('#user-btn').addEventListener('click', function() {
      document.querySelector('.header .profile').classList.toggle('active');
    });

    // Show/hide search form on mobile
    document.querySelector('#search-btn').addEventListener('click', function() {
      document.querySelector('.header .search-form').classList.toggle('active');
      this.classList.toggle('fa-times');
    });
  </script>
</body>
</html>
