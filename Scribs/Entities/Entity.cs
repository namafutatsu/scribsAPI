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
        public E CreateInstance(ScribsDbContext db) {
            return new E { db = db };
        }

        public E GetInstance(ScribsDbContext db, int id) {
            var set = this.GetDbSet<E>(db);
            var entity = set.Find(id);
            entity.db = db;
            return entity;
        }
    }
}