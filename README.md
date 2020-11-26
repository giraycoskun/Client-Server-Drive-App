# CS 408 Project

---
## Database Connection Details

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
- Async PRograms: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/
- https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example

**GUI Elements**

- logBox
- portBox
- fileBox
- startButton
- stopButton

**Possible Issues in Server:**


	- Get IP adress of system: there are multiple IPs DNS. 
	 IPHostEntry ip = Dns.GetHostEntry(host); return multiple IP addresses in ip.AdressList
	 
	 
### Client