using UnityEngine;

namespace RetroGlad
{
    public class StatSheet
    {
        public float Level = 1; //Multiplier
        public float Strength = 1; //Auto Attack Damage
        public float Dexterity = 1; //Movement Speed and Auto Attack Interval
        public float Consitution = 1; //HP
        public float Intelligence = 1; //Mana pool and Spell Damage
        public float Wisdom = 1; //Mana Regen and CDR
        public float Physique = 1; //Mass and HP regen
        //public int Armor = 1;

        public void Load(SquadMember squadMember)
        {
            Level = squadMember.Level;
            Strength = squadMember.Strength;
            Dexterity = squadMember.Dexterity;
            Consitution = squadMember.Constitution;
            Intelligence = squadMember.Intelligence;
            Wisdom = squadMember.Wisdom;
            Physique = squadMember.Physique;
        }      

        public float GetComputedStat(CharacterAttribute statName)
        {
            float computedStat = 1;
            float multiplier = 0.05f;
            switch (statName)
            {
                case CharacterAttribute.Level:
                    computedStat = Level;
                    break;
                case CharacterAttribute.Strength:
                    computedStat = Strength*(1f + (multiplier * Level));
                    break;
                case CharacterAttribute.Dexterity:
                    computedStat = Dexterity * (1f + (multiplier * Level));
                    break;
                case CharacterAttribute.Consitution:
                    computedStat = Consitution * (1f + (multiplier * Level));
                    break;
                case CharacterAttribute.Intelligence:
                    computedStat = Intelligence * (1f + (multiplier * Level));
                    break;
                case CharacterAttribute.Wisdom:
                    computedStat = Wisdom * (1f + (multiplier * Level));
                    break;
                case CharacterAttribute.Physique:
                    computedStat = Physique * (1f + (multiplier * Level));
                    break;
                default:
                    Debug.Log("Invalid stat name");
                    computedStat = 0f;
                    break;
            }
            return computedStat;
        }

        public void IncreaseStat(CharacterAttribute statName, float amount) //prevent calling if the cost multiplier is above crystals
        {
            switch (statName)
            {
                case CharacterAttribute.Level:
                    Level += amount;
                    break;
                case CharacterAttribute.Strength:
                    Strength += amount;
                    break;
                case CharacterAttribute.Dexterity:
                    Dexterity += amount;
                    break;
                case CharacterAttribute.Consitution:
                    Consitution += amount;
                    break;
                case CharacterAttribute.Intelligence:
                    Intelligence += amount;
                    break;
                case CharacterAttribute.Wisdom:
                    Wisdom += amount;
                    break;
                case CharacterAttribute.Physique:
                    Physique += amount;
                    break;
                default:
                    Debug.Log("Invalid stat name");
                    break;
            }
        }

        //for AI
        public void RandStatsBoost(float level)
        {
            Strength += RandomizeLevelAdjust(Strength, level);
            Dexterity += RandomizeLevelAdjust(Dexterity, level);
            Consitution += RandomizeLevelAdjust(Consitution, level);
            Intelligence += RandomizeLevelAdjust(Intelligence, level);
            Wisdom += RandomizeLevelAdjust(Wisdom, level);
            Physique += RandomizeLevelAdjust(Physique, level);

        }

        private float RandomizeLevelAdjust(float stat, float level)
        {
            float randomStat = stat + UnityEngine.Random.Range(0.0f, 3.0f) + UnityEngine.Random.Range(0.0f, 1.5f * level);
            return randomStat;
        }

    }
}
