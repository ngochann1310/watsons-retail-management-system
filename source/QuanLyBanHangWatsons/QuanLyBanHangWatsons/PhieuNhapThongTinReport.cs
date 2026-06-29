using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanHangWatsons
{
    public class PhieuNhapThongTinReport
    {
        public string MaPhieuNhap { get; set; }
        public DateTime NgayNhap { get; set; }

        public string NhanVien { get; set; }

        public string TenChiNhanh { get; set; }
        public string DiaChiChiNhanh { get; set; }

        public decimal TongTien { get; set; }       // tổng tiền của phiếu nhập
        public string SoTienBangChu { get; set; }   // tổng tiền viết bằng chữ       
    }
}
