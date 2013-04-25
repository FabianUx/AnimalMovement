﻿using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DataModel;

namespace AnimalMovement
{
    internal partial class CollarDetailsForm : BaseForm
    {
        private AnimalMovementDataContext Database { get; set; }
        private AnimalMovementViews DatabaseViews { get; set; }
        private AnimalMovementFunctions DatabaseFunctions { get; set; }
        private string ManufacturerId { get; set; }
        private string CollarId { get; set; }
        private string CurrentUser { get; set; }
        private Collar Collar { get; set; }
        private bool IsEditor { get; set; }
        private bool IsEditMode { get; set; }
        internal event EventHandler DatabaseChanged;

        internal CollarDetailsForm(string mfgrId, string collarId, string user)
        {
            InitializeComponent();
            RestoreWindow();
            ManufacturerId = mfgrId;
            CollarId = collarId;
            CurrentUser = Environment.UserDomainName + @"\" + Environment.UserName;
            LoadDataContext();
        }

        private void LoadDataContext()
        {
            Database = new AnimalMovementDataContext();
            //Database.Log = Console.Out;
            DatabaseFunctions = new AnimalMovementFunctions();
            DatabaseViews = new AnimalMovementViews();
            Collar = Database.Collars.FirstOrDefault(c => c.CollarManufacturer == ManufacturerId && c.CollarId == CollarId);
            if (Collar == null)
            {
                MessageBox.Show("Collar not found.", "Form Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            IsEditor = string.Equals(Collar.Manager.Normalize(), CurrentUser.Normalize(), StringComparison.OrdinalIgnoreCase);
            ManufacturerTextBox.Text = Collar.LookupCollarManufacturer.Name;
            CollarIdTextBox.Text = Collar.CollarId;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CollarTabs.SelectedIndex = Properties.Settings.Default.CollarDetailsFormActiveTab;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Properties.Settings.Default.CollarDetailsFormActiveTab = CollarTabs.SelectedIndex;
        }

        private void CollarTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CollarTabs.SelectedIndex)
            {
                default:
                    SetupGeneralTab();
                    break;
                case 1:
                    SetupAnimalsTab();
                    break;
                case 2:
                    SetupArgosTab();
                    break;
                case 3:
                    SetupParametersTab();
                    break;
                case 4:
                    SetupFilesTab();
                    break;
                case 5:
                    SetupFixesTab();
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
            catch (SqlException ex)
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


        #region General Tab

        private void SetupGeneralTab()
        {
            ManagerComboBox.DataSource = Database.ProjectInvestigators;
            ManagerComboBox.DisplayMember = "Name";
            ManagerComboBox.SelectedItem = Collar.ProjectInvestigator;
            ModelComboBox.DataSource = Database.LookupCollarModels.Where(m => m.CollarManufacturer == Collar.CollarManufacturer);
            ModelComboBox.DisplayMember = "CollarModel";
            ModelComboBox.SelectedItem = Collar.LookupCollarModel;
            HasGpsCheckBox.Checked = Collar.HasGps;
            OwnerTextBox.Text = Collar.Owner;
            SerialNumberTextBox.Text = Collar.SerialNumber;
            FrequencyTextBox.Text = Collar.Frequency.HasValue ? Collar.Frequency.Value.ToString(CultureInfo.InvariantCulture) : null;
            NotesTextBox.Text = Collar.Notes;
            EditSaveButton.Enabled = IsEditor;
            ConfigureDatePicker();
            SetEditingControls();
        }

        private void ConfigureDatePicker()
        {
            if (Collar.DisposalDate == null)
            {
                var now = DateTime.Now;
                DisposalDateTimePicker.Value = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
                DisposalDateTimePicker.Checked = false;
                DisposalDateTimePicker.CustomFormat = " ";
            }
            else
            {
                DisposalDateTimePicker.Checked = true;
                DisposalDateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm";
                DisposalDateTimePicker.Value = Collar.DisposalDate.Value.ToLocalTime();
            }
        }

        private void SetEditingControls()
        {
            IsEditMode = EditSaveButton.Text == "Save";
            ManagerComboBox.Enabled = IsEditMode;
            ModelComboBox.Enabled = IsEditMode;
            HasGpsCheckBox.Enabled = IsEditMode;
            OwnerTextBox.Enabled = IsEditMode;
            SerialNumberTextBox.Enabled = IsEditMode;
            FrequencyTextBox.Enabled = IsEditMode;
            DisposalDateTimePicker.Enabled = IsEditMode;
            NotesTextBox.Enabled = IsEditMode;
        }

        private void EditSaveButton_Click(object sender, EventArgs e)
        {
            //This button is not enabled unless editing is permitted 
            if (EditSaveButton.Text == "Edit")
            {
                // The user wants to edit, Enable form
                EditSaveButton.Text = "Save";
                DoneCancelButton.Text = "Cancel";
                SetEditingControls();
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
                    SetEditingControls();
                }
            }
        }

        private void DoneCancelButton_Click(object sender, EventArgs e)
        {
            if (DoneCancelButton.Text == "Cancel")
            {
                DoneCancelButton.Text = "Done";
                EditSaveButton.Text = "Edit";
                SetEditingControls();
                //Reset state from database
                LoadDataContext();
                SetupGeneralTab();
            }
            else
            {
                Close();
            }
        }

        private void DisposalDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DisposalDateTimePicker.CustomFormat = DisposalDateTimePicker.Checked ? "yyyy-MM-dd HH:mm" : " ";
        }

