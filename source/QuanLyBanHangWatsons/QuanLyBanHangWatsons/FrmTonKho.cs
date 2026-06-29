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
    public partial class FrmTonKho : Form
    {
        Ket_noi kn = new Ket_noi();

        // Biến phân trang toàn cục
        int pageSize = 10;        // số dòng mỗi trang
        int pageNumber = 1;       // trang hiện tại
        int totalRecords = 0;     // tổng số dòng
        int totalPages = 0;       // tổng số trang

        // --- Thêm: biến lưu keyword hiện tại ---
        private string currentKeyword = "";
        string filterChiNhanh = "";

        public FrmTonKho()
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
            colSua.Width = 70;
            colSua.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvTK.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 70;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvTK.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblTonKho WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaTonKho LIKE '%{keyword}%' " +
                            $"OR MaSanPham LIKE N'%{keyword}%' " +
                            $"OR MaNhanVien LIKE N'%{keyword}%' )";
            }
            // Lọc theo chi nhánh
            if (!string.IsNullOrEmpty(filterChiNhanh))
            {
                sqlCount += $" AND MaChiNhanh = '{filterChiNhanh}'";
            }

             totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvTK.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvTK.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy bản ghi tồn kho nào!";
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
            FROM tblTonKho
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (MaTonKho LIKE '%{keyword}%' 
                            OR MaSanPham LIKE N'%{keyword}%' 
                            OR MaNhanVien LIKE N'%{keyword}%' " +
                            $"OR MaChiNhanh LIKE N'%{keyword}%')";
            }


            // Lọc chi nhánh
            if (!string.IsNullOrEmpty(filterChiNhanh))
            {
                query += $" AND MaChiNhanh = '{filterChiNhanh}'";
            }
            

            // Cuối cùng thêm phân trang
            query += $@"
                    ORDER BY MaNhanVien
                    OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvTK.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvTK.Columns.Contains("btnSua")) dgvTK.Columns.Remove("btnSua");
            if (dgvTK.Columns.Contains("btnXoa")) dgvTK.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvTK.Columns["MaTonKho"].HeaderText = "Mã tồn kho";
            dgvTK.Columns["MaSanPham"].HeaderText = "Sản phẩm";
            dgvTK.Columns["MaChiNhanh"].HeaderText = "Chi nhánh";
            dgvTK.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvTK.Columns["SoLuongSanBan"].HeaderText = "Số lượng sẵn bán";
            dgvTK.Columns["SoLuongTon"].HeaderText = "Số lượng tồn";           
            dgvTK.Columns["NgayCapNhat"].HeaderText = "Ngày cập nhật";

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvTK.Columns)
            {
                // luôn căn giữa tiêu đề
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Nếu là cột nút thì bỏ qua (không FillWeight)
                if (col.Name == "btnSua" || col.Name == "btnXoa")
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    continue;
                }

                // Các cột text bình thường
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                switch (col.Name)
                {
                    case "MaTonKho":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaSanPham":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaChiNhanh":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaNhanVien":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "SoLuongSanBan":
                        col.FillWeight = 6;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "SoLuongTon":
                        col.FillWeight = 6;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                 
                    case "NgayCapNhat":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                }
            }

            // Định dạng cột 'NgayDatHang' để chỉ hiển thị ngày tháng năm
            dgvTK.Columns["NgayCapNhat"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Toàn bảng
            dgvTK.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTK.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmTonKho_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvTK.CellClick += dgvTK_CellClick;
            dgvTK.CellDoubleClick += dgvTK_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            // Build menu lọc chi nhánh
            BuildFilterMenu(
                cmsLocCN,
                "SELECT MaChiNhanh, TenChiNhanh FROM tblChiNhanh",
                "TenChiNhanh",     // tên hiển thị
                "MaChiNhanh",      // mã ẩn
                value => filterChiNhanh = value
            );

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvTK.Columns)
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
                    dgvTK.Columns[colName].Visible = item.Checked;
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

                    if (dgvTK.Columns.Contains(colName))
                    {
                        item.Checked = dgvTK.Columns[colName].Visible;
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

        private void dgvTK_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string makho = dgvTK.Rows[e.RowIndex].Cells["MaTonKho"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvTK.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTTonKho frm = new FrmCTTonKho(makho, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvTK.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa bản ghi tồn kho này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblTonKho WHERE MaTonKho = '{makho}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy bản ghi tồn kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvTK_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string makho = dgvTK.Rows[e.RowIndex].Cells["MaTonKho"].Value.ToString();

            FrmCTTonKho frm = new FrmCTTonKho(makho, "edit");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} bản ghi tồn kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy bản ghi tồn kho nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTTonKho frm = new FrmCTTonKho("", "add");
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

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }

        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            filterChiNhanh = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }
    }
}
