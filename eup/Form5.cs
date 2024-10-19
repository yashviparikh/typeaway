using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace eup
{
    public partial class Form5 : Form
    {
        private readonly string connectionString;
        public Form5(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();
            string alias = textBox3.Text.Trim();
            string email = textBox4.Text.Trim();
            string password = textBox5.Text.Trim();

            // Validate form inputs
            if (ValidateForm(firstName, lastName, alias, email, password))
            {
                // Proceed to check credentials in the database
                if (AuthenticateUser(email, password))
                {
                    MessageBox.Show("Sign-In Successful!");

                    // Add code to handle successful login, e.g., navigate to the main form
                    this.Hide();  // Hides the current form (Sign-In Form)

                    using (Form2 form2 = new Form2(connectionString))
                    {
                        form2.ShowDialog();  // Shows Form 2 as a modal dialog
                    }

                    this.Show();
                }
                else
                {
                    MessageBox.Show("Invalid credentials. Please try again.", "Sign-In Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Method to authenticate user from the database and set CurrentUser
        private bool AuthenticateUser(string email, string password)
        {
            string query = "SELECT UserID, FirstName, LastName, Alias, Email FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", password);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())  // If a matching user is found
                        {
                            // Set CurrentUser information
                            CurrentUser.UserID = Convert.ToInt32(reader["UserID"]);
                            CurrentUser.FirstName = reader["FirstName"].ToString();
                            CurrentUser.LastName = reader["LastName"].ToString();
                            CurrentUser.Email = reader["Email"].ToString();

                            return true;  // Authentication successful
                        }
                        else
                        {
                            return false;  // No user found
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private bool ValidateForm(string firstName, string lastName, string alias, string email, string password)
        {
            // Validate First Name
            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("First Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return false;
            }

            // Validate Last Name
            if (string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Last Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                return false;
            }

            // Validate Alias
            if (string.IsNullOrEmpty(alias))
            {
                MessageBox.Show("Alias is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Focus();
                return false;
            }

            // Validate Email
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Focus();
                return false;
            }
            else if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid Email format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox4.Focus();
                return false;
            }

            // Validate Password
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox5.Focus();
                return false;
            }
            else if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox5.Focus();
                return false;
            }

            return true;
        }

        // Function to validate email format using regex
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Function to hash the password
        //private string HashPassword(string password)
        //{
            //using (var sha256 = System.Security.Cryptography.SHA256.Create())
            //{
                //byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
               // return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            //}
        //}
    }
}
