using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;

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
    }
}
