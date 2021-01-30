using API_Core;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class MyTcpListener
{
    public static void Main()
    {
        TcpListener server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 9050;
            IPAddress localAddr = IPAddress.Parse("192.168.1.37");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            // Enter the listening loop.
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;
                MainClass mainaclasse = new MainClass();
                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);

                    string[] Splitted = data.Split("|");
                    try
                    {
                        mainaclasse.initialize(int.Parse(Splitted[0]));
                        var res = mainaclasse.getMovieWithTitle(Splitted[1]);
                        string result = "";
                        if (res != null)
                        {
                            foreach (var x in res)
                            {
                                mainaclasse.getStreamList(x);
                                result += x.getMovieTitle() + "~" + x.getMovieDesc() + "~" + x.getMovieImage();
                                foreach(var y in x.getStreams())
                                {
                                    result += "~" + y;
                                }

                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(result);
                                // Send back a response.
                                stream.Write(msg, 0, msg.Length);
                                Console.WriteLine("Sent: {0}", result);
                                result = "";
                                
                            }
                            //result = result.Remove(result.Length - 1);
                        }
                    }
                    catch { }
                }

                // Shutdown and end connection
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }
}