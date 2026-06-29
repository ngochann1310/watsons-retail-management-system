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
    public partial class FrmDangNhap : Form
    {
        Ket_noi kn = new Ket_noi();
        public FrmDangNhap()
        {
            InitializeComponent();
            AddPlaceholder(txtTenDangNhap, "Tên đăng nhập");
            AddPlaceholder(txtMatKhau, "Mật khẩu");
            cboPQ.SelectedIndex = -1;
        }

        private void AddPlaceholder(TextBox textBox, string placeholderText, bool isPassword = false)
        {
            textBox.Text = placeholderText;
            textBox.ForeColor = Color.Gray;
            textBox.UseSystemPasswordChar = false;

            textBox.GotFocus += (sender, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;

                    if (isPassword)
                        textBox.UseSystemPasswordChar = isPasswordHidden;
                }
            };

            textBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = Color.Gray;
                    textBox.UseSystemPasswordChar = false;
                }
            };
        }

        private void FrmDangNhap_Load(object sender, EventArgs e)
        {
            // Ẩn mật khẩu ngay từ đầu
            txtMatKhau.UseSystemPasswordChar = true;

            // Đặt icon mặc định là eye_close + resize
            string iconClose = Path.Combine(Application.StartupPath, @"..\..\tool\icon\eye_close.png");

            if (File.Exists(iconClose))
            {
                using (Image img = Image.FromFile(iconClose))
                {
                    btnAnHienPass.Image = new Bitmap(img, new Size(30, 30));
                }
            }
        }

        private DataTable DangNhap(string user, string pass, string quyen)
        {
            string sql = @"
        SELECT tk.MaNhanVien, nv.HoNhanVien, nv.TenNhanVien, tk.Quyen
        FROM tblTaiKhoanNV tk
        JOIN tblNhanVien nv ON tk.MaNhanVien = nv.MaNhanVien
        WHERE tk.TenTaiKhoan = @User
          AND tk.MatKhau = @Pass
          AND tk.Quyen = @Quyen";

            SqlParameter[] param =
            {
        new SqlParameter("@User", user),
        new SqlParameter("@Pass", pass),
        new SqlParameter("@Quyen", quyen)
    };

            return kn.ExecuteQueryWithParams(sql, param);
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string TenDangNhap = txtTenDangNhap.Text.Trim();
            string MatKhau = txtMatKhau.Text.Trim();

            if (cboPQ.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn quyền!", "Thông báo");
                return;
            }

            string Quyen = cboPQ.SelectedItem.ToString();

            DataTable dt = DangNhap(TenDangNhap, MatKhau, Quyen);

            if (dt.Rows.Count > 0)
            {
                string maNV = dt.Rows[0]["MaNhanVien"].ToString();
                string ho = dt.Rows[0]["HoNhanVien"].ToString();
                string ten = dt.Rows[0]["TenNhanVien"].ToString();
                string quyenNV = dt.Rows[0]["Quyen"].ToString();

                MessageBox.Show("Đăng nhập thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Hide();

                FrmMainFull frm = new FrmMainFull(maNV, ho + " " + ten, quyenNV);
                frm.Show();
            }
            else
            {
                MessageBox.Show("Sai thông tin đăng nhập!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool isPasswordHidden = true;

        private void btnAnHienPass_Click(object sender, EventArgs e)
        {
            string iconOpen = Path.Combine(Application.StartupPath, @"..\..\tool\icon\eye_open.png");
            string iconClose = Path.Combine(Application.StartupPath, @"..\..\tool\icon\eye_close.png");

            if (isPasswordHidden)
            {
                // Hiện mật khẩu
                txtMatKhau.UseSystemPasswordChar = false;

                // Load & resize icon open
                if (File.Exists(iconOpen))
                {
                    using (Image img = Image.FromFile(iconOpen))
                    {
                        btnAnHienPass.Image = new Bitmap(img, new Size(30, 30));
                    }
                }

                isPasswordHidden = false;
            }
            else
            {
                // Ẩn mật khẩu
                txtMatKhau.UseSystemPasswordChar = true;

                // Load & resize icon close
                if (File.Exists(iconClose))
                {
                    using (Image img = Image.FromFile(iconClose))
                    {
                        btnAnHienPass.Image = new Bitmap(img, new Size(30, 30));
                    }
                }

                isPasswordHidden = true;
            }
        }
    }
}
