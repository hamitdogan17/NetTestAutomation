using KeytorcProject.Facilities;
using KeytorcProject.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace KeytorcProject.AllTests
{
    [TestClass]
    public class N11TestScenarios : TestBase
    {
        [TestMethod]
        [TestCategory("AllTests")]
        [TelcoCRMLogin(TCRMUser.CSR)]
        public void TestMethod1()
        { 
            LoginFacilities lf = LoginFacilities.getInstance();
            BaseCaseFacilities bcf = BaseCaseFacilities.getInstance();
            SearchPageFacilities spf = SearchPageFacilities.getInstance();
            FavorilerimFacilities ff = FavorilerimFacilities.getInstance();
            
            Assert.IsTrue(helper.SearchAndFindElement(By.XPath("//span[text()='alışverişin uğurlu adresi']")).Displayed);

            // Login Your Account
            lf.loginAccount("hamitdogan17@gmail.com", "1a2b3c4d5e");
            bcf.SearchProduct("samsung");
            spf.CheckSearchList("samsung");
            spf.gotoPageNumberAndCheck("2");
            
            string favoriUrun = spf.getFavoriteProduct(3);
            bcf.gotoFavorilerim(); 
            ff.CheckElementIsinFavoriteList(favoriUrun);
            ff.removeProductAndCheckInFavorilerim();
        }
    }
}
