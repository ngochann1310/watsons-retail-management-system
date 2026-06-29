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
    public partial class FrmBaoCao : Form
    {
        Ket_noi kn = new Ket_noi();

        public FrmBaoCao()
        {
            InitializeComponent();
        }

        private void FrmBaoCao_Load(object sender, EventArgs e)
        {
            // ComboBox loại báo cáo
            cboLoaiBC.Items.Add("Doanh Thu");
            cboLoaiBC.Items.Add("Nhập Kho");
            cboLoaiBC.Items.Add("Xuất Kho");
            cboLoaiBC.SelectedIndex = 0; // chọn mặc định "Doanh Thu"

            // ComboBox tháng
            for (int i = 1; i <= 12; i++)
                cboThang.Items.Add(i.ToString());
            cboThang.SelectedIndex = DateTime.Now.Month - 1;

            // ComboBox năm (ví dụ 2023 - 2030)
            for (int i = 2023; i <= 2030; i++)
                cboNam.Items.Add(i.ToString());
            cboNam.SelectedItem = DateTime.Now.Year.ToString();

            this.reportViewerBC.RefreshReport();
            this.reportViewerBC.RefreshReport();
        }        

        private void LoadReportDoanhThu(int nam, int thang)
        {
            reportViewerBC.LocalReport.ReportEmbeddedResource =
                "QuanLyBanHangWatsons.Report_BaoCaoDoanhThu.rdlc";

            string query = @"
            SELECT 
                dh.MaDonHang,
                cth.MaSanPham,
                cn.TenChiNhanh,
                kh.HoKhachHang,
                kh.TenKhachHang,
                nv.HoNhanVien,
                nv.TenNhanVien,
                dh.NgayDatHang,
                dh.ThanhTien AS TongTien
            FROM tblDonHang dh
            JOIN tblChiTietDonHang cth ON dh.MaDonHang = cth.MaDonHang
            JOIN tblKhachHang kh ON dh.MaKhachHang = kh.MaKhachHang
            JOIN tblNhanVien nv ON dh.MaNhanVien = nv.MaNhanVien
            JOIN tblChiNhanh cn ON dh.MaChiNhanh = cn.MaChiNhanh
            WHERE YEAR(dh.NgayDatHang) = @Nam 
              AND MONTH(dh.NgayDatHang) = @Thang
            ";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Nam", nam),
                new SqlParameter("@Thang", thang)
            };

            DataTable dt = kn.ExecuteQueryWithParams(query, parameters);

            List<DoanhThuReport> list = dt.AsEnumerable().Select(r => new DoanhThuReport
            {
                MaDonHang = r["MaDonHang"].ToString(),
                MaSanPham = r["MaSanPham"].ToString(),
                TenChiNhanh = r["TenChiNhanh"].ToString(),
                KhachHang = r["HoKhachHang"] + " " + r["TenKhachHang"],
                NhanVien = r["HoNhanVien"] + " " + r["TenNhanVien"],
                NgayDatHang = Convert.ToDateTime(r["NgayDatHang"]),
                TongTien = Convert.ToDecimal(r["TongTien"])
            }).ToList();

            reportViewerBC.LocalReport.DataSources.Clear();
            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetDoanhThu", list)
            );

            decimal tongDoanhThu = list.Sum(x => x.TongTien);
            reportViewerBC.LocalReport.SetParameters(
                new ReportParameter("TongDoanhThu", tongDoanhThu.ToString("N0"))
            );

            reportViewerBC.RefreshReport();
        }

        private void LoadReportNhapKho(int nam, int thang)
        {
            reportViewerBC.LocalReport.ReportEmbeddedResource =
                "QuanLyBanHangWatsons.Report_BaoCaoPN.rdlc";

            string query = @"
        select
            pn.maphieunhap,
            pn.ngaynhap,
            cn.tenchinhanh,
            ncc.tennhacungcap,
            nv.honhanvien,
            nv.tennhanvien,
            sum(ct.soluong) as soluong,
            sum(ct.soluong * ct.gianhap) as thanhtien
        from tblphieunhap pn
        join tblctphieunhap ct on pn.maphieunhap = ct.maphieunhap
        join tblchinhanh cn on pn.machinhanh = cn.machinhanh
        join tblnhacungcap ncc on pn.manhacungcap = ncc.manhacungcap
        join tblnhanvien nv on pn.manhanvien = nv.manhanvien
        where year(pn.ngaynhap) = @Nam
          and month(pn.ngaynhap) = @Thang
        group by
            pn.maphieunhap,
            pn.ngaynhap,
            cn.tenchinhanh,
            ncc.tennhacungcap,
            nv.honhanvien,
            nv.tennhanvien
    ";

            SqlParameter[] parameters =
            {
        new SqlParameter("@Nam", nam),
        new SqlParameter("@Thang", thang)
    };

            DataTable dt = kn.ExecuteQueryWithParams(query, parameters);

            List<PhieuNhapReport> list = dt.AsEnumerable().Select(r => new PhieuNhapReport
            {
                MaPhieuNhap = r["MaPhieuNhap"].ToString(),
                NgayNhap = Convert.ToDateTime(r["NgayNhap"]),
                ChiNhanh = r["TenChiNhanh"].ToString(),
                NhaCungCap = r["TenNhaCungCap"].ToString(),
                NhanVien = r["HoNhanVien"] + " " + r["TenNhanVien"],
                SoLuong = Convert.ToInt32(r["SoLuong"]),
                ThanhTien = Convert.ToDecimal(r["ThanhTien"])
            }).ToList();

            reportViewerBC.LocalReport.DataSources.Clear();
            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetNhapKho", list)
            );

            decimal tongTienNhap = list.Sum(x => x.ThanhTien);

            reportViewerBC.LocalReport.SetParameters(
                new ReportParameter("TongTienNhapHang", tongTienNhap.ToString("N0"))
            );

            reportViewerBC.RefreshReport();
        }

        private void LoadReportXuatKho(int nam, int thang)
        {
            reportViewerBC.LocalReport.ReportEmbeddedResource =
                "QuanLyBanHangWatsons.Report_BaoCaoPX.rdlc";

            string query = @"
            SELECT
                px.MaPhieuXuat,
                px.NgayXuat,
                cn.TenChiNhanh,
                px.MaDonHang,
                nv.HoNhanVien,
                nv.TenNhanVien,
                px.TongTien AS ThanhTien
            FROM tblPhieuXuat px
            JOIN tblNhanVien nv ON px.MaNhanVien = nv.MaNhanVien
            JOIN tblChiNhanh cn ON px.MaChiNhanh = cn.MaChiNhanh
            WHERE YEAR(px.NgayXuat) = @Nam
              AND MONTH(px.NgayXuat) = @Thang
            ORDER BY px.NgayXuat
            ";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Nam", nam),
                new SqlParameter("@Thang", thang)
            };

            DataTable dt = kn.ExecuteQueryWithParams(query, parameters);

            List<PhieuXuatReport> list = dt.AsEnumerable().Select(r => new PhieuXuatReport
            {
                MaPhieuXuat = r["MaPhieuXuat"].ToString(),
                NgayXuat = Convert.ToDateTime(r["NgayXuat"]),
                TenChiNhanh = r["TenChiNhanh"].ToString(),
                MaDonHang = r["MaDonHang"].ToString(),
                NhanVien = r["HoNhanVien"] + " " + r["TenNhanVien"],
                ThanhTien = Convert.ToDecimal(r["ThanhTien"])
            }).ToList();

            reportViewerBC.LocalReport.DataSources.Clear();
            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetPhieuXuat", list)
            );

            decimal tongTienXuat = list.Sum(x => x.ThanhTien);

            reportViewerBC.LocalReport.SetParameters(
                new ReportParameter("TongTienXuatKho", tongTienXuat.ToString("N0"))
            );

            reportViewerBC.RefreshReport();
        }



        private void btnXem_Click(object sender, EventArgs e)
        {
            if (cboLoaiBC.SelectedItem == null || cboNam.SelectedItem == null || cboThang.SelectedItem == null)
            {
                MessageBox.Show("Chọn loại báo cáo, tháng và năm trước khi xem.");
                return;
            }

            string loaiBC = cboLoaiBC.SelectedItem.ToString();
            int thang = int.Parse(cboThang.SelectedItem.ToString());
            int nam = int.Parse(cboNam.SelectedItem.ToString());

            reportViewerBC.LocalReport.DataSources.Clear();

            if (loaiBC == "Doanh Thu")
            {       
                LoadReportDoanhThu(nam, thang);
            }
            else if (loaiBC == "Nhập Kho")
            {
                LoadReportNhapKho(nam, thang);
            }
            else if (loaiBC == "Xuất Kho")
            {
                LoadReportXuatKho(nam, thang);
            }

            reportViewerBC.RefreshReport();
        }
    }
}
