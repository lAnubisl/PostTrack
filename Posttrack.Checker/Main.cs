using log4net;
using log4net.Config;
using Posttrack.BLL.Interfaces;
using Posttrack.DI;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Posttrack.Checker
{
    public class Checker
    {
        public static void Main()
        {
            ConfigureLog4Net();
            var log = LogManager.GetLogger(typeof(Checker));
            var service = InversionOfControlContainer.Instance.Resolve<IPackagePresentationService>();
            try
            {
                service.UpdateComingPackages();
            }
            catch(Exception ex)
            {
                log.Fatal(ex.Message + ex.StackTrace);
            }
        }

        private static void ConfigureLog4Net()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xml = new XmlDocument();
            xml.Load(Path.Combine(dir, "log4net.config"));
            XmlConfigurator.Configure(xml.DocumentElement);
        }
    }
}