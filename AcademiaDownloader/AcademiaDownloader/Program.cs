using System;
using System.IO;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using System.Threading;


namespace AcademiaDownloader
{
    class Program
    {

        static void Main(string[] args)
        {
            Launcher launcher = new Launcher();
            launcher.Start();
            UI ui = new UI();


        }

    }

    class UI
    {
        public static Action<int> AonAddMaxCountPages;
        public static Action<string> AonChangeGroup;
        public static Action<string> AonChangePathToDriver;
        public static Action<string> AonChangePathToParser;
        public static Action<string> AonChangeLogin;
        public static Action<string> AonChangePass;
        public static Action<string> AonChangeTypeDownload;

        public static Action<int> AStartParser;
        public static Action<int> AStopParser;

        private int maxCountImages;
        private string driverPath;
        private string parserPath;
        private string grouptUrl;
        private string vkLogin;
        private string vkPass;
        private string typeDownload;

        private string stateDriver = "-";
        private string statePath = "-";
        private string stateGroup = "-";
        private string stateLogin = "-";
        private string statePass = "-";


        public UI()
        {
            Start();
        }

        private void Start()
        {
            Console.Clear();
            ShowMainMenu();
        }

        #region Menu Add Group
        private void ShowMenuAddGroup()
        {
            Console.WriteLine("---@ Меню настрек параметров @--- ");
            CheckInputMenuAddGroup();
        }
        private bool CheckInputMenuAddGroup()
        {
            Console.Write("--@ url группы в VK [000 - выйти]: ");
            var input = Console.ReadLine();

            if (input == "000")
            {
                return true;
            }
            else
            {
                ChangeGroup(input);
                return true;
            }
        }
        private void ChangeGroup(string url)
        {
            stateGroup = "+";
            grouptUrl = url;
            AonChangeGroup?.Invoke(url);
        }

        #endregion

        #region Menu Add Max Count Images
        private void ShowMenuAddMaxCountImages()
        {
            Console.WriteLine("---@ Меню настрек параметров @--- ");
            while (!CheckInputMenuMaxCountImages())
            {
            }
        }

        private bool CheckInputMenuMaxCountImages()
        {
            Console.Write("--@ Максимальное кол-во загруженных картинок [000 - выйти, 0 - все]: ");
            var input = Console.ReadLine();

            if (input == "000")
            {
                return true;
            }
            else if (Int32.TryParse(input, out var value) && value > -1)
            {
                ChangeMaxCountImages(value);
                return true;
            }
            else
            {
                Console.WriteLine("--@ Не правильный ввод");
                return false;
            }
        }
        private void ChangeMaxCountImages(int value)
        {
            maxCountImages = value;
            AonAddMaxCountPages?.Invoke(value);
        }

        #endregion

        #region Menu add path driver

        private void ShowMenuAddPathDriver()
        {
            Console.WriteLine("---@ Меню настрек параметров @--- ");
            CheckInputMenuAddPathDriver();
        }


        private bool CheckInputMenuAddPathDriver()
        {
            Console.Write("--@ Путь до драйвера \nПример: C:\\ \n---@: ");
            var input = Console.ReadLine();

            ChangePathDriver(input);
            return true;
        }
        private void ChangePathDriver(string path)
        {
            stateDriver = "+";
            driverPath = path;
            AonChangePathToDriver?.Invoke(path);
        }

        #endregion

        #region Menu add path to parser
        private void ShowMenuAddPathParser()
        {
            Console.WriteLine("---@ Меню настрек параметров @--- ");
            CheckInputMenuAddPathParser();
        }


        private bool CheckInputMenuAddPathParser()
        {
            Console.Write("--@ Путь до папки куда будут скидывать файлы\nПример: C:\\Folder\\ \n---@: ");
            var input = Console.ReadLine();

            ChangePathParser(input);
            return true;
        }
        private void ChangePathParser(string path)
        {
            statePath = "+";
            parserPath = path;
            AonChangePathToParser?.Invoke(path);
        }
        #endregion

        #region Menu info

        private void ShowMenuInfo()
        {
            Console.WriteLine("+----------| INFO |---------+");
            Console.WriteLine("Для работы требуется chromedriver.exe");
            Console.WriteLine("Для работы требуется Браузер Chrome");
            Console.WriteLine("Версия chromedriver.exe и версия Chrome должны совпадать.");
            Console.WriteLine("-----------------------------");
            CheckInputMenuInfo();
        }

