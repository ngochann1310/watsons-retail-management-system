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
    public partial class FrmCTDonHang : Form
    {
        Ket_noi kn = new Ket_noi();
        private bool _isLoaded = false;

        // ==== Thêm biến tìm kiếm + phân trang ====
        int pageSize = 10;
        int pageNumber = 1;
        int totalRecords = 0;
        int totalPages = 0;
        private string currentKeyword = "";

        public FrmCTDonHang()
        {
            InitializeComponent();
        }

        //private void LoadData()
        //{
        //    string query = @"
        //    SELECT *
        //    FROM tblChiTietDonHang
        //    ORDER BY MaDonHang
        //";

        //    dgvCTDH.DataSource = kn.ExecuteQuery(query);

        //    foreach (DataGridViewColumn col in dgvCTDH.Columns)
        //    {
        //        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        //        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        //    }

        //}

        private void FrmCTDonHang_Load(object sender, EventArgs e)
        {
            if (_isLoaded) return;
            _isLoaded = true;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            dgvCTDH.CellDoubleClick += dgvCTDH_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            LoadData();

        }

        private void LoadData(string keyword = "")
        {
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            string sqlCount = @"
        SELECT COUNT(*)
        FROM tblChiTietDonHang ctdh
        INNER JOIN tblSanPham sp ON ctdh.MaSanPham = sp.MaSanPham
        WHERE 1 = 1
    ";

            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $@"
            AND (
                ctdh.MaDonHang LIKE '%{keyword}%'
                OR ctdh.MaSanPham LIKE '%{keyword}%'
                OR sp.TenSanPham LIKE N'%{keyword}%'
            )";
            }

            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));
            if (totalRecords == 0)
            {
                dgvCTDH.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvCTDH.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy chi tiết đơn hàng nào!";
                lblTrang.Text = "";
                return;
            }

            totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            int skip = (pageNumber - 1) * pageSize;

            

            string sql = @"
        SELECT
            ctdh.MaDonHang,
            ctdh.MaSanPham,
            sp.TenSanPham,
            ctdh.SoLuong,
            ctdh.DonGia,
            (ctdh.SoLuong * ctdh.DonGia) AS ThanhTien
        FROM tblChiTietDonHang ctdh
        INNER JOIN tblSanPham sp ON ctdh.MaSanPham = sp.MaSanPham
        WHERE 1 = 1
    ";

            if (!string.IsNullOrEmpty(keyword))
            {
                sql += $@"
            AND (
                ctdh.MaDonHang LIKE '%{keyword}%'
                OR ctdh.MaSanPham LIKE '%{keyword}%'
                OR sp.TenSanPham LIKE N'%{keyword}%'
            )";
            }

            sql += $@"
        ORDER BY ctdh.MaDonHang, ctdh.MaSanPham
        OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY
    ";

            dgvCTDH.DataSource = kn.ExecuteQuery(sql);

            dgvCTDH.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCTDH.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            //dgvCTDH.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";

            lblTongSoBanGhi.Text = $"Tổng số bản ghi: {totalRecords}";
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";
        }

        private void dgvCTDH_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string maDH = dgvCTDH.Rows[e.RowIndex].Cells["MaDonHang"].Value.ToString();
            string maSP = dgvCTDH.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();
            string soLuong = dgvCTDH.Rows[e.RowIndex].Cells["SoLuong"].Value.ToString();
            string donGia = dgvCTDH.Rows[e.RowIndex].Cells["DonGia"].Value.ToString();

            FrmNhapCTDH f = new FrmNhapCTDH(
                maDH,
                maSP,
                soLuong,
                donGia,
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
                        MessageBox.Show($"Tìm thấy {totalRecords} đơn hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy đơn hàng nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void btnDanhSach_Click_1(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }
    }
}
