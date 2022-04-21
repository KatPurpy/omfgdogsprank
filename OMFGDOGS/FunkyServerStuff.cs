using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OMFGDOGS
{
    public static class FunkyServerStuff
    {

        public static void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, IPEndPoint server, ref Socket clientSocket)
        {
            string encodedAddress = "c." + server.Address.ToString() + "." + server.Port + ".exe";
            String sBuffer = "";
            // if Mime type is not provided set default to text/html  
            if (sMIMEHeader.Length == 0)
            {
                sMIMEHeader = "text/html";// Default Mime Type is text/html  
            }
            sBuffer += sHttpVersion + sStatusCode + "\r\n";
            sBuffer += "Content-Type: " + sMIMEHeader + "\r\n";
            sBuffer += "Accept-Ranges: bytes\r\n";
            sBuffer += "Content-Length: " + iTotBytes + "\r\n";
            sBuffer += $"Content-Disposition: attachment; filename=\"{encodedAddress}\"\r\n";
            sBuffer += "Connection: close\r\n\r\n";
            Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);
            SendToBrowser(bSendData, ref clientSocket);
            Console.WriteLine("Total Bytes : " + iTotBytes.ToString());
        }


        public static void SendToBrowser(String sData, ref Socket mySocket)
        {
            SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }
        public static void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            int numBytes = 0;
            try
            {
                if (mySocket.Connected)
                {
                    if ((numBytes = mySocket.Send(bSendData, bSendData.Length, 0)) == -1)
                        Console.WriteLine("Socket Error cannot Send Packet");
                    else
                    {
                        Console.WriteLine("No. of bytes send {0}", numBytes);
                    }
                }
                else Console.WriteLine("Connection Dropped....");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occurred : {0} ", e);
            }
        }

    }
}
