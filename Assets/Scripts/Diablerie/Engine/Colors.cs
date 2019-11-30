using UnityEngine;

namespace Diablerie.Engine
{
    public class Colors
    {
        public static readonly Color InvItemBackground = new Color(0.1f, 0.1f, 0.3f, 0.3f);
        public static readonly Color InvItemHighlight = new Color(0.1f, 0.3f, 0.1f, 0.3f);
        public static readonly Color InvItemHighlightForbid = new Color(0.3f, 0.1f, 0.1f, 0.3f);
        public static readonly Color InvItemHighlightSwap = new Color(0.3f, 0.3f, 0.3f, 0.3f);

        public static readonly Color ItemCrafted = new Color32(255, 165, 0, 255);
        public static readonly Color ItemMagic = new Color32(65, 105, 225, 255);
        public static readonly Color ItemRare = new Color32(204, 204, 82, 255);
        public static readonly Color ItemSet = new Color32(0, 255, 0, 255);
        public static readonly Color ItemUnique = new Color32(165, 146, 99, 255);

        public static readonly string ItemCraftedHex = "ffa800";
        public static readonly string ItemMagicHex = "6969ff";
        public static readonly string ItemRareHex = "ffff64";
        public static readonly string ItemSetHex = "00ff00";
        public static readonly string ItemUniqueHex = "c7b377";
        public static readonly string ItemLowQualityHex = "696969";
        public static readonly string ItemNormalHex = "ffffff";
        public static readonly string ItemRedHex = "ff6464";
    }
}