using System;
using System.IO;
using System.Net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Internal;
using System.Net.Http.Headers;

namespace AcademiaDownloader
{

    class Parser
    {
        public delegate void Action();

        public static Action<string> ASendError;
        public static Action<string> ASendMessage;

        public static event Action AOnStopParser;

        public static bool Update = true;

        private IWebDriver driver;

        private int maxCountImages = 0;
        private string urlGroup = "";
        private string pathParser = "";
        private string pathDriver = "";
        private string pathFolder = "";

        private string login = "";
        private string pass = "";

        private string subString = "";

        private TypeDonwload typeDownload = TypeDonwload.Post;

        private bool stop = false;

        private Parser() { }

        public Parser(string urlGroup, string pathParser, string pathDriver, string login, string pass, TypeDonwload typeDownload)
        {
            UI.AStopParser += Stop;

            this.urlGroup = urlGroup;
            this.pathDriver = pathDriver;
            this.pathParser = pathParser;
            this.login = login;
            this.pass = pass;
            this.typeDownload = typeDownload;
        }

        public void TryStart()
        {
            stop = false;
            if (CheckSettings())
            {
                ParsePage();
            }
            Stop();
        }

        public void SendError(string error)
        {
            ASendError?.Invoke(error);
        }

        public void SendMessage(string message)
        {
            ASendMessage?.Invoke(message);
        }

        public void Stop()
        {
            stop = true;
            if (driver != null)
            {
                try
                {
                    driver.Quit();
                }
                catch { }
            }
            ASendMessage?.Invoke("Парсер закончил работу");
            AOnStopParser?.Invoke();
            return;

        }

        private bool CheckSettings()
        {
            if ((urlGroup == "" || urlGroup == null) || (pathDriver == "" || pathDriver == null) || (pathParser == "" || pathParser == null) || (login == "" || login == null) || (pass == "" || pass == null))
            {
                SendError("Что-то из этого не установленны:\n" +
                    $"--| Cсылка на группу\n" +
                    $"--| Путь до драйвера Chrome\n" +
                    $"--| Путь до папки куда будут закидываться папки\n" +
                    $"--| Логин\n" +
                    $"--| Пароль");
                return false;
            }
            else
            {
                return true;
            }

        }

        private void ParsePage()
        {
            pathParser = CreateFolder(pathParser);
            if (pathParser == null) return;


            driver = CreateDriver(pathDriver);
            if (driver == null) return;


            if (!TrySetPage(ref driver, urlGroup)) return;


            TryAccept18();

            if (!TryLoginOnSite(driver)) return;

            TryAccept18();
            try
            {
                Thread.Sleep(2000);
            }
            catch (ThreadInterruptedException)
            {
                return;
            }

            if (!TrySetPage(ref driver, urlGroup)) return;

            try
            {
                Thread.Sleep(600);
            }
            catch (ThreadInterruptedException)
            {
                return;
            }

            if (typeDownload == TypeDonwload.Album)
            {
                DownloadFromAlbums();
            }
            else
            {
                DownloadFromPosts();
            }
            return;
        }

