using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public enum Belts {
        white,
        blackbelt,
    bluebeltblackstripe,
    bluebelt,
    brownbeltblackstripe,
    brownbelt,
    greenbeltblackstripe,
    greenbelt,
    yellowbeltblackstripe,
    yellowbelt,
    }
   
    [Serializable]
    public class Fighter
    {
        public string created_at;
        public string Name;
        public int id;
        public int Status;
    }
    [Serializable]
    public class ActionCard 
    {
        public int id;
        public string Belt;
        public string Type;
        public string sub_type;
        public string FileName;
        public string Path;
        public DateTime created_at;
    }

    [Serializable]
    public class UserProfile
    {
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public string facebook_id { get; set; }
        public string name { get; set; }
        public string device { get; set; }
        public string device_id { get; set; }
        public Belts belt_type { get; set; }
        public string email { get; set; }
        public string firebase_token { get; set; }
        public List<ActionCard> _allActionCards;

        UserProfile()
        {
            _allActionCards = new List<ActionCard>();
            
        }
    }
}
