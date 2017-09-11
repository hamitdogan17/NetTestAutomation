using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeytorcProject.Facilities
{
    public class BaseCaseFacilities : TestBase
    {
        private BaseCaseFacilities() : base() { }
        private static BaseCaseFacilities instance;
        public static BaseCaseFacilities getInstance()
        {
            if (instance == null)
            {
                instance = new BaseCaseFacilities();
            }
            return instance;
        }
        public void SearchProduct( String productName)
        {            
            helper.SearchAndFindElement(By.Id("searchData")).Click();
            helper.SearchAndFindElement(By.Id("searchData")).SendKeys(productName);
           
            helper.sendKeys(Keys.Enter);
        }
        public void gotoFavorilerim()
        {
            helper.SearchAndFindElement(By.XPath("//a[@title='Hesabım']")).Click();
            helper.SearchAndFindElement(By.XPath("//div[@class='accNav']/ul/li[5]/a")).Click();
        }        

        public void JavascriptClick(By locator)
        {
            helper.WaitSeconds(1);
            IJavaScriptExecutor js = helper.Driver as IJavaScriptExecutor;
            js.ExecuteScript("arguments[0].click();", helper.SearchAndFindElement(locator).Element);
            helper.WaitSeconds(1);
        }
    }
}
