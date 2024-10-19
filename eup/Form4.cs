using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using System.Configuration;


namespace eup
{
    public partial class Form4 : Form
    {
        private readonly string connectionString;

        public Form4(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SavePostToDatabase(string content, out int newPostId)
        {
            string query = "INSERT INTO BlogPosts (Content, Likes, Shares, UserID) OUTPUT INSERTED.[Id] VALUES (@Content, 0, 0, @UserID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Check if a user is logged in
                    if (CurrentUser.UserID > 0)
                    {
                        command.Parameters.AddWithValue("@Content", content);
                        command.Parameters.AddWithValue("@UserID", CurrentUser.UserID); // Use the UserID from CurrentUser

                        // Retrieve the newly inserted post ID
                        newPostId = (int)command.ExecuteScalar();
                    }
                    else
                    {
                        throw new InvalidOperationException("No user is logged in.");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string content = richTextBox1.Text;
            if (!string.IsNullOrWhiteSpace(content))
            {
                // Save to database and retrieve the new post ID
                int newPostId;
                SavePostToDatabase(content, out newPostId);

                // Retrieve the newly inserted post's details (alias and likes)
                string alias;
                int likes;
                GetPostDetails(newPostId, out alias, out likes);

                // Assuming you have an instance of Form2 open
                Form2 form2 = Application.OpenForms.OfType<Form2>().FirstOrDefault();
                if (form2 != null)
                {
                    // Pass the new post details to Form2
                    form2.AddPost(newPostId, content, alias, likes);
                }

                richTextBox1.Clear();
            }
            else
            {
                MessageBox.Show("Please enter some text.");
            }
        }

        private void GetPostDetails(int postId, out string alias, out int likes)
        {
            string query = "SELECT U.Alias, BP.Likes FROM BlogPosts BP " +
                           "INNER JOIN Users U ON BP.UserID = U.UserID WHERE BP.Id = @PostId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PostId", postId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            alias = reader["Alias"].ToString();
                            likes = (int)reader["Likes"];
                        }
                        else
                        {
                            alias = "Unknown";
                            likes = 0;
                        }
                    }
                }
            }
        }

        private int GetCurrentUserID()
        {
            // Simply return the UserID from the CurrentUser class
            if (CurrentUser.UserID > 0)
            {
                return CurrentUser.UserID;
            }
            else
            {
                throw new InvalidOperationException("No user is logged in.");
            }
        }
    }
}