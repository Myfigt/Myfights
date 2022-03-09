using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    [Serializable]
    public class Fighter
    {
        public string created_at;
        public string Name;
        public int id;
        public int Status;
    }
    public class Fighter_Videos
    {
        public int id;
        public string Belt;
        public string Type;
        public string sub_type;
        public string FileName;
        public string Path;
        public DateTime created_at;
    }
}
