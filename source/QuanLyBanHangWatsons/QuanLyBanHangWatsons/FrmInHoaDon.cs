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
    public partial class FrmInHoaDon : Form
    {
        Ket_noi kn = new Ket_noi();
        string maHD;

        public FrmInHoaDon(string maHD)
        {
            InitializeComponent();
            this.maHD = maHD;
        }


        private void FrmInHoaDon_Load(object sender, EventArgs e)
        {
            LoadComboMaHoaDon();

            // Nếu có mã hóa đơn truyền vào, chọn luôn trong combo và load report
            if (!string.IsNullOrWhiteSpace(maHD))
            {
                cboMaHD.SelectedValue = maHD;
                LoadReportHoaDon(maHD);
            }

            reportViewerBC.RefreshReport();
        }

        private void LoadReportHoaDon(string maHoaDon)
        {
            reportViewerBC.LocalReport.ReportEmbeddedResource =
                "QuanLyBanHangWatsons.Report_HoaDon.rdlc";

            reportViewerBC.LocalReport.DataSources.Clear();

            // =========================
            // 1. LOAD THÔNG TIN HÓA ĐƠN
            // =========================
            string sqlThongTin = @"
            select *                 
            from view_HoaDon_ThongTin
            where MaHoaDon = @MaHoaDon
             ";

            //SqlParameter[] pr1 =
            //{
            //    new SqlParameter("@MaHoaDon", maHoaDon)
            //};

            DataTable dtThongTin = kn.ExecuteQueryWithParams(sqlThongTin, new SqlParameter[]
            {
                new SqlParameter("@MaHoaDon", maHoaDon)
            });

            if (dtThongTin.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy hóa đơn!");
                return;
            }

            var thongTin = dtThongTin.AsEnumerable().Select(r => new HoaDonThongTinReport
            {
                MaHoaDon = r["MaHoaDon"].ToString(),
                NgayXuatHoaDon = Convert.ToDateTime(r["NgayXuatHoaDon"]),

                TenChiNhanh = r["TenChiNhanh"].ToString(),
                DiaChiChiNhanh = r["DiaChiChiNhanh"].ToString(),

                HoKhachHang = r["HoKhachHang"].ToString(),
                TenKhachHang = r["TenKhachHang"].ToString(),
                DiaChiKhachHang = r["DiaChiKhachHang"].ToString(),
                DienThoaiKhachHang = r["DienThoaiKhachHang"].ToString(),

                HoNhanVien = r["HoNhanVien"].ToString(),
                TenNhanVien = r["TenNhanVien"].ToString(),

                PhuongThucThanhToan = r["PhuongThucThanhToan"].ToString(),
                LoaiDonHang = r["LoaiDonHang"].ToString(),
            }).ToList();

            // =========================
            // 2. LOAD CHI TIẾT HÓA ĐƠN
            // =========================
            string sqlChiTiet = @"
                select TenSanPham, DonViTinh, SoLuong, DonGia, TongTien, ThanhTien, SoTienDaThanhToan, SoDuConLai,
                        dbo.f_sothanhchu(SoTienDaThanhToan) as SoTienBangChu
                from view_HoaDon_ChiTiet
                where MaHoaDon = @MaHoaDon
            ";

            DataTable dtCT = kn.ExecuteQueryWithParams(sqlChiTiet, new SqlParameter[]
            {
                new SqlParameter("@MaHoaDon", maHoaDon)
            });

            var chiTiet = dtCT.AsEnumerable().Select(r => new HoaDonChiTietReport
            {
                TenSanPham = r["TenSanPham"].ToString(),
                DonViTinh = r["DonViTinh"].ToString(),
                SoLuong = Convert.ToInt32(r["SoLuong"]),
                DonGia = Convert.ToDecimal(r["DonGia"]),

                TongTien = Convert.ToDecimal(r["TongTien"]),
                ThanhTien = Convert.ToDecimal(r["ThanhTien"]),
                SoTienDaThanhToan = Convert.ToDecimal(r["SoTienDaThanhToan"]),
                SoDuConLai = Convert.ToDecimal(r["SoDuConLai"]),
                SoTienBangChu = r["SoTienBangChu"].ToString()

            }).ToList();

            // =========================
            // 3. ADD DATASET VÀO RDLC
            // =========================
            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetHoaDonThongTin", thongTin)
            );

            reportViewerBC.LocalReport.DataSources.Add(
                new ReportDataSource("DataSetHoaDonChiTiet", chiTiet)
            );

            reportViewerBC.RefreshReport();
        }


        private void LoadComboMaHoaDon()
        {
            string sql = @"
            SELECT MaHoaDon
            FROM tblHoaDon
            ORDER BY MaHoaDon DESC
            ";

            DataTable dt = kn.ExecuteQuery(sql);

            cboMaHD.DataSource = dt;
            cboMaHD.DisplayMember = "MaHoaDon";
            cboMaHD.ValueMember = "MaHoaDon";
            cboMaHD.SelectedIndex = -1; // chưa chọn gì
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            if (cboMaHD.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn mã hóa đơn!");
                return;
            }

            string maHoaDon = cboMaHD.SelectedValue.ToString();

            LoadReportHoaDon(maHoaDon);
        }
    }
}
