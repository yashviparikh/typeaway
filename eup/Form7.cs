using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eup
{
    public partial class Form7 : Form
    {
        private readonly string connectionString;

        // Declare components as class members
        private System.Windows.Forms.Label labelFirstName;
        private System.Windows.Forms.Label labelLastName;
        private System.Windows.Forms.Label labelAlias;
        private System.Windows.Forms.Label labelPosts;
        private System.Windows.Forms.TextBox textBoxFirstName;
        private System.Windows.Forms.TextBox textBoxLastName;
        private System.Windows.Forms.TextBox textBoxAlias;
        private System.Windows.Forms.ComboBox comboBoxThemes;
        private System.Windows.Forms.PictureBox pictureBoxProfile;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button backButton; // Declare Back button

        public Form7(string connectionString)
        {
            InitializeComponent();
            SetupComponents();
            this.connectionString = connectionString;

            try
            {
                int currentUserID = GetCurrentUserID();
                LoadUserData(currentUserID);  // Fetch and display data for the logged-in user
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);  // Handle case when no user is logged in
            }
        }

        private int GetCurrentUserID()
        {
            // Return the UserID from the CurrentUser class
            if (CurrentUser.UserID > 0)
            {
                return CurrentUser.UserID;
            }
            else
            {
                throw new InvalidOperationException("No user is logged in.");
            }
        }

        private void SetupComponents()
        {
            // Initialize and configure components
            labelFirstName = new System.Windows.Forms.Label();
            labelFirstName.AutoSize = true;
            labelFirstName.Location = new System.Drawing.Point(12, 40);
            labelFirstName.Text = "First Name";
            SetLabelStyle(labelFirstName);

            textBoxFirstName = new System.Windows.Forms.TextBox();
            textBoxFirstName.Location = new System.Drawing.Point(100, 40);
            textBoxFirstName.Size = new System.Drawing.Size(200, 22);

            labelLastName = new System.Windows.Forms.Label();
            labelLastName.AutoSize = true;
            labelLastName.Location = new System.Drawing.Point(12, 80);
            labelLastName.Text = "Last Name";
            SetLabelStyle(labelLastName);

            textBoxLastName = new System.Windows.Forms.TextBox();
            textBoxLastName.Location = new System.Drawing.Point(100, 80);
            textBoxLastName.Size = new System.Drawing.Size(200, 22);

            labelAlias = new System.Windows.Forms.Label();
            labelAlias.AutoSize = true;
            labelAlias.Location = new System.Drawing.Point(12, 120);
            labelAlias.Text = "Alias";
            SetLabelStyle(labelAlias);

            textBoxAlias = new System.Windows.Forms.TextBox();
            textBoxAlias.Location = new System.Drawing.Point(100, 120);
            textBoxAlias.Size = new System.Drawing.Size(200, 22);

            labelPosts = new System.Windows.Forms.Label();
            labelPosts.AutoSize = true;
            labelPosts.Location = new System.Drawing.Point(12, 160);
            labelPosts.Text = "Theme";
            SetLabelStyle(labelPosts);

            comboBoxThemes = new System.Windows.Forms.ComboBox();
            comboBoxThemes.Location = new System.Drawing.Point(100, 160);
            comboBoxThemes.Size = new System.Drawing.Size(200, 24);

            pictureBoxProfile = new System.Windows.Forms.PictureBox();
            pictureBoxProfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pictureBoxProfile.Location = new System.Drawing.Point(320, 40);
            pictureBoxProfile.Size = new System.Drawing.Size(150, 150);
            pictureBoxProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

            // Initialize and configure MenuStrip
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Size = new System.Drawing.Size(500, 28);
            menuStrip1.Dock = DockStyle.Top; // Ensure it docks at the top of the form

            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            fileToolStripMenuItem.Text = "File";

            uploadPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            uploadPictureToolStripMenuItem.Text = "Upload Picture";
            uploadPictureToolStripMenuItem.Click += new EventHandler(uploadPictureToolStripMenuItem_Click);

            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += new EventHandler(exitToolStripMenuItem_Click);

            // Add items to the MenuStrip
            fileToolStripMenuItem.DropDownItems.Add(uploadPictureToolStripMenuItem);
            fileToolStripMenuItem.DropDownItems.Add(exitToolStripMenuItem);
            menuStrip1.Items.Add(fileToolStripMenuItem);

            // Add MenuStrip to the form
            this.Controls.Add(menuStrip1);
            this.MainMenuStrip = menuStrip1;

            // Add other components to the form
            this.Controls.Add(labelFirstName);
            this.Controls.Add(textBoxFirstName);
            this.Controls.Add(labelLastName);
            this.Controls.Add(textBoxLastName);
            this.Controls.Add(labelAlias);
            this.Controls.Add(textBoxAlias);
            this.Controls.Add(labelPosts);
            this.Controls.Add(comboBoxThemes);
            this.Controls.Add(pictureBoxProfile);

            // Add Back button
            backButton = new System.Windows.Forms.Button();
            backButton.Text = "Back";
            backButton.Font = new Font("Trebuchet MS", 12);
            backButton.BackColor = Color.Teal;
            backButton.ForeColor = Color.White;
            backButton.Location = new System.Drawing.Point(180, 180); // Position the button in the bottom-left corner
            backButton.Size = new System.Drawing.Size(100, 40); // Set the size of the button
            backButton.Click += new EventHandler(backButton_Click);
            this.Controls.Add(backButton);

            // Set form properties
            this.ClientSize = new System.Drawing.Size(800, 600); // Set new window size to 800x600
                                                                 // Set window size to 1025x802
            this.Text = "Profile";

            // Populate the theme ComboBox and bind its event
            PopulateThemeComboBox();
            comboBoxThemes.SelectedIndexChanged += new EventHandler(comboBoxThemes_SelectedIndexChanged);
        }

        private void SetLabelStyle(Label label)
        {
            label.Font = new Font("Trebuchet MS", 12);
            label.BackColor = Color.Teal;
            label.ForeColor = Color.White; // Set font color to white for better contrast
        }

        private void PopulateThemeComboBox()
        {
            comboBoxThemes.Items.Clear();
            comboBoxThemes.Items.Add("Light");
            comboBoxThemes.Items.Add("Dark");
            comboBoxThemes.SelectedIndex = 0;  // Default to Light theme
        }

        private void comboBoxThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTheme = comboBoxThemes.SelectedItem.ToString();

            if (selectedTheme == "Dark")
            {
                this.BackColor = Color.Black;  // Change form background for Dark theme
                foreach (Control control in this.Controls)
                {
                    control.ForeColor = Color.White;  // Change text to white for Dark theme
                    if (control is System.Windows.Forms.Button)
                    {
                        control.BackColor = Color.Black;
                    }
                }
            }
            else
            {
                this.BackColor = Color.White;  // Revert to Light theme
                foreach (Control control in this.Controls)
                {
                    control.ForeColor = Color.Black;  // Revert text to black for Light theme
                    if (control is System.Windows.Forms.Button)
                    {
                        control.BackColor = SystemColors.Control;
                    }
                }
            }
        }

        // Method to load user data from the database
        private void LoadUserData(int userId)
        {
            string query = "SELECT FirstName, LastName, Alias FROM Users WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        // Populate the textboxes with data from the logged-in user
                        textBoxFirstName.Text = reader["FirstName"].ToString();
                        textBoxLastName.Text = reader["LastName"].ToString();
                        textBoxAlias.Text = reader["Alias"].ToString();

                        // Load and display the profile picture if it exists
                        string userDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ProfilePictures");
                        string userProfilePicturePath = Path.Combine(userDirectory, $"ProfilePic_{userId}.jpg");
                        if (File.Exists(userProfilePicturePath))
                        {
                            pictureBoxProfile.ImageLocation = userProfilePicturePath; // Load the saved profile picture
                        }
                    }
                    else
                    {
                        MessageBox.Show("User not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching data: " + ex.Message);
                }
            }
        }

        private void uploadPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png",
                Title = "Select a Profile Picture"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Load and display the selected image
                pictureBoxProfile.ImageLocation = openFileDialog.FileName;

                // Define the path where you want to save the image
                string userDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ProfilePictures");
                string userProfilePicturePath = Path.Combine(userDirectory, $"ProfilePic_{GetCurrentUserID()}.jpg");

                // Ensure the directory exists
                Directory.CreateDirectory(userDirectory);

                // Copy the selected image to the user's profile picture directory
                File.Copy(openFileDialog.FileName, userProfilePicturePath, true);

                MessageBox.Show("Profile picture uploaded successfully.");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4(connectionString);  // Link back to Form4
            form4.Show();
            this.Close(); // Close the current form
        }
    }
}