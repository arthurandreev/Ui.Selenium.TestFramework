﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow;

namespace SimpleSeleniumFramework.TestFramework
{
    public class PageManagerFactory
    {
        protected IWebDriver Driver;
        protected ScenarioContext ScenarioContext;  
        public PageManagerFactory(IWebDriver driver, ScenarioContext scenarioContext)
        {
            Driver = driver;
            ScenarioContext = scenarioContext;
        }

        protected string GetUrl() => Driver.Url;
        protected string GetPageTitle() => Driver.Title;
        protected void GoToUrl(string url) => Driver.Navigate().GoToUrl(url);
   
        protected IWebElement GetElement(By by)
        {
            return Driver.FindElement(by);
        }

        protected IList<IWebElement> GetElements(By by)
        {
            return Driver.FindElements(by);
        }

        protected void MoveToElement(IWebElement element)
        {
            try
            {
                Actions action = new Actions(Driver);
                action.MoveToElement(element).Build().Perform();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot move to the following element: {element.Text}");
                Console.WriteLine($"MoveToElement threw the following exception: {e} and stack trace {e.StackTrace}");
                TakeScreenshot();
            }
        }

        protected void ClickElement(IWebElement element)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(condition => element != null && element.Enabled);
                MoveToElement(element);
                element.Click();
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine($"Cannot click the following element: {element.Text}");
                Console.WriteLine($"ClickElement threw the following exception: {e} and stack trace {e.StackTrace}");
                TakeScreenshot();
            }
        }

        protected virtual void FluentWaitForElementToAppear(By by, int timeout, int pollInterval)
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(timeout),
                PollingInterval = TimeSpan.FromMilliseconds(pollInterval)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            fluentWait.Until(x => x.FindElement(by));
        }

        protected virtual void FluentWaitForElementToDisappear(By by, int timeout, int pollInterval)
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(timeout),
                PollingInterval = TimeSpan.FromMilliseconds(pollInterval)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            fluentWait.Until(x => (x.FindElements(by).Count == 0));
        }

        protected void TakeScreenshot()
        {
            try
            {
                Screenshot screenShot = ((ITakesScreenshot)Driver).GetScreenshot();
                string title = ScenarioContext.ScenarioInfo.Title;
                string screenShotName = title + DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss");
                string testResultsFolder = Directory.GetCurrentDirectory();
                string filePathAndName = testResultsFolder + "\\" + screenShotName + ".jpeg";
                screenShot.SaveAsFile(filePathAndName, ScreenshotImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                Console.WriteLine($"TakeScreenShot threw the following exception: {e} and stack trace {e.StackTrace}");
            }
            
        }

        protected void JSClickElement(IWebElement element)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
                wait.Until(condition => element != null && element.Enabled);
                MoveToElement(element);
                js.ExecuteScript("arguments[0].click();", element);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot click the following element: {element.Text}");
                Console.WriteLine($"JSClickElement threw the following exception: {e} and stack trace {e.StackTrace}");
                TakeScreenshot();
            }
        }
    }
}
