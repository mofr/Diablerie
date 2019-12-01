using System.Collections.Generic;
using System.Text;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Engine
{
    public class Item
    {
        public enum Quality
        {
            Unique,
            Set,
            Rare,
            Magic,
            HiQuality,
            Normal,
            LowQuality
        }

        public struct Property
        {
            public ItemPropertyInfo info;
            public string param;
            public int value;
            public int min;
            public int max;
            public string classSpecific;
        }

        public readonly ItemInfo info;
        public string name;
        public Quality quality = Quality.Normal;
        public int level;
        public int levelReq;
        public List<Property> properties = new List<Property>();
        public List<List<Property>> setItemProperties = new List<List<Property>>();
        public bool identified = true;
        public UniqueItem unique;
        public SetItem setItem;
        public int quantity = 1;
        Sprite _invSprite;
        bool _invSpriteIdentified;
        int invFileIndex;

        public static Item Create(string code)
        {
            var info = ItemInfo.Find(code);
            if (info == null)
                return null;

            return new Item(info);
        }

        public Item(ItemInfo info)
        {
            this.info = info;
            name = info.name;
            levelReq = info.levelReq;
            if (info.type.varInvGfx > 0)
            {
                invFileIndex = Random.Range(0, info.type.varInvGfx);
            }
        }

        public string invFile
        {
            get
            {
                string filename;
                if (unique != null && unique.invFile != null && identified)
                {
                    filename = unique.invFile;
                }
                else if (setItem != null && setItem.invFile != null && identified)
                {
                    filename = setItem.invFile;
                }
                else if (info.type.varInvGfx > 0)
                {
                    filename = info.type.invGfx[invFileIndex];
                }
                else
                {
                    filename = info.invFile;
                }

                filename = @"data\global\items\" + filename + ".dc6";
                return filename;
            }
        }

        public Sprite invSprite
        {
            get
            {
                if (_invSprite == null || _invSpriteIdentified != identified)
                {
                    var dc6 = DC6.Load(invFile);
                    _invSprite = dc6.GetSprites(0)[0];
                    _invSpriteIdentified = identified;
                }
                return _invSprite;
            }
        }

        public string flippyFile
        {
            get
            {
                string filename;
                if (setItem != null && setItem.flippyFile != null)
                {
                    filename = setItem.flippyFile;
                }
                else if (unique != null && unique.flippyFile != null)
                {
                    filename = unique.flippyFile;
                }
                else
                {
                    filename = info.flippyFile;
                }

                filename = @"data\global\items\" + filename + ".dc6";
                return filename;
            }
        }

        public SoundInfo useSound
        {
            get
            {
                if (setItem != null && setItem.useSound != null)
                {
                    return setItem.useSound;
                }
                else if (unique != null && unique.useSound != null)
                {
                    return unique.useSound;
                }
                else
                {
                    return info.useSound;
                }
            }
        }

        public SoundInfo dropSound
        {
            get
            {
                if (setItem != null && setItem.dropSound != null)
                {
                    return setItem.dropSound;
                }
                else if (unique != null && unique.useSound != null)
                {
                    return unique.dropSound;
                }
                else
                {
                    return info.dropSound;
                }
            }
        }

        public int dropSoundFrame
        {
            get
            {
                if (setItem != null && setItem.dropSoundFrame != -1)
                {
                    return setItem.dropSoundFrame;
                }
                else if(unique != null && unique.dropSoundFrame != -1)
                {
                    return unique.dropSoundFrame;
                }
                else
                {
                    return info.dropSoundFrame;
                }
            }
        }

        void StartColor(StringBuilder sb, string color)
        {
            sb.Append("<color=#");
            sb.Append(color);
            sb.Append(">");
        }

        void EndColor(StringBuilder sb)
        {
            sb.Append("</color>");
        }

        void AppendColored(StringBuilder sb, string str, string color)
        {
            StartColor(sb, color);
            sb.Append(str);
            EndColor(sb);
        }

        public string GetTitle()
        {
            var sb = new StringBuilder();
            string color = Colors.ItemNormalHex;
            switch (quality)
            {
                case Quality.Unique: color = Colors.ItemUniqueHex; break;
                case Quality.Rare: color = Colors.ItemRareHex; break;
                case Quality.Set: color = Colors.ItemSetHex; break;
                case Quality.Magic: color = Colors.ItemMagicHex; break;
                case Quality.LowQuality: color = Colors.ItemLowQualityHex; break;
            }
            if (quality == Quality.HiQuality)
                AppendColored(sb, Translation.Find("Hiquality") + " ", color);
            else if (quality == Quality.LowQuality)
                AppendColored(sb, Translation.Find("Low Quality") + " ", color);
            if (info.code == "gld")
            {
                AppendColored(sb, quantity.ToString(), color);
                sb.Append(" ");
            }
            if (identified)
            {
                AppendColored(sb, name, color);
                if (quality == Quality.Unique || quality == Quality.Set)
                {
                    AppendColored(sb, "\n" + info.name, color);
                }
            }
            else
            {
                AppendColored(sb, info.name, color);
            }
            return sb.ToString();
        }

        public string GetDescription()
        {
            var sb = new StringBuilder();
            sb.Append(GetTitle());

            if (info.weapon != null)
            {
                if (info.weapon.missileType != null)
                    sb.Append("\nThrow Damage: " + info.weapon.missileMinDamage + " to " + info.weapon.missileMaxDamage);
                if (!info.weapon.twoHanded || info.weapon.oneOrTwoHanded)
                    sb.Append("\nOne-Hand Damage: " + info.weapon.minDamage + " to " + info.weapon.maxDamage);
                if (info.weapon.twoHanded)
                    sb.Append("\nTwo-Hand Damage: " + info.weapon.twoHandedMinDamage + " to " + info.weapon.twoHandedMaxDamage);
                if (!info.weapon.noDurability)
                    sb.Append("\nDurability: " + info.weapon.durability);
                if (info.weapon.reqDex > 0)
                    sb.Append("\nRequired Dexterity: " + info.weapon.reqDex);
                if (info.weapon.reqStr > 0)
                    sb.Append("\nRequired Strength: " + info.weapon.reqStr);
            }

            if (info.armor != null)
            {
                sb.Append("\nDefense: " + info.armor.minAC);
                if (!info.armor.noDurability)
                    sb.Append("\nDurability: " + info.armor.durability);
            }
        
            var onlyForClass = CharStatsInfo.FindByCode(info.type.classCode);
            if (onlyForClass != null)
            {
                bool classMatched = CurrentCharacterClass() == onlyForClass;
                if (!classMatched)
                    StartColor(sb, Colors.ItemRedHex);
                sb.Append("\n(" + onlyForClass.className + " only)");
                if (!classMatched)
                    EndColor(sb);
            }

            if (info.armor != null)
            {
                if (info.armor.reqStr > 0)
                    sb.Append("\nRequired Strength: " + info.armor.reqStr);
            }

            if (levelReq > 0)
                sb.Append("\nRequired Level: " + levelReq);

            if (identified)
            {
                DescribeProperties(sb);
                DescribeSet(sb);
            }
            else
            {
                AppendColored(sb, "\nUnidentified", Colors.ItemRedHex);
            }

            return sb.ToString();
        }

        private void DescribeProperties(StringBuilder sb)
        {
            StartColor(sb, Colors.ItemMagicHex);
            foreach (var prop in properties)
            {
                DescribeProperty(sb, prop);
            }
            EndColor(sb);
        }

        private void DescribeProperty(StringBuilder sb, Property prop)
        {
            foreach (var block in prop.info.blocks)
            {
                int value = prop.value;

                if (block.func == 5)
                {
                    sb.Append("\n");
                    sb.Append("+");
                    sb.Append(value);
                    sb.Append(" To Minimum Damage");
                    continue;
                }
                else if (block.func == 6)
                {
                    sb.Append("\n");
                    sb.Append("+");
                    sb.Append(value);
                    sb.Append(" To Maximum Damage");
                    continue;
                }
                else if (block.func == 7)
                {
                    sb.Append("\n");
                    sb.Append("+");
                    sb.Append(value);
                    sb.Append("% Enhanced Damage");
                    continue;
                }
                else if (block.func == 14)
                {
                    sb.Append("\nSocketed (");
                    sb.Append(prop.value);
                    sb.Append(")");
                }
                else if (block.func == 17)
                {
                    int characterLevel = 10;
                    int perLevel = prop.param != null ? int.Parse(prop.param) : 1;
                    value = perLevel * characterLevel;
                }
                if (block.stat == null)
                    continue;

                string sign = value > 0 ? "+" : "";
                string str1 = value > 0 ? block.stat.descPositive : block.stat.descNegative;
                sb.Append("\n");
                if (block.stat.descFunc == 1 || block.stat.descFunc == 12)
                {
                    if (block.stat.descVal == 1)
                    {
                        sb.Append(sign);
                        sb.Append(value);
                        sb.Append(" ");
                    }
                    sb.Append(str1);
                    if (block.stat.descVal == 2)
                    {
                        sb.Append(" ");
                        sb.Append(sign);
                        sb.Append(value);
                    }
                }
                else if (block.stat.descFunc == 2)
                {
                    if (block.stat.descVal == 1)
                    {
                        sb.Append(value);
                        sb.Append("% ");
                    }
                    sb.Append(str1);
                    if (block.stat.descVal == 2)
                    {
                        sb.Append(" ");
                        sb.Append(value);
                        sb.Append("%");
                    }
                }
                else if (block.stat.descFunc == 3)
                {
                    if (block.stat.descVal == 1)
                    {
                        sb.Append(value);
                        sb.Append(" ");
                    }
                    sb.Append(str1);
                    if (block.stat.descVal == 2)
                    {
                        sb.Append(" ");
                        sb.Append(value);
                    }
                }
                else if (block.stat.descFunc == 4)
                {
                    if (block.stat.descVal == 1)
                    {
                        sb.Append(sign);
                        sb.Append(value);
                        sb.Append("% ");
                    }
                    sb.Append(str1);
                    if (block.stat.descVal == 2)
                    {
                        sb.Append(" ");
                        sb.Append(sign);
                        sb.Append(value);
                        sb.Append("%");
                    }
                }
                else if (block.stat.descFunc == 5)
                {
                    sb.Append(value * 100 / 128);
                    sb.Append("% ");
                    sb.Append(str1);
                }
                else if (block.stat.descFunc == 6)
                {
                    sb.Append(sign);
                    sb.Append(value);
                    sb.Append(" ");
                    sb.Append(str1);
                    sb.Append(" ");
                    sb.Append(block.stat.desc2);
                }
                else if (block.stat.descFunc == 7)
                {
                    sb.Append(value);
                    sb.Append("% ");
                    sb.Append(str1);
                    sb.Append(" ");
                    sb.Append(block.stat.desc2);
                }
                else if (block.stat.descFunc == 8)
                {
                    sb.Append(sign);
                    sb.Append(value);
                    sb.Append("% ");
                    sb.Append(str1);
                    sb.Append(" ");
                    sb.Append(block.stat.desc2);
                }
                else if (block.stat.descFunc == 9)
                {
                    sb.Append(value);
                    sb.Append(" ");
                    sb.Append(str1);
                    sb.Append(" ");
                    sb.Append(block.stat.desc2);
                }
                else if (block.stat.descFunc == 10)
                {
                    sb.Append(value * 100 / 128);
                    sb.Append("% ");
                    sb.Append(str1);
                    sb.Append(" ");
                    sb.Append(block.stat.desc2);
                }
                else if (block.stat.descFunc == 11)
                {
                    sb.Append("Repairs 1 Durability In ");
                    sb.Append(100 / value);
                    sb.Append(" Seconds");
                }
                else if (block.stat.descFunc == 13)
                {
                    sb.Append("+");
                    sb.Append(value);
                    sb.Append(" to ");
                    string className = CharStatsInfo.sheet[block.value].className;
                    sb.Append(className);
                    sb.Append(" Skill Levels");
                }
                else if (block.stat.descFunc == 14)
                {
                    string skillTabId = prop.param;
                    var charStat = CharStatsInfo.FindByCode(prop.classSpecific);
                    string className = charStat != null ? charStat.className : "NULL";
                    sb.Append(sign);
                    sb.Append(value);
                    sb.Append(" to ");
                    sb.Append("skilltab" + skillTabId);
                    sb.Append(" Skill Levels (");
                    sb.Append(className);
                    sb.Append(" Only)");
                }
                else if (block.stat.descFunc == 15)
                {
                    string skillId = prop.param;
                    var skillInfo = SkillInfo.Find(skillId);
                    int chance = prop.min;
                    int skillLevel = prop.max;
                    sb.Append(chance);
                    sb.Append("% to cast Level ");
                    sb.Append(skillLevel);
                    sb.Append(" ");
                    sb.Append(skillInfo.name);
                    sb.Append(" on ");
                    sb.Append(block.stat.itemEvent1);
                }
                else if (block.stat.descFunc == 16)
                {
                    sb.Append("Level [sLvl] [skill] Aura When Equipped");
                }
                else if (block.stat.descFunc == 17)
                {
                    sb.Append("[value] [string1] (Increases near [time]) ");
                }
                else if (block.stat.descFunc == 18)
                {
                    sb.Append("[value]% [string1] (Increases near [time])");
                }
                else if (block.stat.descFunc == 19)
                {
                    sb.Append("sprintf");
                }
                else if (block.stat.descFunc == 20)
                {
                    sb.Append(-value);
                    sb.Append("% ");
                    sb.Append(str1);
                }
                else if (block.stat.descFunc == 21)
                {
                    sb.Append(-value);
                    sb.Append(" ");
                    sb.Append(str1);
                }
                else if (block.stat.descFunc == 22)
                {
                    sb.Append("[value]% [string1] [montype]");
                }
                else if (block.stat.descFunc == 23)
                {
                    sb.Append("[value]% [string1] [monster]");
                }
                else if (block.stat.descFunc == 24)
                {
                    int chargeCount = -prop.min;
                    int skillLevel = -prop.max;
                    string skillId = prop.param;
                    var skillInfo = SkillInfo.Find(skillId);
                    sb.Append("Level ");
                    sb.Append(skillLevel);
                    sb.Append(" ");
                    sb.Append(skillInfo.name);
                    sb.Append(" (");
                    sb.Append(chargeCount);
                    sb.Append("/");
                    sb.Append(chargeCount);
                    sb.Append(" Charges)");
                }
                else if (block.stat.descFunc == 27)
                {
                    string skillId = prop.param;
                    var skillInfo = SkillInfo.Find(skillId);
                    var className = CharStatsInfo.FindByCode(skillInfo.charClass).className;
                    sb.Append("+");
                    sb.Append(value);
                    sb.Append(" to ");
                    sb.Append(skillInfo.name);
                    sb.Append(" (");
                    sb.Append(className);
                    sb.Append(" Only)");
                }
                else if (block.stat.descFunc == 28)
                {
                    string skillId = prop.param;
                    var skillInfo = SkillInfo.Find(skillId);
                    sb.Append("+");
                    sb.Append(value);
                    sb.Append(" to ");
                    sb.Append(skillInfo.name);
                }
                else
                {
                    AppendColored(sb, block.statId + "(descFunc " + block.stat.descFunc + ")" + ": " + str1, Colors.ItemLowQualityHex);
                }
            }
        }

        private void DescribeSet(StringBuilder sb)
        {
            if (setItem == null)
                return;

            int equippedCount = EquippedItemsCount(setItem.set);
            int setItemPropCount = equippedCount - 1;
            if (setItemPropCount > 0)
            {
                StartColor(sb, Colors.ItemSetHex);
                for(int i = 0; i < setItemPropCount && i < setItemProperties.Count; ++i)
                {
                    foreach (var prop in setItemProperties[i])
                    {
                        DescribeProperty(sb, prop);
                    }
                }
                EndColor(sb);

                sb.Append("\n");
                StartColor(sb, Colors.ItemUniqueHex);
                for (int i = 0; i < setItemPropCount && i < setItem.set.props.Length / 2; ++i)
                {
                    for (int j = i * 2; j < i * 2 + 2; ++j)
                    {
                        var mod = setItem.set.props[j];
                        if (mod.prop == null)
                            continue;
                        DescribeProperty(sb, mod.GetItemProperty());
                    }
                }
                if (equippedCount == setItem.set.items.Count)
                {
                    foreach(var mod in setItem.set.fullProps)
                    {
                        if (mod.prop == null)
                            continue;
                        DescribeProperty(sb, mod.GetItemProperty());
                    }
                }
                EndColor(sb);
            }

            sb.Append("\n\n");
            AppendColored(sb, setItem.set.name, Colors.ItemUniqueHex);
            foreach(var item in setItem.set.items)
            {
                sb.Append("\n");
                bool collected = PlayerController.instance.equip.IsEquipped(item.itemInfo);
                string color = collected ? Colors.ItemSetHex : Colors.ItemRedHex;
                AppendColored(sb, item.name, color);
            }
        }

        private static CharStatsInfo CurrentCharacterClass()
        {
            return PlayerController.instance.charStat.info;
        }

        private static int EquippedItemsCount(ItemSet set)
        {
            int result = 0;
            Equipment equip = PlayerController.instance.equip;
            foreach(var setItem in set.items)
            {
                if (equip.IsEquipped(setItem.itemInfo))
                    result += 1;
            }
            return result;
        }
    }
}
