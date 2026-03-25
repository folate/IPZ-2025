using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.Entities
{
    public class RevisionFile
    {
        public int Id { get; set; }
        
        [Required]
        public string FileName { get; set; } // Original name
        
        [Required]
        public string StoredName { get; set; } // GUID based name to avoid collisions
        
        [Required]
        public string FilePath { get; set; }
        
        public int RevisionId { get; set; }
        public Revision Revision { get; set; }
    }
}
