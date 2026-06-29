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
    public partial class FrmCTNhaCungCap : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maNCC;

        public FrmCTNhaCungCap(string ma, string ten, string diachi, string sdt, string email, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maNCC = ma;
            if (mode == "edit")
            {
                txtMaNCC.Text = ma;
                txtTenNCC.Text = ten;
                txtDiaChiNCC.Text = diachi;
                txtSDTNCC.Text = sdt;
                txtEmailNCC.Text = email;

                txtMaNCC.Enabled = false;
                btnThem.Enabled = false;
            }
            else // thêm
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void FrmCTNhaCungCap_Load(object sender, EventArgs e)
        {
            if (mode == "add")
            {
                txtMaNCC.Enabled = false;

                if (string.IsNullOrWhiteSpace(txtMaNCC.Text))
                    txtMaNCC.Text = GenerateNewMaNCC();
            }
        }

        private string GenerateNewMaNCC()
        {
            string sql = "SELECT TOP 1 MaNhaCungCap FROM tblNhaCungCap ORDER BY MaNhaCungCap DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value)
                return "NCC001";

            string last = obj.ToString().Trim();

            // lấy phần số cuối liên tiếp từ cuối chuỗi
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // NCC
            string numPart = last.Substring(i + 1);     // 001

            int num = 0;
            if (!int.TryParse(numPart, out num))
            {
                return prefix + "1";
            }

            num++;
            string newNum = num.ToString().PadLeft(numPart.Length, '0');
            return prefix + newNum;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string ma = txtMaNCC.Text.Trim();
            string ten = txtTenNCC.Text.Trim();
            string diachi = txtDiaChiNCC.Text.Trim();
            string sdt = txtSDTNCC.Text.Trim();
            string email = txtEmailNCC.Text.Trim();

            string query = $@"
            INSERT INTO tblNhaCungCap 
            (MaNhaCungCap, TenNhaCungCap, DiaChi, DienThoai, Email)
            VALUES
            ('{ma}', N'{ten}', N'{diachi}', N'{sdt}', N'{email}')
            ";

            try
            {
                int result = kn.ExecuteNonQuery(query);

                // Nếu trigger báo lỗi → SQL trả về -1
                if (result <= 0)
                {
                    MessageBox.Show("Thêm thất bại. Người dùng nhập dữ liệu không hợp lệ!",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm thất bại. Người dùng nhập dữ liệu không hợp lệ!\n" + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maNCC))
            {
                MessageBox.Show("Không có nhà cung cấp để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa nhà cung cấp này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                // ExecuteNonQuery trả về số dòng bị ảnh hưởng
                int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblNhaCungCap WHERE MaNhaCungCap = '{maNCC}'");

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Xóa thất bại! Không tìm thấy nhà cung cấp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.Close(); // đóng form sau khi thông báo
            }
        }
        
        private void btnSua_Click(object sender, EventArgs e)
        {
            string ten = txtTenNCC.Text.Trim();
            string diachi = txtDiaChiNCC.Text.Trim();
            string sdt = txtSDTNCC.Text.Trim();
            string email = txtEmailNCC.Text.Trim();

            string query = $"UPDATE tblNhaCungCap SET TenNhaCungCap = N'{ten}', DiaChi = N'{diachi}', DienThoai = N'{sdt}', Email = N'{email}'  WHERE MaNhaCungCap = '{maNCC}'";

            try
            {
                int result = kn.ExecuteNonQuery(query);

                // Nếu trigger báo lỗi → SQL trả về -1
                if (result <= 0)
                {
                    MessageBox.Show("Sửa thất bại. Người dùng nhập dữ liệu không hợp lệ!",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show("Sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sửa thất bại. Người dùng nhập dữ liệu không hợp lệ!\n" + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
