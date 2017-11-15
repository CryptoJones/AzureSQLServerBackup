using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.SqlServer.Dac;
using Newtonsoft.Json;

namespace AzureSQLServerBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            Config cfg = new Config();
            string path = @"AzureSQLServerBackup.config.json";

            // Create the config if not found
            if (!File.Exists(path))
            {
                // Create a file to write to.
                Config newConfig = new Config();
                newConfig.BackupDirectory = @"C:\backups\";
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
                    Console.WriteLine("Config created as " + path);
                    Console.WriteLine("Rerun program to backup database after modifying config!");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong. Exiting with error: " + ex);
                    Console.ReadLine();
                    Environment.Exit(29);
                }


                Environment.Exit(2);
            }

            // Open the file to read from.

            try
            {
                File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong. Exiting with error: " + ex);
                Console.ReadLine();
                Environment.Exit(30);
            }


            try
            {
                dynamic result = JsonConvert.DeserializeObject(File.ReadAllText(path));
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
                Console.WriteLine("Something went wrong. Exiting with error: " + ex);
                Console.ReadLine();
                Environment.Exit(29);
            }

            foreach (Server server in cfg.Servers)
            {
                BackupServer(cfg.BackupDirectory, server, server.HostName + DateTime.Now + ".bacpac");
            }

           Environment.Exit(0);
        }

        private static void BackupServer(string directory, Server server, string filename)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder
            {
                DataSource = server.HostName,
                Password = server.Password,
                UserID = server.UserName
            };

            try
            {
                DacServices ds = new DacServices(csb.ConnectionString);
                ds.ExportBacpac(directory + filename, server.DbName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong. Exiting with error: " + ex);
                Console.ReadLine();
                Environment.Exit(1);
            }

        }
    }
}
