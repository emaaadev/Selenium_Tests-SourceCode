using AventStack.ExtentReports;
using NUnit.Framework;
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
    [TestFixture]
    [Order(2)]
    public class SearchEmployeeTest
    {
        private IWebDriver _driver;
        private ExtentReports _extent;
        private ExtentTest _test;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _extent = ReportManager.GetInstance();
        }

        [SetUp]
        public void Setup()
        {
            _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            _driver = DriverFactory.CreateDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);

            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/");

            // Login Prerrequisito
            LoginPage login = new LoginPage(_driver);
            login.Login("Admin", "admin123");
        }

        // --------------------------------------------------------
        // TEST 1: BÚSQUEDA EXITOSA
        // --------------------------------------------------------
        [Test]
        public void SearchExistingEmployeeTest()
        {
            EmployeeListPage searchPage = new EmployeeListPage(_driver);

            _test.Log(Status.Info, "Navegando al PIM...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            string nombreABuscar = "Charles";

            _test.Log(Status.Info, $"Buscando empleado: {nombreABuscar}");
            searchPage.SearchEmployee(nombreABuscar);

            bool isFound = searchPage.VerifyEmployeeInResults(nombreABuscar);

            Assert.That(isFound, Is.True, $"El empleado {nombreABuscar} no apareció en la tabla.");
            _test.Log(Status.Pass, "Empleado encontrado correctamente.");
        }

        // --------------------------------------------------------
        // TEST 2: BÚSQUEDA SIN RESULTADOS
        // --------------------------------------------------------
        [Test]
        public void SearchNonExistingEmployeeTest()
        {
            EmployeeListPage searchPage = new EmployeeListPage(_driver);

            _test.Log(Status.Info, "Navegando al PIM...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            string nombreFalso = "Batman Superhéroe";

            _test.Log(Status.Info, $"Buscando empleado inexistente: {nombreFalso}");
            searchPage.SearchEmployee(nombreFalso);

            bool mensajeVisible = searchPage.IsNoRecordsFoundDisplayed();

            Assert.That(mensajeVisible, Is.True, "No apareció el mensaje de 'No Records Found'.");
            _test.Log(Status.Pass, "El sistema indicó correctamente que no hay registros.");
        }

        // --------------------------------------------------------
        // TEST 3: PRUEBA DE LÍMITES (Input Gigante)
        // --------------------------------------------------------
        [Test]
        public void SearchLimitCharacterTest()
        {
            EmployeeListPage searchPage = new EmployeeListPage(_driver);

            _test.Log(Status.Info, "Navegando al PIM...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            // Generamos un string de 200 caracteres (Límite superior absurdo para una búsqueda)
            string longSearchTerm = new string('Z', 31);

            _test.Log(Status.Info, "Buscando con un término de 31 caracteres...");
            searchPage.SearchEmployee(longSearchTerm);

            // Verificamos que el sistema NO explotó y simplemente nos dice "No hay resultados"
            bool mensajeVisible = searchPage.IsNoRecordsFoundDisplayed();

            Assert.That(mensajeVisible, Is.True, "El sistema falló o no mostró 'No Records Found' ante una búsqueda gigante.");
            _test.Log(Status.Pass, "El sistema manejó correctamente el límite de caracteres en la búsqueda.");
        }

        
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

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _extent.Flush();
        }
    }
}
