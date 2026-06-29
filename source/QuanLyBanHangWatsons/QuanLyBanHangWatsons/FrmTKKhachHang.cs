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
    public partial class FrmTKKhachHang : Form
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

        public FrmTKKhachHang()
        {
            InitializeComponent();
            this.Shown += FrmTKKhachHang_Shown;
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
            dgvTKKH.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 50;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvTKKH.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblTaiKhoanKH WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaTaiKhoanKH LIKE '%{keyword}%' " +
                    $"OR TenTaiKhoan LIKE N'%{keyword}%' " +
                    $"OR MaKhachHang LIKE N'%{keyword}%' " +
                    $"OR MatKhau LIKE N'%{keyword}%' " +
                    $"OR Quyen LIKE N'%{keyword}%')";
            }
            // Lọc theo trạng thái 
            if (filterTrangThai != -1)
            {
                sqlCount += $" AND TrangThaiTaiKhoan = {filterTrangThai}";
            }
            
            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvTKKH.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvTKKH.DataSource = null; // xóa DataGridView cũ
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
            FROM tblTaiKhoanKH
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (MaTaiKhoanKH LIKE '%{keyword}%' 
                            OR TenTaiKhoan LIKE N'%{keyword}%' 
                            OR MaKhachHang LIKE N'%{keyword}%' 
                            OR MatKhau LIKE N'%{keyword}%' " +
                            $"OR Quyen LIKE N'%{keyword}%')";
            }

            // Lọc trạng thái
            if (filterTrangThai != -1)
            {
                query += $" AND TrangThaiTaiKhoan = {filterTrangThai}";
            }

            // Cuối cùng thêm phân trang
            query += $@"
            ORDER BY MaTaiKhoanKH
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvTKKH.DataSource = kn.ExecuteQuery(query);

            // --- TẠO CỘT HÌNH ẢNH TỪ FILE NAME ---
            if (dgvTKKH.Columns.Contains("HinhAnhImg"))
                dgvTKKH.Columns.Remove("HinhAnhImg");

            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "HinhAnhImg";
            imgCol.HeaderText = "Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCol.Width = 55;
            imgCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            dgvTKKH.Columns.Add(imgCol);

            // Gán ảnh vào từng dòng
            foreach (DataGridViewRow row in dgvTKKH.Rows)
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
            if (dgvTKKH.Columns.Contains("btnSua")) dgvTKKH.Columns.Remove("btnSua");
            if (dgvTKKH.Columns.Contains("btnXoa")) dgvTKKH.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvTKKH.Columns["MaTaiKhoanKH"].HeaderText = "Mã tài khoản";
            dgvTKKH.Columns["TenTaiKhoan"].HeaderText = "Tên tài khoản";
            dgvTKKH.Columns["MaKhachHang"].HeaderText = "Mã khách hàng";
            dgvTKKH.Columns["MatKhau"].HeaderText = "Mật khẩu";
            dgvTKKH.Columns["Quyen"].HeaderText = "Quyền";
            dgvTKKH.Columns["TrangThaiTaiKhoan"].HeaderText = "Trạng thái";
            dgvTKKH.Columns["NgayCapNhat"].HeaderText = "Ngày cập nhật";

            //Ẩn cột HinhAnh gốc
            dgvTKKH.Columns["HinhAnh"].Visible = false;

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvTKKH.Columns)
            {
                // Header luôn center
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
                        case "MaTaiKhoanKH":
                            col.FillWeight = 6;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "TenTaiKhoan":
                            col.FillWeight = 5;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "MaKhachHang":
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
            dgvTKKH.Columns["NgayCapNhat"].DefaultCellStyle.Format = "dd/MM/yyyy";


            // Toàn bảng
            dgvTKKH.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTKKH.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        
        }

        private void FrmTKKhachHang_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvTKKH.CellClick += dgvTKKH_CellClick;
            dgvTKKH.CellDoubleClick += dgvTKKH_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            tsmDangHD.Click += tsmDangHD_Click;
            tsmVoHieuHoa.Click += tsmVoHieuHoa_Click;

            //LoadData();
            //TaoMenuCot();
        }

        private void FrmTKKhachHang_Shown(object sender, EventArgs e)
        {
            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvTKKH.Columns)
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
                    dgvTKKH.Columns[colName].Visible = item.Checked;
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

                    if (dgvTKKH.Columns.Contains(colName))
                    {
                        item.Checked = dgvTKKH.Columns[colName].Visible;
                    }
                }
            }
        }

        private void dgvTKKH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string matk = dgvTKKH.Rows[e.RowIndex].Cells["MaTaiKhoanKH"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvTKKH.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTTKKhachHang frm = new FrmCTTKKhachHang(matk, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvTKKH.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa tài khoản này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblTaiKhoanKH WHERE MaTaiKhoanKH = '{matk}'");

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

        private void dgvTKKH_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string matk = dgvTKKH.Rows[e.RowIndex].Cells["MaTaiKhoanKH"].Value.ToString();

            FrmCTTKKhachHang frm = new FrmCTTKKhachHang(matk, "edit");
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
            FrmCTTKKhachHang frm = new FrmCTTKKhachHang("", "add");
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

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }

        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            filterTrangThai = -1;
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }

        private void btnFrmKH_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiParent.MdiChildren)
            {
                if (frm is FrmKhachHang)
                {
                    frm.Activate();
                    this.Close();
                    return;
                }
            }

            // Nếu chưa mở thì mở mới
            FrmKhachHang f = new FrmKhachHang();
            f.MdiParent = this.MdiParent;
            f.Dock = DockStyle.Fill;
            f.Show();

            this.Close();
        }
    }
}
