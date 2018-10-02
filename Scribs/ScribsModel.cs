using System.Data.Entity;

namespace Scribs {
    public partial class ScribsDbContext : DbContext {
        public ScribsDbContext() : base("name=ScribsModel") {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ScribsDbContext, Migrations.Configuration>());
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Access> Accesses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
        }
    }
}
