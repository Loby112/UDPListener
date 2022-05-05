using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using UDPListener.Models;

namespace UDPListener {
    internal class Program{
        private static string URL = "https://yougotmailapi.azurewebsites.net/api/Mail";
        private static Mail mail = new Mail();
        static void Main(string[] args) {
            Console.WriteLine("Goddag");
            using (UdpClient socket = new UdpClient()){
                socket.Client.Bind(new IPEndPoint(IPAddress.Any, 7005));

                using (HttpClient client = new HttpClient()){
                    while (true){
                        IPEndPoint from = null;
                        byte[] data = socket.Receive(ref from);
                        string received = Encoding.UTF8.GetString(data);
                        mail.UnixTimeStamp = 0;
                        mail.Id = 0;
                        mail.Detected = received;

                        string serializedData = JsonSerializer.Serialize(mail);
                        Console.WriteLine("Server received " + received + " From " + from.Address);
                        HttpContent content = new StringContent(serializedData, Encoding.UTF8, "text/json");
                        Console.WriteLine(content.ReadAsStringAsync().Result);
                        client.PostAsync(URL, content);
                            

                    }
                }

            }
        }
    }
}
