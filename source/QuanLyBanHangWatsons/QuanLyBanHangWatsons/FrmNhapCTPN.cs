using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace QuanLyBanHangWatsons
{
    public partial class FrmNhapCTPN : Form
    {
        Ket_noi kn = new Ket_noi();
        string maPN, maSP, mode;

        public FrmNhapCTPN(string _maPN, string _maSP, string soLuong, string giaNhap, DateTime nsx, DateTime hsd, string _mode)
        {
            InitializeComponent();
            maPN = _maPN;
            maSP = _maSP;
            mode = _mode;

            txtMaPN.Text = maPN;

            LoadComboMaSP();
            LoadDgvSanPham();

            cboMaSP.SelectedValue = maSP;
            nupSL.Value = Convert.ToDecimal(soLuong);
            txtGiaNhap.Text = giaNhap;
            dtpNSX.Value = nsx;
            dtpHSD.Value = hsd;
        }

        public FrmNhapCTPN(string _maPN, string _mode)
        {
            InitializeComponent();
            maPN = _maPN;
            mode = _mode;

            txtMaPN.Text = maPN;

            LoadComboMaSP();
            LoadDgvSanPham();

            maSP = "";
        }

        private void FrmNhapCTPN_Load(object sender, EventArgs e)
        {
            txtMaPN.Text = maPN;
            LoadComboMaSP();
            LoadDgvSanPham();

            if (mode == "edit")
                LoadDataEdit();

            dgvSPCTPN.CellDoubleClick += dgvSPCTPN_CellDoubleClick;
        }

        private void LoadComboMaSP()
        {
            string sql = "SELECT MaSanPham, TenSanPham FROM tblSanPham";
            DataTable dt = kn.ExecuteQuery(sql);

            cboMaSP.DataSource = dt;
            cboMaSP.ValueMember = "MaSanPham";
            cboMaSP.DisplayMember = "MaSanPham";
            cboMaSP.SelectedIndex = -1;

            txtTimKiemSP.KeyDown += TxtTimKiemSP_KeyDown;
        }

        private void LoadDgvSanPham(string keyword = "")
        {
            string sql = $@"
                SELECT MaSanPham, TenSanPham, HinhAnh
                FROM tblSanPham
                WHERE MaSanPham LIKE '%{keyword}%'
                   OR TenSanPham LIKE N'%{keyword}%'
                   OR MaVach LIKE '%{keyword}%'
            ";

            DataTable dt = kn.ExecuteQuery(sql);
            dgvSPCTPN.Rows.Clear();

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

                dgvSPCTPN.Rows.Add(
                    row["MaSanPham"].ToString(),
                    row["TenSanPham"].ToString(),
                    img
                );
            }

            //dgvSPCTPN.RowTemplate.Height = 70;  // chiều cao dòng hợp lý
            dgvSPCTPN.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //dgvSPCTPN.Columns["PicSanPham "].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //dgvSPCTPN.Columns["PicSanPham "].Width = 70;  // nhỏ gọn, không phóng to
        }

        private void dgvSPCTPN_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string ma = dgvSPCTPN.Rows[e.RowIndex].Cells["MaSanPham"].Value.ToString();
            cboMaSP.SelectedValue = ma;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cboMaSP.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGiaNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập giá nhập!");
                return;
            }

            string masp = cboMaSP.SelectedValue.ToString();
            int sl = (int)nupSL.Value;
            decimal gianhap = decimal.Parse(txtGiaNhap.Text);
            string nsx = dtpNSX.Value.ToString("yyyy-MM-dd");
            string hsd = dtpHSD.Value.ToString("yyyy-MM-dd");

            string sql = "";

            if (mode == "add")
            {
                sql = $@"
                    INSERT INTO tblCTPhieuNhap(MaPhieuNhap, MaSanPham, SoLuong, GiaNhap, NgaySanXuat, HanSuDung)
                    VALUES('{maPN}', '{masp}', {sl}, {gianhap}, '{nsx}', '{hsd}')
                ";
            }
            else //edit
            {
                sql = $@"
                    UPDATE tblCTPhieuNhap SET
                        MaSanPham='{masp}',
                        SoLuong={sl},
                        GiaNhap={gianhap},
                        NgaySanXuat='{nsx}',
                        HanSuDung='{hsd}'
                    WHERE MaPhieuNhap='{maPN}' AND MaSanPham='{maSP}'
                ";
            }

            kn.ExecuteNonQuery(sql);

            MessageBox.Show("Lưu thành công!");
            this.Close();
        }

        private void LoadDataEdit()
        {
            string sql = $@"
                SELECT * FROM tblCTPhieuNhap 
                WHERE MaPhieuNhap = '{maPN}' AND MaSanPham = '{maSP}'
            ";

            DataTable dt = kn.ExecuteQuery(sql);
            if (dt.Rows.Count == 0) return;

            DataRow r = dt.Rows[0];

            cboMaSP.SelectedValue = r["MaSanPham"].ToString();
            nupSL.Value = Convert.ToDecimal(r["SoLuong"]);
            txtGiaNhap.Text = r["GiaNhap"].ToString();
            dtpNSX.Value = Convert.ToDateTime(r["NgaySanXuat"]);
            dtpHSD.Value = Convert.ToDateTime(r["HanSuDung"]);
        }

        private void TxtTimKiemSP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadDgvSanPham(txtTimKiemSP.Text.Trim());
                e.SuppressKeyPress = true;
            }
        }
    }
}
