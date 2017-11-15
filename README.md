## Back Utility for Azure/SQL Server 2014

Written against .NET Framework 4.6

Utility creates a .bacpac file for each server/database specified in AzureSQLServerBackup.config.json.

On first run it will create the config file for you. Edit the config with your backup directory, hostname, database, login and password and run the application again.

On second run the program will create a .bacpac file backup for each server/database specified in the config file.
