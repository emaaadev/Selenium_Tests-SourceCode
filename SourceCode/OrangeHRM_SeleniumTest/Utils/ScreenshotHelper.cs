using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumTest.Utils
{
    public static class ScreenshotHelper
    {
        public static string TakeScreenshot(IWebDriver driver, string testName)
        {
            // 1. Definimos la carpeta base (TU RUTA EXACTA)
            string baseFolder = @"C:\Users\Emanu\Downloads\seleniumTest_orange-hrm\Selenium_Tests-SourceCode\SourceCode\OrangeHRM_SeleniumTest\tests_reports-logs";

            // 2. Definimos la carpeta de capturas dentro de esa ruta
            string screenshotFolder = Path.Combine(baseFolder, "Screenshots");

            // Crear carpeta si no existe
            if (!Directory.Exists(screenshotFolder))
                Directory.CreateDirectory(screenshotFolder);

            // Nombre del archivo
            string filePath = Path.Combine(screenshotFolder, testName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");

            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            ss.SaveAsFile(filePath);

            return filePath;
        }
    }
}
