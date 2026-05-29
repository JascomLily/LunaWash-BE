# THÔNG TIN DỰ ÁN: SMART CAR WASH SYSTEM
> **Đề tài:** Hệ thống quản lý rửa xe tự động thông minh với đặt lịch trước và chương trình khách hàng thân thiết.
> **Môn học:** SWR302 - Software Requirements

---

## 1. GIỚI THIỆU TỔNG QUAN (INTRODUCTION)
Hệ thống quản lý rửa xe tự động thông minh là giải pháp chuyển đổi số toàn diện cho mô hình tiệm rửa xe hiện đại. Hệ thống giải quyết bài toán quá tải vào giờ cao điểm, tối ưu hóa năng suất làm việc của nhân viên, đồng thời giữ chân khách hàng thông qua cơ chế tích điểm đổi quà và cá nhân hóa trải nghiệm đặt lịch theo thời gian thực.

### Các đối tượng tương tác với hệ thống (Actors):
* **Customer (Khách hàng):** Đặt lịch, thanh toán, theo dõi trạng thái xe, tích điểm và đánh giá.
* **Staff (Nhân viên kỹ thuật):** Tiếp nhận xe, cập nhật trạng thái xử lý, chấm công.
* **Admin / Store Owner (Chủ cửa hàng):** Quản lý cấu hình hệ thống, dịch vụ, nhân sự, khuyến mãi và xem báo cáo doanh thu.
* **IoT & Hardware Subsystem:** Hệ thống camera AI nhận diện biển số xe (ANPR) kết nối tự động.

---

## 2. PHẠM VI CHỨC NĂNG HỆ THỐNG (SYSTEM SCOPE)

### 2.1. Phân hệ Khách hàng (Customer Web/App)
* **Authentication (Xác thực):** Đăng ký tài khoản (SĐT, Email), Đăng nhập mật khẩu hoặc đăng nhập nhanh qua Google.
* **Booking (Đặt lịch thông minh):**
    * Chọn Chi nhánh (Tích hợp Google Maps API để xem vị trí trực quan).
    * Chọn Ngày / Khung giờ (Hiển thị danh sách các slot còn trống theo thời gian thực).
    * Chọn Loại xe (Xe máy, Ô tô 4 chỗ, Ô tô 7 chỗ,...) & Gói dịch vụ (Rửa cơ bản, Rửa premium, Vệ sinh nội thất, Đánh bóng).
    * *Gợi ý giờ thấp điểm:* Tự động hiển thị các khung giờ ít đông kèm theo mã giảm giá để khuyến khích khách hàng đặt lịch, giúp giảm quá tải cho tiệm.
* **Booking Status (Theo dõi trạng thái đơn):** Live-tracking trạng thái xe qua các bước: `Đang chờ` -> `Đang rửa` -> `Hoàn thành`. Nhận thông báo Realtime (Notification) khi sắp tới lượt.
* **Payment (Thanh toán đa phương thức):** Hỗ trợ Tiền mặt, quét mã QR Banking (VietQR), hoặc liên kết Ví điện tử (Momo, VNPAY). Hỗ trợ mở rộng đa tiền tệ.
* **Loyalty (Khách hàng thân thiết):** Quản lý điểm thưởng tích lũy. Đổi điểm lấy Voucher/Mã giảm giá. Phân hạng thành viên (Loyalty Ranking): *Silver / Gold / VIP*.
* **Account (Quản lý tài khoản):** Xem lại lịch sử đặt lịch, hóa đơn điện tử, số dư điểm thưởng.
* **Review (Đánh giá):** Chấm điểm Rating (1-5 sao) và để lại bình luận (Comment) sau mỗi lần sử dụng dịch vụ.

### 2.2. Phân hệ Nhân viên (Staff App)
* **Quản lý luồng xe (Job Queue):** Xem danh sách hàng đợi các xe cần xử lý trong ngày.
* **Kiểm tra & Tiếp nhận (Check-in):** Thực hiện ghi nhận tình trạng xe khi vào bãi (ghi chú vết xước/hỏng hóc cũ nếu có để tránh tranh chấp), chuyển trạng thái sang `Đã Check-in`.
* **Cập nhật trạng thái:** Chuyển trạng thái xe sang `Đang rửa` hoặc `Hoàn thành`.
* **Bàn giao (Check-out):** Đối chiếu mã QR hoặc biển số xe của khách, thu tiền (nếu khách chọn thanh toán tiền mặt) và xác nhận bàn giao xe thành công.
* **Quản lý cá nhân:** Chấm công vào ca/ra ca (Check-in/Check-out ca làm việc), xem lịch phân công công việc từ Admin.

