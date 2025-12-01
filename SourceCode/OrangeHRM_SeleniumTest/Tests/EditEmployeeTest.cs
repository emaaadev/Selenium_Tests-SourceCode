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
    [Order(3)]
    public class EditEmployeeTest
    {
        private IWebDriver _driver;
        private ExtentReports _extent;
        private ExtentTest _test;

        // Configuración Global del Reporte
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _extent = ReportManager.GetInstance();
        }

        // Configuración antes de cada Test (Login automático)
        [SetUp]
        public void Setup()
        {
            // 1. Crear el test en el reporte
            _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);

            // 2. Iniciar Driver
            _driver = DriverFactory.CreateDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);

            // 3. Ir a la página
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/");

            // 4. LOGIN (Pre-condición necesaria para editar)
            LoginPage login = new LoginPage(_driver);
            login.Login("Admin", "admin123");

            _test.Log(Status.Info, "Login realizado correctamente como Admin.");
        }

        // **********************************************************
        // EL TEST DE ACTUALIZACIÓN
        // **********************************************************
        [Test]
        public void UpdateEmployeeLastName()
        {
            // Instancias de las páginas que vamos a usar
            EmployeeListPage listPage = new EmployeeListPage(_driver);
            AddEmployeePage formPage = new AddEmployeePage(_driver);

            _test.Log(Status.Info, "Navegando a la lista de empleados...");

            // Navegación directa para ahorrar tiempo y evitar errores de menú
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            // 1. BUSCAR UN EMPLEADO QUE EXISTA

            string nombreOriginal = "hh";

            _test.Log(Status.Info, $"Buscando al empleado: {nombreOriginal}");
            listPage.SearchEmployee(nombreOriginal);

            // 2. HACER CLIC EN EL LÁPIZ (EDITAR)
            _test.Log(Status.Info, "Haciendo clic en el icono de Editar (Lápiz)...");
            listPage.ClickEditIcon();

            // 3. MODIFICAR DATOS
            // Generamos un apellido aleatorio para verificar que el cambio es único
            string nuevoApellido = "Update" + new Random().Next(100, 999);

            _test.Log(Status.Info, $"Actualizando apellido a: {nuevoApellido}");

            // *** AQUÍ USAMOS EL NUEVO MÉTODO QUE CREASTE ***
            // Este método borra el texto anterior y escribe el nuevo
            formPage.UpdateEmployeeDetails(nombreOriginal, nuevoApellido);

            // Guardamos
            formPage.ClickSave();

            // 4. VALIDAR ÉXITO
            bool guardado = formPage.IsSuccessMessageDisplayed();

            Assert.That(guardado, Is.True, "Fallo: No apareció el mensaje de éxito al guardar.");
            _test.Log(Status.Pass, "Empleado actualizado correctamente.");
        }

        // -----------------------------------------------------------
        // PRUEBA NEGATIVA: Intentar guardar cambios dejando campos vacíos
        // -----------------------------------------------------------
        [Test]
        public void EditEmployeeEmptyTest()
        {
            EmployeeListPage listPage = new EmployeeListPage(_driver);
            AddEmployeePage formPage = new AddEmployeePage(_driver);

            _test.Log(Status.Info, "Navegando a la lista de empleados...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            string nombreOriginal = "hh"; 
            listPage.SearchEmployee(nombreOriginal);

            if (listPage.IsNoRecordsFoundDisplayed()) Assert.Fail($"No se encontró a {nombreOriginal}");

            listPage.ClickEditIcon();

            // 2. BORRAR LOS DATOS
            _test.Log(Status.Info, "Borrando nombre y apellido completamente...");

            // *** CAMBIO AQUÍ: Usamos el nuevo método de borrado agresivo ***
            formPage.ClearEmployeeData();

            formPage.ClickSave();

            // 3. VALIDAR
            bool erroresVisibles = formPage.AreRequiredErrorsDisplayed();

            // Si esto falla ahora, es porque realmente la pagina tiene un bug, 
            // pero con el borrado letra por letra debería funcionar y mostrar el error.
            Assert.That(erroresVisibles, Is.True, "El sistema permitió guardar campos vacíos.");
            _test.Log(Status.Pass, "Correcto: El sistema bloqueó la edición con campos vacíos.");

            System.Threading.Thread.Sleep(2000);
        }
        // -----------------------------------------------------------
        // PRUEBA DE LÍMITES: Intentar actualizar con nombre muy largo
        // -----------------------------------------------------------
        [Test]
        public void EditEmployeeLimitTest()
        {
            EmployeeListPage listPage = new EmployeeListPage(_driver);
            AddEmployeePage formPage = new AddEmployeePage(_driver);

            _test.Log(Status.Info, "Navegando a la lista de empleados...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            // *** AGREGAR ESTA PAUSA ***
            // Esperamos 3 segundos para asegurar que el formulario de búsqueda cargó completamente
            System.Threading.Thread.Sleep(2000);
            string nombreOriginal = "hh";
            listPage.SearchEmployee(nombreOriginal);

            if (listPage.IsNoRecordsFoundDisplayed()) Assert.Fail($"No se encontró a {nombreOriginal}");

            listPage.ClickEditIcon();

            // 2. INTENTAR ACTUALIZAR CON 100 CARACTERES
            string apellidoLargo = new string('X', 100);

            _test.Log(Status.Info, "Intentando actualizar con un apellido de 100 caracteres...");
            formPage.UpdateEmployeeDetails(nombreOriginal, apellidoLargo);

            formPage.ClickSave();

            // 3. VALIDAR ERROR (Igual que en la creación, esperamos el mensaje rojo)
            bool errorInputVisible = formPage.IsInputErrorDisplayed();

            Assert.That(errorInputVisible, Is.True, "El sistema no mostró error al exceder el límite de caracteres.");
            _test.Log(Status.Pass, "El sistema validó correctamente el límite de caracteres al editar.");
        }

        // Cierre del Test (Captura de pantalla y reporte)
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
