using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;


namespace eup
{
    public partial class Form3 : Form
    {
        private readonly string connectionString;
        public Form3(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Reset error messages before validation
            string errorMessage = string.Empty;

            // Check if First Name is empty
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                errorMessage += "First name is required.\n";
            }

            // Check if Last Name is empty
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                errorMessage += "Last name is required.\n";
            }

            // Check if Alias is empty
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                errorMessage += "Alias is required.\n";
            }

            // Check if Email is empty or not valid
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                errorMessage += "Email is required.\n";
            }
            else if (!IsValidEmail(textBox4.Text))
            {
                errorMessage += "Invalid email format.\n";
            }

            // Check if Date of Birth is a valid date
            DateTime dob;
            if (!DateTime.TryParse(dateTimePicker1.Text, out dob))
            {
                errorMessage += "Invalid date of birth.\n";
            }
            else if (dob > DateTime.Now)
            {
                errorMessage += "Date of birth cannot be in the future.\n";
            }

            // Check if Password and Confirm Password are entered and match
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                errorMessage += "Password is required.\n";
            }
            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                errorMessage += "Re-entering password is required.\n";
            }
            if (textBox5.Text != textBox6.Text)
            {
                errorMessage += "Passwords do not match.\n";
            }

            // Show errors if any, otherwise proceed
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Form Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Hash the password for security
                string passwordHash = HashPassword(textBox5.Text);

                // Insert data into the database
                InsertData(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, dob, passwordHash);

                //MessageBox.Show("Sign-up successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Proceed with further logic here, such as navigating to the next form
                Form2 form2 = new Form2(connectionString);
                form2.Show();  // Display Form 2
                this.Hide();   // Hide the current form (Sign-up Form)
            }
        }

        // Method to insert data into the database

        private void InsertData(string firstName, string lastName, string alias, string email, DateTime dob, string passwordHash)
        {
            string query = "INSERT INTO Users (FirstName, LastName, Alias, Email, DateOfBirth, PasswordHash) " +
                           "VALUES (@FirstName, @LastName, @Alias, @Email, @DateOfBirth, @PasswordHash); " +
                           "SELECT CAST(scope_identity() AS int);"; // Get the newly inserted user ID

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add SQL parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Alias", alias);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@DateOfBirth", dob);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                        // Execute the query and retrieve the new user's ID
                        int newUserId = (int)command.ExecuteScalar();

                        // Assuming CurrentUser is a static property holding the logged-in user's details
                        CurrentUser.UserID = newUserId;

                        // Show success message once
                        MessageBox.Show("Sign-up successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    // Display error message if something goes wrong
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // Function to validate email format using regular expressions
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Function to hash the password
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
            
  
