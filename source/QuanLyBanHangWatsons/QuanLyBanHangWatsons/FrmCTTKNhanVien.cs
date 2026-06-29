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
    public partial class FrmCTTKNhanVien : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maTK;

        public FrmCTTKNhanVien(string matk, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maTK = matk;

            if (mode == "add")
            {
                txtMaTKNV.Enabled = false;   // CHẶN NHẬP
                txtMaTKNV.Text = "";
            }


            if (mode == "edit")
            {
                LoadDetail();
                btnThem.Enabled = false;
                txtMaTKNV.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void LoadDetail()
        {
            string sql = $"SELECT * FROM tblTaiKhoanNV WHERE MaTaiKhoanNV = '{maTK}'";
            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            txtMaTKNV.Text = r["MaTaiKhoanNV"].ToString();
            txtTenTKNV.Text = r["TenTaiKhoan"].ToString();
            cboMaNV.Text = r["MaNhanVien"].ToString();
            txtMatKhau.Text = r["MatKhau"].ToString();
            cboQuyen.Text = r["Quyen"].ToString();
            tglTTTK.Checked = Convert.ToInt32(r["TrangThaiTaiKhoan"]) == 1;
            dtpNCN.Value = Convert.ToDateTime(r["NgayCapNhat"]);

            // Load hình ảnh
            string fileAnh = r["HinhAnh"].ToString();
            picTKNV.Tag = fileAnh;            // gán Tag để UPDATE không bị mất ảnh

            if (!string.IsNullOrEmpty(fileAnh))
            {
                string fullPath = Path.Combine(Application.StartupPath, @"..\..\tool\avatar\", fileAnh);

                if (File.Exists(fullPath))
                {
                    // load ảnh không lock file
                    using (Image img = Image.FromFile(fullPath))
                    {
                        picTKNV.Image = new Bitmap(img);
                    }
                    picTKNV.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else picTKNV.Image = null;
            }
            else picTKNV.Image = null;
        }

        private void FrmCTTKNhanVien_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaNV();

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaTKNV.Text))
                {
                    maTK = GenerateNewMaTKNV();  
                    txtMaTKNV.Text = maTK;        
                }
            }

            if (mode == "edit")
            {
                LoadDetail();
            }
        }


        private string GenerateNewMaTKNV()
        {
            string sql = "SELECT TOP 1 MaTaiKhoanNV FROM tblTaiKhoanNV ORDER BY MaTaiKhoanNV DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value)
                return "TKNV0001";

            string last = obj.ToString().Trim();

            // LẤY PHẦN SỐ CUỐI – GIỐNG HỆT FORM MẪU
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // TKNV
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


        private void LoadComboBoxMaNV()
        {
            string query = "SELECT MaNhanVien  FROM tblNhanVien";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaNV.DataSource = dt;
            cboMaNV.DisplayMember = "MaNhanVien";
            cboMaNV.ValueMember = "MaNhanVien";
            cboMaNV.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (mode != "add") return;

            // Validate
            if (string.IsNullOrEmpty(txtTenTKNV.Text.Trim()))
            {
                MessageBox.Show("Tên tài khoản không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cboMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn Mã nhân viên!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int trangThai = tglTTTK.Checked ? 1 : 0;

                string tenFileAnh = (picTKNV.Image != null && picTKNV.Tag != null)
                    ? picTKNV.Tag.ToString()
                    : "";

                string sql = $@"
                INSERT INTO tblTaiKhoanNV
                (MaTaiKhoanNV, TenTaiKhoan, MaNhanVien, MatKhau, Quyen, 
                 HinhAnh, TrangThaiTaiKhoan, NgayCapNhat)
                VALUES
                ('{maTK}',
                 N'{txtTenTKNV.Text}',
                 '{cboMaNV.Text}',
                 N'{txtMatKhau.Text}',
                 N'{cboQuyen.Text}',
                 {(string.IsNullOrEmpty(tenFileAnh) ? "NULL" : $"'{tenFileAnh}'")},
                 {trangThai},
                 '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}')";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Thêm tài khoản nhân viên thành công!", "Thông báo",
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
                    $"DELETE FROM tblTaiKhoanNV WHERE MaTaiKhoanNV = '{maTK}'"
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

                string tenFileAnh = (picTKNV.Image != null && picTKNV.Tag != null)
                    ? picTKNV.Tag.ToString()
                    : "";

                string sql = $@"
                UPDATE tblTaiKhoanNV SET
                    TenTaiKhoan = N'{txtTenTKNV.Text}',
                    MaNhanVien = '{cboMaNV.Text}',
                    MatKhau = N'{txtMatKhau.Text}',
                    Quyen = N'{cboQuyen.Text}',
                    HinhAnh = {(string.IsNullOrEmpty(tenFileAnh) ? "NULL" : $"'{tenFileAnh}'")},
                    TrangThaiTaiKhoan = {trangThai},
                    NgayCapNhat = '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}'
                WHERE MaTaiKhoanNV = '{maTK}'";

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
                    picTKNV.Image = new Bitmap(img);
                }

                picTKNV.SizeMode = PictureBoxSizeMode.Zoom;
                picTKNV.Tag = fileName; // ⭐ GIỮ TÊN FILE CHO UPDATE & INSERT
            }
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboMaNV.Text))
            {
                MessageBox.Show("Không xác định được mã nhân viên!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maNV = cboMaNV.Text;

            // Mở form chi tiết nhân viên
            FrmCTNhanVien frm = new FrmCTNhanVien(maNV, "edit");

            // Nếu đang dùng MDI
            frm.MdiParent = this.MdiParent;

            frm.Show();
        }
    }
}
