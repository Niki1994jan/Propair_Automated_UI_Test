using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

class WizzairTest
{
    static void Main()
    {
        IWebDriver driver = new ChromeDriver();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

        try
        {
            driver.Navigate().GoToUrl("https://wizzair.com/hu-hu?force-country=HU&force-language=hu");
            driver.Manage().Window.Maximize();

            // Cookie popup bezárása (ha van)
            try
            {
                wait.Until(driver =>
                {
                    try
                    {
                        var element = driver.FindElement(By.Id("onetrust-accept-btn-handler"));
                        return (element != null && element.Displayed && element.Enabled) ? element : null;
                    }
                    catch (NoSuchElementException)
                    {
                        return null;
                    }
                })?.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Ha nincs cookie popup, továbblépünk
            }

            // Indulási város (Budapest)
            var departureInput = wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector("input[data-test='search-departure-station']"));
                    return (element != null && element.Displayed && element.Enabled) ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            departureInput.Click();
            departureInput.SendKeys("Budapest");

            //teszt - Budapest megjelenik-e?
            var departureResult = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.XPath("//mark[contains(text(),'Budapest')]"));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            departureResult.Click();

            // Érkezési város (London)
            var arrivalInput = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.CssSelector("input[data-test='search-arrival-station']"));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            arrivalInput.Click();
            arrivalInput.SendKeys("WAW"); //Varsó Chopin reptér

            //teszt = érk.város megjelenik-e?
            var arrivalResult = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.XPath("//mark[contains(text(),'WAW')]"));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            arrivalResult.Click();

            //Indulasi datum megadása - Selector: #wa-input-9; XPath: //*[@id="wa-input-9"]; JSPath: document.querySelector("#wa-input-9")
            var departureDate_Input = wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector("input[placeholder='Indulás']")); //Exception, hogyha az aria-describedby-ra kerestem, mert változik...
                    return (element != null && element.Displayed && element.Enabled) ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            departureDate_Input.Click();

            var departureDate_Calendar = wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector("span[aria-label='2025. július 24., csütörtök']"));
                    return (element != null && element.Displayed) ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            departureDate_Calendar.Click();

            //teszt - ellenőrzés?


            //Érkezési dátum megadása - happy path tesztelés:későbbi dátum megadása

            departureDate_Calendar = wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector("span[aria-label='2025. július 27., vasárnap']"));
                    return (element != null && element.Displayed) ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            departureDate_Calendar.Click();

            ////Utasok számának módosítása
            //var number_passanger = wait.Until(driver =>
            //{
            //    try
            //    {
            //        var el = driver.FindElement(By.CssSelector("input[data-test='flight-search-search-passenger']"));
            //        return (el != null && el.Displayed && el.Enabled) ? el : null;
            //    }
            //    catch (NoSuchElementException)
            //    {
            //        return null;
            //    }
            //});

            //hozzáadunk egy felnőttet
            var number_adult_button = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.XPath("//div[@data-test='adult-stepper']//button[@data-test='stepper-button-increase']"));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            number_adult_button.Click();

            //Hozzáadunk egy gyereket
            var number_child_button = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.XPath("//div[@data-test='stepper-ds']//button[@data-test='stepper-button-increase']"));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            number_child_button.Click();


            // Keresés indítása
            var searchButton = wait.Until(driver =>
            {
                try
                {
                    var el = driver.FindElement(By.CssSelector("button[data-test='flight-search-submit']"));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            searchButton.Click();

            // Eredményoldal betöltése (URL változás)
            wait.Until(driver => driver.Url.Contains("booking/select-flight"));

            // Találati lista UI elem ellenőrzése
            var flightElements = driver.FindElements(By.CssSelector("[data-test*='flight-select']"));

            // Listába mentjük (ha szükséges, például string-ként az elemek szövegét):
            var flightTexts = flightElements.Select(el => el.Text).ToList();

            // Egymás alá, sortöréssel kiírjuk:
            foreach (var flight in flightTexts)
            {
                Console.WriteLine(flight);
            }

            Console.WriteLine("✅ Keresési eredmények betöltve."); //elkezdett Critical error-t dobni...
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Hiba: " + ex.Message);
        }
        finally
        {
            Thread.Sleep(5000); // Csak hogy lássuk, mi történt
            driver.Quit();
        }
    }
}