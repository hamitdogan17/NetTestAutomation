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
    public class staginOtelLogin : TestBase
    {
        [TestMethod]
        [TestCategory("AllTests")]
        [TelcoCRMLogin(TCRMUser.CSR)]
        public void Login()
        {
            BaseCaseFacilities bcf = BaseCaseFacilities.getInstance();
            SearchPageFacilities spf = SearchPageFacilities.getInstance();
            FavorilerimFacilities ff = FavorilerimFacilities.getInstance();

            helper.SearchAndFindElement(By.XPath("//div[@class='insider-opt-in-notification-button insider-opt-in-disallow-button']")).Click();
            helper.WaitSeconds(2);
            bcf.JavascriptClick(By.XPath("//a[text()='Login']"));

            helper.SearchAndFindElement(By.XPath("//input[@placeholder='E-mail address']")).Click();
            helper.sendKeys("hur@expersio.com");
            helper.SearchAndFindElement(By.XPath("//input[@name='password']")).Click();
            helper.sendKeys("q1w2e3r4");

            helper.SearchAndFindElement(By.XPath("//button[@name='login_submit']")).Click();

            helper.SearchAndFindElement(By.XPath("//input[@id='location__input']")).Click();
            helper.sendKeys("London");
            helper.WaitSeconds(1);
            helper.sendKeys(Keys.Down);
            helper.WaitSeconds(1);
            helper.sendKeys(Keys.Down);
            helper.WaitSeconds(1);
            helper.sendKeys(Keys.Enter);

            helper.SearchAndFindElement(By.XPath("//input[@name='checkInDate']")).Click();
            helper.SearchAndFindElement(By.XPath("//div[@class='day toMonth valid' and text()='15']")).Click();
            helper.SearchAndFindElement(By.XPath("//div[@class='day toMonth valid' and text()='16']")).Click();

            helper.SearchAndFindElement(By.XPath("//button[@class='search__button']")).Click();

            helper.SearchAndFindElement(By.XPath("//a[@class='search-item-button']")).Click();

            helper.FindAndSwithNewWindow();
            helper.SearchAndFindElement(By.XPath("//button[@class='book-now-button  ']")).Click();

            helper.SearchAndFindElement(By.XPath("//input[@name='contact_name']")).Click();
            helper.sendKeys("hur sakman");
            helper.SearchAndFindElement(By.XPath("//input[@name='zipcode']")).Click();
            helper.sendKeys("34000");
            helper.SearchAndFindElement(By.XPath("//textarea[@name='address']")).Click();
            helper.sendKeys("asdf");
            try
            {
                helper.SearchAndFindElement(By.XPath("//button[@class='checkout-form__submit']")).Click();
            }
            catch { }            

            helper.SearchAndFindElement(By.XPath("//input[@name='card_holder_name']")).Click();
            helper.sendKeys("hur sakman");
            helper.SearchAndFindElement(By.XPath("//input[@id='cardNumber']")).Click();
            helper.sendKeys("4000000000000002");
            helper.SearchAndFindElement(By.XPath("//input[@name='card_cvc']")).Click();
            helper.sendKeys("000");
            helper.SearchAndFindElement(By.XPath("//input[@name='agreement']/../span")).Click();

            helper.SearchAndFindElement(By.XPath("//button[@class='checkout-form__submit  checkout-form__submit--final']")).Click();
            helper.WaitSeconds(100);
        }

     
    }
}
