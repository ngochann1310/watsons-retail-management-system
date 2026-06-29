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
    public partial class FrmInPhieuNhap : Form
    {
        Ket_noi kn = new Ket_noi();
        string maPN;

        public FrmInPhieuNhap(string maPN)
        {
            InitializeComponent();
            this.maPN = maPN;
        }

        private void FrmInPhieuNhap_Load(object sender, EventArgs e)
        {
            LoadComboMaPhieuNhap();
            // nếu có mã phiếu xuất truyền vào thì chọn luôn trong combo
            if (!string.IsNullOrWhiteSpace(maPN))
            {
                cboMaPN.SelectedValue = maPN;
                LoadReportPhieuNhap(maPN);
            }

            reportViewerBC.RefreshReport();
            
        }

        private void LoadComboMaPhieuNhap()
        {
            string sql = @"SELECT MaPhieuNhap FROM tblPhieuNhap ORDER BY NgayNhap DESC";
            DataTable dt = kn.ExecuteQuery(sql);
            cboMaPN.DataSource = dt;
            cboMaPN.DisplayMember = "MaPhieuNhap";
            cboMaPN.ValueMember = "MaPhieuNhap";
            cboMaPN.SelectedIndex = -1; // chưa chọn gì
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (cboMaPN.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn mã phiếu nhập!");
                return;
            }

            string maPhieuNhap = cboMaPN.SelectedValue.ToString();
            LoadReportPhieuNhap(maPhieuNhap);
        }

        private void LoadReportPhieuNhap(string maPhieuNhap)
        {
            reportViewerBC.LocalReport.ReportEmbeddedResource =
                "QuanLyBanHangWatsons.Report_PhieuNhap.rdlc";

            reportViewerBC.LocalReport.DataSources.Clear();

            // =========================
            // 1. LOAD THÔNG TIN PHIẾU NHẬP
            // =========================
            string sqlThongTin = @"
                SELECT pn.MaPhieuNhap,
                       pn.NgayNhap,
                       nv.HoNhanVien + ' ' + nv.TenNhanVien AS NhanVien,
                       cn.TenChiNhanh,
                       cn.DiaChi AS DiaChiChiNhanh,
                       pn.TongTien,
                       dbo.f_sothanhchu(pn.TongTien) AS SoTienBangChu
                FROM tblPhieuNhap pn
                INNER JOIN tblNhanVien nv ON pn.MaNhanVien = nv.MaNhanVien
                INNER JOIN tblChiNhanh cn ON pn.MaChiNhanh = cn.MaChiNhanh
                WHERE pn.MaPhieuNhap = @MaPhieuNhap
            ";

            DataTable dtThongTin = kn.ExecuteQueryWithParams(sqlThongTin, new SqlParameter[]
            {
                new SqlParameter("@MaPhieuNhap", maPhieuNhap)
            });

            if (dtThongTin.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy phiếu nhập!");
                return;
            }

            var thongTin = dtThongTin.AsEnumerable().Select(r => new PhieuNhapThongTinReport
            {
                MaPhieuNhap = r["MaPhieuNhap"].ToString(),
                NgayNhap = Convert.ToDateTime(r["NgayNhap"]),
                NhanVien = r["NhanVien"].ToString(),
                TenChiNhanh = r["TenChiNhanh"].ToString(),
                DiaChiChiNhanh = r["DiaChiChiNhanh"].ToString(),
                TongTien = Convert.ToDecimal(r["TongTien"]),
                SoTienBangChu = r["SoTienBangChu"].ToString()
            }).ToList();

            // =========================
            // 2. LOAD CHI TIẾT PHIẾU NHẬP
            // =========================
            string sqlChiTiet = @"
                SELECT ct.MaSanPham, sp.TenSanPham, sp.DonViTinh,
                       ct.GiaNhap, ct.SoLuong, (ct.GiaNhap * ct.SoLuong) AS ThanhTien
                FROM tblCTPhieuNhap ct
                INNER JOIN tblSanPham sp ON ct.MaSanPham = sp.MaSanPham
                WHERE ct.MaPhieuNhap = @MaPhieuNhap
            ";

            DataTable dtCT = kn.ExecuteQueryWithParams(sqlChiTiet, new SqlParameter[]
            {
                new SqlParameter("@MaPhieuNhap", maPhieuNhap)
            });

            var chiTiet = dtCT.AsEnumerable().Select(r => new PhieuNhapChiTietReport
            {
                MaSanPham = r["MaSanPham"].ToString(),
                TenSanPham = r["TenSanPham"].ToString(),
                DonViTinh = r["DonViTinh"].ToString(),
                GiaNhap = Convert.ToDecimal(r["GiaNhap"]),
                SoLuong = Convert.ToInt32(r["SoLuong"]),
                ThanhTien = Convert.ToDecimal(r["ThanhTien"])
            }).ToList();

            // =========================
            // 3. ADD DATASET VÀO RDLC
            // =========================
            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetPhieuNhapThongTin", thongTin)
            );

            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetPhieuNhapChiTiet", chiTiet)
            );

            reportViewerBC.RefreshReport();
        }
    }
}
