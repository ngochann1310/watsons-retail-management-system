using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanHangWatsons
{
    public class DoanhThuReport
    {
        public string MaDonHang { get; set; }
        public string MaSanPham { get; set; }
        public string TenChiNhanh { get; set; }
        public string KhachHang { get; set; }    // Ho + Ten
        public string NhanVien { get; set; }    // Ho + Ten
        public DateTime NgayDatHang { get; set; }
        public decimal TongTien { get; set; }
    }


}
