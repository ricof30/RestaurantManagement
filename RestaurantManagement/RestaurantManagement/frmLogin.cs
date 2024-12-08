using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;
using connectState;

namespace RestaurantManagement
{

    public partial class LoginForm : Form
    {
        //private SqlConnection connect;
        
        public LoginForm()
        {
            InitializeComponent();
            //connect = DbHelper.GetConnection();
            InvalidU.Hide();
            InvalidP.Hide();
        }


        private void Guna2Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }




        private void Register_label_Click(object sender, EventArgs e)
        {
            RegisterLabel registerForm = new RegisterLabel();
            registerForm.Show();

            this.Hide();
        }

        private void Login_showPass_CheckedChanged(object sender, EventArgs e)
        {
            txtLoginPassword.PasswordChar = Login_showPass.Checked ? '\0' : '*';
        }

        public bool checkConnection()
        {
            using (SqlConnection connect = DbHelper.GetConnection())
            {
                try
                {
                    connect.Open();
                    return connect.State == ConnectionState.Open;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }


        private void Login_User_Click(object sender, EventArgs e)
        {
            using (SqlConnection connect = DbHelper.GetConnection())
            {
                if (checkConnection())
                {
                    try
                    {
                        string selectData = "SELECT * FROM users WHERE username = @usern AND password = @pass";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            // Ensure to use specific types
                            cmd.Parameters.Add("@usern", SqlDbType.VarChar).Value = txtLoginUsername.Text.Trim();
                            cmd.Parameters.Add("@pass", SqlDbType.VarChar).Value = txtLoginPassword.Text.Trim(); // Hash password if stored in the database

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count > 0)
                            {
                                MessageBox.Show("Login Successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                MainForm mForm = new MainForm();
                                mForm.Show();


                               
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect username/password or there's no Admin Approval", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Correction Failed: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                    
                }
            }
        }

        private void Login_User_Click_1(object sender, EventArgs e)
        {
            if (txtLoginUsername.Text == "" && txtLoginPassword.Text == "")
            {
                MessageBox.Show("Please fill all the empty fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtLoginUsername.Text == "")
            {
                MessageBox.Show("Please fill username fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (txtLoginPassword.Text == "")
            {
                MessageBox.Show("Please fill password fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection connect = DbHelper.GetConnection())
            {
                if (checkConnection())
                {
                    try
                    {
                        string selectData = "SELECT * FROM users WHERE username = @usern AND password = @pass";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            // Ensure to use specific types
                            cmd.Parameters.Add("@usern", SqlDbType.VarChar).Value = txtLoginUsername.Text.Trim();
                            cmd.Parameters.Add("@pass", SqlDbType.VarChar).Value = txtLoginPassword.Text.Trim(); // Hash password if stored in the database

                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count > 0)
                            {
                                MessageBox.Show("Login Successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                MainForm mForm = new MainForm();
                                mForm.Show();

                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect username/password or there's no Admin Approval", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Correction Failed: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }

                }
            }

            //if (txtLoginUsername.Text == "rodel")
            //{
            //    InvalidU.Hide();
            //    if (txtLoginPassword.Text == "Password")
            //    {
            //        InvalidP.Hide();
            //        MainForm mForm = new MainForm();
            //        mForm.Show();

            //        this.Hide();
            //    }
            //    else
            //    {
            //        InvalidP.Show();
            //    }
            //}
            //else
            //{
            //    InvalidU.Show();
            //}
        }

        private void tbnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_Username_TextChanged(object sender, EventArgs e)
        {

        }
    }
}