        private void UpdateDataSource()
        {
            Collar.ProjectInvestigator = (ProjectInvestigator)ManagerComboBox.SelectedItem;
            Collar.LookupCollarModel = (LookupCollarModel)ModelComboBox.SelectedItem;
            Collar.HasGps = HasGpsCheckBox.Checked;
            Collar.Owner = OwnerTextBox.Text;
            Collar.SerialNumber = SerialNumberTextBox.Text;
            Collar.Frequency = FrequencyTextBox.Text.DoubleOrNull() ?? 0;
            Collar.Notes = NotesTextBox.Text;
            if (DisposalDateTimePicker.Checked)
                Collar.DisposalDate = DisposalDateTimePicker.Value.ToUniversalTime();
            else
                Collar.DisposalDate = null;
        }

        #endregion


        #region Animal Deployments Tab

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // public accessors are used by the control when these classes are accessed through the Datasource
        class DeploymentDataItem
        {
            public string Manufacturer { get; set; }
            public string CollarId { get; set; }
            public string Project { get; set; }
            public string AnimalId { get; set; }
            public DateTime? DeploymentDate { get; set; }
            public DateTime? RetrievalDate { get; set; }
            public CollarDeployment Deployment { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        private void SetupAnimalsTab()
        {
            DeploymentDataGridView.DataSource =
                from d in Database.CollarDeployments
                where d.Collar == Collar
                orderby d.DeploymentDate
                select new DeploymentDataItem
                {
                    Manufacturer = d.Collar.LookupCollarManufacturer.Name,
                    CollarId = d.CollarId,
                    Project = d.Animal.Project.ProjectName,
                    AnimalId = d.AnimalId,
                    DeploymentDate = d.DeploymentDate.ToLocalTime(),
                    RetrievalDate = d.RetrievalDate.HasValue ? d.RetrievalDate.Value.ToLocalTime() : (DateTime?)null,
                    Deployment = d,
                };
            EnableAnimalControls();
        }

        private void EnableAnimalControls()
        {
            AddDeploymentButton.Enabled = !IsEditMode && IsEditor;
            DeleteDeploymentButton.Enabled = !IsEditMode && IsEditor && DeploymentDataGridView.SelectedRows.Count > 0;
            EditDeploymentButton.Enabled = !IsEditMode && IsEditor && DeploymentDataGridView.SelectedRows.Count == 1;
            InfoAnimalButton.Enabled = !IsEditMode && DeploymentDataGridView.SelectedRows.Count == 1;
        }

        private void AnimalDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetupAnimalsTab();
        }

