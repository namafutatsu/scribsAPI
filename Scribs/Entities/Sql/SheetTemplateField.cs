using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scribs {

    [Table("SheetTemplateField")]
    public partial class SheetTemplateField {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Index { get; set; }

        [Required]
        [StringLength(255)]
        public string Label { get; set; }

        [Required]
        [ForeignKey("SheetTemplate")]
        public int SheetTemplateId { get; set; }
        public SheetTemplate SheetTemplate { get; set; }

        public ICollection<SheetField> SheetFields { get; set; }
    }
}