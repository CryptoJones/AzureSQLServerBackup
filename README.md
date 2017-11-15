## Back Utility for Azure/SQL Server 2014

Written against .NET Framework 4.6

Purpose: to create **local** backup files for Azure SQL or Remote SQL Server 2014 databases.

Utility creates a .bacpac file for each server/database specified in AzureSQLServerBackup.config.json.

On first run it will create the config file for you. Edit the config with your backup directory, hostname, database, login and password and run the application again. You can specify multiple servers, just seperate them with commas. (See below)

On second run the program will create a .bacpac file backup for each server/database specified in the config file.

Once you have verified your configuration is correct, you can set the program to run as a schedualed task.

Error codes used;
* 0 - Success
* 1 - Failed to backup
* 2 - Configuration file written
* 29 - Couldn't write configuration file
* 30 - Couldn't read configuration file


## Example Configuration File

'''json
{"Servers":[
    {"HostName":"localhost","DbName":"Development","UserName":"sa","Password":"Password1"},
    {"HostName":"frosty.contoso.com","DbName":"Production","UserName":"sa","Password":"IL0v3G0@tS3X!"},
    {"HostName":"71.203.8.65","DbName":"WingTipToys","UserName":"root","Password":"L1nuX>M1cr0$0ftW1nd0ws"}
    ],"BackupDirectory":"C:\\backups\\"}
'''
