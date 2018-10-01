using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriboAPI;
using ScriboAPI.Controllers;

namespace ScriboAPI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.IndexAsync() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
