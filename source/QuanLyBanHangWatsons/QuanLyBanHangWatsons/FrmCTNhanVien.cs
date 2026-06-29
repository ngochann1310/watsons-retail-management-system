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
    public partial class FrmCTNhanVien : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maNV;

        public FrmCTNhanVien(string manv, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maNV = manv;

            if (mode == "add")
            {
                txtMaNV.Enabled = false;   // CHẶN NHẬP
                txtMaNV.Text = "";
            }


            if (mode == "edit")
            {
                LoadDetail();
                btnThem.Enabled = false;
                txtMaNV.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void LoadDetail()
        {
            string sql = $"SELECT * FROM tblNhanVien WHERE MaNhanVien = '{maNV}'";
            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            txtMaNV.Text = r["MaNhanVien"].ToString();
            txtHoNV.Text = r["HoNhanVien"].ToString();
            txtTenNV.Text = r["TenNhanVien"].ToString();
            cboGT.Text = r["GioiTinh"].ToString();
            dtpNS.Value = Convert.ToDateTime(r["NgaySinh"]);
            txtSDT.Text = r["DienThoai"].ToString();
            dptNVL.Value = Convert.ToDateTime(r["NgayVaoLam"]);
            cboCVu.Text = r["ChucVu"].ToString();
            txtEmail.Text = r["Email"].ToString();
            cboMaCN.Text = r["MaChiNhanh"].ToString();
            dtpNCN.Value = Convert.ToDateTime(r["NgayCapNhat"]);          
        }

        private void FrmCTNhanVien_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaCN();

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaNV.Text))
                {
                    maNV = GenerateNewMaNV();     // ✔️ gán cho biến dùng INSERT
                    txtMaNV.Text = maNV;          // ✔️ hiển thị lên textbox
                }
            }

            if (mode == "edit")
            {
                LoadDetail();
            }
        }


        private string GenerateNewMaNV()
        {
            string sql = "SELECT TOP 1 MaNhanVien FROM tblNhanVien ORDER BY MaNhanVien DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value)
                return "NV0001";

            string last = obj.ToString().Trim();

            // lấy phần số cuối liên tiếp từ cuối chuỗi
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // NV
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


        private void LoadComboBoxMaCN()
        {
            string query = "SELECT MaChiNhanh  FROM tblChiNhanh";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaCN.DataSource = dt;
            cboMaCN.DisplayMember = "MaChiNhanh";
            cboMaCN.ValueMember = "MaChiNhanh";
            cboMaCN.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (mode != "add") return;

            // Validate
            if (string.IsNullOrEmpty(txtHoNV.Text.Trim()))
            {
                MessageBox.Show("Họ nhân viên không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtTenNV.Text.Trim()))
            {
                MessageBox.Show("Tên nhân viên không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cboCVu.Text))
            {
                MessageBox.Show("Vui lòng chọn chức vụ!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cboMaCN.Text))
            {
                MessageBox.Show("Vui lòng chọn chi nhánh!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {               
                string sql = $@"
                INSERT INTO tblNhanVien
                (MaNhanVien, HoNhanVien, TenNhanVien, MaChiNhanh, NgaySinh, 
                 GioiTinh, DienThoai, ChucVu, NgayVaoLam, Email,NgayCapNhat)
                VALUES
                ('{maNV}',
                 N'{txtHoNV.Text}',
                N'{txtTenNV.Text}',
                 '{cboMaCN.Text}',
                '{dtpNS.Value:yyyy-MM-dd HH:mm:ss}',
                N'{cboGT.Text}',
                 N'{txtSDT.Text}',
                N'{cboCVu.Text}',
                '{dptNVL.Value:yyyy-MM-dd HH:mm:ss}',
                N'{txtEmail.Text}',
                 '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}')";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Thêm nhân viên thành công!", "Thông báo",
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
            if (mode != "edit" || string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Không xác định nhân viên cần xóa.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa nhân viên này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                int rows = kn.ExecuteNonQuery(
                    $"DELETE FROM tblNhanVien WHERE MaNhanVien = '{maNV}'"
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
            if (mode != "edit" || string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Không xác định nhân viên cần sửa.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {                
                string sql = $@"
                UPDATE tblNhanVien SET
                    HoNhanVien = N'{txtHoNV.Text}',
                    TenNhanVien = N'{txtTenNV.Text}',
                    MaChiNhanh = '{cboMaCN.Text}',
                    NgaySinh = '{dtpNS.Value:yyyy-MM-dd HH:mm:ss}',
                    GioiTinh = N'{cboGT.Text}',
                    DienThoai = N'{txtSDT.Text}',
                    ChucVu = N'{cboCVu.Text}',
                    NgayVaoLam = '{dptNVL.Value:yyyy-MM-dd HH:mm:ss}',
                    Email = N'{txtEmail.Text}',
                    NgayCapNhat = '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}'
                WHERE MaNhanVien = '{maNV}'";

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

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Không xác định được mã nhân viên!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tìm tài khoản theo mã nhân viên
            string sql = @"
        SELECT MaTaiKhoanNV
        FROM tblTaiKhoanNV
        WHERE MaNhanVien = @manv";

            SqlParameter[] p =
            {
        new SqlParameter("@manv", maNV)
    };

            DataTable dt = kn.ExecuteQueryWithParams(sql, p);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Nhân viên này chưa có tài khoản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maTK = dt.Rows[0]["MaTaiKhoanNV"].ToString();

            // Mở form chi tiết tài khoản nhân viên
            FrmCTTKNhanVien frm = new FrmCTTKNhanVien(maTK, "edit");

            // Nếu đang dùng MDI
            frm.MdiParent = this.MdiParent;

            frm.Show();
        }
    }
}
