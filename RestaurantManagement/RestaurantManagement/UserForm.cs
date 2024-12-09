using System;
using System.Collections;
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
    public partial class UserForm : Form
    {
        private SqlConnection connect;
        private string username;

        public UserForm(string loggedInUsername)
        {
            username = loggedInUsername;
            connect = DbHelper.GetConnection();
            InitializeComponent();
            this.Load += new EventHandler(this.UserForm_Load);
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            LoadAppetizerData();            AppetizerForm appetizer = new AppetizerForm(username);
            ShowUserPanel.Controls.Clear();
            appetizer.TopLevel = false;
            ShowUserPanel.Controls.Add(appetizer);
            appetizer.Dock = DockStyle.Fill;
            appetizer.Show();
        }

        private void LoadAppetizerData()
        {
            try
            {
                string query = "SELECT * FROM Categories";
                DataTable table = DbHelper.ExecuteQuery(query);

                dataGridView1.DataSource = table;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ReadOnly = true;
                dataGridView1.RowHeadersVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AppetizerForm appetizer = new AppetizerForm(username);
            ShowUserPanel.Controls.Clear();
            appetizer.TopLevel = false;
            ShowUserPanel.Controls.Add(appetizer);
            appetizer.Dock = DockStyle.Fill;
            appetizer.Show();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowUserPanel.Controls.Clear();
            RamenForm ramen = new RamenForm(username);
            ramen.TopLevel = false;
            ShowUserPanel.Controls.Add(ramen);
            ramen.Dock = DockStyle.Fill;
            ramen.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ShowUserPanel.Controls.Clear();
            DrinkForm drink = new DrinkForm(username);
            drink.TopLevel = false;
            ShowUserPanel.Controls.Add(drink);
            drink.Dock = DockStyle.Fill;
            drink.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ShowUserPanel.Controls.Clear();
            StatusForm status = new StatusForm(username);
            status.TopLevel = false;
            ShowUserPanel.Controls.Add(status);
            status.Dock = DockStyle.Fill;
            status.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Logout?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LoginForm Login = new LoginForm();
                Login.Show();
                username = string.Empty;
                this.Hide();
            }
        }

        private void LoadAppetizerData2()
        {
            try
            {
                string query = "SELECT * FROM Orders";
                DataTable table = DbHelper.ExecuteQuery(query);

                dataGridView1.DataSource = table;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ReadOnly = true;
                dataGridView1.RowHeadersVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UserForm_Load_1(object sender, EventArgs e)
        {
            lblUsername.Text = username;
            LoadAppetizerData2();
        }

    

        private void AddToOrder(string itemId)
        {
            MessageBox.Show($"Item {itemId} added to order!");
        }

        private void AddMenu1_Click(object sender, EventArgs e)
        {

        }

        private void lblUsername_Click(object sender, EventArgs e)
        {

        }

        private void ShowUserPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
