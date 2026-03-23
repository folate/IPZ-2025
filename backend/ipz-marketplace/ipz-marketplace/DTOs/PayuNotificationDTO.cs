namespace ipz_marketplace.DTOs
{
    public class PayUNotificationDTO
    {
        public PayUOrderInfo Order { get; set; }
    }
    public class PayUOrderInfo
    {
        public string OrderId { get; set; }      // ID w systemie PayU
        public string ExtOrderId { get; set; }   // Twoje ID zamówienia z bazy
        public string Status { get; set; }       // np. "COMPLETED", "CANCELED"
    }
}
