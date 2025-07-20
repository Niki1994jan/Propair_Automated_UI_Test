using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;

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
            var departureResult = wait.Until(drv =>
            {
                try
                {
                    var el = drv.FindElement(By.XPath("//mark[contains(text(),'Budapest')]"));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            departureResult.Click();

            // Érkezési város (London)
            var arrivalInput = wait.Until(drv =>
            {
                try
                {
                    var el = drv.FindElement(By.CssSelector("input[data-test='search-arrival-station']"));
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
            var arrivalResult = wait.Until(drv =>
            {
                try
                {
                    var el = drv.FindElement(By.XPath("//mark[contains(text(),'WAW')]"));
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
                    var element = driver.FindElement(By.CssSelector("span[aria-label='2025. július 22., kedd']"));
                    return (element != null && element.Displayed) ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            //teszt - ellenőrzés?


            //Érkezési dátum megadása - happy path tesztelés:későbbi dátum megadása

            departureDate_Calendar = wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector("span[aria-label='2025. július 23., szerda']")); //itt elcrashel....
                    return (element != null && element.Displayed) ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            //Utasok számának módosítása


            // Keresés indítása
            var searchButton = wait.Until(drv =>
            {
                try
                {
                    var el = drv.FindElement(By.CssSelector("button[data-test='flight-search-go']"));
                    return (el != null && el.Displayed && el.Enabled) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            searchButton.Click();

            // Eredményoldal betöltése (URL változás)
            wait.Until(drv => drv.Url.Contains("search"));

            // Találati lista UI elem ellenőrzése
            var resultList = wait.Until(drv =>
            {
                try
                {
                    var el = drv.FindElement(By.CssSelector("div[data-test='flight-search-result']"));
                    return (el != null && el.Displayed) ? el : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            Console.WriteLine("✅ Keresési eredmények betöltve.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Hiba: " + ex.Message);
        }
        finally
        {
            Thread.Sleep(5000); // Csak hogy lásd, mi történt
            driver.Quit();
        }
    }
}