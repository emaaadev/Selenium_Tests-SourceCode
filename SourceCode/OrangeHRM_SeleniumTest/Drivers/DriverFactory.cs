using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace OrangeHRM_SeleniumTest.Drivers
{
    public class DriverFactory
    {
        public static IWebDriver CreateDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());

            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            return new ChromeDriver(options);
        }
    }
}