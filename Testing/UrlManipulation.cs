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

        [TestMethod]
        [DataRow("https://localhost:32493", "Digests/Whatever?key=this")]
        [DataRow("https://localhost:32493/", "Digests/Whatever?key=this")]
        [DataRow("https://localhost:32493", "/Digests/Whatever?key=this")]
        [DataRow("https://localhost:32493/", "/Digests/Whatever?key=this")]
        public void PathCombine(string path1, string path2)
        {
            Assert.IsTrue(PathUtil.Combine(path1, path2).Equals("https://localhost:32493/Digests/Whatever?key=this"));
        }
    }
}