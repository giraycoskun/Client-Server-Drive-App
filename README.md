# CS 408 Project

---
## Database Details

Visual Studio .NET mysql setup
- 1-)Mysql for visual studio -> https://dev.mysql.com/downloads/file/?id=491638
- 2-)Mysql connector .NET -> https://dev.mysql.com/downloads/connector/net/
- 3-)VS -> Server -> Tools-> connect to Database ->  

- host:remotemysql.com
- username: ioI0xzbThf
- password: VGITbQxEEa

Public Static Objects for DB:
- connectionstring = "server=remotemysql.com;userid=ioI0xzbThf;password=VGITbQxEEa;database=ioI0xzbThf"
- MySqlConnection databaseConnection = new MySqlConnection(connectionString);

Common Functions:
  - ??

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

- startButton_Click: Opens Server and realys to <ins>Accept()</ins>

- stopButton_Click: stops server

- browseButton_Click: opens file browser and sets the chosen path as <ins>fileDirectory</ins>

- Accept: get incoming connection and relay to R<ins>Receive()</ins>

- Receive: get username message and check decide to accept or reject if rejected disconnect else relay to <ins>handleClient()</ins>

- checkUsername: basic check is username in list

- handleClient: fiel upload and other processes in a loop

**Globals**

- portNum: int
- MAX_CLIENT: int
- listening: bool
- fileDirectory: string
- server: Socket
- ipAdress: IPAdress
- clientSocketList: list<Socket>
- usernameLİst: <string>

**Possible Issues in Server:**

- Get IP adress of system: there are multiple IPs DNS.IPHostEntry ip = Dns.GetHostEntry(host); return multiple IP addresses in ip.AdressList
	 
---	 
### Client

**Helpful Docs:**

**GUI Elements**

- ipBox: ip adress input
- portBox: port number input
- usernameBox: username input

**Functions**

- connectButton_Click: starts connection -> sends username -> if not accepted closes
- stopButton_Click: stops connection
- button1_Click: (browse button) opens file browser via openFileDialog1 sets uploadFileBox 
- uploadButton_Click: sends file from buffer

**Globals**

- serverIPAdress: IPAdress
- serverPortNum: int
- username: string
- filepath: string
- clientSocket: Socket

**Possible Issues in Server:**
