using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{

    public partial class FrmNhapCTDH : Form
    {
        Ket_noi kn = new Ket_noi();
        string maDH, maSP, mode;

        // CALLBACK để báo cho form cha update tiền hàng
        public delegate void ReloadCT();
        public ReloadCT onSaved;

        public FrmNhapCTDH(string _maDH, string _maSP, string soLuong, string donGia, string _mode)
        {
            InitializeComponent();
            maDH = _maDH;
            maSP = _maSP;
            mode = _mode;

            txtMaDH.Text = maDH;

            LoadComboMaSP();
            LoadDgvSanPham();

            cboMaSP.SelectedValue = maSP;
            nupSL.Value = Convert.ToDecimal(soLuong);
            txtDonGia.Text = donGia;

            this.Shown += FrmNhapCTDH_Shown;
        }

        public FrmNhapCTDH(string _maDH, string _maSP, string _mode)
        {
            InitializeComponent();
            maDH = _maDH;
            maSP = _maSP;
            mode = _mode;

            txtMaDH.Text = maDH;

            LoadComboMaSP();

            if (mode == "edit" && !string.IsNullOrEmpty(maSP))
                LoadDataEdit();

            this.Shown += FrmNhapCTDH_Shown;
        }

        private void FrmNhapCTDH_Load(object sender, EventArgs e)
        {
            txtMaDH.Text = maDH;

            LoadComboMaSP();
            //LoadDgvSanPham();

            if (mode == "edit")
                LoadDataEdit();

            dgvSPDH.CellDoubleClick += dgvSPDH_CellDoubleClick;
        }

        private void FrmNhapCTDH_Shown(object sender, EventArgs e)
        {
            LoadDgvSanPham();

        }

        private void LoadComboMaSP()
        {
            string sql = "SELECT MaSanPham, TenSanPham FROM tblSanPham WHERE TrangThaiSanPham = 1";
            DataTable dt = kn.ExecuteQuery(sql);

            cboMaSP.DataSource = dt;
            cboMaSP.ValueMember = "MaSanPham";
            cboMaSP.DisplayMember = "MaSanPham";
            cboMaSP.SelectedIndex = -1;

            cboMaSP.SelectedIndexChanged += CboMaSP_SelectedIndexChanged;
            txtTimKiemSP.KeyDown += TxtTimKiemSP_KeyDown;
        }

        private void CboMaSP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaSP.SelectedIndex == -1) return;

            string masp = cboMaSP.SelectedValue.ToString();
            object gia = kn.ExecuteScalar($"SELECT GiaBan FROM tblSanPham WHERE MaSanPham = '{masp}'");

            if (gia != null)
                txtDonGia.Text = Convert.ToDecimal(gia).ToString("N0");
        }

        private void LoadDgvSanPham(string keyword = "")
        {
            string sql = $@"
            SELECT MaSanPham, TenSanPham, GiaBan, HinhAnh
            FROM tblSanPham
            WHERE MaSanPham LIKE '%{keyword}%'
               OR TenSanPham LIKE N'%{keyword}%'
               OR MaVach LIKE '%{keyword}%'
        ";

            DataTable dt = kn.ExecuteQuery(sql);
            dgvSPDH.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                Image img = null;
                string fileName = row["HinhAnh"].ToString();

                try
                {
                    string fullPath = Path.Combine(
                        Application.StartupPath, @"..\..\Images\" + fileName
                    );

                    if (File.Exists(fullPath))
                    {
                        using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            // Clone ảnh để tránh lock file
                            img = Image.FromStream(fs).Clone() as Image;
                        }
                    }
                }
                catch { }

                dgvSPDH.Rows.Add(
                    row["MaSanPham"].ToString(),
                    row["TenSanPham"].ToString(),
                    Convert.ToDecimal(row["GiaBan"]).ToString("N0"),
                    img
                );
            }

            dgvSPDH.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void TxtTimKiemSP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadDgvSanPham(txtTimKiemSP.Text.Trim());
                e.SuppressKeyPress = true;
            }
        }

        private void dgvSPDH_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string masp = dgvSPDH.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();
            cboMaSP.SelectedValue = masp;
        }

        private bool CheckTrungSP(string masp)
        {
            string sql = $@"
                SELECT COUNT(*) FROM tblChiTietDonHang
                WHERE MaDonHang = '{maDH}' AND MaSanPham = '{masp}'
            ";
            int kq = Convert.ToInt32(kn.ExecuteScalar(sql));

            if (mode == "edit" && masp == maSP)
                return false;

            return kq > 0;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string masp = cboMaSP.SelectedValue.ToString();

            if (mode == "add" && CheckTrungSP(masp))
            {
                MessageBox.Show("Sản phẩm đã có trong đơn hàng!");
                return;
            }

            // Thêm dòng này để đảm bảo UPDATE đúng
            maSP = masp;

            int sl = (int)nupSL.Value;
            decimal dongia = decimal.Parse(txtDonGia.Text,
            System.Globalization.NumberStyles.Any);

            string sql = "";

            if (mode == "add")
            {
                sql = $@"
                    INSERT INTO tblChiTietDonHang(MaDonHang, MaSanPham, SoLuong, DonGia)
                    VALUES('{maDH}', '{masp}', {sl}, {dongia})
                ";
            }
            else
            {
                sql = $@"
                    UPDATE tblChiTietDonHang SET
                        MaSanPham = '{masp}',
                        SoLuong = {sl},
                        DonGia = {dongia}
                    WHERE MaDonHang = '{maDH}' AND MaSanPham = '{maSP}'
                ";
            }

            kn.ExecuteNonQuery(sql);
            MessageBox.Show("Lưu thành công!");

            onSaved?.Invoke(); // Gọi form cha cập nhật tiền hàng

            this.Close();
        }


        private void LoadDataEdit()
        {
            string sql = $@"
                SELECT * FROM tblChiTietDonHang
                WHERE MaDonHang = '{maDH}' AND MaSanPham = '{maSP}'
            ";

            DataTable dt = kn.ExecuteQuery(sql);
            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            cboMaSP.SelectedValue = r["MaSanPham"].ToString();
            nupSL.Value = Convert.ToDecimal(r["SoLuong"]);
            txtDonGia.Text = Convert.ToDecimal(r["DonGia"]).ToString("N0");
        }



    }
}
