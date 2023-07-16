using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System;
using OpenQA.Selenium.Firefox;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.IE;
using WebDriverManager.Helpers;
using System.Collections.Generic;

namespace ILR_TestSuite
{

   [TestFixture]

    public class TestBase

    {

        private ChromeOptions _chromeOptions;

        public IWebDriver _driver,_webDriver;
        

        private string _userName;

        private string _password;

        public string _screenShotFolder;

        public static int currentMethod { get; set; }

        public static string connectionString = @"Data Source='SRV007232, 1455';Initial Catalog=Automation;Integrated Security=True";
        public static SqlConnection connection = new SqlConnection(connectionString);
        public static SqlCommand command { get; set; }
        public string sqlString { get; set; }
        public static SqlDataReader reader { get; set; }



        [OneTimeSetUp]
        public void StartBrowser()
        {

            //  _driver = new ChromeDriver("C:/Code/bin");

            new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

            _chromeOptions = new ChromeOptions();
            _chromeOptions.AddArguments("--incognito");
            _chromeOptions.AddArguments("--ignore-certificate-errors");
            _driver = new ChromeDriver();
            
    
            //InternetExplorerOptions options = new InternetExplorerOptions();
            //options.IgnoreZoomLevel = true;
            //options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
            //_driver = new InternetExplorerDriver(options);

            _screenShotFolder = $@"{AppDomain.CurrentDomain.BaseDirectory}Failed_ScreenShots​{ScreenShotDailyFolderName()}​\";

            new DirectoryInfo(_screenShotFolder).Create();

            
        }
        public static void OpenDBConnection(string sqlCmnd)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
           command = new SqlCommand(sqlCmnd, connection);
        }

        public string SetproductName()
        {
            var product = _driver.FindElement(By.XPath("//*[@id='CntContentsDiv5']/table/tbody/tr[1]/td[2]")).Text;

            try
            {
                var cmd = $"UPDATE PS_Scenarios SET productName = @product WHERE FunctionID = {currentMethod}";
                OpenDBConnection(cmd);
                command.Parameters.AddWithValue("@product", product);
                command.ExecuteNonQuery();




                return product;



            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }




        }

