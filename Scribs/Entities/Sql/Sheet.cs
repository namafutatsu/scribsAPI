using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scribs {

    [Table("Sheet")]
    public partial class Sheet {
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
        [ForeignKey("SheetTemplate")]
        public int SheetTemplateId { get; set; }
        public SheetTemplate SheetTemplate { get; set; }

        public ICollection<SheetField> SheetFields { get; set; } = new List<SheetField>();
    }
}