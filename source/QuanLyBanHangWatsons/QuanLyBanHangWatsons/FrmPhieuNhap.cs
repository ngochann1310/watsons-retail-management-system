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
    public partial class FrmPhieuNhap : Form
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
        string filterNCC = "";

        public FrmPhieuNhap()
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
            dgvPN.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 60;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvPN.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblPhieuNhap WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaPhieuNhap LIKE '%{keyword}%' OR MaNhanVien LIKE N'%{keyword}%' " +
                            $"OR MaNhaCungCap LIKE N'%{keyword}%' OR MaChiNhanh LIKE N'%{keyword}%' " +
                            $"OR GhiChu LIKE N'%{keyword}%')";
            }

            // Lọc theo CN
            if (!string.IsNullOrEmpty(filterCNhanh))
            {
                sqlCount += $" AND MaChiNhanh = '{filterCNhanh}'";
            }

            // Lọc theo NCC
            if (!string.IsNullOrEmpty(filterNCC))
            {
                sqlCount += $" AND MaNhaCungCap = '{filterNCC}'";
            }


            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvPN.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvPN.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy phiếu nhập nào!";
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
            FROM tblPhieuNhap
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (MaPhieuNhap LIKE '%{keyword}%' OR MaNhanVien LIKE N'%{keyword}%' 
                             OR MaNhaCungCap LIKE N'%{keyword}%' OR MaChiNhanh LIKE N'%{keyword}%' 
                             OR GhiChu LIKE N'%{keyword}%')";
            }
          
            // Lọc theo CN
            if (!string.IsNullOrEmpty(filterCNhanh))
                query += $" AND MaChiNhanh = '{filterCNhanh}'";

            // Lọc theo NCC
            if (!string.IsNullOrEmpty(filterNCC))
                query += $" AND MaNhaCungCap = '{filterNCC}'";


            // Cuối cùng thêm phân trang
            query += $@"
            ORDER BY MaPhieuNhap
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvPN.DataSource = kn.ExecuteQuery(query);            

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvPN.Columns.Contains("btnSua")) dgvPN.Columns.Remove("btnSua");
            if (dgvPN.Columns.Contains("btnXoa")) dgvPN.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvPN.Columns["MaPhieuNhap"].HeaderText = "Mã phiếu nhập";
            dgvPN.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvPN.Columns["MaNhaCungCap"].HeaderText = "Nhà cung cấp";
            dgvPN.Columns["MaChiNhanh"].HeaderText = "Chi nhánh";
            dgvPN.Columns["NgayNhap"].HeaderText = "Ngày nhập";
            dgvPN.Columns["TongTien"].HeaderText = "Tổng tiền";
            dgvPN.Columns["GhiChu"].HeaderText = "Ghi chú";


            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvPN.Columns)
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
                    case "MaPhieuNhap":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaNhanVien":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaNhaCungCap":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "MaChiNhanh":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                    case "NgayNhap":
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
            dgvPN.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPN.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmPhieuNhap_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvPN.CellClick += dgvPN_CellClick;
            dgvPN.CellDoubleClick += dgvPN_CellDoubleClick;
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
                cmsLocNCC,
                "SELECT MaNhaCungCap, TenNhaCungCap FROM tblNhaCungCap",
                "TenNhaCungCap",      // tên hiển thị
                "MaNhaCungCap",       // mã ẩn
                value => filterNCC = value
            );

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvPN.Columns)
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
                    dgvPN.Columns[colName].Visible = item.Checked;
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

                    if (dgvPN.Columns.Contains(colName))
                    {
                        item.Checked = dgvPN.Columns[colName].Visible;
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

        private void dgvPN_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string mapn = dgvPN.Rows[e.RowIndex].Cells["MaPhieuNhap"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvPN.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmPhieuNhap_Tao frm = new FrmPhieuNhap_Tao(mapn, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvPN.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa phiếu nhập này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblPhieuNhap WHERE MaPhieuNhap = '{mapn}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvPN_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string mapn = dgvPN.Rows[e.RowIndex].Cells["MaPhieuNhap"].Value.ToString();


            FrmPhieuNhap_Tao frm = new FrmPhieuNhap_Tao(mapn, "edit");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} phiếu nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy phiếu nhập nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmPhieuNhap_Tao frm = new FrmPhieuNhap_Tao("", "add");
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

        private void btnLocNCC_Click(object sender, EventArgs e)
        {
            cmsLocNCC.Show(btnLocNCC, new Point(0, btnLocNCC.Height));
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
            filterNCC = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }

        public Action OpenFrmCTPN;

        private void btnXem_Click(object sender, EventArgs e)
        {
            OpenFrmCTPN?.Invoke();
        }
    }
}
