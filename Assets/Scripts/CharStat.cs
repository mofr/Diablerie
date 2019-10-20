using UnityEngine;

public class CharStat : MonoBehaviour
{
    public Character character;
    public CharStatsInfo info;
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
        character.health = character.maxHealth;
        character.mana = character.maxMana;
    }
}
