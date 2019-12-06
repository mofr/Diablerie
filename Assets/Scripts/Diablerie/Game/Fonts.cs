using UnityEngine;

namespace Diablerie.Game
{
    public class Fonts
    {
        private static Font font16;
        private static Font font24;
        private static Font font30;
        private static Font font42;
        
        public static Font GetFont16()
        {
            if (font16 == null)
                font16 = Resources.Load<Font>("Fonts/font16");
            return font16;
        }
        
        public static Font GetFont24()
        {
            if (font24 == null)
                font24 = Resources.Load<Font>("Fonts/font24");
            return font24;
        }
        
        public static Font GetFont30()
        {
            if (font30 == null)
                font30 = Resources.Load<Font>("Fonts/font30");
            return font30;
        }
        
        public static Font GetFont42()
        {
            if (font42 == null)
                font42 = Resources.Load<Font>("Fonts/font42");
            return font42;
        }
    }
}