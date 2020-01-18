using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;
using Diablerie.Engine.World;

namespace Diablerie.Game.World
{
    public class Act5 : Act
    {
        public Act5()
        {
            palette = Palette.GetPalette(PaletteType.Act5);
            
            var town = new LevelBuilder("Act 5 - Town", palette);
            root = town.Instantiate(new Vector2i(0, 0));
            entry = town.FindEntry();
        }
    }
}