<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="createAssignment.aspx.cs" Inherits="assignmentDraft1.createAssignment" %>

<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Create Assignment - Bulb</title>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.1.2/css/all.min.css">
  <link rel="stylesheet" href="css/style.css">
</head>
<body>
  <form id="form1" runat="server">
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
        <a href="createAssignment.aspx" class="active"><i class="fas fa-plus-circle"></i><span>Create Assignment</span></a>
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
          <h2 class="section-title">Create New Assignment</h2>
          <a href="TeacherDashboard.aspx" class="btn btn-outline"><i class="fas fa-arrow-left"></i> Back to Dashboard</a>
        </div>
        
        <!-- Success/Error messages -->
        <asp:Panel ID="pnlSuccess" runat="server" CssClass="alert alert-success" Visible="false">
          <i class="fas fa-check-circle"></i> 
          <asp:Label ID="lblSuccessMessage" runat="server" Text="Assignment created successfully!"></asp:Label>
          <asp:HyperLink ID="lnkViewAssignment" runat="server" CssClass="btn-link">View Assignment</asp:HyperLink>
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
            <asp:Label ID="lblAssignmentTitle" runat="server" CssClass="form-label" AssociatedControlID="txtAssignmentTitle">Assignment Title</asp:Label>
            <asp:TextBox ID="txtAssignmentTitle" runat="server" CssClass="form-control" placeholder="Enter assignment title" Required="true"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvAssignmentTitle" runat="server" ControlToValidate="txtAssignmentTitle" 
                                      ErrorMessage="Assignment title is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
          </div>
          
          <div class="form-group">
            <asp:Label ID="lblDescription" runat="server" CssClass="form-label" AssociatedControlID="txtDescription">Assignment Description</asp:Label>
            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-textarea" TextMode="MultiLine" 
                         placeholder="Enter assignment description and instructions" Rows="6" Required="true"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" 
                                      ErrorMessage="Assignment description is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <asp:Label ID="lblDueDate" runat="server" CssClass="form-label" AssociatedControlID="txtDueDate">Due Date</asp:Label>
              <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control" TextMode="DateTimeLocal" Required="true"></asp:TextBox>
              <asp:RequiredFieldValidator ID="rfvDueDate" runat="server" ControlToValidate="txtDueDate" 
                                        ErrorMessage="Due date is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
              <asp:CompareValidator ID="cvDueDate" runat="server" ControlToValidate="txtDueDate" 
                                  Operator="GreaterThan" Type="Date" ErrorMessage="Due date must be in the future" 
                                  CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
            </div>
            
            <div class="form-group">
              <asp:Label ID="lblMaxPoints" runat="server" CssClass="form-label" AssociatedControlID="txtMaxPoints">Maximum Points</asp:Label>
              <asp:TextBox ID="txtMaxPoints" runat="server" CssClass="form-control" placeholder="100" TextMode="Number" min="1" max="1000" Required="true"></asp:TextBox>
              <asp:RequiredFieldValidator ID="rfvMaxPoints" runat="server" ControlToValidate="txtMaxPoints" 
                                        ErrorMessage="Maximum points is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
              <asp:RangeValidator ID="rvMaxPoints" runat="server" ControlToValidate="txtMaxPoints" 
                                MinimumValue="1" MaximumValue="1000" Type="Integer" 
                                ErrorMessage="Points must be between 1 and 1000" CssClass="text-danger" Display="Dynamic"></asp:RangeValidator>
            </div>
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <asp:Label ID="lblAssignmentType" runat="server" CssClass="form-label" AssociatedControlID="ddlAssignmentType">Assignment Type</asp:Label>
              <asp:DropDownList ID="ddlAssignmentType" runat="server" CssClass="form-select">
                <asp:ListItem Text="Select Type" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Essay" Value="Essay"></asp:ListItem>
                <asp:ListItem Text="Programming Assignment" Value="Programming"></asp:ListItem>
                <asp:ListItem Text="Research Paper" Value="Research"></asp:ListItem>
                <asp:ListItem Text="Project" Value="Project"></asp:ListItem>
                <asp:ListItem Text="Presentation" Value="Presentation"></asp:ListItem>
                <asp:ListItem Text="Lab Report" Value="Lab Report"></asp:ListItem>
                <asp:ListItem Text="Quiz" Value="Quiz"></asp:ListItem>
                <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
              </asp:DropDownList>
              <asp:RequiredFieldValidator ID="rfvAssignmentType" runat="server" ControlToValidate="ddlAssignmentType" 
                                        ErrorMessage="Please select assignment type" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
              <asp:Label ID="lblSubmissionFormat" runat="server" CssClass="form-label" AssociatedControlID="ddlSubmissionFormat">Submission Format</asp:Label>
              <asp:DropDownList ID="ddlSubmissionFormat" runat="server" CssClass="form-select">
                <asp:ListItem Text="Select Format" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="File Upload" Value="File Upload"></asp:ListItem>
                <asp:ListItem Text="Text Entry" Value="Text Entry"></asp:ListItem>
                <asp:ListItem Text="Both" Value="Both"></asp:ListItem>
              </asp:DropDownList>
              <asp:RequiredFieldValidator ID="rfvSubmissionFormat" runat="server" ControlToValidate="ddlSubmissionFormat" 
                                        ErrorMessage="Please select submission format" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            </div>
          </div>
          
          <div class="form-group">
            <asp:Label ID="lblInstructions" runat="server" CssClass="form-label" AssociatedControlID="txtInstructions">Additional Instructions (Optional)</asp:Label>
            <asp:TextBox ID="txtInstructions" runat="server" CssClass="form-textarea" TextMode="MultiLine" 
                         placeholder="Enter any additional instructions, submission guidelines, grading criteria, etc." Rows="4"></asp:TextBox>
          </div>
          
          <div class="form-actions">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
            <asp:Button ID="btnSaveDraft" runat="server" Text="Save as Draft" CssClass="btn btn-secondary" OnClick="btnSaveDraft_Click" />
            <asp:Button ID="btnCreateAssignment" runat="server" Text="Create Assignment" CssClass="btn" OnClick="btnCreateAssignment_Click" />
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
      document.querySelector('#menu-btn').addEventListener('click', function () {
          document.querySelector('.side-bar').classList.toggle('active');
      });

      // Close sidebar on mobile
      document.querySelector('#close-btn').addEventListener('click', function () {
          document.querySelector('.side-bar').classList.remove('active');
      });

      // Toggle dark/light mode
      document.querySelector('#toggle-btn').addEventListener('click', function () {
          document.body.classList.toggle('dark-mode');
          this.classList.toggle('fa-sun');
          this.classList.toggle('fa-moon');
      });

      // Show/hide user profile
      document.querySelector('#user-btn').addEventListener('click', function () {
          document.querySelector('.header .profile').classList.toggle('active');
      });

      // Show/hide search form on mobile
      document.querySelector('#search-btn').addEventListener('click', function () {
          document.querySelector('.header .search-form').classList.toggle('active');
          this.classList.toggle('fa-times');
      });

      // Set minimum date to today for due date
      document.addEventListener('DOMContentLoaded', function () {
          const dueDateInput = document.getElementById('<%= txtDueDate.ClientID %>');
        if (dueDateInput) {
            const now = new Date();
            now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
            dueDateInput.min = now.toISOString().slice(0, 16);
        }
    });
  </script>
</body>
</html>