        private void AddDeploymentButton_Click(object sender, EventArgs e)
        {
            var form = new AddCollarDeploymentForm(Collar);
            form.DatabaseChanged += (o, x) => AnimalDataChanged();
            form.Show(this);
        }

        private void DeleteDeploymentButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in DeploymentDataGridView.SelectedRows)
            {
                var deployment = (DeploymentDataItem)row.DataBoundItem;
                Database.CollarDeployments.DeleteOnSubmit(deployment.Deployment);
            }
            if (SubmitChanges())
                AnimalDataChanged();
        }

        private void EditDeploymentButton_Click(object sender, EventArgs e)
        {
            if (DeploymentDataGridView.CurrentRow == null)
                return;
            var item = DeploymentDataGridView.CurrentRow.DataBoundItem as DeploymentDataItem;
            if (item == null)
                return;
            var form = new CollarDeploymentDetailsForm(item.Deployment.DeploymentId, true);
            form.DatabaseChanged += (o, x) => AnimalDataChanged();
            form.Show(this);
        }

        private void InfoAnimalButton_Click(object sender, EventArgs e)
        {
            if (DeploymentDataGridView.CurrentRow == null)
                return;
            var item = DeploymentDataGridView.CurrentRow.DataBoundItem as DeploymentDataItem;
            if (item == null)
                return;
            var form = new AnimalDetailsForm(item.Deployment.ProjectId, item.AnimalId, CurrentUser);
            form.DatabaseChanged += (o, x) => AnimalDataChanged();
            form.Show(this);
        }

