using QuanLyBanHangWatsons.AI;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHangWatsons
{
    public partial class FrmTinNhan : Form
    {
        Ket_noi kn = new Ket_noi();

        // UI: dynamic flow panel to hold message bubbles
        private FlowLayoutPanel _flowMessages;
        private ChatContext _chatContext = new ChatContext();

        private readonly string[] GreetingKeywords =
{
    "xin chào", "chào", "hello", "hi", "hey"
};

        private readonly string[] GreetingReplies =
{
    "👋 Xin chào! Tôi có thể hỗ trợ gì cho bạn?",
    "😊 Chào bạn! Bạn muốn xem thông tin gì?",
    "🤖 Chào nhé! Hỏi tôi về tồn kho, đơn hàng, nhân viên nha."
};
        private readonly string[] HelpKeywords = { "help", "giúp", "hướng dẫn", "làm được gì" };

        private string GetRandomGreetingReply()
        {
            Random rnd = new Random();
            return GreetingReplies[rnd.Next(GreetingReplies.Length)];
        }


        public FrmTinNhan()
        {
            InitializeComponent();

            this.Load += FrmTinNhan_Load;
            this.FormClosing += FrmTinNhan_FormClosing;
        }

        private void FrmTinNhan_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;

            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            this.Location = new Point(
                workingArea.Right - this.Width - 10,
                workingArea.Bottom - this.Height - 10
            );
            // tạo flow panel hiển thị tin nhắn
            _flowMessages = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Width = pnlHienThiTN.Width - 20,
                BackColor = Color.White,
                Location = new Point(0, guna2ShadowPanel1.Bottom + 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            int availableHeight = pnlHienThiTN.Height - guna2ShadowPanel1.Height - 40;
            if (availableHeight < 100) availableHeight = 300;

            _flowMessages.Size = new Size(pnlHienThiTN.Width - 20, availableHeight);
            pnlHienThiTN.Controls.Add(_flowMessages);
            _flowMessages.BringToFront();

            // sự kiện
            btnGui.Click += BtnGui_Click;
            txtMessage.KeyDown += TxtMessage_KeyDown;

            // Tin nhắn chào
            AddSystemMessage("🤖 Xin chào! Tôi là ChatBot hệ thống.");
            AddSystemMessage("Gợi ý: tồn kho SP0001 | đơn hàng | nhân viên");
        }

        private string HandleOfflineSmallTalk(string message)
        {

            // CHÀO HỎI
            if (ContainsAny(message, GreetingKeywords))
            {
                return GetRandomGreetingReply();
            }


            // ===== HELP =====
            if (ContainsAny(message, HelpKeywords))
            {
                return
        @"📘 HƯỚNG DẪN CHATBOT

🔹 Ví dụ câu hỏi:
• tồn kho SP0001
• còn hàng SP0002 không
• có bao nhiêu đơn hàng
• có bao nhiêu nhân viên

🤖 Câu hỏi phức tạp hơn tôi sẽ nhờ AI hỗ trợ.";
            }

            return null;
        }

        private async void BtnGui_Click(object sender, EventArgs e)
        {
            await SendMessage();
        }

        private async void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await SendMessage();
            }
        }

        private async Task SendMessage()
        {
            string userText = txtMessage.Text.Trim();
            if (string.IsNullOrEmpty(userText)) return;

            AddMessage(true, userText);
            txtMessage.Clear();

            string botReply = await GetBotResponseAsync(userText);
            AddMessage(false, botReply);
        }

        /* =========================
           LOGIC CHATBOT SQL
        ========================= */

        private async Task<string> GetBotResponseAsync(string message)
        {
            string rawMessage = message;
            message = message.ToLower();

            // ===== 1. OFFLINE SMALL TALK =====
            string offlineReply = HandleOfflineSmallTalk(message);
            if (offlineReply != null)
                return offlineReply;

            // ===== 2. TỒN KHO =====
            if (ContainsAny(message, "tồn kho", "còn hàng", "bao nhiêu"))
            {
                string maSP = ExtractMaSP(message) ?? _chatContext.LastMaSP;
                if (string.IsNullOrEmpty(maSP))
                    return "⚠️ Bạn muốn hỏi tồn kho sản phẩm nào? (VD: SP0001)";

                _chatContext.LastMaSP = maSP;
                return TraCuuTonKho(maSP);
            }

            // ===== CHI NHÁNH =====
            if (ContainsAny(message, "chi nhánh", "theo chi nhánh", "xem chi tiết", "có"))
            {
                if (string.IsNullOrEmpty(_chatContext.LastMaSP))
                    return "⚠️ Bạn đang muốn xem chi nhánh của sản phẩm nào?";

                return TraCuuTonKhoTheoChiNhanh(_chatContext.LastMaSP);
            }

            // ===== 3. ĐƠN HÀNG =====
            if (message.Contains("đơn"))
                return DemDonHang();

            // ===== 4. NHÂN VIÊN =====
            if (message.Contains("nhân viên"))
                return DemNhanVien();

            // ===== 5. SẢN PHẨM =====
            if (message.Contains("sản phẩm"))
                return DemSanPham();

            // ===== 6. GEMINI (CHỈ KHI CẦN) =====
            string aiReply = await GeminiAiService.AskAsync(rawMessage);

            // Gemini hết quota
            if (aiReply.Contains("hết lượt") || aiReply.Contains("Gemini"))
            {
                return
                @"🤖 Tôi hiện không thể trả lời câu hỏi này bằng AI.

            👉 Bạn có thể hỏi:
            • tồn kho sản phẩm
            • số đơn hàng
            • số nhân viên
            • số sản phẩm";
            }

            return aiReply;
        }

        private string ExtractMaSP(string message)
        {
            var match = Regex.Match(message, @"sp[\s\-_]*\d+", RegexOptions.IgnoreCase);
            return match.Success ? match.Value.ToUpper() : null;
        }

        private bool ContainsAny(string message, params string[] keywords)
        {
            foreach (string k in keywords)
                if (message.Contains(k)) return true;
            return false;
        }

        private string TraCuuTonKho(string maSP)
        {
            _chatContext.LastMaSP = maSP; // 🔥 QUAN TRỌNG

            string sql = @"
        select sum(SoLuongTon)
        from tblTonKho
        where MaSanPham = @MaSP";

            SqlParameter[] param =
            {
        new SqlParameter("@MaSP", maSP)
    };

            object result = kn.ExecuteScalarWithParams(sql, param);

            if (result == null || result == DBNull.Value)
                return $"❌ Không tìm thấy sản phẩm {maSP}";

            return
        $@"📦 Thông tin tồn kho:
• Mã sản phẩm: {maSP}
• Tổng tồn: {result}
• Trạng thái: {(Convert.ToInt32(result) > 0 ? "Còn bán" : "Hết hàng")}

👉 Bạn muốn xem **chi tiết theo chi nhánh** không?";
        }

        private string TraCuuTonKhoTheoChiNhanh(string maSP)
        {
            string sql = @"
        select cn.TenChiNhanh, sum(tk.SoLuongTon) as SoLuong
        from tblTonKho tk
        join tblChiNhanh cn on tk.MaChiNhanh = cn.MaChiNhanh
        where tk.MaSanPham = @MaSP
        group by cn.TenChiNhanh";

            SqlParameter[] param =
            {
        new SqlParameter("@MaSP", maSP)
    };

            DataTable dt = kn.ExecuteQueryWithParams(sql, param);

            if (dt.Rows.Count == 0)
                return $"❌ Không có tồn kho theo chi nhánh cho {maSP}";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"🏬 Tồn kho theo chi nhánh ({maSP}):");

            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine($"• {row["TenChiNhanh"]}: {row["SoLuong"]}");
            }

            return sb.ToString();
        }



        private string DemDonHang()
        {
            object count = kn.ExecuteScalar("select count(*) from tblDonHang");
            return $"🧾 Tổng số đơn hàng hiện có: {count}";
        }

        private string DemNhanVien()
        {
            object count = kn.ExecuteScalar("select count(*) from tblNhanVien");
            return $"👤 Tổng số nhân viên: {count}";
        }

        private string DemKhachHang()
        {
            object count = kn.ExecuteScalar("select count(*) from tblKhachHang");
            return $"👥 Tổng số khách hàng hiện có: {count}";
        }

        private string DemSanPham()
        {
            object count = kn.ExecuteScalar("select count(*) from tblSanPham");
            return $"📦 Tổng số sản phẩm đang quản lý: {count}";
        }

        /* =========================
           HIỂN THỊ TIN NHẮN (UI)
        ========================= */

        private void AddMessage(bool isMine, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddMessage(isMine, text)));
                return;
            }

            // mỗi dòng chat
            FlowLayoutPanel row = new FlowLayoutPanel
            {
                AutoSize = true,
                WrapContents = false,
                Width = _flowMessages.ClientSize.Width - 10,
                Margin = new Padding(0, 4, 0, 4)
            };

            Label lbl = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(_flowMessages.ClientSize.Width * 60 / 100, 0),
                Text = text,
                Font = new Font("Segoe UI", 9f),
                Padding = new Padding(10),
                Margin = new Padding(6),
                TextAlign = ContentAlignment.MiddleLeft
            };

            if (isMine)
            {
                // ===== NGƯỜI DÙNG (BÊN PHẢI) =====
                row.FlowDirection = FlowDirection.RightToLeft;

                lbl.BackColor = Color.FromArgb(0, 120, 215);
                lbl.ForeColor = Color.White;
            }
            else
            {
                // ===== CHATBOT (BÊN TRÁI) =====
                row.FlowDirection = FlowDirection.LeftToRight;

                lbl.BackColor = Color.FromArgb(240, 240, 240);
                lbl.ForeColor = Color.Black;
            }

            row.Controls.Add(lbl);
            _flowMessages.Controls.Add(row);

            _flowMessages.ScrollControlIntoView(row);
        }

        private void AddSystemMessage(string text)
        {
            Label lbl = new Label
            {
                AutoSize = false,
                Text = $"--- {text} ---",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = Color.Gray,
                Width = _flowMessages.Width - 20,
                Height = 30,
                Margin = new Padding(3)
            };

            _flowMessages.Controls.Add(lbl);
            _flowMessages.ScrollControlIntoView(lbl);
        }

        private void FrmTinNhan_FormClosing(object sender, FormClosingEventArgs e)
        {
            // không cần xử lý gì thêm
        }
    }
}
