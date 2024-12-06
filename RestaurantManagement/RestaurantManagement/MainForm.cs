using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantManagement
{
    public partial class MainForm : Form
    {
        private AdminAddCategory adminAddCategory;


        private void InitializeCustomControls()
        {
            adminAddCategory = new AdminAddCategory
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(adminAddCategory);
        }



        public MainForm()




        {
            InitializeComponent();
            InitializeCustomControls();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        private void Logout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Logout?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LoginForm Login = new LoginForm();
                Login.Show();

                this.Hide();
            }
        }

        private void Dashboard_btn(object sender, EventArgs e)
        {


        }

        private void adminAdduser1_Load(object sender, EventArgs e)
        {

        }

        private void adminAddCategory2_Load(object sender, EventArgs e)
        {

        }
    }
}

