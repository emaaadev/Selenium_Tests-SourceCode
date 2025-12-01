using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumTest.Utils
{
    public static class ReportManager
    {
        private static ExtentReports _extent;
        private static ExtentSparkReporter _htmlReporter;

        public static ExtentReports GetInstance()
        {
            if (_extent == null)
            {
                // 1. RUTA FIJA (Sin fecha/hora en el nombre del archivo)
                // Esto es vital para que siempre busque el MISMO archivo y le agregue datos.
                string reportFolder = @"C:\Users\Emanu\Downloads\seleniumTest_orange-hrm\Selenium_Tests-SourceCode\SourceCode\OrangeHRM_SeleniumTest\tests_reports-logs";

                if (!Directory.Exists(reportFolder)) Directory.CreateDirectory(reportFolder);

              
                string reportPath = Path.Combine(reportFolder, "ReporteGeneral.html");

                _htmlReporter = new ExtentSparkReporter(reportPath);

                // CONFIGURACIÓN IMPORTANTE: 
                // Esto ayuda a que el reporte entienda que debe agregar info si el archivo existe.
                _htmlReporter.Config.Encoding = "utf-8";
                _htmlReporter.Config.DocumentTitle = "Reporte Automatizado";
                _htmlReporter.Config.ReportName = "Resultados de Pruebas";

                _extent = new ExtentReports();
                _extent.AttachReporter(_htmlReporter);
            }

            return _extent;
        }

        //Opcion 2: 
        //public static class ReportManager
        //{
        //    private static ExtentReports _extent;
        //    private static ExtentSparkReporter _htmlReporter;

        //    public static ExtentReports GetInstance()
        //    {
        //        if (_extent == null)
        //        {
        //            // USAMOS TU RUTA EXACTA (El @ al principio es importante para las barras)
        //            string reportFolder = @"C:\Users\Emanu\Downloads\seleniumTest_orange-hrm\Selenium_Tests-SourceCode\SourceCode\OrangeHRM_SeleniumTest\tests_reports-logs";

        //            // Crear carpeta si no existe
        //            if (!Directory.Exists(reportFolder))
        //                Directory.CreateDirectory(reportFolder);

        //            string reportPath = Path.Combine(reportFolder, "TestReport.html");

        //            _htmlReporter = new ExtentSparkReporter(reportPath);
        //            _extent = new ExtentReports();
        //            _extent.AttachReporter(_htmlReporter);
        //        }

        //        return _extent;
        //    }
        //}
    }
}
