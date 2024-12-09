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
        private SqlConnection connect;
        public AdminDashboard()
        {
            connect = DbHelper.GetConnection();
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
            LoadTodaysCustomers();
            try
            {
                connect.Open();

                string countQuery = "SELECT COUNT(*) FROM Users";               
                using (SqlCommand countCmd = new SqlCommand(countQuery, connect))
                {
                    int userCount = Convert.ToInt32(countCmd.ExecuteScalar());
                    label10.Text = userCount.ToString();                }

                string customerQuery = @"
                SELECT COUNT(DISTINCT UserId) FROM Orders";
                using (SqlCommand customerCmd = new SqlCommand(customerQuery, connect))
                {
                    int customerCount = Convert.ToInt32(customerCmd.ExecuteScalar());
                    lblAllCustomer.Text = customerCount.ToString();                }

                string todayIncomeQuery = @"
                SELECT SUM(o.Quantity * f.price) 
                FROM Orders o
                INNER JOIN Food f ON o.FoodId = f.Id
                WHERE CONVERT(DATE, o.OrderDate) = CONVERT(DATE, GETDATE())";
                using (SqlCommand todayIncomeCmd = new SqlCommand(todayIncomeQuery, connect))
                {
                    decimal todayIncome = Convert.ToDecimal(todayIncomeCmd.ExecuteScalar());
                    lblTodayIncome.Text = "₱" + todayIncome.ToString("N2");                }

                string totalIncomeQuery = @"
                SELECT SUM(o.Quantity * f.price) 
                FROM Orders o
                INNER JOIN Food f ON o.FoodId = f.Id";
                using (SqlCommand totalIncomeCmd = new SqlCommand(totalIncomeQuery, connect))
                {
                    decimal totalIncome = Convert.ToDecimal(totalIncomeCmd.ExecuteScalar());
                    lblTotalIncome.Text = "₱" + totalIncome.ToString("N2");                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }


        }
        public void LoadTodaysCustomers()
        {
            try
            {
                string query = @"
                SELECT DISTINCT u.Id AS UserId, u.username AS UserName
                FROM Orders o
                INNER JOIN Users u ON o.UserId = u.Id
                WHERE CONVERT(DATE, o.OrderDate) = CONVERT(DATE, GETDATE())";

                connect.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connect);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.DataSource = dataTable;


                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying today's customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }


        private void Dashboard_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadTodaysCustomers();
        }
    }
}
