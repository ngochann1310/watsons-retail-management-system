using System;
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
    public partial class FrmPhieuNhap_Tao : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maPN;

        public FrmPhieuNhap_Tao(string mapn, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maPN = mapn;
 
            if (mode == "add")
            {
                txtMaPN.Enabled = false;
            }

            if (mode == "edit")
            {
                btnThem.Enabled = false;
                txtMaPN.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void FrmPhieuNhap_Tao_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaNV();
            LoadComboBoxMaCN();
            LoadComboBoxMaNCC();

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaPN.Text))
                    txtMaPN.Text = GenerateNewMaPhieuNhap();
            }

            if (mode == "edit")
            {
                LoadDetail();
            }

            SetupDgvPN_SPChon();

            // Load lần đầu
            LoadDanhSachSanPham();

            // Sự kiện click nút sửa/xóa
            dgvPN_SPChon.CellClick += DgvPN_SPChon_CellClick;

            dgvPN_SPChon.CellDoubleClick += DgvPN_SPChon_CellDoubleClick;

        }

        //Tạo mã phiếu nhập tự động
        private string GenerateNewMaPhieuNhap()
        {
            // Lấy mã lớn nhất hiện có, tìm phần số cuối, tăng +1
            // Nếu DB có mã dạng "PN0001" hoặc tương tự, code này sẽ xử lý.
            string sql = "SELECT TOP 1 MaPhieuNhap FROM tblPhieuNhap ORDER BY MaPhieuNhap DESC";
            var obj = kn.ExecuteScalar(sql);
            if (obj == null || obj == DBNull.Value) return "PN0001";

            string last = obj.ToString().Trim();
            // lấy phần số cuối liên tiếp từ cuối chuỗi
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;
            string prefix = last.Substring(0, i + 1);
            string numPart = last.Substring(i + 1);
            int num = 0;
            if (!int.TryParse(numPart, out num))
            {
                // nếu không parse được, thêm 1
                return prefix + "1";
            }
            num++;
            // giữ độ dài phần số như trước (padding bằng 0)
            string newNum = num.ToString().PadLeft(numPart.Length, '0');
            return prefix + newNum;
        }

        private void LoadDetail()
        {
            string sql = $@"
            SELECT pn.*, nv.HoNhanVien + ' ' + nv.TenNhanVien AS HoTenNV,
                   ncc.TenNhaCungCap, ncc.DienThoai, ncc.Email, ncc.DiaChi
            FROM tblPhieuNhap pn
            JOIN tblNhanVien nv ON pn.MaNhanVien = nv.MaNhanVien
            JOIN tblNhaCungCap ncc ON pn.MaNhaCungCap = ncc.MaNhaCungCap
            WHERE pn.MaPhieuNhap = '{maPN}'
            ";

            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            txtMaPN.Text = r["MaPhieuNhap"].ToString();

            if (r["MaNhanVien"] != DBNull.Value)
                cboMaNV.SelectedValue = r["MaNhanVien"].ToString();

            if (r["MaNhaCungCap"] != DBNull.Value)
                cboMaNCC.SelectedValue = r["MaNhaCungCap"].ToString();

            if (r["MaChiNhanh"] != DBNull.Value)
                cboMaCN.SelectedValue = r["MaChiNhanh"].ToString();

            if (r["NgayNhap"] != DBNull.Value)
                dtpNgayNhap.Value = Convert.ToDateTime(r["NgayNhap"]);

            txtTongTien.Text = r["TongTien"]?.ToString() ?? "0";
            txtGhiChu.Text = r["GhiChu"]?.ToString();
        }

        private void SetupDgvPN_SPChon()
        {
            dgvPN_SPChon.AutoGenerateColumns = false;
            dgvPN_SPChon.AllowUserToAddRows = false;
            dgvPN_SPChon.Columns.Clear();

            // 1. Ảnh
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "HinhAnh";
            imgCol.HeaderText = "Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCol.Width = 60;
            dgvPN_SPChon.Columns.Add(imgCol);

            // 2. MaSP (ẩn)
            DataGridViewTextBoxColumn colMaSP = new DataGridViewTextBoxColumn();
            colMaSP.Name = "MaSP";
            colMaSP.HeaderText = "Mã SP";
            colMaSP.Visible = false;
            dgvPN_SPChon.Columns.Add(colMaSP);

            // 3. TenSP
            DataGridViewTextBoxColumn colTen = new DataGridViewTextBoxColumn();
            colTen.Name = "TenSP";
            colTen.HeaderText = "Tên sản phẩm";
            dgvPN_SPChon.Columns.Add(colTen);

            // 4. SoLuong
            DataGridViewTextBoxColumn colSL = new DataGridViewTextBoxColumn();
            colSL.Name = "SoLuong";
            colSL.HeaderText = "Số lượng";
            dgvPN_SPChon.Columns.Add(colSL);

            // 5. GiaNhap
            DataGridViewTextBoxColumn colGia = new DataGridViewTextBoxColumn();
            colGia.Name = "GiaNhap";
            colGia.HeaderText = "Giá nhập";
            dgvPN_SPChon.Columns.Add(colGia);

            // 6. NSX
            DataGridViewTextBoxColumn colNSX = new DataGridViewTextBoxColumn();
            colNSX.Name = "NSX";
            colNSX.HeaderText = "Ngày sản xuất";
            dgvPN_SPChon.Columns.Add(colNSX);

            // 7. HSD
            DataGridViewTextBoxColumn colHSD = new DataGridViewTextBoxColumn();
            colHSD.Name = "HSD";
            colHSD.HeaderText = "Hạn sử dụng";
            dgvPN_SPChon.Columns.Add(colHSD);

            // 8. Sua (icon)
            DataGridViewImageColumn colSua = new DataGridViewImageColumn();
            colSua.Name = "Sua";
            colSua.HeaderText = "Sửa";
            string pathSua = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnSua.png");
            if (File.Exists(pathSua)) colSua.Image = Image.FromFile(pathSua);
            colSua.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colSua.Width = 48;
            colSua.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPN_SPChon.Columns.Add(colSua);

            // 9. Xoa (icon)
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "Xoa";
            colXoa.HeaderText = "Xóa";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 48;
            colXoa.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvPN_SPChon.Columns.Add(colXoa);

            // Căn giữa cho các cột cần thiết
            dgvPN_SPChon.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPN_SPChon.Columns["GiaNhap"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPN_SPChon.Columns["NSX"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPN_SPChon.Columns["HSD"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Căn giữa luôn header cho đẹp
            dgvPN_SPChon.Columns["HinhAnh"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPN_SPChon.Columns["TenSP"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPN_SPChon.Columns["SoLuong"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPN_SPChon.Columns["GiaNhap"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPN_SPChon.Columns["NSX"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPN_SPChon.Columns["HSD"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;


            // Thiết lập mặc định hàng/column sizing
            dgvPN_SPChon.RowTemplate.Height = 60;
            dgvPN_SPChon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LoadComboBoxMaCN()
        {
            string query = "SELECT MaChiNhanh  FROM tblChiNhanh";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaCN.DataSource = dt;
            cboMaCN.DisplayMember = "MaChiNhanh";
            cboMaCN.ValueMember = "MaChiNhanh";
            cboMaCN.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void LoadComboBoxMaNV()
        {
            string query = "SELECT MaNhanVien FROM tblNhanVien";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaNV.DataSource = dt;
            cboMaNV.DisplayMember = "MaNhanVien";
            cboMaNV.ValueMember = "MaNhanVien";
            cboMaNV.SelectedIndex = -1;

            // đăng ký event sau khi set DataSource là an toàn
            cboMaNV.SelectedIndexChanged -= cboMaNV_SelectedIndexChanged;
            cboMaNV.SelectedIndexChanged += cboMaNV_SelectedIndexChanged;
        }

        private void LoadComboBoxMaNCC()
        {
            string query = "SELECT MaNhaCungCap FROM tblNhaCungCap";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaNCC.DataSource = dt;
            cboMaNCC.DisplayMember = "MaNhaCungCap";
            cboMaNCC.ValueMember = "MaNhaCungCap";
            cboMaNCC.SelectedIndex = -1;

            cboMaNCC.SelectedIndexChanged -= cboMaNCC_SelectedIndexChanged;
            cboMaNCC.SelectedIndexChanged += cboMaNCC_SelectedIndexChanged;
        }

        private void cboMaNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaNV.SelectedIndex == -1 || cboMaNV.SelectedValue == null)
            {
                lblHoTenNV.Text = "";
                return;
            }

            var kq = kn.ExecuteScalar(
                $"SELECT HoNhanVien + ' ' + TenNhanVien FROM tblNhanVien WHERE MaNhanVien = '{cboMaNV.SelectedValue}'"
            );

            lblHoTenNV.Text = (kq == null || kq == DBNull.Value) ? "" : kq.ToString();
        }
 
        private void cboMaNCC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaNCC.SelectedIndex == -1 || cboMaNCC.SelectedValue == null)
            {
                lblTenNCC.Text = lblSDTNCC.Text = lblEmailNCC.Text = lblDiaChi.Text = "";
                return;
            }

            string sql = $"SELECT TenNhaCungCap, DienThoai, Email, DiaChi FROM tblNhaCungCap WHERE MaNhaCungCap = '{cboMaNCC.SelectedValue}'";
            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count > 0)
            {
                var r = dt.Rows[0];
                lblTenNCC.Text = r["TenNhaCungCap"].ToString();
                lblSDTNCC.Text = r["DienThoai"].ToString();
                lblEmailNCC.Text = r["Email"].ToString();
                lblDiaChi.Text = r["DiaChi"].ToString();
            }
            else
            {
                lblTenNCC.Text = lblSDTNCC.Text = lblEmailNCC.Text = lblDiaChi.Text = "";
            }
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaPN.Text))
            {
                MessageBox.Show("Mã phiếu trống. Vui lòng tạo hoặc lưu phiếu trước khi thêm chi tiết.", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //FrmNhapCTPN f = new FrmNhapCTPN(txtMaPN.Text, "", "add");
            FrmNhapCTPN f = new FrmNhapCTPN(txtMaPN.Text, "add");
            f.ShowDialog();

            // Cập nhật lại danh sách + tổng
            LoadDanhSachSanPham();
            TinhTongTien();
        }

        private void DgvPN_SPChon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvPN_SPChon.Columns[e.ColumnIndex].Name;

            // Nếu double click vào icon Sửa/Xóa thì bỏ qua
            if (col == "Sua" || col == "Xoa") return;

            string maSP = dgvPN_SPChon.Rows[e.RowIndex].Cells["MaSP"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(maSP)) return;

            //FrmNhapCTPN f = new FrmNhapCTPN(txtMaPN.Text, maSP, "edit");
            //Sửa
            var row = dgvPN_SPChon.Rows[e.RowIndex];

            FrmNhapCTPN f = new FrmNhapCTPN(
                txtMaPN.Text,
                maSP,
                row.Cells["SoLuong"].Value.ToString(),
                row.Cells["GiaNhap"].Value.ToString(),
                DateTime.Parse(row.Cells["NSX"].Value.ToString()),
                DateTime.Parse(row.Cells["HSD"].Value.ToString()),
                "edit"
            );
            f.ShowDialog();

            LoadDanhSachSanPham();
            TinhTongTien();
        }

        private void DgvPN_SPChon_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Lấy mã SP từ hàng
            var cellVal = dgvPN_SPChon.Rows[e.RowIndex].Cells["MaSP"].Value;
            string maSP = (cellVal == null || cellVal == DBNull.Value) ? "" : cellVal.ToString();

            if (string.IsNullOrWhiteSpace(maSP))
            {
                MessageBox.Show("Không tìm thấy mã sản phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mở form sửa
            //FrmNhapCTPN f = new FrmNhapCTPN(txtMaPN.Text, maSP, "edit");
            var row = dgvPN_SPChon.Rows[e.RowIndex];

            FrmNhapCTPN f = new FrmNhapCTPN(
                txtMaPN.Text,
                maSP,
                row.Cells["SoLuong"].Value.ToString(),
                row.Cells["GiaNhap"].Value.ToString(),
                DateTime.Parse(row.Cells["NSX"].Value.ToString()),
                DateTime.Parse(row.Cells["HSD"].Value.ToString()),
                "edit"
            );

            f.ShowDialog();

            // Reload lại danh sách + tổng tiền
            LoadDanhSachSanPham();
            TinhTongTien();
        }

        private void TinhTongTien()
        {
            string sql = $@"
                SELECT SUM(SoLuong * GiaNhap)
                FROM tblCTPhieuNhap
                WHERE MaPhieuNhap = '{txtMaPN.Text}'
            ";

            var kq = kn.ExecuteScalar(sql);

            decimal tong = 0;
            if (kq != null && kq != DBNull.Value)
                tong = Convert.ToDecimal(kq);

            txtTongTien.Text = tong.ToString("0.##");

            kn.ExecuteNonQuery($"UPDATE tblPhieuNhap SET TongTien = {tong} WHERE MaPhieuNhap = '{txtMaPN.Text}'");
        }

        private void LoadDanhSachSanPham()
        {
            if (string.IsNullOrWhiteSpace(txtMaPN.Text))
            {
                dgvPN_SPChon.Rows.Clear();
                return;
            }

            string sql = $@"
            SELECT ctpn.*, sp.TenSanPham, sp.HinhAnh
            FROM tblCTPhieuNhap ctpn
            JOIN tblSanPham sp ON ctpn.MaSanPham = sp.MaSanPham
            WHERE ctpn.MaPhieuNhap = '{txtMaPN.Text}'
            ";

            DataTable dt = kn.ExecuteQuery(sql);

            dgvPN_SPChon.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                Image img = null;

                try
                {
                    string fileName = row["HinhAnh"]?.ToString();
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string fullPath = Path.Combine(Application.StartupPath, @"..\..\Images\" + fileName);
                        if (File.Exists(fullPath))
                        {
                            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                            {
                                img = Image.FromStream(fs).Clone() as Image; // clone để tránh lock
                            }
                        }
                    }
                }
                catch { }

                dgvPN_SPChon.Rows.Add(
                    img,
                    row["MaSanPham"].ToString(),
                    row["TenSanPham"].ToString(),
                    row["SoLuong"].ToString(),
                    row["GiaNhap"].ToString(),
                    Convert.ToDateTime(row["NgaySanXuat"]).ToString("dd/MM/yyyy"),
                    Convert.ToDateTime(row["HanSuDung"]).ToString("dd/MM/yyyy"),
                    null, // cột Sua (icon) sẽ hiển thị sẵn
                    null  // cột Xoa
                );
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (mode != "add") return;

            try
            {
                if (cboMaNV.SelectedIndex == -1 ||
                    cboMaCN.SelectedIndex == -1 ||
                    cboMaNCC.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng chọn đủ thông tin!");
                    return;
                }

                string sql = $@"
                INSERT INTO tblPhieuNhap (MaPhieuNhap, MaNhanVien, 
                            MaNhaCungCap, MaChiNhanh, 
                            NgayNhap, TongTien, GhiChu)
                VALUES 
                ('{txtMaPN.Text.Trim()}', 
                '{cboMaNV.SelectedValue}', 
                '{cboMaNCC.SelectedValue}',
                '{cboMaCN.SelectedValue}', 
                '{dtpNgayNhap.Value:yyyy-MM-dd}',
                0, N'{txtGhiChu.Text}')
                ";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Thêm phiếu nhập thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // chỉ đóng form khi thêm thành công
                }
                else
                {
                    MessageBox.Show("Thêm thất bại! Dữ liệu không hợp lệ, vui lòng kiểm tra lại.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // KHÔNG đóng form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm thất bại!\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return; // KHÔNG đóng form nếu bị lỗi
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (mode != "edit")
            {
                MessageBox.Show("Chỉ có thể xóa khi đang chỉnh sửa phiếu nhập!",
                                "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa phiếu nhập này?\nToàn bộ chi tiết cũng sẽ bị xóa!",
                                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            try
            {
                // Xóa chi tiết trước
                string sqlCT = $@"DELETE FROM tblCTPhieuNhap 
                          WHERE MaPhieuNhap = '{maPN}'";
                kn.ExecuteNonQuery(sqlCT);

                // Xóa phiếu nhập
                string sqlPN = $@"DELETE FROM tblPhieuNhap 
                          WHERE MaPhieuNhap = '{maPN}'";
                kn.ExecuteNonQuery(sqlPN);

                MessageBox.Show("Xóa phiếu nhập thành công!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa thất bại!\n" + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (mode != "edit")
            {
                MessageBox.Show("Chỉ có thể sửa khi đang chỉnh sửa phiếu nhập!",
                                "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboMaNV.SelectedIndex == -1 ||
                cboMaCN.SelectedIndex == -1 ||
                cboMaNCC.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = $@"
            UPDATE tblPhieuNhap SET
                MaNhanVien = '{cboMaNV.SelectedValue}',
                MaNhaCungCap = '{cboMaNCC.SelectedValue}',
                MaChiNhanh = '{cboMaCN.SelectedValue}',
                NgayNhap = '{dtpNgayNhap.Value:yyyy-MM-dd}',
                GhiChu = N'{txtGhiChu.Text}'
            WHERE MaPhieuNhap = '{maPN}'
        ";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Sửa phiếu nhập thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sửa thất bại! Không tìm thấy mã phiếu.",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sửa thất bại!\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaPN.Text))
            {
                MessageBox.Show("Mã phiếu nhập trống!");
                return;
            }

            FrmInPhieuNhap f = new FrmInPhieuNhap(txtMaPN.Text); // truyền mã PX
            f.ShowDialog();
        }

    }
}
