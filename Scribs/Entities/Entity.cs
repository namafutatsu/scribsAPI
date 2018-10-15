using System.Data.Entity;
using System.Threading.Tasks;

namespace Scribs {

    public abstract class Entity {
        public ScribsDbContext db;
    }

    public class Entity<E> : Entity where E : Entity, new() {
        private static Factory<E> factory;
        public static Factory<E> Factory {
            get {
                if (factory == null)
                    factory = new Factory<E>();
                return factory;
            }
        }
    }

    public class Factory<E> where E : Entity, new() {
        DbSet<E> GetSet(ScribsDbContext db) => this.GetDbSet<E>(db);

        public E CreateInstance(ScribsDbContext db) {
            var entity = new E { db = db };
            GetSet(db).Add(entity);
            return entity;
        }

        public E GetInstance(ScribsDbContext db, int id) {
            var set = GetSet(db);
            var entity = set.Find(id);
            entity.db = db;
            return entity;
        }

        public async Task<E> GetInstanceAsync(ScribsDbContext db, int id) {
            var set = GetSet(db);
            var entity = await set.FindAsync(id);
            entity.db = db;
            return entity;
        }

        public void Delete(ScribsDbContext db, E entity) {
            var set = GetSet(db);
            set.Remove(entity);
        }
    }
}