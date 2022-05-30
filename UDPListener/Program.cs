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
        private static string URL = "https://yougotmailv2.azurewebsites.net/api/Mail";
        private static Mail mail = new Mail();
        static void Main(string[] args) {
            Console.WriteLine("Goddag");
            using (UdpClient socket = new UdpClient()){
                socket.Client.Bind(new IPEndPoint(IPAddress.Any, 7005));
                var oldDistance = 41.8;
                using (HttpClient client = new HttpClient()){
                    while (true){
                        IPEndPoint from = null;
                        byte[] data = socket.Receive(ref from);
                        string received = Encoding.UTF8.GetString(data);
                        Console.WriteLine(received);
                        
                        mail.UnixTimeStamp = 0;
                        mail.Id = 0;
                        var convertReceived = double.Parse(received, System.Globalization.CultureInfo.InvariantCulture);
                        //Console.WriteLine(convertReceived);

                        
                        if (oldDistance - convertReceived > 0.5){
                            mail.Detected = "yes";
                            oldDistance = convertReceived;
                        }
                        else if(Math.Abs(convertReceived - 41.8) < 0.5){
                            oldDistance = 41.8;
                            mail.Detected = "no";
                        }
                        else{
                            mail.Detected = "same reading";
                        }

                        Console.WriteLine(mail.Detected);

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
