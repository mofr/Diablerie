using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;

namespace Diablerie.Engine
{
    public class Events
    {
        public delegate void DeathHandler(Character target, Character killer);
        public static event DeathHandler CharacterDied;

        public delegate void InteractHandler(Character target, Character initiator);
        public static event InteractHandler CharacterInteractionStarted;
        
        public delegate void LevelChangeHandler(LevelInfo level, LevelInfo previous);
        public static event LevelChangeHandler LevelChanged;

        public static void InvokeCharacterDied(Character target, Character killer)
        {
            CharacterDied?.Invoke(target, killer);
        }
        
        public static void InvokeCharacterInteractionStarted(Character target, Character initiator)
        {
            CharacterInteractionStarted?.Invoke(target, initiator);
        }

        public static void InvokeLevelChanged(LevelInfo level, LevelInfo previous)
        {
            LevelChanged?.Invoke(level, previous);
        }
    }
}