### 2.3. Phân hệ Admin / Chủ tiệm (Admin Dashboard)
* **Dashboard (Tổng quan):** Theo dõi doanh thu theo ngày/tháng, số lượt xe đã xử lý, biểu đồ giờ cao điểm và danh sách Top khách hàng thân thiết.
* **Service Management (Quản lý dịch vụ):** Thêm/sửa/xóa và cấu hình giá cho từng gói dịch vụ ứng với từng loại xe cụ thể.
* **Booking Management (Quản lý lịch đặt):** Xác nhận lịch, hủy lịch hoặc dời lịch của khách sang khung giờ khác khi xảy ra sự cố vận hành.
* **Customer & Staff Management (Quản lý nhân sự & khách hàng):**
    * Xem thông tin khách hàng, lịch sử rửa xe, thủ công điều chỉnh điểm loyalty.
    * Phân công ca làm việc, duyệt bảng chấm công cho nhân viên.
* **Quản lý khuyến mãi:** Tạo và cấu hình mã Voucher, thiết lập tự động giảm giá cấu hình theo khung giờ thấp điểm (Happy Hours).
* **Quản lý kho vật tư (Inventory):** Quản lý lượng hóa chất, dung dịch, khăn lau tồn kho. Tự động trừ định mức vật tư sau mỗi lượt rửa xe hoàn thành và cảnh báo khi sắp hết.
* **Báo cáo thống kê chuyên sâu:** Xuất báo cáo doanh thu (Revenue), số lượng xe/ngày, biểu đồ dịch vụ phổ biến và tỷ lệ khách hàng quay lại (Retention rate). *Tích hợp AI Predict peak hour* để dự đoán trước giờ đông khách dựa trên dữ liệu cũ.

---

## 3. QUY TẮC NGHIỆP VỤ & KỊCH BẢN VẬN HÀNH (BUSINESS RULES)

Hệ thống cam kết tuân thủ và xử lý các bài toán thực tế tại cửa hàng theo các quy tắc nghiêm ngặt sau:

| STT | Tình huống vận hành | Giải pháp xử lý hệ thống (Business Logic) |
| :--- | :--- | :--- |
| **1** | **Ngăn chặn trùng lịch?** | Mỗi chi nhánh cấu hình số lượng máy rửa (Cầu rửa) cố định. Hệ thống giới hạn số lượng `Slot tối đa = Số cầu rửa` trong cùng một khung giờ 30 phút. Khi số đơn đặt đạt giới hạn, hệ thống sẽ tự động khóa slot đó trên UI của khách hàng (Sử dụng cơ chế khóa tạm thời - Redis Lock khi khách đang thanh toán). |
| **2** | **Thời gian một khung giờ?** | Thời gian tiêu chuẩn là **30 phút** cho một lượt rửa cơ bản. Đối với các gói dịch vụ nặng (Vệ sinh nội thất, đánh bóng), hệ thống tự động tính toán chiếm dụng 2 hoặc 3 slot liên tiếp để tránh bị chồng lịch phía sau. |
| **3** | **Hủy hoặc đổi lịch trước bao lâu?** | Khách hàng được quyền tự do Hủy hoặc Đổi lịch trên hệ thống **trước tối thiểu 30 phút** so với giờ hẹn. Quá thời gian này, nút Hủy/Đổi lịch trên App sẽ bị khóa, khách muốn đổi phải liên hệ hotline của Admin. |
| **4** | **Xử lý khi khách đến muộn?** | Hệ thống áp dụng thời gian giữ chỗ tối đa là **15 phút** (Thời gian ân hạn). Nếu lịch hẹn là `10:00`, khách không check-in trước `10:15` thì hệ thống sẽ tự động chuyển trạng thái thành "Hủy do trễ hẹn", giải phóng slot cho khách vãng lai và trừ điểm uy tín của khách đặt. |
| **5** | **Ưu tiên Khách đặt trước vs Khách vãng lai?** | Khách đặt lịch trước luôn được ưu tiên tuyệt đối vào đúng khung giờ họ đã chọn. Khách vãng lai (Walk-in) đến tiệm sẽ được nhân viên quầy tiếp nhận và xếp vào các slot trống còn sót lại hoặc xếp vào hàng đợi chờ giữa các khoảng nghỉ của máy rửa. |
| **6** | **Sai lệch thông tin loại xe?** | Nếu khách đặt lịch chọn xe 4 chỗ (giá thấp, slot ngắn) nhưng đi xe 7 chỗ đến, nhân viên Check-in có quyền cập nhật lại loại xe trên hệ thống. Hệ thống sẽ tính lại tiền dựa trên giá thực tế và gửi thông báo yêu cầu khách thanh toán phần chênh lệch, đồng thời tự động kiểm tra xem có làm ảnh hưởng đến các slot phía sau hay không để nhân viên điều phối. |
| **7** | **Xử lý dồn toa khi quá tải đơn hàng?** | Trường hợp một xe rửa bị trễ tiến độ làm ảnh hưởng đến các xe đặt lịch slot sau, hệ thống sẽ tự động kích hoạt trạng thái "Delay dự kiến". Hệ thống gửi thông báo Notification/SMS đến các khách hàng ở 2 slot tiếp theo: *"Đội ngũ đang tối ưu chất lượng, lượt rửa của bạn sẽ bắt đầu muộn hơn 10 phút, xin lỗi vì sự bất tiện này"*. |
| **8** | **Công thức tính điểm thưởng?** | Điểm thưởng tích lũy bằng **5%** tổng giá trị hóa đơn thực tế thanh toán (Ví dụ: Rửa xe hết 100k, tích 5 điểm, tương đương 5.000 VND). Điểm này dùng để tích lũy lên hạng thành viên hoặc đổi các Voucher giảm giá trực tiếp cho lần đặt sau. |
| **9** | **Gói thành viên (Subscription)?** | Hệ thống hỗ trợ cấu hình bán các gói thành viên theo Tháng/Năm (Ví dụ: Gói thành viên Vàng trả trước 400k/tháng được rửa tối đa 5 lần cơ bản và miễn phí 1 lần vệ sinh nội thất). |
| **10** | **Quy trình Check-in tự động bằng AI?** | Tại lối vào lắp đặt Camera giám sát kết nối luồng RTSP. Khi xe tiến vào, module AI tự động quét và nhận diện biển số xe (ANPR). Nếu trùng khớp biển số có lịch đặt `Đang chờ` trong khung giờ hiện tại, hệ thống tự động đổi trạng thái sang `Đã Check-in`, mở rào chắn và thông báo cho nhân viên chuẩn bị xe. |
| **11** | **Xử lý lỗi Camera không nhận diện được?** | Trong trường hợp camera gặp sự cố (biển số mờ, thời tiết xấu), khách hàng chỉ cần xuất trình **Mã QR Check-in** trên App của mình. Nhân viên sẽ dùng App của nhân viên quét mã này để hoàn tất Check-in thủ công chỉ trong 3 giây. |
| **12** | **Xử lý khi máy rửa/hệ thống gặp sự cố?** | Khi có sự cố phần cứng đột xuất, Admin bật trạng thái "Bảo trì khẩn cấp" cho chi nhánh đó trên hệ thống. Hệ thống ngay lập tức đóng cổng đặt lịch mới, đồng thời chạy script tự động gửi thông báo xin lỗi, hoàn tiền (nếu đã thanh toán trước) và tặng kèm một Voucher giảm giá 50% cho các khách hàng bị ảnh hưởng. |

