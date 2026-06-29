using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{
    public partial class FrmCTTonKho : Form
    {
        Ket_noi kn = new Ket_noi();
        string mode; // add hoặc edit
        string maKho;

        public FrmCTTonKho(string makho, string mode)
{
    InitializeComponent();
    this.mode = mode;

    if (mode == "add")
    {
        txtMaTK.Enabled = false;

        // ⭐⭐ GẮN MÃ TỰ ĐỘNG Ở ĐÂY ⭐⭐
        maKho = GenerateNewMaTK();
        txtMaTK.Text = maKho;
    }
    else if (mode == "edit")
    {
        this.maKho = makho;
        LoadDetail();
        btnThem.Enabled = false;
        txtMaTK.Enabled = false;
    }
    else
    {
        btnSua.Enabled = false;
        btnXoa.Enabled = false;
    }
}


        public FrmCTTonKho(string makho)
        {
            InitializeComponent();
            this.maKho = makho;

            // FORM CHỈ ĐỂ XEM
            btnThem.Visible = false;
            btnSua.Visible = false;
            btnXoa.Visible = false;

            txtMaTK.Enabled = false;
            txtSLTon.Enabled = false;
            txtSLSB.Enabled = false;

            cboMaSP.Enabled = false;
            cboMaCN.Enabled = false;
            cboMaNV.Enabled = false;

            
        }


        private void LoadDetail()
        {
            string sql = $"SELECT * FROM tblTonKho WHERE MaTonKho = '{maKho}'";
            DataTable dt = kn.ExecuteQuery(sql);

            if (dt.Rows.Count == 0) return;

            var r = dt.Rows[0];

            txtMaTK.Text = r["MaTonKho"].ToString();
            cboMaSP.SelectedValue = r["MaSanPham"].ToString();
            cboMaCN.SelectedValue = r["MaChiNhanh"].ToString();
            cboMaNV.SelectedValue = r["MaNhanVien"].ToString();

            txtSLSB.Text = r["SoLuongSanBan"].ToString();
            txtSLTon.Text = r["SoLuongTon"].ToString();           
            dtpNCN.Value = Convert.ToDateTime(r["NgayCapNhat"]);
        }

        private void FrmCTTonKho_Load(object sender, EventArgs e)
        {
            LoadComboBoxMaSP();
            LoadComboBoxMaCN();
            LoadComboBoxMaNV();

            LoadDetail();

            if (mode == "add")
            {
                if (string.IsNullOrWhiteSpace(txtMaTK.Text))
                {
                    //maKho = GenerateNewMaTK();   // ⭐ gán cho biến dùng INSERT
                    //txtMaTK.Text = maKho;        // ⭐ hiển thị lên textbox
                }
            }

            // Tạo cột hình cho dgvSPCTPN
            dgvSPCTPN.AutoGenerateColumns = false;
            dgvSPCTPN.AllowUserToAddRows = false;

            dgvSPCTPN.Columns.Clear();

            dgvSPCTPN.Columns.Add("MaSP", "Mã sản phẩm");
            dgvSPCTPN.Columns["MaSP"].DataPropertyName = "MaSanPham";

            dgvSPCTPN.Columns.Add("TenSP", "Tên sản phẩm");
            dgvSPCTPN.Columns["TenSP"].DataPropertyName = "TenSanPham";

            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "PicSP";
            imgCol.HeaderText = "Ảnh";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCol.Width = 70;
            dgvSPCTPN.Columns.Add(imgCol);

            // Load lần đầu
            LoadDanhSachSanPham();


            if (mode == "edit") LoadDetail();
            if (mode == "add") LoadDetail();

            // Sự kiện search
            txtTimKiemSP.KeyDown += TxtTimKiemSP_KeyDown;

            // Sự kiện chọn sản phẩm
            dgvSPCTPN.CellDoubleClick += DgvSPCTPN_CellDoubleClick;
        }

        private string GenerateNewMaTK()
        {
            string sql = "SELECT TOP 1 MaTonKho FROM tblTonKho ORDER BY MaTonKho DESC";
            var obj = kn.ExecuteScalar(sql);

            if (obj == null || obj == DBNull.Value)
                return "TK0001";

            string last = obj.ToString().Trim();

            // LẤY PHẦN SỐ CUỐI – GIỐNG HỆT GenerateNewMaDH
            int i = last.Length - 1;
            while (i >= 0 && char.IsDigit(last[i])) i--;

            string prefix = last.Substring(0, i + 1);   // TK
            string numPart = last.Substring(i + 1);     // 0001

            int num = 0;
            if (!int.TryParse(numPart, out num))
            {
                return prefix + "1";
            }

            num++;
            string newNum = num.ToString().PadLeft(numPart.Length, '0');
            return prefix + newNum;
        }


        private void LoadComboBoxMaSP()
        {
            string query = "SELECT MaSanPham  FROM tblSanPham";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaSP.DataSource = dt;
            cboMaSP.DisplayMember = "MaSanPham";
            cboMaSP.ValueMember = "MaSanPham";
            cboMaSP.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void LoadComboBoxMaCN()
        {
            string query = "SELECT MaChiNhanh  FROM tblChiNhanh";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaCN.DataSource = dt;
            cboMaCN.DisplayMember = "MaChiNhanh";
            cboMaCN.ValueMember = "MaChiNhanh";
            cboMaCN.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void LoadComboBoxMaNV()
        {
            string query = "SELECT MaNhanVien  FROM tblNhanVien";
            DataTable dt = kn.ExecuteQuery(query);

            cboMaNV.DataSource = dt;
            cboMaNV.DisplayMember = "MaNhanVien";
            cboMaNV.ValueMember = "MaNhanVien";
            cboMaNV.SelectedIndex = -1; // Không chọn sẵn dòng nào
        }

        private void LoadDanhSachSanPham(string keyword = "")
        {
            string sql = @"
            SELECT MaSanPham, TenSanPham, HinhAnh
            FROM tblSanPham
            WHERE 1 = 1";

                if (!string.IsNullOrEmpty(keyword))
                {
                    sql += $@" AND (MaSanPham LIKE N'%{keyword}%'
                         OR TenSanPham LIKE N'%{keyword}%'
                         OR MaVach LIKE N'%{keyword}%')";
                }

            DataTable dt = kn.ExecuteQuery(sql);

            dgvSPCTPN.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                Image img = null;

                try
                {
                    string fileName = row["HinhAnh"]?.ToString();

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string fullPath = Path.Combine(
                            Application.StartupPath,
                            @"..\..\Images\" + fileName
                        );

                        if (File.Exists(fullPath))
                        {
                            // Dùng Image.FromStream để tránh lỗi "file bị lock"
                            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                            {
                                img = Image.FromStream(fs);
                            }
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
        }

        private void TxtTimKiemSP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadDanhSachSanPham(txtTimKiemSP.Text.Trim());
                e.SuppressKeyPress = true;
            }
        }

        private void DgvSPCTPN_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string maSP = dgvSPCTPN.Rows[e.RowIndex].Cells["MaSP"].Value.ToString();

            cboMaSP.SelectedValue = maSP;
        }


        private void btnThem_Click(object sender, EventArgs e)
        {
            if (mode != "add") return;

 
            if (string.IsNullOrEmpty(cboMaSP.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cboMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cboMaCN.Text))
            {
                MessageBox.Show("Vui lòng chọn chi nhánh!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = $@"
                INSERT INTO tblTonKho
                (MaTonKho, MaSanPham, MaChiNhanh, MaNhanVien,
                 SoLuongSanBan, SoLuongTon, NgayCapNhat)
                VALUES
                ('{maKho}',
                 '{cboMaSP.Text}',
                '{cboMaCN.Text}',
                 '{cboMaNV.Text}',              
                 N'{txtSLSB.Text}',
                N'{txtSLTon.Text}',
                 '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}')";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Thêm bản ghi tồn kho thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // chỉ đóng form khi thêm thành công
                }
                else
                {
                    MessageBox.Show("Thêm thất bại! Dữ liệu không hợp lệ, vui lòng kiểm tra lại.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // KHÔNG đóng form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thêm thất bại!\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return; // KHÔNG đóng form nếu bị lỗi
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (mode != "edit" || string.IsNullOrEmpty(maKho))
            {
                MessageBox.Show("Không xác định bản ghi tồn kho cần xóa.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa bản ghi tồn kho này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                int rows = kn.ExecuteNonQuery(
                    $"DELETE FROM tblTonKho WHERE MaTonKho = '{maKho}'"
                );

                if (rows > 0)
                    MessageBox.Show("Xóa thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Xóa thất bại!", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa thất bại!\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (mode != "edit" || string.IsNullOrEmpty(maKho))
            {
                MessageBox.Show("Không xác định bản ghi tồn kho cần sửa.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sql = $@"
                UPDATE tblTonKho SET
                    MaSanPham = '{cboMaSP.Text}',
                    MaChiNhanh = '{cboMaCN.Text}',
                    MaNhanVien = '{cboMaNV.Text}',
                    SoLuongSanBan = N'{txtSLSB.Text}',
                    SoLuongTon = N'{txtSLTon.Text}',                  
                    NgayCapNhat = '{dtpNCN.Value:yyyy-MM-dd HH:mm:ss}'
                WHERE MaTonKho = '{maKho}'";

                int rows = kn.ExecuteNonQuery(sql);

                if (rows > 0)
                {
                    MessageBox.Show("Sửa thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // chỉ đóng form nếu sửa thành công
                }
                else
                {
                    MessageBox.Show("Sửa thất bại! Dữ liệu không hợp lệ hoặc không thay đổi.", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // KHÔNG đóng form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sửa thất bại!\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // KHÔNG đóng form khi lỗi
            }
        }
    }
}
