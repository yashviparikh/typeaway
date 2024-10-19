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

namespace eup
{
    public partial class Form2 : Form
    {
        private readonly string connectionString;

        public Form2(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;

            // Enable auto-scroll for panel2
            panel2.AutoScroll = true;
            LoadBlogPosts();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();  // Hides the current form (Form 2)
            using (Form4 form4 = new Form4(connectionString))
            {
                form4.ShowDialog();  // Shows Form 4 as a modal dialog
            }
            this.Show();  // Shows Form 2 again after Form 4 is closed
        }

        private void LoadBlogPosts()
        {
            // First, load new posts from the database
            LoadNewBlogPostsFromDatabase();

            // Then, load predefined posts for demo purposes
            var predefinedPosts = new List<(int Id, string Content, string Alias, int Likes)>
            {
                (1, "Welcome to our blog! This is the first post.", "Susan", 10),
                (2, "Today we are going to talk about C# basics.", "Maya", 25),
                (3, "Check out this cool new feature in .NET!", "Henry", 15)
            };

            foreach (var post in predefinedPosts)
            {
                AddPredefinedPost(post.Id, post.Content, post.Alias, post.Likes);
            }
        }

        // Method to load new blog posts from the database
        private void LoadNewBlogPostsFromDatabase()
        {
            string query = "SELECT BP.Id, BP.Content, BP.Likes, U.Alias " +
                           "FROM BlogPosts BP " +
                           "INNER JOIN Users U ON BP.UserID = U.UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader["Id"];
                            string content = reader["Content"].ToString();
                            string alias = reader["Alias"].ToString();
                            int likes = (int)reader["Likes"];

                            // Add the post to the panel
                            AddPost(id, content, alias, likes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading posts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Method to add a post to the panel (for both predefined and new posts)
        public void AddPost(int blogPostId, string content, string alias, int likes)
        {
            GroupBox groupBox = new GroupBox
            {
                Text = alias,  // Show the alias at the top of the post
                Size = new Size(300, 150),
                Padding = new Padding(10),
                BackColor = Color.Transparent,
                Font = new Font("Arial", 12, FontStyle.Regular),
            };

            Label label = new Label
            {
                Text = content,
                AutoSize = false,
                Size = new Size(250, 50),
                Location = new Point(10, 20),
                BackColor = Color.Transparent,
                Font = new Font("Arial", 12, FontStyle.Regular),
            };

            Button likeButton = new Button
            {
                Text = "Like (" + likes + ")",
                Size = new Size(80, 30),
                Location = new Point(10, 80),
                BackColor = Color.Transparent,
            };

            // Increment likes when the Like button is clicked
            likeButton.Click += (s, e) =>
            {
                IncrementLikes(blogPostId);
                likes++;  // Simulate the likes increment for the UI
                likeButton.Text = "Like (" + likes + ")";
            };

            Button shareButton = new Button
            {
                Text = "Share",
                Size = new Size(60, 30),
                Location = new Point(100, 80),
                BackColor = Color.Transparent,
            };

            // Optional: Add share logic
            shareButton.Click += (s, e) => { /* Share logic */ };

            // Add controls to the GroupBox
            groupBox.Controls.Add(label);
            groupBox.Controls.Add(likeButton);
            groupBox.Controls.Add(shareButton);

            // Add the GroupBox to the Panel
            panel2.Controls.Add(groupBox);

            // Adjust the location of the GroupBox for the new post
            int postCount = panel2.Controls.Count;
            groupBox.Location = new Point(10, (postCount - 1) * 160);
        }

        // Method to add predefined posts to the panel without fetching from the database
        public void AddPredefinedPost(int blogPostId, string content, string alias, int likes)
        {
            // Reuse AddPost method for predefined posts
            AddPost(blogPostId, content, alias, likes);
        }

        // Method to increment the like count in the database
        private void IncrementLikes(int blogPostId)
        {
            string query = "UPDATE BlogPosts SET Likes = Likes + 1 WHERE Id = @BlogPostId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BlogPostId", blogPostId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery(); // Update the likes count
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating likes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();  // Hides the current form (Form 2)
            using (Form7 form7 = new Form7(connectionString))
            {
                form7.ShowDialog();  // Shows Form 7 as a modal dialog
            }
            this.Show();
        }
    }
}