using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

using MySql.Data.MySqlClient;

//MySqlClient reference: https://dev.mysql.com/doc/connector-net/en/connector-net-tutorials-sql-command.html

namespace Server
{
    public class File1
    {
        public File1() { }

        public enum AccessType
        {
            PUBLIC = 0,
            PRIVATE = 1
        }

        public File1(int id, string fileName, string filePath, string owner, int counter, AccessType type)
        {
            this.Id = id;
            this.FileName = fileName;
            this.FilePath = filePath;
            this.Owner = owner;
            this.Counter = counter;
            this.FileAccessType = type;
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Owner { get; set; }
        public int Counter { get; set; }
        public AccessType FileAccessType { get; set; }

     
    }

    public static class FileUtils
    {
        public static File1.AccessType AccessTypeConverter(string val)
        {
            switch (val)
            {
                case "PUBLIC":
                    return File1.AccessType.PUBLIC;

                case "PRIVATE":
                    return File1.AccessType.PRIVATE;

                default:
                    throw new Exception("INVALID ACCESS TYPE INPUTTED");

            }
        }

        public static File1.AccessType AccessTypeConverter(int key)
        {
            switch (key)
            {
                case 0:
                    return File1.AccessType.PUBLIC;

                case 1:
                    return File1.AccessType.PRIVATE;

                default:
                    throw new Exception("INVALID ACCESS TYPE INPUTTED");

            }
        }
    }

    public static class Program
    {
        //Singleton (Lazy Initilization)
        private static MySqlConnection mySqlConnection = null;
        private const string CONNECTION_STRING = @"server=remotemysql.com;userid=ioI0xzbThf;password=VGITbQxEEa;database=ioI0xzbThf";

        //QUERIES
        private const string INSERT_SQL =
            "INSERT INTO FILES (fileName, filePath, owner, incCount, accessType) VALUES(@fileName, @filePath, @owner, @accessType)";

        private const string UPDATE_COUNT_SQL = "UPDATE FILES SET incCount = @newIncCount WHERE fileName = @fileName";
        private const string UPDATE_ACCESS_TYPE_SQL = "UPDATE FILES SET accessType = @newAccessType WHERE fileName = @fileName";

        private const string DELETE_FILE_BY_NAME_SQL = "DELETE FROM FILES WHERE fileName = @fileName";
        private const string DELETE_FILE_BY_ID_SQL = "DELETE FROM FILES WHERE id = @id";

        private const string GET_ALL_FILES_SQL = "SELECT * FROM FILES";
        private const string GET_FILES_BY_OWNER_SQL = "SELECT * FROM FILES WHERE owner = @owner";
        private const string GET_FILES_BY_ACCESS_TYPE_SQL = "SELECT * FROM FILES WHERE accessType = @accessType";
        private const string GET_ALL_FILE_NAMES_SQL = "SELECT fileName FROM FILES";
        private const string GET_PUBLIC_FILE_NAMES_SQL = "SELECT fileName FROM FILES WHERE  accessType = 'PUBLIC'";
        private const string GET_FILE_BY_NAME_SQL = "SELECT * FROM FILES WHERE fileName = @fileName";
        private const string GET_INC_COUNT_SQL = "SELECT incCount FROM FILES WHERE fileName = @fileName";
        public static MySqlConnection GetMySqlConnection()
        {
            if (mySqlConnection == null)
            {
                try
                {
                    mySqlConnection = new MySqlConnection(CONNECTION_STRING);
                    mySqlConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.ToString());
                    //Terminate program execution
                    //System.Environment.Exit(0);
                    throw e;
                }
            }

            if (mySqlConnection.State == ConnectionState.Closed || mySqlConnection.State == ConnectionState.Broken)
            {
                try
                {
                    mySqlConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.ToString());
                    throw e;
                }
            }

            return mySqlConnection;

        }

