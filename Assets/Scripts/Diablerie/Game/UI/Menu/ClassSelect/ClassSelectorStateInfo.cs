using UnityEngine;

namespace Diablerie.Game.UI.Menu.ClassSelect
{
    public class ClassSelectorStateInfo
    {
        public Sprite[] Sprites { get; set; }
        
        public Sprite[] HoverSprites { get; set; }
        
        public Sprite[] OverlaySprites { get; set; }
        
        public bool Loop { get; set; }
        
        public bool HideOnFinish { get; set; }
        
        public int SortingOrderShift { get; set; }

        public int Fps { get; set; }
        
        public Material Material { get; set; }
        
        public string SfxPath { get; set; }
    }
}