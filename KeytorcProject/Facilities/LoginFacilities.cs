using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeytorcProject.Facilities
{
    public class LoginFacilities : TestBase
    {
        private LoginFacilities() : base() { }
        private static LoginFacilities instance;
        public static LoginFacilities getInstance()
        {
            if (instance == null)
            {
                instance = new LoginFacilities();
            }
            return instance;
        }
        public void loginAccount( String eMail, String Password)
        {
            helper.SearchAndFindElement(By.XPath("//a[@class='btnSignIn']")).Click();

            helper.SearchAndFindElement(By.XPath("//input[@id='email']")).Click();
            helper.SearchAndFindElement(By.XPath("//input[@id='email']")).SendKeys(eMail);
            helper.SearchAndFindElement(By.XPath("//input[@id='password']")).Click();
            helper.SearchAndFindElement(By.XPath("//input[@id='password']")).SendKeys(Password);

            helper.SearchAndFindElement(By.Id("loginButton")).Click();
            
        }
    }
}
