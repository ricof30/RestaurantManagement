using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using connectState;

namespace RestaurantManagement
{
    public partial class adminFoodForm : Form
    {
        private SqlConnection connect;
        public adminFoodForm()
        {
            connect = DbHelper.GetConnection();
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

     

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtDescrip_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblname_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LoadCategories()
        {
            try
            {
                string query = "SELECT Id, category FROM Categories";
                DataTable dt = new DataTable();

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connect))
                {
                    adapter.Fill(dt);                }

                cbCategory.DataSource = dt;              
                cbCategory.DisplayMember = "category";           
                cbCategory.ValueMember = "Id";             
                cbCategory.SelectedIndex = -1;            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void loadData()
        {
            try
            {
                string query = @"
                        SELECT 
                            f.Id, 
                            f.foodname, 
                            c.category AS CategoryName, 
                            f.price, 
                            f.image
                        FROM 
                            Food f
                        INNER JOIN 
                            Categories c ON f.category_id = c.Id";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connect);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("Id", "ID");
                dataGridView1.Columns.Add("foodname", "Food Name");
                dataGridView1.Columns.Add("CategoryName", "Category");
                dataGridView1.Columns.Add("price", "Price");
              

                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn
                {
                    Name = "Image",
                    HeaderText = "Image",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                };
                dataGridView1.Columns.Add(imageColumn);

                foreach (DataRow row in dataTable.Rows)
                {
                    string imagePath = row["image"].ToString();
                    Image image = null;

                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageBytes = File.ReadAllBytes(imagePath);
                        image = Image.FromStream(new MemoryStream(imageBytes));
                    }

                    dataGridView1.Rows.Add(
                        row["Id"].ToString(),
                        row["foodname"].ToString(),
                        row["CategoryName"].ToString(),                      
                        row["price"].ToString(),
                        image
                    );
                }

                dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying Food: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connect.State == ConnectionState.Open)
                {
                    connect.Close();
                }
            }
        }


        private void SetUpDataGridView()
        {
            DataGridViewTextBoxColumn inputColumn = new DataGridViewTextBoxColumn();
            inputColumn.Name = "InputColumn";
            inputColumn.HeaderText = "Notes";
            dataGridView1.Columns.Add(inputColumn);

            DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();
            editButtonColumn.Name = "EditButton";
            editButtonColumn.HeaderText = "Edit";
            editButtonColumn.Text = "Edit";
            editButtonColumn.UseColumnTextForButtonValue = true;          
            dataGridView1.Columns.Add(editButtonColumn);

            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.Name = "DeleteButton";
            deleteButtonColumn.HeaderText = "Delete";
            deleteButtonColumn.Text = "Delete";
            deleteButtonColumn.UseColumnTextForButtonValue = true;          
            dataGridView1.Columns.Add(deleteButtonColumn);
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void adminFoodForm_Load(object sender, EventArgs e)
        {
            lblname.ForeColor = Color.Black;
            btnUploadImage.ForeColor = Color.Black;
            lblcategory.ForeColor = Color.Black;
            lblPrice.ForeColor = Color.Black;
            label1.ForeColor = Color.Black;
            label5.ForeColor = Color.Black;

            LoadCategories();
            loadData();


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFoodName.Text) ||
                cbCategory.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(selectedImagePath))
            {
                MessageBox.Show("Please fill all fields and upload an image.", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                connect.Open();
                string insertQuery = "INSERT INTO Food (foodname, image, category_id, price) VALUES (@foodname, @image, @category_id, @price)";
                int categoryId = Convert.ToInt32(cbCategory.SelectedValue);

                using (SqlCommand insertCmd = new SqlCommand(insertQuery, connect))
                {
                    insertCmd.Parameters.AddWithValue("@foodname", txtFoodName.Text.Trim());
                    insertCmd.Parameters.AddWithValue("@image", selectedImagePath);
                    insertCmd.Parameters.AddWithValue("@category_id", categoryId);
                    insertCmd.Parameters.AddWithValue("@price", txtPrice.Text.Trim());

                    insertCmd.ExecuteNonQuery();
                    loadData();

                    MessageBox.Show("Food added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtFoodName.Clear();
                    cbCategory.SelectedIndex = -1;
                    txtPrice.Clear();
                    pictureBox.Image = null;
                    selectedImagePath = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding food: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }


        private string selectedImagePath = string.Empty;

        private void btnUploadImage_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Select an Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = openFileDialog.FileName;

                    pictureBox.Image = Image.FromFile(selectedImagePath);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int foodId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                    connect.Open();

                    bool isImageUpdated = pictureBox.Image != null && !string.IsNullOrEmpty(selectedImagePath);

                    string updateQuery = "UPDATE Food SET foodname = @foodname, category_id = @category_id, price = @price" +
                                         (isImageUpdated ? ", image = @image" : "") +
                                         " WHERE id = @id";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, connect))
                    {
                        updateCmd.Parameters.AddWithValue("@foodname", txtFoodName.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@category_id", cbCategory.SelectedValue);
                        updateCmd.Parameters.AddWithValue("@price", txtPrice.Text.Trim());
                        updateCmd.Parameters.AddWithValue("@id", foodId);

                        if (isImageUpdated)
                        {
                            updateCmd.Parameters.AddWithValue("@image", selectedImagePath);
                        }

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            loadData();
                            MessageBox.Show("Food updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtFoodName.Clear();
                            cbCategory.SelectedIndex = -1;
                            txtPrice.Clear();
                            pictureBox.Image = null;
                            selectedImagePath = string.Empty;
                        }
                        else
                        {
                            MessageBox.Show("No food found to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show($"Error updating food: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int categoryId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                    connect.Open();
                    string deleteQuery = "DELETE FROM Food WHERE id = @id";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connect))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", categoryId); ;

                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            loadData();
                            MessageBox.Show("Food delete successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtFoodName.Clear();
                            pictureBox.Image = null;
                            cbCategory.SelectedIndex = -1;
                            txtPrice.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Removing food failed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show($"Error removing food: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                txtFoodName.Text = selectedRow.Cells[1].Value?.ToString();
                cbCategory.Text = selectedRow.Cells[2].Value?.ToString();
                txtPrice.Text = selectedRow.Cells[3].Value?.ToString();

                if (selectedRow.Cells[4].Value is Bitmap bitmapImage)
                {
                    pictureBox.Image = bitmapImage;
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    pictureBox.Image = null;
                }
            }
        }

        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFoodName.Clear();
            pictureBox.Image = null;
            cbCategory.SelectedIndex = -1;
            txtPrice.Clear();
        }
    }
}
