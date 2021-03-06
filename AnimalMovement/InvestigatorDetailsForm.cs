﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using DataModel;

/*
 * I wanted the changes to the projects and collars list to occur in the main datacontext,
 * so that it all edits could be 'canceled' or saved together.
 * unfortunately, the datasources of the list controls query the datacontext, which by
 * design does not return the transient state.  Therefore I cannot see the current state
 * of the lists until I submit changes to the database.
 * 
 * I have therefore reworked the logic on the form.  the edit button enables the text fields
 * and disables the edit controls on the lists.  The edit controls on the lists are only enabled
 * when the form is not in edit mode.
 */

namespace AnimalMovement
{
    internal partial class InvestigatorDetailsForm : BaseForm
    {
        private AnimalMovementDataContext Database { get; set; }
        private string CurrentUser { get; set; }
        private ProjectInvestigator Investigator { get; set; }
        private bool IsInvestigator { get; set; }
        private bool IsEditor { get; set; }
        private bool IsEditMode { get; set; }
        internal event EventHandler DatabaseChanged;

        internal InvestigatorDetailsForm(ProjectInvestigator investigator)
        {
            InitializeComponent();
            RestoreWindow();
            Investigator = investigator;
            CurrentUser = Environment.UserDomainName + @"\" + Environment.UserName;
            SetDefaultPropertiesBeforeFormLoad();
            LoadDataContext();
            SetUpGeneral();
        }

        private void SetDefaultPropertiesBeforeFormLoad()
        {
            ShowEmailFilesCheckBox.Checked = Properties.Settings.Default.InvestigatorFormShowEmailFiles;
            ShowDownloadFilesCheckBox.Checked = Properties.Settings.Default.InvestigatorFormShowDownloadFiles;
            ShowDerivedFilesCheckBox.Checked = Properties.Settings.Default.InvestigatorFormShowDerivedFiles;
        }

        private void LoadDataContext()
        {
            Database = new AnimalMovementDataContext();
            //Database.Log = Console.Out;
            //Investigator is in a different DataContext, get one in this DataContext
            if (Investigator != null)
                Investigator = Database.ProjectInvestigators.First(pi => pi.Login == Investigator.Login);
            if (Investigator == null)
                throw new InvalidOperationException("Investigator Form not provided a valid investigator.");

            var functions = new AnimalMovementFunctions();
            IsInvestigator = Investigator == Database.ProjectInvestigators.FirstOrDefault(pi => pi.Login == CurrentUser);
            IsEditor = functions.IsInvestigatorEditor(Investigator.Login, CurrentUser) ?? false;
            CollarsListBox.DataSource = null;
        }


        #region Form Control

        protected override void OnLoad(EventArgs e)
        {
            ProjectInvestigatorTabs.SelectedIndex = Properties.Settings.Default.InvestigatorFormActiveTab;
            if (ProjectInvestigatorTabs.SelectedIndex == 0)
                //if new index is zero, index changed event will not fire, so fire it manually
                ProjectInvestigatorTabs_SelectedIndexChanged(null,null);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Properties.Settings.Default.InvestigatorFormActiveTab = ProjectInvestigatorTabs.SelectedIndex;
            Properties.Settings.Default.InvestigatorFormShowEmailFiles = ShowEmailFilesCheckBox.Checked;
            Properties.Settings.Default.InvestigatorFormShowDownloadFiles = ShowDownloadFilesCheckBox.Checked;
            Properties.Settings.Default.InvestigatorFormShowDerivedFiles = ShowDerivedFilesCheckBox.Checked;
        }

