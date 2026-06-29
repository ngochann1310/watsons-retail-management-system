using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace QuanLyBanHangWatsons
{
    public partial class FrmCTTKKhachHang : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maTK;

        public FrmCTTKKhachHang(string matk, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maTK = matk;

            if (mode == "add")
            {
                txtMaTKKH.Enabled = false;   // CHẶN NHẬP
                txtMaTKKH.Text = "";
                txtQuyen.Enabled = false;
                txtQuyen.Text = "";
            }


            if (mode == "edit")
            {
                LoadDetail();
                btnThem.Enabled = false;
                txtMaTKKH.Enabled = false;
                txtQuyen.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void LoadDetail()
        {
            string sql = $"SELECT * FROM tblTaiKhoanKH WHERE MaTaiKhoanKH = '{maTK}'";
            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            txtMaTKKH.Text = r["MaTaiKhoanKH"].ToString();
            txtTenTKKH.Text = r["TenTaiKhoan"].ToString();
            cboMaKH.Text = r["MaKhachHang"].ToString();
            txtMatKhau.Text = r["MatKhau"].ToString();
            txtQuyen.Text = r["Quyen"].ToString();
            dtpNCN.Value = Convert.ToDateTime(r["NgayCapNhat"]);
            tglTTTK.Checked = Convert.ToBoolean(r["TrangThaiTaiKhoan"]);


            // Load hình ảnh
            string fileAnh = r["HinhAnh"].ToString();
            picTKKH.Tag = fileAnh;            // gán Tag để UPDATE không bị mất ảnh

            if (!string.IsNullOrEmpty(fileAnh))
            {
                string fullPath = Path.Combine(Application.StartupPath, @"..\..\tool\avatar\", fileAnh);

                if (File.Exists(fullPath))
                {
                    // load ảnh không lock file
                    using (Image img = Image.FromFile(fullPath))
                    {
                        picTKKH.Image = new Bitmap(img);
                    }
                    picTKKH.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else picTKKH.Image = null;
            }
            else picTKKH.Image = null;
        }

        private void FrmCTTKKhachHang_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaKH();

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaTKKH.Text))
                {
                    maTK = GenerateNewMaTKKH();  
                    txtMaTKKH.Text = maTK;        
                }
            }

            if (mode == "edit")
            {
                LoadDetail();
            }
        }


        private string GenerateNewMaTKKH()
        {
            string sql = "SELECT TOP 1 MaTaiKhoanKH FROM tblTaiKhoanKH ORDER BY MaTaiKhoanKH DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value)
                return "TKKH0001";

            string last = obj.ToString().Trim();

            // lấy phần số cuối liên tiếp từ cuối chuỗi (GIỐNG FORM MẪU)
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // TKKH
            string numPart = last.Substring(i + 1);     // 0001

            int num = 0;
            if (!int.TryParse(numPart, out num))
            {
                return prefix + "1";
            }

            num++;
            string newNum = num.ToString().PadLeft(numPart.Length, '0');
            return prefix + newNum;
        }


        private void LoadComboBoxMaKH()
        {
            string query = "SELECT MaKhachHang  FROM tblKhachHang";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaKH.DataSource = dt;
            cboMaKH.DisplayMember = "MaKhachHang";
            cboMaKH.ValueMember = "MaKhachHang";
            cboMaKH.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (mode != "add") return;

            // Validate
            if (string.IsNullOrEmpty(txtTenTKKH.Text.Trim()))
            {
                MessageBox.Show("Tên tài khoản không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cboMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn Mã khách hàng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int trangThai = tglTTTK.Checked ? 1 : 0;

                string tenFileAnh = (picTKKH.Image != null && picTKKH.Tag != null)
                    ? picTKKH.Tag.ToString()
                    : "";

                string sql = $@"
                INSERT INTO tblTaiKhoanKH
                (MaTaiKhoanKH, TenTaiKhoan, MaKhachHang, MatKhau, Quyen, 
                 HinhAnh, TrangThaiTaiKhoan, NgayCapNhat)
                VALUES
                ('{maTK}',
                 N'{txtTenTKKH.Text}',
                 '{cboMaKH.Text}',
                 N'{txtMatKhau.Text}',
                 N'{txtQuyen.Text}',
                 {(string.IsNullOrEmpty(tenFileAnh) ? "NULL" : $"'{tenFileAnh}'")},
                 {trangThai},
                 '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}')";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Thêm tài khoản khách hàng thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // chỉ đóng form khi thêm thành công
                }
                else
                {
                    MessageBox.Show("Thêm thất bại! Dữ liệu không hợp lệ, vui lòng kiểm tra lại.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // KHÔNG đóng form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm thất bại!\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return; // KHÔNG đóng form nếu bị lỗi
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (mode != "edit" || string.IsNullOrEmpty(maTK))
            {
                MessageBox.Show("Không xác định tài khoản cần xóa.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa tài khoản này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                int rows = kn.ExecuteNonQuery(
                    $"DELETE FROM tblTaiKhoanKH WHERE MaTaiKhoanKH = '{maTK}'"
                );

                if (rows > 0)
                    MessageBox.Show("Xóa thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Xóa thất bại!", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa thất bại!\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (mode != "edit" || string.IsNullOrEmpty(maTK))
            {
                MessageBox.Show("Không xác định tài khoản cần sửa.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int trangThai = tglTTTK.Checked ? 1 : 0;

                string tenFileAnh = (picTKKH.Image != null && picTKKH.Tag != null)
                    ? picTKKH.Tag.ToString()
                    : "";

                string sql = $@"
                UPDATE tblTaiKhoanKH SET
                    TenTaiKhoan = N'{txtTenTKKH.Text}',
                    MaKhachHang = '{cboMaKH.Text}',
                    MatKhau = N'{txtMatKhau.Text}',
                    Quyen = N'{txtQuyen.Text}',
                    HinhAnh = {(string.IsNullOrEmpty(tenFileAnh) ? "NULL" : $"'{tenFileAnh}'")},
                    TrangThaiTaiKhoan = {trangThai},
                    NgayCapNhat = '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}'
                WHERE MaTaiKhoanKH = '{maTK}'";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Sửa thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // chỉ đóng form nếu sửa thành công
                }
                else
                {
                    MessageBox.Show("Sửa thất bại! Dữ liệu không hợp lệ hoặc không thay đổi.", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // KHÔNG đóng form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sửa thất bại!\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // KHÔNG đóng form khi lỗi
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
                    picTKKH.Image = new Bitmap(img);
                }

                picTKKH.SizeMode = PictureBoxSizeMode.Zoom;
                picTKKH.Tag = fileName; // ⭐ GIỮ TÊN FILE CHO UPDATE & INSERT
            }
        }

        private void btnQLKH_Click(object sender, EventArgs e)
        {

        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            // Kiểm tra đã chọn mã KH chưa
            if (string.IsNullOrEmpty(cboMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn mã khách hàng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maKH = cboMaKH.Text;

            // Kiểm tra khách hàng có tồn tại không (cho chắc)
            string sql = $"SELECT COUNT(*) FROM tblKhachHang WHERE MaKhachHang = '{maKH}'";
            int kq = Convert.ToInt32(kn.ExecuteScalar(sql));

            if (kq == 0)
            {
                MessageBox.Show("Khách hàng không tồn tại!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mở form chi tiết khách hàng
            FrmCTKhachHang frm = new FrmCTKhachHang(maKH, "edit");

            // Nếu bà dùng MDI
            frm.MdiParent = this.MdiParent;

            frm.Show();
        }
    }
}
