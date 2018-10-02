namespace Scribs {

    public partial class User {

        private Directory directory;
        public Directory Directory {
            get {
                if (directory == null) {
                    directory = FileSystem.RootDir.GetDirectory(Name);
                }
                return directory;
            }
        }

        public void CreateDirectory() {
            Directory.CreateIfNotExistsAsync();
        }
    }
}