        private bool CheckInputMenuInfo()
        {
            Console.Write("--@ ");
            var input = Console.ReadLine();

            return true;
        }

        #endregion

        #region Menu login
        private void ShowMenuLogin()
        {
            Console.WriteLine("---@ Меню настрек параметров @--- ");
            CheckInputMenuLogin();
        }


        private bool CheckInputMenuLogin()
        {
            Console.Write("--@ Логин VK---@: ");
            var input = Console.ReadLine();

            ChangeLogin(input);
            return true;
        }
        private void ChangeLogin(string login)
        {
            vkLogin = login;
            stateLogin = "+";
            AonChangeLogin?.Invoke(login);
        }
        #endregion

        #region Menu Pass
        private void ShowMenuPass()
        {
            Console.WriteLine("---@ Меню настрек параметров @--- ");
            CheckInputMenuPass();
        }


        private bool CheckInputMenuPass()
        {
            Console.Write("--@ Пароль VK---@: ");
            var input = Console.ReadLine();

            ChangePass(input);
            return true;
        }
        private void ChangePass(string pass)
        {
            vkPass = pass;
            statePass = "+";
            AonChangePass?.Invoke(pass);
        }
        #endregion

        #region Menu Type Download
        private void ShowMenuTypeDownload()
        {
            Console.WriteLine("---@ Меню настрек параметров @--- ");
            CheckInputMenuTypeDownload();
        }

        private bool CheckInputMenuTypeDownload()
        {
            Console.WriteLine("--@ Тип загрузки VK---@: ");
            Console.WriteLine("--! 1 - загрузка с постов");
            Console.WriteLine("--! 2 - загрузка с альбома");
            Console.Write("--$ ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ChangeTypeDownload("пост");
                    break;
                case "2":
                    ChangeTypeDownload("альбом");
                    break;
                default:
                    ChangeTypeDownload("пост");
                    break;
            }

            return true;
        }
        private void ChangeTypeDownload(string type)
        {
            typeDownload = type;
            AonChangeTypeDownload?.Invoke(type);
        }

        #endregion

        private void ShowMainMenu()
        {

            while (true)
            {

                ShowLogo();
                Console.WriteLine();
                Console.WriteLine("|------------------------------------------------------------------------------------------------------------|");
                Console.WriteLine("---!!! Внимание, при запуске парсинга группы которая УЖЕ ЕСТЬ в папке, то её файлы БУДУТ ПЕРЕЗАПИСАНЫ! !!!---");
                Console.WriteLine("|------------------------------------------------------------------------------------------------------------|");
                Console.WriteLine("|                                                                                                            |");
                Console.WriteLine($"---@ [1] Изменить максимальное кол-во загрузок                     [{maxCountImages}]");
                Console.WriteLine($"---@ [2] Изменить группу откуда загружать                          [{stateGroup}] | {grouptUrl}");
                Console.WriteLine($"---@ [3] Изменить путь до драйвера Chrome                          [{stateDriver}] | {driverPath}");
                Console.WriteLine($"---@ [4] Изменить путь до папки куда будут закидывать папки парсер [{statePath}] | {parserPath}");
                Console.WriteLine($"---@ [5] Изменить тип загрузки                                         | {typeDownload}");
                Console.WriteLine($"---@ [6] Логин VK                                                  [{stateLogin}] | " + vkLogin);
                Console.WriteLine($"---@ [7] Пароль VK                                                 [{statePass}] | " + vkPass);
                Console.WriteLine("----|--------------------------------------------------------------------------------------------------------|");
                Console.WriteLine($"---@ [8] Запустить ");
                Console.WriteLine($"---@ [9] Остановить");
                Console.WriteLine($"---@ [10] Информация");
                Console.Write("---! ");
                var command = Console.ReadLine();
                CheckCommandMainMenu(command);
            }
        }

        private void CheckCommandMainMenu(string command)
        {
            switch (command)
            {
                case "1":
                    ShowMenuAddMaxCountImages();
                    break;
                case "2":
                    ShowMenuAddGroup();
                    break;
                case "3":
                    ShowMenuAddPathDriver();
                    break;
                case "4":
                    ShowMenuAddPathParser();
                    break;
                case "5":
                    ShowMenuTypeDownload();
                    break;
                case "6":
                    ShowMenuLogin();
                    break;
                case "7":
                    ShowMenuPass();
                    break;
                case "8":
                    StartParser();
                    break;
                case "9":
                    StopParser();
                    break;
                case "10":
                    ShowMenuInfo();
                    break;
                default:
                    Console.WriteLine("---% Такой команды нет");
                    break;
            }
        }

