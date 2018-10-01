//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Http;
//using System.Web.Mvc;
//using Microsoft.Azure; // Namespace for Azure Configuration Manager
//using Microsoft.WindowsAzure.Storage; // Namespace for Storage Client Library
//using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Azure Blobs
//using Microsoft.WindowsAzure.Storage.File; // Namespace for Azure Files

//namespace Scribs.Controllers {

//    public class Note {
//        public string id { get; set; }
//        public int index { get; set; }
//        public string text { get; set; }
//        public int status { get; set; }
//    }

//    public class Entity {
//        public int discriminator { get; set; }
//        public string id { get; set; }
//        public int index { get; set; }
//        public string key { get; set; }
//        public string name { get; set; }
//        public string text { get; set; }
//        public bool open { get; set; }
//        public string userId { get; set; }
//        public IEnumerable<Note> notes { get; set; }
//        public IEnumerable<Entity> Children { get; set; }
//    }

//    public class TestModel {
//        public bool CaMarche { get; set; }
//        public string Text { get; set; }
//        public string Content { get; set; }
//    }

//    public class HomeController : ApiController {
//        [System.Web.Http.HttpGet]
//        public TestModel Index() {
//        }
//    }
//}