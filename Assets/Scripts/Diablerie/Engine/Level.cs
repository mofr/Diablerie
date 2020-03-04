using Diablerie.Engine.Datasheets;
using Diablerie.Engine.UI;
using UnityEngine;

namespace Diablerie.Engine
{
    public class Level : MonoBehaviour
    {
        public static LevelInfo Current => _current;
        
        private static LevelInfo _current;
        public LevelInfo info;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag != "Player")
                return;

            if (_current != null)
                LevelEntryTitle.Show("Entering " + info.levelName);

            var previous = _current;
            _current = info;
            Events.InvokeLevelChanged(_current, previous);
        }
    }
}
