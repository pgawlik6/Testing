using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using System.Threading;

namespace AlarmClockTests
{

    [TestClass]
    public class AlarmClockTests : AlarmClockSession
    {
        private const string NewAlarmName = "Alarm Testowy.";
        private const string AlarmInfo = "EDYTUJ ALARM, Alarm Testowy.,";
        private const string AlarmRepeat = "Powtórzenia, Poniedziałek, Środa, Sobota, wybrano dla powtarzalnego alarmu";

        [TestMethod]
        public void AlarmAdd()
        {
            // Przejście do okna tworzenia Nowego Alarmu
            Thread.Sleep(TimeSpan.FromSeconds(1));
            session.FindElementByAccessibilityId("AddAlarmButton").Click();

            // Ustawienie nazwy
            session.FindElementByAccessibilityId("AlarmNameTextBox").Clear();
            session.FindElementByAccessibilityId("AlarmNameTextBox").SendKeys(NewAlarmName);

            // Ustawienie godziny 
            WindowsElement hourSelector = session.FindElementByAccessibilityId("HourLoopingSelector");
            hourSelector.FindElementByName("10").Click();
            Assert.AreEqual("10", hourSelector.Text);

            // Ustawienie minut
            WindowsElement minuteSelector = session.FindElementByAccessibilityId("MinuteLoopingSelector");
            minuteSelector.FindElementByName("05").Click();
            Assert.AreEqual("05", minuteSelector.Text);


            // Ustawienie powtórzeń
            session.FindElementByAccessibilityId("AlarmRepeatsToggleButton").Click();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            session.FindElementByName("Poniedziałek").Click();
            session.FindElementByName("Środa").Click();
            session.FindElementByName("Sobota").Click();
            Thread.Sleep(TimeSpan.FromSeconds(3));
            session.FindElementByAccessibilityId("AlarmRepeatsToggleButton").Click();
            Assert.AreEqual(AlarmRepeat, session.FindElementByAccessibilityId("AlarmRepeatsToggleButton").Text);

            // Ustawianie dźwięku
            session.FindElementByAccessibilityId("AlarmSoundButton").Click();
            session.FindElementByName("Ksylofon").Click();
            Assert.AreEqual("Dźwięk, Ksylofon, ", session.FindElementByAccessibilityId("AlarmSoundButton").Text);

            // Ustawianie długości drzemki
            session.FindElementByAccessibilityId("AlarmSnoozeCombobox").Click();
            session.FindElementByName("20 minut").Click();
            Assert.AreEqual("20 minut", session.FindElementByAccessibilityId("AlarmSnoozeCombobox").Text);

            // Zapisanie konfiguracji nowego alarmu
            session.FindElementByAccessibilityId("AlarmSaveButton").Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            // Sprawdzenie czy nowy alarm ma odpowiednią godzinę, minuty, nazwę, dni w których alarm ma być powtarzany
            WindowsElement alarmEntry = session.FindElementByXPath($"//ListItem[starts-with(@Name, \"{AlarmInfo}\")]");
            Assert.IsNotNull(alarmEntry);
            Assert.IsTrue(alarmEntry.Text.Contains("10"));
            Assert.IsTrue(alarmEntry.Text.Contains("05"));
            Assert.IsTrue(alarmEntry.Text.Contains(NewAlarmName));
            Assert.IsTrue(alarmEntry.Text.Contains("Poniedziałek"));
            Assert.IsTrue(alarmEntry.Text.Contains("Środa"));
            Assert.IsTrue(alarmEntry.Text.Contains("Sobota"));


        }

        [TestMethod]
        public void AlarmSwitch()
        {
            WindowsElement alarmEntry = null;

            // Sprawdź, czy wcześniej utworzono wpis alarmu.W przeciwnym razie utwórz go, wywołując AlarmAdd()
            try
            {
                alarmEntry = session.FindElementByXPath($"//ListItem[starts-with(@Name, \"{AlarmInfo}\")]");
            }
            catch
            {
                AlarmAdd();
                alarmEntry = session.FindElementByXPath($"//ListItem[starts-with(@Name, \"{AlarmInfo}\")]");
            }
            // Sprawdzanie poprawności działania przełącznika i ostateczne wyłączenie go
            Assert.IsNotNull(alarmEntry);
            WindowsElement alarmEntryToggleSwitch = alarmEntry.FindElementByAccessibilityId("AlarmToggleSwitch") as WindowsElement;
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Assert.IsTrue(alarmEntryToggleSwitch.Selected); //sprawdzenie czy przełącznik jest włączony
            alarmEntryToggleSwitch.Click(); //klikniecie i wyłączenie alarmu
            Assert.IsFalse(alarmEntryToggleSwitch.Selected); //sprawdzenie czy przełącznik jest wyłączony
            Thread.Sleep(TimeSpan.FromSeconds(1));
            alarmEntryToggleSwitch.Click(); //klikniecie i ponowne włączenie alarmu
            Assert.IsTrue(alarmEntryToggleSwitch.Selected); //sprawdzenie czy przełącznik jest włączony
            Thread.Sleep(TimeSpan.FromSeconds(1));
            alarmEntryToggleSwitch.Click(); //klikniecie i ponowne wyłączenie alarmu
            Assert.IsFalse(alarmEntryToggleSwitch.Selected); //sprawdzenie czy przełącznik jest wyłączony
        }

        [TestMethod]
        public void AlarmDelete()
        {
            WindowsElement alarmEntry = null;

            // Sprawdzenie, czy wcześniej utworzono wpis alarmu. W przeciwnym razie tworzony jest poprzez wywołanie AlarmAdd()
            try
            {
                alarmEntry = session.FindElementByXPath($"//ListItem[starts-with(@Name, \"{AlarmInfo}\")]");
            }
            catch
            {
                AlarmAdd();
                alarmEntry = session.FindElementByXPath($"//ListItem[starts-with(@Name, \"{AlarmInfo}\")]");
            }

            Assert.IsNotNull(alarmEntry);
            touchScreen.LongPress(alarmEntry.Coordinates);
            session.FindElementByName("Usuń").Click();

        }


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            //Zakończenie sesji poprzez wywołanie testu z klasy podstawowej
            TearDown();
        }

        [TestInitialize]
        public override void TestInit()
        {
            // Wywołanie inicjalizacji testu klasy podstawowej, aby upewnić się, że aplikacja znajduje się na stronie głównej
            base.TestInit();

            // Przejście do zakładki Alarm
            session.FindElementByAccessibilityId("AlarmButton").Click();
        }
    }
}