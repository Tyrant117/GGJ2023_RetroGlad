using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class CompanyManager
    {
        public Team Team;
        public float Gold;
        public List<SquadMember> Squad;

        public CompanyManager(CompanySaveData save)
        {
            Team = save.Team;
            Gold = save.Gold;
            Squad = new(save.Squad.Count);
            foreach (var member in save.Squad)
            {
                Squad.Add(new(member));
            }
        }

        public void ClearMercs()
        {
            Squad = new();
            Save(this);
        }

        public static CompanyManager New()
        {
            var save = new CompanySaveData()
            {
                Team = Team.Blue,
                Gold = 500,
                Squad = new(),
            };
            CompanyManager cm = new(save);
            Save(cm);
            return cm;
        }

        public static CompanyManager Load()
        {
            var path = Application.persistentDataPath + "/SaveData.json";
            if (!System.IO.File.Exists(path))
            {
                var save = new CompanySaveData()
                {
                    Team = Team.Blue,
                    Gold = 500,
                    Squad = new(),
                };
                return new(save);
            }
            else
            {
                string jsonFile = System.IO.File.ReadAllText(path);
                var save = JsonUtility.FromJson<CompanySaveData>(jsonFile);
                return new(save);
            }
        }

        public static void Save(CompanyManager manager)
        {
            var sd = new CompanySaveData()
            {
                Team = manager.Team,
                Gold = manager.Gold,
                Squad = new(),
            };

            foreach (var member in manager.Squad)
            {
                sd.Squad.Add(new()
                {
                    Name = member.Name,
                    Class = member.Class,
                    Level = member.Level,
                    Strength = member.Strength,
                    Dexterity = member.Dexterity,
                    Constitution = member.Constitution,
                    Physique = member.Physique,
                    Intelligence = member.Intelligence,
                    Wisdom = member.Wisdom,
                });
            }

            var path = Application.persistentDataPath + "/SaveData.json";
            var jsonC = JsonUtility.ToJson(sd, true);
            System.IO.File.WriteAllText(path, jsonC);
        }        
    }

    public class SquadMember
    {
        public string Name;
        public Class Class;
        public float Level;
        public float Strength; // Attack Damage
        public float Dexterity; // Attack Speed, Movement Speed
        public float Constitution; // HP
        public float Physique; // Mass and HP5
        public float Intelligence; // Mana and Spell Damage
        public float Wisdom; // MP5 and Spell Cooldown Reduction

        public SquadMember(MemberSaveData save)
        {
            Name = save.Name;
            Class = save.Class;
            Level = save.Level;
            Strength = save.Strength;
            Dexterity = save.Dexterity;
            Constitution = save.Constitution;
            Physique = save.Physique;
            Intelligence = save.Intelligence;
            Wisdom = save.Wisdom;
        }
    }

    [System.Serializable]
    public class CompanySaveData
    {
        public float Gold;
        public Team Team;
        public List<MemberSaveData> Squad;
    }

    [System.Serializable]
    public class MemberSaveData
    {
        public string Name;
        public Class Class;
        public float Level;
        public float Strength; // Attack Damage
        public float Dexterity; // Attack Speed, Movement Speed
        public float Constitution; // HP
        public float Physique; // Mass and HP5
        public float Intelligence; // Mana and Spell Damage
        public float Wisdom; // MP5 and Spell Cooldown Reduction
    }
}
