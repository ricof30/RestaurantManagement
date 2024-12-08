using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace RestaurantManagement
{
    public partial class adminReport : Form
    {
        private SqlConnection connect; 
        public adminReport()
        {
            InitializeComponent();
            connect = DbHelper.GetConnection();
        }

        public void loadData()
        {
            try
            {
                string query = "SELECT * FROM Report";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connect);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);


                dataGridView1.DefaultCellStyle.ForeColor = Color.Black;

                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying Food: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Handle the click event on the DataGridView
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            loadData();
        }

        private void adminReport_Load_1(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            loadData();
        }
    }
}
