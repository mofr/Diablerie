using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine
{
    public class Events
    {
        public delegate void InteractHandler(Unit target, Unit initiator);
        public static event InteractHandler UnitInteractionStarted;
        
        public delegate void UnitInitializedHandler(Unit unit);
        public static event UnitInitializedHandler UnitInitialized;
        
        public delegate void UnitStartedSkillHandler(Unit unit, SkillInfo skillInfo);
        public static event UnitStartedSkillHandler UnitStartedSkill;
        
        public delegate void UnitTookDamageHandler(Unit unit, int damage);
        public static event UnitTookDamageHandler UnitTookDamage;
        
        public delegate void UnitDiedHandler(Unit unit, Unit killer);
        public static event UnitDiedHandler UnitDied;

        public delegate void LevelChangeHandler(LevelInfo level, LevelInfo previous);
        public static event LevelChangeHandler LevelChanged;

        public delegate void LootFlippedHandler(Loot loot);
        public static event LootFlippedHandler LootFlipped; // Consider using something more generic like entity mode

        public delegate void StaticObjectOperateHandler(StaticObject staticObject, Unit unit);
        public static event StaticObjectOperateHandler StaticObjectOperate;

        public delegate void MissileMovedHandler(Missile missile);
        public static event MissileMovedHandler MissileMoved;

        public delegate void MissileHitHandler(Missile missile, Vector2 pos, GameObject gameObject);
        public static event MissileHitHandler MissileHit;

        public delegate void MissileLifetimeEndHandler(Missile missile);
        public static event MissileLifetimeEndHandler MissileLifetimeEnd;

        public static void InvokeUnitInteractionStarted(Unit target, Unit initiator)
        {
            UnitInteractionStarted?.Invoke(target, initiator);
        }

        public static void InvokeUnitInitialized(Unit unit)
        {
            UnitInitialized?.Invoke(unit);
        }

        public static void InvokeUnitStartedSkill(Unit unit, SkillInfo skillInfo)
        {
            UnitStartedSkill?.Invoke(unit, skillInfo);
        }

        public static void InvokeUnitTookDamage(Unit unit, int damage)
        {
            UnitTookDamage?.Invoke(unit, damage);
        }

        public static void InvokeUnitDied(Unit unit, Unit killer)
        {
            UnitDied?.Invoke(unit, killer);
        }

        public static void InvokeLevelChanged(LevelInfo level, LevelInfo previous)
        {
            LevelChanged?.Invoke(level, previous);
        }

        public static void InvokeLootFlipped(Loot loot)
        {
            LootFlipped?.Invoke(loot);
        }

        public static void InvokeStaticObjectOperate(StaticObject staticObject, Unit unit)
        {
            StaticObjectOperate?.Invoke(staticObject, unit);
        }

        public static void InvokeMissileMoved(Missile missile)
        {
            MissileMoved?.Invoke(missile);
        }

        public static void InvokeMissileHit(Missile missile, Vector2 pos, GameObject gameObject)
        {
            MissileHit?.Invoke(missile, pos, gameObject);
        }

        public static void InvokeMissileLifetimeEnd(Missile missile)
        {
            MissileLifetimeEnd?.Invoke(missile);
        }
    }
}
