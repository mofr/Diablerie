using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine
{
    public class CharStat : MonoBehaviour
    {
        public Unit unit;
        public int level = 1;
        public uint experience = 0;

        public uint nextLevelExp
        {
            get
            {
                return ExpTable.GetExperienceRequired(level + 1);
            }
        }

        public uint currentLevelExp
        {
            get
            {
                return ExpTable.GetExperienceRequired(level);
            }
        }

        public void AddExp(uint exp)
        {
            experience += exp;
            while (experience >= nextLevelExp)
            {
                LevelUp();
            }
        }

        void LevelUp()
        {
            AudioManager.instance.Play("cursor_level_up");
            level += 1;
            unit.health = unit.maxHealth;
            unit.mana = unit.maxMana;
        }
    }
}
