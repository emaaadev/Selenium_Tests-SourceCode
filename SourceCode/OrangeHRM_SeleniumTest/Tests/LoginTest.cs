using AventStack.ExtentReports;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OrangeHRM_SeleniumTest.Drivers;
using OrangeHRM_SeleniumTest.Pages;
using OrangeHRM_SeleniumTest.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumTest.Tests
{
    [Order(1)]
    [TestFixture]
        public class LoginTest
        {
            private IWebDriver _driver;
            private ExtentReports _extent;
            private ExtentTest _test;

            // 1. CONFIGURACIÓN GLOBAL (Se ejecuta UNA vez antes de todos los tests)
            // Aquí inicializamos el archivo HTML del reporte.
            [OneTimeSetUp]
            public void OneTimeSetup()
            {
                _extent = ReportManager.GetInstance();
            }

            // 2. CONFIGURACIÓN DEL TEST (Se ejecuta antes de CADA test)
            [SetUp]
            public void Setup()
            {
                // Creamos una entrada en el reporte con el nombre del Test actual
                _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);

                // Iniciamos el navegador
                _driver = DriverFactory.CreateDriver();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/");

                _test.Log(Status.Info, "Navegador iniciado y URL cargada.");
            }

            // 3. EL TEST EN SÍ
            [Test]
            public void ValidLogin()
            {
                LoginPage login = new LoginPage(_driver);
                _test.Log(Status.Info, "Ingresando credenciales...");
                login.Login("Admin", "admin123");

                
            }

        [Test]
        public void InvalidLoginTest()
        {
            LoginPage login = new LoginPage(_driver);
            _test.Log(Status.Info, "Prueba Negativa: Credenciales invalidas");

            login.Login("AdminFalso", "1234");

            bool isErrorVisible = login.IsInvalidCredentialsErrorDisplayed();
            Assert.That(isErrorVisible, Is.True, "El mensaje no apareció");
            _test.Log(Status.Pass, "El sistema bloqueó el acceso.");
        }

        [Test]
        public void EmptyFieldsTest()
        {
            LoginPage login = new LoginPage(_driver);
            _test.Log(Status.Info, "Prueba de Limites: Campos vacios");

            login.Login("", "");

            bool isRequiredVisible = login.IsRequiredMessageDisplayed();
            Assert.That(isRequiredVisible,Is.True, "El mensaje 'Required' no apareció");
            _test.Log(Status.Pass, "El sistema validó campos vacíos.");
        }

        // 4. CIERRE DEL TEST (Se ejecuta después de CADA test)
        [TearDown]
        public void TearDown()
        {
            System.Threading.Thread.Sleep(2000); // Espera técnica
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {

                _extent.RemoveTest(_test);
            }
            else if (status == NUnit.Framework.Interfaces.TestStatus.Passed)
            {
                // SI PASÓ: Tomamos foto y lo dejamos listo para guardar.
                string absolutePath = ScreenshotHelper.TakeScreenshot(_driver, TestContext.CurrentContext.Test.Name);
                string relativePath = "Screenshots/" + Path.GetFileName(absolutePath);

                _test.Log(Status.Pass, "Test Exitoso");
                _test.AddScreenCaptureFromPath(relativePath);
            }

            if (_driver != null) { _driver.Quit(); _driver.Dispose(); }

            _extent.Flush();

            System.Threading.Thread.Sleep(2000); // Enfriamiento
        }

    }
        
}

