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
    public partial class FrmKhachHang : Form
    {
        Ket_noi kn = new Ket_noi();

        // Biến phân trang toàn cục
        int pageSize = 10;        // số dòng mỗi trang
        int pageNumber = 1;       // trang hiện tại
        int totalRecords = 0;     // tổng số dòng
        int totalPages = 0;       // tổng số trang

        // --- Thêm: biến lưu keyword hiện tại ---
        private string currentKeyword = "";
        string filterLoaiKH = "";
        string filterHangTV= "";

        public FrmKhachHang()
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
            dgvKH.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 60;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvKH.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblKhachHang WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaKhachHang LIKE '%{keyword}%' " +
                            $"OR HoKhachHang LIKE N'%{keyword}%' " +
                            $"OR TenKhachHang LIKE N'%{keyword}%' " +
                            $"OR LoaiKhachHang LIKE N'%{keyword}%' " +
                            $"OR GioiTinh LIKE N'%{keyword}%' " +
                            $"OR DiaChi LIKE N'%{keyword}%'" +
                            $"OR HangThanhVien LIKE N'%{keyword}%'" +
                            $"OR MaNhanVien LIKE N'%{keyword}%')";
            }
            
            // Lọc theo loại KH
            if (!string.IsNullOrEmpty(filterLoaiKH))
            {
                var roles = filterLoaiKH.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"LoaiKhachHang LIKE N'%{r}%'"));
                sqlCount += $" AND ({likeCondition})";
            }
            // Lọc theo hạng TV
            if (!string.IsNullOrEmpty(filterHangTV))
            {
                var roles = filterHangTV.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"HangThanhVien LIKE N'%{r}%'"));
                sqlCount += $" AND ({likeCondition})";
            }


            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvKH.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvKH.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy khách hàng nào!";
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
            FROM tblKhachHang
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (MaKhachHang LIKE '%{keyword}%' 
                            OR HoKhachHang LIKE N'%{keyword}%' 
                            OR TenKhachHang LIKE N'%{keyword}%' 
                            OR LoaiKhachHang LIKE N'%{keyword}%'
                            OR GioiTinh LIKE N'%{keyword}%'
                            OR DiaChi LIKE N'%{keyword}%' " +
                            $"OR LoaiKhachHang LIKE N'%{keyword}%'" +
                            $"OR HangThanhVien LIKE N'%{keyword}%')";
            }
           
            // Lọc nhiều loại khách hàng
            if (!string.IsNullOrEmpty(filterLoaiKH))
            {
                var roles = filterLoaiKH.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"LoaiKhachHang LIKE N'%{r}%'"));
                query += $" AND ({likeCondition})";
            }
            // Lọc nhiều hạng thành viên
            if (!string.IsNullOrEmpty(filterHangTV))
            {
                var roles = filterHangTV.Split(',');
                string likeCondition = string.Join(" OR ", roles.Select(r => $"HangThanhVien LIKE N'%{r}%'"));
                query += $" AND ({likeCondition})";
            }

            // Cuối cùng thêm phân trang
            query += $@"
                    ORDER BY MaKhachHang
                    OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvKH.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvKH.Columns.Contains("btnSua")) dgvKH.Columns.Remove("btnSua");
            if (dgvKH.Columns.Contains("btnXoa")) dgvKH.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvKH.Columns["MaKhachHang"].HeaderText = "Mã khách hàng";
            dgvKH.Columns["HoKhachHang"].HeaderText = "Họ khách hàng";
            dgvKH.Columns["TenKhachHang"].HeaderText = "Tên khách hàng";
            dgvKH.Columns["LoaiKhachHang"].HeaderText = "Loại khách hàng";
            dgvKH.Columns["NgaySinh"].HeaderText = "Ngày sinh";
            dgvKH.Columns["GioiTinh"].HeaderText = "Giới tính";
            dgvKH.Columns["DienThoai"].HeaderText = "Điện thoại";
            dgvKH.Columns["DiaChi"].HeaderText = "Địa chỉ";
            dgvKH.Columns["NgayDangKy"].HeaderText = "Ngày đăng ký";
            dgvKH.Columns["DiemTichLuy"].HeaderText = "Điểm tích lũy";
            dgvKH.Columns["HangThanhVien"].HeaderText = "Hạng thành viên";
            dgvKH.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvKH.Columns["NgayCapNhat"].HeaderText = "Ngày cập nhật";

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvKH.Columns)
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
                    case "MaKhachHang":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "HoKhachHang":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;

                    case "TenKhachHang":
                        col.FillWeight = 10;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "LoaiKhachHang":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "NgaySinh":
                        col.FillWeight = 6;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "GioiTinh":
                        col.FillWeight = 4;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "DienThoai":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "DiaChi":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "Email":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                   
                    case "NgayDangKy":
                        col.FillWeight = 6;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                 
                    case "DiemTichLuy":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "HangThanhVien":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "MaNhanVien":
                        col.FillWeight = 5;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break; 

                    case "NgayCapNhat":
                        col.FillWeight = 6;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;
                }
            }

            // Định dạng cột 'NgayDatHang' để chỉ hiển thị ngày tháng năm
            dgvKH.Columns["NgayCapNhat"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvKH.Columns["NgaySinh"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvKH.Columns["NgayDangKy"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Toàn bảng
            dgvKH.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKH.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmKhachHang_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvKH.CellClick += dgvKH_CellClick;
            dgvKH.CellDoubleClick += dgvKH_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            BuildFilterMenuLoaiKH();
            BuildFilterMenu1HangTV();

            LoadData();
            TaoMenuCot();
        }       

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvKH.Columns)
            {
                // loại khỏi menu: nút chức năng
                if (col.Name == "btnSua" || col.Name == "btnXoa") continue;

                ToolStripMenuItem item = new ToolStripMenuItem(col.HeaderText);
                item.CheckOnClick = true;
                item.Checked = col.Visible;
                item.Tag = col.Name;   // liên kết item → column

                item.Click += (s, e) =>
                {
                    string colName = item.Tag.ToString();
                    dgvKH.Columns[colName].Visible = item.Checked;
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

                    if (dgvKH.Columns.Contains(colName))
                    {
                        item.Checked = dgvKH.Columns[colName].Visible;
                    }
                }
            }
        }

        private void BuildFilterMenuLoaiKH()
        {
            cmsLocLoaiKH.Items.Clear();

            string[] dsLoaiKH = { "Thành viên", "Vãng lai" };

            foreach (var q in dsLoaiKH)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(q);
                item.Tag = q;
                item.CheckOnClick = true; // Cho phép check/uncheck nhiều cái

                item.Click += (s, e) =>
                {
                    // Lấy tất cả quyền đang được check
                    var selectedRoles = cmsLocLoaiKH.Items
                        .OfType<ToolStripMenuItem>()
                        .Where(m => m.Checked)
                        .Select(m => m.Tag.ToString())
                        .ToList();

                    // Gán filter = danh sách quyền (cách nhau dấu phẩy)
                    filterLoaiKH = string.Join(",", selectedRoles);

                    pageNumber = 1;
                    LoadData(currentKeyword);
                };

                cmsLocLoaiKH.Items.Add(item);
            }
        }

        private void BuildFilterMenu1HangTV()
        {
            cmsLocHangTV.Items.Clear();

            string[] dsHangTV = { "Watsons Club", "Watsons Club Elite" };

            foreach (var q in dsHangTV)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(q);
                item.Tag = q;
                item.CheckOnClick = true; // Cho phép check/uncheck nhiều cái

                item.Click += (s, e) =>
                {
                    // Lấy tất cả quyền đang được check
                    var selectedRoles = cmsLocHangTV.Items
                        .OfType<ToolStripMenuItem>()
                        .Where(m => m.Checked)
                        .Select(m => m.Tag.ToString())
                        .ToList();

                    // Gán filter = danh sách quyền (cách nhau dấu phẩy)
                    filterHangTV = string.Join(",", selectedRoles);

                    pageNumber = 1;
                    LoadData(currentKeyword);
                };

                cmsLocHangTV.Items.Add(item);
            }
        }

        private void dgvKH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string makh = dgvKH.Rows[e.RowIndex].Cells["MaKhachHang"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvKH.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTKhachHang frm = new FrmCTKhachHang(makh, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvKH.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblKhachHang WHERE MaKhachHang = '{makh}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy khách hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvKH_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            string makh = dgvKH.Rows[e.RowIndex].Cells["MaKhachHang"].Value.ToString();

            FrmCTKhachHang frm = new FrmCTKhachHang(makh, "edit");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} khách hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy khách hàng nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTKhachHang frm = new FrmCTKhachHang("", "add");
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

        private void btnLocLoaiKH_Click(object sender, EventArgs e)
        {
            cmsLocLoaiKH.Show(btnLocLoaiKH, new Point(0, btnLocLoaiKH.Height));
        }

        private void btnLocHangTV_Click(object sender, EventArgs e)
        {
            cmsLocHangTV.Show(btnLocHangTV, new Point(0, btnLocHangTV.Height));
        }

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }

        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            filterLoaiKH = "";
            filterHangTV = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }

        public Action OpenFrmTKKhachHang;

        private void btnFrmTKKH_Click(object sender, EventArgs e)
        {
            OpenFrmTKKhachHang?.Invoke();
        }
    }
}
