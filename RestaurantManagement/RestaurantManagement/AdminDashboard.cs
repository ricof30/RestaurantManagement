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
using connectState;

namespace RestaurantManagement
{
    public partial class AdminDashboard : UserControl
    {
        private SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\RestaurantManagement\RestaurantManagement\RestaurantManagement\RestaurantManagement\Database.mdf;Integrated Security=True");
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {
         
        }

        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                connect.Open();

                // SQL query to count all users in the 'Users' table
                string countQuery = "SELECT COUNT(*) FROM Users"; // Replace 'Users' with your actual table name

                using (SqlCommand countCmd = new SqlCommand(countQuery, connect))
                {
                    // Execute the query and retrieve the count
                    int userCount = Convert.ToInt32(countCmd.ExecuteScalar());

                    label10.Text = userCount.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error counting users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }
    }
}