        private void ProjectInvestigatorTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ProjectInvestigatorTabs.SelectedIndex)
            {
                case 0:
                    SetUpProjectTab();
                    break;
                case 1:
                    SetUpCollarsTab();
                    break;
                case 2:
                    SetUpArgosTab();
                    break;
                case 3:
                    SetUpCollarFilesTab();
                    break;
                case 4:
                    SetUpParameterFilesTab();
                    break;
                case 5:
                    SetUpAssistantsTab();
                    break;
                case 6:
                    SetUpReportsTab();
                    break;
            }
        }

        private bool SubmitChanges()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Database.SubmitChanges();
            }
            catch (Exception ex)  //SqlException && Linq.DuplicateKeyException
            {
                string msg = "Unable to submit changes to the database.\n" +
                             "Error message:\n" + ex.Message;
                MessageBox.Show(msg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            return true;
        }

        private void OnDatabaseChanged()
        {
            EventHandler handle = DatabaseChanged;
            if (handle != null)
                handle(this, EventArgs.Empty);
        }

        #endregion


        #region General

        private void SetUpGeneral()
        {
            LoginTextBox.Text = Investigator.Login;
            NameTextBox.Text = Investigator.Name;
            EmailTextBox.Text = Investigator.Email;
            PhoneTextBox.Text = Investigator.Phone;
            EnableGeneralControls();
        }

        private void EnableGeneralControls()
        {
            EditSaveButton.Enabled = IsEditor;
            IsEditMode = EditSaveButton.Text == "Save";
            NameTextBox.Enabled = IsEditMode;
            EmailTextBox.Enabled = IsEditMode;
            PhoneTextBox.Enabled = IsEditMode;
            //trigger's the active tab to enable/disable it's controls
            ProjectInvestigatorTabs_SelectedIndexChanged(null, null);
        }

        private void EditSaveButton_Click(object sender, EventArgs e)
        {
            //This button is not enabled unless editing is permitted 
            if (EditSaveButton.Text == "Edit")
            {
                // The user wants to edit, Enable form
                EditSaveButton.Text = "Save";
                DoneCancelButton.Text = "Cancel";
                EnableGeneralControls();
            }
            else
            {
                //User is saving
                UpdateDataSource();
                if (SubmitChanges())
                {
                    OnDatabaseChanged();
                    EditSaveButton.Text = "Edit";
                    DoneCancelButton.Text = "Done";
                    EnableGeneralControls();
                }
            }
        }

        private void DoneCancelButton_Click(object sender, EventArgs e)
        {
            if (DoneCancelButton.Text == "Cancel")
            {
                DoneCancelButton.Text = "Done";
                EditSaveButton.Text = "Edit";
                EnableGeneralControls();
                //Reset state from database
                LoadDataContext();
                SetUpGeneral();
            }
            else
            {
                Close();
            }
        }

        private void UpdateDataSource()
        {
            Investigator.Name = NameTextBox.Text;
            Investigator.Email = EmailTextBox.Text;
            Investigator.Phone = PhoneTextBox.Text;
        }

        #endregion


        #region Project List

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // public accessors are used by the control when these classes are accessed through the Datasource
        class ProjectListItem
        {
            public string Name { get; set; }
            public bool CanDelete { get; set; }
            public Project Project { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private void SetUpProjectTab()
        {
            var query = from project in Database.Projects
                        where project.ProjectInvestigator1 == Investigator
                        select new ProjectListItem
                        {
                            Project = project,
                            Name = project.ProjectName + " (" + project.ProjectId + ")",
                            CanDelete = !project.Animals.Any() && !project.CollarFiles.Any()
                        };
            var sortedList = query.OrderBy(p => p.Name).ToList();
            ProjectsListBox.DataSource = sortedList;
            ProjectsListBox.DisplayMember = "Name";
            ProjectsTab.Text = sortedList.Count < 5 ? "Projects" : String.Format("Projects ({0})", sortedList.Count);
        }

        private void EnableProjectControls()
        {
            AddProjectButton.Enabled = !IsEditMode && IsEditor;
            DeleteProjectsButton.Enabled = !IsEditMode && IsEditor &&
                                          ProjectsListBox.SelectedItems.Cast<ProjectListItem>()
                                                        .Any(item => item.CanDelete);
            InfoProjectButton.Enabled = !IsEditMode && ProjectsListBox.SelectedItems.Count == 1;
            ProjectsListBox.Enabled = !IsEditMode;
        }

        private void ProjectDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetUpProjectTab();
        }

        private void AddProjectButton_Click(object sender, EventArgs e)
        {
            var form = new AddProjectForm();
            form.DatabaseChanged += (o, x) => ProjectDataChanged();
            form.Show(this);
        }

        private void DeleteProjectsButton_Click(object sender, EventArgs e)
        {
            foreach (ProjectListItem item in ProjectsListBox.SelectedItems.Cast<ProjectListItem>().Where(item => item.CanDelete))
                Database.Projects.DeleteOnSubmit(item.Project);
            if (SubmitChanges())
                ProjectDataChanged();
        }

        private void InfoProjectButton_Click(object sender, EventArgs e)
        {
            var project = ((ProjectListItem)ProjectsListBox.SelectedItem).Project;
            var form = new ProjectDetailsForm(project);
            form.DatabaseChanged += (o, args) => ProjectDataChanged();
            form.Show(this);
        }

        private void ProjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableProjectControls();
        }

        #endregion


        #region Collar List

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // public accessors are used by the control when these classes are accessed through the Datasource

        class CollarListItem
        {
            public string Name { get; set; }
            public bool CanDelete { get; set; }
            public Collar Collar { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private void SetUpCollarsTab()
        {
            if (CollarsListBox.DataSource != null)
                return;
            var query = from collar in Investigator.Collars
                        select new CollarListItem
                            {
                                Collar = collar,
                                Name = BuildCollarText(collar),
                                CanDelete = CanDeleteCollar(collar)
                            };
            var sortedList = query.OrderBy(c => c.Collar.DisposalDate != null).ThenBy(c => c.Collar.CollarManufacturer).ThenBy(c => c.Collar.CollarId).ToList();
            CollarsListBox.DataSource = sortedList;
            CollarsListBox.DisplayMember = "Name";
            CollarsListBox.ClearItemColors();
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].Collar.DisposalDate != null)
                    CollarsListBox.SetItemColor(i, Color.DarkGray);
            }
            CollarsTab.Text = sortedList.Count < 5 ? "Collars" : String.Format("Collars ({0})", sortedList.Count);
            EnableCollarControls();
        }

        private void EnableCollarControls()
        {
            AddCollarButton.Enabled = !IsEditMode && IsEditor;
            DeleteCollarsButton.Enabled = !IsEditMode && IsEditor &&
                                          CollarsListBox.SelectedItems.Cast<CollarListItem>()
                                                        .Any(item => item.CanDelete);
            InfoCollarButton.Enabled = !IsEditMode && CollarsListBox.SelectedItems.Count == 1;
            CollarsListBox.Enabled = !IsEditMode;
        }

        private static bool CanDeleteCollar(Collar collar)
        {
            return !collar.CollarDeployments.Any() &&
                   !collar.CollarFixes.Any() &&
                   !collar.CollarParameters.Any(p => p.CollarFiles.Any()) &&
                   !collar.ArgosDeployments.Any();
        }

        private string BuildCollarText(Collar collar)
        {
            string name = collar.ToString();
            var animals = from deployment in Database.CollarDeployments
                          where deployment.Collar == collar && deployment.RetrievalDate == null
                          select deployment.Animal;
            var animal = animals.FirstOrDefault();
            if (animal != null)
                name += " on " + animal;
            if (collar.DisposalDate != null)
                name = String.Format("{0} (disp:{1:M/d/yy})", name, collar.DisposalDate.Value.ToLocalTime());
            return name;
        }

        private void CollarDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetUpCollarsTab();
        }

        private void AddCollarButton_Click(object sender, EventArgs e)
        {
            var form = new AddCollarForm(Investigator);
            form.DatabaseChanged += (o, x) => CollarDataChanged();
            form.Show(this);
        }

        private void DeleteCollarsButton_Click(object sender, EventArgs e)
        {
            foreach (CollarListItem item in CollarsListBox.SelectedItems.Cast<CollarListItem>().Where(item => item.CanDelete))
                Database.Collars.DeleteOnSubmit(item.Collar);
            if (SubmitChanges())
                CollarDataChanged();
        }

        private void InfoCollarButton_Click(object sender, EventArgs e)
        {
            var collar = ((CollarListItem)CollarsListBox.SelectedItem).Collar;
            var form = new CollarDetailsForm(collar);
            form.DatabaseChanged += (o, args) => CollarDataChanged();
            form.Show(this);
        }

        private void CollarsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableCollarControls();
        }

        #endregion


        #region Argos Tab

        private void SetUpArgosTab()
        {
            LoadProgramList();
            //The other lists will be populated when a program is selected
            EnableArgosControls();
            if (IsInvestigator)
                EmailCheckBox.Checked = Settings.GetWantsEmail();
            else
                EmailCheckBox.CheckState = CheckState.Indeterminate;
        }

        private void EnableArgosControls()
        {
            AddProgramButton.Enabled = !IsEditMode && IsEditor;
            DeleteProgramButton.Enabled = !IsEditMode && ProgramsListBox.SelectedItems.Cast<ProgramListItem>().Any(i => i.CanDelete);
            InfoProgramButton.Enabled = !IsEditMode && ProgramsListBox.SelectedItems.Count == 1;

            AddPlatformButton.Enabled = !IsEditMode && IsEditor && ProgramsListBox.SelectedItems.Count == 1;
            DeletePlatformButton.Enabled = !IsEditMode && PlatformsListBox.SelectedItems.Cast<PlatformListItem>().Any(i => i.CanDelete);
            InfoPlatformButton.Enabled = !IsEditMode && PlatformsListBox.SelectedItems.Count == 1;

            AddArgosDeploymentButton.Enabled = !IsEditMode && IsEditor && PlatformsListBox.SelectedItems.Count == 1;
            DeleteArgosDeploymentButton.Enabled = !IsEditMode && 
                ArgosDeploymentsGridView.SelectedRows.Cast<DataGridViewRow>()
                                        .Any(r => (bool) r.Cells["CanDelete"].Value);
            EditArgosDeploymentButton.Enabled = !IsEditMode && ArgosDeploymentsGridView.SelectedRows.Count == 1;
            InfoArgosCollarButton.Enabled = !IsEditMode && ArgosDeploymentsGridView.SelectedRows.Count == 1;

            EmailCheckBox.Enabled = !IsEditMode && IsInvestigator;
            ProgramsListBox.Enabled = !IsEditMode;
            PlatformsListBox.Enabled = !IsEditMode;
            ArgosDeploymentsGridView.Enabled = !IsEditMode;
        }

        private void ArgosDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetUpArgosTab();
        }

        private void EmailCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetWantsEmail(EmailCheckBox.Checked);
        }


        #region Argos Programs

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // public accessors are used by the control when these classes are accessed through the Datasource
        class ProgramListItem
        {
            public string Name { get; set; }
            public bool CanDelete { get; set; }
            public ArgosProgram Program { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private void LoadProgramList()
        {
            var query = from program in Investigator.ArgosPrograms
                        select new ProgramListItem
                        {
                            Program = program,
                            Name = GetProgramName(program),
                            CanDelete = !program.ArgosPlatforms.Any()
                        };
            var sortedList = query.OrderBy(p => p.Program.Active.HasValue ? (p.Program.Active.Value ? 0 : 2) : 1).ThenBy(p => p.Name).ToList();
            ProgramsListBox.DataSource = sortedList;
            ProgramsListBox.DisplayMember = "Name";
            ProgramsListBoxLabel.Text = sortedList.Count < 5 ? "Programs" : String.Format("Programs ({0})", sortedList.Count);
            ProgramsListBox.ClearItemColors();
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (!sortedList[i].Program.Active.HasValue)
                    ProgramsListBox.SetItemColor(i, Color.DimGray);
                if (sortedList[i].Program.Active.HasValue && !sortedList[i].Program.Active.Value)
                    ProgramsListBox.SetItemColor(i, Color.DarkGray);
            }
        }

        private static string GetProgramName(ArgosProgram program)
        {
            var active = program.Active.HasValue
                             ? (program.Active.Value ? "Active download" : "Inactive download")
                             : "Download status defered to platforms";
            return program + " (" + active + ")";
        }


        private void AddProgramButton_Click(object sender, EventArgs e)
        {
            var form = new AddArgosProgramForm(Investigator);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void DeleteProgramButton_Click(object sender, EventArgs e)
        {
            foreach (ProgramListItem item in ProgramsListBox.SelectedItems)
                if (item.CanDelete)
                    Database.ArgosPrograms.DeleteOnSubmit(item.Program);
            if (SubmitChanges())
                ArgosDataChanged();
        }

        private void InfoProgramButton_Click(object sender, EventArgs e)
        {
            var program = ((ProgramListItem)ProgramsListBox.SelectedItem).Program;
            var form = new ArgosProgramDetailsForm(program);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void ProgramsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPlatformsListBox(((ProgramListItem)ProgramsListBox.SelectedItem).Program);
            EnableArgosControls();
        }


        #endregion


        #region Argos Platforms

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // public accessors are used by the control when these classes are accessed through the Datasource
        class PlatformListItem
        {
            public string Name { get; set; }
            public bool CanDelete { get; set; }
            public ArgosPlatform Platform { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private void LoadPlatformsListBox(ArgosProgram program)
        {
            var query = from platform in program.ArgosPlatforms
                        select new PlatformListItem
                        {
                            Platform = platform,
                            Name = GetPlatformName(platform),
                            CanDelete = !platform.ArgosDeployments.Any()
                        };
            var sortedList = query.OrderBy(p => p.Platform.Active ? 0 : 1).ThenBy(p => p.Name).ToList();
            PlatformsListBox.DataSource = sortedList;
            PlatformsListBox.DisplayMember = "Name";
            PlatformsListBoxLabel.Text = sortedList.Count < 5 ? "Argos Ids" : String.Format("Argos Ids ({0})", sortedList.Count);
            PlatformsListBox.ClearItemColors();
            for (int i = 0; i < sortedList.Count; i++)
            {
                var programStatus = sortedList[i].Platform.ArgosProgram.Active;
                var platformStatus = sortedList[i].Platform.Active;
                if ((programStatus.HasValue && !programStatus.Value) || (!programStatus.HasValue && !platformStatus))
                    PlatformsListBox.SetItemColor(i, Color.DarkGray);
            }
        }

        private static string GetPlatformName(ArgosPlatform platform)
        {
            var active = platform.ArgosProgram.Active.HasValue
                             ? ""
                             : (platform.Active ? " (Active)" : " (Inactive)");
            return platform.PlatformId + active;
        }

        private void AddPlatformButton_Click(object sender, EventArgs e)
        {
            var program = ((ProgramListItem)ProgramsListBox.SelectedItem).Program;
            var form = new AddArgosPlatformForm(program);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void DeletePlatformButton_Click(object sender, EventArgs e)
        {
            foreach (PlatformListItem item in PlatformsListBox.SelectedItems)
                if (item.CanDelete)
                    Database.ArgosPlatforms.DeleteOnSubmit(item.Platform);
            if (SubmitChanges())
                ArgosDataChanged();
        }

        private void InfoPlatformButton_Click(object sender, EventArgs e)
        {
            var platform = ((PlatformListItem)PlatformsListBox.SelectedItem).Platform;
            var form = new ArgosPlatformDetailsForm(platform);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void PlatformsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadArgosDataGridView(((PlatformListItem)PlatformsListBox.SelectedItem).Platform);
            EnableArgosControls();
        }

        #endregion


        #region Argos Deployments

        private void LoadArgosDataGridView(ArgosPlatform platform)
        {
            ArgosDeploymentsGridView.DataSource =
                platform.ArgosDeployments.Select(d => new
                    {
                        ArgosDeployment = d,
                        d.Collar,
                        ArgosId = d.PlatformId,
                        Start = d.StartDate == null ? "Long ago" : d.StartDate.Value.ToString("g"),
                        End = d.EndDate == null ? "Never" : d.EndDate.Value.ToString("g"),
                        CanDelete = true
                    }).ToList();
            ArgosDeploymentsGridView.Columns[0].Visible = false;
            ArgosDeploymentsGridView.Columns[5].Visible = false;
        }

        private void AddArgosDeploymentButton_Click(object sender, EventArgs e)
        {
            var platform = ((PlatformListItem) PlatformsListBox.SelectedItem).Platform;
            var form = new AddArgosDeploymentForm(null, platform);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void DeleteArgosDeploymentButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in ArgosDeploymentsGridView.SelectedRows)
            {
                var argosDeployment = (ArgosDeployment) row.Cells[0].Value;
                if ((bool)row.Cells["CanDelete"].Value)
                    Database.ArgosDeployments.DeleteOnSubmit(argosDeployment);
            }
            if (SubmitChanges())
                ArgosDataChanged();
        }

        private void EditArgosDeploymentButton_Click(object sender, EventArgs e)
        {
            var deploymentId = ((ArgosDeployment)ArgosDeploymentsGridView.SelectedRows[0].Cells[0].Value).DeploymentId;
            var form = new ArgosDeploymentDetailsForm(deploymentId, false, true);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void InfoArgosCollarButton_Click(object sender, EventArgs e)
        {
            var deployment = (ArgosDeployment)ArgosDeploymentsGridView.SelectedRows[0].Cells[0].Value;
            var form = new CollarDetailsForm(deployment.Collar);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void ArgosDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && InfoArgosCollarButton.Enabled)
                InfoArgosCollarButton_Click(sender, e);
        }

        private void ArgosDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableArgosControls();
        }

        #endregion

        #endregion


        #region Collar File List

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // public accessors are used by the control when these classes are accessed through the Datasource
        class CollarFileListItem
        {
            public string Name { get; set; }
            public bool CanDelete { get; set; }
            public CollarFile File { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private void SetUpCollarFilesTab()
        {
            var query = from file in Database.CollarFiles
                        where file.ProjectInvestigator == Investigator &&
                              (ShowDerivedFilesCheckBox.Checked || file.ParentFileId == null) &&
                              (ShowEmailFilesCheckBox.Checked || file.Format != 'E') &&
                              (ShowDownloadFilesCheckBox.Checked || file.Format != 'F')
                        select new CollarFileListItem
                        {
                            File = file,
                            Name = file.FileName + (file.Status == 'I' ? " (Inactive)" : ""),
                            CanDelete = file.ParentFileId == null && !file.ArgosDownloads.Any()
                        };
            var sortedList = query.OrderBy(f => f.File.Status)
                                  .ThenByDescending(f => f.File.ParentFileId ?? f.File.FileId)
                                  .ThenByDescending(f => f.File.FileId)
                                  .ToList();
            CollarFilesListBox.DataSource = sortedList;
            CollarFilesListBox.DisplayMember = "Name";
            CollarFilesListBox.ClearItemColors();
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].File.ParentFileId != null)
                    CollarFilesListBox.SetItemColor(i, Color.Brown);
                if (sortedList[i].File.Format == 'E')
                    CollarFilesListBox.SetItemColor(i, Color.MediumBlue);
                if (sortedList[i].File.Format == 'F')
                    CollarFilesListBox.SetItemColor(i, Color.DarkMagenta);
                if (sortedList[i].File.Status == 'I')
                {
                    //Dim color of inactive files
                    var c = CollarFilesListBox.GetItemColor(i);
                    CollarFilesListBox.SetItemColor(i, ControlPaint.Light(c, 1.4f));
                }
            }
            CollarFilesTab.Text = sortedList.Count < 5 ? "Collar Files" : String.Format("Collar Files ({0})", sortedList.Count);
            EnableCollarFilesControls();
        }

        private void EnableCollarFilesControls()
        {
            AddCollarFileButton.Enabled = !IsEditMode && IsEditor;
            DeleteCollarFilesButton.Enabled = !IsEditMode && IsEditor &&
                                              CollarFilesListBox.SelectedItems.Cast<CollarFileListItem>()
                                                                .Any(item => item.CanDelete);
            InfoCollarFileButton.Enabled = !IsEditMode && CollarFilesListBox.SelectedItems.Count == 1;
            CollarFilesListBox.Enabled = !IsEditMode;
        }

        private void CollarFileDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetUpCollarFilesTab();
        }

        private void AddCollarFileButton_Click(object sender, EventArgs e)
        {
            var form = new UploadFilesForm(null, Investigator);
            form.DatabaseChanged += (o, x) => CollarFileDataChanged();
            form.Show(this);
        }

        private void DeleteCollarFilesButton_Click(object sender, EventArgs e)
        {
            foreach (CollarFileListItem item in CollarFilesListBox.SelectedItems.Cast<CollarFileListItem>().Where(item => item.CanDelete))
                Database.CollarFiles.DeleteOnSubmit(item.File);
            if (SubmitChanges())
                CollarFileDataChanged();
        }

        private void InfoCollarFileButton_Click(object sender, EventArgs e)
        {
            var file = ((CollarFileListItem)CollarFilesListBox.SelectedItem).File;
            var form = new FileDetailsForm(file);
            form.DatabaseChanged += (o, args) => CollarFileDataChanged();
            form.Show(this);
        }

        private void CollarFilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableCollarFilesControls();
        }

        private void ShowFilesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Visible)
                SetUpCollarFilesTab();
        }

        #endregion


        #region Parameter File List

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // public accessors are used by the control when these classes are accessed through the Datasource
        class ParameterFileListItem
        {
            public string Name { get; set; }
            public bool CanDelete { get; set; }
            public CollarParameterFile File { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private void SetUpParameterFilesTab()
        {
            var query = from file in Database.CollarParameterFiles
                        where file.ProjectInvestigator == Investigator
                        select new ParameterFileListItem
                        {
                            File = file,
                            Name = file.FileName,
                            CanDelete = !file.CollarParameters.Any(p => p.CollarFiles.Any())
                        };
            var sortedList = query.OrderBy(f => f.File.Status).ThenBy(f => f.Name).ToList();
            ParameterFilesListBox.DataSource = sortedList;
            ParameterFilesListBox.DisplayMember = "Name";
            ParameterFilesListBox.ClearItemColors();
            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].File.Status == 'I')
                    ParameterFilesListBox.SetItemColor(i, Color.DarkGray);
            }
            ParameterFilesTab.Text = sortedList.Count < 5 ? "Parameter Files" : String.Format("Parameter Files ({0})", sortedList.Count);
            EnableParameterFilesControls();
        }

        private void EnableParameterFilesControls()
        {
            AddParameterFileButton.Enabled = !IsEditMode && IsEditor;
            DeleteParameterFilesButton.Enabled = !IsEditMode && IsEditor &&
                                              ParameterFilesListBox.SelectedItems.Cast<ParameterFileListItem>()
                                                                .Any(item => item.CanDelete);
            InfoParameterFileButton.Enabled = !IsEditMode && ParameterFilesListBox.SelectedItems.Count == 1;
            ParameterFilesListBox.Enabled = !IsEditMode;
        }

        private void ParameterFileDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetUpParameterFilesTab();
        }

        private void AddParameterFileButton_Click(object sender, EventArgs e)
        {
            var form = new AddCollarParameterFileForm(Investigator);
            form.DatabaseChanged += (o, x) => ParameterFileDataChanged();
            form.Show(this);
        }

        private void DeleteParameterFilesButton_Click(object sender, EventArgs e)
        {
            foreach (ParameterFileListItem item in ParameterFilesListBox.SelectedItems.Cast<ParameterFileListItem>().Where(item => item.CanDelete))
                Database.CollarParameterFiles.DeleteOnSubmit(item.File);
            if (SubmitChanges())
                ParameterFileDataChanged();
        }

        private void InfoParameterFileButton_Click(object sender, EventArgs e)
        {
            var file = ((ParameterFileListItem)ParameterFilesListBox.SelectedItem).File;
            var form = new CollarParameterFileDetailsForm(file);
            form.DatabaseChanged += (o, args) => ParameterFileDataChanged();
            form.Show(this);
        }

        private void ParameterFilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableParameterFilesControls();
        }

        #endregion


        #region Assistants

        private void SetUpAssistantsTab()
        {
            var assistants = Investigator.ProjectInvestigatorAssistants;
            AssistantsListBox.DataSource = assistants;
            AssistantsListBox.DisplayMember = "Assistant";
            EnableAssistantControls();
        }

        private void EnableAssistantControls()
        {
            AddAssistantButton.Enabled = !IsEditMode && IsInvestigator;
            DeleteAssistantButton.Enabled = !IsEditMode && AssistantsListBox.SelectedItems.Count > 0 &&
                                            (IsInvestigator ||
                                             (IsEditor && AssistantsListBox.SelectedItems.Count == 1 &&
                                              String.Equals(((ProjectInvestigatorAssistant)AssistantsListBox.SelectedItem).Assistant.Normalize(),
                                                            CurrentUser.Normalize(), StringComparison.OrdinalIgnoreCase)));
            AssistantsListBox.Enabled = !IsEditMode;
        }

        private void AssistantDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetUpAssistantsTab();
        }

        private void AddAssistantButton_Click(object sender, EventArgs e)
        {
            var form = new AddEditorForm(null, Investigator);
            form.DatabaseChanged += (o, x) => AssistantDataChanged();
            form.Show(this);
        }

        private void DeleteAssistantButton_Click(object sender, EventArgs e)
        {
            foreach (var item in AssistantsListBox.SelectedItems)
                Database.ProjectInvestigatorAssistants.DeleteOnSubmit((ProjectInvestigatorAssistant)item);
            if (SubmitChanges())
                AssistantDataChanged();
        }

        private void AssistantsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableAssistantControls();
        }

        #endregion


        #region QC Reports

        private XDocument _queryDocument;

        private void SetUpReportsTab()
        {
            if (_queryDocument != null)
                return;
            var xmlFilePath = Properties.Settings.Default.InvestigatorReportsXml;
            string error = null;
            try
            {
                _queryDocument = XDocument.Load(xmlFilePath);
            }
            catch (Exception ex)
            {
                error = String.Format("Unable to load '{0}': {1}", xmlFilePath, ex.Message);
                _queryDocument = null;
            }
            if (error != null)
            {
                ReportDescriptionTextBox.Text = error;
                MessageBox.Show(error, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReportComboBox.DataSource = null;
                return;
            }
            var names = new List<string>{"Pick a report"};
            names.AddRange(_queryDocument.Descendants("name").Select(i => i.Value.Trim()));
            ReportComboBox.DataSource = names;
        }

        private void ReportComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var report = _queryDocument.Descendants("report")
                                       .FirstOrDefault(
                                           r => ((string) r.Element("name")).Trim() == (string) ReportComboBox.SelectedItem);
            ReportDescriptionTextBox.Text = report == null ? null : (string) report.Element("description");
            FillDataGrid(report == null ? null : (string)report.Element("query"));
        }

        private void FillDataGrid(string sql)
        {
            if (String.IsNullOrEmpty(sql))
            {
                ReportDataGridView.DataSource = null;
                return;
            }
            var command = new SqlCommand(sql, (SqlConnection)Database.Connection);
            command.Parameters.Add(new SqlParameter("@PI", SqlDbType.NVarChar) { Value = Investigator.Login });
            var dataAdapter = new SqlDataAdapter(command);
            var table = new DataTable();
            try
            {
                dataAdapter.Fill(table);
            }
            catch (Exception ex)
            {
                table.Columns.Add("Error");
                table.Rows.Add(ex.Message);
            }
            ReportDataGridView.DataSource = new BindingSource { DataSource = table };
        }

        #endregion

    }
}
