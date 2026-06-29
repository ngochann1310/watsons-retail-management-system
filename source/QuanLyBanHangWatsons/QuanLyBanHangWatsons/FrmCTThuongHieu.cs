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
    public partial class FrmCTThuongHieu : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maTH;

        public FrmCTThuongHieu(string ma, string ten, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maTH = ma;
            if (mode == "edit")
            {
                txtMaTH.Text = ma;
                txtTenTH.Text = ten;

                txtMaTH.Enabled = false;
                btnThem.Enabled = false;
            }
            else // thêm
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void FrmCTThuongHieu_Load(object sender, EventArgs e)
        {
            if (mode == "add")
            {
                txtMaTH.Enabled = false;

                if (string.IsNullOrWhiteSpace(txtMaTH.Text))
                {
                    maTH = GenerateNewMaTH();  
                    txtMaTH.Text = maTH;        
                }
            }
        }


        private string GenerateNewMaTH()
        {
            string sql = "SELECT TOP 1 MaThuongHieu FROM tblThuongHieu ORDER BY MaThuongHieu DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value)
                return "TH0001";

            string last = obj.ToString().Trim();

            // lấy phần số cuối liên tiếp từ cuối chuỗi (GIỐNG form mẫu)
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // TH
            string numPart = last.Substring(i + 1);     // 0001

            int num = 0;
            if (!int.TryParse(numPart, out num))
                return prefix + "1";

            num++;
            string newNum = num.ToString().PadLeft(numPart.Length, '0');
            return prefix + newNum;
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            string ten = txtTenTH.Text.Trim();

            if (string.IsNullOrEmpty(ten))
            {
                MessageBox.Show("Tên thương hiệu không được để trống!",
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = $@"
            INSERT INTO tblThuongHieu (MaThuongHieu, TenThuongHieu)
            VALUES ('{maTH}', N'{ten}')
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
            if (string.IsNullOrEmpty(maTH))
            {
                MessageBox.Show("Không có thương hiệu để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa thương hiệu này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                // ExecuteNonQuery trả về số dòng bị ảnh hưởng
                int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblThuongHieu WHERE MaThuongHieu = '{maTH}'");

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Xóa thất bại! Không tìm thấy thương hiệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.Close(); // đóng form sau khi thông báo
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string ten = txtTenTH.Text.Trim();

            if (string.IsNullOrEmpty(ten))
            {
                MessageBox.Show("Tên thương hiệu không được để trống!",
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = $"UPDATE tblThuongHieu SET TenThuongHieu = N'{ten}' WHERE MaThuongHieu = '{maTH}'";

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
