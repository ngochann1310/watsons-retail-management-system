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
    public partial class FrmCTThanhToan : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode = "";     // add | edit
        string maTT = "";     // mã đơn

        string tmpMaDH = "";
        string tmpMaKH = "";
        string tmpMaNV = "";
        string tmpTongTien = "";


        public FrmCTThanhToan(string _maTT = "", string maDH = "", string maKH = "", string maNV = "", string tongTien = "", string _mode = "add")
        {
            InitializeComponent();
            maTT = _maTT;   // ← MUST HAVE
            mode = _mode;

            tmpMaDH = maDH;
            tmpMaKH = maKH;
            tmpMaNV = maNV;
            tmpTongTien = tongTien;

            if (mode == "add") txtMaTT.Enabled = false;
            if (mode == "edit")
            {
                txtMaTT.Enabled = false;
                btnThem.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void FrmCTThanhToan_Load(object sender, EventArgs e)
        {
            LoadComboMaNV();
            LoadComboMaKH();
            LoadComboMaDH();

            // set SelectedValue sau khi combo bind xong
            if (!string.IsNullOrWhiteSpace(tmpMaKH)) cboMaKH.SelectedValue = tmpMaKH;
            if (!string.IsNullOrWhiteSpace(tmpMaNV)) cboMaNV.SelectedValue = tmpMaNV;
            if (!string.IsNullOrWhiteSpace(tmpMaDH)) cboMaDH.SelectedValue = tmpMaDH;
            if (!string.IsNullOrWhiteSpace(tmpTongTien))
                txtTongTien.Text = tmpTongTien;

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaTT.Text))
                    txtMaTT.Text = GenerateNewMaTT();
            }

            if (mode == "edit")
            {
                LoadDetail();
            }
        }

        private void LoadComboMaKH()
        {
            string sql = "SELECT MaKhachHang FROM tblKhachHang";
            DataTable dt = kn.ExecuteQuery(sql);
            cboMaKH.DataSource = dt;
            cboMaKH.ValueMember = "MaKhachHang";
            cboMaKH.DisplayMember = "MaKhachHang";
            cboMaKH.SelectedIndex = -1;     
        }

        private void LoadComboMaNV()
        {
            string sql = "SELECT MaNhanVien FROM tblNhanVien";
            DataTable dt = kn.ExecuteQuery(sql);
            cboMaNV.DataSource = dt;
            cboMaNV.ValueMember = "MaNhanVien";
            cboMaNV.DisplayMember = "MaNhanVien"; 
            cboMaNV.SelectedIndex = -1;
        }

        private void LoadComboMaDH()
        {
            string sql = "SELECT MaDonHang FROM tblDonHang";
            DataTable dt = kn.ExecuteQuery(sql);
            cboMaDH.DataSource = dt;
            cboMaDH.ValueMember = "MaDonHang";
            cboMaDH.DisplayMember = "MaDonHang";
            cboMaDH.SelectedIndex = -1;

            // Nếu FrmDonHang_TaoDon truyền maDH thì set combo luôn
            if (!string.IsNullOrWhiteSpace(tmpMaDH))
            {
                cboMaDH.SelectedValue = tmpMaDH;
            }
        }

        private string GenerateNewMaTT()
        {
            string sql = "SELECT TOP 1 MaThanhToan FROM tblThanhToan ORDER BY MaThanhToan DESC";
            var obj = kn.ExecuteScalar(sql);
            if (obj == null || obj == DBNull.Value) return "TT0001";

            string last = obj.ToString().Trim();
            // lấy phần số cuối liên tiếp từ cuối chuỗi
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;
            string prefix = last.Substring(0, i + 1);
            string numPart = last.Substring(i + 1);
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

        private void LoadDetail()
        {
            string sql = @"
            SELECT * FROM tblThanhToan WHERE MaThanhToan = @MaTT
        ";

            SqlParameter[] pr =
            {
            new SqlParameter("@MaTT", maTT)
        };

            DataTable dt = kn.ExecuteQueryWithParams(sql, pr);

            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            txtMaTT.Text = r["MaThanhToan"].ToString();

            if (r["MaKhachHang"] != DBNull.Value) cboMaKH.SelectedValue = r["MaKhachHang"].ToString();
            if (r["MaNhanVien"] != DBNull.Value) cboMaNV.SelectedValue = r["MaNhanVien"].ToString();
            if (r["MaDonHang"] != DBNull.Value) cboMaDH.SelectedValue = r["MaDonHang"].ToString();
            cboPTTT.Text = r["PhuongThucThanhToan"]?.ToString() ?? "";
            if (r["NgayThanhToan"] != DBNull.Value)
            {
                dtpNTT.Value = Convert.ToDateTime(r["NgayThanhToan"]);
            }
            else
            {
                dtpNTT.Value = DateTime.Now;
            }
            tglTrangThai.Checked = Convert.ToBoolean(r["TrangThaiThanhToan"]);
            //txtTongTien.Text = r["TongTien"]?.ToString() ?? "";
            txtTongTien.ReadOnly = true;
        }        

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            if (cboMaDH.SelectedIndex == -1 || cboMaKH.SelectedIndex == -1 || cboMaNV.SelectedIndex == -1 || string.IsNullOrWhiteSpace(cboPTTT.Text))
            {
                MessageBox.Show("Vui lòng chọn Đơn hàng, Khách hàng, Nhân viên và phương thức thanh toán!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"
            INSERT INTO tblThanhToan
            (
                MaThanhToan,
                MaKhachHang,
                MaDonHang,
                MaNhanVien,
                PhuongThucThanhToan,
                NgayThanhToan,
                TongTien,
                TrangThaiThanhToan
            )
            VALUES
            (
                @MaTT,
                @MaKH,
                @MaDH,
                @MaNV,
                @PTTT,
                @NgayTT,
                (SELECT ThanhTien FROM tblDonHang WHERE MaDonHang = @MaDH), -- lấy từ đơn hàng
                @TTTT
            )";

            SqlParameter[] param = {
            new SqlParameter("@MaTT", txtMaTT.Text),
            new SqlParameter("@MaKH", cboMaKH.SelectedValue),
            new SqlParameter("@MaDH", cboMaDH.SelectedValue),
            new SqlParameter("@MaNV", cboMaNV.SelectedValue),
            new SqlParameter("@PTTT", cboPTTT.Text),
            new SqlParameter("@NgayTT", dtpNTT.Value),
            new SqlParameter("@TTTT", tglTrangThai.Checked)
            };

            try
            {
                int result = kn.ExecuteNonQueryWithParams(query, param);
                if (result > 0)
                {
                    CapNhatTrangThaiDonHang_TaiCuaHang();
                    MessageBox.Show("Thêm thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                    MessageBox.Show("Thêm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Thêm
            string sqlReload = @"
            SELECT TongTien 
            FROM tblThanhToan 
            WHERE MaThanhToan = @MaTT";

            SqlParameter[] pr =
            {
                new SqlParameter("@MaTT", txtMaTT.Text)
            };

            DataTable dt = kn.ExecuteQueryWithParams(sqlReload, pr);
            if (dt.Rows.Count > 0)
            {
                txtTongTien.Text = dt.Rows[0]["TongTien"].ToString();
            }

        }

        private void btnXoa_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaTT.Text))
            {
                MessageBox.Show("Chọn thanh toán cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa thanh toán này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            string query = "DELETE FROM tblThanhToan WHERE MaThanhToan = @MaTT";
            SqlParameter[] param = {
        new SqlParameter("@MaTT", txtMaTT.Text)
    };

            int result = kn.ExecuteNonQueryWithParams(query, param);

            if (result > 0)
            {
                MessageBox.Show("Xóa thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaTT.Text))
            {
                MessageBox.Show("Chọn thanh toán cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"
            UPDATE tblThanhToan
            SET MaKhachHang = @MaKH,
                MaDonHang = @MaDH,
                MaNhanVien = @MaNV,
                PhuongThucThanhToan = @PTTT,
                TrangThaiThanhToan = @TTTT
            WHERE MaThanhToan = @MaTT
            ";

            SqlParameter[] param = {
            new SqlParameter("@MaTT", txtMaTT.Text),
            new SqlParameter("@MaKH", cboMaKH.SelectedValue),
            new SqlParameter("@MaDH", cboMaDH.SelectedValue),
            new SqlParameter("@MaNV", cboMaNV.SelectedValue),
            new SqlParameter("@PTTT", cboPTTT.Text),
            new SqlParameter("@TTTT", tglTrangThai.Checked)
        };

            try
            {
                int result = kn.ExecuteNonQueryWithParams(query, param);
                if (result > 0)
                {
                    CapNhatTrangThaiDonHang_TaiCuaHang();
                    MessageBox.Show("Sửa thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sửa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                // Trigger sẽ RAISERROR nếu tổng tiền bị sửa, bắt lỗi ở đây
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaTT.Text))
            {
                MessageBox.Show("Chưa có mã thanh toán. Lưu thanh toán trước khi tạo hóa đơn!", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maTT = txtMaTT.Text.Trim();

            // 1. Kiểm tra MaTT đã có hóa đơn chưa
            string sql = "SELECT MaHoaDon FROM tblHoaDon WHERE MaThanhToan = @MaTT";
            SqlParameter[] pr = {
        new SqlParameter("@MaTT", maTT)
    };

            DataTable dt = kn.ExecuteQueryWithParams(sql, pr);

            string maHD = "";
            string mode = "add";

            if (dt.Rows.Count > 0)
            {
                // ĐÃ CÓ HÓA ĐƠN → EDIT
                maHD = dt.Rows[0]["MaHoaDon"].ToString();
                mode = "edit";
            }
            else
            {
                // CHƯA CÓ HÓA ĐƠN → ADD
                maHD = "";  // để Form tự sinh
            }

            // 2. Mở form đúng mode
            FrmCTHoaDon f = new FrmCTHoaDon(
                maHD,              // nếu edit → truyền mã, nếu add → ""
                maTT,              // luôn truyền đúng mã thanh toán
                cboMaNV.SelectedValue?.ToString(),
                mode
            );

            f.ShowDialog();
        }        

        private void tglTrangThai_CheckedChanged(object sender, EventArgs e)
        {
            if (mode != "edit") return;   // chỉ xử lý khi đã có thanh toán

            //CapNhatTrangThaiDonHangTheoThanhToan();
        }

        private void CapNhatTrangThaiDonHang_TaiCuaHang()
        {
            //    if (cboMaDH.SelectedValue == null) return;
            //    //if (!tglTrangThai.Checked) return; // chưa thanh toán thì không update

            //    string maDH = cboMaDH.SelectedValue.ToString();

            //    // Lấy loại đơn hàng
            //    string sqlLoai = @"
            //    SELECT LoaiDonHang 
            //    FROM tblDonHang 
            //    WHERE MaDonHang = @MaDH";

            //    object loai = kn.ExecuteScalarWithParams(
            //        sqlLoai,
            //        new SqlParameter[]
            //        {
            //    new SqlParameter("@MaDH", maDH)
            //        }
            //    );

            //    if (loai == null || loai == DBNull.Value) return;

            //    string trangThaiMoi = "";

            //    // PHÂN NHÁNH THEO LOẠI ĐƠN
            //    if (loai.ToString() == "Tại cửa hàng")
            //    {
            //        trangThaiMoi = "Giao hàng thành công";
            //    }
            //    else if (loai.ToString() == "Trực tuyến")
            //    {
            //        trangThaiMoi = "Chờ lấy hàng";
            //    }
            //    else
            //    {
            //        return; // loại khác thì bỏ
            //    }

            //    // Update trạng thái đơn hàng
            //    string sqlUpdate = @"
            //    UPDATE tblDonHang
            //    SET TrangThaiDonHang = @TrangThai
            //    WHERE MaDonHang = @MaDH";

            //    kn.ExecuteNonQueryWithParams(
            //        sqlUpdate,
            //        new SqlParameter[]
            //        {
            //    new SqlParameter("@TrangThai", trangThaiMoi),
            //    new SqlParameter("@MaDH", maDH)
            //        }
            //    );

            if (cboMaDH.SelectedValue == null) return;

            string maDH = cboMaDH.SelectedValue.ToString();

            // Lấy loại đơn hàng
            object loaiObj = kn.ExecuteScalarWithParams(
                "SELECT LoaiDonHang FROM tblDonHang WHERE MaDonHang = @MaDH",
                new SqlParameter[] { new SqlParameter("@MaDH", maDH) }
            );

            if (loaiObj == null) return;
            string loai = loaiObj.ToString();

            // 🔴 TRƯỜNG HỢP 1: TẠI CỬA HÀNG → BẮT BUỘC THANH TOÁN
            if (loai == "Tại cửa hàng")
            {
                object tt = kn.ExecuteScalarWithParams(
                    "SELECT TrangThaiThanhToan FROM tblThanhToan WHERE MaDonHang = @MaDH",
                    new SqlParameter[] { new SqlParameter("@MaDH", maDH) }
                );

                if (tt == null || tt == DBNull.Value || Convert.ToInt32(tt) != 1)
                    return; // ❌ chưa thanh toán thì KHÔNG update

                UpdateTrangThaiDonHang(maDH, "Giao hàng thành công");
            }

            // 🟢 TRƯỜNG HỢP 2: TRỰC TUYẾN → KHÔNG CẦN THANH TOÁN NGAY
            else if (loai == "Trực tuyến")
            {
                UpdateTrangThaiDonHang(maDH, "Chờ lấy hàng");
            }
        }

        private void UpdateTrangThaiDonHang(string maDH, string trangThai)
        {
            if (string.IsNullOrWhiteSpace(maDH) || string.IsNullOrWhiteSpace(trangThai))
                return;

            string sql = @"
    UPDATE tblDonHang
    SET TrangThaiDonHang = @TrangThai
    WHERE MaDonHang = @MaDH";

            kn.ExecuteNonQueryWithParams(
                sql,
                new SqlParameter[]
                {
            new SqlParameter("@TrangThai", trangThai),
            new SqlParameter("@MaDH", maDH)
                }
            );
        }



    }
}
