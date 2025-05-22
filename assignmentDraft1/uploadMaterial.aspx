<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="uploadMaterial.aspx.cs" Inherits="assignmentDraft1.uploadMaterial" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Upload Material - Bulb</title>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.1.2/css/all.min.css">
  <link rel="stylesheet" href="css/style.css">
</head>
<body>
  <form id="form1" runat="server" enctype="multipart/form-data">
    <!-- Header -->
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
        <a href="addCourse.aspx"><i class="fas fa-plus"></i><span>Add Course</span></a>
        <a href="manageStudents.aspx"><i class="fas fa-users"></i><span>Students</span></a>
        <a href="assignments.aspx"><i class="fas fa-tasks"></i><span>Assignments</span></a>
        <a href="uploadMaterial.aspx" class="active"><i class="fas fa-upload"></i><span>Upload Material</span></a>
        <a href="analytics.aspx"><i class="fas fa-chart-line"></i><span>Analytics</span></a>
        <a href="settings.aspx"><i class="fas fa-cog"></i><span>Settings</span></a>
        <a href="logout.aspx"><i class="fas fa-sign-out-alt"></i><span>Logout</span></a>
      </nav>
    </div>
    
    <!-- Main Content -->
    <div class="main-content">
      <div class="section">
        <div class="section-header">
          <h2 class="section-title">Upload Course Material</h2>
          <a href="TeacherDashboard.aspx" class="btn btn-outline"><i class="fas fa-arrow-left"></i> Back to Dashboard</a>
        </div>
        
        <!-- Success/Error messages -->
        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
          <i class="fas fa-check-circle"></i> Material uploaded successfully!
        </asp:Panel>
        
        <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-danger" Visible="false">
          <i class="fas fa-exclamation-circle"></i>
          <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
        </asp:Panel>
        
        <div class="form-container">
          <div class="form-group">
            <asp:Label ID="lblCourse" runat="server" CssClass="form-label" AssociatedControlID="ddlCourse">Select Course</asp:Label>
            <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-select">
              <asp:ListItem Text="Select a Course" Value="" Selected="True"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvCourse" runat="server" ControlToValidate="ddlCourse" 
                                      ErrorMessage="Please select a course" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
          </div>
          
          <div class="form-group">
            <asp:Label ID="lblMaterialTitle" runat="server" CssClass="form-label" AssociatedControlID="txtMaterialTitle">Material Title</asp:Label>
            <asp:TextBox ID="txtMaterialTitle" runat="server" CssClass="form-control" placeholder="Enter material title" Required="true"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvMaterialTitle" runat="server" ControlToValidate="txtMaterialTitle" 
                                      ErrorMessage="Material title is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
          </div>
          
          <div class="form-group">
            <asp:Label ID="lblDescription" runat="server" CssClass="form-label" AssociatedControlID="txtDescription">Description (Optional)</asp:Label>
            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-textarea" TextMode="MultiLine" 
                         placeholder="Enter material description" Rows="3"></asp:TextBox>
          </div>
          
          <div class="form-group">
            <asp:Label ID="lblMaterialType" runat="server" CssClass="form-label" AssociatedControlID="ddlMaterialType">Material Type</asp:Label>
            <asp:DropDownList ID="ddlMaterialType" runat="server" CssClass="form-select">
              <asp:ListItem Text="Select Type" Value="" Selected="True"></asp:ListItem>
              <asp:ListItem Text="Lecture Notes" Value="Lecture Notes"></asp:ListItem>
              <asp:ListItem Text="Assignment" Value="Assignment"></asp:ListItem>
              <asp:ListItem Text="Reading Material" Value="Reading Material"></asp:ListItem>
              <asp:ListItem Text="Video" Value="Video"></asp:ListItem>
              <asp:ListItem Text="Presentation" Value="Presentation"></asp:ListItem>
              <asp:ListItem Text="Code Sample" Value="Code Sample"></asp:ListItem>
              <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvMaterialType" runat="server" ControlToValidate="ddlMaterialType" 
                                      ErrorMessage="Please select material type" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
          </div>
          
          <div class="form-group">
            <asp:Label ID="lblFile" runat="server" CssClass="form-label" AssociatedControlID="fileUpload">Upload File</asp:Label>
            <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control" />
            <small class="form-text">Supported formats: PDF, DOC, DOCX, PPT, PPTX, XLS, XLSX, MP4, MP3, ZIP (Max 50MB)</small>
            <asp:RequiredFieldValidator ID="rfvFile" runat="server" ControlToValidate="fileUpload" 
                                      ErrorMessage="Please select a file to upload" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
          </div>
          
          <div class="form-actions">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
            <asp:Button ID="btnUpload" runat="server" Text="Upload Material" CssClass="btn" OnClick="btnUpload_Click" />
          </div>
        </div>
      </div>
    </div>
    
    <footer class="footer">
      &copy; copyright @ 2025 by <span><a href="https://github.com/stanlynilam" target="_blank" style="color: var(--primary); text-decoration: none;">Stanley Nilam</a></span> | all rights reserved!
    </footer>
  </form>

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
