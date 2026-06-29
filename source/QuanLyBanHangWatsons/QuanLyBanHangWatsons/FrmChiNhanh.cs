using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{
    public partial class FrmChiNhanh : Form
    {
        Ket_noi kn = new Ket_noi();

        // Biến phân trang toàn cục
        int pageSize = 10;        // số dòng mỗi trang
        int pageNumber = 1;       // trang hiện tại
        int totalRecords = 0;     // tổng số dòng
        int totalPages = 0;       // tổng số trang

        // --- Thêm: biến lưu keyword hiện tại ---
        private string currentKeyword = "";

        public FrmChiNhanh()
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
            colSua.Width = 200;
            colSua.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvCN.Columns.Add(colSua);

            // Tạo cột Xóa
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "btnXoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 200;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
            dgvCN.Columns.Add(colXoa);
        }
        

        private void LoadData(string keyword = "")
        {
            // Lấy số dòng mỗi trang từ ComboBox
            pageSize = int.Parse(cboSoBanGhiHienThi.SelectedItem.ToString());

            // Đếm tổng số bản ghi
            string sqlCount = "SELECT COUNT(*) FROM tblChiNhanh";
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlCount += $" WHERE MaChiNhanh LIKE '%{keyword}%' OR TenChiNhanh LIKE N'%{keyword}%' " +
                    $"OR DiaChi LIKE N'%{keyword}%'";
            }
            totalRecords = Convert.ToInt32(kn.ExecuteScalar(sqlCount));

            // Nếu tìm kiếm mà không có bản ghi
            if (!string.IsNullOrEmpty(keyword) && totalRecords == 0)
            {
                dgvCN.DataSource = null; // xóa DataGridView cũ
                lblTongSoBanGhi.Text = "Không tìm thấy chi nhánh nào!";
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
            FROM tblChiNhanh
            {(string.IsNullOrEmpty(keyword)
                ? ""
                : $"WHERE MaChiNhanh LIKE '%{keyword}%' OR TenChiNhanh LIKE N'%{keyword}%' " +
                $"OR DiaChi LIKE N'%{keyword}%'")}
            ORDER BY MaChiNhanh
            OFFSET {skip} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            dgvCN.DataSource = kn.ExecuteQuery(query);

            // --- Thêm: xóa cột button cũ trước khi thêm để tránh lặp ---
            if (dgvCN.Columns.Contains("btnSua")) dgvCN.Columns.Remove("btnSua");
            if (dgvCN.Columns.Contains("btnXoa")) dgvCN.Columns.Remove("btnXoa");

            AddActionButtons();

            // Đổi header text
            dgvCN.Columns["MaChiNhanh"].HeaderText = "Mã chi nhánh";
            dgvCN.Columns["TenChiNhanh"].HeaderText = "Tên chi nhánh";
            dgvCN.Columns["DiaChi"].HeaderText = "Địa chỉ";

            // Thiết lập AutoSize và FillWeight
            foreach (DataGridViewColumn col in dgvCN.Columns)
            {
                // Header luôn center
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (col is DataGridViewImageColumn)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // giữ cố định
                    col.Width = 200;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // nút cũng center
                }
                else
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // tự giãn
                    switch (col.Name)
                    {
                        case "MaChiNhanh":
                            col.FillWeight = 10;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // căn giữa
                            break;
                        case "TenChiNhanh":
                            col.FillWeight = 35;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // căn trái
                            break;
                        case "DiaChi":
                            col.FillWeight = 55;
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // căn trái
                            break;
                    }
                }
            }


            // Định dạng cột 'NgayDatHang' để chỉ hiển thị ngày tháng năm
            //dgvDH.Columns["NgayDatHang"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Toàn bảng
            dgvCN.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCN.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // row tự giãn

            // Cập nhật nhãn phân trang
            lblTongSoBanGhi.Text = "Tổng số bản ghi: " + totalRecords;
            lblTrang.Text = $"Trang {pageNumber}/{totalPages}";

            CapNhatTrangThaiMenuCot();
        }

        private void FrmChiNhanh_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            // Thiết lập ComboBox số bản ghi
            cboSoBanGhiHienThi.Items.AddRange(new string[] { "5", "10", "20", "50", "100" });
            cboSoBanGhiHienThi.SelectedItem = "10";

            //Event
            dgvCN.CellClick += dgvCN_CellClick;
            dgvCN.CellDoubleClick += dgvCN_CellDoubleClick;
            txtTimKiem.KeyDown += txtTimKiem_KeyDown;
            cboSoBanGhiHienThi.SelectedIndexChanged += cboSoBanGhiHienThi_SelectedIndexChanged;

            LoadData();
            TaoMenuCot();
        }

        private void TaoMenuCot()
        {
            cmsCot.Items.Clear();

            foreach (DataGridViewColumn col in dgvCN.Columns)
            {
                if (col.Name == "btnSua" || col.Name == "btnXoa") continue;

                ToolStripMenuItem item = new ToolStripMenuItem(col.HeaderText);
                item.CheckOnClick = true;
                item.Checked = col.Visible;
                item.Tag = col.Name;   // liên kết item → column

                item.Click += (s, e) =>
                {
                    string colName = item.Tag.ToString();
                    dgvCN.Columns[colName].Visible = item.Checked;
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

                    if (dgvCN.Columns.Contains(colName))
                    {
                        item.Checked = dgvCN.Columns[colName].Visible;
                    }
                }
            }
        }


        private void dgvCN_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string ma = dgvCN.Rows[e.RowIndex].Cells["MaChiNhanh"].Value.ToString();
            string ten = dgvCN.Rows[e.RowIndex].Cells["TenChiNhanh"].Value.ToString();
            string diachi = dgvCN.Rows[e.RowIndex].Cells["DiaChi"].Value.ToString();

            // Nếu nhấn nút sửa
            if (dgvCN.Columns[e.ColumnIndex].Name == "btnSua")
            {
                FrmCTChiNhanh frm = new FrmCTChiNhanh(ma, ten, diachi, "edit");
                frm.FormClosed += (s, ev) => LoadData();
                frm.ShowDialog();
            }

            // Nếu nhấn nút xóa
            if (dgvCN.Columns[e.ColumnIndex].Name == "btnXoa")
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn xóa chi nhánh này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    int rowsAffected = kn.ExecuteNonQuery($"DELETE FROM tblChiNhanh WHERE MaChiNhanh = '{ma}'");

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại! Không tìm thấy chi nhánh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    LoadData(); // load lại dgv sau khi xóa
                }
            }
        }

        private void dgvCN_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // tránh lỗi click header

            //var row = dgvCN.Rows[e.RowIndex];

            string ma = dgvCN.Rows[e.RowIndex].Cells["MaChiNhanh"].Value.ToString();
            string ten = dgvCN.Rows[e.RowIndex].Cells["TenChiNhanh"].Value.ToString();
            string diachi = dgvCN.Rows[e.RowIndex].Cells["DiaChi"].Value.ToString();

            FrmCTChiNhanh frm = new FrmCTChiNhanh(ma, ten, diachi, "edit");
            // --- Thêm: FormClosed event để reload data ---
            frm.FormClosed += (s, ev) => LoadData();
            frm.ShowDialog();
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            FrmCTChiNhanh frm = new FrmCTChiNhanh("", "", "", "add");
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
                        MessageBox.Show($"Tìm thấy {totalRecords} chi nhánh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Không tìm thấy chi nhánh nào.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
