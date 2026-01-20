using System.ComponentModel.DataAnnotations;


namespace ipz_marketplace.Entities

{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
