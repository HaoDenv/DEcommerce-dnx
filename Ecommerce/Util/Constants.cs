using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Util
{
    public class Constants
    {
        public class JwtConfig
        {
            public const string SecretKey = "6d3492e466f2d9be10cbe956cdd7c61d";
            public const int ExpirationInMinutes = 14400;
        }

        public class EmailKeyGuide
        {
            public const string OTP = "@OTP";
            public const string OrderCode = "@OrderCode";
            public const string OrderDate = "@OrderDate";
            public const string Customer = "@Customer";
            public const string Address = "@Address";
            public const string OrderDetail = "@OrderDetail";
            public const string NewPassword = "@NewPassword";
        }

        public class OrderStatus
        {
            //10: Chờ xác nhận
            //20: Đã xác nhận
            //30: Đang vận chuyển
            //40: Đã giao
            //50: Đã hủy
            public const int CHO_XAC_NHAN = 10;
            public const int DA_XAC_NHAN = 20;
            public const int DANG_VAN_CHUYEN = 30;
            public const int DA_GIAO = 40;
            public const int DA_HUY = 50;
        }

        public class ReviewStatus
        {
            //10: Chờ duyệt
            //20: Đã duyệt
            //30: Cập nhật lại - Chờ duyệt
            public const int CHO_DUYET = 10;
            public const int DA_DUYET = 20;
            public const int CAP_NHAT_LAI_CHO_DUYET = 30;
        }
    }
}
