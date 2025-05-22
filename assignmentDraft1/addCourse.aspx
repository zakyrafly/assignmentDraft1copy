<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="addCourse.aspx.cs" Inherits="assignmentDraft1.addCourse" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Add Course - Bulb</title>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.1.2/css/all.min.css">
  <link rel="stylesheet" href="css/style.css">
  <style>
    .form-container {
      max-width: 800px;
      margin: 0 auto;
      background: var(--white);
      padding: 2rem;
      border-radius: 0.5rem;
      box-shadow: var(--box-shadow);
    }
    
    .form-group {
      margin-bottom: 1.5rem;
    }
    
    .form-label {
      display: block;
      margin-bottom: 0.5rem;
      color: var(--black);
      font-weight: 500;
    }
    
    .form-control, .form-textarea, .form-select {
      width: 100%;
      padding: 1rem;
      border: var(--border);
      border-radius: 0.5rem;
      font-size: 1.6rem;
      color: var(--black);
      background: var(--light-bg);
    }
    
    .form-control:focus, .form-textarea:focus, .form-select:focus {
      outline: none;
      border-color: var(--primary);
    }
    
    .form-textarea {
      resize: vertical;
      min-height: 120px;
    }
    
    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 1.5rem;
    }
    
    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      margin-top: 2rem;
      padding-top: 2rem;
      border-top: var(--border);
    }
    
    .alert {
      padding: 1rem;
      border-radius: 0.5rem;
      margin-bottom: 1.5rem;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    
    .alert-success {
      background: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }
    
    .alert-danger {
      background: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }
    
    .text-danger {
      color: var(--red);
      font-size: 1.4rem;
      margin-top: 0.5rem;
      display: block;
    }
    
    .btn-link {
      color: var(--primary);
      text-decoration: none;
      margin-left: 1rem;
    }
    
    .btn-link:hover {
      text-decoration: underline;
    }
    
    @media (max-width: 768px) {
      .form-row {
        grid-template-columns: 1fr;
      }
      
      .form-actions {
        flex-direction: column;
      }
      
      .form-container {
        margin: 1rem;
        padding: 1.5rem;
      }
    }
  </style>
</head>
<body>
  <form id="form1" runat="server">
    <!-- Header -->
    <header class="header">
      <section class="flex">
        <a href="TeacherDashboard.aspx" class="logo">Bulb</a>
        
        <div class="search-form">
          <asp:TextBox ID="txtSearch" runat="server" placeholder="Search courses, students..."></asp:TextBox>
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
    
    <!-- Sidebar -->
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
        <a href="TeacherDashboard.aspx"><i class="fas fa-home"></i><span>Dashboard</span></a>
        <a href="addCourse.aspx" class="active"><i class="fas fa-plus"></i><span>Add Course</span></a>
        <a href="manageStudents.aspx"><i class="fas fa-users"></i><span>Students</span></a>
        <a href="assignments.aspx"><i class="fas fa-tasks"></i><span>Assignments</span></a>
        <a href="createAssignment.aspx"><i class="fas fa-plus-circle"></i><span>Create Assignment</span></a>
        <a href="uploadMaterial.aspx"><i class="fas fa-upload"></i><span>Upload Material</span></a>
        <a href="analytics.aspx"><i class="fas fa-chart-line"></i><span>Analytics</span></a>
        <a href="settings.aspx"><i class="fas fa-cog"></i><span>Settings</span></a>
        <a href="logout.aspx"><i class="fas fa-sign-out-alt"></i><span>Logout</span></a>
      </nav>
    </div>
    
    <!-- Main Content -->
    <div class="main-content">
      <div class="section">
        <div class="section-header">
          <h2 class="section-title">Add New Course</h2>
          <a href="TeacherDashboard.aspx" class="btn btn-outline"><i class="fas fa-arrow-left"></i> Back to Dashboard</a>
        </div>
        
        <!-- Success/Error messages -->
        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
          <i class="fas fa-check-circle"></i> Course added successfully!
          <asp:HyperLink ID="lnkViewCourse" runat="server" CssClass="btn-link">View Course</asp:HyperLink>
        </asp:Panel>
        
        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
          <i class="fas fa-exclamation-circle"></i>
          <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
        </asp:Panel>
        
        <div class="form-container">
          <div class="form-group">
            <asp:Label ID="lblCourseTitle" runat="server" CssClass="form-label" AssociatedControlID="txtCourseTitle">
              Course Title <span style="color: var(--red);">*</span>
            </asp:Label>
            <asp:TextBox ID="txtCourseTitle" runat="server" CssClass="form-control" 
                         placeholder="Enter course title (e.g., Introduction to Web Development)" 
                         MaxLength="255" Required="true"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvCourseTitle" runat="server" ControlToValidate="txtCourseTitle" 
                                      ErrorMessage="Course title is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
          </div>
          
          <div class="form-group">
            <asp:Label ID="lblCourseDescription" runat="server" CssClass="form-label" AssociatedControlID="txtCourseDescription">
              Course Description <span style="color: var(--red);">*</span>
            </asp:Label>
            <asp:TextBox ID="txtCourseDescription" runat="server" CssClass="form-textarea" TextMode="MultiLine" 
                         placeholder="Provide a detailed description of the course content, objectives, and what students will learn..." 
                         Rows="5" Required="true"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvCourseDescription" runat="server" ControlToValidate="txtCourseDescription" 
                                      ErrorMessage="Course description is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <asp:Label ID="lblCategory" runat="server" CssClass="form-label" AssociatedControlID="ddlCategory">
                Category <span style="color: var(--red);">*</span>
              </asp:Label>
              <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                <asp:ListItem Text="Select Category" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Programming" Value="Programming"></asp:ListItem>
                <asp:ListItem Text="Web Development" Value="Web Development"></asp:ListItem>
                <asp:ListItem Text="Data Science" Value="Data Science"></asp:ListItem>
                <asp:ListItem Text="Mobile Development" Value="Mobile Development"></asp:ListItem>
                <asp:ListItem Text="Game Development" Value="Game Development"></asp:ListItem>
                <asp:ListItem Text="Design" Value="Design"></asp:ListItem>
                <asp:ListItem Text="Business" Value="Business"></asp:ListItem>
                <asp:ListItem Text="Marketing" Value="Marketing"></asp:ListItem>
                <asp:ListItem Text="Science" Value="Science"></asp:ListItem>
                <asp:ListItem Text="Mathematics" Value="Mathematics"></asp:ListItem>
                <asp:ListItem Text="Language" Value="Language"></asp:ListItem>
                <asp:ListItem Text="Engineering" Value="Engineering"></asp:ListItem>
                <asp:ListItem Text="Arts" Value="Arts"></asp:ListItem>
                <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
              </asp:DropDownList>
              <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="ddlCategory" 
                                        ErrorMessage="Please select a category" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
          </div>
          
          <div class="form-actions">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" 
                        OnClick="btnCancel_Click" CausesValidation="false" />
            <asp:Button ID="btnAddCourse" runat="server" Text="Add Course" CssClass="btn" 
                        OnClick="btnAddCourse_Click" />
          </div>
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

    // Form enhancements
    document.addEventListener('DOMContentLoaded', function() {
      // Auto-resize textarea
      const textarea = document.getElementById('<%= txtCourseDescription.ClientID %>');
      if (textarea) {
        textarea.addEventListener('input', function() {
          this.style.height = 'auto';
          this.style.height = this.scrollHeight + 'px';
        });
      }

      // Character counter for title
      const titleInput = document.getElementById('<%= txtCourseTitle.ClientID %>');
      if (titleInput) {
        titleInput.addEventListener('input', function() {
          const maxLength = this.getAttribute('maxlength');
          const currentLength = this.value.length;
          
          // You can add a character counter here if needed
          if (currentLength > maxLength * 0.9) {
            this.style.borderColor = 'var(--warning)';
          } else {
            this.style.borderColor = '';
          }
        });
      }
    });
  </script>
</body>
</html>