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
        public string Photo;
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
        public float result;
        public int fighter_id;
        public int fighter_video_id;
        public int player_id;
        public ComparisonResults comparison_results;
        public ActionCard()
        {
            id = 0;
            created_at = DateTime.Now;
        }
    }
    [Serializable]
    public class ComparisonResults
    {
        public float Head;
        public float LShoulder;
        public float RShoulder;
        public float LElbow;
        public float RElbow;
        public float LWrist;
        public float RWrist;
        public float LHip;
        public float RHip;
        public float LKnee;
        public float RKnee;
        public float LAnkle;
        public float RAnkle;

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
        public List<Friends> _allFriends;
        public FightCombo _myCombo;
        UserProfile()
        {
            _allActionCards = new List<ActionCard>();
            
        }
    }
}
