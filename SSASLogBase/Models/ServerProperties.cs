using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSASLogBase.Models
{
    public class AsAdministrators
    {
        public List<string> members { get; set; }
    }

    public class Properties
    {
        public AsAdministrators asAdministrators { get; set; }
        public string provisioningState { get; set; }
        public string serverFullName { get; set; }
        public string state { get; set; }
    }

    public class Sku
    {
        public int capacity { get; set; }
        public string name { get; set; }
        public string tier { get; set; }
    }

    public class Tags
    {
        public string testKey { get; set; }
    }

    public class ServerProperties
    {
        public string id { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public Properties properties { get; set; }
        public Sku sku { get; set; }
        public Tags tags { get; set; }
    }
}
