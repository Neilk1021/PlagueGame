using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory{
    public class ArmorUUID
    {
        string Helmet;
        string Torso;
        string Legs;

        public ArmorUUID(string CurrentHelmet, string CurrentTorso, string CurrentLegs)
        {
            Helmet = CurrentHelmet;
            Torso = CurrentTorso;
            Legs = CurrentLegs;
        }

        public string GetHelemet()
        {
            return Helmet;
        }
        public string GetTorso()
        {
            return Torso;
        }
        public string GetLegs()
        {
            return Legs;
        }
    }
}
