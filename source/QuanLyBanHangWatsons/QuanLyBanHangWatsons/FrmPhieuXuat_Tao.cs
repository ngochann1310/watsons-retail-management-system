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
    public partial class FrmPhieuXuat_Tao : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maPX;
        bool dangNhapCT = false;

        public FrmPhieuXuat_Tao(string mapx, string mode)
        {
            InitializeComponent();
            this.mode = mode;
            this.maPX = mapx;

            if (mode == "add")
            {
                txtMaPX.Enabled = false;
            }

            if (mode == "edit")
            {
                btnThem.Enabled = false;
                txtMaPX.Enabled = false;
                cboMaDH.SelectedIndexChanged -= cboMaDH_SelectedIndexChanged;

                LoadDetail();
                LoadCTPhieuXuatTheoMaPX();
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }

        private void LoadCTPhieuXuatTheoMaPX()
        {
            string sql = $@"
        SELECT ct.MaSanPham, sp.TenSanPham, ct.SoLuong, ct.DonGia
        FROM tblCTPhieuXuat ct
        JOIN tblSanPham sp ON ct.MaSanPham = sp.MaSanPham
        WHERE ct.MaPhieuXuat = '{maPX}'
    ";

            DataTable dt = kn.ExecuteQuery(sql);

            dgvPX_SPChon.Rows.Clear();

            foreach (DataRow r in dt.Rows)
            {
                dgvPX_SPChon.Rows.Add(
                    r["MaSanPham"],
                    r["TenSanPham"],
                    r["SoLuong"],
                    r["DonGia"]
                );
            }

            CapNhatTongTien();
            if (string.IsNullOrWhiteSpace(txtMaPX.Text)) return;
        }


        private void FrmPhieuXuat_Tao_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaNV();
            LoadComboBoxMaCN();
            LoadComboBoxMaDH();

            SetupDgvPX_SPChon();

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaPX.Text))
                    txtMaPX.Text = GenerateNewMaPhieuXuat();

                // ADD → chỉ load theo đơn hàng
                cboMaDH.SelectedIndexChanged += cboMaDH_SelectedIndexChanged;
            }
            else if (mode == "edit")
            {
                LoadDetail();

                // EDIT → không cho load theo MaDH
                cboMaDH.SelectedIndexChanged -= cboMaDH_SelectedIndexChanged;

                // EDIT → phải load từ tblCTPhieuXuat
                LoadCTPhieuXuatTheoMaPX();
            }

            dgvPX_SPChon.CellDoubleClick += dgvPX_SPChon_CellDoubleClick;

        }

        //Tạo mã phiếu nhập tự động
        private string GenerateNewMaPhieuXuat()
        {
            // Lấy mã lớn nhất hiện có, tìm phần số cuối, tăng +1
            // Nếu DB có mã dạng "PN0001" hoặc tương tự, code này sẽ xử lý.
            string sql = "SELECT TOP 1 MaPhieuXuat FROM tblPhieuXuat ORDER BY MaPhieuXuat DESC";
            var obj = kn.ExecuteScalar(sql);
            if (obj == null || obj == DBNull.Value) return "PX0001";

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
            SELECT px.*, 
                   nv.HoNhanVien + ' ' + nv.TenNhanVien AS HoTenNV,
                   dh.MaDonHang
            FROM tblPhieuXuat px
            JOIN tblNhanVien nv ON px.MaNhanVien = nv.MaNhanVien
            JOIN tblDonHang dh ON px.MaDonHang = dh.MaDonHang
            WHERE px.MaPhieuXuat = '{maPX}'
            ";

            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            txtMaPX.Text = r["MaPhieuXuat"].ToString();
            cboMaNV.SelectedValue = r["MaNhanVien"].ToString();
            cboMaDH.SelectedValue = r["MaDonHang"].ToString();
            cboMaCN.SelectedValue = r["MaChiNhanh"].ToString();
            dtpNgayXuat.Value = Convert.ToDateTime(r["NgayXuat"]);
            txtTongTienHang.Text = r["TongTien"].ToString();
            txtGhiChu.Text = r["GhiChu"].ToString();
        }

        private void SetupDgvPX_SPChon()
        {
            dgvPX_SPChon.AutoGenerateColumns = false;
            dgvPX_SPChon.AllowUserToAddRows = false;
            dgvPX_SPChon.Columns.Clear();

            // 2. MaSP 
            DataGridViewTextBoxColumn colMaSP = new DataGridViewTextBoxColumn();
            colMaSP.Name = "MaSanPham";
            colMaSP.HeaderText = "Mã sản phẩm";
            dgvPX_SPChon.Columns.Add(colMaSP);

            // 3. TenSP
            DataGridViewTextBoxColumn colTen = new DataGridViewTextBoxColumn();
            colTen.Name = "TenSP";
            colTen.HeaderText = "Tên sản phẩm";
            dgvPX_SPChon.Columns.Add(colTen);

            // 4. SoLuong
            DataGridViewTextBoxColumn colSL = new DataGridViewTextBoxColumn();
            colSL.Name = "SoLuong";
            colSL.HeaderText = "Số lượng";
            dgvPX_SPChon.Columns.Add(colSL);

            // 5. GiaNhap
            DataGridViewTextBoxColumn colGia = new DataGridViewTextBoxColumn();
            colGia.Name = "DonGia";
            colGia.HeaderText = "Đơn giá";
            dgvPX_SPChon.Columns.Add(colGia);          

            // Căn giữa cho các cột cần thiết
            dgvPX_SPChon.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPX_SPChon.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPX_SPChon.Columns["MaSanPham"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            // Căn giữa luôn header cho đẹp
            dgvPX_SPChon.Columns["MaSanPham"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPX_SPChon.Columns["TenSP"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPX_SPChon.Columns["SoLuong"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPX_SPChon.Columns["DonGia"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;



            // Thiết lập mặc định hàng/column sizing
            dgvPX_SPChon.RowTemplate.Height = 60;
            dgvPX_SPChon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        private void LoadComboBoxMaDH()
        {
            string query = "SELECT MaDonHang FROM tblDonHang";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaDH.DataSource = dt;
            cboMaDH.DisplayMember = "MaDonHang";
            cboMaDH.ValueMember = "MaDonHang";
            cboMaDH.SelectedIndex = -1;

            cboMaDH.SelectedIndexChanged -= cboMaDH_SelectedIndexChanged;
            cboMaDH.SelectedIndexChanged += cboMaDH_SelectedIndexChanged;
        }

        private void cboMaNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaNV.SelectedIndex == -1 || cboMaNV.SelectedValue == null)
            {
                lblTenNV.Text = "";
                return;
            }

            var kq = kn.ExecuteScalar(
                $"SELECT HoNhanVien + ' ' + TenNhanVien FROM tblNhanVien WHERE MaNhanVien = '{cboMaNV.SelectedValue}'"
            );

            lblTenNV.Text = (kq == null || kq == DBNull.Value) ? "" : kq.ToString();
        }

        private void LoadMaCNTheoMaDH(string maDH)
        {
            string sql = $@"
        SELECT MaChiNhanh 
        FROM tblDonHang
        WHERE MaDonHang = '{maDH}'
    ";

            var maCN = kn.ExecuteScalar(sql);

            if (maCN != null && maCN != DBNull.Value)
            {
                cboMaCN.SelectedValue = maCN.ToString();
            }
            else
            {
                cboMaCN.SelectedIndex = -1;
            }
        }


        private void cboMaDH_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mode == "edit") return;
            if (!this.Visible) return;
            if (dangNhapCT) return; 

            if (cboMaDH.SelectedIndex == -1 || cboMaDH.SelectedValue == null)
                return;

            string maDH = cboMaDH.SelectedValue.ToString();

            LoadMaCNTheoMaDH(maDH);
            LoadCTPhieuXuatTheoMaPX();
        }


        private void dgvPX_SPChon_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string maSP = dgvPX_SPChon.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();
            int sl = Convert.ToInt32(dgvPX_SPChon.Rows[e.RowIndex].Cells["SoLuong"].Value);
            decimal gia = Convert.ToDecimal(dgvPX_SPChon.Rows[e.RowIndex].Cells["DonGia"].Value);

            dangNhapCT = true;

            FrmNhapCTPX f = new FrmNhapCTPX(
                txtMaPX.Text,
                cboMaDH.SelectedValue.ToString(),
                maSP,
                sl,
                gia
            );

            f.FormClosed += (s, a) =>
            {
                dangNhapCT = false;
                LoadCTPhieuXuatTheoMaPX();
                CapNhatTongTien();
            };

            f.ShowDialog();

        }

        private void LoadDanhSachSanPham()
        {
            if (cboMaDH.SelectedIndex == -1 || cboMaDH.SelectedValue == null)
            {
                dgvPX_SPChon.Rows.Clear();
                txtTongTienHang.Text = "0";
                return;
            }

            string maDH = cboMaDH.SelectedValue.ToString();

            string sql = $@"
        SELECT ctdh.MaSanPham, sp.TenSanPham, ctdh.SoLuong, ctdh.DonGia
        FROM tblChiTietDonHang ctdh
        JOIN tblSanPham sp ON ctdh.MaSanPham = sp.MaSanPham
        WHERE ctdh.MaDonHang = '{maDH}'
    ";

            DataTable dt = kn.ExecuteQuery(sql);

            dgvPX_SPChon.Rows.Clear();

            foreach (DataRow r in dt.Rows)
            {
                dgvPX_SPChon.Rows.Add(
                    r["MaSanPham"].ToString(),
                    r["TenSanPham"].ToString(),
                    r["SoLuong"].ToString(),
                    r["DonGia"].ToString()
                );
            }

            // Cập nhật tổng tiền
            CapNhatTongTien();
        }

        private void CapNhatTongTien()
        {
            decimal tong = 0;

            foreach (DataGridViewRow row in dgvPX_SPChon.Rows)
            {
                if (row.IsNewRow) continue;

                decimal sl = 0, gia = 0;

                decimal.TryParse(row.Cells["SoLuong"].Value?.ToString(), out sl);
                decimal.TryParse(row.Cells["DonGia"].Value?.ToString(), out gia);

                tong += sl * gia;
            }

            txtTongTienHang.Text = tong.ToString("0");
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cboMaNV.SelectedIndex == -1 ||
        cboMaDH.SelectedIndex == -1 ||
        cboMaCN.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            // tạo mã mới nếu chưa có
            if (string.IsNullOrWhiteSpace(txtMaPX.Text))
                txtMaPX.Text = GenerateNewMaPhieuXuat();

            // kiểm tra PX đã tồn tại chưa
            var kt = kn.ExecuteScalar($"SELECT COUNT(*) FROM tblPhieuXuat WHERE MaPhieuXuat = '{txtMaPX.Text}'");

            bool isNewInsert = Convert.ToInt32(kt) == 0;

            if (isNewInsert)
            {
                string sqlInsert = $@"
            INSERT INTO tblPhieuXuat 
                (MaPhieuXuat, MaDonHang, MaNhanVien, MaChiNhanh, NgayXuat, TongTien, GhiChu)
            VALUES
                ('{txtMaPX.Text}',
                 '{cboMaDH.SelectedValue}',
                 '{cboMaNV.SelectedValue}',
                 '{cboMaCN.SelectedValue}',
                 '{dtpNgayXuat.Value:yyyy-MM-dd}',
                 0,
                 N'{txtGhiChu.Text}')
        ";

                kn.ExecuteNonQuery(sqlInsert);
                MessageBox.Show("Tạo phiếu xuất thành công! Hãy thêm sản phẩm vào phiếu.");
            }

            // Không xóa dgvPX_SPChon — GIỮ NGUYÊN
            // Không load CTPhieuXuat — vì lúc này chi tiết chưa có dữ liệu
            dangNhapCT = true;
            // mở form nhập chi tiết
            FrmNhapCTPX f = new FrmNhapCTPX(
             txtMaPX.Text,
             cboMaDH.SelectedValue.ToString()
            );


            f.FormClosed += (s, a) =>
            {
                dangNhapCT = false;
                // Sau khi thêm chi tiết xong → load từ DB
                LoadCTPhieuXuatTheoMaPX();
                CapNhatTongTien();

                // update tổng tiền vào bảng
                string sqlUpdate = $@"
            UPDATE tblPhieuXuat
            SET TongTien = {txtTongTienHang.Text}
            WHERE MaPhieuXuat = '{txtMaPX.Text}'
        ";
                kn.ExecuteNonQuery(sqlUpdate);
            };

            f.ShowDialog();
        }

        private void F_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoadCTPhieuXuatTheoMaPX();  // load từ bảng CT
            CapNhatTongTien();     // lấy tổng tiền từ SQL
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (mode != "edit")
            {
                MessageBox.Show("Không ở chế độ sửa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa phiếu xuất này?",
                "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            kn.ExecuteNonQuery($"DELETE FROM tblCTPhieuXuat WHERE MaPhieuXuat = '{txtMaPX.Text}'");
            kn.ExecuteNonQuery($"DELETE FROM tblPhieuXuat WHERE MaPhieuXuat = '{txtMaPX.Text}'");

            MessageBox.Show("Xóa phiếu xuất thành công!");
            this.Close();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (mode != "edit")
            {
                MessageBox.Show("Không ở chế độ sửa!");
                return;
            }

            string sql = $@"
        UPDATE tblPhieuXuat
        SET MaDonHang = '{cboMaDH.SelectedValue}',
            MaNhanVien = '{cboMaNV.SelectedValue}',
            MaChiNhanh = '{cboMaCN.SelectedValue}',
            NgayXuat = '{dtpNgayXuat.Value:yyyy-MM-dd}',
            GhiChu = N'{txtGhiChu.Text}'
        WHERE MaPhieuXuat = '{txtMaPX.Text}'
    ";

            kn.ExecuteNonQuery(sql);

            // Xóa hết chi tiết cũ
            kn.ExecuteNonQuery($"DELETE FROM tblCTPhieuXuat WHERE MaPhieuXuat = '{txtMaPX.Text}'");

            // Insert lại chi tiết
            foreach (DataGridViewRow r in dgvPX_SPChon.Rows)
            {
                string maSP = r.Cells["MaSanPham"].Value.ToString();
                string sl = r.Cells["SoLuong"].Value.ToString();
                string gia = r.Cells["DonGia"].Value.ToString();

                string sqlCT = $@"
            INSERT INTO tblCTPhieuXuat (MaPhieuXuat, MaSanPham, SoLuong, DonGia)
            VALUES ('{txtMaPX.Text}', '{maSP}', {sl}, {gia})
        ";

                kn.ExecuteNonQuery(sqlCT);
            }

            MessageBox.Show("Cập nhật phiếu xuất thành công!");
            LoadDetail();                    // load lại thông tin phiếu xuất
            LoadCTPhieuXuatTheoMaPX();       // load lại chi tiết từ DB
            CapNhatTongTien();

            string sqlUpdateDH = $@"
            UPDATE tblDonHang
            SET TrangThaiDonHang = N'Đã lấy hàng'
            WHERE MaDonHang = '{cboMaDH.SelectedValue}'
            ";

            kn.ExecuteNonQuery(sqlUpdateDH);
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaPX.Text))
            {
                MessageBox.Show("Mã phiếu xuất trống!");
                return;
            }

            FrmInPhieuXuat f = new FrmInPhieuXuat(txtMaPX.Text); // truyền mã PX
            f.ShowDialog();
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            if (cboMaNV.SelectedIndex == -1 ||
                    cboMaDH.SelectedIndex == -1 ||
                    cboMaCN.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            // mã phiếu phải lấy từ textbox, không dùng maPX trong constructor
            string mapx = txtMaPX.Text;
            dangNhapCT = true;
            FrmNhapCTPX f = new FrmNhapCTPX(
            txtMaPX.Text,
            cboMaDH.SelectedValue.ToString()
            );

            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadCTPhieuXuatTheoMaPX(); // 🔥 LOAD LẠI TỪ DB
                CapNhatTongTien();
            }

            dangNhapCT = false;
        }
    }
}
