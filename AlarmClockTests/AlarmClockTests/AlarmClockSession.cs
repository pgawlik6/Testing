using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace AlarmClockTests
{
    public class AlarmClockSession
    {

        //private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string AlarmClockAppId = "Microsoft.WindowsAlarms_8wekyb3d8bbwe!App";

        protected static WindowsDriver<WindowsElement> session;
        protected static RemoteTouchScreen touchScreen;

        public static void Setup(TestContext context)
        {
            // Uruchom aplikację jeżeli jeszcze nie została uruchomiona
            if (session == null || touchScreen == null)
            {
                TearDown();

                //Tworzenie sesji

                Process.Start(@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe");

                AppiumOptions options = new AppiumOptions();
                options.AddAdditionalCapability("deviceName", "WindowsPC");
                options.AddAdditionalCapability("platformName", "Windows");
                options.AddAdditionalCapability("app", "Microsoft.WindowsAlarms_8wekyb3d8bbwe!App");

                session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), options);

                Assert.IsNotNull(session);
                Assert.IsNotNull(session.SessionId);
                Assert.AreEqual("Alarmy i zegar", session.FindElementByName("Alarmy i zegar").Text);

                // Ustawienie domyślnego limitu czasu na 1.5 sekundy, aby wyszukiwanie elementu powtarzało się co 500mili sekund
                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);

                touchScreen = new RemoteTouchScreen(session);
                Assert.IsNotNull(touchScreen);
            }
        }

        public static void TearDown()
        {
            // Wyczyszczenie obiektu RemoteTouchScreen jeżeli został zainicjalizowany
            touchScreen = null;

            // Zamknięcie aplikacji i zakończenie sesji
            if (session != null)
            {
                session.Quit();
                session = null;
            }

        }

        [TestInitialize]
        public virtual void TestInit()
        {
            WindowsElement alarmButtonElement = null;

            // Próba wrócenia do strony głównej w przypadku uruchomienia aplikacji Alarmy i zegar w oknie Edycja Alarmu
            try
            {
                alarmButtonElement = session.FindElementByAccessibilityId("AlarmButton");
            }
            catch
            {
                // Kliknięcie przycisku Anuluj, jeśli aplikacja znajduje się na zagnieżdżonej stronie np. "Nowy alarm"
                session.FindElementByAccessibilityId("CancelButton").Click();
                Thread.Sleep(TimeSpan.FromSeconds(1));
                alarmButtonElement = session.FindElementByAccessibilityId("AlarmButton");
            }

            // Sprawdzenie czy aplikacja znajduje się w głównym oknie
            Assert.IsNotNull(alarmButtonElement);
            Assert.IsTrue(alarmButtonElement.Displayed);
        }
    }
}