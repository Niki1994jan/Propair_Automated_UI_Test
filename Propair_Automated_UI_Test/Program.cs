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
            //kellene az ellenőrzéshez egy listát létrehozni, amibe belementjük a UI-on megjelenő adatokat
                var content_of_realTextboxes_check = new List<string>(7); //dinamikus lista is lehetne, hogy az utasok szamat annak megfelelően lehessen eltárolni.. habár egyszerűbb a 0,0-kat is elmenteni.
    
                //le kellene kerni, hogy valojaban mit tartalmaz az indulasi varos mezo - bar lehet, hogy inkabb a Search gomb megnyomasa elott kellene mindegyik mezo tartalmat checkolni
                var departureCityTextbox_content;
                content_of_realTextboxes_check.Add(departureCityTextbox_content);

            // Érkezési város - Varsó Chopin
            var arrivalInput = WaitAndGetElement(wait, driver, By.CssSelector("input[data-test='search-arrival-station']"));
            
            arrivalInput.Click();
            arrivalInput.SendKeys("WAW"); //Varsó Chopin reptér

                //teszt = érk.város megjelenik-e?
                var arrivalCityTextbox_content;
                content_of_realTextboxes_check.Add(arrivalCityTextbox_content);

            
            //Indulasi datum megadása - Selector: #wa-input-9; XPath: //*[@id="wa-input-9"]; JSPath: document.querySelector("#wa-input-9")
            var departureDate_Input = WaitAndGetElement(wait, driver, By.CssSelector("input[placeholder='Indulás']")); //Exception, hogyha az aria-describedby-ra kerestem, mert változik...
            departureDate_Input.Click();

            var departureDate_Calendar = WaitAndGetElement(wait, driver, By.CssSelector("span[aria-label='2025. július 24., csütörtök']"));
            departureDate_Calendar.Click();
                
            //Érkezési dátum megadása - happy path tesztelés:későbbi dátum megadása
            ////if - else -be megírni: ha van visszaút, vagy ha nincs retúr repjegy
            departureDate_Calendar = WaitAndGetElement(wait, driver, By.CssSelector("span[aria-label='2025. július 27., vasárnap']")); //azért kapta ugyanazt az elnevezést, mert nem kell új változót létrehozni
            departureDate_Calendar.Click();


                //teszt dátumok helyes mentése - ellenőrzés a listaba mentve
                content_of_realTextboxes_check.Add(arrivalCityTextbox_content);
                var arrivalDateCalendar_content;
                content_of_realTextboxes_check.Add(arrivalCityTextbox_content);
                var departureDateCalendar_content;
                content_of_realTextboxes_check.Add(arrivalCityTextbox_content);
            

            //Utasok számának módosítása - rögtön megnyílik a városok kiválasztása után

            //hozzáadunk egy felnőttet
            var number_adult_button = WaitAndGetElement(wait, driver, By.XPath("//div[@data-test='adult-stepper']//button[@data-test='stepper-button-increase']"));
            number_adult_button.Click();

            //Hozzáadunk egy gyereket
            var number_child_button = WaitAndGetElement(wait, driver, By.XPath("//div[@data-test='stepper-ds']//button[@data-test='stepper-button-increase']"));
            number_child_button.Click();


                //ellenorzes - lekerni az utasok számát + hozzáadni a listához (ez több elem!!)
                /*
                foreach (){
                    content_of_realTextboxes_check.Add(arrivalCityTextbox_content);
                }
                */
            
            // Keresés indítása
            var searchButton = WaitAndGetElement(wait, driver, By.CssSelector("button[data-test='flight-search-submit']"));
            searchButton.Click();

            // Eredményoldal betöltése (URL változás)
            wait.Until(driver => driver.Url.Contains("booking/select-flight")); //elkezdett Critical error-t dobni...

                //itt is kellene valamilyen ellenőrzés, hogy biztosan jó eredményoldal töltődött-e be

            // Találati lista UI elem ellenőrzése
            var flightElements = driver.FindElements(By.CssSelector("[data-test*='flight-select']"));

            // Listába mentjük (ha szükséges, például string-ként az elemek szövegét):
            var flightTexts = flightElements.Select(el => el.Text).ToList();

            // Egymás alá, sortöréssel kiírjuk:
            foreach (var flight in flightTexts)
            {
                Console.WriteLine(flight);
            }

            Console.WriteLine("✅ Keresési eredmények betöltve.");
            
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
