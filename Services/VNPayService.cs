using System.Security.Cryptography; // HMACSHA512
using System.Text;                  // Encoding
namespace hotel_booking_api.Services
{
    public class VNPayService
    {
        private readonly IConfiguration _config;

        public VNPayService(IConfiguration config) => _config = config;

        public string CreatePaymentUrl(int bookingId, decimal amount, string orderInfo)
        {
            var vnpay = _config.GetSection("VNPay");
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            // Phòng hờ nếu orderInfo bị null
            if (string.IsNullOrEmpty(orderInfo)) orderInfo = $"Thanh toan don hang {bookingId}";

            var params_ = new SortedDictionary<string, string>
    {
        { "vnp_Version",    "2.1.0" },
        { "vnp_Command",    "pay" },
        { "vnp_TmnCode",    vnpay["TmnCode"] ?? "THAY_MA_TMN_CODE_CUA_BAN_VAO" }, // 👈 Không lo bị null sập app
        { "vnp_Amount",     ((long)(amount * 100)).ToString() },
        { "vnp_CurrCode",   "VND" },
        { "vnp_TxnRef",     bookingId.ToString() },
        { "vnp_OrderInfo",  orderInfo },
        { "vnp_OrderType",  "hotel" },
        { "vnp_Locale",     "vn" },
        { "vnp_ReturnUrl",  vnpay["ReturnUrl"] ?? "http://localhost:5099/api/payment/return" }, // 👈 An toàn
        { "vnp_IpAddr",     "127.0.0.1" },
        { "vnp_CreateDate", now.ToString("yyyyMMddHHmmss") },
        { "vnp_ExpireDate", now.AddMinutes(15).ToString("yyyyMMddHHmmss") }
    };

            // 🌟 KHẮC PHỤC TRIỆT ĐỂ: Thêm (kv.Value ?? "") để chặn đứng lỗi ArgumentNullException
            var queryString = string.Join("&", params_
                .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value ?? "")}"));

            var hashSecret = vnpay["HashSecret"] ?? "THAY_HASH_SECRET_CUA_BAN_VAO";
            var signature = HmacSHA512(hashSecret, queryString);

            // Lấy link gốc VnPay sandbox, nếu cấu hình null thì tự điền link mặc định luôn
            var baseUrl = vnpay["BaseUrl"] ?? vnpay["Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";

            return $"{baseUrl}?{queryString}&vnp_SecureHash={signature}";
        }

        public bool VerifySignature(IQueryCollection query)
        {
            var hashSecret = _config["VNPay:HashSecret"]!;
            var vnpSecureHash = query["vnp_SecureHash"].ToString();

            // Lấy tất cả params trừ vnp_SecureHash
            var params_ = new SortedDictionary<string, string>();
            foreach (var key in query.Keys)
            {
                if (key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                    params_[key] = query[key]!;
            }

            var queryString = string.Join("&", params_
                .Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

            var checkSignature = HmacSHA512(hashSecret, queryString);
            return checkSignature == vnpSecureHash;
        }

        private string HmacSHA512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

}

