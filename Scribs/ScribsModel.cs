using System;
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

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }

    public static class DbUtils {
        public static DbSet<E> GetDbSet<E>(this Factory<E> factory, ScribsDbContext db) where E : Entity, new() {
            if (typeof(E) == typeof(User))
                return db.Users as DbSet<E>;
            throw new NotImplementedException();
        }
    }
}
