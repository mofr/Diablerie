using UnityEngine;

public class Experience : MonoBehaviour
{
    void Start()
    {
        Character.OnDeath += OnCharacterDeath;
    }

    void OnCharacterDeath(Character target, Character killer)
    {
        if (target.monStat != null && killer.charStat != null)
        {
            killer.charStat.AddExp(target.monStat.stats[0].exp);
        }
    }
}
