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
    public partial class StatusForm : Form
    {
        private SqlConnection connect;

        public StatusForm()
        {
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
                string query = "SELECT * FROM Orders";
                DataTable table = DbHelper.ExecuteQuery(query);

                appetizerGridView.DataSource = table;


                appetizerGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                appetizerGridView.RowHeadersVisible = true;
                appetizerGridView.ReadOnly = false;            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }
    }
}