        public static int  getFuctionID(string funcName)
        {
            int id = 0;
            try
            {
                OpenDBConnection("SELECT ID FROM Functions WHERE function_name = '" + funcName + "'");
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    id = (int)reader["ID"];

                }

                connection.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.ToString());

            }
            return id;
        }
        public static string getFuncName(int id)
        {
            string funcName;
            funcName = String.Empty;

                try
                {
                   OpenDBConnection("SELECT function_name FROM Functions WHERE ID =" + id);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        funcName = reader["function_name"].ToString();

                    }
                    connection.Close();

            }

                catch (Exception ex)
                {
                    Console.WriteLine("Exception:" + ex.ToString());

                }
            

            return funcName;
        }

        private static string ScreenShotDailyFolderName()

        {

            return DateTime.Now.ToString("yyyyMMdd").Replace("AM", string.Empty).Replace("PM", string.Empty);

        }

        public void TakeScreenshot(string fileName)
        {
            var filePath = $@"{_screenShotFolder}\Failed_Scenarios\";

            if (!Directory.Exists(filePath))

                new DirectoryInfo(filePath).Create();


            ITakesScreenshot ssdriver = _driver as ITakesScreenshot;

            Screenshot screenshot = ssdriver.GetScreenshot();

            fileName = $"{fileName}{ScreenShotTime()}.png";

            screenshot.SaveAsFile($"{filePath}{fileName}", ScreenshotImageFormat.Png);

        }

        private static string ScreenShotTime()

        {

            return DateTime.Now.TimeOfDay.ToString().Replace(":", "_").Replace(".", "_");

        }

        public IWebDriver SiteConnection()

        {
         

            _driver.Url = "http://ilr-tst.safrican.co.za/web/wspd_cgi.sh/WService=wsb_ilrtst/run.w";

           // IAlert alert = _driver.SwitchTo().Alert();
          //  alert.Dismiss();

            _userName = "G992092";//TODO add your user name and password


            _password = "G992092saftst";

            _driver.Manage().Window.Maximize();

            System.Threading.Thread.Sleep(2000);

       
            System.Threading.Thread.Sleep(2000);

            IWebElement loginTextBox = _driver.FindElement(By.XPath("/html/body/center/center/form[2]/div/table/tbody/tr/td/span/table/tbody/tr[2]/td/div/center/div/table/tbody/tr[4]/td[2]/span/table/tbody/tr[2]/td[2]/input"));
            IWebElement passwordTextBox = _driver.FindElement(By.CssSelector("#CntContentsDiv2 > table > tbody > tr:nth-child(3) > td:nth-child(2) > input[type=password]"));
            IWebElement loginBtn = _driver.FindElement(By.CssSelector("#GBLbl-1 > span > a"));
            loginTextBox.SendKeys(_userName);
            System.Threading.Thread.Sleep(6000);
           passwordTextBox.SendKeys(_password);

            System.Threading.Thread.Sleep(4000);
            loginBtn.Click();
            System.Threading.Thread.Sleep(2000);
            return _driver;
        }

        public Decimal getPremuimFromRateTable(string idNumber, string rolePlayer, string sumAsured, string product)
        {
            var premium = String.Empty;
            var age = String.Empty;
            //Calculate age based on IdNo
            var thisYear = DateTime.Now.Year.ToString().Substring(2);
            thisYear = DateTime.Now.Year.ToString();
            var id_year = Int32.Parse(idNumber.Substring(0, 2));
            if (id_year >= 00 && id_year <= Int32.Parse(DateTime.Now.Year.ToString().Substring(2)))
            {
                age = (DateTime.Now.Year - Int32.Parse("200" + id_year)).ToString();
            }
            else
            {
                age = (DateTime.Now.Year - Int32.Parse("19" + id_year)).ToString();
            }
            //Get product name for the rate table
            switch (product.Trim())
            {
                case "Safrican Serenity Funeral Premium (1000)":
                    product = "Serenity_Premium";
                    break;
                case "Safrican Serenity Funeral (2000)":
                    product = "Safrican_Serenity_Funeral";
                    break;
                case "Safrican Just Funeral (3000)":
                    product = "Safrican_Just_Funeral";
                    break;
                default:
                    break;
            }
            //Get roleplayer ref for DB table
            if ((rolePlayer.Trim()).Contains("Parent"))
            {
                rolePlayer = "Parent";
            }
            else if ((rolePlayer.Trim()).Contains("Child"))
            {
                rolePlayer = "Children";
            }
            else if ((rolePlayer.Trim()).Contains("Spouse"))
            {
                rolePlayer = "Spouse";
            }
            else if ((rolePlayer.Trim()).Contains("Wider"))
            {
                rolePlayer = "Extended";
            }
            else
            {
                rolePlayer = "ML";
            }
            var cover = rolePlayer + "_" + sumAsured;
            OpenDBConnection($"SELECT {cover} FROM {product} WHERE AGE = " + age);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                premium = reader[cover].ToString();
            }
            connection.Close();
            return Convert.ToDecimal(premium);
        }

        public void writeResultsToDB(string results , int scenario_id, string comments)
        {

                    OpenDBConnection($"UPDATE PS_Scenarios SET Test_Results = @results, Test_Date =@testDate, Comments = @comments WHERE ID = {scenario_id}");
                    var testDate = DateTime.Now.ToString();
                    command.Parameters.AddWithValue("@results",results);
                    command.Parameters.AddWithValue("@testDate", testDate);
                    command.Parameters.AddWithValue("@comments", comments);
                    command.ExecuteNonQuery();
        }


        public void DisconnectBrowser()

        {

            if (_driver != null)

                _driver.Quit();

        }



        public string GetSeleniumFormatTag(string inputControlName)

        {

            var result = $"//*[@id=\"{inputControlName}\"]";

            return result;

        }



        public void Delay(int delaySeconds)

        {

            Thread.Sleep(delaySeconds * 1000);

        }


        public static IEnumerable<string[]> GetTestData(string methodName)
        {
            var conractRef = String.Empty;
            var scenarioID = String.Empty;
            int id = getFuctionID(methodName);
            OpenDBConnection($"SELECT PolicyNo,ID FROM PS_Scenarios WHERE FunctionID = {id}");
            reader = command.ExecuteReader();
          
            while (reader.Read())
            {
                if (!reader.HasRows)
                {
                    break;
                }
                scenarioID = reader["ID"].ToString();
                conractRef = reader["PolicyNo"].ToString();
                yield return new[] { conractRef,scenarioID };

            }
                connection.Close();
            
        }
    }

}