using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace RestaurantManagement
{


    public static class DbHelper
    {
        // Method to get the connection string
        public static string GetConnectionString()
        {
            // Replace with your actual connection string
            return "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\RestaurantManagement\\RestaurantManagement\\RestaurantManagement\\RestaurantManagement\\Database.mdf;Integrated Security=True";
        }

        // Method to create and return a new SqlConnection
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
    }

    internal class dbHelper
    {
    }
}
