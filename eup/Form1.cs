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

namespace eup
{
    public partial class Form1 : Form
    {
        private readonly string connectionString;


        public Form1(string connectionString)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3(connectionString);
            this.Hide();
            f3.ShowDialog();  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form5 form5 = new Form5(connectionString);  
            form5.Show();
        }
    }
}
