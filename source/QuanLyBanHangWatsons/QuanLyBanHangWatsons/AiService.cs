using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanHangWatsons.AI
{
    public class GeminiAiService
    {
        private static readonly string API_KEY =
            ConfigurationManager.AppSettings["GeminiApiKey"];

        private static readonly HttpClient _http = new HttpClient();

        // ❗ KHÔNG GẮN KEY Ở HEADER
        private const string GEMINI_URL =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public static async Task<string> AskAsync(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(API_KEY))
                return "⚠️ Chưa cấu hình Gemini API Key.";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                role = "user",
                parts = new[]
                {
                    new
                    {
                        text =
                        $@"Bạn là chatbot hỗ trợ hệ thống quản lý bán hàng Watsons.
                        Trả lời NGẮN GỌN, RÕ RÀNG, bằng TIẾNG VIỆT.
                        Nếu không biết thì nói không biết.

                        Câu hỏi:
                        {userMessage}"
                    }
                }
            }
        }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(
                $"{GEMINI_URL}?key={API_KEY}",
                content
            );

            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == (System.Net.HttpStatusCode)429)
                    return "⚠️ Gemini hiện đã hết lượt miễn phí. Vui lòng thử lại sau.";

                return "❌ Lỗi Gemini:\n" + raw;
            }

            dynamic result = JsonConvert.DeserializeObject(raw);

            return result.candidates[0].content.parts[0].text.ToString();
        }
    }
}
