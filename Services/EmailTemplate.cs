namespace hotel_booking_api.Services
{
    public class EmailTemplate
    {
        // BASE WRAPPER
        private static string Wrap(string content)
        {
            return $@"
            <div style='
                background:#f4f6f9;
                padding:40px 0;
                font-family:Segoe UI, Arial, sans-serif;
                color:#333;'>

                <div style='
                    max-width:700px;
                    margin:auto;
                    background:white;
                    border-radius:14px;
                    overflow:hidden;
                    box-shadow:0 4px 12px rgba(0,0,0,.08);'>

                    {content}

                </div>
            </div>";
        }

        // HEADER TEMPLATE
        private static string Header(string bg, string title, string sub)
        {
            return $@"
            <div style='background:{bg};color:white;padding:32px;text-align:center;'>

                <h1 style='margin:0;font-size:28px;'>
                    {title}
                </h1>

                <p style='margin-top:12px;opacity:.9'>
                    {sub}
                </p>
            </div>";
        }

        // FOOTER
        private static string Footer()
        {
            return @"
            <div style='
                background:#f9fafb;
                border-top:1px solid #e5e7eb;
                padding:18px;
                text-align:center;
                color:#6b7280;
                font-size:13px;'>

                Đây là email tự động từ hệ thống. Vui lòng không phản hồi email này.
            </div>";
        }

        // ACCOUNT LOCKED
        public static string AccountLocked(string fullName, string reason, bool isHotelStaff = false)
        {
            var hotelNotice = isHotelStaff ? @"
            <p style='font-size:15px;line-height:1.8;'>
                Vì tài khoản đang quản lý khách sạn trên hệ thống,
                quyền truy cập và hoạt động của khách sạn liên kết cũng sẽ bị
                tạm ngừng cho đến khi trạng thái được khôi phục.
            </p>" : "";

            var body = $@"
            {Header("#b91c1c", "THÔNG BÁO KHÓA TÀI KHOẢN", "Hotel Booking Management System")}

            <div style='padding:40px 35px;'>

                <p style='font-size:16px;line-height:1.8;'>
                    Kính gửi <b>{fullName}</b>,
                </p>

                <p style='font-size:15px;line-height:1.8;'>
                    Hệ thống ghi nhận tài khoản của quý khách đã bị
                    <b>tạm khóa</b> do vi phạm chính sách
                    hoặc quy định hoạt động của hệ thống.
                </p>

                {hotelNotice}

                <div style='
                    margin:28px 0;
                    background:#fef2f2;
                    border-left:5px solid #dc2626;
                    padding:18px;
                    border-radius:8px;'>

                    <p style='margin:0;font-size:15px;'>
                        <b>Lý do:</b>
                    </p>

                    <p style='margin-top:10px;line-height:1.7;'>
                        {reason}
                    </p>
                </div>

                <p style='font-size:15px;line-height:1.8;'>
                    Nếu quý khách cho rằng đây là nhầm lẫn hoặc cần thêm hỗ trợ,
                    vui lòng liên hệ đội ngũ quản trị hệ thống để được xem xét.
                </p>

                <p style='font-size:15px;line-height:1.8;margin-top:35px;'>
                    Trân trọng,
                </p>

                <p style='font-size:15px;line-height:1.6;'>
                    <b>Administration Team</b><br/>
                    Hotel Booking Management System
                </p>
            </div>

            {Footer()}";

            return Wrap(body);
        }

        // ACCOUNT UNLOCKED
        public static string AccountUnlocked(string fullName, string reason, bool isHotelStaff = false)
        {
            var hotelNotice = isHotelStaff ? @"
            <p style='font-size:15px;line-height:1.8;'>
                Vì tài khoản đang quản lý khách sạn trên hệ thống,
                các quyền hoạt động liên quan cũng đã được khôi phục tương ứng.
            </p>" : "";

            var body = $@"
            {Header("#15803d", "THÔNG BÁO MỞ KHÓA TÀI KHOẢN", "Hotel Booking Management System")}

            <div style='padding:40px 35px;'>

                <p style='font-size:16px;line-height:1.8;'>
                    Kính gửi <b>{fullName}</b>,
                </p>

                <p style='font-size:15px;line-height:1.8;'>
                    Tài khoản của quý khách đã được
                    <b>mở khóa</b> và hiện có thể sử dụng lại bình thường.
                </p>

                {hotelNotice}

                <div style='
                    margin:28px 0;
                    background:#f0fdf4;
                    border-left:5px solid #16a34a;
                    padding:18px;
                    border-radius:8px;'>

                    <p style='margin:0;font-size:15px;'>
                        <b>Ghi chú từ quản trị viên:</b>
                    </p>

                    <p style='margin-top:10px;line-height:1.7;'>
                        {reason}
                    </p>
                </div>

                <p style='font-size:15px;line-height:1.8;'>
                    Cảm ơn quý khách đã tiếp tục đồng hành cùng hệ thống.
                </p>

                <p style='font-size:15px;line-height:1.8;margin-top:35px;'>
                    Trân trọng,
                </p>

                <p style='font-size:15px;line-height:1.6;'>
                    <b>Administration Team</b><br/>
                    Hotel Booking Management System
                </p>
            </div>

            {Footer()}";

            return Wrap(body);
        }

        // HOTEL APPROVED
        public static string HotelRegistrationApproved(string fullName, string hotelName)
        {
            var body = $@"
            {Header("#15803d", "HỒ SƠ ĐĂNG KÝ KHÁCH SẠN ĐÃ ĐƯỢC PHÊ DUYỆT", "Hotel Booking Management System")}

            <div style='padding:40px;'>

                <p style='font-size:16px;line-height:1.8'>
                    Kính gửi <strong>{fullName}</strong>,
                </p>

                <p style='font-size:15px;line-height:1.9'>
                    Chúng tôi xin trân trọng thông báo hồ sơ đăng ký khách sạn
                    của Quý đối tác đã được đội ngũ quản trị xem xét và
                    <strong>phê duyệt thành công</strong>.
                </p>

                <div style='
                    margin:28px 0;
                    background:#f0fdf4;
                    border-left:5px solid #16a34a;
                    padding:20px;
                    border-radius:8px;'>

                    <p style='margin:0'>
                        <strong>Tên khách sạn:</strong>
                    </p>

                    <p style='margin-top:10px;font-size:16px'>
                        {hotelName}
                    </p>
                </div>

                <p style='font-size:15px;line-height:1.9'>
                    Khách sạn hiện đã đủ điều kiện tham gia hệ thống và có thể
                    tiếp tục các bước thiết lập thông tin, quản lý phòng,
                    giá phòng và tiếp nhận đặt phòng từ khách hàng.
                </p>

                <p style='font-size:15px;line-height:1.9'>
                    Chúng tôi rất hân hạnh được đồng hành cùng Quý đối tác
                    trong quá trình phát triển hoạt động kinh doanh trên nền tảng.
                </p>

                <p style='font-size:15px;line-height:1.8;margin-top:35px;'>
                    Trân trọng,
                </p>

                <p style='font-size:15px;line-height:1.6;'>
                    <b>Administration Team</b><br/>
                    Hotel Booking Management System
                </p>
            </div>

            {Footer()}";

            return Wrap(body);
        }

        // HOTEL REJECTED
        public static string HotelRegistrationRejected(string fullName, string hotelName, string reason)
        {
            var body = $@"
            {Header("#b91c1c", "KẾT QUẢ THẨM ĐỊNH HỒ SƠ ĐĂNG KÝ KHÁCH SẠN", "Hotel Booking Management System")}

            <div style='padding:40px;'>

                <p style='font-size:16px;line-height:1.8'>
                    Kính gửi <strong>{fullName}</strong>,
                </p>

                <p style='font-size:15px;line-height:1.9'>
                    Sau quá trình xem xét và đánh giá hồ sơ đăng ký khách sạn,
                    chúng tôi rất tiếc phải thông báo rằng hồ sơ hiện tại
                    <strong>chưa đáp ứng yêu cầu phê duyệt</strong>.
                </p>

                <div style='
                    margin:28px 0;
                    background:#fef2f2;
                    border-left:5px solid #dc2626;
                    padding:20px;
                    border-radius:8px;'>

                    <p style='margin:0'>
                        <strong>Tên khách sạn:</strong>
                    </p>

                    <p style='margin-top:10px'>
                        {hotelName}
                    </p>

                    <hr style='margin:18px 0;border:none;border-top:1px solid #fecaca;'>

                    <p style='margin:0'>
                        <strong>Lý do từ chối:</strong>
                    </p>

                    <p style='margin-top:10px;line-height:1.8'>
                        {reason}
                    </p>
                </div>

                <p style='font-size:15px;line-height:1.9'>
                    Quý đối tác có thể rà soát, bổ sung hoặc điều chỉnh thông tin
                    theo nội dung phản hồi ở trên và gửi lại hồ sơ để được
                    xem xét trong lần tiếp theo.
                </p>

                <p style='font-size:15px;line-height:1.8;margin-top:35px;'>
                    Trân trọng,
                </p>

                <p style='font-size:15px;line-height:1.6;'>
                    <b>Administration Team</b><br/>
                    Hotel Booking Management System
                </p>
            </div>

            {Footer()}";

            return Wrap(body);
        }

        // HOTEL LOCKED / UNLOCKED
        public static string HotelLocked(string fullName, string hotelName, string reason)
        {
            var body = $@"
            {Header("#b91c1c", "THÔNG BÁO KHÓA KHÁCH SẠN", "Hotel Booking Management System")}

            <div style='padding:40px;'>
                <!-- giữ nguyên nội dung bạn đã viết -->
                <p style='font-size:16px;line-height:1.8'>
                    Kính gửi <strong>{fullName}</strong>,
                </p>

                <p style='font-size:15px;line-height:1.9'>
                    Khách sạn của Quý đối tác đã bị
                    <strong>tạm khóa hoạt động</strong> trên hệ thống bởi quản trị viên.
                </p>

                <div style='
                    margin:28px 0;
                    background:#fef2f2;
                    border-left:5px solid #dc2626;
                    padding:20px;
                    border-radius:8px;'>

                    <p><strong>Tên khách sạn:</strong> {hotelName}</p>
                    <hr style='margin:18px 0;border-top:1px solid #fecaca;'/>
                    <p><strong>Lý do khóa:</strong> {reason}</p>

                <p style='font-size:15px;line-height:1.8;margin-top:35px;'>
                    Trân trọng,
                </p>

                <p style='font-size:15px;line-height:1.6;'>
                    <b>Administration Team</b><br/>
                    Hotel Booking Management System
                </p>
                </div>
            </div>

            {Footer()}";

            return Wrap(body);
        }

        public static string HotelUnlocked(string fullName, string hotelName, string reason)
        {
            var body = $@"
            {Header("#15803d", "THÔNG BÁO MỞ KHÓA KHÁCH SẠN", "Hotel Booking Management System")}

            <div style='padding:40px;'>
                <p style='font-size:16px;line-height:1.8'>
                    Kính gửi <strong>{fullName}</strong>,
                </p>

                <p style='font-size:15px;line-height:1.9'>
                    Khách sạn của Quý đối tác đã được
                    <strong>mở khóa hoạt động</strong>.
                </p>

                <div style='
                    margin:28px 0;
                    background:#f0fdf4;
                    border-left:5px solid #16a34a;
                    padding:20px;
                    border-radius:8px;'>

                    <p><strong>Tên khách sạn:</strong> {hotelName}</p>
                    <hr style='margin:18px 0;border-top:1px solid #bbf7d0;'/>
                    <p><strong>Ghi chú:</strong> {reason}</p>

                <p style='font-size:15px;line-height:1.8;margin-top:35px;'>
                    Trân trọng,
                </p>

                <p style='font-size:15px;line-height:1.6;'>
                    <b>Administration Team</b><br/>
                    Hotel Booking Management System
                </p>
                </div>
            </div>

            {Footer()}";

            return Wrap(body);
        }

        public static string BookingAffected(string fullName, string hotelName, string bookingCode)
        {
            return $@"
            <div style='
                background:#f4f6f9;
                padding:40px 0;
                font-family:Segoe UI, Arial, sans-serif;
                color:#333;'>

                <div style='
                    max-width:700px;
                    margin:auto;
                    background:white;
                    border-radius:14px;
                    overflow:hidden;
                    box-shadow:0 4px 12px rgba(0,0,0,.08);'>

                    <!-- HEADER -->
                    <div style='
                        background:#b91c1c;
                        color:white;
                        padding:32px;
                        text-align:center;'>

                        <h1 style='margin:0;font-size:28px;'>
                            THÔNG BÁO ẢNH HƯỞNG ĐẶT PHÒNG
                        </h1>

                        <p style='margin-top:12px;opacity:.9'>
                            Hotel Booking Management System
                        </p>
                    </div>

                    <!-- BODY -->
                    <div style='padding:40px;'>

                        <p style='font-size:16px;line-height:1.8'>
                            Kính gửi <strong>{fullName}</strong>,
                        </p>

                        <p style='font-size:15px;line-height:1.9'>
                            Chúng tôi xin thông báo rằng một số đặt phòng của bạn
                            đã bị <strong>ảnh hưởng bởi trạng thái khách sạn</strong>.
                        </p>

                        <div style='
                            margin:28px 0;
                            background:#fef2f2;
                            border-left:5px solid #dc2626;
                            padding:20px;
                            border-radius:8px;'>

                            <p style='margin:0'>
                                <strong>Khách sạn:</strong> {hotelName}
                            </p>

                            <p style='margin-top:10px'>
                                <strong>Mã đặt phòng:</strong> {bookingCode}
                            </p>
                        </div>

                        <p style='font-size:15px;line-height:1.9'>
                            Đặt phòng này có thể bị tạm dừng hoặc cần được xử lý lại
                            tùy theo quyết định của quản trị viên hệ thống.
                        </p>

                        <p style='font-size:15px;line-height:1.9'>
                            Vui lòng theo dõi trạng thái đặt phòng hoặc liên hệ hỗ trợ
                            nếu cần thêm thông tin.
                        </p>

                        <p style='margin-top:35px'>
                            Trân trọng,
                        </p>

                        <p>
                            <strong>Administration Team</strong><br/>
                            Hotel Booking Management System
                        </p>
                    </div>

                    <!-- FOOTER -->
                    <div style='
                        background:#f9fafb;
                        border-top:1px solid #e5e7eb;
                        padding:18px;
                        text-align:center;
                        color:#6b7280;
                        font-size:13px;'>

                        Đây là email tự động từ hệ thống. Vui lòng không phản hồi email này.
                    </div>

                </div>
            </div>";
        }
    }
}