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
    public partial class FrmCTPhieuXuat : Form
    {
        Ket_noi kn = new Ket_noi();
        

        // ==== Thêm biến tìm kiếm + phân trang ====
        int pageSize = 10;
        int pageNumber = 1;
        int totalRecords = 0;
        int totalPages = 0;
        string currentKeyword = "";

        public FrmCTPhieuXuat()
        {
            InitializeComponent();
            
        }

        private void FrmCTPhieuXuat_Load(object sender, EventArgs e)
        {
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            dgvCTPX.CellDoubleClick += dgvCTPX_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;


            LoadData();
        }

        private void LoadData(string keyword = "")
        {
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // ---- Đếm số bản ghi ----
            string sqlCount = @"
                SELECT COUNT(*)
                FROM tblCTPhieuXuat ct
                INNER JOIN tblSanPham sp ON ct.MaSanPham = sp.MaSanPham
                WHERE 1 = 1
            ";

            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $@"
            AND (
                ct.MaSanPham LIKE '%{keyword}%'
                OR sp.TenSanPham LIKE N'%{keyword}%'
                OR ct.MaPhieuXuat LIKE '%{keyword}%'
            )";
            }

            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            if (totalRecords == 0)
            {
                dgvCTPX.DataSource = null;
                lblTrang.Text = "Không có dữ liệu!";
                return;
            }

            totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            int skip = (pageNumber - 1) * pageSize;

            // ---- Truy vấn dữ liệu ----
            string sql = @"
            SELECT 
                ct.MaPhieuXuat,
                ct.MaSanPham,
                sp.TenSanPham,
                ct.SoLuong,
                ct.DonGia,
                (ct.SoLuong * ct.DonGia) AS ThanhTien
            FROM tblCTPhieuXuat ct
            INNER JOIN tblSanPham sp ON ct.MaSanPham = sp.MaSanPham
            WHERE 1 = 1
        ";

            if (!string.IsNullOrEmpty(keyword))
            {
                sql += $@"
            AND (
                ct.MaSanPham LIKE '%{keyword}%'
                OR sp.TenSanPham LIKE N'%{keyword}%'
                OR ct.MaPhieuXuat LIKE '%{keyword}%'
            )";
            }

            sql += $@"
    ORDER BY ct.MaPhieuXuat, ct.MaSanPham
        OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY
    ";

            dgvCTPX.DataSource = kn.ExecuteQuery(sql);

            // ===== FORMAT =====
            dgvCTPX.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCTPX.Columns["DonGia"].DefaultCellStyle.Format = "N0";

            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";
        }

        private void dgvCTPX_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string maPX = dgvCTPX.Rows[e.RowIndex].Cells["MaPhieuXuat"].Value.ToString();
            string maSP = dgvCTPX.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();
            int soLuong = Convert.ToInt32(dgvCTPX.Rows[e.RowIndex].Cells["SoLuong"].Value);
            decimal donGia = Convert.ToDecimal(dgvCTPX.Rows[e.RowIndex].Cells["DonGia"].Value);

            FrmNhapCTPX f = new FrmNhapCTPX(
                maPX,
                null,       // không cần MaDonHang khi edit
                maSP,
                soLuong,
                donGia
            );
            f.IsEdit = true;
            f.MaSanPham_Edit = maSP;
            f.SoLuong_Edit = soLuong;
            f.DonGia_Edit = donGia;

            f.ShowDialog();

            LoadData();
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                pageNumber = 1;
                currentKeyword = txtTimKiem.Text.Trim();

                LoadData(currentKeyword);

                if (!string.IsNullOrEmpty(currentKeyword))
                {
                    if (totalRecords > 0)
                        MessageBox.Show($"Tìm thấy {totalRecords} phiếu xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy phiếu xuất nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void cboSoBanGhiHienThi_SelectedIndexChanged(object sender, EventArgs e)
        {
            pageNumber = 1;
            if (string.IsNullOrEmpty(txtTimKiem.Text.Trim()))
                LoadData();
            else
                LoadData(txtTimKiem.Text.Trim());
        }

        private void btnTrangDau_Click(object sender, EventArgs e)
        {
            pageNumber = 1;
            if (string.IsNullOrEmpty(txtTimKiem.Text.Trim()))
                LoadData();
            else
                LoadData(txtTimKiem.Text.Trim());
        }

        private void btnTrangTruoc_Click(object sender, EventArgs e)
        {
            if (pageNumber > 1) pageNumber--;
            if (string.IsNullOrEmpty(txtTimKiem.Text.Trim()))
                LoadData(); // không search → load toàn bộ
            else
                LoadData(txtTimKiem.Text.Trim());
        }

        private void btnTrangSau_Click(object sender, EventArgs e)
        {
            if (pageNumber < totalPages) pageNumber++;
            if (string.IsNullOrEmpty(txtTimKiem.Text.Trim()))
                LoadData();
            else
                LoadData(txtTimKiem.Text.Trim());
        }

        private void btnTrangCuoi_Click(object sender, EventArgs e)
        {
            pageNumber = totalPages;
            if (string.IsNullOrEmpty(txtTimKiem.Text.Trim()))
                LoadData();
            else
                LoadData(txtTimKiem.Text.Trim());
        }

        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }
    }
}
