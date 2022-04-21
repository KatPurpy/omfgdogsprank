using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace OMFGDOGS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            var proc = new System.Diagnostics.Process();

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "ipconfig.exe";
            p.Start();
            p.WaitForExit();
            Console.Write(p.StandardOutput.ReadToEnd());
            Console.WriteLine("(c)lient or (s)erver? ");

            choice:
            char option = Console.ReadKey().KeyChar;
            switch (option) {
                case 's': Server(); break;
                case 'c': Client(); break;
                case 't': Application.Run(new Form1()); break;
                default: goto choice;
            }
        }

       

        static IPEndPoint Configure()
        {
            Console.Write("ENTER IP: ");
            var ip = Console.ReadLine();

            Console.Write("PORT: ");
            var port = int.Parse(Console.ReadLine());

            return new IPEndPoint(IPAddress.Parse(ip), port);
        }
        static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Client()
        {
            Console.WriteLine("ENTER SERVER IP AND PORT");
            IPEndPoint endpoint = Configure();
            clientSocket.Connect(endpoint);

            ConsoleExtension.Hide();

            while (true)
            {
                // получаем ответ
                var response = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт
                while (clientSocket.Available == 0)
                {
                    //подождать, пока не появятся новые данные
                }

                do
                {
                    bytes = clientSocket.Receive(response, response.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(response, 0, bytes));
                }
                while (clientSocket.Available > 0);

                string command = builder.ToString();

                switch (command)
                {
                    case "ABORT":
                        Environment.Exit(0);
                        break;
                    case "GO GO POWERDOGS!":
                        Application.Run(new Form1());
                        break;
                }
            }

            Console.ReadKey();
            //ConsoleExtension.Hide();
        }


        static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<Socket> clients = new List<Socket>();
        static void Server()
        {
            IPEndPoint endpoint = Configure();

            // создаем сокет
            
            serverSocket.Bind(endpoint);
            serverSocket.Listen(64);


            Thread thread = new Thread(new ThreadStart(ServerThread));
            thread.Start();
            while (thread.IsAlive)
            {
                PropagadeMessage(Console.ReadLine());
            }
        }
        
        static void PropagadeMessage(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            foreach (var c in clients)
            {
                try {
                    c.Send(bytes);
                }
                catch { }
            }

            if(s == "GO GO POWERDOGS!")
            {
                Application.Run(new Form1());
            }
        }

        static void ServerThread()
        {
            
            while (true)
            {
                Socket s = serverSocket.Accept();
                clients.Add(s);
                Console.WriteLine("NEW CLIENT " + s.RemoteEndPoint.Serialize());
            }
        }
            
    }

}

static class ConsoleExtension
{
    const int SW_HIDE = 0;
    const int SW_SHOW = 5;
    readonly static IntPtr handle = GetConsoleWindow();
    [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static void Hide()
    {
        ShowWindow(handle, SW_HIDE); //hide the console
    }
    public static void Show()
    {
        ShowWindow(handle, SW_SHOW); //show the console
    }
}