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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace QuanLyBanHangWatsons
{
    public partial class FrmCTKhachHang : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maKH;       

        public FrmCTKhachHang(string makh, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maKH = makh;

            //if (mode == "add")
            //{
            //    txtMaKH.Enabled = false;   // CHẶN NHẬP
            //    txtMaKH.Text = "";
            //}


            //if (mode == "edit")
            //{
            //    LoadDetail();
            //    btnThem.Enabled = false;
            //    txtMaKH.Enabled = false;
            //}
            //else
            //{
            //    btnSua.Enabled = false;
            //    btnXoa.Enabled = false;
            //}
        }

        private void FrmCTKhachHang_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaNV();

            if (mode == "add")
            {
                txtMaKH.Enabled = false;
                txtMaKH.Text = GenerateNewMaKH();
                maKH = txtMaKH.Text;

                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
            else if (mode == "edit")
            {
                txtMaKH.Enabled = false;
                btnThem.Enabled = false;
                LoadDetail();
            }

            LoadLichSuSDKhuyenMai();
            LoadLichSuTichDiem();

        }

        private string GenerateNewMaKH()
        {
            string sql = "SELECT TOP 1 MaKhachHang FROM tblKhachHang ORDER BY MaKhachHang DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value) return "KH0001";

            string last = obj.ToString().Trim();

            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);
            string numPart = last.Substring(i + 1);

            int num = int.Parse(numPart) + 1;
            return prefix + num.ToString().PadLeft(numPart.Length, '0');
        }

        private void LoadDetail()
        {
            string sql = "SELECT * FROM tblKhachHang WHERE MaKhachHang = @makh";
            SqlParameter[] p =
            {
                new SqlParameter("@makh", maKH)
            };

            DataTable dt = kn.ExecuteQueryWithParams(sql, p);
            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            txtMaKH.Text = r["MaKhachHang"].ToString();
            txtHoKH.Text = r["HoKhachHang"].ToString();
            txtTenKH.Text = r["TenKhachHang"].ToString();
            cboLoaiKH.Text = r["LoaiKhachHang"].ToString();

            if (r["NgaySinh"] != DBNull.Value)
                dtpNS.Value = Convert.ToDateTime(r["NgaySinh"]);

            cboGT.Text = r["GioiTinh"].ToString();
            txtSDT.Text = r["DienThoai"].ToString();
            txtDiaChi.Text = r["DiaChi"].ToString();
            txtEmail.Text = r["Email"].ToString();

            if (r["NgayDangKy"] != DBNull.Value)
                dtpNDK.Value = Convert.ToDateTime(r["NgayDangKy"]);

            txtDiemTichLuy.Text = r["DiemTichLuy"].ToString();
            cboHangTV.Text = r["HangThanhVien"].ToString();
            cboMaNV.Text = r["MaNhanVien"].ToString();

            if (r["NgayCapNhat"] != DBNull.Value)
                dtpNCN.Value = Convert.ToDateTime(r["NgayCapNhat"]);
        }

        private void LoadComboBoxMaNV()
        {
            DataTable dt = kn.ExecuteQuery("SELECT MaNhanVien FROM tblNhanVien");
            cboMaNV.DataSource = dt;
            cboMaNV.DisplayMember = "MaNhanVien";
            cboMaNV.ValueMember = "MaNhanVien";
            cboMaNV.SelectedIndex = -1;
        }


        private void LoadLichSuSDKhuyenMai()
        {
            string sql = @"
                SELECT km.MaKhuyenMai, mk.SoLanDaSuDung
                FROM tblMaKMCuaKH mk
                JOIN tblKhuyenMai km ON mk.MaKhuyenMai = km.MaKhuyenMai
                WHERE mk.MaKhachHang = @makh";

            SqlParameter[] p = {
                new SqlParameter("@makh", maKH)
            };

            DataTable dt = kn.ExecuteQueryWithParams(sql, p);
            dgvLSKhuyenMai.Rows.Clear();

            foreach (DataRow r in dt.Rows)
            {
                dgvLSKhuyenMai.Rows.Add(
                    r["MaKhuyenMai"].ToString(),
                    r["SoLanDaSuDung"].ToString()
                );
            }
        }

        private void LoadLichSuTichDiem()
        {
            string sql = @"
                SELECT MaDonHang, DiemDaSuDung, DiemTichLuy, NgayHetHan
                FROM tblChiTietDiemTichLuy
                WHERE MaKhachHang = @makh
                ORDER BY NgayHetHan ASC";

            SqlParameter[] p = {
                new SqlParameter("@makh", maKH)
            };

            DataTable dt = kn.ExecuteQueryWithParams(sql, p);

            dgvLSTichDiem.Rows.Clear();

            foreach (DataRow r in dt.Rows)
            {
                string ngay = "";
                if (r["NgayHetHan"] != DBNull.Value)
                    ngay = Convert.ToDateTime(r["NgayHetHan"]).ToString("yyyy-MM-dd HH:mm:ss");

                dgvLSTichDiem.Rows.Add(
                    r["MaDonHang"].ToString(),
                    r["DiemDaSuDung"].ToString(),
                    r["DiemTichLuy"].ToString(),
                    ngay
                );
            }
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            if (mode != "add") return;

            if (string.IsNullOrWhiteSpace(cboMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn mã nhân viên!");
                return;
            }

            bool vangLai = cboLoaiKH.Text == "Vãng lai";

            string sql;

            SqlParameter[] p;

            if (vangLai)
            {
                // KH VÃNG LAI: chỉ lưu 4 trường
                sql = @"
            INSERT INTO tblKhachHang
            (MaKhachHang, LoaiKhachHang, MaNhanVien, NgayCapNhat)
            VALUES
            (@MaKH, @LoaiKH, @MaNV, @NgayCN)";

                p = new SqlParameter[]
                {
            new SqlParameter("@MaKH", txtMaKH.Text),
            new SqlParameter("@LoaiKH", cboLoaiKH.Text),
            new SqlParameter("@MaNV", cboMaNV.Text),
            new SqlParameter("@NgayCN", dtpNCN.Value)
                };
            }
            else
            {
                // KH THÀNH VIÊN: full dữ liệu
                sql = @"
            INSERT INTO tblKhachHang
            (MaKhachHang, HoKhachHang, TenKhachHang, LoaiKhachHang,
             NgaySinh, GioiTinh, DienThoai, DiaChi, Email,
             NgayDangKy, DiemTichLuy, HangThanhVien, MaNhanVien, NgayCapNhat)
            VALUES
            (@MaKH, @HoKH, @TenKH, @LoaiKH, @NgaySinh, @GioiTinh, @SDT,
             @DiaChi, @Email, @NgayDK, @Diem, @Hang, @MaNV, @NgayCN)";

                p = new SqlParameter[]
                {
            new SqlParameter("@MaKH", txtMaKH.Text),
            new SqlParameter("@HoKH", txtHoKH.Text),
            new SqlParameter("@TenKH", txtTenKH.Text),
            new SqlParameter("@LoaiKH", cboLoaiKH.Text),
            new SqlParameter("@NgaySinh", dtpNS.Value),
            new SqlParameter("@GioiTinh", cboGT.Text),
            new SqlParameter("@SDT", txtSDT.Text),
            new SqlParameter("@DiaChi", txtDiaChi.Text),
            new SqlParameter("@Email", txtEmail.Text),
            new SqlParameter("@NgayDK", dtpNDK.Value),
            new SqlParameter("@Diem", txtDiemTichLuy.Text),
            new SqlParameter("@Hang", cboHangTV.Text),
            new SqlParameter("@MaNV", cboMaNV.Text),
            new SqlParameter("@NgayCN", dtpNCN.Value)
                };
            }

            int rows = kn.ExecuteNonQueryWithParams(sql, p);

            if (rows > 0)
                {
                    MessageBox.Show("Thêm khách hàng thành công!", "Thông báo",
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

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (mode != "edit") return;

            if (MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            string sql = "DELETE FROM tblKhachHang WHERE MaKhachHang = @MaKH";
            SqlParameter[] p = { new SqlParameter("@MaKH", maKH) };

            int rows = kn.ExecuteNonQueryWithParams(sql, p);

            if (rows > 0)
            {
                MessageBox.Show("Xóa thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {       MessageBox.Show("Xóa thất bại!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
               
                return;
            }           
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (mode != "edit") return;

            bool vangLai = cboLoaiKH.Text == "Vãng lai";

            string sql;
            SqlParameter[] p;

            if (vangLai)
            {
                // KH VÃNG LAI: Cập nhật 4 trường
                sql = @"
            UPDATE tblKhachHang SET
                LoaiKhachHang = @LoaiKH,
                MaNhanVien = @MaNV,
                NgayCapNhat = @NgayCN
            WHERE MaKhachHang = @MaKH";

                p = new SqlParameter[]
                {
            new SqlParameter("@LoaiKH", cboLoaiKH.Text),
            new SqlParameter("@MaNV", cboMaNV.Text),
            new SqlParameter("@NgayCN", dtpNCN.Value),
            new SqlParameter("@MaKH", maKH)
                };
            }
            else
            {
                // THÀNH VIÊN: cập nhật full
                sql = @"
            UPDATE tblKhachHang SET
                HoKhachHang = @Ho,
                TenKhachHang = @Ten,
                LoaiKhachHang = @Loai,
                NgaySinh = @NS,
                GioiTinh = @GT,
                DienThoai = @SDT,
                DiaChi = @DiaChi,
                Email = @Email,
                NgayDangKy = @NgayDK,
                DiemTichLuy = @Diem,
                HangThanhVien = @Hang,
                MaNhanVien = @NV,
                NgayCapNhat = @NCN
            WHERE MaKhachHang = @MaKH";

                p = new SqlParameter[]
                {
            new SqlParameter("@Ho", txtHoKH.Text),
            new SqlParameter("@Ten", txtTenKH.Text),
            new SqlParameter("@Loai", cboLoaiKH.Text),
            new SqlParameter("@NS", dtpNS.Value),
            new SqlParameter("@GT", cboGT.Text),
            new SqlParameter("@SDT", txtSDT.Text),
            new SqlParameter("@DiaChi", txtDiaChi.Text),
            new SqlParameter("@Email", txtEmail.Text),
            new SqlParameter("@NgayDK", dtpNDK.Value),
            new SqlParameter("@Diem", txtDiemTichLuy.Text),
            new SqlParameter("@Hang", cboHangTV.Text),
            new SqlParameter("@NV", cboMaNV.Text),
            new SqlParameter("@NCN", dtpNCN.Value),
            new SqlParameter("@MaKH", maKH)
                };
            }

            int rows = kn.ExecuteNonQueryWithParams(sql, p);

            if (rows > 0)
                {
                    MessageBox.Show("Sửa thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // chỉ đóng form nếu sửa thành công
                }
                else
                {
                    MessageBox.Show("Sửa thất bại! Dữ liệu không hợp lệ, vui lòng kiểm tra lại.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // KHÔNG đóng form
                }
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maKH))
            {
                MessageBox.Show("Không xác định được mã khách hàng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tìm tài khoản KH theo mã khách hàng
            string sql = @"
        SELECT MaTaiKhoanKH 
        FROM tblTaiKhoanKH 
        WHERE MaKhachHang = @makh";

            SqlParameter[] p =
            {
        new SqlParameter("@makh", maKH)
    };

            DataTable dt = kn.ExecuteQueryWithParams(sql, p);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Khách hàng này chưa có tài khoản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maTK = dt.Rows[0]["MaTaiKhoanKH"].ToString();

            // Mở form chi tiết tài khoản KH
            FrmCTTKKhachHang frm = new FrmCTTKKhachHang(maTK, "edit");

            // Nếu đang dùng MDI
            frm.MdiParent = this.MdiParent;

            frm.Show();
        }
    }
}
