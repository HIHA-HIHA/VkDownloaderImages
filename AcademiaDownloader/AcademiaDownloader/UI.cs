using System;


namespace AcademiaDownloader
{
    class UI
    {
        public delegate void Action();

        public static Action<int> AonAddMaxCountPages;
        public static Action<string> AonChangeGroup;
        public static Action<string> AonChangePathToDriver;
        public static Action<string> AonChangePathToParser;
        public static Action<string> AonChangeLogin;
        public static Action<string> AonChangePass;
        public static Action<TypeDonwload> AonChangeTypeDownload;

        public static Action AStartParser;
        public static Action AStopParser;

        private int maxCountImages = 0;
        private string driverPath = "";
        private string parserPath = "";
        private string grouptUrl = "";
        private string vkLogin = "";
        private string vkPass = "";
        private string typeDownload = "";

        private string stateDriver = "-";
        private string statePath = "-";
        private string stateGroup = "-";
        private string stateLogin = "-";
        private string statePass = "-";
        private string stateTypeDownload= "-";

        public UI()
        {
            Start();
        }

        private void Start()
        {
            Console.Clear();
            ShowMainMenu();
        }

        #region Menus


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
                    ChangeTypeDownload(TypeDonwload.Post);
                    break;
                case "2":
                    ChangeTypeDownload(TypeDonwload.Album);
                    break;
                default:
                    ChangeTypeDownload(TypeDonwload.Post);
                    break;
            }

            return true;
        }
        private void ChangeTypeDownload(TypeDonwload type)
        {
            switch (type)
            {
                case TypeDonwload.Post:
                    typeDownload = "Пост";
                    break;
                case TypeDonwload.Album:
                    typeDownload = "Альбом";
                    break;
            }
            stateTypeDownload = "+";
            AonChangeTypeDownload?.Invoke(type);

        }

        #endregion

        #region Main Menu
        private void ShowMainMenu()
        {

            while (true)
            {

                ShowLogo();
                Console.Write("" +
                "|------------------------------------------------------------------------------------------------------------|\n" +
                "---!!! Внимание, при запуске парсинга группы которая УЖЕ ЕСТЬ в папке, то её файлы БУДУТ ПЕРЕЗАПИСАНЫ! !!!---\n" +
                "|------------------------------------------------------------------------------------------------------------|\n" +
                "|                                                                                                            |\n" +
                $"---@ [1] Изменить максимальное кол-во загрузок                     [{maxCountImages}]\n" +
                $"---@ [2] Изменить группу откуда загружать                          [{stateGroup}] | {grouptUrl}\n" +
                $"---@ [3] Изменить путь до драйвера Chrome                          [{stateDriver}] | {driverPath}\n" +
                $"---@ [4] Изменить путь до папки куда будут закидывать папки парсер [{statePath}] | {parserPath}\n" +
                $"---@ [5] Изменить тип загрузки                                     [{stateTypeDownload}] | {typeDownload}\n" +
                $"---@ [6] Логин VK                                                  [{stateLogin}] | " + vkLogin + "\n" +
                $"---@ [7] Пароль VK                                                 [{statePass}] | " + vkPass + "\n" +
                "----|--------------------------------------------------------------------------------------------------------|\n" +
                $"---@ [8] Запустить \n" +
                $"---@ [9] Остановить\n" +
                $"---@ [10] Информация\n" +
                "---! ");
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
        #endregion

        #endregion


        private void StartParser()
        {
            Console.WriteLine("---$ Парсер запущен");
            AStartParser?.Invoke();
        }

        private void StopParser()
        {
            Console.WriteLine("---$ Парсер выключен");
            AStopParser?.Invoke();
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
}
