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
    public partial class FrmNhapCTPX : Form
    {
        Ket_noi kn = new Ket_noi();
        public string MaPhieuXuat { get; set; }
        public string MaDonHang { get; set; }

        // dùng cho EDIT
        public string MaSanPham_Edit { get; set; }
        public int SoLuong_Edit { get; set; }
        public decimal DonGia_Edit { get; set; }

        public bool IsEdit { get; set; } = false;

        public FrmNhapCTPX(string maPX, string maDH)
        {
            InitializeComponent();

            MaPhieuXuat = maPX;
            MaDonHang = maDH;

            IsEdit = false;
        }

        // ===== EDIT =====
        public FrmNhapCTPX(string maPX, string maDH, string maSP, int soLuong, decimal donGia)
        {
            if (string.IsNullOrWhiteSpace(maSP))
                throw new ArgumentException("Mã sản phẩm không hợp lệ");

            InitializeComponent();

            MaPhieuXuat = maPX;
            MaDonHang = maDH;

            IsEdit = true;
            MaSanPham_Edit = maSP;
            SoLuong_Edit = soLuong;
            DonGia_Edit = donGia;
        }

        //private bool DaTonTaiSPTrongCTPX()
        //{
        //    if (cboMaSP.SelectedIndex == -1) return false;

        //    string sql = $@"
        //    SELECT 1 
        //    FROM tblCTPhieuXuat 
        //    WHERE MaPhieuXuat = '{maPX}'
        //      AND MaSanPham = '{cboMaSP.SelectedValue}'
        //";

        //    return kn.ExecuteQuery(sql).Rows.Count > 0;
        //}

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (cboMaSP.SelectedValue == null ||
             nupSL.Value <= 0 ||
             string.IsNullOrWhiteSpace(txtDonGia.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!");
                return;
            }

            string sqlCheck = $@"
                select count(*) 
                from tblctphieuxuat
                where maphieuxuat = '{MaPhieuXuat}'
                  and masanpham = '{cboMaSP.SelectedValue}'
            ";

            int exists = Convert.ToInt32(kn.ExecuteScalar(sqlCheck));
            string sql;

            if (exists == 0)
            {
                // ===== INSERT =====
                sql = $@"
                    insert into tblctphieuxuat
                    values (
                        '{MaPhieuXuat}',
                        '{cboMaSP.SelectedValue}',
                        {nupSL.Value},
                        {txtDonGia.Text}
                    )
                ";
            }
            else
            {
                // ===== UPDATE =====
                sql = $@"
                    update tblctphieuxuat
                    set soluong = {nupSL.Value},
                        dongia = {txtDonGia.Text}
                    where maphieuxuat = '{MaPhieuXuat}'
                      and masanpham = '{cboMaSP.SelectedValue}'
                ";
            }

            int rows = kn.ExecuteNonQuery(sql);

            if (rows <= 0)
            {
                MessageBox.Show("Không thể lưu chi tiết phiếu xuất. Vui lòng thử lại!");
                return; 
            }

            MessageBox.Show("Lưu chi tiết phiếu xuất thành công!");
            this.DialogResult = DialogResult.OK; // tốt cho form cha
            this.Close();
        }

        private void LoadComboSanPham()
        {
            string sql = "SELECT MaSanPham FROM tblSanPham";
            DataTable dt = kn.ExecuteQuery(sql);

            cboMaSP.DataSource = dt;
            cboMaSP.DisplayMember = "MaSanPham";
            cboMaSP.ValueMember = "MaSanPham";
            cboMaSP.SelectedIndex = -1;
        }

        private void LoadSanPhamTheoDonHang()
        {
            if (string.IsNullOrWhiteSpace(MaDonHang))
            {
                dgvSPCTPX.DataSource = null;
                return;
            }

            string sql = $@"
        select 
            ctdh.masanpham,
            sp.tensanpham,
            ctdh.soluong,
            ctdh.dongia
        from tblchitietdonhang ctdh
        join tblsanpham sp on ctdh.masanpham = sp.masanpham
        where ctdh.madonhang = '{MaDonHang}'
    ";

            dgvSPCTPX.DataSource = kn.ExecuteQuery(sql);
        }

        private void dgvSPCTPX_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvSPCTPX.Rows[e.RowIndex];

            if (row.Cells["MaSanPham"].Value == null) return;

            cboMaSP.SelectedValue = row.Cells["MaSanPham"].Value.ToString();
            nupSL.Value = Convert.ToDecimal(row.Cells["SoLuong"].Value);
            txtDonGia.Text = row.Cells["DonGia"].Value.ToString();
        }


        private void FrmNhapCTPX_Load(object sender, EventArgs e)
        {
            SetupDgvSPCTPX();
            LoadComboSanPham();
            LoadSanPhamTheoDonHang();

            dgvSPCTPX.CellDoubleClick -= dgvSPCTPX_CellDoubleClick; // chống gắn trùng
            dgvSPCTPX.CellDoubleClick += dgvSPCTPX_CellDoubleClick;

            if (IsEdit)
            {
                cboMaSP.SelectedValue = MaSanPham_Edit;
                nupSL.Value = SoLuong_Edit;
                txtDonGia.Text = DonGia_Edit.ToString();
                cboMaSP.Enabled = false;
            }
            else
            {
                cboMaSP.SelectedIndex = -1;
                nupSL.Value = 1;
                txtDonGia.Text = "";
                cboMaSP.Enabled = true;
            }

        }

        private void SetupDgvSPCTPX()
        {
            txtMaPX.Text = MaPhieuXuat;
            txtMaPX.ReadOnly = true;

            dgvSPCTPX.AutoGenerateColumns = false;
            dgvSPCTPX.Columns.Clear();
            dgvSPCTPX.AllowUserToAddRows = false;

            dgvSPCTPX.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaSanPham",
                HeaderText = "Mã sản phẩm",
                DataPropertyName = "masanpham"
            });

            dgvSPCTPX.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSanPham",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "tensanpham"
            });

            dgvSPCTPX.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "soluong"
            });

            dgvSPCTPX.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonGia",
                HeaderText = "Đơn giá",
                DataPropertyName = "dongia"
            });
        }

    }
}
