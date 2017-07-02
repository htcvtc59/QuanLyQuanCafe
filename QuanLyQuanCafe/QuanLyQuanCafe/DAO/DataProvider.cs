using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class DataProvider
    {
        //Singleton
        private static DataProvider instance;


        private string connectionStr = "Data Source=.;Initial Catalog=QuanLyQuanCafe;Integrated Security=True;User ID=sa";
        SqlConnection conn;

        public static DataProvider Instance
        {
            get
            {
                if (instance == null) { instance = new DataProvider(); }
                return instance;
            }

            private set
            {
                instance = value;
            }
        }

        private DataProvider()
        {
            conn = new SqlConnection(connectionStr);
        }

        public DataTable ExecuteQuery(string sql, object[] parameter = null)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (parameter != null)
            {
                string[] listPara = sql.Split(' ');
                int i = 0;
                foreach (string item in listPara)
                {
                    if (item.Contains('@'))
                    {
                        cmd.Parameters.AddWithValue(item, parameter[i]);
                        i++;
                    }
                }
            }

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            conn.Close();
            return dt;
        }
        public int ExecuteNonQuery(string sql, object[] parameter = null)
        {
            int dt = 0;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (parameter != null)
            {
                string[] listPara = sql.Split(' ');
                int i = 0;
                foreach (string item in listPara)
                {
                    if (item.Contains('@'))
                    {
                        cmd.Parameters.AddWithValue(item, parameter[i]);
                        i++;
                    }
                }
            }
            dt = cmd.ExecuteNonQuery();

            conn.Close();
            return dt;
        }


        public object ExecuteScalar(string sql, object[] parameter = null)
        {
            object dt = 0;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (parameter != null)
            {
                string[] listPara = sql.Split(' ');
                int i = 0;
                foreach (string item in listPara)
                {
                    if (item.Contains('@'))
                    {
                        cmd.Parameters.AddWithValue(item, parameter[i]);
                        i++;
                    }
                }
            }
            dt = cmd.ExecuteScalar();

            conn.Close();
            return dt;
        }

    }
}
