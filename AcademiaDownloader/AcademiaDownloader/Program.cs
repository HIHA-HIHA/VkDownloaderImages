using System;

namespace AcademiaDownloader
{
    class Program
    {

        static void Main(string[] args)
        {
            new SenderToServer();
            Console.ReadLine();
            new Launcher();
            new UI();
        }

    }
}
