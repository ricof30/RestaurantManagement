using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RestaurantManagement
{
    public partial class AdminAddCategory : UserControl
    {

        private SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ROBEL MANALON\OneDrive\Documents\RMS.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=False");

        public AdminAddCategory()
        {
            InitializeComponent();
            displayCategoriesData();
        }

        // Display categories data in DataGridView
        public void displayCategoriesData()
        {
            try
            {
                CategoriesData cData = new CategoriesData();
                List<CategoriesData> listData = cData.AllCategoriesData();

                dataGridView1.DataSource = null; // Clear any existing binding
                dataGridView1.DataSource = listData; // Bind new data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addCategories_addBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(addCategories_category.Text))
            {
                MessageBox.Show("Category name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Open the connection if it's not already open
                if (checkConnection())
                {
                    connect.Open();

                    // Check if the category already exists
                    string checkCatQuery = "SELECT * FROM Categories WHERE Category = @cat";

                    using (SqlCommand cmd = new SqlCommand(checkCatQuery, connect))
                    {
                        cmd.Parameters.AddWithValue("@cat", addCategories_category.Text.Trim());

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            MessageBox.Show($"Category '{addCategories_category.Text.Trim()}' already exists.",
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // Insert new category
                            string insertQuery = "INSERT INTO Categories (Category, date) VALUES (@cat, @date)";

                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, connect))
                            {
                                insertCmd.Parameters.AddWithValue("@cat", addCategories_category.Text.Trim());
                                insertCmd.Parameters.AddWithValue("@date", DateTime.Now.Date);

                                insertCmd.ExecuteNonQuery();
                                displayCategoriesData();

                                MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close(); // Ensure the connection is closed
            }
        }

        // Check if the connection is valid and open it if necessary
        public bool checkConnection()
        {
            try
            {
                if (connect.State == ConnectionState.Closed)
                {
                    connect.Open();
                }
                return connect.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}