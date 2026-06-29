using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanHangWatsons
{
    public class HoaDonThongTinReport
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayXuatHoaDon { get; set; }

        public string TenChiNhanh { get; set; }
        public string DiaChiChiNhanh { get; set; }

        public string HoKhachHang { get; set; }       // thêm
        public string TenKhachHang { get; set; }      // thêm
        public string DiaChiKhachHang { get; set; }
        public string DienThoaiKhachHang { get; set; }

        public string HoNhanVien { get; set; }        // thêm
        public string TenNhanVien { get; set; }       // thêm

        public string PhuongThucThanhToan { get; set; }
        public string LoaiDonHang { get; set; }

        // Có thể thêm property tính toán cho RDLC nếu muốn
        public string KhachHangKyTen => TenKhachHang;
        public string NhanVienKyTen => TenNhanVien;

    }

}
