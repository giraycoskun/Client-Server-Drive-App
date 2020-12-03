# CS 408 Project FALL 2020-2021 - Sabancı University

- Giray Coşkun - 25137 - giraycoskun@sabanciuniv.edu
- Cankut Coşkun - 23776 - cankutcoksun@sabanciuniv.edu
- Dilara Müstecep - 25292 - dilaramustecep@sabanciuniv.edu
- Can Savrun - 23706 - cansavrun@sabanciuniv.edu
- Mert Güngör - 24148 - mertgungor@sabanciuniv.edu

---
## Database Details

Visual Studio .NET mysql setup
- 1-)Mysql for visual studio -> https://dev.mysql.com/downloads/file/?id=491638
- 2-)Mysql connector .NET -> https://dev.mysql.com/downloads/connector/net/
- 3-)VS -> Server -> Tools-> connect to Database ->  

- host: https://remotemysql.com/phpmyadmin/
- username: ioI0xzbThf
- password: VGITbQxEEa

**Database Structure**
id(int) | fileName(string) | filePath(string) | owner(string) | incCount(int) | accessType(PUBLIC, PRIVATE)

Primary Key - id
UNIQUE (filename, owner)

**Public Static Objects for DB:**
- connectionstring = "server=remotemysql.com;userid=ioI0xzbThf;password=VGITbQxEEa;database=ioI0xzbThf"
- MySqlConnection databaseConnection = new MySqlConnection(connectionString);

**Common Functions:**
  - InsertFile(String fileName, String filePath, String owner, File1.AccessType accessType)
  - IncrementFileCount(String fileName)
  - GetIncCountByName(String fileName)
  
---

## Connection

### Server

**Helpful Docs:**
- Async Programs: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/
- https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example

**GUI Elements**

- logBox : richtext box  for logs
- portBox : port number input
- fileBox : directory path input
- chooseButton: open file browser via folderBrowserDialog1 and sets it to fileBox
- startButton : starts server
- stopButton : stops server

**Functions**

- void startButton_Click(object sender, EventArgs e): Opens Server and relays to <ins>Accept()</ins>

- void stopButton_Click(object sender, EventArgs e): stops server

- void Accept(): get incoming clients and relay to <ins>Receive()</ins>

- void Receive(Socket thisClient): get username message and check decide to accept or reject if rejected disconnect else relays to <ins>handleClient()</ins>

- bool checkUsername(string username): checks if the username is already connected

- void handleClient(Socket client, string username): Main Loop to wait for client commands. And handles the command.

	Commands:
		- UPLOAD <filename>
		- ...
- void sendClientMessage(Socket client, string message): sends message to client socket.

- void rejectClient(Socket client, string username): Sends "REJECT" to client socket.

- void browseButton_Click(object sender, EventArgs e): opens file browser and sets the chosen path as <ins>fileDirectory</ins>

- bool checkConnection(Socket socket): checks if the connection is up by sending zero byte message.

- bool checkEnd(Byte b): checks if the byte is '\0'


**Globals**

- portNum: int
- MAX_CLIENT: int
- MAX_BUF: int
- listening: bool
- fileDirectory: string
- server: Socket
- ipAdress: IPAdress
- clientSocketList: list<Socket>
- usernameList: <string>

**Possible Issues in Server:**

- Get IP adress of system: there are multiple IPs DNS.IPHostEntry ip = Dns.GetHostEntry(host); returns multiple IP addresses in ip.AdressList second is chosen and printed
	 
---	 
### Client

**Helpful Docs:**

**GUI Elements**

- ipBox: ip adress input
- portBox: port number input
- usernameBox: username input
- connectButton: start connection to server
- stopButton: stops connection
- uploadFileBox: shows full file path
- browseButton: opens file browser for .txt files
- uploadButton: start uploding file to server
- outputBox: shows messages

**Functions**

- void connectButton_Click(object sender, EventArgs e): starts connection -> sends username -> if "rejected" closes connection
- void stopButton_Click(object sender, EventArgs e): stops connection
- void button1_Click(object sender, EventArgs e): (browse button) opens file browser via openFileDialog1 sets uploadFileBox 
- void uploadButton_Click(object sender, EventArgs e): checks connection and filepath; then relays to <ins>uploadFile()</ins>
- void enableInputBoxes(): enables input boxes in thread safe manner.
-  void uploadFile(string filepath): sends "UPLOAD <filename>" command then filesize; after that starts to loop of read and send in a buffer with MAX_BUF sized 
- void listenServer(): listens server and writes incoming messages to outputBox
- bool checkConnection(Socket socket): checks connection by sending zero byte message
- void safeLogWrite(string EventText): writes to  <ins>outputBox</ins> in a thread safe manner



**Globals**

- MAX_BUF: int
- serverIPAdress: IPAdress
- serverPortNum: int
- username: string
- filepath: string
- coonected: bool
- clientSocket: Socket

**Possible Issues in Client:**

---
## BUGS

- **Issue: 1:** Upload button without connection -> fixed :+1:
- **Issue 2:** PortNUmber Input Check -> fixed :+1:
- **Issue 3:** PortNUmber Input Check -> fixed :+1:
- **Issue 3:** Box Enables after Disconnection -> fixed :+1:
- **Issue 4:** Server disconnection does not trigger client -> fixed :+1:
- **Issue 5:** Client Disconnection crashes Server !!! -> fixed :+1:
- **Issue 6:** Logging is missing in some points -> fixed :+1:
- **Issue 7:** Upload information is not sent to client -> fixed :+1:
- **Issue 8:** When Server is closed from cross when client threads running application is not really closed stays at line 151 -> fixed :+1:
- **Issue 9:** Database insert is not working!! -> fixed :+1:
- **Issue 10:** Server does not use thread safe GUI.

## TESTS

### Invalid Inputs

- Invalid Port Number
- Empty Directory
- Invalid IP adress

### File Sizes

- A few bytes
- 1000 kb
- 500 MB
- 4GB

### Disconnections

- User disconnection
- Server Disconnection

### Specifications

- Multiple Client Connection
- Reject already used usernames
- Multiple Client File Upload


