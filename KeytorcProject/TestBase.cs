using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Text;
using System.Threading;
using OpenQA.Selenium.Interactions;
using KeytorcProject.Utils;
using KeytorcProject.DataSources;
using KeytorcProject.Facilities;
//using Microsoft.TeamFoundation.TestManagement.Client;
using System.Configuration;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.Authentication;
using System.Management;
using System.Reflection;
using System.Collections.Generic;
using OpenQA.Selenium.Firefox;
using System.Diagnostics.Tracing;
using OpenQA.Selenium.Remote;

namespace KeytorcProject
{
    [TestClass]
    public abstract class TestBase
    {
        //protected IWebDriver driver;
        public static DriverHelper helper
        {
            get
            {
                return DriverHelper.GetInstance(loginAttr);
            }
        }

        protected TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
        protected int TestCaseID = 0;
        protected List<int> TestCaseIDList;
        protected string TestCaseOwner;
        private DateTime TestStartTime;
        private DateTime TestEndTime;
        private string screenShotFileName;
        private string htmlSourceFileName;
        private static TelcoCRMLoginAttribute loginAttr;
        

        private void TakeScreenShotOnFail()
        {
            if (TFSHelper.GetAppSetting("TakeScreenShotOnFail") == "1")
            {
                if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
                {
                    this.screenShotFileName = "C:\\TestAutomationParameterFiles\\ScreenShots\\";
                    this.screenShotFileName = this.screenShotFileName + Guid.NewGuid().ToString() + ".jpg";
                    helper.TakeScreenShot(this.screenShotFileName);
                    GetHtmlSourceOnFail();
                }
                else
                {
                    this.screenShotFileName = "";
                }
            }
        }
        [TestInitialize]
        public void SetupTest()
        {
            this.TestStartTime = DateTime.Now;
            this.screenShotFileName = "";
            this.htmlSourceFileName = "";

            helper.TearDownDriver();
           // initComponents();
            startUpTest();
        }
        [TestCleanup]
        public void TeardownTest()
        {
            //endTest();
        }
        private void GetHtmlSourceOnFail()
        {
            this.htmlSourceFileName = "C:\\TestAutomationParameterFiles\\PageSources\\";
            this.htmlSourceFileName = this.htmlSourceFileName + Guid.NewGuid().ToString() + ".txt";
            helper.GetHtmlSource(this.htmlSourceFileName);
        }
               
        protected void startUpTest()
        {
            MethodBase method = GetType().GetMethod(this.TestContext.TestName);
            object[] attrs = method.GetCustomAttributes(typeof(TelcoCRMLoginAttribute), true);
            if (attrs.Length > 0)
                loginAttr = (TelcoCRMLoginAttribute)attrs[0];

            if (loginAttr.UserName != TCRMUser.NoLogin)
            {
                helper.WindowLogin(loginAttr);
                helper.NavigateToBase();
                helper.NoThanks();
            }
        }
        protected void endTest()
        {
            TelcoCRMLoginAttribute attr = null;
            MethodBase method = GetType().GetMethod(this.TestContext.TestName);
            object[] attrs = method.GetCustomAttributes(typeof(TelcoCRMLoginAttribute), true);
            if (attrs.Length > 0)
                attr = (TelcoCRMLoginAttribute)attrs[0];

            this.TestEndTime = DateTime.Now;

            if (attr.UserName != TCRMUser.NoLogin)
            {
                TakeScreenShotOnFail();
                //WriteToTFS();
                helper.TearDownDriver();
               // helper.ResetNewUserLogin();
                if (this.screenShotFileName.Length > 0)
                    Assert.Fail("Screen Shot File: " + this.screenShotFileName + " \n" + "  Html Page Source File: " + this.htmlSourceFileName);
            }
        }

    }

    public enum TCRMUser { CSR, PrivilegedCSR, ProductManager, Admin, NoLogin };

}
