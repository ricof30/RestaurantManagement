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
    public partial class adminOrderForm : Form
    {
        private SqlConnection connect;
        public adminOrderForm()
        {
            connect = DbHelper.GetConnection();
            InitializeComponent();
        }
        public void loadData()
        {
            try
            {
                string query = "SELECT * FROM Order";
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            loadData();
        }

        private void adminOrderForm_Load(object sender, EventArgs e)
        {
            loadData();
        }
    }
}
