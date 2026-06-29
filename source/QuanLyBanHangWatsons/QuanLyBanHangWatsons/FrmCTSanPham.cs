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
    public partial class FrmCTSanPham : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maSP;

        public FrmCTSanPham(string masp, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maSP = masp;

            if (mode == "add")
            {
                txtMaSP.Enabled = false;   // CHẶN NHẬP
                txtMaSP.Text = "";
                txtMaVach.Enabled = false;   // CHẶN NHẬP
                txtMaVach.Text = "";
            }


            if (mode == "edit")
            {
                LoadDetail();
                btnThem.Enabled = false;
                txtMaSP.Enabled = false;
                txtMaVach.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }

        }

        private void LoadDetail()
        {
            string sql = $"SELECT * FROM tblSanPham WHERE MaSanPham = '{maSP}'";
            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            txtMaSP.Text = r["MaSanPham"].ToString();
            txtTenSP.Text = r["TenSanPham"].ToString();
            cboMaLSP.Text = r["MaLoaiSanPham"].ToString();
            txtMaVach.Text = r["MaVach"].ToString();
            txtMoTa.Text = r["MoTa"].ToString();
            tglTTSP.Checked = Convert.ToBoolean(r["TrangThaiSanPham"]);
            cboMaTH.Text = r["MaThuongHieu"].ToString();
            cboMaNCC.Text = r["MaNhaCungCap"].ToString();
            cboDonViTinh.Text = r["DonViTinh"].ToString();
            txtGiaBan.Text = r["GiaBan"].ToString();
            cboMaNV.Text = r["MaNhanVien"].ToString();
            dtpNCN.Value = Convert.ToDateTime(r["NgayCapNhat"]);

            // Load hình ảnh
            string fileAnh = r["HinhAnh"].ToString();
            picSP.Tag = fileAnh;            // gán Tag để UPDATE không bị mất ảnh

            if (!string.IsNullOrEmpty(fileAnh))
            {
                string fullPath = Path.Combine(Application.StartupPath, @"..\..\Images\", fileAnh);

                if (File.Exists(fullPath))
                {
                    // load ảnh không lock file
                    using (Image img = Image.FromFile(fullPath))
                    {
                        picSP.Image = new Bitmap(img);
                    }
                    picSP.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else picSP.Image = null;
            }
            else picSP.Image = null;
        }

        private void FrmCTSanPham_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaNV();
            LoadComboBoxMaLSP();
            LoadComboBoxMaTH();
            LoadComboBoxMaNCC();

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaSP.Text))
                {
                    maSP = GenerateNewMaSP();   // ⭐ gán cho biến dùng INSERT
                    txtMaSP.Text = maSP;        // ⭐ hiển thị lên textbox
                }
            }

            if (mode == "edit")
            {
                LoadDetail();
            }

            // Load số lượng tồn kho
            LoadTongSoLuongSanBan();
            LoadSoLuongSanBanTheoChiNhanh();
        }


        private string GenerateNewMaSP()
        {
            string sql = "SELECT TOP 1 MaSanPham FROM tblSanPham ORDER BY MaSanPham DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value)
                return "SP0001";

            string last = obj.ToString().Trim();

            // lấy phần số cuối liên tiếp từ cuối chuỗi
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // SP
            string numPart = last.Substring(i + 1);     // 0001

            int num = 0;
            if (!int.TryParse(numPart, out num))
            {
                // nếu không parse được, thêm 1
                return prefix + "1";
            }

            num++;
            // giữ độ dài phần số như trước (padding bằng 0)
            string newNum = num.ToString().PadLeft(numPart.Length, '0');
            return prefix + newNum;
        }


        private void LoadComboBoxMaLSP()
        {
            string query = "SELECT MaLoaiSanPham FROM tblLoaiSanPham";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaLSP.DataSource = dt;
            cboMaLSP.DisplayMember = "MaLoaiSanPham";
            cboMaLSP.ValueMember = "MaLoaiSanPham";
            cboMaLSP.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void LoadComboBoxMaTH()
        {
            string query = "SELECT MaThuongHieu FROM tblThuongHieu";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaTH.DataSource = dt;
            cboMaTH.DisplayMember = "MaThuongHieu";
            cboMaTH.ValueMember = "MaThuongHieu";
            cboMaTH.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void LoadComboBoxMaNV()
        {
            string query = "SELECT MaNhanVien FROM tblNhanVien";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaNV.DataSource = dt;
            cboMaNV.DisplayMember = "MaNhanVien";
            cboMaNV.ValueMember = "MaNhanVien";
            cboMaNV.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void LoadComboBoxMaNCC()
        {
            string query = "SELECT MaNhaCungCap FROM tblNhaCungCap";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaNCC.DataSource = dt;
            cboMaNCC.DisplayMember = "MaNhaCungCap";
            cboMaNCC.ValueMember = "MaNhaCungCap";
            cboMaNCC.SelectedIndex = -1;
        }

        private void LoadTongSoLuongSanBan()
        {
            string sql = $@"
            SELECT SUM(SoLuongSanBan) 
            FROM tblTonKho 
            WHERE MaSanPham = '{maSP}'";

            object result = kn.ExecuteScalar(sql);

            int tong = 0;
            if (result != null && result != DBNull.Value)
                tong = Convert.ToInt32(result);

            txtSumSLSanBan.Text = tong.ToString();
        }

        private void LoadSoLuongSanBanTheoChiNhanh()
        {
            string sql = $@"
            SELECT 
                cn.TenChiNhanh,
                tk.SoLuongSanBan
            FROM tblTonKho tk
            JOIN tblChiNhanh cn ON tk.MaChiNhanh = cn.MaChiNhanh
            WHERE tk.MaSanPham = '{maSP}'
            ORDER BY cn.TenChiNhanh";

            DataTable dt = kn.ExecuteQuery(sql);

            dgvSLMoiCN.Rows.Clear();

            foreach (DataRow r in dt.Rows)
            {
                dgvSLMoiCN.Rows.Add(
                    r["TenChiNhanh"].ToString(),
                    r["SoLuongSanBan"].ToString()
                );
            }
        }          

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Chặn khi không phải chế độ thêm
            if (mode != "add") return;

            // Validate bắt buộc
            if (string.IsNullOrEmpty(txtTenSP.Text.Trim()))
            {
                MessageBox.Show("Tên sản phẩm không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cboMaLSP.Text) ||
                string.IsNullOrEmpty(cboMaTH.Text) ||
                string.IsNullOrEmpty(cboMaNCC.Text) ||
                string.IsNullOrEmpty(cboMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn đủ Loại SP, Thương hiệu, Nhà cung cấp và Nhân viên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Lấy giá
                decimal giaBan = 0;
                decimal.TryParse(txtGiaBan.Text, out giaBan);

                // Lấy trạng thái
                int trangThai = tglTTSP.Checked ? 1 : 0;

                // Lấy file ảnh
                string tenFileAnh = (picSP.Image != null && picSP.Tag != null)
                    ? picSP.Tag.ToString()
                    : "";

                // INSERT
                string sql = $@"
                INSERT INTO tblSanPham
                (MaSanPham, TenSanPham, MaLoaiSanPham, MaThuongHieu, DonViTinh, GiaBan, MoTa,
                 TrangThaiSanPham, HinhAnh, MaVach, MaNhaCungCap, MaNhanVien, NgayCapNhat)
                VALUES
                ('{maSP}',
                 N'{txtTenSP.Text}',
                 '{cboMaLSP.Text}',
                 '{cboMaTH.Text}',
                 N'{cboDonViTinh.Text}',
                 {giaBan},
                 N'{txtMoTa.Text}',
                 {trangThai},
                 {(string.IsNullOrEmpty(tenFileAnh) ? "NULL" : $"'{tenFileAnh}'")},
                 '{txtMaVach.Text}',
                 '{cboMaNCC.Text}',
                 '{cboMaNV.Text}',
                 '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}')";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo",
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
            if (mode != "edit" || string.IsNullOrEmpty(maSP))
            {
                MessageBox.Show("Không xác định sản phẩm để xóa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                int rows = kn.ExecuteNonQuery(
                    $"DELETE FROM tblSanPham WHERE MaSanPham = '{maSP}'"
                );

                if (rows > 0)
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Xóa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa thất bại!\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (mode != "edit" || string.IsNullOrEmpty(maSP))
            {
                MessageBox.Show("Không xác định sản phẩm để sửa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                decimal giaBan = 0;
                decimal.TryParse(txtGiaBan.Text, out giaBan);

                int trangThai = tglTTSP.Checked ? 1 : 0;

                string tenFileAnh = (picSP.Image != null && picSP.Tag != null)
                        ? picSP.Tag.ToString()
                        : "";

                string sql = $@"
                UPDATE tblSanPham SET
                    TenSanPham = N'{txtTenSP.Text}',
                    MaLoaiSanPham = '{cboMaLSP.Text}',
                    MaThuongHieu = '{cboMaTH.Text}',
                    DonViTinh = N'{cboDonViTinh.Text}',
                    GiaBan = {giaBan},
                    MoTa = N'{txtMoTa.Text}',
                    TrangThaiSanPham = {trangThai},
                    HinhAnh = {(string.IsNullOrEmpty(tenFileAnh) ? "NULL" : $"'{tenFileAnh}'")},
                    MaVach = '{txtMaVach.Text}',
                    MaNhaCungCap = '{cboMaNCC.Text}',
                    MaNhanVien = '{cboMaNV.Text}',
                    NgayCapNhat = '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}'
                WHERE MaSanPham = '{maSP}'";

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

                string destDir = Path.Combine(Application.StartupPath, @"..\..\Images\");

                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                string destPath = Path.Combine(destDir, fileName);

                File.Copy(sourcePath, destPath, true);

                // load ảnh không lock file
                using (Image img = Image.FromFile(destPath))
                {
                    picSP.Image = new Bitmap(img);
                }

                picSP.SizeMode = PictureBoxSizeMode.Zoom;
                picSP.Tag = fileName; // ⭐ GIỮ TÊN FILE CHO UPDATE & INSERT
            }
        }

        private void btnQLLSP_Click(object sender, EventArgs e)
        {
            FrmLoaiSP f = new FrmLoaiSP();
            f.StartPosition = FormStartPosition.CenterScreen;  // căn giữa màn hình
            f.WindowState = FormWindowState.Normal;
            f.Show();
        }

        private void btnQLNCC_Click(object sender, EventArgs e)
        {
            FrmNhaCungCap f = new FrmNhaCungCap();
            f.StartPosition = FormStartPosition.CenterScreen;  // căn giữa màn hình
            f.WindowState = FormWindowState.Normal;
            f.Show();
        }

        private void btnQLTH_Click(object sender, EventArgs e)
        {
            FrmThuongHieu f = new FrmThuongHieu();
            f.StartPosition = FormStartPosition.CenterScreen;  // căn giữa màn hình
            f.WindowState = FormWindowState.Normal;
            f.Show();
        }
    }
}
