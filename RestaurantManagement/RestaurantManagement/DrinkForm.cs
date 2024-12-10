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
    public partial class DrinkForm : Form
    {
        private SqlConnection connect;
        private string username;

        public DrinkForm(string loggedInUsername)
        {
            username = loggedInUsername;
            connect = DbHelper.GetConnection();
            InitializeComponent();
        }

        private void DrinkForm_Load(object sender, EventArgs e)
        {
            if (connect.State == ConnectionState.Open)
            {
                connect.Close();
            }
            else { 
            connect.Open(); 

            }
            LoadMenu();
            LoadAppetizerData();
            getTotalItem();
            getTotalPrice();
            lblUsername.Text = username;
        }

        private void LoadAppetizerData()
        {
            connect.Close();
            int userId = GetUserId(connect, username);
            try
            {
                string query = @"
                SELECT 
                    Orders.OrderId,
                    Orders.FoodId,
                    Food.foodname,
                    Food.price,
                    Categories.category,
                    Orders.Quantity,
                    Orders.TotalAmount
                FROM Orders
                INNER JOIN Food ON Orders.FoodId = Food.Id
                INNER JOIN Categories ON Food.category_id = Categories.Id
                WHERE Orders.UserId = @UserId
                AND Categories.category = 'Drinks'";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    // Add parameter
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        // Bind the data to the DataGridView
                        appetizerGridView.DataSource = table;
                    }
                }

                if (appetizerGridView.Columns["FoodId"] != null)
                {
                    appetizerGridView.Columns["FoodId"].Visible = false;
                }
                if (appetizerGridView.Columns["OrderId"] != null)
                {
                    appetizerGridView.Columns["OrderId"].Visible = false;
                }

                appetizerGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                appetizerGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                appetizerGridView.RowHeadersVisible = false;               
                appetizerGridView.ReadOnly = true;             
                appetizerGridView.AllowUserToResizeRows = false;
                foreach (DataGridViewColumn column in appetizerGridView.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;                }

                appetizerGridView.Columns[appetizerGridView.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appetizer data: " + ex.Message);
            }
        }



        private void LoadMenu()
        {
            flowLayoutPanel1.Controls.Clear();

            flowLayoutPanel1.AutoScroll = true;

            string query = @"
                SELECT 
                    Food.Id, 
                    Food.foodname, 
                    Food.image, 
                    Food.price, 
                    Categories.category 
                FROM Food
                INNER JOIN Categories ON Food.category_id = Categories.Id
                WHERE Categories.category = 'Drinks'";

            DataTable menuData = DbHelper.ExecuteQuery(query);

            foreach (DataRow row in menuData.Rows)
            {
                Panel panel = new Panel
                {
                    Size = new Size(150, 200),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(10)                };

                PictureBox pictureBox = new PictureBox
                {
                    Size = new Size(130, 100),
                    Location = new Point(10, 10),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                if (row["image"] != DBNull.Value)
                {
                    pictureBox.Image = Image.FromFile(row["image"].ToString());
                }
                panel.Controls.Add(pictureBox);

                Label lblName = new Label
                {
                    Text = row["foodname"].ToString(),
                    Location = new Point(10, 120),
                    AutoSize = true
                };
                panel.Controls.Add(lblName);

                Label lblPrice = new Label
                {
                    Text = "Php " + row["price"].ToString(),
                    Location = new Point(10, 140),
                    AutoSize = true
                };
                panel.Controls.Add(lblPrice);

                Button btnAdd = new Button
                {
                    Text = "Add",
                    Size = new Size(130, 30),
                    Location = new Point(10, 160)
                };
                btnAdd.Click += (sender, e) => AddToOrder(row["Id"].ToString(), row["foodname"].ToString());
                panel.Controls.Add(btnAdd);

                flowLayoutPanel1.Controls.Add(panel);
            }
        }
        public int GetUserId(SqlConnection con, string username)
        {
            string query = @"SELECT id FROM Users WHERE username = @username";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
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

        private void AddToOrder(string foodId, string foodName)
        {
            connect.Close();
            try
            {
                int userId = GetUserId(connect, username);
                int quantityToAdd = 1;              
                decimal foodPrice = GetFoodPrice(foodId);

                using (SqlConnection conn = DbHelper.GetConnection())   
                {
                    conn.Open();
                    string checkQuery = @"SELECT Quantity, TotalAmount 
                                  FROM Orders 
                                  WHERE UserId = @UserId AND FoodId = @FoodId";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        checkCmd.Parameters.AddWithValue("@FoodId", foodId);

                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int existingQuantity = reader.GetInt32(0);
                                decimal existingTotal = reader.GetDecimal(1);

                                int newQuantity = existingQuantity + quantityToAdd;
                                decimal newTotal = newQuantity * foodPrice;

                                reader.Close();                         
                                UpdateOrder(conn, userId, foodId, newQuantity, newTotal, foodName);
                            }
                            else
                            {
                                reader.Close();                         
                                decimal totalAmount = quantityToAdd * foodPrice;
                                InsertOrder(conn, userId, foodId, quantityToAdd, totalAmount, foodName);
                            }
                        }
                    }


                    LoadAppetizerData();
                    getTotalItem();
                    getTotalPrice();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item to order: " + ex.Message);
            }
        }




        private void InsertOrder(SqlConnection conn, int userId, string foodId, int quantity, decimal totalAmount, string foodName)
        {
            string insertQuery = @"INSERT INTO Orders (OrderDate, UserId, FoodId, Quantity, TotalAmount) 
                           VALUES (@OrderDate, @UserId, @FoodId, @Quantity, @TotalAmount)";

            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FoodId", foodId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show($"Item {foodName} added to order!");
                }
                else
                {
                    MessageBox.Show("Failed to add item to order.");
                }
            }
        }

        private void UpdateOrder(SqlConnection conn, int userId, string foodId, int newQuantity, decimal newTotal, string foodName)
        {
            string updateQuery = @"UPDATE Orders 
                           SET Quantity = @Quantity, TotalAmount = @TotalAmount 
                           WHERE UserId = @UserId AND FoodId = @FoodId";

            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Quantity", newQuantity);
                cmd.Parameters.AddWithValue("@TotalAmount", newTotal);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@FoodId", foodId);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show($"Order updated: {newQuantity} of item {foodName}.");
                }
                else
                {
                    MessageBox.Show("Failed to update order.");
                }
            }
        }




        private decimal GetFoodPrice(string foodId)
        {
            try
            {
                string query = "SELECT price FROM Food WHERE Id = @FoodId";

                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FoodId", foodId);

                        object result = cmd.ExecuteScalar();
                        if (result != null && decimal.TryParse(result.ToString(), out decimal price))
                        {
                            return price;
                        }
                        else
                        {
                            throw new Exception("Price not found for the selected food item.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching food price: " + ex.Message);
                return 0;
            }
        }

        //private void getTotalItem()
        //{
        //    try
        //    {
        //        string query = @"
        //SELECT SUM(Orders.Quantity) 
        //FROM Orders
        //INNER JOIN Food ON Orders.FoodId = Food.Id
        //INNER JOIN Categories ON Food.category_id = Categories.Id
        //WHERE Categories.category = 'Drinks'";

        //        using (SqlConnection conn = DbHelper.GetConnection())
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(query, conn))
        //            {
        //                object result = cmd.ExecuteScalar();

        //                if (result != DBNull.Value && result != null)
        //                {
        //                    int totalItems = Convert.ToInt32(result);
        //                    total.Text = $"{totalItems}";                        }
        //                else
        //                {
        //                    total.Text = "0";                        }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error calculating total items for appetizers: " + ex.Message);
        //    }
        //}
        private void getTotalItem()
        {
            connect.Close();
            try
            {
                int userId = GetUserId(connect, username);
                string query = @"
        SELECT SUM(Orders.Quantity) 
        FROM Orders
        INNER JOIN Food ON Orders.FoodId = Food.Id
        INNER JOIN Categories ON Food.category_id = Categories.Id
        WHERE Categories.category = 'Drinks' AND Orders.UserId = @UserId"; // Corrected query syntax

                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId); // Added UserId parameter

                        object result = cmd.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            int totalItems = Convert.ToInt32(result);
                            total.Text = $"{totalItems}";
                            conn.Close();
                        }
                        else
                        {
                            total.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating total items for appetizers: " + ex.Message);
            }
        }


        //private void getTotalPrice()
        //{
        //    try
        //    {
        //        string query = @"
        //SELECT SUM(Orders.TotalAmount) 
        //FROM Orders
        //INNER JOIN Food ON Orders.FoodId = Food.Id
        //INNER JOIN Categories ON Food.category_id = Categories.Id
        //WHERE Categories.category = 'Drinks'";

        //        using (SqlConnection conn = DbHelper.GetConnection())
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(query, conn))
        //            {
        //                object result = cmd.ExecuteScalar();

        //                if (result != DBNull.Value && result != null)
        //                {
        //                    decimal total = Convert.ToDecimal(result);
        //                    totalPrice.Text = $"Php {total:F2}";                        }
        //                else
        //                {
        //                    totalPrice.Text = "Php 0.00";                        }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error calculating total price for appetizers: " + ex.Message);
        //    }
        //}
        private void getTotalPrice()
        {
            connect.Close();
            try
            {
                int userId = GetUserId(connect, username);
                string query = @"
        SELECT SUM(Orders.TotalAmount) 
        FROM Orders
        INNER JOIN Food ON Orders.FoodId = Food.Id
        INNER JOIN Categories ON Food.category_id = Categories.Id
        WHERE Categories.category = 'Drinks' AND Orders.UserId = @UserId"; // Corrected query syntax

                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId); // Added UserId parameter

                        object result = cmd.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            decimal total = Convert.ToDecimal(result);
                            totalPrice.Text = $"Php {total:F2}";
                            conn.Close();
                        }
                        else
                        {
                            totalPrice.Text = "Php 0.00";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating total price for appetizers: " + ex.Message);
            }
        }

        private void DeleteRowFromDatabase(string OrderId)
        {
            try
            {
                string query = "DELETE FROM Orders WHERE OrderId = @OrderId";

                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", OrderId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            MessageBox.Show("Item not found in the database.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting from database: " + ex.Message);
            }
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (appetizerGridView.SelectedRows.Count > 0)
                {
                    int selectedRowIndex = appetizerGridView.SelectedRows[0].Index;
                    string idToDelete = appetizerGridView.Rows[selectedRowIndex].Cells["OrderId"].Value.ToString();

                    var confirmation = MessageBox.Show($"Are you sure you want to delete the item with ID {idToDelete}?", "Confirm Delete", MessageBoxButtons.YesNo);
                    if (confirmation == DialogResult.Yes)
                    {
                        DeleteRowFromDatabase(idToDelete);

                        appetizerGridView.Rows.RemoveAt(selectedRowIndex);

                        MessageBox.Show("Item deleted successfully.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }

                getTotalItem();                getTotalPrice();            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting item: " + ex.Message);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void appetizerGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadAppetizerData();
        }
    }
}
