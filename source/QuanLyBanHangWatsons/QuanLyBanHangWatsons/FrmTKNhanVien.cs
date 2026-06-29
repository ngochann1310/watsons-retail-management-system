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
    public partial class FrmTKNhanVien : Form
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
        string filterQuyen = "";

        public FrmTKNhanVien()
        {
            InitializeComponent();
            this.Shown += FrmTKNhanVien_Shown;
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
            dgvTKNV.Columns.Add(colSua);           

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 50;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvTKNV.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblTaiKhoanNV WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaTaiKhoanNV LIKE '%{keyword}%' " +
                            $"OR TenTaiKhoan LIKE N'%{keyword}%' " +
                            $"OR MaNhanVien LIKE N'%{keyword}%' " +
                            $"OR MatKhau LIKE N'%{keyword}%' " +
                            $"OR Quyen LIKE N'%{keyword}%')";
            }
            // Lọc theo trạng thái 
            if (filterTrangThai != -1)
            {
                sqlCount += $" AND TrangThaiTaiKhoan = {filterTrangThai}";
            }
            // Lọc theo quyền
            if (!string.IsNullOrEmpty(filterQuyen))
            {
                var roles = filterQuyen.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"Quyen LIKE N'%{r}%'"));
                sqlCount += $" AND ({likeCondition})";
            }

            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvTKNV.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvTKNV.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy tài khoản nào!";
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
            FROM tblTaiKhoanNV
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (MaTaiKhoanNV LIKE '%{keyword}%' 
                            OR TenTaiKhoan LIKE N'%{keyword}%' 
                            OR MaNhanVien LIKE N'%{keyword}%' 
                            OR MatKhau LIKE N'%{keyword}%' " +
                            $"OR Quyen LIKE N'%{keyword}%')";
            }

            // Lọc trạng thái
            if (filterTrangThai != -1)
            {
                query += $" AND TrangThaiTaiKhoan = {filterTrangThai}";
            }
            // Lọc nhiều quyền
            if (!string.IsNullOrEmpty(filterQuyen))
            {
                var roles = filterQuyen.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"Quyen LIKE N'%{r}%'"));
                query += $" AND ({likeCondition})";
            }

            // Cuối cùng thêm phân trang
            query += $@"
                    ORDER BY MaTaiKhoanNV
                    OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvTKNV.DataSource = kn.ExecuteQuery(query);

            // --- TẠO CỘT HÌNH ẢNH TỪ FILE NAME ---
            if (dgvTKNV.Columns.Contains("HinhAnhImg"))
                dgvTKNV.Columns.Remove("HinhAnhImg");

            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "HinhAnhImg";
            imgCol.HeaderText = "Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCol.Width = 55;
            imgCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            dgvTKNV.Columns.Add(imgCol);

            // Gán ảnh vào từng dòng
            foreach (DataGridViewRow row in dgvTKNV.Rows)
            {
                try
                {
                    string fileName = row.Cells["HinhAnh"].Value?.ToString();

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string fullPath = Path.Combine(Application.StartupPath, @"..\..\tool\avatar\", fileName);
                        if (File.Exists(fullPath))
                            row.Cells["HinhAnhImg"].Value = Image.FromFile(fullPath);
                    }
                }
                catch { }
            }

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvTKNV.Columns.Contains("btnSua")) dgvTKNV.Columns.Remove("btnSua");
            if (dgvTKNV.Columns.Contains("btnXoa")) dgvTKNV.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvTKNV.Columns["MaTaiKhoanNV"].HeaderText = "Mã tài khoản";
            dgvTKNV.Columns["TenTaiKhoan"].HeaderText = "Tên tài khoản";
            dgvTKNV.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvTKNV.Columns["MatKhau"].HeaderText = "Mật khẩu";
            dgvTKNV.Columns["Quyen"].HeaderText = "Quyền";
            dgvTKNV.Columns["TrangThaiTaiKhoan"].HeaderText = "Trạng thái";
            dgvTKNV.Columns["NgayCapNhat"].HeaderText = "Ngày cập nhật";

            //Ẩn cột HinhAnh gốc
            dgvTKNV.Columns["HinhAnh"].Visible = false;


            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvTKNV.Columns)
            {
                // luôn căn giữa tiêu đề
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (col.Name == "HinhAnhImg")  // thêm dòng này
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    continue;
                }

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
                    case "MaTaiKhoanNV":
                        col.FillWeight = 6;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "TenTaiKhoan":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaNhanVien":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MatKhau":
                        col.FillWeight = 6;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "Quyen":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "TrangThaiTaiKhoan":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "NgayCapNhat":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                }
            }

            // Định dạng cột 'NgayDatHang' để chỉ hiển thị ngày tháng năm
            dgvTKNV.Columns["NgayCapNhat"].DefaultCellStyle.Format = "dd/MM/yyyy";


            // Toàn bảng
            dgvTKNV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTKNV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmTKNhanVien_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvTKNV.CellClick += dgvTKNV_CellClick;
            dgvTKNV.CellDoubleClick += dgvTKNV_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            tsmDangHD.Click += tsmDangHD_Click;
            tsmVoHieuHoa.Click += tsmVoHieuHoa_Click;

            // Build menu lọc quyền
            BuildFilterMenu();

            //LoadData();
            //TaoMenuCot();
        }
        private void FrmTKNhanVien_Shown(object sender, EventArgs e)
        {
            LoadData();
            TaoMenuCot();
        }


        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvTKNV.Columns)
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
                    dgvTKNV.Columns[colName].Visible = item.Checked;
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

                    if (dgvTKNV.Columns.Contains(colName))
                    {
                        item.Checked = dgvTKNV.Columns[colName].Visible;
                    }
                }
            }
        }

        private void BuildFilterMenu()
        {
            cmsLocQuyen.Items.Clear();

            string[] dsQuyen = { "Nhân viên bán hàng", "Nhân viên kho", "Quản lý cửa hàng", "Quản lý kho", "Quản trị viên" };

            foreach (var q in dsQuyen)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(q);
                item.Tag = q;
                item.CheckOnClick = true; // Cho phép check/uncheck nhiều cái

                item.Click += (s, e) =>
                {
                    // Lấy tất cả quyền đang được check
                    var selectedRoles = cmsLocQuyen.Items
                        .OfType<ToolStripMenuItem>()
                        .Where(m => m.Checked)
                        .Select(m => m.Tag.ToString())
                        .ToList();

                    // Gán filter = danh sách quyền (cách nhau dấu phẩy)
                    filterQuyen = string.Join(",", selectedRoles);

                    pageNumber = 1;
                    LoadData(currentKeyword);
                };

                cmsLocQuyen.Items.Add(item);
            }
        }

        private void dgvTKNV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string matk = dgvTKNV.Rows[e.RowIndex].Cells["MaTaiKhoanNV"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvTKNV.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTTKNhanVien frm = new FrmCTTKNhanVien(matk, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvTKNV.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa tài khoản này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblTaiKhoanNV WHERE MaTaiKhoanNV = '{matk}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy tài khoản.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvTKNV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string matk = dgvTKNV.Rows[e.RowIndex].Cells["MaTaiKhoanNV"].Value.ToString();

            FrmCTTKNhanVien frm = new FrmCTTKNhanVien(matk, "edit");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} tài khoản.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy tài khoản nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTTKNhanVien frm = new FrmCTTKNhanVien("", "add");
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

        private void btnLocTTTK_Click(object sender, EventArgs e)
        {
            cmsTTTK.Show(btnLocTTTK, new Point(0, btnLocTTTK.Height));
        }

        private void tsmDangHD_Click(object sender, EventArgs e)
        {
            filterTrangThai = 1;
            pageNumber = 1;
            LoadData(currentKeyword);
        }

        private void tsmVoHieuHoa_Click(object sender, EventArgs e)
        {
            filterTrangThai = 0;
            pageNumber = 1;
            LoadData(currentKeyword);
        }

        private void btnLocQuyen_Click(object sender, EventArgs e)
        {
            cmsLocQuyen.Show(btnLocQuyen, new Point(0, btnLocQuyen.Height));
        }

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }

        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            filterTrangThai = -1;
            filterQuyen = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }

        private void btnFrmNV_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiParent.MdiChildren)
            {
                if (frm is FrmNhanVien)
                {
                    frm.Activate();
                    this.Close();
                    return;
                }
            }

            // Nếu chưa mở thì mở mới
            FrmNhanVien f = new FrmNhanVien();
            f.MdiParent = this.MdiParent;
            f.Dock = DockStyle.Fill;
            f.Show();

            this.Close();
        }
    }
}
