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
using System.IO;

namespace QuanLyBanHangWatsons
{
    public partial class FrmThanhToan : Form
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
        string filterPTTT = "";

        public FrmThanhToan()
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
            colSua.Width = 60;
            colSua.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvTT.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 60;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvTT.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblThanhToan WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $@" AND (
                MaThanhToan LIKE '%{keyword}%' 
                OR MaKhachHang LIKE '%{keyword}%'
                OR MaDonHang LIKE '%{keyword}%'
                OR MaNhanVien LIKE '%{keyword}%'
                OR PhuongThucThanhToan LIKE N'%{keyword}%'
            )";
            }
            // Lọc theo trạng thái 
            if (filterTrangThai != -1)
            {
                sqlCount += $" AND TrangThaiThanhToan = {filterTrangThai}";
            }

            // Lọc theo loại DH
            if (!string.IsNullOrEmpty(filterPTTT))
            {
                var roles = filterPTTT.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"PhuongThucThanhToan LIKE N'%{r}%'"));
                sqlCount += $" AND ({likeCondition})";
            }
            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvTT.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvTT.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy thanh toán nào!";
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
            FROM tblThanhToan
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (
                MaThanhToan LIKE '%{keyword}%' 
                OR MaKhachHang LIKE '%{keyword}%'
                OR MaDonHang LIKE '%{keyword}%'
                OR MaNhanVien LIKE '%{keyword}%'
                OR PhuongThucThanhToan LIKE N'%{keyword}%'
            )";
            }

            // Lọc trạng thái
            if (filterTrangThai != -1)
            {
                query += $" AND TrangThaiThanhToan = {filterTrangThai}";
            }
            // Lọc nhiều loại đơn hàng
            if (!string.IsNullOrEmpty(filterPTTT))
            {
                var roles = filterPTTT.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"PhuongThucThanhToan LIKE N'%{r}%'"));
                query += $" AND ({likeCondition})";
            }
            // Cuối cùng thêm phân trang
            query += $@"
        ORDER BY MaThanhToan
        OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY
        ";

            dgvTT.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvTT.Columns.Contains("btnSua")) dgvTT.Columns.Remove("btnSua");
            if (dgvTT.Columns.Contains("btnXoa")) dgvTT.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvTT.Columns["MaThanhToan"].HeaderText = "Mã thanh toán";
            dgvTT.Columns["MaKhachHang"].HeaderText = "Khách hàng";
            dgvTT.Columns["MaDonHang"].HeaderText = "Đơn hàng";
            dgvTT.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvTT.Columns["PhuongThucThanhToan"].HeaderText = "Phương thức";
            dgvTT.Columns["NgayThanhToan"].HeaderText = "Ngày thanh toán";
            dgvTT.Columns["TongTien"].HeaderText = "Tổng tiền";
            dgvTT.Columns["TrangThaiThanhToan"].HeaderText = "Trạng thái";

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvTT.Columns)
            {
                // Header luôn center
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

               

                // Nếu là cột nút thì bỏ qua (không FillWeight)
                if (col.Name == "btnSua" || col.Name == "btnXoa")
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    continue;
                }

                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // tự giãn
                switch (col.Name)
                {
                    case "MaThanhToan":
                    case "MaKhachHang":
                    case "MaDonHang":
                    case "MaNhanVien":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "PhuongThucThanhToan":
                        col.FillWeight = 20;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "TongTien":
                        col.FillWeight = 10;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.DefaultCellStyle.Format = "#,##0 đ";  // format tiền
                        break;

                    case "NgayThanhToan":
                        col.FillWeight = 10;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                        break;

                    case "TrangThaiThanhToan":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                }
            }



            // Toàn bảng
            dgvTT.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTT.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }


        private void FrmThanhToan_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvTT.CellClick += dgvTT_CellClick;
            dgvTT.CellDoubleClick += dgvTT_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            tsmDaTT.Click += tsmDaTT_Click;
            tsmChuaTT.Click += tsmChuaTT_Click;

            BuildFilterMenuPTTT();

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvTT.Columns)
            {
                // loại khỏi menu: nút chức năng, ảnh mới, và ảnh gốc
                if (col.Name == "btnSua" || col.Name == "btnXoa" ||
                    col.Name == "colImg" || col.Name == "HinhAnh")
                    continue;

                ToolStripMenuItem item = new ToolStripMenuItem(col.HeaderText);
                item.CheckOnClick = true;
                item.Checked = col.Visible;
                item.Tag = col.Name;   // liên kết item → column

                item.Click += (s, e) =>
                {
                    string colName = item.Tag.ToString();
                    dgvTT.Columns[colName].Visible = item.Checked;
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

                    if (dgvTT.Columns.Contains(colName))
                    {
                        item.Checked = dgvTT.Columns[colName].Visible;
                    }
                }
            }
        }

        private void BuildFilterMenuPTTT()
        {
            cmsPTTT.Items.Clear();

            string[] dsPTTT = { "Thẻ tín dụng/Thẻ ghi nợ", "Chuyển khoản",
                                    "Thẻ ATM", "Tiền mặt khi giao hàng",
                                    "Thanh toán QR", "Ví điện tử"};

            foreach (var q in dsPTTT)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(q);
                item.Tag = q;
                item.CheckOnClick = true; // Cho phép check/uncheck nhiều cái

                item.Click += (s, e) =>
                {
                    // Lấy tất cả quyền đang được check
                    var selectedRoles = cmsPTTT.Items
                        .OfType<ToolStripMenuItem>()
                        .Where(m => m.Checked)
                        .Select(m => m.Tag.ToString())
                        .ToList();

                    // Gán filter = danh sách quyền (cách nhau dấu phẩy)
                    filterPTTT = string.Join(",", selectedRoles);

                    pageNumber = 1;
                    LoadData(currentKeyword);
                };

                cmsPTTT.Items.Add(item);
            }
        }

        private void dgvTT_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string matt = dgvTT.Rows[e.RowIndex].Cells["MaThanhToan"].Value.ToString();

            // ==== SỬA ====
            if (dgvTT.Columns[e.ColumnIndex].Name == "btnSua")
            {
                string maTT = dgvTT.Rows[e.RowIndex].Cells["MaThanhToan"].Value.ToString();
                string maDH = dgvTT.Rows[e.RowIndex].Cells["MaDonHang"].Value.ToString();
                string maKH = dgvTT.Rows[e.RowIndex].Cells["MaKhachHang"].Value.ToString();
                string maNV = dgvTT.Rows[e.RowIndex].Cells["MaNhanVien"].Value.ToString();

                string tongTien = dgvTT.Rows[e.RowIndex].Cells["TongTien"].Value.ToString();

                FrmCTThanhToan f = new FrmCTThanhToan(
                    maTT,
                    maDH,
                    maKH,
                    maNV,
                    tongTien,
                    "edit");
    
                    f.FormClosed += (s, ev) => LoadData();
                f.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvTT.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa thanh toán này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblThanhToan WHERE MaThanhToan = '{matt}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvTT_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
 

            if (e.RowIndex < 0) return;

            string maTT = dgvTT.Rows[e.RowIndex].Cells["MaThanhToan"].Value.ToString();
            string maDH = dgvTT.Rows[e.RowIndex].Cells["MaDonHang"].Value.ToString();
            string maKH = dgvTT.Rows[e.RowIndex].Cells["MaKhachHang"].Value.ToString();
            string maNV = dgvTT.Rows[e.RowIndex].Cells["MaNhanVien"].Value.ToString();

            string tongTien = dgvTT.Rows[e.RowIndex].Cells["TongTien"].Value.ToString();

            FrmCTThanhToan f = new FrmCTThanhToan(
                maTT,
                maDH,
                maKH,
                maNV,
                tongTien,
                "edit"
            );
            f.FormClosed += (s, ev) => LoadData();
            f.ShowDialog();
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
                        MessageBox.Show($"Tìm thấy {totalRecords} thanh toán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy thanh toán nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        
        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTThanhToan frm = new FrmCTThanhToan("", "add");
            // --- Thêm: FormClosed event để reload data ---
            frm.FormClosed += (s, ev) => LoadData();
            frm.ShowDialog();
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

        private void btnLocPTTT_Click(object sender, EventArgs e)
        {
            cmsPTTT.Show(btnLocPTTT, new Point(0, btnLocPTTT.Height));
        }

        private void btnLocTTTT_Click(object sender, EventArgs e)
        {
            cmsTTTT.Show(btnLocTTTT, new Point(0, btnLocTTTT.Height));
        }

        private void tsmDaTT_Click(object sender, EventArgs e)
        {
            filterTrangThai = 1;
            pageNumber = 1;
            LoadData(currentKeyword);
        }

        private void tsmChuaTT_Click(object sender, EventArgs e)
        {
            filterTrangThai = 0;
            pageNumber = 1;
            LoadData(currentKeyword);
        }

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }

        private void btnDanhSach_Click_1(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            filterTrangThai = -1;
            filterPTTT = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }
    }
}
