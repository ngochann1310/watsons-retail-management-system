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
    public partial class FrmPhieuXuat : Form
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
        string filterDH = "";

        public FrmPhieuXuat()
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
            dgvPX.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 60;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvPX.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblPhieuXuat WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaPhieuXuat LIKE '%{keyword}%' OR MaNhanVien LIKE N'%{keyword}%' " +
                            $"OR MaDonHang LIKE N'%{keyword}%' OR MaChiNhanh LIKE N'%{keyword}%' " +
                            $"OR GhiChu LIKE N'%{keyword}%')";
            }

            // Lọc theo CN
            if (!string.IsNullOrEmpty(filterCNhanh))
            {
                sqlCount += $" AND MaChiNhanh = '{filterCNhanh}'";
            }

            // Lọc theo NCC
            if (!string.IsNullOrEmpty(filterDH))
            {
                sqlCount += $" AND MaDonHang = '{filterDH}'";
            }


            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvPX.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvPX.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy phiếu xuất nào!";
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
            FROM tblPhieuXuat
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (MaPhieuXuat LIKE '%{keyword}%' OR MaNhanVien LIKE N'%{keyword}%' " +
                            $"OR MaDonHang LIKE N'%{keyword}%' OR MaChiNhanh LIKE N'%{keyword}%' " +
                            $"OR GhiChu LIKE N'%{keyword}%')";
            }

            // Lọc theo CN
            if (!string.IsNullOrEmpty(filterCNhanh))
                query += $" AND MaChiNhanh = '{filterCNhanh}'";

            // Lọc theo NCC
            if (!string.IsNullOrEmpty(filterDH))
                query += $" AND MaDonHang = '{filterDH}'";


            // Cuối cùng thêm phân trang
            query += $@"
            ORDER BY MaPhieuXuat
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvPX.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvPX.Columns.Contains("btnSua")) dgvPX.Columns.Remove("btnSua");
            if (dgvPX.Columns.Contains("btnXoa")) dgvPX.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvPX.Columns["MaPhieuXuat"].HeaderText = "Mã phiếu xuất";
            dgvPX.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvPX.Columns["MaDonHang"].HeaderText = "Đơn hàng";
            dgvPX.Columns["MaChiNhanh"].HeaderText = "Chi nhánh";
            dgvPX.Columns["NgayXuat"].HeaderText = "Ngày xuất";
            dgvPX.Columns["TongTien"].HeaderText = "Tổng tiền";
            dgvPX.Columns["GhiChu"].HeaderText = "Ghi chú";


            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvPX.Columns)
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
                    case "MaPhieuXuat":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaNhanVien":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaDonHang":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaChiNhanh":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "NgayXuat":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "TongTien":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "GhiChu":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;
                }
            }

            // Toàn bảng
            dgvPX.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPX.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmPhieuXuat_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvPX.CellClick += dgvPX_CellClick;
            dgvPX.CellDoubleClick += dgvPX_CellDoubleClick;
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

            // Build menu lọc NCC
            BuildFilterMenu(
                cmsLocDH,
                "SELECT MaDonHang FROM tblDonHang",
                "MaDonHang",
                "MaDonHang",
                value => filterDH = value
            );

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvPX.Columns)
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
                    dgvPX.Columns[colName].Visible = item.Checked;
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

                    if (dgvPX.Columns.Contains(colName))
                    {
                        item.Checked = dgvPX.Columns[colName].Visible;
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

        private void dgvPX_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string mapx = dgvPX.Rows[e.RowIndex].Cells["MaPhieuXuat"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvPX.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmPhieuXuat_Tao frm = new FrmPhieuXuat_Tao(mapx, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvPX.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa phiếu xuất này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblPhieuXuat WHERE MaPhieuXuat = '{mapx}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy phiếu xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvPX_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string mapx = dgvPX.Rows[e.RowIndex].Cells["MaPhieuXuat"].Value.ToString();


            FrmPhieuXuat_Tao frm = new FrmPhieuXuat_Tao(mapx, "edit");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} phiếu xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy phiếu xuất nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmPhieuXuat_Tao frm = new FrmPhieuXuat_Tao("", "add");
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

        private void btnLocCN_Click(object sender, EventArgs e)
        {
            cmsLocCN.Show(btnLocCN, new Point(0, btnLocCN.Height));
        }

        private void btnLocDH_Click(object sender, EventArgs e)
        {
            cmsLocDH.Show(btnLocDH, new Point(0, btnLocDH.Height));
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
            filterDH = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }
        public Action OpenFrmCTPX;
        private void btnXem_Click(object sender, EventArgs e)
        {
            OpenFrmCTPX?.Invoke();
        }
    }
}
