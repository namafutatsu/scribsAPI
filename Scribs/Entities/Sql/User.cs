using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scribs {

    [Table("User")]
    public partial class User {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Index]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Mail { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        public ICollection<Access> Accesses { get; set; }
    }
}