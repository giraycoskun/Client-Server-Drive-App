using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class CLIENT : Form
    {
        int MAX_BUF = (2 << 22);
        IPAddress serverIPAddress;
        int serverPortNum;
        bool sendFilePermit = false;
        string username;
        string filepath;
        bool connected = false;
        Socket clientSocket;
        
        public CLIENT()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void CLIENT_Load(object sender, EventArgs e)
        {

        }

        private void ipBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipBox.Enabled = false;
            portBox.Enabled = false;
            usernameBox.Enabled = false;
            string ipBoxText= ipBox.Text;
            bool inputCheck = true;
            username = usernameBox.Text;
            //TODO: Input Check
            try
            {
                Int32.TryParse(portBox.Text, out serverPortNum);
            }
            catch
            {
                outputBox.AppendText("ERROR: port number is not valid \n");
                inputCheck = false;
            }


            serverIPAddress = IPAddress.Parse(ipBoxText);

            if (inputCheck)
            {

                try
                {
                    clientSocket.Connect(serverIPAddress, serverPortNum);
                    connected = true;
                    outputBox.AppendText($"CONNECTED to the server with ip: {ipBoxText} and port: {serverPortNum} !\n");
                    string helloMessage = username;
                    if (username != "" && helloMessage.Length <= 64)
                    {
                        Byte[] buffer = new Byte[64];
                        buffer = Encoding.Default.GetBytes(helloMessage);
                        clientSocket.Send(buffer);
                    }
                    else
                    {
                        outputBox.AppendText("Username length is either empty or too long!\n");
                    }

                    string incomingMessage = "";

                    try
                    {
                        Byte[] hello_buffer = new Byte[64];
                        clientSocket.Receive(hello_buffer);
                        incomingMessage = Encoding.Default.GetString(hello_buffer);
                        incomingMessage = incomingMessage.TrimEnd('\0');
                        outputBox.AppendText("Server: " + incomingMessage + "\n");
                        Thread listenThread = new Thread(listenServer);
                        listenThread.IsBackground = true;
                        listenThread.Start();
                    }
                    catch
                    {
                        outputBox.AppendText("Connection STOPPED \n");
                        clientSocket.Close();
                        connected = false;
                        enableInputBoxes();
                    }

                    if (incomingMessage == "REJECT")
                    {
                        clientSocket.Close();
                        connected = false;
                        enableInputBoxes();
                    }

                }
                catch (Exception except)
                {
                    outputBox.AppendText("ERROR: Connection Fault not established \n");
                    connected = false;
                    clientSocket.Close();
                    enableInputBoxes();
                    
                }
            }

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                try
                {
                    enableInputBoxes();
                    //ipBox.Enabled = true;
                    //portBox.Enabled = true;
                    //usernameBox.Enabled = true;
                    connected = false;
                    clientSocket.Close();
                    
                    outputBox.AppendText($"Connection STOPPED by client\n");
                }
                catch (Exception except)
                {

                    outputBox.AppendText($"ERROR: Cannot Stop \n");
                }
            }            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            uploadFileBox.Enabled = true;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.ShowDialog();
            uploadFileBox.Text = openFileDialog1.FileName;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            bool connection = false;
            try
            {
                connection = checkConnection(clientSocket);
            }
            catch(Exception except)
            {
                outputBox.AppendText("Cannot upload without connection\n");
            }

            if (connection)
            {
                uploadFileBox.Enabled = false;
                filepath = uploadFileBox.Text;
                if (!File.Exists(filepath))
                {
                    outputBox.AppendText("ERROR: File Does Not Exist\n");
                    uploadFileBox.Text = "";
                    uploadFileBox.Enabled = true;
                }
                else
                {
                    uploadFile(filepath);
                    uploadFileBox.Enabled = true;
                }
            }
            else
            {
                outputBox.AppendText("ERROR: NOT CONNECTED\n");
                enableInputBoxes();
                clientSocket.Close();
            }
            
        }

        private void enableInputBoxes()
        {
            if(ipBox.InvokeRequired || portBox.InvokeRequired || usernameBox.InvokeRequired)
            {
                ipBox.BeginInvoke(new Action(delegate { enableInputBoxes(); }));
                return;
            }
            ipBox.Enabled = true;
            portBox.Enabled = true;
            usernameBox.Enabled = true;
        }



        private void uploadFile(string filepath)
        {
            string filename = Path.GetFileName(filepath);
            string uploadMessage = "UPLOAD" + " " + filename;
            Byte[] commandBuffer = new Byte[64];
            commandBuffer = Encoding.Default.GetBytes(uploadMessage);
            Array.Resize(ref commandBuffer, 64);
            clientSocket.Send(commandBuffer);

            
            Byte[] uploadBuffer = new Byte[MAX_BUF];
            try
            {

                //clientSocket.SendFile(filepath);
               // StreamReader sr = new StreamReader("TestFile.txt")
                using (FileStream fsSource = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {

                    // Read the source file into a byte array.
                    ulong numBytesToRead = (ulong)fsSource.Length;
                    //Byte[] fileSizeBuffer = new Byte[64];

                    Byte[] fileSizeBuffer = BitConverter.GetBytes(numBytesToRead);
                    Array.Resize(ref fileSizeBuffer, 64);
                    clientSocket.Send(fileSizeBuffer);

                    
                    ulong numBytesRead = 0;
                    int n, temp;

                    while (numBytesToRead > 0)
                    {
                        
                        // Read may return anything from 0 to numBytesToRead.
                        n = fsSource.Read(uploadBuffer, 0, MAX_BUF);

                        temp = clientSocket.Send(uploadBuffer);

                        // Break when the end of the file is reached.

                        if (n == 0)
                            break;

                        numBytesRead += (ulong)n;
                        numBytesToRead -= (ulong)n;
                        
                    }

                    sendFilePermit = false;
                }
                
                /*
                if(!getServerMessage())
                {
                    throw new System.InvalidOperationException("Logfile cannot be read-only");
                }
                */
            }
            catch(Exception except)
            {
                outputBox.AppendText("Error: during file sending.\n");
                outputBox.AppendText("Connection STOPPED \n");
                clientSocket.Close();
                connected = false;
                enableInputBoxes();
                return;

            }
            safeLogWrite("File Sending is finished!!\n");
            

            /*
            string filePath = "";
            /* File reading operation. */
            /*
            Console.WriteLine(fileName);
            fileName = fileName.Replace("\\", "/");
            while (fileName.IndexOf("/") > -1)
            {
                filePath += fileName.Substring(0, fileName.IndexOf("/") + 1);
                fileName = fileName.Substring(fileName.IndexOf("/") + 1);
            }
            byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
      
           //TODO
            byte[] fileData = File.ReadAllBytes(filePath + fileName);
            /* Read & store file byte data in byte array. */
            /*
            byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
            /* clientData will store complete bytes which will store file name length, 
            file name & file data. */
            /*
            byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
            /* File name length’s binary data. */
            /*
            fileNameLen.CopyTo(clientData, 0);
            fileNameByte.CopyTo(clientData, 4);
            fileData.CopyTo(clientData, 4 + fileNameByte.Length);
            /* copy these bytes to a variable with format line [file name length]
            [file name] [ file content] */


            //clientSocket.Send(clientData);
        }

        private void listenServer()
        {
            while (connected)
            {
                string incomingMessage = "";

                try
                {
                    Byte[] buffer = new Byte[64];
                    clientSocket.Receive(buffer);
                    incomingMessage = Encoding.Default.GetString(buffer);
                    incomingMessage = incomingMessage.TrimEnd('\0');
                    if (incomingMessage != "")
                    {
                        string text = "Server: " + incomingMessage + "\n";
                        safeLogWrite(text);
                    }
                    else
                    {
                        throw new System.InvalidOperationException("Empty String Received");
                    }
                }
                catch
                {
                    //outputBox.AppendText("Connection STOPPED \n");
                    if (connected)
                    {
                        string text = "Connection Stopped by Server \n";
                        safeLogWrite(text);
                        clientSocket.Close();
                        connected = false;
                        enableInputBoxes();
                    }
                }
            }
        }

        private bool checkConnection(Socket socket)
        {
            bool blockingState = socket.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                socket.Blocking = false;
                socket.Send(tmp, 0, 0);
                //Console.WriteLine("Connected!");
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
                outputBox.AppendText("ERROR: Connection Check is a failed !!\n");
            }
            finally
            {
                socket.Blocking = blockingState;
            }
            return socket.Connected;
        }

        //TODO
        public void safeLogWrite(string EventText)
        {
            if (outputBox.InvokeRequired)
            {
                outputBox.BeginInvoke(new Action(delegate {
                    safeLogWrite(EventText);
                }));
                return;
            }
            outputBox.AppendText(EventText);          
        }
    }
}
