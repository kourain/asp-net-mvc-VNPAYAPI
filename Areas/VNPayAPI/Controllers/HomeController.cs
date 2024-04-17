using Microsoft.AspNetCore.Mvc;
using System.Web;
using VNPAYAPI.Areas.VNPayAPI.Util;

namespace VNPAYAPI.Areas.VNPayAPI.Controllers
{
    [Area("VNPayAPI")]
    public class HomeController : Controller
    {
        public string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public string returnUrl = $"https://localhost:{"để cổng của bạn vào đây"}/vnpayAPI/PaymentConfirm";
        public string tmnCode = "";
        public string hashSecret = "";
        public ActionResult Index()
        {
            return View();
        }
        [Route("/VNPayAPI/{amount}&{infor}&{orderinfor}")]
        public ActionResult Payment(string amount,string infor,string orderinfor)
        {
            string hostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(hostName).GetValue(0).ToString();
            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", amount); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", clientIPAddress); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", infor); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", orderinfor); //mã hóa đơn
                                                                           
            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return Redirect(paymentUrl);
        }
        [Route("/VNpayAPI/paymentconfirm")]
        public IActionResult PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                //lấy toàn bộ dữ liệu trả về
                var queryString = Request.QueryString.Value;
                var json = HttpUtility.ParseQueryString(queryString);

                long orderId = Convert.ToInt64(json["vnp_TxnRef"]); //mã hóa đơn
                string orderInfor = json["vnp_OrderInfo"].ToString(); //Thông tin giao dịch
                long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = json["vnp_ResponseCode"].ToString(); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = json["vnp_SecureHash"].ToString(); //hash của dữ liệu trả về
                var pos = Request.QueryString.Value.IndexOf("&vnp_SecureHash");

                //return Ok(Request.QueryString.Value.Substring(1, pos-1) + "\n" + vnp_SecureHash + "\n"+ PayLib.HmacSHA512(hashSecret, Request.QueryString.Value.Substring(1, pos-1)));
                bool checkSignature = ValidateSignature(Request.QueryString.Value.Substring(1, pos - 1), vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?
                if (checkSignature && tmnCode == json["vnp_TmnCode"].ToString())
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                       return Redirect("LINK");
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        return Redirect("LINK");
                    }
                }
                else
                {
                    //phản hồi không khớp với chữ ký
                    return Redirect("đường dẫn nếu phản hồi ko hợp lệ");
                }
            }
            //phản hồi không hợp lệ
            return Redirect("LINK");

        }
        public bool ValidateSignature(string rspraw,string inputHash, string secretKey)
        {
            string myChecksum = PayLib.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}