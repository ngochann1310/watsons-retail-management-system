using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{
    public partial class FrmCTKhuyenMai : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maKM;

        public FrmCTKhuyenMai(string makm, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maKM = makm;

            if (mode == "add")
            {
                txtMaKM.Enabled = false;   // CHẶN NHẬP
                txtMaKM.Text = "";
            }


            if (mode == "edit")
            {
                LoadDetail();
                btnThem.Enabled = false;
                txtMaKM.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private string GenerateNewMaKM()
        {
            string sql = "SELECT TOP 1 MaKhuyenMai FROM tblKhuyenMai ORDER BY MaKhuyenMai DESC";
            var obj = kn.ExecuteScalar(sql);

            // Nếu bảng chưa có dữ liệu → trả về mã đầu tiên
            if (obj == null || obj == DBNull.Value)
                return "KM0001";

            string last = obj.ToString().Trim();

            // Lấy phần số liên tiếp ở cuối chuỗi
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // chữ: CTCN
            string numPart = last.Substring(i + 1);     // số: 0001

            int num = 0;
            if (!int.TryParse(numPart, out num))
                return prefix + "1";   // fallback nếu parse lỗi

            num++;

            // Giữ nguyên độ dài phần số
            string newNum = num.ToString().PadLeft(numPart.Length, '0');
            return prefix + newNum;
        }

        private void LoadDetail()
        {
            string sql = $"SELECT * FROM tblKhuyenMai WHERE MaKhuyenMai = '{maKM}'";
            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            txtMaKM.Text = r["MaKhuyenMai"].ToString();
            txtTenKM.Text = r["TenKhuyenMai"].ToString();
            txtGiaTri.Text = r["GiaTri"].ToString();
            txtMoTa.Text = r["MoTa"].ToString();
            txtGTriDH.Text = r["GiaTriDonHang"].ToString();

            cboDonAD.Text = r["LoaiDonHangApDung"].ToString();
            cboHangAD.Text = r["HangThanhVienApDung"].ToString();

            tglSinhNhat.Checked = Convert.ToBoolean(r["ApDungThangSinhNhat"]);
            tglKHMoi.Checked = Convert.ToBoolean(r["ApDungKhachHangMoi"]);

            cboMaNV.Text = r["MaNhanVien"].ToString();
            cboMaSP.Text = r["MaSanPham"].ToString();
            cboMaLSP.Text = r["MaLoaiSanPham"].ToString();
            cboMaTH.Text = r["MaThuongHieu"].ToString();

            if (r["NgayBatDau"] != DBNull.Value)
            {
                dtpNBD.Value = Convert.ToDateTime(r["NgayBatDau"]);
                dtpNBD.Checked = true;
            }
            else
            {
                dtpNBD.Checked = false;
            }

            if (r["NgayKetThuc"] != DBNull.Value)
            {
                dtpNKT.Value = Convert.ToDateTime(r["NgayKetThuc"]);
                dtpNKT.Checked = true;
            }
            else
            {
                dtpNKT.Checked = false;
            }

            txtSoLanSD.Text = r["SoLanSuDungToiDa"].ToString();

            tglTTKM.Checked = Convert.ToBoolean(r["TrangThaiKhuyenMai"]);
        }


        private void FrmCTKhuyenMai_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaNV();
            LoadComboBoxMaSP();
            LoadComboBoxMaLSP();
            LoadComboBoxMaTH();

            if (mode == "add")
            {
                txtMaKM.Enabled = false;

                if (string.IsNullOrWhiteSpace(txtMaKM.Text))
                    txtMaKM.Text = GenerateNewMaKM();
                LoadDetail();
            }

            if (mode == "edit")
            {
                txtMaKM.Enabled = false;
                LoadDetail();
            }
        }

        private void LoadComboBoxMaSP()
        {
            string query = "SELECT MaSanPham FROM tblSanPham";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaSP.DataSource = dt;
            cboMaSP.DisplayMember = "MaSanPham";
            cboMaSP.ValueMember = "MaSanPham";
            cboMaSP.SelectedIndex = -1; // Không chọn sẵn dòng nào
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

        private void btnThem_Click(object sender, EventArgs e)
        {
            // validate tối thiểu
            if (string.IsNullOrEmpty(txtTenKM.Text.Trim()))
            {
                MessageBox.Show("Tên khuyến mãi không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = $@"
                    INSERT INTO tblKhuyenMai
                    (MaKhuyenMai, TenKhuyenMai, GiaTri, MoTa, GiaTriDonHang,
                    LoaiDonHangApDung, HangThanhVienApDung, ApDungThangSinhNhat,
                    ApDungKhachHangMoi, MaSanPham, MaLoaiSanPham, MaThuongHieu,
                    NgayBatDau, NgayKetThuc, SoLanSuDungToiDa, TrangThaiKhuyenMai, MaNhanVien)
                    VALUES
                    ('{txtMaKM.Text}',
                     N'{txtTenKM.Text}',
                     {(string.IsNullOrEmpty(txtGiaTri.Text) ? "0" : txtGiaTri.Text)},
                     N'{txtMoTa.Text}',
                     {(string.IsNullOrEmpty(txtGTriDH.Text) ? "0" : txtGTriDH.Text)},
                     N'{cboDonAD.Text}',
                     N'{cboHangAD.Text}',
                     {(tglSinhNhat.Checked ? 1 : 0)},
                     {(tglKHMoi.Checked ? 1 : 0)},
                     {(string.IsNullOrEmpty(cboMaSP.Text) ? "NULL" : $"'{cboMaSP.Text}'")},
                     {(string.IsNullOrEmpty(cboMaLSP.Text) ? "NULL" : $"'{cboMaLSP.Text}'")},
                     {(string.IsNullOrEmpty(cboMaTH.Text) ? "NULL" : $"'{cboMaTH.Text}'")},
                     {(dtpNBD.Checked ? $"'{dtpNBD.Value:yyyy-MM-dd}'" : "NULL")},
                     {(dtpNKT.Checked ? $"'{dtpNKT.Value:yyyy-MM-dd}'" : "NULL")},
                     {(string.IsNullOrEmpty(txtSoLanSD.Text) ? "0" : txtSoLanSD.Text)},
                     {(tglTTKM.Checked ? 1 : 0)},
                     '{cboMaNV.Text}')";

                int rows = kn.ExecuteNonQuery(sql);
                if (rows > 0) MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Thêm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm thất bại! Dữ liệu không hợp lệ.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maKM))
            {
                MessageBox.Show("Không có khuyến mãi để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Xóa khuyến mãi này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int rows = kn.ExecuteNonQuery($"DELETE FROM tblKhuyenMai WHERE MaKhuyenMai = '{maKM}'");
                if (rows > 0) MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Xóa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.Close();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maKM))
            {
                MessageBox.Show("Không xác định khuyến mãi để sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = $@"
                    UPDATE tblKhuyenMai SET
                        TenKhuyenMai = N'{txtTenKM.Text}',
                        GiaTri = {(string.IsNullOrEmpty(txtGiaTri.Text) ? "0" : txtGiaTri.Text)},
                        MoTa = N'{txtMoTa.Text}',
                        GiaTriDonHang = {(string.IsNullOrEmpty(txtGTriDH.Text) ? "0" : txtGTriDH.Text)},
                        LoaiDonHangApDung = N'{cboDonAD.Text}',
                        HangThanhVienApDung = N'{cboHangAD.Text}',
                        ApDungThangSinhNhat = {(tglSinhNhat.Checked ? 1 : 0)},
                        ApDungKhachHangMoi = {(tglKHMoi.Checked ? 1 : 0)},
                        MaSanPham = {(string.IsNullOrEmpty(cboMaSP.Text) ? "NULL" : $"'{cboMaSP.Text}'")},
                        MaLoaiSanPham = {(string.IsNullOrEmpty(cboMaLSP.Text) ? "NULL" : $"'{cboMaLSP.Text}'")},
                        MaThuongHieu = {(string.IsNullOrEmpty(cboMaTH.Text) ? "NULL" : $"'{cboMaTH.Text}'")},
                        NgayBatDau = {(dtpNBD.Checked ? $"'{dtpNBD.Value:yyyy-MM-dd}'" : "NULL")},
                        NgayKetThuc = {(dtpNKT.Checked ? $"'{dtpNKT.Value:yyyy-MM-dd}'" : "NULL")},
                        SoLanSuDungToiDa = {(string.IsNullOrEmpty(txtSoLanSD.Text) ? "0" : txtSoLanSD.Text)},
                        TrangThaiKhuyenMai = {(tglTTKM.Checked ? 1 : 0)},
                        MaNhanVien = '{cboMaNV.Text}'
                    WHERE MaKhuyenMai = '{maKM}'";

                int rows = kn.ExecuteNonQuery(sql);
                if (rows > 0) MessageBox.Show("Sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Sửa thất bại! Dữ liệu không hợp lệ hoặc không thay đổi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sửa thất bại! Dữ liệu không hợp lệ.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
