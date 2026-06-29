using Microsoft.Reporting.WinForms;
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

namespace QuanLyBanHangWatsons
{
    public partial class FrmInPhieuXuat : Form
    {
        Ket_noi kn = new Ket_noi();
        string maPX;

        public FrmInPhieuXuat(string maPX)
        {
            InitializeComponent();
            this.maPX = maPX;
        }

        private void FrmInPhieuXuat_Load(object sender, EventArgs e)
        {
            LoadComboMaPhieuXuat();
            // nếu có mã phiếu xuất truyền vào thì chọn luôn trong combo
            if (!string.IsNullOrWhiteSpace(maPX))
            {
                cboMaPX.SelectedValue = maPX;
                LoadReportPhieuXuat(maPX);
            }
            reportViewerBC.RefreshReport();
        }

        private void LoadComboMaPhieuXuat()
        {
            string sql = @"SELECT MaPhieuXuat FROM tblPhieuXuat ORDER BY NgayXuat DESC";
            DataTable dt = kn.ExecuteQuery(sql);
            cboMaPX.DataSource = dt;
            cboMaPX.DisplayMember = "MaPhieuXuat";
            cboMaPX.ValueMember = "MaPhieuXuat";
            cboMaPX.SelectedIndex = -1;
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (cboMaPX.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn mã phiếu xuất!");
                return;
            }

            string maPhieuXuat = cboMaPX.SelectedValue.ToString();
            LoadReportPhieuXuat(maPhieuXuat);
        }

        private void LoadReportPhieuXuat(string maPhieuXuat)
        {
            reportViewerBC.LocalReport.ReportEmbeddedResource =
                "QuanLyBanHangWatsons.Report_PhieuXuat.rdlc";

            reportViewerBC.LocalReport.DataSources.Clear();

            // =========================
            // 1. LOAD THÔNG TIN PHIẾU XUẤT
            // =========================
            string sqlThongTin = @"
                SELECT px.MaPhieuXuat,
                       px.NgayXuat,
                       kh.HoKhachHang,
                       kh.TenKhachHang,
                       nv.HoNhanVien + ' ' + nv.TenNhanVien AS NhanVien,
                       cn.TenChiNhanh,
                       cn.DiaChi AS DiaChiChiNhanh,
                       px.TongTien,
                       dbo.f_sothanhchu(px.TongTien) AS SoTienBangChu
                FROM tblPhieuXuat px
                INNER JOIN tblDonHang dh ON px.MaDonHang = dh.MaDonHang
                INNER JOIN tblKhachHang kh ON dh.MaKhachHang = kh.MaKhachHang
                INNER JOIN tblNhanVien nv ON px.MaNhanVien = nv.MaNhanVien
                INNER JOIN tblChiNhanh cn ON px.MaChiNhanh = cn.MaChiNhanh
                WHERE px.MaPhieuXuat = @MaPhieuXuat
            ";

            DataTable dtThongTin = kn.ExecuteQueryWithParams(sqlThongTin, new SqlParameter[]
            {
                new SqlParameter("@MaPhieuXuat", maPhieuXuat)
            });

            if (dtThongTin.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy phiếu xuất!");
                return;
            }

            var thongTin = dtThongTin.AsEnumerable().Select(r => new PhieuXuatThongTinReport
            {
                MaPhieuXuat = r["MaPhieuXuat"].ToString(),
                NgayXuat = Convert.ToDateTime(r["NgayXuat"]),
                HoKhachHang = r["HoKhachHang"].ToString(),
                TenKhachHang = r["TenKhachHang"].ToString(),
                TenChiNhanh = r["TenChiNhanh"].ToString(),
                DiaChiChiNhanh = r["DiaChiChiNhanh"].ToString(),
                TongTien = Convert.ToDecimal(r["TongTien"]),
                SoTienBangChu = r["SoTienBangChu"].ToString()
            }).ToList();

            // =========================
            // 2. LOAD CHI TIẾT PHIẾU XUẤT
            // =========================
            string sqlChiTiet = @"
                SELECT ct.MaSanPham, sp.TenSanPham, sp.DonViTinh,
                       ct.DonGia, ct.SoLuong, (ct.DonGia * ct.SoLuong) AS ThanhTien
                FROM tblCTPhieuXuat ct
                INNER JOIN tblSanPham sp ON ct.MaSanPham = sp.MaSanPham
                WHERE ct.MaPhieuXuat = @MaPhieuXuat
            ";

            DataTable dtCT = kn.ExecuteQueryWithParams(sqlChiTiet, new SqlParameter[]
            {
                new SqlParameter("@MaPhieuXuat", maPhieuXuat)
            });

            var chiTiet = dtCT.AsEnumerable().Select(r => new PhieuXuatChiTietReport
            {
                MaSanPham = r["MaSanPham"].ToString(),
                TenSanPham = r["TenSanPham"].ToString(),
                DonViTinh = r["DonViTinh"].ToString(),
                DonGia = Convert.ToDecimal(r["DonGia"]),
                SoLuong = Convert.ToInt32(r["SoLuong"]),
                ThanhTien = Convert.ToDecimal(r["ThanhTien"])
            }).ToList();

            // =========================
            // 3. ADD DATASET VÀO RDLC
            // =========================
            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetPhieuXuatThongTin", thongTin)
            );

            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetPhieuXuatChiTiet", chiTiet)
            );

            reportViewerBC.RefreshReport();
        }
    }
}
