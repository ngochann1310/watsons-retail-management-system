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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.IO;

namespace QuanLyBanHangWatsons
{
    public partial class FrmDonHang : Form
    {
        Ket_noi kn = new Ket_noi();

        // Biến phân trang toàn cục
        int pageSize = 10;        // số dòng mỗi trang
        int pageNumber = 1;       // trang hiện tại
        int totalRecords = 0;     // tổng số dòng
        int totalPages = 0;       // tổng số trang

        // --- Thêm: biến lưu keyword hiện tại ---
        private string currentKeyword = "";
        string filterCNhanh = "";
        string filterLoaiDH = "";
        string filterTTDH = "";


        public FrmDonHang()
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
            dgvDH.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 60;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvDH.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblDonHang WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaDonHang LIKE '%{keyword}%' OR MaNhanVien LIKE N'%{keyword}%' " +
                            $"OR MaKhachHang LIKE N'%{keyword}%' OR MaChiNhanh LIKE N'%{keyword}%' " +
                            $"OR MaKhuyenMai LIKE N'%{keyword}%')";
            }

            // Lọc theo CN
            if (!string.IsNullOrEmpty(filterCNhanh))
            {
                sqlCount += $" AND MaChiNhanh = '{filterCNhanh}'";
            }
          
            // Lọc theo loại DH
            if (!string.IsNullOrEmpty(filterLoaiDH))
            {
                var roles = filterLoaiDH.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"LoaiDonHang LIKE N'%{r}%'"));
                sqlCount += $" AND ({likeCondition})";
            }
            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(filterTTDH))
            {
                var roles = filterTTDH.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"TrangThaiDonHang LIKE N'%{r}%'"));
                sqlCount += $" AND ({likeCondition})";
            }


            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvDH.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvDH.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy đơn hàng nào!";
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
            SELECT dh.*, km.GiaTri
            FROM tblDonHang dh
            LEFT JOIN tblKhuyenMai km ON dh.MaKhuyenMai = km.MaKhuyenMai
            WHERE 1 = 1
            ";

            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@"
                AND (
                    dh.MaDonHang LIKE '%{keyword}%'
                    OR dh.MaNhanVien LIKE N'%{keyword}%'
                    OR dh.MaKhachHang LIKE N'%{keyword}%'
                    OR dh.MaChiNhanh LIKE N'%{keyword}%'
                    OR dh.MaKhuyenMai LIKE N'%{keyword}%'
                    OR dh.LoaiDonHang LIKE N'%{keyword}%'
                    OR dh.TrangThaiDonHang LIKE N'%{keyword}%'
                )";
            }

            // Lọc theo CN
            if (!string.IsNullOrEmpty(filterCNhanh))
                query += $" AND MaChiNhanh = '{filterCNhanh}'";

            // Lọc nhiều loại đơn hàng
            if (!string.IsNullOrEmpty(filterLoaiDH))
            {
                var roles = filterLoaiDH.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"LoaiDonHang LIKE N'%{r}%'"));
                query += $" AND ({likeCondition})";
            }

            // Lọc nhiều loại trạng thái
            if (!string.IsNullOrEmpty(filterTTDH)) 
            {
                var roles = filterTTDH.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"TrangThaiDonHang LIKE N'%{r}%'"));
                query += $" AND ({likeCondition})";
            }


            // Cuối cùng thêm phân trang
            query += $@"
            ORDER BY MaDonHang
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            DataTable dt = kn.ExecuteQuery(query);

            // --- Tính ThanhTien trực tiếp, phân biệt tiền / % khuyến mãi ---
            foreach (DataRow row in dt.Rows)
            {
                decimal tongTienHang = row["TongTienHang"] != DBNull.Value ? Convert.ToDecimal(row["TongTienHang"]) : 0;
                decimal giaTriKM = row["GiaTri"] != DBNull.Value ? Convert.ToDecimal(row["GiaTri"]) : 0;

                decimal thanhTien = tongTienHang;

                // Nếu giá trị khuyến mãi > 0 mới áp dụng
                if (giaTriKM > 0)
                {
                    if (giaTriKM <= 1) // coi là % giảm
                        thanhTien = tongTienHang - tongTienHang * giaTriKM;
                    else // coi là tiền giảm
                        thanhTien = tongTienHang - giaTriKM;
                }

                row["ThanhTien"] = Math.Round(thanhTien < 0 ? 0 : thanhTien, 2);
            }

            dgvDH.DataSource = dt;

            // --- Ẩn cột phụ không cần thiết ---
            if (dgvDH.Columns.Contains("GiaTri")) dgvDH.Columns["GiaTri"].Visible = false;


            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvDH.Columns.Contains("btnSua")) dgvDH.Columns.Remove("btnSua");
            if (dgvDH.Columns.Contains("btnXoa")) dgvDH.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvDH.Columns["MaDonHang"].HeaderText = "Mã đơn hàng";
            dgvDH.Columns["MaKhachHang"].HeaderText = "Khách hàng";
            dgvDH.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvDH.Columns["LoaiDonHang"].HeaderText = "Loại đơn hàng";
            dgvDH.Columns["NgayDatHang"].HeaderText = "Ngày đặt hàng";
            dgvDH.Columns["TrangThaiDonHang"].HeaderText = "Trạng thái";
            dgvDH.Columns["TongTienHang"].HeaderText = "Tổng tiền hàng";
            dgvDH.Columns["MaKhuyenMai"].HeaderText = "Khuyến mãi";
            dgvDH.Columns["DiemSuDung"].HeaderText = "Điểm sử dụng";
            dgvDH.Columns["ThanhTien"].HeaderText = "Thành tiền";
            dgvDH.Columns["MaChiNhanh"].HeaderText = "Chi nhánh";
            dgvDH.Columns["GhiChu"].HeaderText = "Ghi chú";


            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvDH.Columns)
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
                    case "MaDonHang":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaKhachHang":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaNhanVien":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "LoaiDonHang":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "NgayDatHang":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "TrangThaiDonHang":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "TongTienHang":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaKhuyenMai":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "DiemSuDung":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "ThanhTien":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaChiNhanh":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "GhiChu":
                        col.FillWeight = 4;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;
                }
            }

            // Toàn bảng
            dgvDH.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDH.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmDonHang_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvDH.CellClick += dgvDH_CellClick;
            dgvDH.CellDoubleClick += dgvDH_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            // Build menu lọc CN
            BuildFilterMenu(
                cmsLocCN,
                "SELECT MaChiNhanh, TenChiNhanh FROM tblChiNhanh",
                "TenChiNhanh",     // tên hiển thị
                "MaChiNhanh",      // mã ẩn
                value => filterCNhanh = value
            );

            BuildFilterMenuLoaiDH();
            BuildFilterMenuTTDH();

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvDH.Columns)
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
                    dgvDH.Columns[colName].Visible = item.Checked;
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

                    if (dgvDH.Columns.Contains(colName))
                    {
                        item.Checked = dgvDH.Columns[colName].Visible;
                    }
                }
            }
        }

        private void BuildFilterMenu(
        ContextMenuStrip menu,
        string sql,
        string displayField,
        string valueField,
        Action<string> onFilterChange)
        {
            menu.Items.Clear();

            // Mục đầu tiên: TẤT CẢ
            ToolStripMenuItem allItem = new ToolStripMenuItem("Tất cả");
            allItem.Tag = "";       // filter rỗng
            allItem.Checked = true; // mặc định
            allItem.Click += (s, e) =>
            {
                foreach (ToolStripMenuItem item in menu.Items)
                    item.Checked = false;

                allItem.Checked = true;
                onFilterChange("");
                LoadData();
            };
            menu.Items.Add(allItem);

            // Load dữ liệu từ SQL (Tên hiển thị – Mã ẩn)
            DataTable dt = kn.ExecuteQuery(sql);

            foreach (DataRow row in dt.Rows)
            {
                string text = row[displayField].ToString();
                string value = row[valueField].ToString();

                ToolStripMenuItem item = new ToolStripMenuItem(text);
                item.Tag = value;

                item.Click += (s, e) =>
                {
                    foreach (ToolStripMenuItem i in menu.Items)
                        i.Checked = false;

                    item.Checked = true;

                    onFilterChange(value);
                    LoadData();
                };

                menu.Items.Add(item);
            }
        }

        private void BuildFilterMenuLoaiDH()
        {
            cmsLocLoaiDH.Items.Clear();

            string[] dsLoaiDH = { "Tại cửa hàng", "Trực tuyến" };

            foreach (var q in dsLoaiDH)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(q);
                item.Tag = q;
                item.CheckOnClick = true; // Cho phép check/uncheck nhiều cái

                item.Click += (s, e) =>
                {
                    // Lấy tất cả quyền đang được check
                    var selectedRoles = cmsLocLoaiDH.Items
                        .OfType<ToolStripMenuItem>()
                        .Where(m => m.Checked)
                        .Select(m => m.Tag.ToString())
                        .ToList();

                    // Gán filter = danh sách quyền (cách nhau dấu phẩy)
                    filterLoaiDH = string.Join(",", selectedRoles);

                    pageNumber = 1;
                    LoadData(currentKeyword);
                };

                cmsLocLoaiDH.Items.Add(item);
            }
        }

        private void BuildFilterMenuTTDH()
        {
            cmsLocTTDH.Items.Clear();

            string[] dsTTDH = { "Chờ xác nhận", "Đã xác nhận", "Chờ lấy hàng",
                                "Đã lấy hàng", "Đang giao hàng", 
                                "Giao hàng thành công", "Giao hàng thất bại"};

            foreach (var q in dsTTDH)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(q);
                item.Tag = q;
                item.CheckOnClick = true; // Cho phép check/uncheck nhiều cái

                item.Click += (s, e) =>
                {
                    // Lấy tất cả quyền đang được check
                    var selectedRoles = cmsLocTTDH.Items
                        .OfType<ToolStripMenuItem>()
                        .Where(m => m.Checked)
                        .Select(m => m.Tag.ToString())
                        .ToList();

                    // Gán filter = danh sách quyền (cách nhau dấu phẩy)
                    filterTTDH = string.Join(",", selectedRoles);

                    pageNumber = 1;
                    LoadData(currentKeyword);
                };

                cmsLocTTDH.Items.Add(item);
            }
        }

        private void dgvDH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string madh = dgvDH.Rows[e.RowIndex].Cells["MaDonHang"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvDH.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmDonHang_TaoDon frm = new FrmDonHang_TaoDon(madh, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvDH.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa đơn hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblDonHang WHERE MaDonHang = '{madh}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy đơn hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvDH_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string madh = dgvDH.Rows[e.RowIndex].Cells["MaDonHang"].Value.ToString();


            FrmDonHang_TaoDon frm = new FrmDonHang_TaoDon(madh, "edit");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} đơn hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy đơn hàng nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmDonHang_TaoDon frm = new FrmDonHang_TaoDon("", "add");
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

        private void btnLocLoaiDH_Click(object sender, EventArgs e)
        {
            cmsLocLoaiDH.Show(btnLocLoaiDH, new Point(0, btnLocLoaiDH.Height));
        }

        private void btnLocTTDH_Click(object sender, EventArgs e)
        {
            cmsLocTTDH.Show(btnLocTTDH, new Point(0, btnLocTTDH.Height));
        }

        private void btnLocCN_Click(object sender, EventArgs e)
        {
            cmsLocCN.Show(btnLocCN, new Point(0, btnLocCN.Height));
        }

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }

        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            filterCNhanh = "";
            filterTTDH = "";
            filterLoaiDH = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }
        public Action OpenFrmCTDonHang;
        private void btnXem_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiParent.MdiChildren)
            {
                if (frm is FrmCTDonHang)
                {
                    frm.Activate();
                    this.Close();
                    return;
                }
            }

            // Nếu chưa mở thì mở mới
            FrmCTDonHang f = new FrmCTDonHang();
            f.MdiParent = this.MdiParent;
            f.Dock = DockStyle.Fill;
            f.Show();

            this.Close();
        }
    }
}