---

## 4. YÊU CẦU PHI CHỨC NĂNG (NON-FUNCTIONAL REQUIREMENTS)

* **Hiệu năng (Performance):** Hệ thống cập nhật trạng thái đơn hàng thời gian thực (Realtime) qua kết nối WebSocket/Firebase Cloud Messaging, đảm bảo độ trễ hiển thị dưới 1 giây. API tra cứu slot trống phải phản hồi dưới 200ms dưới điều kiện 500 người dùng truy cập đồng thời.
* **Bảo mật (Security):** Toàn bộ dữ liệu truyền tải phải được mã hóa qua giao thức HTTPS (TLS 1.3). Các giao dịch thanh toán qua QR hay Ví điện tử bắt buộc sử dụng chữ ký số bảo mật (Checksum SHA256) mã hóa đầu cuối.
* **Giám sát & Đối soát (Camera Logging):** Hệ thống camera giám sát khu vực rửa xe phải đảm bảo luồng truyền mượt mà, ghi hình liên tục và lưu trữ video tối thiểu 7 ngày trên Cloud/Local Storage phục vụ mục đích đối soát, làm bằng chứng bảo vệ nhân viên khi có khiếu nại làm xước hoặc hỏng xe của khách.

---

## 5. KIẾN TRÚC CÔNG NGHỆ ĐỀ XUẤT (TECHNICAL STACK)

* **Frontend:** React Native  React.js .
* **Backend:**  C# .NET Core (Đảm bảo kiến trúc vững chắc, xử lý bất đồng bộ tốt để giải quyết bài toán chống trùng lịch).
* **Database:** MySQL hoặc PostgreSQL lưu trữ dữ liệu quan hệ (Khách hàng, Lịch đặt, Hóa đơn); Redis dùng làm Cache lưu trữ trạng thái các Slot trống giúp tối ưu tốc độ đọc dữ liệu.
* **AI Module:** Thư viện OpenCV hoặc mô hình YOLOv8 được huấn luyện để nhận diện biển số xe tại Việt Nam.