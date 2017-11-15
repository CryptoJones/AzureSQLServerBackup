using System.Collections.Generic;

namespace AzureSQLServerBackup
{
    class Config
    {
        public List<Server> Servers = new List<Server>();
        public string BackupDirectory { get; set; }
    }
}
