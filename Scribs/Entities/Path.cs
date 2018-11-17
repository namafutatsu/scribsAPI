//using System;
//using System.Linq;

//namespace Scribs {

//    public class Path : Entity<Path> {

//        public Path(string url) {

//        }

//        private string url;
//        public string Url {
//            get {
//                if (url == null)
//                    throw new Exception("Path not declared");
//                return url;
//            }
//            set {
//                url = value;
//                segments = value.Split('/').Skip(value.StartsWith("/") ? 1 : 0).ToArray();
//            }
//        }
//        private string[] segments;
//        public string Share => segments.First();
//        public string UserName => segments[1];
//        private User user;
//        public User User {
//            get {
//                if (user == null) {
//                    user = db.Users.SingleOrDefault(o => o.Name == UserName);
//                    user.db = db;
//                }
//                return user;
//            }
//        }
//        public string Last => segments.Last();
//        public string Relative => segments.Skip(2).Aggregate((a, b) => a + "/" + b);
//        public Path Parent => new Path(db, segments.Take(segments.Length - 1).Aggregate((a, b) => a + "/" + b));
//        public override string ToString() {
//            return url;
//        }
//    }
//}