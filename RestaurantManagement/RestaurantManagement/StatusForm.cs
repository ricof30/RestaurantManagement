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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace RestaurantManagement
{
    public partial class StatusForm : Form
    {
        private SqlConnection connect;
        private string username;
        public StatusForm(string loggedInUsername)
        {
            username = loggedInUsername;
            connect = DbHelper.GetConnection();
            InitializeComponent();
        }

        private void StatusForm_Load(object sender, EventArgs e)
        {
            LoadAppetizerData();
        }

        private void LoadAppetizerData()
        {
           
            try
            {
                int userId = GetUserId(connect, username);

                string query = "SELECT * FROM Orders WHERE UserId = @UserId";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        // Bind the data to the DataGridView
                        appetizerGridView.DataSource = table;
                    }
                }


                appetizerGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                appetizerGridView.RowHeadersVisible = true;
                appetizerGridView.ReadOnly = false;            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }
        public int GetUserId(SqlConnection conn, string username)
        {
            string query = @"SELECT id FROM Users WHERE username = @username";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                connect.Open();
                cmd.Parameters.AddWithValue("@username", username);


                object result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int userId))
                {
                    return userId;
                }
                else
                {
                    throw new Exception("User not found.");
                }
            }
        }

        private void appetizerGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
