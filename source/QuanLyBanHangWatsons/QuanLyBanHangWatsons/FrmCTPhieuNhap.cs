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
    public partial class FrmCTPhieuNhap : Form
    {
        Ket_noi kn = new Ket_noi();
        private bool _isLoaded = false;

        // ==== Thêm biến tìm kiếm + phân trang ====
        int pageSize = 10;
        int pageNumber = 1;
        int totalRecords = 0;
        int totalPages = 0;
        string currentKeyword = "";

        public FrmCTPhieuNhap()
        {
            InitializeComponent();
          
        }

        private void FrmCTPhieuNhap_Load(object sender, EventArgs e)
        {
            if (_isLoaded) return;
            _isLoaded = true;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            dgvCTPN.CellDoubleClick += dgvCTPN_CellDoubleClick;
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
            FROM tblCTPhieuNhap ctpn
            INNER JOIN tblSanPham sp ON ctpn.MaSanPham = sp.MaSanPham
            WHERE 1 = 1
            ";
          

            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $@"
                    AND (
                        ctpn.MaSanPham LIKE '%{keyword}%'
                        OR sp.TenSanPham LIKE N'%{keyword}%'
                        OR ctpn.MaPhieuNhap LIKE '%{keyword}%'
                    )";
            }

            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            if (totalRecords == 0)
            {
                dgvCTPN.DataSource = null;
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
                ctpn.MaPhieuNhap,
                ctpn.MaSanPham,
                sp.TenSanPham,
                ctpn.SoLuong,
                ctpn.GiaNhap,
                ctpn.NgaySanXuat,
                ctpn.HanSuDung,
                (ctpn.SoLuong * ctpn.GiaNhap) AS ThanhTien
            FROM tblCTPhieuNhap ctpn
            INNER JOIN tblSanPham sp ON ctpn.MaSanPham = sp.MaSanPham
            WHERE 1 = 1
            ";
            

            if (!string.IsNullOrEmpty(keyword))
            {
                sql += $@"
                    AND (
                        ctpn.MaSanPham LIKE '%{keyword}%'
                        OR sp.TenSanPham LIKE N'%{keyword}%'
                        OR ctpn.MaPhieuNhap LIKE '%{keyword}%'
                    )";
            }

            sql += $@"
                ORDER BY ctpn.MaPhieuNhap, ctpn.MaSanPham
                OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY
            ";

            dgvCTPN.DataSource = kn.ExecuteQuery(sql);

            // ===== FORMAT =====
            dgvCTPN.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCTPN.Columns["GiaNhap"].DefaultCellStyle.Format = "N0";
            dgvCTPN.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
            dgvCTPN.Columns["NgaySanXuat"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvCTPN.Columns["HanSuDung"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";
        }

        private void dgvCTPN_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string maPN = dgvCTPN.Rows[e.RowIndex].Cells["MaPhieuNhap"].Value.ToString();
            string maSP = dgvCTPN.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();
            string soLuong = dgvCTPN.Rows[e.RowIndex].Cells["SoLuong"].Value.ToString();
            string giaNhap = dgvCTPN.Rows[e.RowIndex].Cells["GiaNhap"].Value.ToString();
            DateTime nsx = Convert.ToDateTime(dgvCTPN.Rows[e.RowIndex].Cells["NgaySanXuat"].Value);
            DateTime hsd = Convert.ToDateTime(dgvCTPN.Rows[e.RowIndex].Cells["HanSuDung"].Value);

            FrmNhapCTPN f = new FrmNhapCTPN(
            maPN,
            maSP,
            soLuong,
            giaNhap,
            nsx,
            hsd,
            "edit"
    );
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
                        MessageBox.Show($"Tìm thấy {totalRecords} phiếu nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy phiếu nhập nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
