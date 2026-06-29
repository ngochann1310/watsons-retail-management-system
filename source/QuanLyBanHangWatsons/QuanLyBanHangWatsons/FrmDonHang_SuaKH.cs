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
    public partial class FrmDonHang_SuaKH : Form
    {
        Ket_noi kn = new Ket_noi();

        // Callback trả kết quả về form tạo đơn
        public Action<string> OnKhachHangSelected;


        public FrmDonHang_SuaKH()
        {
            InitializeComponent();
        }

        private void FrmDonHang_SuaKH_Load(object sender, EventArgs e)
        {
            LoadDanhSachKH();

            dgvKHDH.CellDoubleClick += dgvKHDH_CellDoubleClick;
            txtTimKiem.KeyDown += TxtTimKiem_KeyDown;
        }

        private void LoadDanhSachKH(string keyword = "")
        {
            string sql = @"
                SELECT MaKhachHang, HoKhachHang, TenKhachHang,
                       DienThoai, DiemTichLuy, DiaChi
                FROM tblKhachHang
            ";

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += $@"
                    WHERE MaKhachHang LIKE '%{keyword}%'
                       OR HoKhachHang LIKE N'%{keyword.Replace("'", "''")}%'
                       OR TenKhachHang LIKE N'%{keyword.Replace("'", "''")}%'
                       OR DienThoai LIKE '%{keyword}%'
                ";
            }

            dgvKHDH.DataSource = kn.ExecuteQuery(sql);
        }

        private void TxtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadDanhSachKH(txtTimKiem.Text.Trim());
                e.SuppressKeyPress = true;
            }
        }

        private void dgvKHDH_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string maKH = dgvKHDH.Rows[e.RowIndex].Cells["MaKhachHang"].Value.ToString();

            OnKhachHangSelected?.Invoke(maKH);

            this.Close();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvKHDH.CurrentRow == null) return;

            string maKH = dgvKHDH.CurrentRow.Cells["MaKhachHang"].Value.ToString();

            OnKhachHangSelected?.Invoke(maKH);

            this.Close();
        }
    }
}
