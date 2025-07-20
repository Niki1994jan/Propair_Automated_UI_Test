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
                var cookieAcceptButton = WaitAndGetElement(wait, driver, By.Id("onetrust-accept-btn-handler"));
                cookieAcceptButton?.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Ha nincs cookie popup, továbblépünk
            }

            // Indulási város (Budapest)
            var departureInput = WaitAndGetElement(wait, driver, By.CssSelector("input[data-test='search-departure-station']"));

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

            // Érkezési város - Varsó Chopin
            var arrivalInput = WaitAndGetElement(wait, driver, By.CssSelector("input[data-test='search-arrival-station']"));
            
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
            var departureDate_Input = WaitAndGetElement(wait, driver, By.CssSelector("input[placeholder='Indulás']")); //Exception, hogyha az aria-describedby-ra kerestem, mert változik...
            departureDate_Input.Click();

            var departureDate_Calendar = WaitAndGetElement(wait, driver, By.CssSelector("span[aria-label='2025. július 24., csütörtök']"));
            departureDate_Calendar.Click();

            //teszt - ellenőrzés?


            //Érkezési dátum megadása - happy path tesztelés:későbbi dátum megadása

            departureDate_Calendar = WaitAndGetElement(wait, driver, By.CssSelector("span[aria-label='2025. július 27., vasárnap']"));
            departureDate_Calendar.Click();

            //Utasok számának módosítása - rögtön megnyílik a városok kiválasztása után

            //hozzáadunk egy felnőttet
            var number_adult_button = WaitAndGetElement(wait, driver, By.XPath("//div[@data-test='adult-stepper']//button[@data-test='stepper-button-increase']"));
            number_adult_button.Click();

            //Hozzáadunk egy gyereket
            var number_child_button = WaitAndGetElement(wait, driver, By.XPath("//div[@data-test='stepper-ds']//button[@data-test='stepper-button-increase']"));
            number_child_button.Click();


            // Keresés indítása
            var searchButton = WaitAndGetElement(wait, driver, By.CssSelector("button[data-test='flight-search-submit']"));
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

    public static IWebElement WaitAndGetElement(WebDriverWait wait, IWebDriver driver, By selector)
    {
        return wait.Until(drv =>
        {
            try
            {
                var el = drv.FindElement(selector);
                return (el != null && el.Displayed && el.Enabled) ? el : null;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        });
    }
}