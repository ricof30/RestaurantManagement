using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using connectState;

namespace RestaurantManagement
{
    public partial class AdminAddCategory : UserControl
    {

        private SqlConnection connect;
        public AdminAddCategory()
        {
            connect = DbHelper.GetConnection();
            InitializeComponent();
            loadData();
        }

        public void loadData()
        {
            try
            {
                
                string query = "SELECT * FROM Categories"; 
                SqlDataAdapter adapter = new SqlDataAdapter(query, connect);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);


                dataGridView1.DefaultCellStyle.ForeColor = Color.Black;

                dataGridView1.DataSource = dataTable;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addCategories_addBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Category name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           
            try
            {
               
                connect.Open();
                string insertQuery = "INSERT INTO Categories (Category) VALUES (@cat)";

                using (SqlCommand insertCmd = new SqlCommand(insertQuery, connect))
                {
                    insertCmd.Parameters.AddWithValue("@cat", txtCategory.Text.Trim());

                    insertCmd.ExecuteNonQuery();
                    loadData();

                    MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtCategory.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }



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

        private void addCategories_category_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void AdminAddCategory_Load(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCategory.Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int categoryId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                    connect.Open();
                    string updateQuery = "UPDATE Categories SET Category = @cat, Date = @date WHERE id = @id";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, connect))
                    {
                        updateCmd.Parameters.AddWithValue("@cat", txtCategory.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@date", DateTime.Now);
                        updateCmd.Parameters.AddWithValue("@id", categoryId);

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            loadData();                         
                            MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtCategory.Clear();
                        }
                        else
                        {
                            MessageBox.Show("No category found to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView1.ClearSelection();

                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                selectedRow.Selected = true;

                txtCategory.Text = selectedRow.Cells[1].Value?.ToString();
            }
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int categoryId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                    connect.Open();
                    string deleteQuery = "DELETE FROM Categories WHERE id = @id";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connect))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", categoryId); ;

                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            loadData();                          
                            MessageBox.Show("Category remove successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtCategory.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Removing category failed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to remove.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}