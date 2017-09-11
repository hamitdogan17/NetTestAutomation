using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeytorcProject.Utils;

namespace KeytorcProject.Facilities
{
    public class SearchPageFacilities : TestBase
    {
        private SearchPageFacilities() : base() { }
        private static SearchPageFacilities instance;
        public static SearchPageFacilities getInstance()
        {
            if (instance == null)
            {
                instance = new SearchPageFacilities();
            }
            return instance;
        }

        public void gotoPageNumberAndCheck(string pageNumber)
        {
            string pageURL = helper.SearchAndFindElement(By.XPath("//div[@class='pagination']/a[" + pageNumber + "]")).GetAttribute("href");
            helper.Driver.Navigate().GoToUrl(pageURL);
            Assert.IsTrue(helper.SearchAndFindElement(By.Id("currentPage")).GetAttribute("value").Contains(pageNumber));
        }

        public void CheckSearchList(string productName)
        {
            IList<IWebElement> searchList = helper.SearchAndFindElements(By.XPath("//h3[@class='productName ']"));

            for (int i = 1; i <= searchList.Count; i++)
               Assert.IsTrue(helper.SearchAndFindElement(By.XPath("//h3[@class='productName ']")).Text.ToLower().Contains(productName.ToLower()));
        }
        public string getFavoriteProduct(int whichProduct)
        {
            string favoriUrun = helper.SearchAndFindElement(By.XPath("//div[@id='view']/ul/li[" + whichProduct + "]/div/div/a/h3")).Text.Replace("\n", "");
            helper.SearchAndFindElements(By.XPath("//span[@class='textImg followBtn']"))[2].Click();
                        
            return favoriUrun;
        }
    }
}
