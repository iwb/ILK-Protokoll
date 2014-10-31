using System.Web.Mvc;
using ILK_Protokoll.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ILK_Protokoll.Tests.Controllers
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
			ViewResult result = controller.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}