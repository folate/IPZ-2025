namespace ipz_marketplace.Entities
{
    public class Revision
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