        private void StartParser()
        {
            Console.WriteLine("---$ Парсер запущен");
            AStartParser?.Invoke(0);
        }

        private void StopParser()
        {
            Console.WriteLine("---$ Парсер выключен");
            AStopParser?.Invoke(0);
        }

        public static void ShowMessage(string message)
        {
            Console.WriteLine("----^% Сообшение: " + message);
        }

        public static void ShowError(string error)
        {
            Console.WriteLine("----^% Ошибка: " + error);
        }

        private void ShowLogo()
        {
            Console.WriteLine("                 +-----------------------------------------+");
            Console.WriteLine("                 +---------------| PARSER |----------------+");
            Console.WriteLine("                 +-----------------------------------------+");
        }
    }

    class Launcher
    {
        private string urlGroups;
        private string driverPath;
        private string pathParser;

        private string login;
        private string pass;

        private string typeDownload;

        private Parser runnedParser;
        private Thread activeTheard;
        public void Start()
        {
            UI.AonChangeGroup += SetGroups;
            UI.AonChangePathToDriver += SetDriverPath;
            UI.AonChangePathToParser += SetParserPath;
            UI.AStartParser += LaunchParsers;
            UI.AStopParser += StopParsers;
            UI.AonChangeLogin += SetLogin;
            UI.AonChangePass += SetPass;
            UI.AonChangeTypeDownload += SetTypeDownload;
        }

        public void LaunchParsers(int value)
        {
            if (runnedParser == null)
            {
                Parser parser = new Parser(urlGroups, pathParser, driverPath, login, pass, typeDownload);
                Parser.ASendError += UI.ShowError;
                Parser.ASendMessage += UI.ShowMessage;
                Parser.AOnStopParser += StopParsers;

                var thread = new Thread(new ThreadStart(parser.Start));

                activeTheard = thread;
                runnedParser = parser;
                activeTheard.Start();
            }
        }

        public void StopParsers(int value)
        {

            Parser.ASendError -= UI.ShowError;
            Parser.ASendMessage -= UI.ShowMessage;

            runnedParser.Stop(0);
            try
            {
                Thread.Sleep(200);
            }
            catch { }
            if (activeTheard != null)
            {
                activeTheard.Interrupt();
            }
            activeTheard = null;
            runnedParser = null;
        }

        public void StopParsers()
        {

            Parser.ASendError -= UI.ShowError;
            Parser.ASendMessage -= UI.ShowMessage;

            try
            {
                Thread.Sleep(200);
            }
            catch { }
            if (activeTheard != null)
            {
                activeTheard.Interrupt();
            }
            activeTheard = null;
            runnedParser = null;
        }

        public void SetGroups(string url)
        {
            urlGroups = url;
        }

        public void SetDriverPath(string path)
        {
            driverPath = path;
        }

        public void SetParserPath(string path)
        {
            pathParser = path;
        }

        public void SetLogin(string login)
        {
            this.login = login;
        }

        public void SetPass(string pass)
        {
            this.pass = pass;
        }

        public void SetTypeDownload(string type)
        {
            typeDownload = type;
        }
    }

    class Parser
    {
        public static Action<string> ASendError;
        public static Action<string> ASendMessage;

        public delegate void Action();
        public static event Action AOnStopParser;

        public static bool Update = true;

        private IWebDriver driver;
        private IReadOnlyCollection<IWebElement> posts;

        private int maxCountImages = 0;
        private string urlGroup;
        private string pathParser;
        private string pathDriver;
        private string pathFolder;

        private string login = "+79044507796";
        private string pass = "";

        private bool stop = false;
        private string subString = "";

        private string typeDownload = "поле";

        public Parser(string urlGroup, string pathParser, string pathDriver, string login, string pass, string downloadFromAllImages)
        {
            UI.AStopParser += Stop;

            this.urlGroup = urlGroup;
            this.pathDriver = pathDriver;
            this.pathParser = pathParser;
            this.login = login;
            this.pass = pass;
            this.typeDownload = downloadFromAllImages;
        }


        public void Start()
        {
            stop = false;
            if (CheckSettings())
            {
                ParsePage();
            }
        }

