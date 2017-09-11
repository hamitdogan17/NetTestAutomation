using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;
//using System.Data.OleDb;
using System.IO;
//using System.Data;
//using System.Xml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using System.Drawing;
using KeytorcProject.DataSources;
using OpenQA.Selenium.Firefox;
using System.Security.Principal;
using System.Management;
using System.Security.Authentication;
using System.Reflection;
using OpenQA.Selenium.Remote;
using System.Drawing.Imaging;
using KeytorcProject.Facilities;
using KeytorcProject.Utils;
using OpenQA.Selenium.Chrome;


//using SeleniumWebDriverExtensions;

namespace KeytorcProject.Utils
{
    public class EricssonWebElement : IWebElement
    {
        By by;
        IWebElement element;
        public IWebElement Element
        {
            get
            {
                return element;
            }
        }
        public EricssonWebElement(IWebElement element, By by)
        {
            this.element = element;
            this.by = by;
        }
        public string TagName
        {
            get
            {
                return this.element.TagName;
            }
        }
        public string Text
        {
            get
            {
                return this.element.Text;
            }
        }
        public bool Enabled
        {
            get
            {
                return this.element.Enabled;
            }
        }
        public bool Selected
        {
            get
            {
                return this.element.Selected;
            }
        }
        public Point Location
        {
            get
            {
                return this.element.Location;
            }
        }
        public Size Size
        {
            get
            {
                return this.element.Size;
            }
        }
        public bool Displayed
        {
            get
            {
                return this.element.Displayed;
            }
        }
        public IWebElement FindElement(By by)
        {
            return this.element.FindElement(by);
        }
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return this.element.FindElements(by);
        }
        public string GetAttribute(string attributeName)
        {
            return this.element.GetAttribute(attributeName);
        }
        public string GetCssValue(string propertyName)
        {
            return this.GetCssValue(propertyName);
        }
        public void SendKeys(string text)
        {
            this.element.SendKeys(text);
        }
        public void Submit()
        {
            this.element.Submit();
        }
        public void Clear()
        {
            this.element.Clear();
        }
        public void Click()
        {
            try
            {
                this.element.Click();
            }
            catch (Exception ex)
            {
                try
                {
                    IJavaScriptExecutor js = DriverHelper.GetInstance().Driver as IJavaScriptExecutor;
                    js.ExecuteScript("arguments[0].scrollIntoView(true);", this.element);
                    js.ExecuteScript("arguments[0].click();", this.element);

                    DriverHelper.GetInstance().Driver.SwitchTo().DefaultContent();
                    IWebDriver dr = DriverHelper.GetInstance().Driver.SwitchTo().Frame("contentIFrame1");
                    dr.FindElement(by).Click();
                }
                catch (Exception ex2)
                {
                    try
                    {
                        DriverHelper.GetInstance().Driver.SwitchTo().DefaultContent();
                        IWebDriver dr = DriverHelper.GetInstance().Driver.SwitchTo().Frame("contentIFrame1");
                        dr.FindElement(by).Click();
                    }
                    catch
                    {
                        //throw new Exception("Unable to click element... ");
                    }

                }
            }
        }
        public void ClickForNavigate()
        {
            try
            {
                this.element.Click();
            }
            catch (Exception ex)
            {
                try
                {
                    DriverHelper.GetInstance().WaitSeconds(1);
                    this.element.Click();
                }
                catch
                {
                    IJavaScriptExecutor js = DriverHelper.GetInstance().Driver as IJavaScriptExecutor;
                    js.ExecuteScript("arguments[0].click();", this.element);
                }
            }

        }
    }
    
    public class DriverHelper
    {
        //private static RemoteWebDriver driver2;

        private static IWebDriver driver;
        public IWebDriver Driver
        {
            get
            {
                if (driver == null)
                    InitDriver();
                return driver;
            }
        }

        private static WebDriverWait wait; // refactor
        private static Stack windowStack; // refactor

        private static int loadTimeOut;
        private static int elementFindTimeOut;
        private static int alertFindTimeOut;
        private static string environment;
        private static string baseURL;
        private static TelcoCRMLoginAttribute loginAttr;
        private static string webDriver;

        public string Environment
        {
            get { return environment; }
        }
        public string WebDriver
        {
            get { return webDriver; }
        }
        private static DriverHelper instance;
        private DriverHelper()
        {

        }
        public static DriverHelper GetInstance()
        {
            if (instance == null)
            {
                InitDriverHelper();
            }
            return instance;
        }
        public static DriverHelper GetInstance(TelcoCRMLoginAttribute loginAttribute) // for firefox login issue.
        {
            loginAttr = loginAttribute;

            if (instance == null)
            {
                InitDriverHelper();
            }
            return instance;
        }

        private static void InitDriverHelper()
        {
            InitEnvironment();
            instance = new DriverHelper();
            elementFindTimeOut = Convert.ToInt32(TFSHelper.GetAppSetting("ElementFindTimeOut"));
            loadTimeOut = Convert.ToInt32(TFSHelper.GetAppSetting("LoadTimeOut"));
            alertFindTimeOut = Convert.ToInt32(TFSHelper.GetAppSetting("AlertFindTimeOut"));
        }
        private static void InitEnvironment()
        {
            environment = TFSHelper.GetAppSetting("Environment");
            webDriver = TFSHelper.GetAppSetting("WebDriver");
            List<string> supportedEnvironments = new List<string> { "LSV", "MasterDev", "Main" };
            if (!supportedEnvironments.Contains(environment))
                throw new Exception("Given environment in app.config is not supported. Please contact your sys admin :))) Given Environment: " + environment);
        }
        public void InitDriver()
        {
            if (TFSHelper.GetAppSetting("WebDriver") == "InternetExplorer")
            {
                if (TFSHelper.GetAppSetting("ExecuteRemotely") == "0")
                {
                    InternetExplorerDriverService service = InternetExplorerDriverService.CreateDefaultService(@"C:\\TestAutomationParameterFiles", "IEDriverServer.exe");
                    service.HideCommandPromptWindow = true;

                    InternetExplorerOptions options = new InternetExplorerOptions();
                    options.UnexpectedAlertBehavior = InternetExplorerUnexpectedAlertBehavior.Accept;
                   // driver = new EricssonInternetExplorerDriver(service, options, TimeSpan.FromSeconds(loadTimeOut));
                }
            }
            else if (TFSHelper.GetAppSetting("WebDriver") == "Firefox")
            {
                if (TFSHelper.GetAppSetting("ExecuteRemotely") == "0")
                {
                    string profile = environment + loginAttr.UserName.ToString();
                    FirefoxProfile fp = new FirefoxProfile("C:\\TestAutomationParameterFiles\\Firefox\\" + profile);
                    //FirefoxProfile fp =  new FirefoxProfile();
                    //fp.SetPreference("network.http.phishy-userpass-length", 255);
                    //fp.SetPreference("network.automatic-ntlm-auth.trusted-uris", "crmlsv");
                    driver = new FirefoxDriver(fp);
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(0));
                }
            }
            else if (TFSHelper.GetAppSetting("WebDriver") == "Chrome")
            {
                if (TFSHelper.GetAppSetting("ExecuteRemotely") == "0")
                {
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.LeaveBrowserRunning = false;
                    chromeOptions.AddArgument("--disable-extensions");
                    // driver = new ChromeDriver(System.IO.Directory.GetCurrentDirectory(), chromeOptions);
                    driver = new ChromeDriver(@"C:\TestAutomationParameterFiles", chromeOptions);
                    driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(60));
                    driver.Manage().Cookies.DeleteAllCookies();
                    Thread.Sleep(2000);
                }
            }

            elementFindTimeOut = Convert.ToInt32(TFSHelper.GetAppSetting("ElementFindTimeOut"));
            loadTimeOut = Convert.ToInt32(TFSHelper.GetAppSetting("LoadTimeOut"));
            windowStack = new Stack();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(loadTimeOut));
            driver.Manage().Window.Maximize();
        }
        public void TearDownDriver()
        {
            killPreviousInstances();
            driver = null;
            wait = null;
            windowStack = null;
            instance = null;
        }
        public void ClearCookies()
        {
            try
            {
                if (driver != null)
                    driver.Manage().Cookies.DeleteAllCookies();
            }
            catch
            { }
        }
        private void CloseAllWindows()
        {
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    ReadOnlyCollection<string> lst = driver.WindowHandles;
                    foreach (string window in lst)
                    {
                        driver.SwitchTo().Window(window).Close();
                    }
                }
            }
            catch { }
        }
        private void killPreviousInstances()
        {
            if (TFSHelper.GetAppSetting("ExecuteRemotely") == "1")
            {
                if (driver != null)
                {
                    CloseAllWindows();
                    driver.Quit();
                    driver.Dispose();
                    WaitSeconds(5);
                }
            }
            else if (TFSHelper.GetAppSetting("ExecuteRemotely") == "0")
            {
                if (TFSHelper.GetAppSetting("WebDriver") == "InternetExplorer")
                {
                    KillProcessByName("iexplore.exe", true);
                    KillProcessByName("iexplore32.exe", true);
                    KillProcessByName("IEDriverServer.exe", true);
                    KillProcessByName("WerFault.exe", true);
                }
                else if (TFSHelper.GetAppSetting("WebDriver") == "Firefox")
                {
                    KillProcessByName("firefox.exe", true);
                }
                else if (TFSHelper.GetAppSetting("WebDriver") == "Chrome")
                {
                    KillProcessByName("chromedriver.exe", true);
                    KillProcessByName("chrome.exe", true);
                }
            }
        }
        public static void KillProcessByName(string processName, bool currentUserOnly = true)
        {
            try
            {
                string userName = null;
                if (currentUserOnly)
                {
                    WindowsIdentity user = WindowsIdentity.GetCurrent();
                    if (user == null)
                        throw new InvalidCredentialException("No current user?!");
                    userName = user.Name;
                }
                var processFinder = new ManagementObjectSearcher(string.Format("Select * from Win32_Process where Name='{0}'", processName));
                var processes = processFinder.Get();
                if (processes.Count == 0)
                    return;
                foreach (ManagementObject managementObject in processes)
                {
                    var pId = Convert.ToInt32(managementObject["ProcessId"]);
                    var process = Process.GetProcessById(pId);
                    if (currentUserOnly) //current user
                    {
                        var processOwnerInfo = new object[2];
                        managementObject.InvokeMethod("GetOwner", processOwnerInfo);
                        var processOwner = (string)processOwnerInfo[0];
                        var net = (string)processOwnerInfo[1];
                        if (!string.IsNullOrEmpty(net))
                            processOwner = string.Format("{0}\\{1}", net, processOwner);
                        if (string.CompareOrdinal(processOwner, userName) == 0)
                            process.Kill();
                    }
                    else //any user                    
                        process.Kill();
                }
            }
            catch (Exception ex)
            {
                //There is a good chance for UnauthorizedAccessException here, so
                //log the error or handle otherwise
            }
        }

        public void NavigateToBase()
        {
            if (TFSHelper.GetAppSetting("WebDriver") != "Firefox")
            {
                Driver.Navigate().GoToUrl(baseURL);
                Driver.Navigate().GoToUrl(TFSHelper.GetAppSetting("WebSiteUrl"));
                if (TFSHelper.GetAppSetting("WebDriver") == "Chrome")
                {
                    sendKeys(Keys.F5);
                }
            }
            else
            {
                Driver.Navigate().GoToUrl(TFSHelper.GetAppSetting("WebSiteUrl"));
                sendKeys(Keys.F5);
            }
        }
        public void WindowLogin(TelcoCRMLoginAttribute attr)
        {
            string webDriver = TFSHelper.GetAppSetting("WebDriver");             
            baseURL = TFSHelper.GetAppSetting("WebSiteUrl");
        }

        #region Window Functions
        public void FindNewWindow(int waitingTimeForWindow)
        {
            wait.Until(d => d.WindowHandles.Count > windowStack.Count);
            string foundWindow = string.Empty;
            DateTime endTime = DateTime.Now.Add(TimeSpan.FromSeconds(waitingTimeForWindow));
            while (string.IsNullOrEmpty(foundWindow) && DateTime.Now < endTime)
            {
                foreach (string currentHandle in Driver.WindowHandles)
                {
                    if (!windowStack.Contains(currentHandle))
                    {
                        foundWindow = currentHandle;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(foundWindow))
                {
                    WaitSeconds(2);
                }
            }

            windowStack.Push(foundWindow);
        }
        public void FindAndSwithNewWindow()
        {
            FindNewWindow(20);
            Driver.SwitchTo().Window(windowStack.Peek().ToString());
        }
        public void selectNewWindow()
        {
            Driver.SwitchTo().Window(Driver.WindowHandles.ToString());
        }
        public void SwithPeekWindow()
        {
            Driver.SwitchTo().Window(windowStack.Peek().ToString());
        }
        public void SwitchToPrevWindow()
        {
            WaitSeconds(2);
            windowStack.Pop();
            Driver.SwitchTo().Window(windowStack.Peek().ToString());
        }
        public void SwitchToPrevWindowAndClose()
        {
            WaitSeconds(4);
            driver.Close();
            //WaitSeconds(2);
            //System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            WaitSeconds(2);
            windowStack.Pop();
            Driver.SwitchTo().Window(windowStack.Peek().ToString());
        }
        public EricssonWebElement SearchAndFindElement(By id)
        {
            this.WaitForDOMLoad();
            IWebElement element = null;
            Stopwatch timeOutWatch = Stopwatch.StartNew();

            while (timeOutWatch.ElapsedMilliseconds < (elementFindTimeOut * 1000))
            {
                try
                {
                    try
                    {
                        if (webDriver == "Chrome" || webDriver == "Firefox")
                        {
                            WaitSeconds(0.8);
                            IAlert alert = driver.SwitchTo().Alert();
                            alert.Accept();
                        }
                    }
                    catch { }

                    element = Driver.FindElement(id);
                    break;
                }
                catch (Exception e1)
                {
                    IWebDriver component;

                    if (windowStack.Count > 0)
                    {
                        component = Driver.SwitchTo().Window(windowStack.Peek().ToString());
                    }
                    else
                    {
                        component = Driver.SwitchTo().DefaultContent();
                    }

                    if ((element = SearchAndFindElement(component, new Stack(), id)) != null)
                    {
                        break;
                    }
                }
            }

            timeOutWatch.Stop();
            if (element == null)
                throw new Exception("Timed out after " + elementFindTimeOut.ToString() + " seconds. Following component cannot be found = " + id.ToString());

            //if (TFSHelper.GetAppSetting("WebDriver") == "Chrome")
            //{
            //    CommonFacilities.getInstance().JavaScriptScrollIntoElement(id);
            //}

            //return element;
            return new EricssonWebElement(element, id);
        }
        private IWebElement SearchAndFindElement(ISearchContext component, Stack frameStack, By id)
        {
            try
            {
                try
                {
                    if (webDriver == "Chrome")
                    {
                        IAlert alert = driver.SwitchTo().Alert();
                        alert.Accept();
                    }                    
                }
                catch { }


                IWebElement control = component.FindElement(id);
                return control;
            }
            catch
            {
                try
                {
                    //find iframes
                    ReadOnlyCollection<IWebElement> elements = component.FindElements(By.TagName("iframe"));
                    List<string> frameIdList = new List<string>();

                    foreach (IWebElement targetIFrame in elements)
                    {
                        // this is for remote run.. 
                        if (targetIFrame.GetAttribute("id") != "")
                            frameIdList.Add(targetIFrame.GetAttribute("id"));
                    }
                    foreach (string name in frameIdList)
                    {
                        //switch to previous frames
                        Driver.SwitchTo().DefaultContent();
                        foreach (string frameHistoryElement in frameStack)
                        {
                            Driver.SwitchTo().Frame(frameHistoryElement);
                        }
                        //switch to latest frame
                        IWebDriver targetIFrame = null;
                        if (name != "")
                            targetIFrame = Driver.SwitchTo().Frame(name);
                        //push to stack
                        frameStack.Push(name);
                        IWebElement searchedElement = SearchAndFindElement(targetIFrame, frameStack, id);
                        //remove from stack
                        frameStack.Pop();
                        if (searchedElement != null)
                            return searchedElement;
                    }
                }
                catch (OpenQA.Selenium.WebDriverException e)
                {

                }
            }
            return null;
        }
        public EricssonWebElement SearchAndFindElementWithOutWait(By id, bool waitForDOMLoad = true)
        {
            this.WaitForDOMLoad();
            try
            {
                try
                {
                    if (webDriver == "Chrome")
                    {
                        //WaitSeconds(2);
                        IAlert alert = driver.SwitchTo().Alert();
                        alert.Accept();
                    }
                }
                catch { }
                //return Driver.FindElement(id);
                return new EricssonWebElement(Driver.FindElement(id), id);
            }
            catch
            {
                IWebDriver component;
                IWebElement element;

                if (windowStack.Count > 0)
                {
                    component = Driver.SwitchTo().Window(windowStack.Peek().ToString());
                }
                else
                {
                    component = Driver.SwitchTo().DefaultContent();
                }


                if ((element = SearchAndFindElement(component, new Stack(), id)) == null)
                    throw new Exception("Following component cannot be found = " + id.ToString());

                //return element;
                return new EricssonWebElement(element, id);
            }
        }
        public IWebElement GetParentElementWithBy(By id)
        {
            return this.SearchAndFindElement(id).FindElement(By.XPath(".."));
        }
        public IWebElement GetParent(IWebElement element)
        {
            return element.FindElement(By.XPath(".."));
        }
        public IAlert SearchAndFindAlert()
        {
            IAlert alert = null;
            Stopwatch timeOutWatch = Stopwatch.StartNew();
            while (timeOutWatch.ElapsedMilliseconds < (alertFindTimeOut * 1000))
            {
                try
                {
                    alert = driver.SwitchTo().Alert();
                    break;
                }
                catch
                { }
            }
            timeOutWatch.Stop();
            if (alert == null)
                throw new Exception("No Alert is displayed...");

            return alert;
        }
        public ReadOnlyCollection<IWebElement> SearchAndFindElements(By id)
        {
            this.WaitForDOMLoad();
            ReadOnlyCollection<IWebElement> elements = null;
            Stopwatch timeOutWatch = Stopwatch.StartNew();

            try
            {
                if (TFSHelper.GetAppSetting("WebDriver") == "Chrome")
                {
                    WaitSeconds(2);
                }

                elements = Driver.FindElements(id);
                return elements;
            }
            catch
            {
                IWebDriver component;

                if (windowStack.Count > 0)
                {
                    component = Driver.SwitchTo().Window(windowStack.Peek().ToString());
                }
                else
                {
                    component = Driver.SwitchTo().DefaultContent();
                }

                while (timeOutWatch.ElapsedMilliseconds < (elementFindTimeOut * 1000))
                {
                    if ((elements = Driver.FindElements(id)) != null)
                    {
                        break;
                    }
                }

                timeOutWatch.Stop();

                if (elements == null)
                    throw new Exception("Timed out after " + elementFindTimeOut.ToString() + " seconds. Following component cannot be found = " + id.ToString());
                WaitSeconds(0.3);

                return elements;
            }
        }
        private void WaitForDOMLoad()
        {
            Stopwatch timeOutWatch = Stopwatch.StartNew();

            while (timeOutWatch.ElapsedMilliseconds < (elementFindTimeOut * 1000))
            {
                try
                {
                    IWait<IWebDriver> wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(loadTimeOut));
                    wait.Until(driver1 => ((IJavaScriptExecutor)Driver).ExecuteScript("return document.readyState").Equals("complete"));
                    break;
                }
                catch
                {
                    if (WebDriver == "Firefox")
                        break;
                }
            }
        }
        public void WaitSeconds(double second)
        {
            if (second > 0)
                Thread.Sleep(TimeSpan.FromSeconds(second));
        }

        char c = '\u0001';
        public void writeText(By id, string value)
        {
            SearchAndFindElement(id);
            WaitSeconds(1);
            SearchAndFindElement(id).Click();
            WaitSeconds(0.3);
            sendKeys(c.ToString());
            sendKeys(Keys.Backspace);
            sendKeys(value);
        }
        public void writeText(By id, string value, int time, bool clearText)
        {
            SearchAndFindElement(id).Click();
            if (clearText)
            {
                sendKeys(c.ToString());
                sendKeys(Keys.Backspace);
            }
            WaitSeconds(0.3);
            sendKeys(value);
        }
        public void clickAndSelectText(By id, string value)
        {
            SearchAndFindElement(id).Click();
            SearchAndFindElement(By.CssSelector("option[title=\"" + value + "\"]")).Click();
        }
        public void clickWithOffset(By id, int offsetX, int offsetY, bool relativeToTopRight)
        {
            IWebElement ele = SearchAndFindElement(id);
            if (relativeToTopRight) offsetX = ele.Size.Width - offsetX;
            Actions act = new Actions(Driver);
            act.MoveToElement(ele, offsetX, offsetY).Click().Build().Perform();
        }
        public void clickWithOffset(IWebElement element, int offsetX, int offsetY, bool relativeToTopRight)
        {
            IWebElement ele = element;
            if (relativeToTopRight) offsetX = ele.Size.Width - offsetX;
            Actions act = new Actions(Driver);
            act.MoveToElement(ele, offsetX, offsetY).Click().Build().Perform();
        }
        public void DoubleClick(By element)
        {
            IWebElement ele = SearchAndFindElement(element);
            Actions act = new Actions(Driver);
            act.DoubleClick().Build().Perform();
        }
        public void sendKeys(string key)
        {
            this.WaitForDOMLoad();
            WaitSeconds(0.4);
            try
            {
                IAlert alert = Driver.SwitchTo().Alert();
                alert.Accept();
            }
            catch
            {
            }

            Actions act = new Actions(Driver);
            act.SendKeys(key);
            act.Perform();
            //WaitSeconds(1);
        }
        public void TakeScreenShot(string fileName)
        {
            if (TFSHelper.GetAppSetting("ExecuteRemotely") == "1")
            {
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                string screenshot = ss.AsBase64EncodedString;
                byte[] screenshotAsByteArray = ss.AsByteArray;
                ss.SaveAsFile(fileName, ImageFormat.Jpeg); //use any of the built in image formating
            }
            else
            {
                ScreenCapture sc = new ScreenCapture();
                sc.CaptureScreenToFile(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
        public void NoThanks()
        {
            try
            {
                if (WebDriver == "Firefox")
                    WaitSeconds(2);
                SearchAndFindElementWithOutWait(By.Id("buttonClose")).Click();
            }
            catch { }

            windowStack.Push(driver.WindowHandles[0]);
        }
        #endregion

        //NEW FUNCTIONS
        public void HighlightElementByXpath(String xpathVal)
        {
            IWebElement el = SearchAndFindElement(By.XPath(xpathVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");
        }
        public void HighlightElementById(String idVal)
        {
            IWebElement el = SearchAndFindElement(By.Id(idVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");
        }
        public void UnHighlightElementByXpath(String xpathVal)
        {
            IWebElement el = SearchAndFindElement(By.XPath(xpathVal));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 2px solid rgba(0, 0, 0, 0.3);");
        }
        public void UnHighlightElementById(String idVal)
        {
            IWebElement el = SearchAndFindElement(By.Id(idVal));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 2px solid rgba(0, 0, 0, 0.3);");
        }

        private static TimeSpan waitForElement = TimeSpan.FromSeconds(10);

        private void WaitForAjaxDataLoadReadyAndFinish()
        {
            WebDriverWait wait = new WebDriverWait(driver, waitForElement);
            wait.Until(d =>
            {
                bool isAjaxFinished = (bool)((IJavaScriptExecutor)d).
                    ExecuteScript("return jQuery.active == 0");
                bool isLoaderHidden = (bool)((IJavaScriptExecutor)d).
                    ExecuteScript("return $('.spinner').is(':visible') == false");
                return isAjaxFinished & isLoaderHidden;
            });
        }

        public EricssonWebElement FindElementWithWait(By by)
        {
            try
            {
                WaitForAjaxDataLoadReadyAndFinish();
                WebDriverWait wait = new WebDriverWait(driver, waitForElement);
                return new EricssonWebElement(wait.Until(ExpectedConditions.ElementIsVisible(by)), by);
            }
            catch
            {
                return null;
            }
        }

        public void FindElementByXpath(String xpathVal)
        {
            IWebElement el = SearchAndFindElement(By.XPath(xpathVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            FindElementWithWait(By.XPath(xpathVal));
            //Thread.Sleep(2000);

            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 2px solid rgba(0, 0, 0, 0.3);");
        }
        public void FindElementByXpathAndClick(String xpathVal)
        {
            IWebElement el = SearchAndFindElement(By.XPath(xpathVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            FindElementWithWait(By.XPath(xpathVal)).Click();
            //Thread.Sleep(2000);

            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 2px solid rgba(0, 0, 0, 0.3);");
        }
        public void FindElementByXpathClick(String xpathVal)
        {
            IWebElement el = SearchAndFindElement(By.XPath(xpathVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            FindElementWithWait(By.XPath(xpathVal)).Click();
            //Thread.Sleep(2000);
        }
        public void FindElementByIdAndClick(String IdVal)
        {
            IWebElement el = SearchAndFindElement(By.Id(IdVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            FindElementWithWait(By.Id(IdVal)).Click();
            //Thread.Sleep(2000);

            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 2px solid rgba(0, 0, 0, 0.3);");
        }
        public void FindElementByIdClick(String IdVal)
        {
            IWebElement el = SearchAndFindElement(By.Id(IdVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            FindElementWithWait(By.Id(IdVal)).Click();
            //Thread.Sleep(2000);
        }

        //DENEME COUNTRY *******************
        public void SelectValueFromLookUp(String lookupID, String value)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            js.ExecuteScript(@"Xrm.Page.getControl('" + lookupID + "').setFocus(true);");
            sendKeys(value);

            WaitSeconds(2);
            sendKeys(Keys.Enter);
            WaitSeconds(2);
            sendKeys(Keys.Enter);

            //js.ExecuteScript(@"var obj = Xrm.Page.getControl('etel_countryid'");
            //    var filter = ""<filter type='and'><condition attribute='etel_name' operator='eq' value='Turkey'/></filter>"";
            //    obj.setFocus(true);
            //    obj.addCustomFilter(filter);
            //");
        }
        public void FocusOnElement(String elementID)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(@"Xrm.Page.getControl('" + elementID + "').setFocus(true);");
        }

        //********************************************************************************
        //Highlight fonksiyonu bazı menulerde ilgili tabın aşağıya inmesine neden olabiliyor. 
        //Bu gibi durumlarda bu iki fonksiyon kullanılmalı.
        public void FindElementByXpathClickWithoutHighlight(string xpathVal)
        {
            EricssonWebElement el = SearchAndFindElement(By.XPath(xpathVal));
            el.Click();
        }
        public void FindElementByIdClickWithoutHighlight(String IdVal)
        {
            EricssonWebElement el = SearchAndFindElement(By.Id(IdVal));
            SearchAndFindElement(By.Id(IdVal)).Click();
        }

        //********************************************************************************
        public void FindElementByXpathAndClear(String xpathVal)
        {
            EricssonWebElement el = SearchAndFindElement(By.XPath(xpathVal));
            el.Clear();
            Thread.Sleep(2000);
        }
        public void FindElementByIdAndClear(String IdVal)
        {
            EricssonWebElement el = SearchAndFindElement(By.Id(IdVal));
            el.Clear();
            Thread.Sleep(2000);
        }
        public void FindElementByXpathAndSendText(String xpathVal, String recordName)
        {
            EricssonWebElement el = SearchAndFindElement(By.XPath(xpathVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            SearchAndFindElement(By.XPath(xpathVal)).Click();
            WaitSeconds(2);
            el.SendKeys(recordName);
            Thread.Sleep(2000);

            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 2px solid rgba(0, 0, 0, 0.3);");
        }
        public void FindElementByIdAndSendText(String IdVal, String recordName)
        {
            EricssonWebElement el = SearchAndFindElement(By.Id(IdVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            FindElementWithWait(By.Id(IdVal)).SendKeys(recordName);
            Thread.Sleep(2000);

            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 2px solid rgba(0, 0, 0, 0.3);");
        }
        public void FindElementById(String IdVal, String element)
        {
            EricssonWebElement el = SearchAndFindElement(By.Id(IdVal));
        }
        public void FindElementByXpath(String xpathVal, String element)
        {
            EricssonWebElement el = SearchAndFindElement(By.Id(xpathVal));
        }
        public void isElementVisibleByXpathThenClick(String xpathVal)
        {
            bool elementControl = DriverHelper.GetInstance().IsElementVisible(SearchAndFindElement(By.XPath(xpathVal)));

            if (elementControl == true)
            {
                FindElementByXpathClick(xpathVal);
                WaitSeconds(3);
            }

            else
            {
                Console.Write(xpathVal + " " + "  does not exist");
            }
        }
        public void isElementVisibleByIdThenClick(String IdVal)
        {
            bool elementControl = DriverHelper.GetInstance().IsElementVisible(SearchAndFindElement(By.Id(IdVal)));

            if (elementControl == true)
            {
                FindElementByXpathClick(IdVal);
                WaitSeconds(3);
            }

            else
            {
                Console.Write(IdVal + "" + " does not exist");
            }
        }
        public void AssertTextById(String IdVal, String assertText)
        {
            EricssonWebElement el = SearchAndFindElement(By.Id(IdVal));
            Assert.IsTrue(SearchAndFindElement(By.Id(IdVal)).Text.Contains(assertText));
        }
        public void AssertTextByXpath(String xpathVal, String assertText)
        {
            EricssonWebElement el = SearchAndFindElement(By.XPath(xpathVal));
            //Assert.IsTrue(SearchAndFindElement(By.XPath(xpathVal)).Text.Contains(assertText));
            Assert.IsTrue(SearchAndFindElement(By.XPath(xpathVal)).Text.Contains(assertText));
        }

        public void WaitUntilElementDisappearByXpath(String xpathVal)
        {
            bool elementControl;
            Stopwatch timeOutWatch = Stopwatch.StartNew();
            while (timeOutWatch.ElapsedMilliseconds < (alertFindTimeOut * 1000))
            {
                try
                {
                    elementControl = DriverHelper.GetInstance().IsElementVisible(SearchAndFindElement(By.XPath(xpathVal)));
                    if (!elementControl)
                        break;
                }
                catch
                { }
            }
        }
        public void WaitUntilElementDisappearById(String IdVal)
        {
            bool elementControl;
            Stopwatch timeOutWatch = Stopwatch.StartNew();
            while (timeOutWatch.ElapsedMilliseconds < (alertFindTimeOut * 1000))
            {
                try
                {
                    elementControl = DriverHelper.GetInstance().IsElementVisible(SearchAndFindElement(By.Id(IdVal)));
                    if (!elementControl)
                        break;
                }
                catch
                { }
            }
        }
        public void WaitForElementByXpath(String xpathVal)
        {
            bool elementControl;
            Stopwatch timeOutWatch = Stopwatch.StartNew();
            while (timeOutWatch.ElapsedMilliseconds < (alertFindTimeOut * 1000))
            {
                try
                {
                    elementControl = DriverHelper.GetInstance().IsElementVisible(SearchAndFindElement(By.XPath(xpathVal)));
                    if (elementControl)
                        break;
                }
                catch
                { }
            }
        }
        public void WaitForElementUntilClickable()
        {
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
        }
        public void WaitForElementById(String IdVal)
        {
            bool elementControl;
            Stopwatch timeOutWatch = Stopwatch.StartNew();
            while (timeOutWatch.ElapsedMilliseconds < (alertFindTimeOut * 1000))
            {
                try
                {
                    elementControl = DriverHelper.GetInstance().IsElementVisible(SearchAndFindElement(By.Id(IdVal)));
                    if (elementControl)
                        break;
                }
                catch
                { }
            }
        }
        public void GetHtmlSource(string htmlFileName)
        {
            string allPageSource = driver.PageSource;
            System.IO.File.WriteAllText(htmlFileName, allPageSource);

        }

        public EricssonWebElement FindElementbyClass(String xpathValue)
        {
            return FindElementWithWait(By.ClassName(xpathValue));
        }
       
        //Is element visible
        public bool IsElementVisible(IWebElement element)
        {
            return element.Displayed && element.Enabled && !element.Size.IsEmpty;
        }
        public bool IsElementVisibleByXpath(String xpathVal)
        {
            EricssonWebElement element = SearchAndFindElement(By.XPath(xpathVal));
            //IWebElement element = driver.FindElement(By.XPath(xpathVal));
            return element.Displayed && element.Enabled && !element.Size.IsEmpty;
        }
        public bool IsElementVisibleById(String IdVal)
        {
            EricssonWebElement element = SearchAndFindElement(By.Id(IdVal));
            //IWebElement element = driver.FindElement(By.Id(IdVal));
            //return element.Displayed && element.Enabled && !element.Size.IsEmpty;
            return element.Displayed;
        }
        public bool GetElementTextAndControlByXpath(String xpathVal, String controlText)
        {
            EricssonWebElement el = SearchAndFindElement(By.XPath(xpathVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            string textValue = el.Text;


            if (textValue == controlText)
            {
                return true;
            }

            else
            {
                return false;
            }
        }
        public bool CompareTextAndControlByXpath(String xpathVal, String controlText)
        {
            EricssonWebElement el = SearchAndFindElement(By.XPath(xpathVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            string textValue = el.Text;

            if (textValue.Contains(controlText))
            {
                return true;
            }

            else
            {
                return false;
            }
        }
        public bool CompareTextAndControlById(String IdVal, String controlText)
        {
            EricssonWebElement el = SearchAndFindElement(By.Id(IdVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            string textValue = el.Text;

            if (textValue.Contains(controlText))
            {
                return true;
            }

            else
            {
                return false;
            }
        }
        public bool GetElementTextAndControlById(String IdVal, String controlText)
        {
            EricssonWebElement el = SearchAndFindElement(By.Id(IdVal));

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            string textValue = el.Text;

            if (textValue == controlText)
            {
                return true;
            }

            else
            {
                return false;
            }
        }
        public String GetElementTextByXpath(String xpathVal)
        {
            string el = SearchAndFindElement(By.XPath(xpathVal)).Text;
            return el;

        }
        public String GetElementTextById(String IdVal)
        {
            string el = SearchAndFindElement(By.Id(IdVal)).Text;

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("arguments[0].setAttribute('style', arguments[1]);", el,
            //" border: 1px solid red;");

            return el;
        }
        public bool AssertTextControlByXpath(String xpathVal, String assertControlText)
        {
            try
            {
                Assert.IsTrue(SearchAndFindElement(By.XPath(xpathVal)).Text.Contains(assertControlText));
                return true;
            }

            catch
            {
                return false;
            }
        }
        public void AssertTextControlById(String IdVal, String assertControlText)
        {
            Assert.IsTrue(SearchAndFindElement(By.Id(IdVal)).Text.Contains(assertControlText));
        }
        //Scroll to element
        public void ScrollToElementByXpath(String xpathVal)
        {
            IWebElement element = SearchAndFindElement(By.XPath(xpathVal));
            Actions actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }
        public void ScrollToElementById(String IdVal)
        {
            IWebElement element = SearchAndFindElement(By.Id(IdVal));
            Actions actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }
        public void openConsoleWhileOnTheScreen()
        {
            WaitForDOMLoad();
            sendKeys(Keys.F12);
            WaitSeconds(5);
            sendKeys(Keys.Control + Keys.NumberPad2);
            WaitSeconds(5);

            //String consoleTab = Keys.(Keys.Control, "2");
            //driver.FindElement(By.XPath(".//*[@id='newCustContactAddrSection']/table/tbody/tr[1]/td[2]/div/input")).sendKeys(consoleTab);
        }
        public void IfElementExistsReturnFalseById(String IdVal)
        {
            bool flag;
            try
            {
                SearchAndFindElementWithOutWait(By.Id(IdVal));
                flag = false;
            }
            catch (Exception)
            {
                flag = true;
            }

            if (flag == true)
            {
                Assert.IsTrue(1 == 1);
            }
            else
            {
                Assert.Fail("Element still displayed");
            }
        }
        public void IfElementExistsReturnFalseByXpath(String xpathVal)
        {
            bool flag;
            try
            {
                SearchAndFindElementWithOutWait(By.XPath(xpathVal));
                flag = false;
            }
            catch (Exception)
            {
                flag = true;
            }

            if (flag == true)
            {
                Assert.IsTrue(1 == 1);
            }
            else
            {
                Assert.Fail("Element still displayed");
            }
        }
        public void IfElementExistsClickElemenAndSendTexttByXpath(String xpathVal, String textValue)
        {

            bool flag;
            try
            {
                EricssonWebElement el = SearchAndFindElementWithOutWait(By.XPath(xpathVal));
                flag = true;
                el.Click();
                sendKeys(textValue);
            }
            catch (Exception)
            {
                flag = false;
            }

            if (flag == true)
            {
                Assert.IsTrue(1 == 1);
            }
        }
        public void ChoseFromLookupByXpath(String xpathVal)
        {
            FindElementByXpathClickWithoutHighlight(xpathVal);
            WaitSeconds(2);
            sendKeys(Keys.Enter);
        }
        public void ChoseFromLookupById(String IdVal)
        {
            FindElementByXpathClickWithoutHighlight(IdVal);
            WaitSeconds(2);
            sendKeys(Keys.Enter);
        }
        public void IsElementVisibleClickByXpath(String xpathVal)
        {
            int saniye;
            for (saniye = 0; saniye < 15; saniye++)
            {
                WaitSeconds(1);
                try
                {
                    if (IsElementVisibleByXpath(xpathVal) == true)
                    {
                        FindElementByXpathClickWithoutHighlight(xpathVal);
                        break;
                    }
                }
                catch
                {
                    if (saniye == 14)
                        Assert.Fail("No such element");
                }

            }
        }
        public void IsElementVisibleClickById(String IdVal)
        {
            int saniye;
            for (saniye = 0; saniye < 15; saniye++)
            {
                WaitSeconds(1);
                try
                {
                    if (IsElementVisibleByXpath(IdVal) == true)
                    {
                        FindElementByXpathClickWithoutHighlight(IdVal);
                        break;
                    }
                }
                catch
                {
                    if (saniye == 14)
                        Assert.Fail("No such element");
                }

            }
        }
        public void DoubleClick2(IWebElement element)
        {
            Actions action = new Actions(driver).DoubleClick(element);
            action.Build().Perform();
        }
        public void DoubleClickk(String xpathVal)
        {
            WaitSeconds(1);
            new Actions(driver).DoubleClick(driver.FindElement(By.XPath(xpathVal))).Perform();
        }
        public void DoubleClickById(String IdVal)
        {
            WaitSeconds(1);
            new Actions(driver).DoubleClick(driver.FindElement(By.Id(IdVal))).Perform();
        }
        //public void OpenNewTab()

        //{
        //    //driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Control + "t");
        //    //ArrayList tabs = new ArrayList(driver.WindowHandles);
        //    //driver.SwitchTo().Window(tabs.get(0));

        //    //sendKeys(Keys.Control + "t");
        //    //string msisdn = "05056785432";
        //    //List<string> tabs2 = new List<string>(driver.WindowHandles);
        //    //driver.SwitchTo().Window(tabs2[1]);
        //    //ERMSFacilities.getInstance().CheckSimMsisdnIsReserved(msisdn);
        //    //driver.Close();
        //    //driver.SwitchTo().Window(tabs2[0]);

        //    //IWebElement element = driver.FindElement(By.TagName("body"));
        //    //WaitSeconds(4);
        //    //element.SendKeys(Keys.Control + "n");
        //    //WaitSeconds(2);
        //    //FindAndSwithNewWindow();
        //    WaitSeconds(4);
        //    sendKeys(Keys.Control + "n");
        //    String currentWindowHandle = driver.CurrentWindowHandle;
        //    //ERMS
        //    //ERMSFacilities.getInstance().InitERMS();

        //    List<String> windowHandles = new List<String>(driver.WindowHandles);

        //    foreach (string window in windowHandles)
        //    {
        //        if (window != currentWindowHandle)
        //        {
        //            driver.SwitchTo().Window(window);
        //            FindAndSwithNewWindow();
        //            ERMSFacilities.getInstance().InitERMS();
        //            driver.Close();

        //        }
        //    }




        //}
    }
}


