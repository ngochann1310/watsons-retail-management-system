using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace QuanLyBanHangWatsons
{
    public partial class FrmDonHang_TaoDon : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode = "";     // add | edit
        string maDH = "";     // mã đơn
        

        public FrmDonHang_TaoDon(string _maDH, string _mode)
        {
            InitializeComponent();
            maDH = _maDH;
            mode = _mode;

            if (mode == "add")
            {
                txtMaDH.Enabled = false;
            }

            if (mode == "edit")
            {
                txtMaDH.Enabled = false;
                btnThem.Enabled = false;
            }
            else
            {
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }
        bool isLoading = true;
        private void FrmDonHang_TaoDon_Load(object sender, EventArgs e)
        {
            isLoading = true;

            LoadComboMaCN();
            LoadComboMaNV();
            LoadComboMaKH();          
            LoadComboMaKM();

            SetupDgvSanPhamChon();

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaDH.Text))
                    txtMaDH.Text = GenerateNewMaDH();
            }


            if (mode == "edit")
            {
                LoadDetail();
            }

            dgvDH_SPChon.CellClick += dgvDH_SPChon_CellClick;
            dgvDH_SPChon.CellDoubleClick += dgvDH_SPChon_CellDoubleClick;


            //LoadDanhSachSanPham();

            cboMaNV.SelectedIndexChanged += cboMaNV_SelectedIndexChanged;
            cboMaKH.SelectedIndexChanged += cboMaKH_SelectedIndexChanged;
            cboMaKM.SelectedIndexChanged += cboMaKM_SelectedIndexChanged;

            if (mode == "edit")
            {
                LoadDgv_SanPhamTrongDon();
                TinhTien();
            }
            else
            {
                // mới tạo: clear dgv
                dgvDH_SPChon.Rows.Clear();
            }

            isLoading = false;   

            TinhTien();

            timerDangGiaoHang = new Timer();
            timerDangGiaoHang.Interval = 20000; // 20s
            timerDangGiaoHang.Tick += timerDangGiaoHang_Tick;

            timerGiaoHangThanhCong = new Timer();
            timerGiaoHangThanhCong.Interval = 10000; // 10s
            timerGiaoHangThanhCong.Tick += timerGiaoHangThanhCong_Tick;
            CheckAndStartTimerTheoTrangThai();
        }


        private void LoadComboMaCN()
        {
            string sql = "SELECT MaChiNhanh FROM tblChiNhanh";
            cboMaCN.DataSource = kn.ExecuteQuery(sql);
            cboMaCN.ValueMember = "MaChiNhanh";
            cboMaCN.DisplayMember = "MaChiNhanh";
            cboMaCN.SelectedIndex = -1;
        }

        private void LoadComboMaNV()
        {
            string sql = "SELECT MaNhanVien, (HoNhanVien + ' ' + TenNhanVien) AS HoTen FROM tblNhanVien";
            DataTable dt = kn.ExecuteQuery(sql);

            cboMaNV.DataSource = dt;
            cboMaNV.ValueMember = "MaNhanVien";
            cboMaNV.DisplayMember = "MaNhanVien"; // giữ giống mẫu; hiện tên bằng label
            cboMaNV.SelectedIndex = -1;
        }

        private void LoadComboMaKH()
        {
            string sql = "SELECT MaKhachHang, TenKhachHang, DienThoai, DiemTichLuy, DiaChi FROM tblKhachHang";
            DataTable dt = kn.ExecuteQuery(sql);
            cboMaKH.DataSource = dt;
            cboMaKH.ValueMember = "MaKhachHang";
            cboMaKH.DisplayMember = "MaKhachHang";
            cboMaKH.SelectedIndex = -1;
        }

        private void LoadComboMaKM()
        {
            try
            {
                string sql = "SELECT MaKhuyenMai, GiaTri FROM tblKhuyenMai WHERE TrangThaiKhuyenMai = 1";
                cboMaKM.DataSource = kn.ExecuteQuery(sql);
            }
            catch
            {
                cboMaKM.DataSource = kn.ExecuteQuery("SELECT MaKhuyenMai FROM tblKhuyenMai");
            }

            cboMaKM.ValueMember = "MaKhuyenMai";
            cboMaKM.DisplayMember = "MaKhuyenMai";
            cboMaKM.SelectedIndex = -1;
        }

        private string GenerateNewMaDH()
        {
            string sql = "SELECT TOP 1 MaDonHang FROM tblDonHang ORDER BY MaDonHang DESC";
            var obj = kn.ExecuteScalar(sql);
            if (obj == null || obj == DBNull.Value) return "DH0001";

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
            string sql = @"
            SELECT dh.*,
                   nv.HoNhanVien + ' ' + nv.TenNhanVien AS HoTenNV,
                   kh.TenKhachHang, kh.DienThoai, kh.DiemTichLuy, kh.DiaChi
            FROM tblDonHang dh
            LEFT JOIN tblNhanVien nv ON dh.MaNhanVien = nv.MaNhanVien
            LEFT JOIN tblKhachHang kh ON dh.MaKhachHang = kh.MaKhachHang
            WHERE dh.MaDonHang = @MaDH
        ";

                    SqlParameter[] pr =
                    {
            new SqlParameter("@MaDH", maDH)
        };

            DataTable dt = kn.ExecuteQueryWithParams(sql, pr);

            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            txtMaDH.Text = r["MaDonHang"].ToString();

            if (r["MaChiNhanh"] != DBNull.Value) cboMaCN.SelectedValue = r["MaChiNhanh"].ToString();
            if (r["MaNhanVien"] != DBNull.Value) cboMaNV.SelectedValue = r["MaNhanVien"].ToString();
            if (r["MaKhachHang"] != DBNull.Value) cboMaKH.SelectedValue = r["MaKhachHang"].ToString();
            cboLoaiDH.Text = r["LoaiDonHang"]?.ToString() ?? "";
            if (r["NgayDatHang"] != DBNull.Value)
            {
                dtpNDH.Value = Convert.ToDateTime(r["NgayDatHang"]);
            }
            else
            {
                dtpNDH.Value = DateTime.Now;
            }
            cboTrangThaiDH.Text = r["TrangThaiDonHang"]?.ToString() ?? "";
            if (r["MaKhuyenMai"] != DBNull.Value)
            {
                cboMaKM.SelectedValue = r["MaKhuyenMai"].ToString();

                string maKM = r["MaKhuyenMai"].ToString();

                // Lấy giá trị KM theo cách hàm của bà hỗ trợ
                var gtriKM = kn.ExecuteScalar(
                    $"SELECT GiaTri FROM tblKhuyenMai WHERE MaKhuyenMai = '{maKM}'"
                );

                if (gtriKM != null && gtriKM != DBNull.Value)
                {
                    decimal gtri = Convert.ToDecimal(gtriKM);
                    if (gtri < 1) // phần trăm
                        txtGTGiam.Text = (gtri * 100).ToString("0") + "%";
                    else // VND
                        txtGTGiam.Text = gtri.ToString("N0");
                }
            }
            else
            {
                txtGTGiam.Text = "0";
            }
            txtGhiChu.Text = r["GhiChu"]?.ToString() ?? "";

            lblHoTenNV.Text = r["HoTenNV"]?.ToString() ?? "";
            lblTenKH.Text = r["TenKhachHang"]?.ToString() ?? "";
            lblSDTKH.Text = r["DienThoai"]?.ToString() ?? "";        // ✔️ FIX
            lblDiemTichLuy.Text = r["DiemTichLuy"]?.ToString() ?? "0";
            lblDiaChi.Text = r["DiaChi"]?.ToString() ?? "";

            if (r["MaKhuyenMai"] != DBNull.Value)
            {
                string mk = r["MaKhuyenMai"].ToString();
                cboMaKM.SelectedIndex = -1;  // reset
                cboMaKM.SelectedValue = mk;  // set lại
            }

            LoadDgv_SanPhamTrongDon();
            TinhTien();
        }



        private void SetupDgvSanPhamChon()
        {
            dgvDH_SPChon.AutoGenerateColumns = false;
            dgvDH_SPChon.AllowUserToAddRows = false;
            dgvDH_SPChon.Columns.Clear();

            // 1. Ảnh
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "HinhAnh";
            imgCol.HeaderText = "Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCol.Width = 60;
            dgvDH_SPChon.Columns.Add(imgCol);

            // 2. MaSP (ẩn)
            DataGridViewTextBoxColumn colMaSP = new DataGridViewTextBoxColumn();
            colMaSP.Name = "MaSanPham";
            colMaSP.HeaderText = "Mã SP";
            colMaSP.Visible = false;
            dgvDH_SPChon.Columns.Add(colMaSP);

            // 3. TenSP
            DataGridViewTextBoxColumn colTen = new DataGridViewTextBoxColumn();
            colTen.Name = "TenSanPham";
            colTen.HeaderText = "Tên sản phẩm";
            dgvDH_SPChon.Columns.Add(colTen);

            // 4. SoLuong
            DataGridViewTextBoxColumn colSL = new DataGridViewTextBoxColumn();
            colSL.Name = "SoLuong";
            colSL.HeaderText = "Số lượng";
            dgvDH_SPChon.Columns.Add(colSL);

            // 5. DonGia
            DataGridViewTextBoxColumn colGia = new DataGridViewTextBoxColumn();
            colGia.Name = "DonGia";
            colGia.HeaderText = "Đơn giá";
            dgvDH_SPChon.Columns.Add(colGia);

            // 6. ThanhTien
            DataGridViewTextBoxColumn colThanh = new DataGridViewTextBoxColumn();
            colThanh.Name = "ThanhTien";
            colThanh.HeaderText = "Thành tiền";
            dgvDH_SPChon.Columns.Add(colThanh);

            // 7. Sửa bằng icon
            DataGridViewImageColumn colSua = new DataGridViewImageColumn();
            colSua.Name = "Sua";
            colSua.HeaderText = "";
            string pathSua = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnSua.png");
            if (File.Exists(pathSua)) colSua.Image = Image.FromFile(pathSua);
            colSua.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colSua.Width = 48;
            dgvDH_SPChon.Columns.Add(colSua);

            // 8. Xóa bằng icon
            DataGridViewImageColumn colXoa = new DataGridViewImageColumn();
            colXoa.Name = "Xoa";
            colXoa.HeaderText = "";
            string pathXoa = Path.Combine(Application.StartupPath, @"..\..\tool\icon\btnXoa.png");
            if (File.Exists(pathXoa)) colXoa.Image = Image.FromFile(pathXoa);
            colXoa.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colXoa.Width = 48;
            dgvDH_SPChon.Columns.Add(colXoa);

            // style
            dgvDH_SPChon.RowTemplate.Height = 60;
            dgvDH_SPChon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDH_SPChon.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDH_SPChon.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDH_SPChon.Columns["ThanhTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvDH_SPChon.Columns["HinhAnh"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void LoadDgv_SanPhamTrongDon()
        {
            dgvDH_SPChon.Rows.Clear();

            if (string.IsNullOrWhiteSpace(txtMaDH.Text)) return;

                    string sql = @"
            SELECT ctdh.MaSanPham, sp.TenSanPham, sp.HinhAnh,
                   ctdh.SoLuong, ctdh.DonGia,
                   (ctdh.SoLuong * ctdh.DonGia) AS ThanhTien
            FROM tblChiTietDonHang ctdh
            LEFT JOIN tblSanPham sp ON ctdh.MaSanPham = sp.MaSanPham
            WHERE ctdh.MaDonHang = @MaDH
        ";

                    SqlParameter[] pr =
                    {
            new SqlParameter("@MaDH", txtMaDH.Text.Trim())
};

            DataTable dt = kn.ExecuteQueryWithParams(sql, pr);

            foreach (DataRow row in dt.Rows)
            {
                Image img = null;
                try
                {
                    string file = row["HinhAnh"]?.ToString();
                    if (!string.IsNullOrEmpty(file))
                    {
                        string full = Path.Combine(Application.StartupPath, @"..\..\Images\", file);
                        if (File.Exists(full))
                        {
                            using (FileStream fs = new FileStream(full, FileMode.Open, FileAccess.Read))
                            {
                                img = Image.FromStream(fs).Clone() as Image;
                            }
                        }
                    }
                }
                catch { }

                dgvDH_SPChon.Rows.Add(
                    img,
                    row["MaSanPham"].ToString(),
                    row["TenSanPham"].ToString(),
                    row["SoLuong"].ToString(),
                    Convert.ToDecimal(row["DonGia"]).ToString("N0"),
                    Convert.ToDecimal(row["ThanhTien"]).ToString("N0")
                );
            }
        }

        private void cboMaNV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaNV.SelectedIndex == -1 || cboMaNV.SelectedValue == null)
            {
                lblHoTenNV.Text = "";
                return;
            }

            var kq = kn.ExecuteScalar($"SELECT HoNhanVien + ' ' + TenNhanVien FROM tblNhanVien WHERE MaNhanVien = '{cboMaNV.SelectedValue}'");
            lblHoTenNV.Text = (kq == null || kq == DBNull.Value) ? "" : kq.ToString();
        }

        private void cboMaKH_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaKH.SelectedIndex == -1 || cboMaKH.SelectedValue == null)
            {
                lblTenKH.Text = lblSDTKH.Text = lblDiemTichLuy.Text = lblDiaChi.Text = "";
                return;
            }

            LoadThongTinKhachHang(cboMaKH.SelectedValue.ToString());
        }

        private void cboMaKM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
            // Lấy giá trị giảm giá
            if (cboMaKM.SelectedIndex != -1 && cboMaKM.SelectedValue != null)
            {
                var kq = kn.ExecuteScalar($"SELECT GiaTri FROM tblKhuyenMai WHERE MaKhuyenMai = '{cboMaKM.SelectedValue}'");
                if (kq != null && kq != DBNull.Value)
                {
                    decimal gtri = Convert.ToDecimal(kq);

                    if (gtri < 1) // giá trị dạng %, ví dụ 0.12
                        txtGTGiam.Text = (gtri * 100).ToString("0") + "%";
                    else // giá trị dạng VND
                        txtGTGiam.Text = gtri.ToString("N0");
                }
                else
                    txtGTGiam.Text = "0";
            }
            else
            {
                txtGTGiam.Text = "0";
            }

            TinhTien();
        }

        private void dgvDH_SPChon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dgvDH_SPChon.Columns[e.ColumnIndex].Name;
            string maSP = dgvDH_SPChon.Rows[e.RowIndex].Cells["MaSanPham"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(maSP)) return;

            if (col == "Sua")
            {
                FrmNhapCTDH f = new FrmNhapCTDH(txtMaDH.Text, maSP, "edit");
                f.ShowDialog();
                AutoSelectBestKhuyenMai();
                LoadDgv_SanPhamTrongDon();
                TinhTien();
            }
            else if (col == "Xoa")
            {
                if (MessageBox.Show("Bạn muốn xóa sản phẩm khỏi đơn?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    kn.ExecuteNonQuery($@"
                DELETE FROM tblChiTietDonHang
                WHERE MaDonHang = '{txtMaDH.Text}' AND MaSanPham = '{maSP}'
            ");

                    LoadDgv_SanPhamTrongDon();
                    AutoSelectBestKhuyenMai();
                    TinhTien();
                }
            }
        }

        private void dgvDH_SPChon_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string maSP = dgvDH_SPChon.Rows[e.RowIndex].Cells["MaSanPham"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(maSP)) return;

            FrmNhapCTDH f = new FrmNhapCTDH(txtMaDH.Text, maSP, "edit");
            f.ShowDialog();
            LoadDgv_SanPhamTrongDon();
            AutoSelectBestKhuyenMai();
            TinhTien();
        }

        private void AutoSelectBestKhuyenMai()
        {
            if (isLoading) return;
            if (string.IsNullOrWhiteSpace(txtMaDH.Text)) return;

            var obj = kn.ExecuteScalar(
                $"SELECT dbo.fn_ChonKhuyenMaiTotNhat('{txtMaDH.Text.Trim()}')"
            );

            if (obj == null || obj == DBNull.Value)
            {
                cboMaKM.SelectedIndex = -1;
                txtGTGiam.Text = "0";
                return;
            }

            string maKM = obj.ToString();

            // ÉP SelectedIndexChanged chạy lại
            cboMaKM.SelectedIndex = -1;
            cboMaKM.SelectedValue = maKM;
        }


        private void TinhTien()
        {
            if (isLoading) return;

            decimal tong = 0;

            foreach (DataGridViewRow r in dgvDH_SPChon.Rows)
            {
                if (r.Cells["ThanhTien"].Value != null)
                {
                    string tt = r.Cells["ThanhTien"].Value.ToString().Replace(",", "");
                    if (decimal.TryParse(tt, out decimal v)) tong += v;
                }
            }

            txtTongTienHang.Text = tong.ToString("N0");

            // Lấy giá trị giảm giá
            decimal giamKM = 0;

            int giamDiem = 0;
            int.TryParse(txtDiemSuDung.Text.Replace(",", ""), out giamDiem);

            if (cboMaKM.SelectedIndex != -1 && cboMaKM.SelectedValue != null)
            {
                var gtriObj = kn.ExecuteScalar($"SELECT GiaTri FROM tblKhuyenMai WHERE MaKhuyenMai = '{cboMaKM.SelectedValue}'");
                if (gtriObj != null && gtriObj != DBNull.Value)
                {
                    decimal gtri = Convert.ToDecimal(gtriObj);

                    if (gtri < 1) // phần trăm
                    {
                        giamKM = Math.Round(tong * gtri, 0); // nhân tổng tiền
                    }
                    else // VND
                    {
                        giamKM = gtri;
                    }
                }
            }

            txtTienGiam.Text = (giamKM + giamDiem).ToString("N0");

            decimal thanhTien = tong - giamKM - giamDiem;
            if (thanhTien < 0) thanhTien = 0;
            txtThanhTien.Text = thanhTien.ToString("N0");

            // Cập nhật vào DB nếu đang ở mode edit
            if (mode == "edit" && !string.IsNullOrWhiteSpace(txtMaDH.Text))
            {
                string sql = @"
            UPDATE tblDonHang SET
                TongTienHang = @TongTienHang,
                DiemSuDung = @DiemSuDung,
                ThanhTien = @ThanhTien
            WHERE MaDonHang = @MaDH
        ";

                SqlParameter[] pr =
                {
            new SqlParameter("@TongTienHang", tong),
            new SqlParameter("@DiemSuDung", giamDiem),
            new SqlParameter("@ThanhTien", thanhTien),
            new SqlParameter("@MaDH", txtMaDH.Text.Trim())
        };

                kn.ExecuteNonQueryWithParams(sql, pr);
            }
        }



        private void RefreshDonHang()
        {
            if (string.IsNullOrWhiteSpace(txtMaDH.Text)) return;

            try
            {
                // 1️⃣ Load thông tin cơ bản đơn hàng + khách hàng + nhân viên
                string sql = @"
            SELECT dh.*,
                   nv.HoNhanVien + ' ' + nv.TenNhanVien AS HoTenNV,
                   kh.TenKhachHang, kh.DienThoai, kh.DiemTichLuy, kh.DiaChi
            FROM tblDonHang dh
            LEFT JOIN tblNhanVien nv ON dh.MaNhanVien = nv.MaNhanVien
            LEFT JOIN tblKhachHang kh ON dh.MaKhachHang = kh.MaKhachHang
            WHERE dh.MaDonHang = @MaDH
        ";

                SqlParameter[] pr =
                {
            new SqlParameter("@MaDH", txtMaDH.Text.Trim())
        };

                DataTable dt = kn.ExecuteQueryWithParams(sql, pr);
                if (dt.Rows.Count == 0) return;

                DataRow r = dt.Rows[0];

                txtMaDH.Text = r["MaDonHang"].ToString();
                cboMaCN.SelectedValue = r["MaChiNhanh"] != DBNull.Value ? r["MaChiNhanh"] : -1;
                cboMaNV.SelectedValue = r["MaNhanVien"] != DBNull.Value ? r["MaNhanVien"] : -1;
                cboMaKH.SelectedValue = r["MaKhachHang"] != DBNull.Value ? r["MaKhachHang"] : -1;
                cboLoaiDH.Text = r["LoaiDonHang"]?.ToString() ?? "";
                dtpNDH.Value = r["NgayDatHang"] != DBNull.Value ? Convert.ToDateTime(r["NgayDatHang"]) : DateTime.Now;
                cboTrangThaiDH.Text = r["TrangThaiDonHang"]?.ToString() ?? "";
                cboMaKM.SelectedValue = r["MaKhuyenMai"] != DBNull.Value ? r["MaKhuyenMai"] : -1;
                txtGhiChu.Text = r["GhiChu"]?.ToString() ?? "";

                lblHoTenNV.Text = r["HoTenNV"]?.ToString() ?? "";
                lblTenKH.Text = r["TenKhachHang"]?.ToString() ?? "";
                lblSDTKH.Text = r["DienThoai"]?.ToString() ?? "";
                lblDiemTichLuy.Text = r["DiemTichLuy"]?.ToString() ?? "0";
                lblDiaChi.Text = r["DiaChi"]?.ToString() ?? "";

                // 2️⃣ Load chi tiết đơn hàng vào dgv
                LoadDgv_SanPhamTrongDon();

                // 3️⃣ Tính tổng tiền, giảm giá và điểm tích lũy
                decimal tong = 0;
                foreach (DataGridViewRow row in dgvDH_SPChon.Rows)
                {
                    if (row.Cells["ThanhTien"].Value != null)
                    {
                        string tt = row.Cells["ThanhTien"].Value.ToString().Replace(",", "");
                        if (decimal.TryParse(tt, out decimal v)) tong += v;
                    }
                }
                txtTongTienHang.Text = tong.ToString("N0");

                // Lấy giá trị khuyến mãi
                decimal giamKM = 0;
                if (cboMaKM.SelectedIndex != -1 && cboMaKM.SelectedValue != null)
                {
                    var gtriObj = kn.ExecuteScalar($"SELECT GiaTri FROM tblKhuyenMai WHERE MaKhuyenMai = '{cboMaKM.SelectedValue}'");
                    if (gtriObj != null && gtriObj != DBNull.Value)
                    {
                        decimal gtri = Convert.ToDecimal(gtriObj);
                        if (gtri < 1) // %
                            txtGTGiam.Text = (gtri * 100).ToString("0") + "%";
                        else
                            txtGTGiam.Text = gtri.ToString("N0");

                        giamKM = gtri < 1 ? Math.Round(tong * gtri, 0) : gtri;
                    }
                    else
                    {
                        txtGTGiam.Text = "0";
                        giamKM = 0;
                    }
                }
                else
                {
                    txtGTGiam.Text = "0";
                    giamKM = 0;
                }

                // Điểm sử dụng
                int giamDiem = 0;
                int.TryParse(txtDiemSuDung.Text.Replace(",", ""), out giamDiem);

                txtTienGiam.Text = (giamKM + giamDiem).ToString("N0");

                decimal thanhTien = tong - giamKM - giamDiem;
                if (thanhTien < 0) thanhTien = 0;
                txtThanhTien.Text = thanhTien.ToString("N0");

                // 4️⃣ Cập nhật vào DB luôn nếu đang edit
                if (mode == "edit" && !string.IsNullOrWhiteSpace(txtMaDH.Text))
                {
                    string sqlUpdate = @"
                UPDATE tblDonHang SET
                    TongTienHang = @TongTienHang,
                    DiemSuDung = @DiemSuDung,
                    ThanhTien = @ThanhTien
                WHERE MaDonHang = @MaDH
            ";

                    SqlParameter[] prUpdate =
                    {
                new SqlParameter("@TongTienHang", tong),
                new SqlParameter("@DiemSuDung", giamDiem),
                new SqlParameter("@ThanhTien", thanhTien),
                new SqlParameter("@MaDH", txtMaDH.Text.Trim())
            };

                    kn.ExecuteNonQueryWithParams(sqlUpdate, prUpdate);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể refresh đơn hàng.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (mode != "add") return;

            if (cboMaNV.SelectedIndex == -1 || cboMaCN.SelectedIndex == -1 || cboMaKH.SelectedIndex == -1 || string.IsNullOrWhiteSpace(cboLoaiDH.Text))
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TinhTien();
            string trangThai = "Đã xác nhận";

            string sql = @"
                INSERT INTO tblDonHang
                (MaDonHang, MaChiNhanh, MaNhanVien, MaKhachHang, LoaiDonHang, NgayDatHang, TrangThaiDonHang, MaKhuyenMai, GhiChu,
                 TongTienHang, DiemSuDung, ThanhTien)
                VALUES
                (@MaDonHang, @MaChiNhanh, @MaNhanVien, @MaKhachHang, @LoaiDonHang, @NgayDatHang, @TrangThaiDonHang, @MaKhuyenMai, @GhiChu,
                 @TongTienHang, @DiemSuDung, @ThanhTien)
            ";

            SqlParameter[] pr =
            {
                new SqlParameter("@MaDonHang", txtMaDH.Text.Trim()),
                new SqlParameter("@MaChiNhanh", cboMaCN.SelectedValue),
                new SqlParameter("@MaNhanVien", cboMaNV.SelectedValue),
                new SqlParameter("@MaKhachHang", cboMaKH.SelectedValue),
                new SqlParameter("@LoaiDonHang", cboLoaiDH.Text),
                new SqlParameter("@NgayDatHang", dtpNDH.Value),
                new SqlParameter("@TrangThaiDonHang", trangThai),
                new SqlParameter("@MaKhuyenMai",
                    cboMaKM.SelectedIndex < 0 ? (object)DBNull.Value : cboMaKM.SelectedValue),
                new SqlParameter("@GhiChu", txtGhiChu.Text.Trim()),
                new SqlParameter("@TongTienHang", string.IsNullOrWhiteSpace(txtTongTienHang.Text) ? 0 : decimal.Parse(txtTongTienHang.Text.Replace(",", ""))),
                new SqlParameter("@DiemSuDung", string.IsNullOrWhiteSpace(txtDiemSuDung.Text) ? 0 : decimal.Parse(txtDiemSuDung.Text.Replace(",", ""))),
                new SqlParameter("@ThanhTien", string.IsNullOrWhiteSpace(txtThanhTien.Text) ? 0 : decimal.Parse(txtThanhTien.Text.Replace(",", ""))),
            };

            int rows = kn.ExecuteNonQueryWithParams(sql, pr);

            if (rows > 0)
                {
                    MessageBox.Show("Thêm đơn hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Sau khi thêm, chuyển sang mode edit để có thể thêm chi tiết
                    mode = "edit";
                    btnThem.Enabled = false;
                    btnSua.Enabled = true;
                    btnXoa.Enabled = true;
                    cboTrangThaiDH.Text = "Đã xác nhận";
                RefreshDonHang();
            }
                else
                {
                    MessageBox.Show("Thêm thất bại! Vui lòng kiểm tra dữ liệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }          
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (mode != "edit")
            {
                MessageBox.Show("Chỉ có thể xóa khi đang chỉnh sửa.", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMaDH.Text))
            {
                MessageBox.Show("Mã đơn trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa đơn hàng này? Toàn bộ chi tiết sẽ bị xóa!", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            try
            {
                kn.ExecuteNonQuery($@"DELETE FROM tblChiTietDonHang WHERE MaDonHang = '{txtMaDH.Text}'");
                kn.ExecuteNonQuery($@"DELETE FROM tblDonHang WHERE MaDonHang = '{txtMaDH.Text}'");
                MessageBox.Show("Xóa đơn hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa thất bại!\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            //CapNhatTrangThaiDaXacNhanNeuCan();
            if (mode != "edit") return;
            AutoSelectBestKhuyenMai();
            TinhTien();
            string sql = @"
            UPDATE tblDonHang SET
                MaChiNhanh = @MaChiNhanh,
                MaNhanVien = @MaNhanVien,
                MaKhachHang = @MaKhachHang,
                LoaiDonHang = @LoaiDonHang,
                NgayDatHang = @NgayDatHang,
                TrangThaiDonHang = @TrangThaiDonHang,
                MaKhuyenMai = @MaKhuyenMai,
                GhiChu = @GhiChu,
                TongTienHang = @TongTienHang,
                DiemSuDung = @DiemSuDung,
                ThanhTien = @ThanhTien
            WHERE MaDonHang = @MaDonHang
            ";

            decimal tongTien = 0;
            decimal.TryParse(txtTongTienHang.Text.Replace(",", ""), out tongTien);

            int diemSuDung = 0;
            int.TryParse(txtDiemSuDung.Text.Replace(",", ""), out diemSuDung);

            decimal thanhTien = 0;
            decimal.TryParse(txtThanhTien.Text.Replace(",", ""), out thanhTien);

            SqlParameter[] pr =
            {
                new SqlParameter("@MaDonHang", txtMaDH.Text.Trim()),
                new SqlParameter("@MaChiNhanh", cboMaCN.SelectedValue),
                new SqlParameter("@MaNhanVien", cboMaNV.SelectedValue),
                new SqlParameter("@MaKhachHang", cboMaKH.SelectedValue),
                new SqlParameter("@LoaiDonHang", cboLoaiDH.Text),
                new SqlParameter("@NgayDatHang", dtpNDH.Value),
                new SqlParameter("@TrangThaiDonHang", cboTrangThaiDH.Text),
                new SqlParameter("@MaKhuyenMai", cboMaKM.SelectedValue != null ? cboMaKM.SelectedValue : (object)DBNull.Value),
                new SqlParameter("@GhiChu", txtGhiChu.Text.Trim()),
                new SqlParameter("@TongTienHang", tongTien),
                new SqlParameter("@DiemSuDung", diemSuDung),
                new SqlParameter("@ThanhTien", thanhTien)
            };

            int rows = kn.ExecuteNonQueryWithParams(sql, pr);

            if (rows > 0)
                {
                    MessageBox.Show("Cập nhật đơn hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshDonHang();
                CheckAndStartTimerTheoTrangThai();

            }
                else
                {
                    MessageBox.Show("Cập nhật thất bại! Không tìm thấy mã đơn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
        }

        private void btnDoi_Click(object sender, EventArgs e)
        {
            FrmDonHang_SuaKH f = new FrmDonHang_SuaKH();

            // Nhận mã KH được chọn từ form kia
            f.OnKhachHangSelected = (maKH) =>
            {
                cboMaKH.SelectedValue = maKH;   // tự động đổi KH
                                                // callback: đổi label KH
                LoadThongTinKhachHang(maKH);
            };

            f.ShowDialog();
        }

        private void LoadThongTinKhachHang(string maKH)
        {
            string sql = $@"SELECT TenKhachHang, DienThoai, DiemTichLuy, DiaChi 
                    FROM tblKhachHang WHERE MaKhachHang = '{maKH}'";

            DataTable dt = kn.ExecuteQuery(sql);
            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            lblTenKH.Text = r["TenKhachHang"]?.ToString() ?? "";
            lblSDTKH.Text = r["DienThoai"]?.ToString() ?? "";
            lblDiemTichLuy.Text = r["DiemTichLuy"]?.ToString() ?? "";
            lblDiaChi.Text = r["DiaChi"]?.ToString() ?? "";
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaDH.Text))
            {
                MessageBox.Show("Mã đơn trống. Vui lòng tạo đơn trước.", "Lưu ý");
                return;
            }

            FrmNhapCTDH f = new FrmNhapCTDH(txtMaDH.Text, "", "add");
            f.ShowDialog();

            LoadDgv_SanPhamTrongDon();
            AutoSelectBestKhuyenMai();
            TinhTien();

            //CapNhatTrangThaiDaXacNhanNeuCan();
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaDH.Text))
            {
                MessageBox.Show("Chưa có mã đơn hàng. Lưu đơn hàng trước khi thanh toán!",
                                "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maDH = txtMaDH.Text.Trim();

            // 1️⃣ Kiểm tra đơn hàng đã có thanh toán chưa
            string sql = "SELECT MaThanhToan FROM tblThanhToan WHERE MaDonHang = @MaDH";
            SqlParameter[] pr =
            {
        new SqlParameter("@MaDH", maDH)
    };

            DataTable dt = kn.ExecuteQueryWithParams(sql, pr);

            string maTT = "";
            string modeTT = "add";

            if (dt.Rows.Count > 0)
            {
                maTT = dt.Rows[0]["MaThanhToan"].ToString();
                modeTT = "edit";
            }

            // 2️⃣ LẤY TỔNG TIỀN (ThanhTien)
            string tongTien = txtThanhTien.Text; // ✔️ dùng ThanhTien là chuẩn nhất

            // 3️⃣ Mở form Thanh Toán — ĐÚNG THỨ TỰ
            FrmCTThanhToan f = new FrmCTThanhToan(
                maTT,                                   // MaThanhToan
                maDH,                                   // MaDonHang
                cboMaKH.SelectedValue?.ToString(),      // ✔️ MaKhachHang
                cboMaNV.SelectedValue?.ToString(),      
                tongTien,                              
                modeTT                                  
            );

            f.ShowDialog();
        }

        private void btnDanhSach_Click(object sender, EventArgs e)
        {
            LoadDetail();   // reload dữ liệu từ DB vào form
            TinhTien();     // cập nhật lại tiền nếu cần
            MessageBox.Show("Đã load lại dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CheckAndStartTimerTheoTrangThai();
        }

        private void CheckAndStartTimerTheoTrangThai()
        {
            //if (cboTrangThaiDH.Text == "Đã lấy hàng"
            //&& !timerDangGiaoHang.Enabled
            //&& !timerGiaoHangThanhCong.Enabled)
            //{
            //    StartTimerDangGiaoHang();
            //}
            if (string.IsNullOrWhiteSpace(cboTrangThaiDH.Text)) return;

            // ❗ Chỉ đơn ONLINE mới có giao hàng
            if (cboLoaiDH.Text != "Trực tuyến") return;

            if (cboTrangThaiDH.Text == "Đã lấy hàng")
            {
                if (!timerDangGiaoHang.Enabled && !timerGiaoHangThanhCong.Enabled)
                {
                    StartTimerDangGiaoHang();
                }
            }

        }

        private void StartTimerDangGiaoHang()
        {
            timerDangGiaoHang.Stop();
            timerGiaoHangThanhCong.Stop();

            timerDangGiaoHang.Start();
        }

        private void timerDangGiaoHang_Tick(object sender, EventArgs e)
        {
            timerDangGiaoHang.Stop(); // chỉ chạy 1 lần

            if (string.IsNullOrWhiteSpace(txtMaDH.Text)) return;

            string maDH = txtMaDH.Text.Trim();

            DialogResult dr = MessageBox.Show(
                "Thông báo từ đơn vị vận chuyển:\nĐơn hàng đã rời kho và bắt đầu giao.\n\nXác nhận chuyển sang trạng thái 'Đang giao hàng'?",
                "Thông báo giao hàng",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information
            );

            if (dr == DialogResult.OK)
            {
                kn.ExecuteNonQuery($@"
            UPDATE tblDonHang 
            SET TrangThaiDonHang = N'Đang giao hàng'
            WHERE MaDonHang = '{maDH}'
        ");

                cboTrangThaiDH.Text = "Đang giao hàng";

                // Bắt đầu timer tiếp theo (10s)
                timerGiaoHangThanhCong.Start();
            }
        }

        private void timerGiaoHangThanhCong_Tick(object sender, EventArgs e)
        {
            timerGiaoHangThanhCong.Stop(); // chỉ chạy 1 lần

            if (string.IsNullOrWhiteSpace(txtMaDH.Text)) return;

            string maDH = txtMaDH.Text.Trim();

            DialogResult dr = MessageBox.Show(
                "Thông báo từ đơn vị vận chuyển:\nKhách hàng đã nhận hàng thành công!\n\nXác nhận hoàn tất đơn hàng?",
                "Thông báo giao hàng",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information
            );

            if (dr == DialogResult.OK)
            {
                kn.ExecuteNonQuery($@"
            UPDATE tblDonHang 
            SET TrangThaiDonHang = N'Giao hàng thành công'
            WHERE MaDonHang = '{maDH}'
        ");

                cboTrangThaiDH.Text = "Giao hàng thành công";
                CapNhatThanhToanTheoDonHang();
            }
        }

        private void CapNhatThanhToanTheoDonHang()
        {
            if (string.IsNullOrWhiteSpace(txtMaDH.Text)) return;

            string maDH = txtMaDH.Text.Trim();

            // Lấy loại đơn + phương thức thanh toán
            DataTable dt = kn.ExecuteQueryWithParams(@"
        SELECT dh.LoaiDonHang, tt.PhuongThucThanhToan
        FROM tblDonHang dh
        JOIN tblThanhToan tt ON dh.MaDonHang = tt.MaDonHang
        WHERE dh.MaDonHang = @MaDH",
                new SqlParameter[] { new SqlParameter("@MaDH", maDH) }
            );

            if (dt.Rows.Count == 0) return;

            string loaiDH = dt.Rows[0]["LoaiDonHang"].ToString();
            string pttt = dt.Rows[0]["PhuongThucThanhToan"].ToString();

            // 🔴 COD → chỉ update khi giao hàng xong
            if (loaiDH == "Trực tuyến" && pttt == "Tiền mặt khi giao hàng")
            {
                // hàm này CHỈ được gọi khi giao hàng thành công
                kn.ExecuteNonQueryWithParams(@"
            UPDATE tblThanhToan
            SET TrangThaiThanhToan = 1
            WHERE MaDonHang = @MaDH",
                    new SqlParameter[] { new SqlParameter("@MaDH", maDH) }
                );
            }

            // 🟢 Online payment → update ngay
            else
            {
                kn.ExecuteNonQueryWithParams(@"
            UPDATE tblThanhToan
            SET TrangThaiThanhToan = 1
            WHERE MaDonHang = @MaDH",
                    new SqlParameter[] { new SqlParameter("@MaDH", maDH) }
                );
            }
        }
    }
}