        public void SetPathToParser(string path)
        {
            pathParser = path;
        }

        public void SetPathToDriver(string path)
        {
            pathDriver = path;
        }

        public void SendError(string error)
        {
            ASendError?.Invoke(error);
        }

        public void SendMessage(string message)
        {
            ASendMessage?.Invoke(message);
        }

        private bool CheckSettings()
        {
            if ((urlGroup == "" || urlGroup == null) ||  (pathDriver == ""|| pathDriver == null) || (pathParser == "" || pathParser == null) || (login == "" || login == null) || (pass == "" || pass == null))
            {
                SendError("Что-то из этого не установленны:\n" +
                    $"--| Cсылка на группу\n" +
                    $"--| Путь до драйвера Chrome\n" +
                    $"--| Путь до папки куда будут закидываться папки\n" +
                    $"--| Логин\n" +
                    $"--| Пароль");
                Stop(0);
                return false;
            }
            else
            {
                return true;
            }

        }

        public void Stop(int value)
        {
            stop = true;
            if (driver != null)
            {
                driver.Quit();
            }
            ASendMessage?.Invoke("Парсер закончил работу");
            AOnStopParser?.Invoke();

        }

        public void ChangeMaxCountImages(int value)
        {
            maxCountImages = value;
        }

        public void ChangeUrlGroup(string url)
        {
            urlGroup = url;
        }

        private void ParsePage()
        {
            SendMessage(" Идёт попытка создать папку парсера...");
            pathParser = CreateFolder(pathParser);
            if (pathParser == "")
            {
                SendError("Из-за ошибки при создании папки парсера, парсер остановлен!");
                Stop(0);
            }
            SendMessage(" Папка была успешна получена!");

            SendMessage(" Идёт попытка запустить драйвер браузера...");
            driver = CreateDriver(pathDriver);
            if (driver == null)
            {
                SendError("Из-за ошибки при запуске драйвера, парсер остановлен");
                Stop(0);
                return;
            }
            SendMessage(" Драйвер запушен...");

            SendMessage(" [1/2] Идёт открытие страницы: " + urlGroup);
            if (!SetPage(ref driver, urlGroup))
            {
                SendError("Из-за ошибки при загрузки страницы, парсер остановлен");
                Stop(0);
                return;
            }
            SendMessage(" [1/2] Страница успешно открыта");
            TryAccept18();

            SendMessage(" Идёт попытка зайти в аккаунт");
            LoginOnSite(driver);

            SendMessage(" Скорее всего программа вошла в аккаунт...");

            TryAccept18();
            Thread.Sleep(2000);

            SendMessage(" [2/2] Идёт открытие страницы: " + urlGroup);
            if (!SetPage(ref driver, urlGroup))
            {
                SendError("Из-за ошибки при загрузки страницы, парсер остановлен");
                Stop(0);
                return;
            }
            SendMessage(" [2/2] Страница успешно открыта");
            Thread.Sleep(600);

            if (typeDownload == "альбом")
            {
                SendMessage(" Запущен режим загрузки с альбома");
                DownloadFromAlbums();
            }
            else
            {
                SendMessage(" Запущен режим загрузки с постов");
                DownloadFromPosts();
            }
        }

        private void DownloadFromAlbums()
        {
            SendMessage(" Идёт получение имени папки");
            var nameFolder = GetNameFolder(urlGroup) + subString;
            if (nameFolder == "")
            {
                SendError("Из-за ошибки при получении имени папки из URL, парсер остановлен");
                return;
            }
            SendMessage(" Имя папку успешно получена");

            SendMessage(" Идёт получение первого изображения с альбома");
            IWebElement imageDiv = null;
            ScrollPageWidht(ref driver, 16);
            try
            {
                 imageDiv = driver.FindElements(By.XPath("//*[@aria-label=\"Фотография\"]"))[0];
            }
            catch
            {
                SendError("Произошла ошибка при получении первого изображения ");
                Stop(0);
            }

            SendMessage(" Идёт процесс создания папки альбома/группы");
            pathFolder = CreateFolder(pathParser + nameFolder);
            if (pathFolder == "")
            {
                SendError("Из-за ошибки при создании папки группы/альбома, парсер остановлен!");
                Stop(0);
            }
            SendMessage(" Папка альбома/группы успещно созданна");

            SendMessage(" Запущен процесс скачивания фотографий");
            Actions action = new Actions(driver);
            action.MoveToElement(imageDiv).Click().Perform();

            int index = 0;
            do
            {
                Thread.Sleep(300);

                var url = GetURL();
                var name = GetNameImage(url) + ".png";
                var path = @$"{pathFolder}\{index}_{name}";


                if (!DownloadFile(url, path))
                {
                    SendError("Из-за ошибки при загрузке файла, изображение пропущенно");

                }

                index++;
                action = new Actions(driver);
                action.SendKeys(Keys.ArrowRight).Build().Perform();

                Thread.Sleep(300);

            }
            while (!stop && index <= maxCountImages);
            Stop(0);
        }

