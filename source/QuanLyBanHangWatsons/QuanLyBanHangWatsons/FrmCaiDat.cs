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
using System.IO;

namespace QuanLyBanHangWatsons
{
    public partial class FrmCaiDat : Form
    {
        Ket_noi kn = new Ket_noi();
        string _maNV;

        public FrmCaiDat(string maNhanVienDangNhap)
        {
            InitializeComponent();
            _maNV = maNhanVienDangNhap;
        }

        private void FrmCaiDat_Load(object sender, EventArgs e)
        {
            LoadTaiKhoan();
        }

        private void LoadTaiKhoan()
        {
            string sql = "SELECT * FROM tblTaiKhoanNV WHERE MaNhanVien = @MaNV";
            SqlParameter[] param = {
            new SqlParameter("@MaNV", _maNV)
            };

            DataTable dt = kn.ExecuteQueryWithParams(sql, param);
            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];
            txtTenTKNV.Text = r["TenTaiKhoan"].ToString();
            txtMatKhau.Text = r["MatKhau"].ToString();
            txtQuyen.Text = r["Quyen"].ToString();
            dtpNCN.Value = Convert.ToDateTime(r["NgayCapNhat"]);

            // Load hình ảnh
            string fileAnh = r["HinhAnh"].ToString();
            picTKNV.Tag = fileAnh;

            if (!string.IsNullOrEmpty(fileAnh))
            {
                string fullPath = Path.Combine(Application.StartupPath, @"..\..\tool\avatar\", fileAnh);
                if (File.Exists(fullPath))
                {
                    using (Image img = Image.FromFile(fullPath))
                    {
                        picTKNV.Image = new Bitmap(img);
                    }
                    picTKNV.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else picTKNV.Image = null;
            }
            else picTKNV.Image = null;

            // Chỉ cho sửa tên và mật khẩu
            txtTenTKNV.Enabled = true;
            txtMatKhau.Enabled = true;
            btnTaiAnh.Enabled = true;

            // Các textbox khác chỉ đọc
            txtQuyen.Enabled = false;
            dtpNCN.Enabled = false;

            // ⭐ Load trạng thái tài khoản từ database
            if (r["TrangThaiTaiKhoan"] != DBNull.Value)
                tglTTTK.Checked = Convert.ToBoolean(r["TrangThaiTaiKhoan"]);

            tglTTTK.Enabled = false; // vẫn chỉ đọc
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenTKNV.Text.Trim()) || string.IsNullOrEmpty(txtMatKhau.Text.Trim()))
            {
                MessageBox.Show("Tên tài khoản và mật khẩu không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tenFileAnh = (picTKNV.Image != null && picTKNV.Tag != null) ? picTKNV.Tag.ToString() : "";

            string sql = @"
            UPDATE tblTaiKhoanNV SET
                TenTaiKhoan = @TenTaiKhoan,
                MatKhau = @MatKhau,
                HinhAnh = @HinhAnh,
                NgayCapNhat = GETDATE()
            WHERE MaNhanVien = @MaNV";

            SqlParameter[] param = {
        new SqlParameter("@TenTaiKhoan", txtTenTKNV.Text.Trim()),
        new SqlParameter("@MatKhau", txtMatKhau.Text.Trim()),
        new SqlParameter("@HinhAnh", string.IsNullOrEmpty(tenFileAnh) ? (object)DBNull.Value : tenFileAnh),
        new SqlParameter("@MaNV", _maNV)
    };

            int rows = kn.ExecuteNonQueryWithParams(sql, param);
            if (rows > 0)
            {
                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTaiKhoan(); // load lại dữ liệu mới
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTaiAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Ảnh (*.png;*.jpg)|*.png;*.jpg";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string sourcePath = dlg.FileName;
                string fileName = Path.GetFileName(sourcePath);

                string destDir = Path.Combine(Application.StartupPath, @"..\..\tool\avatar\");

                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                string destPath = Path.Combine(destDir, fileName);

                File.Copy(sourcePath, destPath, true);

                // load ảnh không lock file
                using (Image img = Image.FromFile(destPath))
                {
                    picTKNV.Image = new Bitmap(img);
                }

                picTKNV.SizeMode = PictureBoxSizeMode.Zoom;
                picTKNV.Tag = fileName; // ⭐ GIỮ TÊN FILE CHO UPDATE & INSERT
            }
        }
    }
}
