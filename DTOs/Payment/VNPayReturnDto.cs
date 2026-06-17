namespace hotel_booking_api.DTOs.Payment
{
    public class VNPayReturnDto
    {
        public string vnp_TmnCode { get; set; } = "";        // Mã merchant
        public string vnp_Amount { get; set; } = "";         // Số tiền (đã nhân 100)
        public string vnp_BankCode { get; set; } = "";       // Ngân hàng: NCB, VCB...
        public string vnp_BankTranNo { get; set; } = "";     // Mã giao dịch bên ngân hàng
        public string vnp_CardType { get; set; } = "";       // ATM hoặc QRCODE
        public string vnp_PayDate { get; set; } = "";        // Thời gian thanh toán
        public string vnp_OrderInfo { get; set; } = "";      // Nội dung thanh toán
        public string vnp_TransactionNo { get; set; } = ""; // Mã giao dịch VNPay
        public string vnp_ResponseCode { get; set; } = "";  // "00" = thành công
        public string vnp_TransactionStatus { get; set; } = ""; // "00" = thành công
        public string vnp_TxnRef { get; set; } = "";        // BookingId bạn truyền vào
        public string vnp_SecureHash { get; set; } = "";    // Chữ ký để verify
    }
}
