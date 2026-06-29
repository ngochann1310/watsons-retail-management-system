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
    public partial class FrmMainFull : Form
    {
        private string _maNV;
        private string _hoTen;
        private string _quyen;

        FrmTongQuan tq;
        FormTongQuanNV tqnv;
        FrmBaoCao bc;

        FrmDonHang dh;
        FrmCTDonHang ctdh;

        FrmSanPham sp;
        FrmLoaiSP lsp;
        FrmThuongHieu th;
        FrmChiNhanh cn;


        FrmThanhToan tt;
        FrmHoaDon hd;

        FrmKhachHang kh;
        FrmTKKhachHang tkkh;

        FrmKhuyenMai km;
        FrmNhaCungCap ncc;

        FrmPhieuNhap pn;
        FrmCTPhieuNhap ctpn;

        FrmPhieuXuat px;
        FrmCTPhieuXuat ctpx;

        FrmTonKho tonkho;

        FrmTinNhan tn;

        FrmNhanVien nv;
        FrmTKNhanVien tknv;

        FrmCaiDat cd;
        FrmBackUp bak;
        FrmRestore rs;

        public FrmMainFull(string maNV, string hoTen, string quyen)
        {
            InitializeComponent();
            _maNV = maNV;
            _hoTen = hoTen;
            _quyen = quyen;
        }
        bool menuBHExpand = false;

        private void OpenChildForm(Form childForm)
        {
            // Đóng tất cả các form con đang mở
            foreach (Form form in this.MdiChildren)
            {
                form.Close();
            }

            // Mở form mới
            childForm.MdiParent = this;
            childForm.Dock = DockStyle.Fill;
            childForm.Show();
            childForm.BringToFront();
        }


        private void menuTransition_Tick(object sender, EventArgs e)
        {
            if (menuBHExpand == false)
            {
                pnlMenuBH.Height += 10;
                if (pnlMenuBH.Height >= 596)
                {
                    menuTransition.Stop();
                    menuBHExpand = true;
                }
            }
            else
            {
                pnlMenuBH.Height -= 10;
                if (pnlMenuBH.Height <= 80)
                {
                    menuTransition.Stop();
                    menuBHExpand = false;
                }
            }
        }

        private void btnBH_Click(object sender, EventArgs e)
        {
            menuTransition.Start();
        }

        bool menuMHExpand = false;


        private void menuMHTransition_Tick(object sender, EventArgs e)
        {
            if (menuMHExpand == false)
            {
                pnlMenuMH.Height += 10;
                if (pnlMenuMH.Height >= 434)
                {
                    menuMHTransition.Stop();
                    menuMHExpand = true;
                }
            }
            else
            {
                pnlMenuMH.Height -= 10;
                if (pnlMenuMH.Height <= 84)
                {
                    menuMHTransition.Stop();
                    menuMHExpand = false;
                }
            }
        }

        private void btnMH_Click(object sender, EventArgs e)
        {
            menuMHTransition.Start();
        }

        bool sidebarExpand = true;

        private void sidebarTransition_Tick(object sender, EventArgs e)
        {
            if (sidebarExpand == true)
            {
                pnlLeftMain.Width -= 10;
                if (pnlLeftMain.Width <= 100)
                {
                    sidebarExpand = false;
                    sidebarTransition.Stop();

                    pnlTQ.Width = pnlLeftMain.Width;
                    pnlTN.Width = pnlLeftMain.Width;
                    pnlNV.Width = pnlLeftMain.Width;
                    pnlSetting.Width = pnlLeftMain.Width;


                }
            }
            else
            {
                pnlLeftMain.Width += 10;
                if (pnlLeftMain.Width >= 346)
                {
                    sidebarExpand = true;
                    sidebarTransition.Stop();

                    pnlTQ.Width = pnlLeftMain.Width;
                    pnlTN.Width = pnlLeftMain.Width;
                    pnlNV.Width = pnlLeftMain.Width;
                    pnlSetting.Width = pnlLeftMain.Width;
                }
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            sidebarTransition.Start();
        }

        private void btnDH_Click(object sender, EventArgs e)
        {
            //if (dh == null)
            //{
                dh = new FrmDonHang();
                dh.FormClosed += FrmDonHang_FormClosed;
            OpenChildForm(dh);

            //    dh.OpenFrmCTDH = OpenFrmCTDH;

            //    dh.MdiParent = this;
            //    dh.Dock = DockStyle.Fill;
            //    dh.Show();
            //}
            //else
            //{
            //    dh.Activate();
            //}
        }

        private void FrmDonHang_FormClosed(object sender, EventArgs e)
        {
            dh = null;
        }

        private void FrmMainFull_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true;

            lblTenND.Text = _hoTen;
            lblPQuyen.Text = _quyen;

            ApplyPermission();
            OpenFormTongQuanTheoQuyen();
        }

        private void OpenFormTongQuanTheoQuyen()
        {
            if (_quyen == "Nhân viên bán hàng" || _quyen == "Nhân viên kho")
            {
                //// Form dành cho NV
                //if (tqnv == null || tqnv.IsDisposed)
                //{
                    tqnv = new FormTongQuanNV(); // Lưu ý: tạo FormTongQuanNV mới
                    tqnv.FormClosed += FormTongQuanNV_FormClosed;
                OpenChildForm(tqnv);
                //tqnv.MdiParent = this;
                //tqnv.Dock = DockStyle.Fill;
                //tqnv.Show();
                //tqnv.BringToFront();
                //}
                //else
                //{
                //    tqnv.Activate();
                //}
            }
            else
            {
                // Form dành cho quản lý/admin
                OpenFrmTongQuan();
            }
        }

        private void OpenFrmTongQuan()
        {
            if (tq == null || tq.IsDisposed)
            {
                tq = new FrmTongQuan();
                tq.FormClosed += FrmTongQuan_FormClosed;
                tq.OpenFrmBaoCao = OpenFrmBaoCao; // thêm dòng này
                tq.MdiParent = this;
                tq.Dock = DockStyle.Fill;
                tq.Show();
                tq.BringToFront();
            }
            else
            {
                tq.Activate();
            }
        }


        private void ApplyPermission()
        {
            switch (_quyen)
            {
                case "Nhân viên bán hàng":
                    pnlMenuMH.Visible = false;
                    pnlNV.Visible = false;
                    pnlTN.Visible = false;
                    pnlSL.Visible = false;
                    break;

                case "Nhân viên kho":
                    pnlMenuBH.Visible = false;
                    pnlNV.Visible = false;
                    pnlTN.Visible = false;
                    pnlSL.Visible = false;
                    break;

                case "Quản lý cửa hàng":
                    pnlMenuMH.Visible = false;
                    pnlTN.Visible = false;
                    pnlSL.Visible = false;
                    // ẩn gì thêm thì thêm
                    break;

                case "Quản lý kho":
                    pnlMenuBH.Visible = false;
                    pnlNV.Visible = false;
                    pnlTN.Visible = false;
                    pnlSL.Visible = false;
                    break;

                case "Quản trị viên":
                    // quyền cao nhất → không ẩn gì
                    break;
            }
        }

        private void btnSP_Click(object sender, EventArgs e)
        {
            if (sp == null)
            {
                sp = new FrmSanPham();
                sp.FormClosed += FrmSanPham_FormClosed;

                // GÁN CALLBACK
                sp.OpenFrmLSP = OpenFrmLSP;
                sp.OpenFrmTH = OpenFrmTH;
                sp.OpenFrmCN = OpenFrmCN;

                sp.MdiParent = this;
                sp.Dock = DockStyle.Fill;
                sp.Show();
            }
            else
            {
                sp.Activate();
            }
        }

        private void FrmSanPham_FormClosed(object sender, EventArgs e)
        {
            sp = null;
        }

        private void OpenFrmCTDH()
        {
            

            if (ctdh == null || ctdh.IsDisposed)
            {
                ctdh = new FrmCTDonHang();
                ctdh.FormClosed += (s, e) => ctdh = null;

                ctdh.MdiParent = this;
                ctdh.Dock = DockStyle.Fill;
                ctdh.Show();
                ctdh.BringToFront();
            }
            else
            {
                ctdh.Activate();
            }
        }

        private void OpenFrmLSP()
        {
            if (lsp == null)
            {
                lsp = new FrmLoaiSP();
                lsp.FormClosed += FrmLoaiSP_FormClosed;
                lsp.MdiParent = this;
                lsp.Dock = DockStyle.Fill;
                lsp.Show();

            }
            else
            {
                lsp.Activate();
            }
        }

        private void FrmLoaiSP_FormClosed(object sender, EventArgs e)
        {
            lsp = null;
        }

        private void OpenFrmTH()
        {
            if (th == null)
            {
                th = new FrmThuongHieu();
                th.FormClosed += FrmThuongHieu_FormClosed;
                th.MdiParent = this;
                th.Dock = DockStyle.Fill;
                th.Show();

            }
            else
            {
                th.Activate();
            }
        }

        private void FrmThuongHieu_FormClosed(object sender, EventArgs e)
        {
            th = null;
        }

        private void OpenFrmCN()
        {
            if (cn == null)
            {
                cn = new FrmChiNhanh();
                cn.FormClosed += FrmChiNhanh_FormClosed;
                cn.MdiParent = this;
                cn.Dock = DockStyle.Fill;
                cn.Show();

            }
            else
            {
                cn.Activate();
            }
        }

        private void FrmChiNhanh_FormClosed(object sender, EventArgs e)
        {
            cn = null;
        }


        private void btnTT_Click(object sender, EventArgs e)
        {
            if (tt == null)
            {
                tt = new FrmThanhToan();
                tt.FormClosed += FrmThanhToan_FormClosed;
                tt.MdiParent = this;
                tt.Dock = DockStyle.Fill;
                tt.Show();
            }
            else
            {
                tt.Activate();
            }
        }

        private void FrmThanhToan_FormClosed(object sender, EventArgs e)
        {
            tt = null;
        }

        private void btnHD_Click(object sender, EventArgs e)
        {
            if (hd == null)
            {
                hd = new FrmHoaDon();
                hd.FormClosed += FrmHoaDon_FormClosed;
                hd.MdiParent = this;
                hd.Dock = DockStyle.Fill;
                hd.Show();
            }
            else
            {
                hd.Activate();
            }
        }

        private void FrmHoaDon_FormClosed(object sender, EventArgs e)
        {
            hd = null;
        }

        private void btnKH_Click(object sender, EventArgs e)
        {
            if (kh == null)
            {
                kh = new FrmKhachHang();
                kh.FormClosed += FrmKhachHang_FormClosed;

                // GÁN CALLBACK
                kh.OpenFrmTKKhachHang = OpenFrmTKKhachHang;

                kh.MdiParent = this;
                kh.Dock = DockStyle.Fill;
                kh.Show();
            }
            else
            {
                kh.Activate();
            }
        }

        private void FrmKhachHang_FormClosed(object sender, EventArgs e)
        {
            kh = null;
        }

        private void OpenFrmTKKhachHang()
        {
            if (tkkh == null)
            {
                tkkh = new FrmTKKhachHang();
                tkkh.FormClosed += FrmTKKhachHang_FormClosed;
                tkkh.MdiParent = this;
                tkkh.Dock = DockStyle.Fill;
                tkkh.Show();

            }
            else
            {
                tkkh.Activate();
            }
        }

        private void FrmTKKhachHang_FormClosed(object sender, EventArgs e)
        {
            tkkh = null;
        }


        private void btnKM_Click(object sender, EventArgs e)
        {
            if (km == null)
            {
                km = new FrmKhuyenMai();
                km.FormClosed += FrmKhuyenMai_FormClosed;
                km.MdiParent = this;
                km.Dock = DockStyle.Fill;
                km.Show();
            }
            else
            {
                km.Activate();
            }
        }

        private void FrmKhuyenMai_FormClosed(object sender, EventArgs e)
        {
            km = null;
        }

        private void btnNCC_Click(object sender, EventArgs e)
        {
            if (ncc == null)
            {
                ncc = new FrmNhaCungCap();
                ncc.FormClosed += FrmNhaCungCap_FormClosed;
                ncc.MdiParent = this;
                ncc.Dock = DockStyle.Fill;
                ncc.Show();
            }
            else
            {
                ncc.Activate();
            }
        }

        private void FrmNhaCungCap_FormClosed(object sender, EventArgs e)
        {
            ncc = null;
        }

        private void btnPN_Click(object sender, EventArgs e)
        {
            if (pn == null)
            {
                pn = new FrmPhieuNhap();
                pn.FormClosed += FrmPhieuNhap_FormClosed;

                pn.OpenFrmCTPN = OpenFrmCTPN;

                pn.MdiParent = this;
                pn.Dock = DockStyle.Fill;
                pn.Show();
            }
            else
            {
                pn.Activate();
            }
        }

        private void FrmPhieuNhap_FormClosed(object sender, EventArgs e)
        {
            pn = null;
        }

        private void OpenFrmCTPN()
        {
            if (ctpn == null || ctpn.IsDisposed)
            {
                ctpn = new FrmCTPhieuNhap();
                ctpn.FormClosed += (s, e) => ctpn = null;

                ctpn.MdiParent = this;
                ctpn.Dock = DockStyle.Fill;
                ctpn.Show();
            }
            else
            {
                ctpn.Activate();
            }
        }

        private void btnPX_Click(object sender, EventArgs e)
        {
            if (px == null)
            {
                px = new FrmPhieuXuat();
                px.FormClosed += FrmPhieuXuat_FormClosed;

                px.OpenFrmCTPX = OpenFrmCTPX;

                px.MdiParent = this;
                px.Dock = DockStyle.Fill;
                px.Show();
            }
            else
            {
                px.Activate();
            }
        }

        private void FrmPhieuXuat_FormClosed(object sender, EventArgs e)
        {
            px = null;
        }

        private void OpenFrmCTPX()
        {
            if (ctpx == null || ctpx.IsDisposed)
            {
                ctpx = new FrmCTPhieuXuat();
                ctpx.FormClosed += (s, e) => ctpx = null;

                ctpx.MdiParent = this;
                ctpx.Dock = DockStyle.Fill;
                ctpx.Show();
            }
            else
            {
                ctpx.Activate();
            }
        }


        private void btnTK_Click(object sender, EventArgs e)
        {
            if (tonkho == null)
            {
                tonkho = new FrmTonKho();
                tonkho.FormClosed += FrmTonKho_FormClosed;
                tonkho.MdiParent = this;
                tonkho.Dock = DockStyle.Fill;
                tonkho.Show();
            }
            else
            {
                tonkho.Activate();
            }
        }

        private void FrmTonKho_FormClosed(object sender, EventArgs e)
        {
            tonkho = null;
        }

        private void btnTN_Click(object sender, EventArgs e)
        {

        }

        private void btnNV_Click(object sender, EventArgs e)
        {
            if (nv == null)
            {
                nv = new FrmNhanVien();
                nv.FormClosed += FrmNhanVien_FormClosed;

                // GÁN CALLBACK
                nv.OpenFrmTKNV = OpenFrmTKNV;

                nv.MdiParent = this;
                nv.Dock = DockStyle.Fill;
                nv.Show();
            }
            else
            {
                nv.Activate();
            }
        }

        private void FrmNhanVien_FormClosed(object sender, EventArgs e)
        {
            nv = null;
        }

        private void OpenFrmTKNV()
        {
            if (tknv == null)
            {
                tknv = new FrmTKNhanVien();
                tknv.FormClosed += FrmTKNhanVien_FormClosed;
                tknv.MdiParent = this;
                tknv.Dock = DockStyle.Fill;
                tknv.Show();

            }
            else
            {
                tknv.Activate();
            }
        }

        private void FrmTKNhanVien_FormClosed(object sender, EventArgs e)
        {
            tknv = null;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            if (cd == null)
            {
                cd = new FrmCaiDat(_maNV);
                cd.FormClosed += FrmCaiDat_FormClosed;
                cd.MdiParent = this;
                cd.Dock = DockStyle.Fill;
                cd.Show();
            }
            else
            {
                cd.Activate();
            }
        }

        private void FrmCaiDat_FormClosed(object sender, EventArgs e)
        {
            cd = null;
        }

        private void btnDXuat_Click(object sender, EventArgs e)
        {
            // Hỏi lại người dùng (tùy chọn)
            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất không?",
                "Đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                this.Close(); // Ẩn form chính trước

                FrmDangNhap frmLogin = new FrmDangNhap();
                frmLogin.Show();
            }
        }

        private void btnTQ_Click(object sender, EventArgs e)
        {
            OpenFormTongQuanTheoQuyen();
        }

        private void FrmTongQuan_FormClosed(object sender, EventArgs e)
        {
            tq = null;
        }
        private void FormTongQuanNV_FormClosed(object sender, FormClosedEventArgs e)
        {
            tqnv = null;
        }

        private void OpenFrmBaoCao()
        {
            if (bc == null || ctdh.IsDisposed)
            {
                bc = new FrmBaoCao();
                bc.FormClosed += (s, e) => bc = null;

                bc.MdiParent = this;
                bc.Dock = DockStyle.Fill;
                bc.Show();
                bc.BringToFront();
            }
            else
            {
                bc.Activate();
            }
        }

       

        private void btnSaoLuu_Click(object sender, EventArgs e)
        {
            FrmBackUp frm = new FrmBackUp();
            frm.ShowDialog();
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            FrmRestore frm = new FrmRestore();
            frm.ShowDialog();
        }

        private void btnChatBot_Click(object sender, EventArgs e)
        {
            FrmTinNhan frm = new FrmTinNhan();
            frm.ShowDialog();
        }
    }
}