        private void DownloadFromPosts()
        {
            SendMessage(" Запущен режим загрузки с постов");
            var nameFolder = GetNameFolder(urlGroup);
            if (nameFolder == null) return;
            nameFolder += subString;

            pathFolder = CreateFolder(pathParser + nameFolder);
            if (pathFolder == null) return;

            var posts = GetPosts(nameFolder);
            if (posts == null)
            {
                return;
            }

            SendMessage(" Запуск процесса скачивания фотографий");
            int index = 0;
            do
            {
                foreach (var post in posts)
                {
                    if (stop) break;
                    IReadOnlyCollection<IWebElement> images = GetImagesFromPost(post);
                    if (images == null)
                    {
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
                                TryDownloadFile(url, path);
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
                try
                {
                    try
                    {
                        Thread.Sleep(200);
                    }
                    catch (ThreadInterruptedException)
                    {
                        return;
                    }
                    posts = GetPosts(driver, "div[class='page_post_sized_thumbs  clear_fix']");
                    ScrollPageHeight(ref driver);
                }
                catch (ThreadInterruptedException)
                {
                    return;
                }
            }
            while (!stop && posts != null && posts.Count > 0);
            return;
        }

        private void DownloadFromAlbums()
        {
            SendMessage(" Запущен режим загрузки с альбома");
            string nameFolder = GetNameFolder(urlGroup);
            if (nameFolder == null) return;
            nameFolder += subString;

            IWebElement imageDiv = GetMainImageDiv();
            if (imageDiv == null) return;

            pathFolder = CreateFolder(pathParser + nameFolder);
            if (pathFolder == null) return;

            SendMessage(" Запущен процесс скачивания фотографий");
            Actions action = new Actions(driver);
            action.MoveToElement(imageDiv).Click().Perform();

            int index = 0;
            do
            {
                try
                {

                    Thread.Sleep(300);

                    var url = GetURL();
                    var name = GetNameImage(url) + ".png";
                    var path = @$"{pathFolder}\{index}_{name}";
                    TryDownloadFile(url, path);
                    index++;
                    action = new Actions(driver);
                    action.SendKeys(Keys.ArrowRight).Build().Perform();
                    Thread.Sleep(300);

                }
                catch (ThreadInterruptedException)
                {
                    return;
                }

            }
            while (!stop && index <= maxCountImages);
            return;
        }

        private bool TrySetPage(ref IWebDriver webDriver, string url)
        {
            try
            {
                SendMessage(" Идёт открытие страницы: " + urlGroup);
                webDriver.Navigate().GoToUrl(url);
                return true;
            }
            catch (Exception error)
            {
                SendError(error.Message);
                SendError("Из-за ошибки при загрузки страницы, парсер остановлен");
                return false;
            }
        }

        private IWebDriver CreateDriver(string pathToDriver)
        {
            try
            {
                SendMessage(" Идёт попытка запустить драйвер браузера...");
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
                SendMessage(" Драйвер запушен");
                return driver;
            }
            catch (Exception error)
            {
                SendError(error.Message);
                SendError("Из-за ошибки при запуске драйвера, парсер остановлен");
                return null;
            }
        }

        private IWebElement GetMainImageDiv()
        {
            SendMessage(" Идёт получение первого изображения с альбома");
            IWebElement imageDiv = null;
            ScrollPageWidht(ref driver);
            try
            {
                imageDiv = driver.FindElements(By.XPath("//*[@aria-label=\"Фотография\"]"))[0];
                return imageDiv;
            }
            catch
            {
                SendError("Произошла ошибка при получении первого изображения ");
                return null;
            }


        }

        private IReadOnlyCollection<IWebElement> GetImagesFromPost(IWebElement post)
        {
            var images = GetImages(post, "a[aria-label='фотография']");
            if (images == null)
            {
                SendError("Из-за ошибки получения картинок, парсер остановлен");
                return null;
            }

            return images;
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

        private string GetNameImage(string url)
        {
            try
            {
                string name = url.Split("https://")[1].Split("/")[3].Split(".jpg?")[0];
                return name;
            }
            catch
            {
                SendError("Произошла ошибка при получении имени картинки, парсер остановлен");
                return null;
            }
        }

        private string GetNameFolder(string url)
        {
            SendMessage(" Идёт получение имени папки");
            try
            {
                string nameFolder = url.Split("vk.com/")[1];

                SendMessage(" Имя папку успешно получена");
                return nameFolder;
            }
            catch (Exception error)
            {
                SendError(error.Message);
                SendError("Из-за ошибки при получении имени папки из URL, парсер остановлен");
                return null;
            }
        }

        private string GetURL()
        {
            IWebElement layerWrap = null;
            layerWrap = driver.FindElement(By.CssSelector("div[id='layer_wrap']"));
            try
            {
                Thread.Sleep(300);
            }
            catch (ThreadInterruptedException)
            {
                return null;
            }
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

        private IReadOnlyCollection<IWebElement> GetPosts(string nameFolder)
        {
            SendMessage(" Идёт получение постов в группе");
            var posts = driver.FindElements(By.CssSelector("div[class='page_post_sized_thumbs  clear_fix']"));
            if (posts.Count != 0)
            {
                SendMessage(" Посты получены");
                return posts;

            }
            else
            {
                SendMessage("Количество постов: 0");
                SendMessage("Проверьте URL группы, логин и пароль от VK");
                return null;
            }
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
                SendError("Не удалось принять ограничение 18 плюс");
            }
            try
            {
                Thread.Sleep(200);
            }
            catch (ThreadInterruptedException)
            {
                return;
            }
        }

        private  bool TryLoginOnSite(IWebDriver webDriver)
        {
            if (login != "" && pass != "")
            {
                SendMessage(" Идёт попытка зайти в аккаунт");
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
                    catch (ObjectDisposedException)
                    {
                        return false;
                    }

                    IWebElement buttonLogin = null;
                    try
                    {
                        buttonLogin = webDriver.FindElement(By.CssSelector("button[id='login_button']"));
                    }
                    catch (ObjectDisposedException)
                    {
                        return false;
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
                        try
                        {
                            Thread.Sleep(600);
                        }
                        catch (ThreadInterruptedException)
                        {
                            return false;
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        return false;
                    }
                    catch (Exception error)
                    {
                        SendError(error.Message);
                        SendError("Произошла ошибка при вводе текста, проверье логин и пароль");
                        return false;
                    }
                }
                catch (NoSuchElementException)
                {
                    SendError("Ошибка при регистрации. Не найдены поля ввода, проверьте URL");
                    return false;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }

                Thread.Sleep(300);
                if (webDriver.Url.Contains("https://vk.com/login"))
                {
                    SendMessage(" НЕ удалось ввойти в аккаунт");
                    return false;
                }
                else
                {
                    SendMessage(" Удалось ввойти в аккаунт");
                    return true ;
                }
            }
            else
            {
                SendMessage(" Нет учётных данных, процесс входа в аккаунт пропущен");
                return false;
            }
        }

        private string CreateFolder(string localPath)
        {
            try
            {
                SendMessage(" Идёт процесс создания папки альбома/группы");
                if (!Directory.Exists(localPath))
                {
                    var info = Directory.CreateDirectory(localPath);
                    SendMessage(" Папка альбома/группы успещно созданна");
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
                SendError("Из-за ошибки при создании папки группы/альбома, парсер остановлен!");
                return null;
            }
        }

        private bool TryDownloadFile(string url, string localPath)
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
                    SendError("Из-за ошибки при загрузке файла, изображение пропущенно");
                    return false;
                }
            }
        }

        private void ScrollPageWidht(ref IWebDriver webDriver)
        {
            try
            {
                ((IJavaScriptExecutor)webDriver).ExecuteScript($"window.scrollTo(document.body.scrollWidth, 0)");
            }
            catch { }
        }

        private void ScrollPageHeight(ref IWebDriver webDriver)
        {
            try
            {
                ((IJavaScriptExecutor)webDriver).ExecuteScript($"window.scrollTo(0, document.body.scrollHeight)");
            }
            catch { }
        }

    }
}
