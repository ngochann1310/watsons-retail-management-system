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
    public partial class FrmCTHoaDon : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode = "";     // add | edit
        string maHD = "";     // mã hóa đơn

       
        string tmpMaTT = "";
        string tmpMaNV = "";

        public FrmCTHoaDon(string _maHD = "", string maTT = "", string maNV = "", string _mode = "add")
        {
            InitializeComponent();
            maHD = _maHD;   // ← MUST HAVE
            mode = _mode;

            tmpMaTT = maTT;
            tmpMaNV = maNV;

            txtTongTien.ReadOnly = true;
            txtDaTT.ReadOnly = true;
            txtSoDu.ReadOnly = true;

            if (mode == "add") txtMaHD.Enabled = false;
            if (mode == "edit")
            {
                txtMaHD.Enabled = false;
                btnThem.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        bool isLoading = true;
        private void FrmCTHoaDon_Load(object sender, EventArgs e)
        {
            LoadComboMaNV();
            LoadComboMaTT();

            if (mode == "add")
            {
                txtMaHD.Text = GenerateNewMaHD();
            }
            else
            {
                LoadDetail();
            }

            if (!string.IsNullOrWhiteSpace(tmpMaTT))
            {
                cboMaTT.SelectedValue = tmpMaTT;
                LoadInfoByMaTT();   
            }

            if (!string.IsNullOrWhiteSpace(tmpMaNV))
                cboMaNV.SelectedValue = tmpMaNV;

            isLoading = false;
            cboMaTT.SelectedIndexChanged += cboMaTT_SelectedIndexChanged;
        }

        private void cboMaTT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
            LoadInfoByMaTT();
        }

        private void LoadComboMaTT()
        {
            string sql = "SELECT MaThanhToan FROM tblThanhToan";
            DataTable dt = kn.ExecuteQuery(sql);
            cboMaTT.DataSource = dt;
            cboMaTT.ValueMember = "MaThanhToan";
            cboMaTT.DisplayMember = "MaThanhToan";
            cboMaTT.SelectedIndex = -1;
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



        private string GenerateNewMaHD()
        {
            string sql = "SELECT TOP 1 MaHoaDon FROM tblHoaDon ORDER BY MaHoaDon DESC";
            var obj = kn.ExecuteScalar(sql);
            if (obj == null || obj == DBNull.Value) return "HD0001";

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
            SELECT * FROM tblHoaDon WHERE MaHoaDon = @MaHD
        ";

            SqlParameter[] pr =
            {
            new SqlParameter("@MaHD", maHD)
        };

            DataTable dt = kn.ExecuteQueryWithParams(sql, pr);

            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            txtMaHD.Text = r["MaHoaDon"].ToString();

            if (r["MaThanhToan"] != DBNull.Value) cboMaTT.SelectedValue = r["MaThanhToan"].ToString();
            if (r["MaNhanVien"] != DBNull.Value) cboMaNV.SelectedValue = r["MaNhanVien"].ToString();
            
            if (r["NgayXuatHoaDon"] != DBNull.Value)
            {
                dtpNXHD.Value = Convert.ToDateTime(r["NgayXuatHoaDon"]);
            }
            else
            {
                dtpNXHD.Value = DateTime.Now;
            }
            txtTongTien.Text = r["TongTien"]?.ToString() ?? "";
            txtDaTT.Text = r["SoTienDaThanhToan"]?.ToString() ?? "";
            txtSoDu.Text = r["SoDuConLai"]?.ToString() ?? "";
        }

        private void LoadInfoByMaTT()
        {
            if (cboMaTT.SelectedValue == null) return;

            string maTT = cboMaTT.SelectedValue.ToString();

            string sql = @"
        SELECT 
            tt.MaThanhToan,
            tt.NgayThanhToan,
            tt.TrangThaiThanhToan,
            tt.TongTien AS TongTienThanhToan,
            dh.ThanhTien AS TongTienDonHang
        FROM tblThanhToan tt
        JOIN tblDonHang dh ON tt.MaDonHang = dh.MaDonHang
        WHERE tt.MaThanhToan = @MaTT
    ";

            SqlParameter[] pr =
            {
        new SqlParameter("@MaTT", maTT)
    };

            DataTable dt = kn.ExecuteQueryWithParams(sql, pr);
            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            // Ngày xuất HĐ = ngày thanh toán
            dtpNXHD.Value = Convert.ToDateTime(r["NgayThanhToan"]);

            // Tổng tiền hóa đơn = Tổng tiền đơn hàng
            txtTongTien.Text = r["TongTienDonHang"].ToString();

            // Số tiền đã thanh toán
            int trangThai = Convert.ToInt32(r["TrangThaiThanhToan"]);
            if (trangThai == 1)
                txtDaTT.Text = r["TongTienThanhToan"].ToString();
            else
                txtDaTT.Text = "0";

            // Số dư = ĐH – đã trả
            decimal tongDH = Convert.ToDecimal(r["TongTienDonHang"]);
            decimal daTT = trangThai == 1 ? Convert.ToDecimal(r["TongTienThanhToan"]) : 0;

            txtSoDu.Text = (tongDH - daTT).ToString();
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cboMaTT.SelectedIndex == -1 || cboMaNV.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Thanh toán, Nhân viên!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"
            INSERT INTO tblHoaDon
            (
                MaHoaDon,
                MaThanhToan,
                MaNhanVien,
                NgayXuatHoaDon,
                TongTien,
                SoTienDaThanhToan,
                SoDuConLai
            )
            VALUES
            (
                @MaHD,
                @MaTT,
                @MaNV,
                @Ngay,
                @TongTien,
                @DaTT,
                @SoDu
            )";

            SqlParameter[] param =
            {
                new SqlParameter("@MaHD", txtMaHD.Text),
                new SqlParameter("@MaTT", cboMaTT.SelectedValue),
                new SqlParameter("@MaNV", cboMaNV.SelectedValue),
                new SqlParameter("@Ngay", dtpNXHD.Value),
                new SqlParameter("@TongTien", decimal.Parse(txtTongTien.Text)),
                new SqlParameter("@DaTT", decimal.Parse(txtDaTT.Text)),
                new SqlParameter("@SoDu", decimal.Parse(txtSoDu.Text))
            };

            try
            {
                int result = kn.ExecuteNonQueryWithParams(query, param);
                if (result > 0)
                {
                    MessageBox.Show("Thêm hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                    MessageBox.Show("Thêm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHD.Text))
            {
                MessageBox.Show("Chọn hóa đơn cần xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            string query = "DELETE FROM tblHoaDon WHERE MaHoaDon = @MaHD";
            SqlParameter[] param = {
        new SqlParameter("@MaHoaDon", txtMaHD.Text)
    };

            int result = kn.ExecuteNonQueryWithParams(query, param);

            if (result > 0)
            {
                MessageBox.Show("Xóa hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHD.Text))
            {
                MessageBox.Show("Chọn hóa đơn cần sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"
            UPDATE tblHoaDon
            SET MaThanhToan = @MaTT,
                MaNhanVien = @MaNV,
                NgayXuatHoaDon = @Ngay
            WHERE MaHoaDon = @MaHD
            ";

            SqlParameter[] param = {
            new SqlParameter("@MaHD", txtMaHD.Text),
            new SqlParameter("@MaTT", cboMaTT.SelectedValue),
            new SqlParameter("@MaNV", cboMaNV.SelectedValue),
            new SqlParameter("@Ngay", dtpNXHD.Value),
            };

            try
            {
                int result = kn.ExecuteNonQueryWithParams(query, param);
                if (result > 0)
                {
                    MessageBox.Show("Sửa hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHD.Text))
            {
                MessageBox.Show("Mã hóa đơn trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FrmInHoaDon f = new FrmInHoaDon(txtMaHD.Text); // truyền mã HD
            f.ShowDialog();
        }
    }
}
