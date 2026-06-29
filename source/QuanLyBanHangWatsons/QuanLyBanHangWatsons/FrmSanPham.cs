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
    public partial class FrmSanPham : Form
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
        string filterLoaiSP = "";
        string filterThuongHieu = "";

        public FrmSanPham()
        {
            InitializeComponent();
            this.Shown += FrmSanPham_Shown;
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
            colSua.Width = 30;
            colSua.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvSP.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 30;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvSP.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblSanPham WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" AND (MaSanPham LIKE '%{keyword}%' OR TenSanPham LIKE N'%{keyword}%' " +
                            $"OR MaVach LIKE N'%{keyword}%' OR MaNhaCungCap LIKE N'%{keyword}%' " +
                            $"OR MaLoaiSanPham LIKE N'%{keyword}%' OR MaThuongHieu LIKE N'%{keyword}%' " +
                            $"OR MaNhanVien LIKE N'%{keyword}%')";
            }
            // Lọc theo trạng thái 
            if (filterTrangThai != -1)
            {
                sqlCount += $" AND TrangThaiSanPham = {filterTrangThai}";
            }
            // Lọc theo loại sản phẩm
            if (!string.IsNullOrEmpty(filterLoaiSP))
            {
                sqlCount += $" AND MaLoaiSanPham = '{filterLoaiSP}'";
            }

            // Lọc theo thương hiệu
            if (!string.IsNullOrEmpty(filterThuongHieu))
            {
                sqlCount += $" AND MaThuongHieu = '{filterThuongHieu}'";
            }


            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvSP.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvSP.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy sản phẩm nào!";
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
            FROM tblSanPham
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (MaSanPham LIKE '%{keyword}%' OR TenSanPham LIKE N'%{keyword}%' 
                             OR MaVach LIKE N'%{keyword}%' OR MaNhaCungCap LIKE N'%{keyword}%' 
                             OR MaLoaiSanPham LIKE N'%{keyword}%' OR MaThuongHieu LIKE N'%{keyword}%' 
                             OR MaNhanVien LIKE N'%{keyword}%')";
            }

            // Lọc trạng thái
            if (filterTrangThai != -1)
            {
                query += $" AND TrangThaiSanPham = {filterTrangThai}";
            }
            // Lọc theo loại sản phẩm
            if (!string.IsNullOrEmpty(filterLoaiSP))
                query += $" AND MaLoaiSanPham = '{filterLoaiSP}'";

            // Lọc theo thương hiệu
            if (!string.IsNullOrEmpty(filterThuongHieu))
                query += $" AND MaThuongHieu = '{filterThuongHieu}'";


            // Cuối cùng thêm phân trang
            query += $@"
            ORDER BY MaSanPham
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvSP.DataSource = kn.ExecuteQuery(query);

            // --- TẠO CỘT HÌNH ẢNH TỪ FILE NAME ---
            if (dgvSP.Columns.Contains("HinhAnhImg"))
                dgvSP.Columns.Remove("HinhAnhImg");

            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "HinhAnhImg";
            imgCol.HeaderText = "Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCol.Width = 60;
            imgCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            dgvSP.Columns.Add(imgCol);

            // Gán ảnh vào từng dòng
            foreach (DataGridViewRow row in dgvSP.Rows)
            {
                try
                {
                    string fileName = row.Cells["HinhAnh"].Value?.ToString();

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string fullPath = Path.Combine(Application.StartupPath, @"..\..\Images\", fileName);
                        if (File.Exists(fullPath))
                            row.Cells["HinhAnhImg"].Value = Image.FromFile(fullPath);
                    }
                }
                catch { }
            }


            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvSP.Columns.Contains("btnSua")) dgvSP.Columns.Remove("btnSua");
            if (dgvSP.Columns.Contains("btnXoa")) dgvSP.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvSP.Columns["MaSanPham"].HeaderText = "Mã sản phẩm";
            dgvSP.Columns["TenSanPham"].HeaderText = "Tên sản phẩm";
            dgvSP.Columns["MaLoaiSanPham"].HeaderText = "Loại sản phẩm";
            dgvSP.Columns["MaThuongHieu"].HeaderText = "Thương hiệu";
            dgvSP.Columns["DonViTinh"].HeaderText = "Đơn vị tính";
            dgvSP.Columns["GiaBan"].HeaderText = "Giá bán";
            dgvSP.Columns["MoTa"].HeaderText = "Mô tả";
            dgvSP.Columns["TrangThaiSanPham"].HeaderText = "Trạng Thái";
            dgvSP.Columns["MaVach"].HeaderText = "Mã vạch";
            dgvSP.Columns["MaNhaCungCap"].HeaderText = "Nhà cung cấp";       
            dgvSP.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvSP.Columns["NgayCapNhat"].HeaderText = "Ngày cập nhật";

            //Ẩn cột HinhAnh gốc
            dgvSP.Columns["HinhAnh"].Visible = false;

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvSP.Columns)
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

                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // tự giãn
                switch (col.Name)
                {
                        case "MaSanPham":
                            col.FillWeight = 7;   
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "TenSanPham":
                            col.FillWeight = 20;  
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "MaLoaiSanPham":
                            col.FillWeight = 5;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "MaThuongHieu":
                            col.FillWeight = 5;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "DonViTinh":
                            col.FillWeight = 8;  
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "GiaBan":
                            col.FillWeight = 8;  
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "MoTa":
                            col.FillWeight = 5;  
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "TrangThaiSanPham":
                            col.FillWeight = 5;  
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "MaVach":
                            col.FillWeight = 8;  
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "MaNhaCungCap":
                            col.FillWeight = 8;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "MaNhanVien":
                            col.FillWeight = 5;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        case "NgayCapNhat":
                            col.FillWeight = 7;  
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;

                }
            }
            

            // Định dạng cột 'NgayDatHang' để chỉ hiển thị ngày tháng năm
            dgvSP.Columns["NgayCapNhat"].DefaultCellStyle.Format = "dd/MM/yyyy";


            // Toàn bảng
            dgvSP.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSP.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmSanPham_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvSP.CellClick += dgvSP_CellClick;
            dgvSP.CellDoubleClick += dgvSP_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            tsmDangBan.Click += tsmDangBan_Click;
            tsmNgungKD.Click += tsmNgungKD_Click;

            // Build menu lọc Loại SP
            BuildFilterMenu(
                cmsLocLoaiSP,
                "SELECT MaLoaiSanPham, TenLoaiSanPham FROM tblLoaiSanPham",
                "TenLoaiSanPham",     // tên hiển thị
                "MaLoaiSanPham",      // mã ẩn
                value => filterLoaiSP = value
            );

            // Build menu lọc Thương hiệu
            BuildFilterMenu(
                cmsLocTH,
                "SELECT MaThuongHieu, TenThuongHieu FROM tblThuongHieu",
                "TenThuongHieu",      // tên hiển thị
                "MaThuongHieu",       // mã ẩn
                value => filterThuongHieu = value
            );

            //LoadData();
            //TaoMenuCot();
        }

        private void FrmSanPham_Shown(object sender, EventArgs e)
        {
            LoadData();
            TaoMenuCot();
        }



        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvSP.Columns)
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
                    dgvSP.Columns[colName].Visible = item.Checked;
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

                    if (dgvSP.Columns.Contains(colName))
                    {
                        item.Checked = dgvSP.Columns[colName].Visible;
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

        private void dgvSP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string masp = dgvSP.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvSP.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTSanPham frm = new FrmCTSanPham(masp, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvSP.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblSanPham WHERE MaSanPham = '{masp}'");

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

        private void dgvSP_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string masp = dgvSP.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();


            FrmCTSanPham frm = new FrmCTSanPham(masp, "edit");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy sản phẩm nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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



        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            filterTrangThai = -1;
            filterLoaiSP = "";
            filterThuongHieu = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
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

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTSanPham frm = new FrmCTSanPham("", "add");
            // --- Thêm: FormClosed event để reload data ---
            frm.FormClosed += (s, ev) => LoadData();
            frm.ShowDialog();
        }

        private void btnLocTTSP_Click(object sender, EventArgs e)
        {
            cmsTTSP.Show(btnLocTTSP, new Point(0, btnLocTTSP.Height));
        }

        private void tsmDangBan_Click(object sender, EventArgs e)
        {
            filterTrangThai = 1;
            pageNumber = 1;
            LoadData(currentKeyword);
        }

        private void tsmNgungKD_Click(object sender, EventArgs e)
        {
            filterTrangThai = 0;
            pageNumber = 1;
            LoadData(currentKeyword);
        }

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }

        private void btnLocLoaiSP_Click(object sender, EventArgs e)
        {
            cmsLocLoaiSP.Show(btnLocLoaiSP, 0, btnLocLoaiSP.Height);
        }

        private void btnLocTH_Click(object sender, EventArgs e)
        {
            cmsLocTH.Show(btnLocTH, 0, btnLocTH.Height);
        }

        public Action OpenFrmLSP;
        public Action OpenFrmTH;
        public Action OpenFrmCN;

        private void btnFrmLSP_Click(object sender, EventArgs e)
        {
            OpenFrmLSP?.Invoke();
        }

        private void btnFrmTH_Click(object sender, EventArgs e)
        {
            OpenFrmTH?.Invoke();
        }
       
        private void btnFrmCN_Click(object sender, EventArgs e)
        {
            OpenFrmCN?.Invoke();
        }
    }
}
