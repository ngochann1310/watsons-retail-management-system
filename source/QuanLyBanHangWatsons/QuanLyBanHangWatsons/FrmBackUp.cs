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

namespace QuanLyBanHangWatsons
{
    public partial class FrmBackUp : Form
    {
        Ket_noi kn = new Ket_noi();

        public FrmBackUp()
        {
            InitializeComponent();
        }

        private void FrmBackUp_Load(object sender, EventArgs e)
        {
            txtUrl.Enabled = false;
            btnBackup.Enabled = false;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtUrl.Text = dialog.SelectedPath;
                btnBackup.Enabled = true;
            }    
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                MessageBox.Show("Vui lòng chọn thư mục lưu backup!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Tạo tên file backup theo ngày giờ
            string dbName = "QuanLyBanHangWatsons";
            string fileName = dbName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";
            string fullPath = System.IO.Path.Combine(txtUrl.Text, fileName);

            string sqlBackup = $"BACKUP DATABASE [{dbName}] TO DISK = N'{fullPath}' WITH INIT";

            using (SqlConnection conn = kn.GetConnect())
            {
                if (conn == null) return;

                try
                {
                    using (SqlCommand cmd = new SqlCommand(sqlBackup, conn))
                    {
                        cmd.CommandTimeout = 0; // tránh timeout khi DB lớn
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Backup dữ liệu thành công!",
                                    "Thành công",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Backup thất bại: " + ex.Message,
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }
    }
}
