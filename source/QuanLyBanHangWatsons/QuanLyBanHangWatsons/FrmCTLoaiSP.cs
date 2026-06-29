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
    public partial class FrmCTLoaiSP : Form
    {

        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maLSP;

        public FrmCTLoaiSP(string ma, string ten, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maLSP = ma;


            if (mode == "edit")
            {
                txtMaLoaiSP.Text = ma;
                txtTenLoaiSP.Text = ten;

                txtMaLoaiSP.Enabled = false;
                btnThem.Enabled = false;
            }
            else // thêm
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void FrmCTLoaiSP_Load(object sender, EventArgs e)
        {
            if (mode == "add")
            {
                txtMaLoaiSP.Enabled = false;

                if (string.IsNullOrWhiteSpace(txtMaLoaiSP.Text))
                    txtMaLoaiSP.Text = GenerateNewMaLoaiSP();
            }
        }

        private string GenerateNewMaLoaiSP()
        {
            string sql = "SELECT TOP 1 MaLoaiSanPham FROM tblLoaiSanPham ORDER BY MaLoaiSanPham DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value)
                return "LSP001";

            string last = obj.ToString().Trim();

            // tách phần chữ và phần số giống form mẫu
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // ví dụ: LSP
            string numPart = last.Substring(i + 1);     // ví dụ: 001

            int num;
            if (!int.TryParse(numPart, out num))
                return prefix + "1";

            num++;
            string newNum = num.ToString().PadLeft(numPart.Length, '0');

            return prefix + newNum;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string ma = txtMaLoaiSP.Text.Trim();
            string ten = txtTenLoaiSP.Text.Trim();

            string query = $@"
            INSERT INTO tblLoaiSanPham (MaLoaiSanPham, TenLoaiSanPham)
            VALUES ('{ma}', N'{ten}')
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
            if (string.IsNullOrEmpty(maLSP))
            {
                MessageBox.Show("Không có loại sản phẩm để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa loại sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                // ExecuteNonQuery trả về số dòng bị ảnh hưởng
                int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblLoaiSanPham WHERE MaLoaiSanPham = '{maLSP}'");

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Xóa thất bại! Không tìm thấy loại sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.Close(); // đóng form sau khi thông báo
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string ten = txtTenLoaiSP.Text.Trim();

            string query = $"UPDATE tblLoaiSanPham SET TenLoaiSanPham = N'{ten}' WHERE MaLoaiSanPham = '{maLSP}'";

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
