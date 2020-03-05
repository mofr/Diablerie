using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;

namespace Diablerie.Engine
{
    public class Events
    {
        public delegate void DeathHandler(Unit target, Unit killer);
        public static event DeathHandler UnitDied;

        public delegate void InteractHandler(Unit target, Unit initiator);
        public static event InteractHandler UnitInteractionStarted;
        
        public delegate void LevelChangeHandler(LevelInfo level, LevelInfo previous);
        public static event LevelChangeHandler LevelChanged;

        public static void InvokeUnitDied(Unit target, Unit killer)
        {
            UnitDied?.Invoke(target, killer);
        }
        
        public static void InvokeUnitInteractionStarted(Unit target, Unit initiator)
        {
            UnitInteractionStarted?.Invoke(target, initiator);
        }

        public static void InvokeLevelChanged(LevelInfo level, LevelInfo previous)
        {
            LevelChanged?.Invoke(level, previous);
        }
    }
}
