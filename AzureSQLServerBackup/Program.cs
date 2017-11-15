using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.SqlServer.Dac;
using Newtonsoft.Json;

namespace AzureSQLServerBackup
{
    class Program
    {
        private static void Main(string[] args)
        {
            Config cfg = new Config();
            const string path = @"AzureSQLServerBackup.config.json";

            // Create the config if not found
            if (!File.Exists(path))
            {
                // Create a file to write to.
                Config newConfig = new Config {BackupDirectory = @"C:\backups\"};
                Server newServer = new Server
                {
                    DbName = "Development",
                    HostName = "localhost",
                    UserName = "sa",
                    Password = "Password"
                };
                newConfig.Servers.Add(newServer);


                string jsonData = JsonConvert.SerializeObject(newConfig);

                try
                {
                    File.WriteAllText(path, jsonData);
                    LogMessage("Config created as " + path);
                    LogMessage("Rerun program to backup database after modifying config");
                }
                catch (Exception ex)
                {
                    LogMessage("Something went wrong. Exiting with error: ", ex);
                    Environment.Exit(29);
                }


                Environment.Exit(2);
            }

            // Open the config file and read it

            try
            {
                File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                LogMessage("Something went wrong. Exiting with error: ", ex);
                Environment.Exit(30);
            }

            // convert the JSON Data to .NET Objects
            try
            {
                dynamic result = JsonConvert.DeserializeObject(File.ReadAllText(path));
                cfg.BackupDirectory = result.BackupDirectory;
                foreach (var server in result.Servers)
                {
                    Server srv = new Server();
                    srv.Password = server.Password;
                    srv.DbName = server.DbName;
                    srv.HostName = server.HostName;
                    srv.UserName = server.UserName;
                    cfg.Servers.Add(srv);
                }
            }
            catch (Exception ex)
            {
                LogMessage("Something went wrong. Exiting with error: ", ex);
                Environment.Exit(29);
            }

            // Iterate through servers and back them up to .bacpac files
            foreach (Server server in cfg.Servers)
            {
                BackupServer(cfg.BackupDirectory, server);
            }

            Environment.Exit(0);
        }


        private static void BackupServer(string directory, Server server)
        {
            string filename = server.HostName + "_" + server.DbName + ".bacpac";

            // build the connection string to use for the backup
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder
            {
                DataSource = server.HostName,
                Password = server.Password,
                UserID = server.UserName,
                PersistSecurityInfo = false,
                TrustServerCertificate = true,
                IntegratedSecurity = false,
                InitialCatalog = server.DbName
            };
            
            try
            {
                LogMessage("Backup for " + server.DbName + "@" + server.HostName + " started");
                // connect to database
                DacServices ds = new DacServices(csb.ConnectionString);

                // backup
                ds.ExportBacpac(directory + filename, server.DbName);
            }
            catch (Exception ex)
            {
                LogMessage("Something went wrong. Error: ", ex);
            }

            LogMessage("Backup for " + server.DbName + "@" + server.HostName + " completed");
        }

        public static void LogMessage(string msg, Exception ex)
        {
            LogMessage(msg + " " + ex);
        }

        public static void LogMessage(string msg)
        {
            StreamWriter sw = File.AppendText("AzureSQLServerBackup.Log");
            try
            {
                string logLine = String.Format("{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
                Console.WriteLine(msg);
            }
            catch
            {
                Console.WriteLine("WARNING: Can't write to logfile");
            }
            finally
            {
                sw.Close();
            }
        }
    }
}
