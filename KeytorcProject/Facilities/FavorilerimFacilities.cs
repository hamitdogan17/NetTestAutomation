using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeytorcProject.Facilities
{
    public class FavorilerimFacilities : TestBase
    {
        private FavorilerimFacilities() : base() { }
        private static FavorilerimFacilities instance;
        public static FavorilerimFacilities getInstance()
        {
            if (instance == null)
            {
                instance = new FavorilerimFacilities();
            }
            return instance;
        }
        public void CheckElementIsinFavoriteList( String favoriUrun)
        {
            Assert.IsTrue(helper.SearchAndFindElement(By.XPath("//td[@class='productTitle']/p/a")).Text.Contains(favoriUrun));
        }
        public void removeProductAndCheckInFavorilerim()
        {
            helper.SearchAndFindElement(By.XPath("//a[@class='removeSelectedProduct']")).Click();
            Assert.IsTrue(helper.SearchAndFindElement(By.XPath(("//div[@class='emptyWatchList hiddentext']"))).Text.Contains("ürün bulunmamaktadır"));
        }
    }
}
