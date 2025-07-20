using System;

public static class VNPayHelper
{
    public static string CreatePaymentUrl(decimal amount, string orderId, string description)
    {
        var pay = new VnPayLibrary();

        pay.AddRequestData("vnp_Version", "2.1.0");
        pay.AddRequestData("vnp_Command", "pay");
        pay.AddRequestData("vnp_TmnCode", VNPayConfig.TmnCode);
        pay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
        pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", "VND");
        pay.AddRequestData("vnp_IpAddr", "127.0.0.1");
        pay.AddRequestData("vnp_Locale", "vn");
        pay.AddRequestData("vnp_OrderInfo", description);
        pay.AddRequestData("vnp_OrderType", "other");
        pay.AddRequestData("vnp_ReturnUrl", VNPayConfig.ReturnUrl);
        pay.AddRequestData("vnp_TxnRef", orderId);

        return pay.CreateRequestUrl(VNPayConfig.BaseUrl, VNPayConfig.HashSecret);
    }
}
