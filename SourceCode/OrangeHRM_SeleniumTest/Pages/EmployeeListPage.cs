using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumTest.Pages
{
    public class EmployeeListPage
    {
        private readonly IWebDriver _driver;
        private WebDriverWait _wait;

        public EmployeeListPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // ********** LOCALIZADORES **********

        // 1. Input de Nombre de Empleado (Busca la etiqueta y baja al input)
        private By employeeNameInput = By.XPath("//label[contains(text(),'Employee Name')]/../following-sibling::div//input");

        // 2. Botón Buscar
        private By searchButton = By.XPath("//button[@type='submit']");

        // 3. Botón Reset (Para limpiar filtros)
        private By resetButton = By.XPath("//button[@type='reset']");

        // 4. Las filas de la tabla de resultados
        private By tableRows = By.XPath("//div[@class='oxd-table-card']");

        // 5. Celdas con nombres (Tercera columna aprox)
        private By firstRowName = By.XPath("//div[@class='oxd-table-body']/div[1]//div[@role='cell'][3]");

        // 6. Mensaje de "No Records Found"
        private By noRecordsMessage = By.XPath("//span[text()='No Records Found']");


        // ********** MÉTODOS **********

        public void SearchEmployee(string name)
        {
            // Limpiamos filtros previos por seguridad
            // _driver.FindElement(resetButton).Click(); 
            // System.Threading.Thread.Sleep(1000); // Espera visual breve

            // Escribimos el nombre
            var input = _wait.Until(ExpectedConditions.ElementIsVisible(employeeNameInput));
            input.SendKeys(name);

            // Click en Buscar
            _driver.FindElement(searchButton).Click();

            // Esperamos a que la tabla se recargue (truco: esperar que el loader desaparezca o esperar unos segundos)
            System.Threading.Thread.Sleep(2000);
        }

        // Método para verificar si el nombre buscado aparece en la primera fila
        public bool VerifyEmployeeInResults(string expectedName)
        {
            try
            {
                IWebElement rowName = _wait.Until(ExpectedConditions.ElementIsVisible(firstRowName));
                string actualName = rowName.Text;

                // Verificamos si el texto en la tabla contiene el nombre buscado
                return actualName.Contains(expectedName);
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        // Método para verificar el mensaje "No Records Found"
        public bool IsNoRecordsFoundDisplayed()
        {
            try
            {
                return _wait.Until(ExpectedConditions.ElementIsVisible(noRecordsMessage)).Displayed;
            }
            catch
            {
                return false;
            }
        }

     
        // Usamos XPath que busca en la primera fila (div[1]) cualquier botón que contenga un icono de 'pencil'
        private By editIcon = By.XPath("//div[@class='oxd-table-body']/div[1]//button[.//i[contains(@class, 'bi-pencil')]]");

        public void ClickEditIcon()
        {
            // Esperamos a que el botón sea cliqueable
            _wait.Until(ExpectedConditions.ElementToBeClickable(editIcon)).Click();
        }

        // --- AGREGAR EN EmployeeListPage.cs ---

        // 1. Icono de Basura (Delete) en la primera fila
        private By deleteIcon = By.XPath("//div[@class='oxd-table-body']/div[1]//button[.//i[contains(@class, 'bi-trash')]]");

        // 2. Botón "Yes, Delete" en el Popup de confirmación
        // OrangeHRM usa un modal, así que buscamos el botón con ese texto específico
        private By yesDeleteButton = By.XPath("//button[contains(., 'Yes, Delete')]");

        // 3. Botón "No, Cancel" (Para la prueba negativa)
        private By cancelDeleteButton = By.XPath("//button[contains(., 'No, Cancel')]");


        // MÉTODOS

        public void ClickDeleteIcon()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(deleteIcon)).Click();
        }

        public void ConfirmDelete()
        {
            // Esperamos a que aparezca el popup y el botón sea cliqueable
            _wait.Until(ExpectedConditions.ElementToBeClickable(yesDeleteButton)).Click();
        }

        public void CancelDelete()
        {
            _wait.Until(ExpectedConditions.ElementToBeClickable(cancelDeleteButton)).Click();
        }

        // ************************************************************************
        // AGREGAR ESTO AL FINAL DE EmployeeListPage.cs (ANTES DE LA ÚLTIMA LLAVE)
        // ************************************************************************

        // 1. Localizador del Checkbox "Seleccionar Todo" (en el encabezado gris)
        private By selectAllCheckbox = By.XPath("//div[@role='columnheader']//span[contains(@class, 'oxd-checkbox-input')]");

        // 2. Localizador del Botón "Delete Selected" (Rojo)
        private By bulkDeleteButton = By.XPath("//button[contains(., 'Delete Selected')]");


        // MÉTODOS NUEVOS (Sin tocar los viejos)

        // Verifica si el botón rojo es visible
        public bool IsBulkDeleteButtonVisible()
        {
            try
            {
                return _driver.FindElement(bulkDeleteButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                // Si no existe en el HTML, retornamos falso (es lo correcto cuando no hay selección)
                return false;
            }
        }

        // Hace clic en "Seleccionar Todo"
        public void ClickSelectAllCheckbox()
        {
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(selectAllCheckbox)).Click();
        }

        // Hace clic en el botón de borrado masivo
        public void ClickBulkDeleteButton()
        {
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(bulkDeleteButton)).Click();
        }
    }
}
