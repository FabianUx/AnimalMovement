﻿using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataModel;

namespace AnimalMovement
{
    internal partial class FileDetailsForm : BaseForm
    {
        private AnimalMovementDataContext Database { get; set; }
        private string CurrentUser { get; set; }
        private int FileId { get; set; }
        private CollarFile File { get; set; }
        private bool IsFileEditor { get; set; }
        internal event EventHandler DatabaseChanged;

        internal FileDetailsForm(int fileId, string user)
        {
            InitializeComponent();
            RestoreWindow();
            FileId = fileId;
            CurrentUser = user;
            LoadDataContext();
        }

        private void LoadDataContext()
        {
 	        Database = new AnimalMovementDataContext();
            File = Database.CollarFiles.FirstOrDefault(f => f.FileId == FileId);
            if (File == null)
            {
                MessageBox.Show("File not found.", "Form Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            IsFileEditor = Database.IsEditor(File.Project, CurrentUser) ?? false;
            FileNameTextBox.Text = File.FileName;
            FileIdTextBox.Text = File.FileId.ToString(CultureInfo.CurrentCulture);
            FormatTextBox.Text = File.LookupCollarFileFormat.Name;
            CollarManufacturerTextBox.Text = File.LookupCollarManufacturer.Name.Trim();
            CollarIdTextBox.Text = File.CollarId;
            UserNameTextBox.Text = File.UserName;
            UploadDateTextBox.Text = File.UploadDate.ToString(CultureInfo.CurrentCulture);
            ProjectTextBox.Text = File.Project1.ProjectName;
            StatusTextBox.Text = File.LookupCollarFileStatus.Name;
            UpdateCollarFixes();
            EnableForm();
            DoneCancelButton.Focus();
        }

        private void UpdateDataSource()
        {
            File.CollarId = CollarIdTextBox.Text.NullifyIfEmpty();
            File.FileName = FileNameTextBox.Text.NullifyIfEmpty();
        }

        private void UpdateCollarFixes()
        {
            var db = new AnimalMovementViewsDataContext();
            var data = from fix in db.AnimalFixesByFiles
                       where fix.FileId == FileId
                       select fix;
            FixInfoDataGridView.DataSource = data;
        }

        private void ShowContentsButton_Click(object sender, EventArgs e)
        {
            var cursor = Cursor.Current;
            Cursor = Cursors.WaitCursor;
            string contents = Encoding.UTF8.GetString(File.Contents.ToArray());
            var form = new FileContentsForm(contents, File.FileName);
            Cursor = cursor;
            form.Show(this);
        }

        private void ChangeStatusbutton_Click(object sender, EventArgs e)
        {
            var cursor = Cursor.Current;
            Cursor = Cursors.WaitCursor;
            //When changing status, the database can only process about 80 records per second
            int defaultTimeout = Database.CommandTimeout;
            Database.CommandTimeout = 1250;  //In seconds, default is 30
            try
            {
                Database.CollarFile_UpdateStatus(File.FileId, File.Status == 'A' ? 'I' : 'A');
            }
            catch (Exception ex)
            {
                string msg = "Unable to change the status.\n" +
                             "Error message:\n" + ex.Message;
                MessageBox.Show(msg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = cursor;
                return;
            }
            Cursor = cursor;
            Database.CommandTimeout = defaultTimeout;
            OnDatabaseChanged();
            LoadDataContext();
        }

        private void EnableForm()
        {
            ChangeStatusbutton.Enabled = IsFileEditor;
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
                try
                {
                    UpdateDataSource();
                    Database.SubmitChanges();
                    OnDatabaseChanged();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to save changes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                EditSaveButton.Text = "Edit";
                DoneCancelButton.Text = "Done";
                SetEditingControls();
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
            }
            else
            {
                Close();
            }
        }


        private void SetEditingControls()
        {
            bool editModeEnabled = EditSaveButton.Text == "Save";
            FileNameTextBox.Enabled = editModeEnabled;
            CollarIdTextBox.Enabled = editModeEnabled && File.Status == 'I';
            ChangeStatusbutton.Enabled = !editModeEnabled && IsFileEditor;
        }


        private void OnDatabaseChanged()
        {
            EventHandler handle = DatabaseChanged;
            if (handle != null)
                handle(this, EventArgs.Empty);
        }

    }
}
