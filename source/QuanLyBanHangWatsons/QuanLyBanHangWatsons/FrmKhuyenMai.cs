using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace QuanLyBanHangWatsons
{
    public partial class FrmKhuyenMai : Form
    {
        Ket_noi kn = new Ket_noi();

        // Biến phân trang toàn cục
        int pageSize = 10;        // số dòng mỗi trang
        int pageNumber = 1;       // trang hiện tại
        int totalRecords = 0;     // tổng số dòng
        int totalPages = 0;       // tổng số trang

        // --- Thêm: biến lưu keyword hiện tại ---
        private string currentKeyword = "";

        private int filterTrangThai = -1;

        public FrmKhuyenMai()
        {
            InitializeComponent();
        }
        private void AddActionButtons()
        {
            // Tạo cột Sửa
            DataGridViewImageColumn colSua = new DataGridViewImageColumn();
            colSua.Name = "btnSua";
            colSua.HeaderText = "Sửa";
            string pathSua = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnSua.png");
            if (File.Exists(pathSua)) colSua.Image = Image.FromFile(pathSua);
            colSua.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colSua.Width = 50;
            colSua.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvKM.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 50;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvKM.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            // Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            //string sqlCount = "SELECT COUNT(*) FROM tblKhuyenMai";

            string sqlCount = "SELECT COUNT(*) FROM tblKhuyenMai WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaKhuyenMai LIKE '%{keyword}%' OR TenKhuyenMai LIKE N'%{keyword}%' OR MoTa LIKE N'%{keyword}%' OR MaSanPham LIKE N'%{keyword}%' " +
                    $"OR MaLoaiSanPham LIKE N'%{keyword}%' OR MaThuongHieu LIKE N'%{keyword}%' OR MaNhanVien LIKE N'%{keyword}%')";
            }
            // Lọc theo trạng thái KM
            if (filterTrangThai != -1)
            {
                sqlCount += $" AND TrangThaiKhuyenMai = {filterTrangThai}";
            }

            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));
            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvKM.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvKM.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy chương trình khuyến mãi nào!";
                lblTrang.Text = "";
                return;
            }

            // Tính tổng số trang
            totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Chống vượt giới hạn
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            int skip = (pageNumber - 1) * pageSize;

            // Lấy dữ liệu phân trang
            string query = $@"
            SELECT *
            FROM tblKhuyenMai
            WHERE 1 = 1
            ";
                        // Lọc keyword
                        if (!string.IsNullOrEmpty(keyword))
                        {
                            query += $@" AND (MaKhuyenMai LIKE '%{keyword}%' OR TenKhuyenMai LIKE N'%{keyword}%' 
                             OR MoTa LIKE N'%{keyword}%' OR MaSanPham LIKE N'%{keyword}%' 
                             OR MaLoaiSanPham LIKE N'%{keyword}%' OR MaThuongHieu LIKE N'%{keyword}%' 
                             OR MaNhanVien LIKE N'%{keyword}%')";
                        }

                        // Lọc trạng thái
                        if (filterTrangThai != -1)
                        {
                            query += $" AND TrangThaiKhuyenMai = {filterTrangThai}";
                        }

                        // Cuối cùng thêm phân trang
                        query += $@"
            ORDER BY MaKhuyenMai
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvKM.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvKM.Columns.Contains("btnSua")) dgvKM.Columns.Remove("btnSua");
            if (dgvKM.Columns.Contains("btnXoa")) dgvKM.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvKM.Columns["MaKhuyenMai"].HeaderText = "Mã khuyến mãi";
            dgvKM.Columns["TenKhuyenMai"].HeaderText = "Tên khuyến mãi";
            dgvKM.Columns["GiaTri"].HeaderText = "Giá trị";
            dgvKM.Columns["MoTa"].HeaderText = "Mô tả";
            dgvKM.Columns["GiaTriDonHang"].HeaderText = "Giá trị đơn hàng";
            dgvKM.Columns["LoaiDonHangApDung"].HeaderText = "Loại đơn hàng";
            dgvKM.Columns["HangThanhVienApDung"].HeaderText = "Hạng thành viên";
            dgvKM.Columns["ApDungThangSinhNhat"].HeaderText = "Tháng sinh nhật";
            dgvKM.Columns["ApDungKhachHangMoi"].HeaderText = "Khách hàng mới";
            dgvKM.Columns["MaSanPham"].HeaderText = "Sản phẩm";
            dgvKM.Columns["MaLoaiSanPham"].HeaderText = "Loại sản phẩm";
            dgvKM.Columns["MaThuongHieu"].HeaderText = "Thương hiệu";
            dgvKM.Columns["NgayBatDau"].HeaderText = "Ngày bắt đầu";
            dgvKM.Columns["NgayKetThuc"].HeaderText = "Ngày kết thúc";
            dgvKM.Columns["SoLanSuDungToiDa"].HeaderText = "Số lần sử dụng";
            dgvKM.Columns["TrangThaiKhuyenMai"].HeaderText = "Trạng Thái";
            dgvKM.Columns["MaNhanVien"].HeaderText = "Nhân viên";


            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvKM.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                // Cột icon
                if (col is DataGridViewImageColumn)
                {
                    col.Width = 50;
                    col.MinimumWidth = 50;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    continue;
                }

                switch (col.Name)
                {
                    case "MaKhuyenMai":
                        col.Width = 120;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "TenKhuyenMai":
                        col.Width = 260;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;

                    case "GiaTri":
                        col.Width = 90;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MoTa":
                        col.Width = 320;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;

                    case "GiaTriDonHang":
                        col.Width = 150;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "LoaiDonHangApDung":
                        col.Width = 160;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "HangThanhVienApDung":
                        col.Width = 160;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "ApDungThangSinhNhat":
                        col.Width = 130;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "ApDungKhachHangMoi":
                        col.Width = 140;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaSanPham":
                        col.Width = 120;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaLoaiSanPham":
                        col.Width = 140;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaThuongHieu":
                        col.Width = 140;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "NgayBatDau":
                        col.Width = 130;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "NgayKetThuc":
                        col.Width = 130;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "SoLanSuDungToiDa":
                        col.Width = 150;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "TrangThaiKhuyenMai":
                        col.Width = 130;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaNhanVien":
                        col.Width = 120;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                }           
        }


            // Định dạng cột 'NgayDatHang' để chỉ hiển thị ngày tháng năm
            dgvKM.Columns["NgayBatDau"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvKM.Columns["NgayKetThuc"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Toàn bảng
            //dgvKM.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKM.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn
                                                                            // Bật scroll ngang + dọc
            dgvKM.ScrollBars = ScrollBars.Both;

            // Không cho tự co giãn theo form
            dgvKM.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Không cho user resize lung tung
            dgvKM.AllowUserToResizeColumns = true;
            dgvKM.AllowUserToResizeRows = false;

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmKhuyenMai_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvKM.CellClick += dgvKM_CellClick;
            dgvKM.CellDoubleClick += dgvKM_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            tsmDangAD.Click += tsmDangAD_Click;
            tsmDaHH.Click += tsmDaHH_Click;

            LoadData();
            TaoMenuCot();


        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvKM.Columns)
            {
                if (col.Name == "btnSua" || col.Name == "btnXoa") continue;

                ToolStripMenuItem item = new ToolStripMenuItem(col.HeaderText);
                item.CheckOnClick = true;
                item.Checked = col.Visible;
                item.Tag = col.Name;   // liên kết item → column

                item.Click += (s, e) =>
                {
                    string colName = item.Tag.ToString();
                    dgvKM.Columns[colName].Visible = item.Checked;
                };

                cmsCot.Items.Add(item);
            }
        }

        private void CapNhatTrangThaiMenuCot()
        {
            foreach (ToolStripItem i in cmsCot.Items)
            {
                if (i is ToolStripMenuItem item && item.Tag != null)
                {
                    string colName = item.Tag.ToString();

                    if (dgvKM.Columns.Contains(colName))
                    {
                        item.Checked = dgvKM.Columns[colName].Visible;
                    }
                }
            }
        }


        private void dgvKM_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string makm = dgvKM.Rows[e.RowIndex].Cells["MaKhuyenMai"].Value.ToString();
            //string ten = dgvKM.Rows[e.RowIndex].Cells["TenKhuyenMai"].Value.ToString();
            //string giatri = dgvKM.Rows[e.RowIndex].Cells["GiaTri"].Value.ToString();
            //string mota = dgvKM.Rows[e.RowIndex].Cells["MoTa"].Value.ToString();



            // Nếu nhấn nút sửa
            if (dgvKM.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTKhuyenMai frm = new FrmCTKhuyenMai(makm, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvKM.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa chương trình khuyến mãi này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblKhuyenMai WHERE MaKhuyenMai = '{makm}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy chương trình khuyến mãi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvKM_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string makm = dgvKM.Rows[e.RowIndex].Cells["MaKhuyenMai"].Value.ToString();
            //string ten = dgvKM.Rows[e.RowIndex].Cells["TenKhuyenMai"].Value.ToString();
            //string giatri = dgvKM.Rows[e.RowIndex].Cells["GiaTri"].Value.ToString();
            //string mota = dgvKM.Rows[e.RowIndex].Cells["MoTa"].Value.ToString();

            FrmCTKhuyenMai frm = new FrmCTKhuyenMai(makm, "edit");
            // --- Thêm: FormClosed event để reload data ---
            frm.FormClosed += (s, ev) => LoadData();
            frm.ShowDialog();
        }

        private void btnLocTTTT_Click(object sender, EventArgs e)
        {
            cmsTTKM.Show(btnLocTTTT, new Point(0, btnLocTTTT.Height));
        }

        private void tsmDangAD_Click(object sender, EventArgs e)
        {
            filterTrangThai = 1;
            pageNumber = 1;
            LoadData(currentKeyword);
        }

        private void tsmDaHH_Click(object sender, EventArgs e)
        {
            filterTrangThai = 0;
            pageNumber = 1;
            LoadData(currentKeyword);
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTKhuyenMai frm = new FrmCTKhuyenMai("", "add");
            // --- Thêm: FormClosed event để reload data ---
            frm.FormClosed += (s, ev) => LoadData();
            frm.ShowDialog();
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
                        MessageBox.Show($"Tìm thấy {totalRecords} chương trình khuyến mãi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy chương trình khuyến mãi nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            filterTrangThai = -1;
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }
    }
}
