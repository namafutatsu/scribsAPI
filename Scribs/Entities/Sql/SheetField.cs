using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scribs {

    [Table("SheetField")]
    public partial class SheetField {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(5000)]
        public string Value { get; set; }

        [Required]
        [ForeignKey("Sheet")]
        public int SheetId { get; set; }
        public Sheet Sheet { get; set; }

        [ForeignKey("SheetTemplateField")]
        public int? SheetTemplateFieldId { get; set; }
        public SheetTemplateField SheetTemplateField { get; set; }
    }
}