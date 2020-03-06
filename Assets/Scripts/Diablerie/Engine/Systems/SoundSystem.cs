using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.Systems
{
    public class SoundSystem : MonoBehaviour
    {
        private void Awake()
        {
            Events.UnitInitialized += OnUnitInitialized;
            Events.UnitStartedSkill += OnUnitStartedSkill;
            Events.UnitTookDamage += OnUnitTookDamage;
            Events.UnitDied += OnUnitDied;
            Events.LootFlipped += OnLootFlipped;
        }

        private void OnUnitInitialized(Unit unit)
        {
            if (unit.monStat != null)
                AudioManager.instance.Play(unit.monStat.sound.init, unit.transform);
        }

        private void OnUnitStartedSkill(Unit unit, SkillInfo skillInfo)
        {
            AudioManager.instance.Play(skillInfo.startSound, unit.transform);

            if (skillInfo == SkillInfo.Attack)
            {
                if (unit.monStat != null)
                {
                    AudioManager.instance.Play(unit.monStat.sound.weapon1, unit.transform, 
                        delay: unit.monStat.sound.weapon1Delay, volume: unit.monStat.sound.weapon1Volume);
                    AudioManager.instance.Play(unit.monStat.sound.attack1, unit.transform, 
                        delay: unit.monStat.sound.attack1Delay);
                }
                else
                {
                    Item weapon = unit.equip == null ? null : unit.equip.GetWeapon();
                    WeaponHitClass hitClass = WeaponHitClass.HandToHand;
                    if (weapon != null)
                        hitClass = weapon.info.weapon.hitClass;
                    AudioManager.instance.Play(hitClass.sound, unit.transform);
                }
            }
        }

        private void OnUnitTookDamage(Unit unit, int damage)
        {
            if (unit.monStat != null)
                AudioManager.instance.Play(unit.monStat.sound.hit, unit.transform, unit.monStat.sound.hitDelay);
        }

        private void OnUnitDied(Unit unit, Unit killer)
        {
            if (unit.monStat != null)
                AudioManager.instance.Play(unit.monStat.sound.death, unit.transform, unit.monStat.sound.deathDelay);
        }

        private void OnLootFlipped(Loot loot)
        {
            AudioManager.instance.Play(SoundInfo.itemFlippy, loot.transform);
            AudioManager.instance.Play(loot.item.dropSound, loot.transform, delay: loot.item.dropSoundDelay);
        }
    }
}