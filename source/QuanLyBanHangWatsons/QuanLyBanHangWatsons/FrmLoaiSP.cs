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
    public partial class FrmLoaiSP : Form
    {
        Ket_noi kn = new Ket_noi();

        // Biến phân trang toàn cục
        int pageSize = 10;        // số dòng mỗi trang
        int pageNumber = 1;       // trang hiện tại
        int totalRecords = 0;     // tổng số dòng
        int totalPages = 0;       // tổng số trang

        // --- Thêm: biến lưu keyword hiện tại ---
        private string currentKeyword = "";

        public FrmLoaiSP()
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
            colSua.Width = 250;
            colSua.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvLSP.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 250;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvLSP.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            // Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblLoaiSanPham";
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" WHERE MaLoaiSanPham LIKE '%{keyword}%' OR TenLoaiSanPham LIKE N'%{keyword}%'";
            }
            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvLSP.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy loại sản phẩm nào!";
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
            FROM tblLoaiSanPham
            {(string.IsNullOrEmpty(keyword)
                ? ""
                : $"WHERE MaLoaiSanPham LIKE '%{keyword}%' OR TenLoaiSanPham LIKE N'%{keyword}%'")}
            ORDER BY MaLoaiSanPham
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            dgvLSP.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvLSP.Columns.Contains("btnSua")) dgvLSP.Columns.Remove("btnSua");
            if (dgvLSP.Columns.Contains("btnXoa")) dgvLSP.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvLSP.Columns["MaLoaiSanPham"].HeaderText = "Mã loại sản phẩm";
            dgvLSP.Columns["TenLoaiSanPham"].HeaderText = "Tên loại sản phẩm";

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvLSP.Columns)
            {
                // Header luôn center
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (col is DataGridViewImageColumn)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
                    col.Width = 250;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // nút cũng center
                }
                else
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // tự giãn
                    switch (col.Name)
                    {
                        case "MaLoaiSanPham":
                            col.FillWeight = 40;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // căn giữa
                            break;
                        case "TenLoaiSanPham":
                            col.FillWeight = 60;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // căn trái
                            break;
                    }
                }
            }


            // Định dạng cột 'NgayDatHang' để chỉ hiển thị ngày tháng năm
            //dgvDH.Columns["NgayDatHang"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Toàn bảng
            dgvLSP.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLSP.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();

        }

        private void FrmLoaiSP_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvLSP.CellClick += dgvLSP_CellClick;
            dgvLSP.CellDoubleClick += dgvLSP_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvLSP.Columns)
            {
                if (col.Name == "btnSua" || col.Name == "btnXoa") continue;

                ToolStripMenuItem item = new ToolStripMenuItem(col.HeaderText);
                item.CheckOnClick = true;
                item.Checked = col.Visible;
                item.Tag = col.Name;   // liên kết item → column

                item.Click += (s, e) =>
                {
                    string colName = item.Tag.ToString();
                    dgvLSP.Columns[colName].Visible = item.Checked;
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

                    if (dgvLSP.Columns.Contains(colName))
                    {
                        item.Checked = dgvLSP.Columns[colName].Visible;
                    }
                }
            }
        }

        private void dgvLSP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string ma = dgvLSP.Rows[e.RowIndex].Cells["MaLoaiSanPham"].Value.ToString();
            string ten = dgvLSP.Rows[e.RowIndex].Cells["TenLoaiSanPham"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvLSP.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTLoaiSP frm = new FrmCTLoaiSP(ma, ten, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
            }

            // Nếu nhấn nút xóa
            if (dgvLSP.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa loại sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblLoaiSanPham WHERE MaLoaiSanPham = '{ma}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy loại sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvLSP_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string ma = dgvLSP.Rows[e.RowIndex].Cells["MaLoaiSanPham"].Value.ToString();
            string ten = dgvLSP.Rows[e.RowIndex].Cells["TenLoaiSanPham"].Value.ToString();

            FrmCTLoaiSP frm = new FrmCTLoaiSP(ma, ten, "edit");
            // --- Thêm: FormClosed event để reload data ---
            frm.FormClosed += (s, ev) => LoadData();
            frm.ShowDialog();
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTLoaiSP frm = new FrmCTLoaiSP("", "", "add");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} loại sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy loại sản phẩm nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = ""; // reset ô tìm kiếm
            currentKeyword = "";
            pageNumber = 1;
            LoadData(); // load toàn bộ
        }

        private void btnTuyChinhCot_Click(object sender, EventArgs e)
        {
            cmsCot.Show(btnTuyChinhCot, new Point(0, btnTuyChinhCot.Height));
        }

        private void btnFrmSP_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiParent.MdiChildren)
            {
                if (frm is FrmSanPham)
                {
                    frm.Activate();
                    this.Close();
                    return;
                }
            }

            // Nếu chưa mở thì mở mới
            FrmSanPham f = new FrmSanPham();
            f.MdiParent = this.MdiParent;
            f.Dock = DockStyle.Fill;
            f.Show();

            this.Close();
        }
    }
}
