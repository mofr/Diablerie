using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;
using Diablerie.Engine.World;

namespace Diablerie.Game.World
{
    public class Act2 : Act
    {
        public Act2()
        {
            palette = Palette.GetPalette(PaletteType.Act2);
            
            var town = new LevelBuilder("Act 2 - Town", palette);
            root = town.Instantiate(new Vector2i(0, 0));
            entry = town.FindEntry();
        }
    }
}