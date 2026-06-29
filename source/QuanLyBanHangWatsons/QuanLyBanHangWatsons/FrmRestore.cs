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
    public partial class FrmRestore : Form
    {
        Ket_noi kn = new Ket_noi();
        public FrmRestore()
        {
            InitializeComponent();
        }

        private void FrmRestore_Load(object sender, EventArgs e)
        {
            txtUrl.Enabled = false;
            btnRestore.Enabled = false;
        }

       
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Backup files (*.bak)|*.bak";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtUrl.Text = dialog.FileName;
                btnRestore.Enabled = true;
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                MessageBox.Show("Vui lòng chọn file backup!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show(
                "Restore sẽ ghi đè dữ liệu hiện tại.\nBạn có chắc chắn không?",
                "Xác nhận",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (dr != DialogResult.OK) return;

            string dbName = "QuanLyBanHangWatsons";
            string backupFile = txtUrl.Text;

            string sql = $@"
            ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [{dbName}]
            FROM DISK = N'{backupFile}'
            WITH REPLACE;
            ALTER DATABASE [{dbName}] SET MULTI_USER;
            ";

            using (SqlConnection conn = kn.GetConnectMaster())
            {
                if (conn == null) return;

                try
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = 0; // tránh timeout
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Restore dữ liệu thành công!\nỨng dụng sẽ đóng để khởi động lại.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                    Application.Exit();// rất nên có
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Restore thất bại: " + ex.Message,
                                    "Lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }
    }
}