        private void DownloadFromPosts()
        {
            SendMessage(" Идёт получение имени папки");
            var nameFolder = GetNameFolder(urlGroup) + subString;
            if (nameFolder == "")
            {
                SendError("Из-за ошибки при получении имени папки из URL, парсер остановлен");
                return;
            }
            SendMessage(" Имя папку успешно получена");

            SendMessage(" Идёт получение постов в группе");
            posts = driver.FindElements(By.CssSelector("div[class='page_post_sized_thumbs  clear_fix']"));
            if (posts.Count == 0)
            {
                SendMessage("Количество постов: 0");
                SendMessage("Проверьте URL группы, логин и пароль от VK");
            }
            else
            {
                SendMessage(" Посты получены");
                pathFolder = CreateFolder(pathParser + nameFolder);
                if (pathFolder == "")
                {
                    SendError("Из-за ошибки при создании папки группы, парсер остановлен!");
                    Stop(0);
                }
            }
            SendMessage(" Запуск процесса скачивания фотографий");
            int index = 0;
            do
            {
                foreach (var post in posts)
                {
                    if (stop) break;
                    var images = GetImages(post, "a[aria-label='фотография']");
                    if (images == null)
                    {
                        SendError("Из-за ошибки получения картинок, парсер остановлен");
                        return;
                    }

                    foreach (var item in images)
                    {
                        if (stop) break;
                        if (maxCountImages == 0 || index <= maxCountImages)
                        {
                            try
                            {
                                item.Click();
                                var url = GetURL();
                                var name = GetNameImage(url) + ".jpg";
                                var path = @$"{pathFolder}\{index}_{name}";


                                if (!DownloadFile(url, path))
                                {
                                    SendError("Из-за ошибки при загрузке файла, изображение пропущенно");

                                }

                                index++;

                                driver.Navigate().Back();
                            }
                            catch { }
                        }
                        else
                        {
                            return;
                        }
                    }

                }
                ScrollPageHeight(ref driver);
                Thread.Sleep(200);
                posts = GetPosts(driver, "div[class='page_post_sized_thumbs  clear_fix']");
                ScrollPageHeight(ref driver);
            }
            while (!stop && posts != null && posts.Count > 0);
            Stop(0);
        }
        private void TryAccept18()
        {
            try
            {
                var layer = driver.FindElement(By.Id("box_layer_wrap"));

                var checkBox = layer.FindElement(By.XPath("//*[@id=\"box_layer\"]/div[2]/div/div[2]/div/div"));
                checkBox.Click();

                var buttonAccept = layer.FindElement(By.XPath("//*[@id=\"box_layer\"]/div[2]/div/div[3]/div[1]/table/tbody/tr/td[1]/button"));
                buttonAccept.Click();
                subString = "18plus";
            }
            catch
            {

            }
            Thread.Sleep(200);
        }

        private void LoginOnSite(IWebDriver webDriver)
        {
            if (login != "" && pass != "")
            {
                try
                {
                    IWebElement passwordField;
                    IWebElement loginFiled;

                    try
                    {
                        passwordField = webDriver.FindElement(By.CssSelector("input[id='pass']"));
                        loginFiled = webDriver.FindElement(By.CssSelector("input[id='email']"));
                    }
                    catch (NoSuchElementException)
                    {
                        passwordField = webDriver.FindElement(By.CssSelector("input[id='quick_pass']"));
                        loginFiled = webDriver.FindElement(By.CssSelector("input[id='quick_email']"));
                    }

                    IWebElement buttonLogin = null;
                    try
                    {
                        buttonLogin = webDriver.FindElement(By.CssSelector("button[id='login_button']"));
                    }
                    catch { }

                    if (buttonLogin == null)
                    {
                        buttonLogin = webDriver.FindElement(By.CssSelector("button[id='quick_login_button']"));
                    }
                    try
                    {
                        passwordField.SendKeys(pass);
                        loginFiled.SendKeys(login);
                        buttonLogin.Click();
                        Thread.Sleep(600);
                    }
                    catch (Exception error)
                    {
                        SendError(error.Message);
                        SendError("Произошла ошибка при вводе текста, проверье логин и пароль");
                    }
                }
                catch (NoSuchElementException)
                {
                    SendError("Ошибка при регистрации. Не найдены поля ввода");
                }
            }
            else
            {
                SendMessage(" Нет учётных данных, процесс входа в аккаунт пропущен");
            }
        }

