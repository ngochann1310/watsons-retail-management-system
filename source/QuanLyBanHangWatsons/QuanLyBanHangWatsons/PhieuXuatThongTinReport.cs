using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanHangWatsons
{
    public class PhieuXuatThongTinReport
    {
        public string MaPhieuXuat { get; set; }
        public DateTime NgayXuat { get; set; }

        // Khách hàng
        public string HoKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DiaChiKhachHang { get; set; }

        public string TenChiNhanh { get; set; }
        public string DiaChiChiNhanh { get; set; }

        public decimal TongTien { get; set; }       // tổng tiền phiếu xuất
        public string SoTienBangChu { get; set; }   // tổng tiền viết bằng chữ
    }
}
