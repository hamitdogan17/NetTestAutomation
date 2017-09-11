using KeytorcProject.Facilities;
using KeytorcProject.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeytorcProject.AllTests
{
    [TestClass]
    public class SmartModeration : TestBase
    {
        [TestMethod]
        [TestCategory("AllTests")]
        [TelcoCRMLogin(TCRMUser.CSR)]
        public void Login()
        {
            BaseCaseFacilities bcf = BaseCaseFacilities.getInstance();
            SearchPageFacilities spf = SearchPageFacilities.getInstance();
            FavorilerimFacilities ff = FavorilerimFacilities.getInstance();

            helper.Driver.Navigate().GoToUrl("https://www.smartmoderation.com/signin");
            helper.SearchAndFindElement(By.XPath("//input[@name='email']")).Click();
            helper.sendKeys("sm.demo.v3@gmail.com");
            helper.SearchAndFindElement(By.XPath("//input[@name='password']")).Click();
            helper.sendKeys("Turkgen123");
            helper.SearchAndFindElement(By.XPath("//button[text()='Sign In']")).Click();
        }
    }
}