        public static void DumbDB()
        {

            MySqlConnection conn = GetMySqlConnection();

            string query = "SELECT * FROM FILES;";
            MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "table_name");
            DataTable dt = ds.Tables["table_name"];

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    Console.Write(row[col] + "\t");
                }

                Console.Write("\n");
            }
        }

        public static List<File1> GetAllFiles()
        {
            List<File1> fileList = new List<File1>();

            try
            {
                MySqlConnection conn = GetMySqlConnection();
                MySqlCommand cmd = new MySqlCommand(GET_ALL_FILES_SQL, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    File1 newFile = new File1(rdr.GetInt16(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt16(4), FileUtils.AccessTypeConverter(rdr.GetString(5)));
                    fileList.Add(newFile);
                }

                rdr.Close();
            }
            catch (Exception e)
            {
                //TO-DO
            }

            return fileList;
        }
        public static List<File1> GetFilesByOwner(String owner)
        {
            List<File1> fileList = new List<File1>();

            try
            {
                MySqlConnection conn = GetMySqlConnection();
                MySqlCommand cmd = new MySqlCommand(GET_FILES_BY_OWNER_SQL, conn);
                cmd.Parameters.AddWithValue("@owner", owner);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    File1 newFile = new File1(rdr.GetInt16(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt16(4), FileUtils.AccessTypeConverter(rdr.GetString(5)));
                    fileList.Add(newFile);
                }

                rdr.Close();
            }
            catch (Exception e)
            {
                //TO-DO
            }

            return fileList;
        }
        public static List<File1> GetFilesByAccessType(File1.AccessType accessType)
        {
            List<File1> fileList = new List<File1>();

            try
            {
                MySqlConnection conn = GetMySqlConnection();
                MySqlCommand cmd = new MySqlCommand(GET_FILES_BY_ACCESS_TYPE_SQL, conn);
                cmd.Parameters.AddWithValue("@accessType", accessType.ToString());
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    File1 newFile = new File1(rdr.GetInt16(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt16(4), FileUtils.AccessTypeConverter(rdr.GetString(5)));
                    fileList.Add(newFile);
                }

                rdr.Close();
            }
            catch (Exception e)
            {
                //TO-DO
            }

            return fileList;
        }
        public static File1 GetFileByName(String fileName)
        {
            MySqlDataReader rdr = null;
            File1 newFile = null;

            try
            {
                MySqlConnection conn = GetMySqlConnection();
                MySqlCommand cmd = new MySqlCommand(GET_FILE_BY_NAME_SQL, conn);
                cmd.Parameters.AddWithValue("@fileName", fileName);
                rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    rdr.Read();
                    newFile = new File1(rdr.GetInt16(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt16(4), FileUtils.AccessTypeConverter(rdr.GetString(5)));
                }

            }
            catch (Exception e)
            {
                //TO-DO
            }
            finally
            {
                if(rdr != null)
                {
                    rdr.Close();

                }
            }

            return newFile;

        }

        public static List<String> GetAllFileNames()
        {
            List<String> fileNames = new List<String>();
            MySqlDataReader rdr = null ;

            try
            {
                MySqlConnection conn = GetMySqlConnection();
                MySqlCommand cmd = new MySqlCommand(GET_ALL_FILE_NAMES_SQL, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    String fileName = rdr.GetString(0);
                    fileNames.Add(fileName);
                }

            }
            catch (Exception e)
            {
                //TO-DO
            }
            finally
            {
                if(rdr != null)
                {
                    rdr.Close();
                }
            }

            return fileNames;
        }
        public static List<String> GetPublicFileNames()
        {
            List<String> fileNames = new List<String>();
            MySqlDataReader rdr = null;

            try
            {
                MySqlConnection conn = GetMySqlConnection();
                MySqlCommand cmd = new MySqlCommand(GET_PUBLIC_FILE_NAMES_SQL, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    String fileName = rdr.GetString(0);
                    fileNames.Add(fileName);
                }
            }
            catch (Exception e)
            {
                //TO-DO
            }
            finally
            {
                if(rdr != null)
                {
                    rdr.Close();
                }
            }  

            return fileNames;
        }
        public static int? GetIncCountByName(String fileName)
        {
            try
            {
                MySqlConnection conn = GetMySqlConnection();
                MySqlCommand cmd = new MySqlCommand(GET_INC_COUNT_SQL, conn);
                cmd.Parameters.AddWithValue("@fileName", fileName);
                Object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    int incCount = Convert.ToInt32(result);
                    return incCount;
                }

            }
            catch (Exception e)
            {
                //TO-DO                
                Console.WriteLine("Error: {0}", e.ToString());

            }

            return null;
        }
        public static void InsertFile(String fileName, String filePath, String owner, File1.AccessType accessType)
        {
            try
            {
                MySqlConnection conn = GetMySqlConnection();

                MySqlCommand cmd = new MySqlCommand(INSERT_SQL, conn);
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@filePath", filePath);
                cmd.Parameters.AddWithValue("@owner", owner);
                cmd.Parameters.AddWithValue("@accessType", accessType);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
            }

        }
        public static void DeleteFileByName(String fileName)
        {
            try
            {
                MySqlConnection conn = GetMySqlConnection();

                MySqlCommand cmd = new MySqlCommand(DELETE_FILE_BY_NAME_SQL, conn);
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
            }
        }
        public static void UpdateAccessType(File1.AccessType newAccessType, String fileName)
        {
            try
            {
                MySqlConnection conn = GetMySqlConnection();

                MySqlCommand cmd = new MySqlCommand(UPDATE_ACCESS_TYPE_SQL, conn);
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@newAccessType", newAccessType.ToString());

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
            }
        }
        private static void UpdateCount(int? newCount, String fileName)
        {
            try
            {
                MySqlConnection conn = GetMySqlConnection();

                MySqlCommand cmd = new MySqlCommand(UPDATE_COUNT_SQL, conn);
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@newIncCount", newCount);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.ToString());
            }
        }
        public static void IncrementFileCount(String fileName)
        {
            int? preCount = GetIncCountByName(fileName);
            if (preCount != null)
            {
                int? newCount = preCount + 1;
                UpdateCount(newCount, fileName);

            }
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //InsertFile("testFile", "testPath", "testOwner", 1, File.AccessType.PRIVATE);
            //List<File> files = GetFilesByOwner("cankutcoskun");
            //String fileName = "cankutcoskun_myFile_0";
            //IncrementFileCount(fileName);
            //UpdateAccessType(File.AccessType.PRIVATE, fileName);
            //DeleteFileByName(fileName);
            //File returnedFile = GetFileByName("testFile");
            //List<String> publicFileNames = GetPublicFileNames();
            //List<String> fileNames = GetAllFileNames(); 
            //List<File> privateFiles = GetFilesByAccessType(File.AccessType.PRIVATE);
            //List<File> publicFiles = GetFilesByAccessType(File.AccessType.PUBLIC);
            //List<File> files = GetFilesByOwner("testOwner");
            //List<File> files = GetAllFiles();
            //DeleteFileByName("testFile");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }

}
