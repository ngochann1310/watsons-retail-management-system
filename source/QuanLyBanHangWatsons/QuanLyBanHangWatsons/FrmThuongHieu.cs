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
    public partial class FrmThuongHieu : Form
    {
        Ket_noi kn = new Ket_noi();

        // Biến phân trang toàn cục
        int pageSize = 10;        // số dòng mỗi trang
        int pageNumber = 1;       // trang hiện tại
        int totalRecords = 0;     // tổng số dòng
        int totalPages = 0;       // tổng số trang

        // --- Thêm: biến lưu keyword hiện tại ---
        private string currentKeyword = "";

        public FrmThuongHieu()
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
            dgvTH.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 250;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvTH.Columns.Add(colXoa);
        }

        private void LoadData(string keyword = "")
        {
            // Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblThuongHieu";
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" WHERE MaThuongHieu LIKE '%{keyword}%' " +
                    $"OR TenThuongHieu LIKE N'%{keyword}%'";
            }
            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvTH.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy thương hiệu nào!";
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
            FROM tblThuongHieu
            {(string.IsNullOrEmpty(keyword)
                ? ""
                : $"WHERE MaThuongHieu LIKE '%{keyword}%' OR TenThuongHieu LIKE N'%{keyword}%'")}
            ORDER BY MaThuongHieu
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            dgvTH.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvTH.Columns.Contains("btnSua")) dgvTH.Columns.Remove("btnSua");
            if (dgvTH.Columns.Contains("btnXoa")) dgvTH.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvTH.Columns["MaThuongHieu"].HeaderText = "Mã thương hiệu";
            dgvTH.Columns["TenThuongHieu"].HeaderText = "Tên thương hiệu";

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvTH.Columns)
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
                        case "MaThuongHieu":
                            col.FillWeight = 40;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // căn giữa
                            break;
                        case "TenThuongHieu":
                            col.FillWeight = 60;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // căn trái
                            break;
                    }
                }
            }

            // Toàn bảng
            dgvTH.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTH.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmThuongHieu_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvTH.CellClick += dgvTH_CellClick;
            dgvTH.CellDoubleClick += dgvTH_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvTH.Columns)
            {
                if (col.Name == "btnSua" || col.Name == "btnXoa") continue;

                ToolStripMenuItem item = new ToolStripMenuItem(col.HeaderText);
                item.CheckOnClick = true;
                item.Checked = col.Visible;
                item.Tag = col.Name;   // liên kết item → column

                item.Click += (s, e) =>
                {
                    string colName = item.Tag.ToString();
                    dgvTH.Columns[colName].Visible = item.Checked;
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

                    if (dgvTH.Columns.Contains(colName))
                    {
                        item.Checked = dgvTH.Columns[colName].Visible;
                    }
                }
            }
        }


        private void dgvTH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string ma = dgvTH.Rows[e.RowIndex].Cells["MaThuongHieu"].Value.ToString();
            string ten = dgvTH.Rows[e.RowIndex].Cells["TenThuongHieu"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvTH.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTThuongHieu frm = new FrmCTThuongHieu(ma, ten, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
            }

            // Nếu nhấn nút xóa
            if (dgvTH.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa thương hiệu này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblThuongHieu WHERE MaThuongHieu = '{ma}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy thương hiệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvTH_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string ma = dgvTH.Rows[e.RowIndex].Cells["MaThuongHieu"].Value.ToString();
            string ten = dgvTH.Rows[e.RowIndex].Cells["TenThuongHieu"].Value.ToString();

            FrmCTThuongHieu frm = new FrmCTThuongHieu(ma, ten, "edit");
            // --- Thêm: FormClosed event để reload data ---
            frm.FormClosed += (s, ev) => LoadData();
            frm.ShowDialog();
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTThuongHieu frm = new FrmCTThuongHieu("", "", "add");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} thương hiệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy thương hiệu nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
