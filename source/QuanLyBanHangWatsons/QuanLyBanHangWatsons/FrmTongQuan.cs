using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{
    public partial class FrmTongQuan : Form
    {
        Ket_noi kn = new Ket_noi();

        private Guna.Charts.WinForms.GunaDoughnutDataset doughnutDataset;
        private Guna.Charts.WinForms.GunaBarDataset chiNhanhDataset;
        private string currentTimeFrame = "Tháng"; // mặc định

        public FrmTongQuan()
        {
            InitializeComponent();
        }

        private void InitializeGunaDoughnutChart()
        {
            

            // 2. Tạo dataset kiểu Doughnut
            doughnutDataset = new Guna.Charts.WinForms.GunaDoughnutDataset();
            doughnutDataset.Label = "Đơn hàng theo loại";
            doughnutDataset.FillColors.Add(ColorTranslator.FromHtml("#93d150")); // Tại cửa hàng
            doughnutDataset.FillColors.Add(ColorTranslator.FromHtml("#d9d9d9"));  // Online


            gunaChartLoaiDon.Datasets.Add(doughnutDataset);
        }

        private void InitializeGunaBarChartChiNhanh()
        {
            // 1. Tạo dataset horizontal bar
            chiNhanhDataset = new Guna.Charts.WinForms.GunaBarDataset();
            chiNhanhDataset.Label = "Chi nhánh bán chạy";
            // 2. Thêm màu khác nhau cho từng chi nhánh (tối đa 6)
            chiNhanhDataset.FillColors.Add(ColorTranslator.FromHtml("#003f5c"));
            chiNhanhDataset.FillColors.Add(ColorTranslator.FromHtml("#444e86"));
            chiNhanhDataset.FillColors.Add(ColorTranslator.FromHtml("#955196"));
            chiNhanhDataset.FillColors.Add(ColorTranslator.FromHtml("#dd5182"));
            chiNhanhDataset.FillColors.Add(ColorTranslator.FromHtml("#ff6e54"));
            chiNhanhDataset.FillColors.Add(ColorTranslator.FromHtml("#ffa600"));


            // 2. Xóa dataset cũ nếu có
            gunaChartChiNhanh.Datasets.Clear();

            // 3. Thêm dataset mới
            gunaChartChiNhanh.Datasets.Add(chiNhanhDataset);
        }

        private void FrmTongQuan_Load(object sender, EventArgs e)
        {

            LoadSummaryCards();

            InitializeGunaDoughnutChart();   // Doughnut chart Loại đơn
            LoadCircleProgressLoaiDon();

            LoadTopSanPham();
            LoadSplineChartDoanhThu();
            //LoadDataGrid();

            InitializeGunaBarChartChiNhanh(); // Khởi tạo dataset bar chart
            LoadTopChiNhanh();

            cboTimeFrame.Items.AddRange(new string[] { "Ngày", "Tuần", "Tháng" });
            cboTimeFrame.SelectedItem = "Tháng"; // mặc định
            cboTimeFrame.SelectedIndexChanged += CboTimeFrame_SelectedIndexChanged;
        }

        private void CboTimeFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentTimeFrame = cboTimeFrame.SelectedItem.ToString();
            LoadSplineChartDoanhThu(); // gọi lại chart
        }


        // -------------------- Summary Cards --------------------
        private void LoadSummaryCards()
        {
            // Doanh thu
            object tongDoanhThu = kn.ExecuteScalar("select sum(tongtien) from tblThanhToan where TrangThaiThanhToan = 1");
            lblDT.Text = tongDoanhThu != null ? string.Format("{0:N0} VNĐ", tongDoanhThu) : "0 VNĐ";

            // Số đơn hàng
            object soDonHang = kn.ExecuteScalar("select count(*) from tblDonHang");
            lblSoDonHang.Text = soDonHang != null ? soDonHang.ToString() : "0";

            // Khách hàng
            object soKhachHang = kn.ExecuteScalar("select count(*) from tblKhachHang");
            lblSoKhachHang.Text = soKhachHang != null ? soKhachHang.ToString() : "0";

            // Đơn hoàn tất
            object donHoanTat = kn.ExecuteScalar("select count(*) from tblDonHang where TrangThaiDonHang = N'Giao hàng thành công'");
            lblDonHoanTat.Text = donHoanTat != null ? donHoanTat.ToString() : "0";
        }

        private void LoadCircleProgressLoaiDon()
        {
            // Lấy số lượng Offline và Online
            int soDonTaiCuaHang = Convert.ToInt32(kn.ExecuteScalar("select count(*) from tblDonHang where LoaiDonHang = N'Tại cửa hàng'"));
            int soDonTrucTuyen = Convert.ToInt32(kn.ExecuteScalar("select count(*) from tblDonHang where LoaiDonHang = N'Trực tuyến'"));

            doughnutDataset.DataPoints.Clear();
            doughnutDataset.DataPoints.Add(new Guna.Charts.WinForms.LPoint { Label = "Tại cửa hàng", Y = soDonTaiCuaHang });
            doughnutDataset.DataPoints.Add(new Guna.Charts.WinForms.LPoint { Label = "Trực tuyến", Y = soDonTrucTuyen });

            gunaChartLoaiDon.Update();
        }


        private void LoadTopSanPham()
        {
            DataTable dt = kn.ExecuteQuery(@"
            select top 5 sp.TenSanPham, sum(ct.SoLuong) as TongBan
            from tblChiTietDonHang ct
            join tblSanPham sp on ct.MaSanPham = sp.MaSanPham
            group by sp.TenSanPham
            order by TongBan desc");

            // Tạo dataset kiểu HorizontalBar
            var horizontalBarDataset = new Guna.Charts.WinForms.GunaHorizontalBarDataset();
            horizontalBarDataset.Label = "Top sản phẩm";

            // Thêm màu khác nhau cho từng sản phẩm (tối đa 5)
            horizontalBarDataset.FillColors.Add(ColorTranslator.FromHtml("#1e396e"));
            horizontalBarDataset.FillColors.Add(ColorTranslator.FromHtml("#01b1af"));
            horizontalBarDataset.FillColors.Add(ColorTranslator.FromHtml("#f04e63"));
            horizontalBarDataset.FillColors.Add(ColorTranslator.FromHtml("#fcb316"));
            horizontalBarDataset.FillColors.Add(ColorTranslator.FromHtml("#c2c923") );

            // Xóa dữ liệu cũ nếu có
            gunaChartSanPham.Datasets.Clear();

            // Thêm dữ liệu
            foreach (DataRow row in dt.Rows)
            {
                horizontalBarDataset.DataPoints.Add(row["TenSanPham"].ToString(),
                                                   Convert.ToDouble(row["TongBan"]));
            }

            // Thêm dataset vào chart
            gunaChartSanPham.Datasets.Add(horizontalBarDataset);

            // Cập nhật chart
            gunaChartSanPham.Update();
        }

        private void LoadSplineChartDoanhThu()
        {
            gunaChartDThuThang.Datasets.Clear();

            var splineDataset = new Guna.Charts.WinForms.GunaSplineDataset();
            splineDataset.PointRadius = 3;
            splineDataset.PointStyle = Guna.Charts.WinForms.PointStyle.Circle;
            splineDataset.Label = "Doanh thu";

            DataTable dt = new DataTable();

            if (currentTimeFrame == "Ngày")
            {
                dt = kn.ExecuteQuery(@"
                select cast(NgayThanhToan as date) as Ngay, sum(TongTien) as DoanhThu
                from tblThanhToan
                where TrangThaiThanhToan = 1
                group by cast(NgayThanhToan as date)
                order by Ngay");
                foreach (DataRow row in dt.Rows)
                {
                    splineDataset.DataPoints.Add(
                        Convert.ToDateTime(row["Ngay"]).ToString("dd/MM"),
                        Convert.ToDouble(row["DoanhThu"])
                    );
                }
            }
            else if (currentTimeFrame == "Tuần")
            {
                dt = kn.ExecuteQuery(@"
                select datepart(week, NgayThanhToan) as Tuan, sum(TongTien) as DoanhThu
                from tblThanhToan
                where TrangThaiThanhToan = 1
                group by datepart(week, NgayThanhToan)
                order by Tuan");
                foreach (DataRow row in dt.Rows)
                {
                    splineDataset.DataPoints.Add("Tuần " + row["Tuan"].ToString(),
                                                Convert.ToDouble(row["DoanhThu"]));
                }
            }
            else if (currentTimeFrame == "Tháng")
            {
                // luôn tạo label Tháng 1 -> Tháng 12
                for (int m = 1; m <= 12; m++)
                {
                    object val = kn.ExecuteScalar($@"
                select sum(TongTien) from tblThanhToan
                where TrangThaiThanhToan = 1 and month(NgayThanhToan) = {m}");
                    double y = val != DBNull.Value && val != null ? Convert.ToDouble(val) : 0;
                    splineDataset.DataPoints.Add("Tháng " + m, y);
                }
            }

            gunaChartDThuThang.Datasets.Add(splineDataset);
            gunaChartDThuThang.Update();
        }

        private void LoadTopChiNhanh()
        {
            DataTable dt = kn.ExecuteQuery(@"
            select top 6 cn.TenChiNhanh, count(dh.MaDonHang) as SoDonHang
            from tblDonHang dh
            join tblChiNhanh cn on dh.MaChiNhanh = cn.MaChiNhanh
            where dh.TrangThaiDonHang = N'Giao hàng thành công'
            group by cn.TenChiNhanh
            order by SoDonHang desc");

            // Xóa dữ liệu cũ
            chiNhanhDataset.DataPoints.Clear();

            // Thêm dữ liệu mới
            foreach (DataRow row in dt.Rows)
            {
                chiNhanhDataset.DataPoints.Add(new Guna.Charts.WinForms.LPoint
                {
                    Label = row["TenChiNhanh"].ToString(),
                    Y = Convert.ToDouble(row["SoDonHang"])
                });
            }

            // Cập nhật chart
            gunaChartChiNhanh.Update();
        }

        public Action OpenFrmBaoCao;
        private void btnXem_Click(object sender, EventArgs e)
        {
            OpenFrmBaoCao?.Invoke();
        }
    }
}
