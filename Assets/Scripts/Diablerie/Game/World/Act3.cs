using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;
using Diablerie.Engine.World;

namespace Diablerie.Game.World
{
    public class Act3 : Act
    {
        public Act3()
        {
            palette = Palette.GetPalette(PaletteType.Act3);
            
            var town = new LevelBuilder("Act 3 - Town", palette);
            root = town.Instantiate(new Vector2i(0, 0));
            entry = town.FindEntry();
        }
    }
}