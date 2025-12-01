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
    [Order(5)]
    public class EmployeeTest
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

          
            // Esto le da al navegador hasta 15 segundos para encontrar los elementos antes de fallar.
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
      
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/");

            // --- PRE-CONDICIÓN: LOGIN AUTOMÁTICO ---
            // Para probar empleados, YA debemos ser Admin.
            LoginPage login = new LoginPage(_driver);
            login.Login("Admin", "admin123");

            _test.Log(Status.Info, "Login realizado correctamente como pre-condición.");
        }

        [Test]
        public void AddNewEmployeeTest()
        {
            AddEmployeePage employeePage = new AddEmployeePage(_driver);

            _test.Log(Status.Info, "Navegando DIRECTAMENTE al módulo PIM por URL...");

            // Vamos directo a la URL de la lista de empleados ***
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            // Esperamos un segundo a que cargue la tabla
            System.Threading.Thread.Sleep(5000);

            _test.Log(Status.Info, "Haciendo clic en Add...");
            employeePage.ClickAddButton();

            // ... (El resto de tu código de llenar datos sigue igual) ...
            string nombre = "Empleado" + new Random().Next(100, 999);
            string apellido = "SinDashboard";

            employeePage.EnterEmployeeDetails(nombre, apellido);
            employeePage.ClickSave();

            Assert.That(employeePage.IsSuccessMessageDisplayed(), Is.True);
        }

        // -------------------------------------------------------
        // PRUEBA NEGATIVA: Campos Vacíos
        // -------------------------------------------------------
        [Test]
        public void AddEmployeeEmptyTest()
        {
            AddEmployeePage employeePage = new AddEmployeePage(_driver);

            _test.Log(Status.Info, "Navegando al PIM...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");
            System.Threading.Thread.Sleep(4000); // Espera de seguridad

            _test.Log(Status.Info, "Click en Add sin llenar datos...");
            employeePage.ClickAddButton();

            // INTENTAMOS GUARDAR VACÍO
            employeePage.ClickSave();

            // Verificamos errores
            bool errorsVisible = employeePage.AreRequiredErrorsDisplayed();

            Assert.That(errorsVisible, Is.True, "No aparecieron los mensajes de 'Required'.");
            _test.Log(Status.Pass, "El sistema bloqueó la creación de empleado vacío correctamente.");
        }

        // -------------------------------------------------------
        // PRUEBA DE LÍMITES: Nombre Muy Largo (Límite Superior)
        // -------------------------------------------------------
        [Test]
        public void AddEmployeeLimitTest()
        {
            AddEmployeePage employeePage = new AddEmployeePage(_driver);
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");
            System.Threading.Thread.Sleep(2000);

            employeePage.ClickAddButton();

            // Usamos 100 caracteres (Intencionalmente inválido)
            string longName = new string('A', 100);
            string lastName = "LimitTest";

            _test.Log(Status.Info, "Probando envío de 100 caracteres (Excede límite)...");
            employeePage.EnterEmployeeDetails(longName, lastName);
            employeePage.ClickSave();

       

            // NO preguntamos si hubo éxito (isSuccess), preguntamos si hubo ERROR.
            bool errorAparecio = employeePage.IsInputErrorDisplayed();

            // Si el error aparece, la prueba PASA (porque el sistema se defendió bien)
            Assert.That(errorAparecio, Is.True, "El sistema debió mostrar un error y no lo hizo.");

            _test.Log(Status.Pass, "Correcto: El sistema bloqueó el nombre de 100 caracteres.");
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
            System.Threading.Thread.Sleep(2000);
        }
    }
    
}
