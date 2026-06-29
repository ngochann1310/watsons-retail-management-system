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
    public partial class FrmCTChiNhanh : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maCN;

        public FrmCTChiNhanh(string ma, string ten, string diachi, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maCN = ma;
            if (mode == "edit")
            {
                txtMaCN.Text = ma;
                txtTenCN.Text = ten;
                txtDiaChi.Text = diachi;

                txtMaCN.Enabled = false;
                btnThem.Enabled = false;
            }
            else // thêm
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void FrmCTChiNhanh_Load(object sender, EventArgs e)
        {
            if (mode == "add")
            {
                txtMaCN.Enabled = false;

                if (string.IsNullOrWhiteSpace(txtMaCN.Text))
                    txtMaCN.Text = GenerateNewMaCTCN();
            }

            if (mode == "edit")
            {
                txtMaCN.Enabled = false;
            }
        }

        private string GenerateNewMaCTCN()
        {
            string sql = "SELECT TOP 1 MaChiNhanh FROM tblChiNhanh ORDER BY MaChiNhanh DESC";
            var obj = kn.ExecuteScalar(sql);

            // Nếu bảng chưa có dữ liệu → trả về mã đầu tiên
            if (obj == null || obj == DBNull.Value)
                return "CN01";

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


        private void btnThem_Click(object sender, EventArgs e)
        {
            string ten = txtTenCN.Text.Trim();
            string diachi = txtDiaChi.Text.Trim();

            string query = $"INSERT INTO tblChiNhanh (TenChiNhanh, DiaChi) VALUES (N'{ten}', N'{diachi}')";

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
                return; // KHÔNG đóng form nếu bị lỗi
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maCN))
            {
                MessageBox.Show("Không có chi nhánh để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa chi nhánh này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                // ExecuteNonQuery trả về số dòng bị ảnh hưởng
                int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblChiNhanh WHERE MaChiNhanh = '{maCN}'");

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Xóa thất bại! Không tìm thấy chi nhánh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.Close(); // đóng form sau khi thông báo
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string ten = txtTenCN.Text.Trim();
            string diachi = txtDiaChi.Text.Trim();

            string query = $"UPDATE tblChiNhanh SET TenChiNhanh = N'{ten}', DiaChi = N'{diachi}' WHERE MaChiNhanh = '{maCN}'";

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
                return; // KHÔNG đóng form khi lỗi
            }
        }
    }
}
