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
    public partial class FrmHoaDon : Form
    {
        Ket_noi kn = new Ket_noi();

        // Biến phân trang toàn cục
        int pageSize = 10;        // số dòng mỗi trang
        int pageNumber = 1;       // trang hiện tại
        int totalRecords = 0;     // tổng số dòng
        int totalPages = 0;       // tổng số trang

        // --- Thêm: biến lưu keyword hiện tại ---
        private string currentKeyword = "";
        private string filterNgay = "";

        public FrmHoaDon()
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
            dgvHD.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 60;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvHD.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            /// Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblHoaDon WHERE 1 = 1";
            // Lọc theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $@" AND (
                MaHoaDon LIKE '%{keyword}%' 
                OR MaThanhToan LIKE '%{keyword}%'
                OR MaNhanVien  LIKE '%{keyword}%'     
            )";
            }

            // Lọc theo ngày
            if (!string.IsNullOrEmpty(filterNgay))
            {
                sqlCount += $" AND CAST(NgayXuatHoaDon AS date) = '{filterNgay}'";
            }

            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu không có bản ghi sau khi áp dụng keyword hoặc trạng thái
            if (totalRecords == 0)
            {
                dgvHD.DataSource = null;
                lblTongSoBanGhi.Text = "Không có dữ liệu.";
                lblTrang.Text = "";
                return; // Quan trọng: tránh OFFSET âm
            }

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvHD.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy hóa đơn nào!";
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
            FROM tblHoaDon
            WHERE 1 = 1
            ";
            // Lọc keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query += $@" AND (
                MaHoaDon LIKE '%{keyword}%' 
                OR MaThanhToan LIKE '%{keyword}%'
                OR MaNhanVien  LIKE '%{keyword}%'           
            )";
            }

            // Lọc nhiều loại đơn hàng
            if (!string.IsNullOrEmpty(filterNgay))
            {
                query += $" AND CAST(NgayXuatHoaDon AS date) = '{filterNgay}'";
            }

            // Cuối cùng thêm phân trang
            query += $@"
        ORDER BY MaHoaDon
        OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY
        ";

            dgvHD.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvHD.Columns.Contains("btnSua")) dgvHD.Columns.Remove("btnSua");
            if (dgvHD.Columns.Contains("btnXoa")) dgvHD.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvHD.Columns["MaHoaDon"].HeaderText = "Hóa đơn";
            dgvHD.Columns["MaThanhToan"].HeaderText = "Thanh toán";
            dgvHD.Columns["MaNhanVien"].HeaderText = "Nhân viên";
            dgvHD.Columns["NgayXuatHoaDon"].HeaderText = "Ngày xuất hóa đơn";
            dgvHD.Columns["TongTien"].HeaderText = "Tổng tiền";
            dgvHD.Columns["SoTienDaThanhToan"].HeaderText = "Số tiền đã thanh toán";
            dgvHD.Columns["SoDuConLai"].HeaderText = "Số dư còn lại";

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvHD.Columns)
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
                    case "MaHoaDon":
                    case "MaThanhToan":
                    case "MaNhanVien":
                        col.FillWeight = 8;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case "NgayXuatHoaDon":
                        col.FillWeight = 20;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                        break;

                    case "TongTien":
                        col.FillWeight = 10;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.DefaultCellStyle.Format = "#,##0 đ";  // format tiền
                        break;

                    case "SoTienDaThanhToan":
                        col.FillWeight = 10;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.DefaultCellStyle.Format = "#,##0 đ";
                        break;

                    case "SoDuConLai":
                        col.FillWeight = 7;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.DefaultCellStyle.Format = "#,##0 đ";
                        break;
                }
            }
            dgvHD.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHD.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmHoaDon_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvHD.CellClick += dgvHD_CellClick;
            dgvHD.CellDoubleClick += dgvHD_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            //BuildFilterMenuPTTT();

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvHD.Columns)
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
                    dgvHD.Columns[colName].Visible = item.Checked;
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

                    if (dgvHD.Columns.Contains(colName))
                    {
                        item.Checked = dgvHD.Columns[colName].Visible;
                    }
                }
            }
        }

        private void dgvHD_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string mahd = dgvHD.Rows[e.RowIndex].Cells["MaHoaDon"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvHD.Columns[e.ColumnIndex].Name == "btnSua")
            {
                string maHD = dgvHD.Rows[e.RowIndex].Cells["MaHoaDon"].Value.ToString();
                string maTT = dgvHD.Rows[e.RowIndex].Cells["MaThanhToan"].Value.ToString();
                string maNV = dgvHD.Rows[e.RowIndex].Cells["MaNhanVien"].Value.ToString();

                FrmCTHoaDon f = new FrmCTHoaDon(maHD, maTT, maNV, "edit");
                f.FormClosed += (s, ev) => LoadData();
                f.ShowDialog();
                return;
            }

            // Nếu nhấn nút xóa
            if (dgvHD.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblHoaDon WHERE MaHoaDon = '{mahd}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvHD_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {


            if (e.RowIndex < 0) return;
            string maHD = dgvHD.Rows[e.RowIndex].Cells["MaHoaDon"].Value.ToString();
            string maTT = dgvHD.Rows[e.RowIndex].Cells["MaThanhToan"].Value.ToString();
            string maNV = dgvHD.Rows[e.RowIndex].Cells["MaNhanVien"].Value.ToString();

            FrmCTHoaDon f = new FrmCTHoaDon(maHD, maTT, maNV, "edit");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy hóa đơn nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTHoaDon f = new FrmCTHoaDon("", "add");
            // --- Thêm: FormClosed event để reload data ---
            f.FormClosed += (s, ev) => LoadData();
            f.ShowDialog();
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

        private void btnLocHD_Click(object sender, EventArgs e)
        {
            pageNumber = 1;

            // Chuyển ngày sang dạng yyyy-MM-dd để SQL so sánh đúng
            filterNgay = dtpNgay.Value.ToString("yyyy-MM-dd");

            LoadData();
        }

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }


        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";          
            filterNgay = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }
    }
}
