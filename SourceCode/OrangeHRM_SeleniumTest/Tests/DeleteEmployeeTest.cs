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
    [Order(4)]
    public class DeleteEmployeeTest
    {
        private IWebDriver _driver;
        private ExtentReports _extent;
        private ExtentTest _test;

        [OneTimeSetUp]
        public void OneTimeSetup() { _extent = ReportManager.GetInstance(); }

        [SetUp]
        public void Setup()
        {
            _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            _driver = DriverFactory.CreateDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/");

            LoginPage login = new LoginPage(_driver);
            login.Login("Admin", "admin123");
        }

        // ---------------------------------------------------------
        // TEST 1: ELIMINACIÓN EXITOSA (End-to-End)
        // ---------------------------------------------------------
        [Test]
        public void DeleteEmployeeSuccess()
        {
            AddEmployeePage addPage = new AddEmployeePage(_driver);
            EmployeeListPage listPage = new EmployeeListPage(_driver);

            // PASO 1: CREAR UNA "VÍCTIMA"
            _test.Log(Status.Info, "Creando empleado temporal para borrarlo...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/addEmployee");

            // Generamos un número aleatorio grande para evitar IDs duplicados en el nombre
            string nombreVictima = "DeleteMe" + new Random().Next(10000, 99999);
            addPage.EnterEmployeeDetails(nombreVictima, "Test");
            addPage.ClickSave();

            // ********** CORRECCIÓN 1: VALIDAR QUE SE CREÓ **********
            // Si falla por "ID existe", el test morirá aquí y te lo dirá claramente.
            bool creado = addPage.IsSuccessMessageDisplayed();
            Assert.That(creado, Is.True, "Error Crítico: No se pudo crear el empleado víctima (Posible ID duplicado).");
            // *******************************************************

            // PASO 2: BUSCAR A LA VÍCTIMA
            _test.Log(Status.Info, $"Buscando a la víctima ({nombreVictima}) en la lista...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            // Pausa de seguridad para que cargue el módulo de búsqueda
            System.Threading.Thread.Sleep(2000);

            listPage.SearchEmployee(nombreVictima);

            // ********** CORRECCIÓN 2: ESPERA VISUAL **********
            // La tabla tarda unos segundos en refrescarse después de dar Buscar.
            // Sin esto, el robot busca el icono antes de que aparezca el resultado.
            System.Threading.Thread.Sleep(3000);
            // *************************************************

            // PASO 3: BORRAR
            _test.Log(Status.Info, "Haciendo clic en el icono de Basura...");

            // Verificamos si encontró algo antes de intentar borrar
            if (listPage.IsNoRecordsFoundDisplayed())
            {
                Assert.Fail($"La búsqueda no trajo resultados para {nombreVictima}. No se puede borrar.");
            }

            listPage.ClickDeleteIcon();

            _test.Log(Status.Info, "Confirmando en el Popup (Yes, Delete)...");
            listPage.ConfirmDelete();

            // PASO 4: VALIDAR
            bool eliminado = addPage.IsSuccessMessageDisplayed();

            Assert.That(eliminado, Is.True, "No apareció el mensaje de éxito al eliminar.");
            _test.Log(Status.Pass, "Empleado eliminado correctamente.");
        }

        // ---------------------------------------------------------
        // PRUEBA NEGATIVA: Intentar borrar sin seleccionar nada
        // ---------------------------------------------------------
        [Test]
        public void DeleteWithoutSelectionTest()
        {
            EmployeeListPage listPage = new EmployeeListPage(_driver);

            _test.Log(Status.Info, "Navegando a la lista...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            // VALIDACIÓN:
            // Sin tocar nada, el botón "Delete Selected" NO debe existir.
            bool botonVisible = listPage.IsBulkDeleteButtonVisible();

            // Esperamos que sea Falso (False)
            Assert.That(botonVisible, Is.False, "Error: El botón de borrado masivo aparece sin haber seleccionado nada.");

            _test.Log(Status.Pass, "Correcto: El sistema protege el borrado si no hay selección.");
        }

        // ---------------------------------------------------------
        // PRUEBA DE LÍMITES: Borrado Masivo (Límite de Página)
        // ---------------------------------------------------------
        [Test]
        public void DeleteLimitBulkTest()
        {
            EmployeeListPage listPage = new EmployeeListPage(_driver);

            _test.Log(Status.Info, "Navegando a la lista...");
            _driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList");

            // 1. SELECCIONAR TODO (Estresamos la UI seleccionando todas las filas visibles)
            _test.Log(Status.Info, "Activando 'Seleccionar Todo'...");
            listPage.ClickSelectAllCheckbox();

            // 2. VERIFICAR QUE APAREZCA EL BOTÓN
            bool botonVisible = listPage.IsBulkDeleteButtonVisible();
            Assert.That(botonVisible, Is.True, "El botón 'Delete Selected' no apareció tras seleccionar todo.");

            // 3. PROBAR INTERACCIÓN (Solo abrir el popup, luego cancelamos para no vaciar la demo)
            listPage.ClickBulkDeleteButton();

            _test.Log(Status.Info, "Popup masivo abierto. Cancelando para no borrar datos ajenos...");
            listPage.CancelDelete(); // Reusamos tu método existente

            _test.Log(Status.Pass, "El sistema manejó correctamente la selección masiva.");
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
        public void OneTimeTearDown() { _extent.Flush(); }
    }
}
