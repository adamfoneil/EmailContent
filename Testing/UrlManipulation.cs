using RazorToStringServices.Extensions;

namespace Testing
{
    [TestClass]
    public class UrlManipulation
    {
        [TestMethod]
        public void RemovePort()
        {
            var url = "https://practicalsn.azurewebsites.net:443/Online/whatever";
            var result = ServiceProviderExtensions.RemovePort(url, 443);
            Assert.IsTrue(result.Equals("https://practicalsn.azurewebsites.net/Online/whatever"));
        }
    }
}