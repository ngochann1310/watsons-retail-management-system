using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanHangWatsons
{
    public class HoaDonChiTietReport
    {
        public string TenSanPham { get; set; }
        public string DonViTinh { get; set; }

        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        // tiền từng dòng (SoLuong * DonGia)
        public decimal TongTien { get; set; }

        // các giá trị tổng
        public decimal ThanhTien { get; set; }
        public decimal SoTienDaThanhToan { get; set; }
        public decimal SoDuConLai { get; set; }

        public string SoTienBangChu { get; set; }
    }

}
