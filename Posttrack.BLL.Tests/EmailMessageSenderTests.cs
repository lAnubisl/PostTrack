using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Posttrack.BLL.Properties;

namespace Posttrack.BLL.Tests
{
    /// <summary>
    /// Summary description for EmailMessageSenderTests
    /// </summary>
    [TestClass]
    public class EmailMessageSenderTests
    {
        public EmailMessageSenderTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            using (var message = new MailMessage(Settings.Default.SmtpFrom, "alexander.panfilenok@gmail.com"))
            {
                message.Subject = "test";
                message.Body = "test";
                message.IsBodyHtml = true;
                
                var smtpClient = new SmtpClient(Settings.Default.SmtpHost, Settings.Default.SmtpPort);
                smtpClient.Credentials = new NetworkCredential(Settings.Default.SmtpUser, "nothingspecial123");
                smtpClient.EnableSsl = Settings.Default.SmtpSecure;
                smtpClient.Timeout = 20000;
                smtpClient.Send(message);
            }
        }
    }
}