        private void DeploymentDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableAnimalControls();
        }

        #endregion


        #region Argos Tab

        private void SetupArgosTab()
        {
            ArgosDataGridView.DataSource =
                Collar.ArgosDeployments.Select(a => new
                {
                    Id = a.DeploymentId,
                    Argos_Id = a.PlatformId,
                    Start = a.StartDate == null ? "Long ago" : a.StartDate.Value.ToString("g"),
                    End = a.EndDate == null ? "Never" : a.EndDate.Value.ToString("g")
                }).ToList();
            ArgosDataGridView.Columns[0].Visible = false;
            EnableArgosControls();
        }

        private void EnableArgosControls()
        {
            AddArgosButton.Enabled = !IsEditMode && IsEditor;
            DeleteArgosButton.Enabled = !IsEditMode && IsEditor && ArgosDataGridView.SelectedRows.Count > 0;
            InfoArgosButton.Enabled = !IsEditMode && ArgosDataGridView.SelectedRows.Count == 1;
        }

        private void ArgosDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetupArgosTab();
        }

        private void AddArgosButton_Click(object sender, EventArgs e)
        {
            var form = new AddArgosDeploymentForm(Collar);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void DeleteArgosButton_Click(object sender, EventArgs e)
        {
            if (ArgosDataGridView.SelectedRows.Count < 1 || ArgosDataGridView.Columns.Count < 1)
                return;
            foreach (DataGridViewRow row in ArgosDataGridView.SelectedRows)
            {
                var deploymentId = (int)row.Cells[0].Value;
                var argosDeployment = Collar.ArgosDeployments.First(d => d.DeploymentId == deploymentId);
                Database.ArgosDeployments.DeleteOnSubmit(argosDeployment);
            }
            if (SubmitChanges())
                ArgosDataChanged();
        }

        private void InfoArgosButton_Click(object sender, EventArgs e)
        {
            if (ArgosDataGridView.SelectedRows.Count < 1 || ArgosDataGridView.Columns.Count < 1)
                return;
            var deploymentId = (int)ArgosDataGridView.SelectedRows[0].Cells[0].Value;
            var form = new ArgosDeploymentDetailsForm(deploymentId, true);
            form.DatabaseChanged += (o, x) => ArgosDataChanged();
            form.Show(this);
        }

        private void ArgosDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            InfoArgosButton_Click(sender, e);
        }

        private void ArgosDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableArgosControls();
        }

        #endregion


        #region Parameters Tab

        private void SetupParametersTab()
        {
            switch (Collar.CollarModel)
            {
                case "Gen3":
                    ParametersDataGridView.DataSource =
                        Collar.CollarParameters.Select(
                            p =>
                            new
                                {
                                    Id = p.ParameterId,
                                    Period = p.Gen3Period % 60 == 0 ? p.Gen3Period / 60 + " hrs" : p.Gen3Period + " min",
                                    File = p.CollarParameterFile == null ? null : p.CollarParameterFile.FileName,
                                    Start = p.StartDate == null ? "Long ago" : p.StartDate.Value.ToString("g"),
                                    End = p.EndDate == null ? "Never" : p.EndDate.Value.ToString("g")
                                }).ToList();
                    break;
                case "Gen4":
                    ParametersDataGridView.DataSource =
                        Collar.CollarParameters.Select(p => new {
                            Id = p.ParameterId,
                            File = p.CollarParameterFile == null ? null : p.CollarParameterFile.FileName,
                            Start = p.StartDate == null ? "Long ago" : p.StartDate.Value.ToString("g"),
                            End = p.EndDate == null ? "Never" : p.EndDate.Value.ToString("g")
                        })
                              .ToList();
                    break;
            }
            ParametersDataGridView.Columns[0].Visible = false;
            EnableParametersControls();
        }

        private void EnableParametersControls()
        {
            AddParameterButton.Enabled = !IsEditMode && IsEditor;
            DeleteParameterButton.Enabled = !IsEditMode && IsEditor && ParametersDataGridView.SelectedRows.Count > 0;
            InfoParameterButton.Enabled = !IsEditMode && ParametersDataGridView.SelectedRows.Count == 1;
        }

        private void ParametersDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetupParametersTab();
        }

        private void AddParameterButton_Click(object sender, EventArgs e)
        {
            var form = new AddCollarParametersForm(Collar);
            form.DatabaseChanged += (o, x) => ParametersDataChanged();
            form.Show(this);
        }

        private void DeleteParameterButton_Click(object sender, EventArgs e)
        {
            if (ParametersDataGridView.SelectedRows.Count < 1 || ParametersDataGridView.Columns.Count < 1)
                return;
            foreach (DataGridViewRow row in ParametersDataGridView.SelectedRows)
            {
                var parameterId = (int) row.Cells[0].Value;
                var collarParameter = Collar.CollarParameters.First(p => p.ParameterId == parameterId);
                Database.CollarParameters.DeleteOnSubmit(collarParameter);
            }
            if (SubmitChanges())
                ParametersDataChanged();
        }

        private void InfoParameterButton_Click(object sender, EventArgs e)
        {
            if (ParametersDataGridView.SelectedRows.Count < 1 || ParametersDataGridView.Columns.Count < 1)
                return;
            var parameterId = (int)ParametersDataGridView.SelectedRows[0].Cells[0].Value;
            var form = new CollarParametersDetailsForm(parameterId, true);
            form.DatabaseChanged += (o, x) => ParametersDataChanged();
            form.Show(this);
        }

        private void ParametersDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            InfoParameterButton_Click(sender, e);
        }

        private void ParametersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableParametersControls();
        }

        #endregion


        #region Files Tab

        private void SetupFilesTab()
        {
            if (Collar == null)
                return;
            FilesDataGridView.DataSource = DatabaseViews.CollarFixesByFile(Collar.CollarManufacturer, Collar.CollarId);
            EnableFileButtons();
        }

        private void EnableFileButtons()
        {
            FileInfoButton.Enabled = FilesDataGridView.CurrentRow != null && !IsEditMode && FilesDataGridView.SelectedRows.Count == 1;
            ChangeFileStatusButton.Enabled = FilesDataGridView.CurrentRow != null && !IsEditMode && IsEditor;
            if (FilesDataGridView.SelectedRows.Count > 1)
            {
                var firstRowStatus = (string)FilesDataGridView.SelectedRows[0].Cells["Status"].Value;
                ChangeFileStatusButton.Text = firstRowStatus != "Active" ? "Activate" : "Deactivate";
                if (FilesDataGridView.SelectedRows.Cast<DataGridViewRow>()
                                     .Any(row => (string)row.Cells["Status"].Value != firstRowStatus))
                    ChangeFileStatusButton.Enabled = false;
            }
        }

        private void FileInfoButton_Click(object sender, EventArgs e)
        {
            if (FilesDataGridView.CurrentRow == null)
                return;
            var item = FilesDataGridView.CurrentRow.DataBoundItem as CollarFixesByFileResult;
            if (item == null)
                return;
            var form = new FileDetailsForm(item.FileId, CurrentUser);
            form.DatabaseChanged += (o, x) => FileDataChanged();
            form.Show(this);
        }

        private void ChangeFileStatusButton_Click(object sender, EventArgs e)
        {
            //If the button is labeled/enabled correctly, then the user wants to change the status of all the selected files
            var newStatus = Database.LookupFileStatus.First(s => s.Code == 'A');
            if (ChangeFileStatusButton.Text == "Deactivate")
                newStatus = Database.LookupFileStatus.First(s => s.Code == 'I');

            var fileIds = FilesDataGridView.SelectedRows.Cast<DataGridViewRow>()
                                           .Select(row => (int)row.Cells["FileId"].Value).ToList();
            var files = Database.CollarFiles.Where(f => fileIds.Contains(f.FileId));
            foreach (var collarFile in files)
                collarFile.LookupFileStatus = newStatus;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                Database.SubmitChanges();
            }
            catch (Exception ex)
            {
                string msg = "Unable to change the status.\n" +
                             "Error message:\n" + ex.Message;
                MessageBox.Show(msg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
                return;
            }
            Cursor.Current = Cursors.Default;
            FileDataChanged();
        }

        private void FileDataChanged()
        {
            OnDatabaseChanged();
            LoadDataContext();
            SetupFilesTab();
        }

        private void FilesDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableFileButtons();
        }

        #endregion


        #region Fixes Tab

        private void SetupFixesTab()
        {
            if (Collar == null)
                return;
            FixConflictsDataGridView.DataSource = DatabaseViews.ConflictingFixes(Collar.CollarManufacturer, Collar.CollarId, 36500); //last 100 years
            var summary = DatabaseViews.CollarFixSummary(Collar.CollarManufacturer, Collar.CollarId).FirstOrDefault();
            SummaryLabel.Text = summary == null
                              ? "There are NO fixes."
                              : (summary.Count == summary.Unique
                                 ? String.Format("{0} fixes between {1:yyyy-MM-dd} and {2:yyyy-MM-dd}.", summary.Count, summary.First, summary.Last)
                                 : String.Format("{3}/{0} unique/total fixes between {1:yyyy-MM-dd} and {2:yyyy-MM-dd}.", summary.Count, summary.First, summary.Last, summary.Unique));
            FixConflictsDataGridView_SelectionChanged(null, null);
        }

        private void FixConflictsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (FixConflictsDataGridView.CurrentRow == null)
            {
                UnhideFixButton.Enabled = false;
                return;
            }
            var selectedFix = FixConflictsDataGridView.CurrentRow.DataBoundItem as ConflictingFixesResult;
            if (selectedFix == null)
            {
                UnhideFixButton.Enabled = false;
                return;
            }
            bool isEditor = DatabaseFunctions.IsFixEditor(selectedFix.FixId, CurrentUser) ?? false;
            bool isHidden = selectedFix.HiddenBy != null;
            UnhideFixButton.Enabled = isEditor && isHidden;
        }

        private void UnhideFixButton_Click(object sender, EventArgs e)
        {
            if (FixConflictsDataGridView.CurrentRow == null)
                return;
            var selectedFix = FixConflictsDataGridView.CurrentRow.DataBoundItem as ConflictingFixesResult;
            if (selectedFix == null)
                return;
            try
            {
                Database.CollarFixes_UpdateUnhideFix(selectedFix.FixId);
            }
            catch (Exception ex)
            {
                string msg = "Unable to update the fix.\n" +
                             "Error message:\n" + ex.Message;
                MessageBox.Show(msg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OnDatabaseChanged();
            LoadDataContext();
            SetupFixesTab();
        }


        #endregion

    }
}
