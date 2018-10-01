using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scribs {

    public enum Status {
        Active,
        Expired
    }

    [Table("Access")]
    public partial class Access {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime MTime { get; set; }

        public Status Status { get; set; } = Status.Active;

        [Required]
        [StringLength(255)]
        public string Token { get; set; }

        [Required]
        [StringLength(255)]
        public string Secret { get; set; }

        [Required]
        [ForeignKey("User")]
        [Index]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