        private string GetNameFolder(string url)
        {
            try
            {
                string nameFolder = url.Split("vk.com/")[1];
                return nameFolder;
            }
            catch (Exception error)
            {
                SendError(error.Message);
                return "";
            }
        }

        private string CreateFolder(string localPath)
        {
            try
            {
                if (!Directory.Exists(localPath))
                {
                    var info = Directory.CreateDirectory(localPath);
                    return info.FullName;
                }
                else
                {
                    return localPath;
                }
            }
            catch (Exception error)
            {
                SendError(error.Message);
                return "";
            }
        }

        private IReadOnlyCollection<IWebElement> GetImages(IWebElement post, string filter)
        {
            try
            {
                var images = post.FindElements(By.CssSelector(filter));
                return images;
            }
            catch (Exception error)
            {
                SendError(error.Message);
                return null;
            }
        }

        private string GetNameImage(string url)
        {
            string name = url.Split("https://")[1].Split("/")[3].Split(".jpg?")[0];
            return name;
        }

        private string GetURL()
        {
            IWebElement layerWrap = null;
            layerWrap = driver.FindElement(By.CssSelector("div[id='layer_wrap']"));
            Thread.Sleep(300);
            IWebElement photoDiv = null;
            while (photoDiv == null)
            {
                try
                {
                    photoDiv = layerWrap.FindElement(By.XPath("//*[@id=\"pv_photo\"]"));
                }
                catch
                {

                }
            }
            var photo = photoDiv.FindElement(By.TagName("img"));
            var url = photo.GetAttribute("src");
            return url;
        }

        private bool DownloadFile(string url, string localPath)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, localPath);
                    SendMessage("Загружен файл по пути: " + localPath);
                    return true;
                }
                catch (Exception error)
                {
                    SendError(error.Message);
                    return false;
                }
            }
        }

        private void ScrollPageWidht(ref IWebDriver webDriver, int value = 10)
        {
            try
            {
                ((IJavaScriptExecutor)webDriver).ExecuteScript($"window.scrollTo(document.body.scrollWidth, 0)");
            }
            catch { }
        }

        private void ScrollPageHeight(ref IWebDriver webDriver, int value = 10)
        {
            try
            {
                ((IJavaScriptExecutor)webDriver).ExecuteScript($"window.scrollTo(0, document.body.scrollHeight)");
            }
            catch { }
        }

        private IReadOnlyCollection<IWebElement> GetPosts(IWebDriver webDriver, string filter)
        {
            try
            {
                var posts = webDriver.FindElements(By.CssSelector(filter));
                return posts;
            }
            catch
            {
                return null;
            }
        }

        private bool SetPage(ref IWebDriver webDriver, string url)
        {
            try
            {
                webDriver.Navigate().GoToUrl(url);
                return true;
            }
            catch (Exception error)
            {
                SendError(error.Message);
                return false;
            }
        }

        private IWebDriver CreateDriver(string pathToDriver)
        {
            try
            {
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(pathToDriver);
                ChromeOptions options = new ChromeOptions();
                options.LeaveBrowserRunning = false;
                options.SetLoggingPreference("Browser", LogLevel.Off);
                options.SetLoggingPreference("Client", LogLevel.Off);
                options.SetLoggingPreference("Server", LogLevel.Off);
                options.SetLoggingPreference("Profiler", LogLevel.Off);
                options.SetLoggingPreference("Driver", LogLevel.Off);
                options.AddArgument("--log-level=3");
                options.AddArgument("--start-maximized");
                options.AddArgument("headless");


                ChromeDriver driver = new ChromeDriver(chromeDriverService, options);
                return driver;
            }
            catch (Exception error)
            {
                SendError(error.Message);
                return null;
            }
        }
    }
}
