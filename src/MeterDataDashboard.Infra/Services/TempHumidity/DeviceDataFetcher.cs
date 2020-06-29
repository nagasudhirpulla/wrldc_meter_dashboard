using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MeterDataDashboard.Infra.Services.TempHumidity
{
    public partial class DeviceDataService
    {
        public class DeviceDataFetcher
        {
            public static double? FetchData(string devIp, DataType dataType, int port = 4567)
            {
                try
                {
                    //---data to send to the server---
                    string textToSend = "s";

                    //---create a TCPClient object at the IP and port no.---
                    TcpClient client = new TcpClient(devIp, port);
                    NetworkStream nwStream = client.GetStream();
                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

                    //---send the text---
                    //Console.WriteLine("Sending : " + textToSend);
                    nwStream.Write(bytesToSend, 0, bytesToSend.Length);

                    //---read back the text---
                    byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    string resp = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                    //Console.WriteLine($"Received : {resp}");
                    client.Close();
                    string pattern;
                    if (dataType == DataType.Humidity)
                    {
                        pattern = @".*t2=(.+?)$";
                    }
                    else
                    {
                        pattern = @".*t1=(.+?)&.*";
                    }
                    MatchCollection matches = Regex.Matches(resp, pattern, RegexOptions.IgnoreCase);
                    if (matches == null || matches.Count == 0)
                    {
                        return null;
                    }
                    double val = double.Parse(matches[0].Groups[1].Value);
                    return val;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching device realtime data : {ex.Message}");
                    return null;
                }

            }
        }
    }
}
