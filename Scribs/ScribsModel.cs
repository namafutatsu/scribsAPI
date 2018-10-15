using System;
using System.Data.Entity;

namespace Scribs {
    public partial class ScribsDbContext : DbContext {
        public ScribsDbContext() : base("name=ScribsModel") {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ScribsDbContext, Migrations.Configuration>());
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Access> Accesses { get; set; }
        public virtual DbSet<SheetTemplate> SheetTemplates { get; set; }
        public virtual DbSet<SheetTemplateField> SheetTemplateFields { get; set; }
        public virtual DbSet<Sheet> Sheets { get; set; }
        public virtual DbSet<SheetField> SheetFields { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }

    public static class DbUtils {
        public static DbSet<E> GetDbSet<E>(this Factory<E> factory, ScribsDbContext db) where E : Entity, new() {
            if (typeof(E) == typeof(User))
                return db.Users as DbSet<E>;
            if (typeof(E) == typeof(Access))
                return db.Accesses as DbSet<E>;
            if (typeof(E) == typeof(SheetTemplate))
                return db.SheetTemplates as DbSet<E>;
            if (typeof(E) == typeof(SheetTemplateField))
                return db.SheetTemplateFields as DbSet<E>;
            if (typeof(E) == typeof(Sheet))
                return db.Sheets as DbSet<E>;
            if (typeof(E) == typeof(SheetField))
                return db.SheetFields as DbSet<E>;
            throw new NotImplementedException();
        }
    }
}
