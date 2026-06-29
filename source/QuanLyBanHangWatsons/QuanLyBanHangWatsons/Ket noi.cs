using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{
    internal class Ket_noi
    {
        private string connectionString = @"Data Source=DESKTOP-L2OD0ED; Initial Catalog=QuanLyBanHangWatsons; Integrated Security=True";

        public SqlConnection GetConnect()
        {
            SqlConnection sqlConn = new SqlConnection(connectionString);
            try
            {
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kết nối không thành công: " + ex.Message);
                return null;
            }
            return sqlConn;
        }

        public int ExecuteNonQuery(string query)
        {
            using (SqlConnection conn = GetConnect())
            {
                if (conn == null) return 0;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        return cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi thực thi câu lệnh: " + ex.Message);
                        return 0;
                    }
                }
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnect())
            {
                if (conn == null) return dt;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    try
                    {
                        da.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi truy vấn dữ liệu: " + ex.Message);
                    }
                }
            }
            return dt;
        }

        public DataTable ExecuteQueryWithParams(string query, SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnect())
            {
                if (conn == null) return dt;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            da.Fill(dt);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi truy vấn dữ liệu với tham số: " + ex.Message);
                        }
                    }
                }
            }
            return dt;
        }

        public int ExecuteNonQueryWithParams(string query, SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnect())
            {
                if (connection == null) return 0;
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        // Kiểm tra nếu parameters là null, tạo mảng rỗng để tránh lỗi
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        return command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi thực thi câu lệnh với tham số: " + ex.Message);
                        return 0;
                    }
                }
            }
        }

        public object ExecuteScalar(string query)
        {
            using (SqlConnection conn = GetConnect())
            {
                if (conn == null) return null;
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        return cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi thực thi ExecuteScalar: " + ex.Message);
                        return null;
                    }
                }
            }
        }

        public object ExecuteScalarWithParams(string query, SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnect())
            {
                if (conn == null) return null;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        return cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi ExecuteScalarWithParams: " + ex.Message);
                        return null;
                    }
                }
            }
        }

        public SqlConnection GetConnectMaster()
        {
            string connStr = @"Data Source=DESKTOP-L2OD0ED\SQLEXPRESS;
                       Initial Catalog=master;
                       Integrated Security=True";

            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kết nối master thất bại: " + ex.Message);
                return null;
            }
            return conn;
        }



    }
}
