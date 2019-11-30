using Diablerie.Engine.Utility;
using Diablerie.Engine.World;

namespace Diablerie.Game.World
{
    public class Act3 : Act
    {
        public Act3()
        {
            var town = new LevelBuilder("Act 3 - Town");
            root = town.Instantiate(new Vector2i(0, 0));
            entry = town.FindEntry();
        }
    }
}