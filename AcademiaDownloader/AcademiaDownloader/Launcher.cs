using System.Threading;


namespace AcademiaDownloader
{
    class Launcher
    {
        private string urlGroups = "";
        private string driverPath = "";
        private string pathParser = "";

        private string login = "";
        private string pass = "" ;

        private TypeDonwload typeDownload = TypeDonwload.Post;

        private Parser runnedParser;
        private Thread activeTheard;

        public Launcher()
        {
            Start();
        }

        private void Start()
        {
            UI.AonChangeGroup += SetGroups;
            UI.AonChangePathToDriver += SetDriverPath;
            UI.AonChangePathToParser += SetParserPath;
            UI.AStartParser += LaunchParsers;
            UI.AStopParser += StopParser;
            UI.AonChangeLogin += SetLogin;
            UI.AonChangePass += SetPass;
            UI.AonChangeTypeDownload += SetTypeDownload;
        }

        public void LaunchParsers()
        {
            if (runnedParser == null)
            {
                Parser parser = new Parser(urlGroups, pathParser, driverPath, login, pass, typeDownload);
                Parser.ASendError += UI.ShowError;
                Parser.ASendMessage += UI.ShowMessage;
                Parser.AOnStopParser += DeleteParser;

                var thread = new Thread(new ThreadStart(parser.TryStart));

                activeTheard = thread;
                runnedParser = parser;
                activeTheard.Start();
            }
        }

        public void StopParser()
        {
            if (runnedParser != null)
            {
                Parser.ASendError -= UI.ShowError;
                Parser.ASendMessage -= UI.ShowMessage;

                runnedParser.Stop();
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
        }

        public void DeleteParser()
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

        public void SetTypeDownload(TypeDonwload type)
        {
            typeDownload = type;
        }
    }
}
