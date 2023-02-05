using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetroGlad
{
    public class EntityNPCStats : MonoBehaviour
    {
        //public float Level = 1; //Multiplier
        public float Strength = 1; //Auto Attack Damage
        public float Dexterity = 1; //Movement Speed and Auto Attack Interval
        public float Consitution = 1; //HP
        public float Intelligence = 1; //Mana pool and Spell Damage
        public float Wisdom = 1; //Mana Regen and CDR
        public float Physique = 1; //Mass and HP regen
        


        /*public float GetComputedStat(string statName)
        {
            float computedStat = 1;
            float multiplier = 0.05f;
            switch (statName)
            {
                case "Level":
                    computedStat = Level;
                    break;
                case "Strength":
                    computedStat = Strength * (1f + (multiplier * Level));
                    break;
                case "Dexterity":
                    computedStat = Dexterity * (1f + (multiplier * Level));
                    break;
                case "Constitution":
                    computedStat = Consitution * (1f + (multiplier * Level));
                    break;
                case "Intelligence":
                    computedStat = Intelligence * (1f + (multiplier * Level));
                    break;
                case "Wisdom":
                    computedStat = Wisdom * (1f + (multiplier * Level));
                    break;
                case "Physique":
                    computedStat = Physique * (1f + (multiplier * Level));
                    break;
                default:
                    Debug.Log("Invalid stat name");
                    computedStat = 0f;
                    break;
            }
            return computedStat;
        }*/

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
            float randomStat = stat * (1f + (0.05f * level)) +  Random.Range(0.0f, 3.0f) + Random.Range(0.0f, 1.5f*level);
            return randomStat;
        }
    }
}
