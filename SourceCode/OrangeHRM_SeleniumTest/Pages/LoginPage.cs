using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumTest.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver driver;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        private IWebElement Username => driver.FindElement(By.Name("username"));
        private IWebElement Password => driver.FindElement(By.Name("password"));
        private IWebElement LoginBtn => driver.FindElement(By.CssSelector("button[type='submit']"));

        public void Login(string user, string pass)
        {
            Username.SendKeys(user);
            Password.SendKeys(pass);
            LoginBtn.Click();
        }

        // 1. Mensaje rojo de "Invalid credentials" (Aparece arriba)
        private By errorAlert = By.XPath("//p[text()='Invalid credentials']");

        // 2. Mensaje de "Required" (Aparece debajo de los inputs vacíos)
        private By requiredFieldMessage = By.XPath("//span[text()='Required']");
        public bool IsInvalidCredentialsErrorDisplayed()
        {
            try
            {
                return driver.FindElement(errorAlert).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Nuevo: Verifica si aparece el mensaje de "Campo Requerido"
        public bool IsRequiredMessageDisplayed()
        {
            try
            {
                return driver.FindElement(requiredFieldMessage).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
