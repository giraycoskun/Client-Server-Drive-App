using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example

namespace Server
{

    public partial class Form1 : Form
    {
        
        int portNum;
        int MAX_CLIENT = 20;
        int MAX_BUF = (2 << 22);
        bool listening;
        string fileDirectory;
        Socket server;
        //public static ManualResetEvent allDone = new ManualResetEvent(false);
        IPAddress ipAddress;
        List<Socket> clientSocketList = new List<Socket>() ;
        List<string> usernameList = new List<string>();

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            string host = Dns.GetHostName();
            IPHostEntry ip = Dns.GetHostEntry(host);
            logBox.AppendText("Server IP: " + ip.AddressList[1].ToString()+"\n");
            ipAddress = ip.AddressList[1];
            server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //clientSocketList = new List<Socket>();
            //usernameList = new List<string>();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            bool input_check = true;
            string port = portBox.Text;
            portBox.Enabled = false;
            try
            {
                portNum = Int32.Parse(port);
            }
            catch (FormatException)
            {
                logBox.AppendText($"ERROR not a valid number: '{portNum}' \n");
                input_check = false;
            }

            if(fileDirectory== "")
            {
                input_check = false;
            }
            if (input_check && (!listening))
            {
                try
                {
                    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNum);
                    server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    server.Bind(localEndPoint);
                    server.Listen(MAX_CLIENT);
                    listening = true;

                    logBox.AppendText($"Server STARTED at port: {portNum} \n");

                    Thread acceptThread = new Thread(Accept);
                    acceptThread.IsBackground = true;
                    acceptThread.Start();


                }
                catch (Exception except)
                {

                    logBox.AppendText($"ERROR: server socket stopped\n");
                    server.Close();
                    listening = false;
                    portBox.Text = "";
                    fileBox.Text = "";
                    portBox.Enabled = true;
                    fileBox.Enabled = true;

                }
            }
            else if(listening)
            {
                logBox.AppendText($"Server is already listening.\n");
            }
            else
            {
                portBox.Enabled = true;
            }

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                listening = false;
                server.Close();
                logBox.AppendText($"Server STOPPED \n");
                foreach (Socket socket in clientSocketList)
                {
                    try
                    {
                        socket.Close();
                    }
                    catch
                    {
                        logBox.AppendText("ERROR: Closing client socket failed\n");
                    }

                }
                portBox.Text = "";
                fileBox.Text = "";
                portBox.Enabled = true;
                fileBox.Enabled = true;
            }
            catch (Exception)
            {
                logBox.AppendText($"ERROR: Server cannot stop properly \n");
                listening = false;
                logBox.AppendText($"Server STOPPED \n");
                portBox.Text = "";
                fileBox.Text = "";
                portBox.Enabled = true;
                fileBox.Enabled = true;
            }            
        }

        private void Accept()
        {
            while (listening)
            {
                try
                {
                    Socket newClient = server.Accept();
                    clientSocketList.Add(newClient);
                    logBox.AppendText("A client is connected.\n");

                    Thread receiveThread = new Thread(() => Receive(newClient)); // updated
                    receiveThread.IsBackground = true;
                    receiveThread.Start();
                    
                }
                catch (Exception e)
                {

                    //logBox.AppendText("The server socket stopped.\n");
                    server.Close();
                    listening = false;
                    portBox.Enabled = true;
                    fileBox.Enabled = true;
                }
            }
        }

        private void Receive(Socket thisClient) // updated
        {
            string username = "";
            try
            {
                Byte[] buffer = new Byte[64];
                thisClient.Receive(buffer);
                username = Encoding.Default.GetString(buffer);
                username = username.Substring(0, username.IndexOf("\0"));
                logBox.AppendText("Client tries to connect: " + username + "\n");
                //incomingmessage = "giray"
                //string username = incomingMessage.Split(' ').ToList()[1];
            }
            catch
            {
                thisClient.Close();
                clientSocketList.Remove(thisClient);
                listening = false;
                return;
            }
            bool result = checkUsername(username);
            if (result)
            {
                usernameList.Add(username);
                logBox.AppendText("Client is Accepted: HI " + username + "\n");
                //logBox.AppendText(usernameList.ToString()+"\n");
                handleClient(thisClient, username);
            }
            else
            {
                rejectClient(thisClient, username);
                thisClient.Shutdown(SocketShutdown.Both);
                thisClient.Close();
                logBox.AppendText("Client is Rejected: " + username + "\n");
                clientSocketList.Remove(thisClient);
            }
        }
        private bool checkUsername(string username)
        {
            bool result = true;
            
            if (usernameList.Contains(username))
            {
                result = false;
            }
            return result;
        }

        private void handleClient(Socket client, string username)
        {
            try
            {
                string hello_message = "Hi from server";
                Byte[] buffer = Encoding.Default.GetBytes(hello_message);
                client.Send(buffer);
            }
            catch
            {
                logBox.AppendText($"ERROR: hi message from server to client {username} could not sent!!\n");
            }
            bool fileUploadError = true;
            while(checkConnection(client))
            {
                string commandMessage = "";
                string command = "";
                string filename = "";
                try
                {
                    Byte[] commandBuffer = new Byte[64];
                    commandMessage = "";
                    client.Receive(commandBuffer);
                    commandMessage = Encoding.Default.GetString(commandBuffer);
                    commandMessage = commandMessage.TrimEnd('\0');
                    //UPLOAD filename
                    //DELETE filename
                    //DOWNLOAD filename
                    //CH_ACCESS filename
                    command = "";
                    filename = "";
                      
                    command = commandMessage.Split()[0];
                    filename = commandMessage.Split()[1];
                }
                catch
                {
                    //logBox.AppendText("Client is disconnected !!\n");
                    break;
                }

                logBox.AppendText("Client: " + commandMessage + "\n");
                if (command == "UPLOAD")
                {
                    int? count = Program.GetIncCountByName(filename);
                    count = count == null ? 0 : count+1;
                    /*
                    if (count == null)
                    {
                        count = 0;
                    }
                    else
                    {
                        count = count + 1;
                    }
                    */
                    
                    string tempFileName = username + filename + "." + count.ToString();
                    FileStream uploadFile = File.Create(Path.Combine(fileDirectory, tempFileName));
                    Byte[] uploadFileBuffer = new Byte[MAX_BUF];

                    Byte[] fileSizeBuffer = new Byte[64];
                    try
                    {
                        client.Receive(fileSizeBuffer);
                    }
                    catch
                    {
                        logBox.AppendText("ERROR: During File Upload\n");
                        fileUploadError = false;
                        uploadFile.Close();
                        break;
                    }
                    
                    ulong fileSize = BitConverter.ToUInt64(fileSizeBuffer, 0);
                    //string data = null;
                    ulong numBytesRead = 0;

                    while (fileSize > numBytesRead)
                    {
                        try
                        {
                            
                            int numBytes = client.Receive(uploadFileBuffer);
                            numBytesRead += (ulong)numBytes;
                            
                            int index = Array.FindIndex(uploadFileBuffer, checkEnd);
                            
                            if (index > -1)
                            {
                                uploadFile.Write(uploadFileBuffer, 0, index);
                                break;
                            }
                            else
                            {
                                
                                uploadFile.Write(uploadFileBuffer, 0, numBytes);
                            }
                           
                            //uploadFile.Write(uploadFileBuffer, 0, numBytes);
                        }
                        catch
                        {
                            logBox.AppendText("ERROR: During File Upload\n");
                            uploadFile.Close();
                            fileUploadError = false;
                            break;
                        }          
                    }
                    uploadFile.Close();
                    if (fileUploadError)
                    {
                        if (count == 0)
                        {
                            Program.InsertFile(filename, fileDirectory, username, File1.AccessType.PRIVATE);
                        }
                        else
                        {
                            Program.IncrementFileCount(filename);
                        }

                        //List<File1> files = Program.GetAllFiles();

                        logBox.AppendText($"File {tempFileName} UPLOADED\n");
                        string message = filename + " UPLOADED";
                        sendClientMessage(client, message);
                    }
                }
                else if(command == "DOWNLOAD")
                { }
                else if(command == "DELETE")
                { }
                else if(command == "CH_ACCESS")
                { }
                else
                {
                    logBox.AppendText($"Unknown Command: {command}\n");
                }


                /*
                //connected = client.Connected;
                byte[] clientData = new byte[1024 * 5000];
                int receivedBytesLen = client.Receive(clientData);
                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                int dummy = Program.GetFilesByOwner(username).Count();
                
                string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
                string complete_name = "";
                Console.WriteLine("dummy");
                if (dummy == 0)
                {
                    complete_name = username + fileName;

                }
                else
                {
                     complete_name = username + fileName + "-" + "0" + $"{dummy}";          
                }
                */
                /* Read file name */
                /*
                BinaryWriter bWrite = new BinaryWriter(File.Open(fileDirectory + "/" + (complete_name), FileMode.Append)); ;
               
                bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
               
                bWrite.Close();
                
               
                Program.InsertFile(complete_name,fileDirectory,username,0);
                logBox.AppendText("Dosya geldi");
                */
            }
            logBox.AppendText($"User: {username} disconnected\n");
            usernameList.Remove(username);
            clientSocketList.Remove(client);
        }

        private void sendClientMessage(Socket client, string message)
        {
            try
            {
                Byte[] buffer = Encoding.Default.GetBytes(message);
                client.Send(buffer);
            }     
            catch
            {
                logBox.AppendText($"ERROR: message to client: {message} not sent\n");
            }
        }

        private void rejectClient(Socket client, string username)
        {
            try
            {
                string hello_message = "REJECT";
                Byte[] buffer = Encoding.Default.GetBytes(hello_message);
                client.Send(buffer);
            }
            catch
            {
                logBox.AppendText($"ERROR: REJECT message from server to client {username} could not sent!!\n");
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            string temp = "";
            fileBox.Enabled = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                fileBox.Text = folderBrowserDialog1.SelectedPath;
                temp = fileBox.Text;
            }
            if(Directory.Exists(temp))
            {
                fileDirectory = temp;
                logBox.AppendText($"DOWNLOAD DIRECTORY: {fileDirectory} \n");
                fileBox.Enabled = false;
            }
            else
            {
                logBox.AppendText("ERROR: path does not exist \n");
                fileDirectory = "";
            }
        }

        private void fileBox_TextChanged(object sender, EventArgs e)
        {

        }

        private bool checkConnection(Socket socket)
        {
            if (listening)
            {
                bool blockingState = socket.Blocking;
                try
                {
                    byte[] tmp = new byte[1];

                    socket.Blocking = false;
                    socket.Send(tmp, 0, 0);
                    Console.WriteLine("Connected!");
                }
                catch (SocketException e)
                {
                    // 10035 == WSAEWOULDBLOCK
                    if (e.NativeErrorCode.Equals(10035))
                    {
                        //Console.WriteLine("Still Connected, but the Send would block");
                    }
                    else
                    {
                        //Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
                    }
                    logBox.AppendText("ERROR: Connection Check is a failed !!\n");
                }
                finally
                {
                    socket.Blocking = blockingState;
                }
                return socket.Connected;
            }
            else
            {
                return false;
            }
        }

        private bool checkEnd(Byte b)
        {
            if(b=='\0')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            try
            {
                server.Close();

                foreach (Socket socket in clientSocketList)
                {
                    socket.Close();
                }
            }
            catch
            {

            }
            */
        }
    }
}
