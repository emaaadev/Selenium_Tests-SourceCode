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
    public class AddEmployeePage
    {
        private readonly IWebDriver _driver;
        private WebDriverWait _wait;

        // ********** CONSTRUCTOR **********
        public AddEmployeePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // ********** LOCALIZADORES **********

        // Botón "Add" (aparece cuando entras al módulo PIM)
        private By addButton = By.XPath("//button[normalize-space()='Add']"); // Usamos normalize-space por si hay espacios

        // Campos del formulario
        private By firstNameInput = By.Name("firstName");
        private By lastNameInput = By.Name("lastName");

        // Botón Guardar
        private By saveButton = By.XPath("//button[@type='submit']");

        // Mensaje de éxito (el recuadro verde que sale arriba a la derecha)
        private By successMessage = By.XPath("//div[contains(@class, 'oxd-toast-content') and contains(., 'Success')]");


        // ********** MÉTODOS **********

        public void ClickAddButton()
        {
            // Esperamos que el botón sea cliqueable
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(addButton)).Click();
        }

        public void EnterEmployeeDetails(string firstName, string lastName)
        {
            // Esperamos que el campo sea visible antes de escribir
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(firstNameInput)).SendKeys(firstName);
            _driver.FindElement(lastNameInput).SendKeys(lastName);
        }

        public void ClickSave()
        {
            _driver.FindElement(saveButton).Click();
        }

        public bool IsSuccessMessageDisplayed()
        {
            try
            {
                // Esperamos a que aparezca el toast verde
                return _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(successMessage)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        // --- AGREGAR EN AddEmployeePage.cs ---

        // 1. Localizadores de los mensajes de error "Required"
        // Usamos XPath relativo: Buscamos el input, subimos a su padre y buscamos el span de error
        private By firstNameError = By.XPath("//input[@name='firstName']/../following-sibling::span");
        private By lastNameError = By.XPath("//input[@name='lastName']/../following-sibling::span");

        // 2. Método para verificar si aparecen los errores
        public bool AreRequiredErrorsDisplayed()
        {
            try
            {
                // Verificamos que AMBOS mensajes sean visibles
                bool first = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(firstNameError)).Displayed;
                bool last = _driver.FindElement(lastNameError).Displayed;
                return first && last;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Localizador del mensaje de error (letras rojas debajo del input)
        private By inputErrorMessage = By.CssSelector("span.oxd-input-group__message");

        // Método que verifica si el error existe
        public bool IsInputErrorDisplayed()
        {
            try
            {
                // Esperamos hasta que aparezca el error
                return _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(inputErrorMessage)).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                // Si pasaron los segundos y no apareció, retornamos falso
                return false;
            }
        }

        // ***************************************************************
        // AGREGAR AL FINAL DE AddEmployeePage.cs (Sin borrar lo anterior)
        // ***************************************************************

        public void UpdateEmployeeDetails(string newFirstName, string newLastName)
        {
            // 1. CAMPO FIRST NAME
            // Esperamos que sea visible
            var txtFirst = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(firstNameInput));

            // Lógica de limpieza (Ctrl + A + Backspace)
            txtFirst.SendKeys(Keys.Control + "a");
            txtFirst.SendKeys(Keys.Backspace);

            // Escribimos el nuevo nombre
            txtFirst.SendKeys(newFirstName);

            // 2. CAMPO LAST NAME
            var txtLast = _driver.FindElement(lastNameInput);

            // Lógica de limpieza
            txtLast.SendKeys(Keys.Control + "a");
            txtLast.SendKeys(Keys.Backspace);

            // Escribimos el nuevo apellido
            txtLast.SendKeys(newLastName);
        }

        // ***************************************************************
        // MÉTODO EXCLUSIVO PARA LA PRUEBA NEGATIVA (Borrado profundo)
        // ***************************************************************

        public void ClearEmployeeData()
        {
            // 1. LIMPIAR FIRST NAME (Borrado agresivo letra por letra)
            var txtFirst = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(firstNameInput));
            txtFirst.Click();

            // Bucle: Mientras el campo tenga texto, presiona Backspace
            // Esto asegura que quede VACÍO sí o sí.
            string valorFirst = txtFirst.GetAttribute("value");
            for (int i = 0; i < valorFirst.Length; i++)
            {
                txtFirst.SendKeys(Keys.Backspace);
            }


            // 2. LIMPIAR LAST NAME (Borrado agresivo)
            var txtLast = _driver.FindElement(lastNameInput);
            txtLast.Click();

            string valorLast = txtLast.GetAttribute("value");
            for (int i = 0; i < valorLast.Length; i++)
            {
                txtLast.SendKeys(Keys.Backspace);
            }
        }

        // *************************************************************
        // NUEVOS ELEMENTOS PARA PRUEBAS DE LÍMITES (AGREGAR AL FINAL)
        // *************************************************************

        // Checkbox "Seleccionar Todo" (en el encabezado de la tabla)
        private By selectAllCheckbox = By.XPath("//div[@role='columnheader']//span[contains(@class, 'oxd-checkbox-input')]");

        // Botón "Delete Selected" (Aparece solo cuando seleccionas algo)
        private By bulkDeleteButton = By.XPath("//button[contains(., 'Delete Selected')]");

        // Método para saber si el botón de borrado masivo es visible
        public bool IsBulkDeleteButtonVisible()
        {
            try
            {
                return _driver.FindElement(bulkDeleteButton).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Método para marcar "Seleccionar Todo"
        public void ClickSelectAllCheckbox()
        {
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(selectAllCheckbox)).Click();
        }

        // Método para clicar el botón de borrado masivo
        public void ClickBulkDeleteButton()
        {
            _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(bulkDeleteButton)).Click();
        }
    }
    }
