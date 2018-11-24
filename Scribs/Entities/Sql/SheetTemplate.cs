using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scribs {

    [Table("SheetTemplate")]
    public partial class SheetTemplate {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(36)]
        [Index]
        public string ProjectKey { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; }

        public ICollection<SheetTemplateField> SheetTemplateFields { get; set; } = new List<SheetTemplateField>();
        public ICollection<Sheet> Sheets { get; set; } = new List<Sheet>();
    }
}