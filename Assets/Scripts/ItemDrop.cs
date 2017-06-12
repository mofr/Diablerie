using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    struct QualityFactors
    {
        public int unique;
        public int set;
        public int rare;
        public int magic;
    }

    void OnEnable()
    {
        Character.OnDeath += OnCharacterDeath;
    }

    private void OnCharacterDeath(Character target, Character killer)
    {
        if (target.monStat == null)
            return;
        
        TreasureClass tc = target.monStat.treasureClass[0].normal;
        tc = tc.Upgraded(target.level);
        Drop(tc, target.transform.position, target.level);
    }

    public static bool TestQuality(Item item, int baseChance, int divisor, int qualityFactor, int mf, int mfFactor, int minChance = 0)
    {
        int chance = (baseChance - (item.level - item.info.level) / divisor) * 128;
        if (mfFactor > 0)
        {
            int effectiveMF = mf * mfFactor / (mf + mfFactor);
            chance = chance * 100 / (100 + effectiveMF);
        }
        int finalChance = chance - (chance * qualityFactor / 1024);
        finalChance = Mathf.Max(finalChance, minChance);
        int sample = Random.Range(0, finalChance + 1);
        return sample < 128;
    }

    static void SelectItemQuality(Item item, QualityFactors qualityFactors, int mf)
    {
        if (item.info.alwaysUnique)
        {
            item.quality = Item.Quality.Unique;
            return;
        }

        if (item.info.type.alwaysNormal)
        {
            item.quality = Item.Quality.Normal;
            return;
        }

        if (item.info.type.alwaysMagic)
        {
            item.quality = Item.Quality.Magic;
            return;
        }

        bool classSpecific = item.info.type.classCode != null;
        bool uber = item.info.code != item.info.normCode;
        var ratio = ItemRatio.sheet.Find(r => r.classSpecific == classSpecific && r.uber == uber);

        if (TestQuality(item, ratio.unique, ratio.uniqueDivisor, qualityFactors.unique, mf, mfFactor: 250, minChance: ratio.uniqueMin))
        {
            item.quality = Item.Quality.Unique;
        }
        else if (TestQuality(item, ratio.set, ratio.setDivisor, qualityFactors.set, mf, mfFactor: 500, minChance: ratio.setMin))
        {
            item.quality = Item.Quality.Set;
        }
        else if (item.info.type.canBeRare && TestQuality(item, ratio.rare, ratio.rareDivisor, qualityFactors.rare, mf, mfFactor: 600, minChance: ratio.rareMin))
        {
            item.quality = Item.Quality.Rare;
        }
        else if (TestQuality(item, ratio.magic, ratio.magicDivisor, qualityFactors.magic, mf, mfFactor: 600, minChance: ratio.magicMin))
        {
            item.quality = Item.Quality.Magic;
        }
        else if (TestQuality(item, ratio.hiQuality, ratio.hiQualityDivisor, qualityFactor: 0, mf: 0, mfFactor: 0))
        {
            item.quality = Item.Quality.HiQuality;
        }
        else if (TestQuality(item, ratio.normal, ratio.normalDivisor, qualityFactor: 0, mf: 0, mfFactor: 0))
        {
            item.quality = Item.Quality.Normal;
        }
        else
        {
            item.quality = Item.Quality.LowQuality;
        }
    }

    static UniqueItem SelectUniqueForItem(Item item)
    {
        int probSum = 0;
        foreach (var unique in item.info.uniques)
        {
            probSum += unique.rarity;
        }
        int sample = Random.Range(0, probSum);
        foreach (var unique in item.info.uniques)
        {
            if (sample < unique.rarity && item.level >= unique.level)
            {
                return unique;
            }

            sample -= unique.rarity;
        }

        return null;
    }

    static bool GenerateUnique(Item item)
    {
        if (item.info.uniques.Count == 0)
            return false;

        UniqueItem unique = SelectUniqueForItem(item);
        if (unique == null)
            return false;

        item.name = unique.name;
        item.unique = unique;
        item.levelReq = unique.levelReq;

        foreach (var mod in unique.props)
        {
            if (mod.prop == null)
                break;

            var prop = new Item.Property();
            prop.info = ItemPropertyInfo.Find(mod.prop);
            prop.param = mod.param;
            prop.value = Random.Range(mod.min, mod.max + 1);
            prop.min = mod.min;
            prop.max = mod.max;
            item.properties.Add(prop);
        }

        return true;
    }

    static void GenerateItemProperties(Item item)
    {
        if (item.quality == Item.Quality.Unique && !GenerateUnique(item))
        {
            item.quality = Item.Quality.Rare;
        }

        if (item.quality == Item.Quality.Magic)
        {
            int rand = Random.Range(0, 4);
            if (rand < 2)
            {
                var prefixes = MagicAffix.GetSpawnableAffixes(item, MagicAffix.prefixes);
                var prefix = prefixes[Random.Range(0, prefixes.Count)];
                AddAffix(item, prefix);
                item.name = prefix.name + " " + item.name;
            }
            if (rand != 1)
            {
                var suffixes = MagicAffix.GetSpawnableAffixes(item, MagicAffix.suffixes);
                var suffix = suffixes[Random.Range(0, suffixes.Count)];
                AddAffix(item, suffix);
                item.name += " " + suffix.name;
            }
        }
        else if (item.quality == Item.Quality.Rare)
        {
            var affixes = MagicAffix.GetSpawnableAffixes(item, MagicAffix.all);
            for (int i = 0; i < 4; ++i)
            {
                var affix = affixes[Random.Range(0, affixes.Count)];
                AddAffix(item, affix);
            }
        }
    }

    private static void AddAffix(Item item, MagicAffix affix)
    {
        foreach (var mod in affix.mods)
        {
            if (mod.code == null)
                break;
            var prop = new Item.Property();
            prop.info = ItemPropertyInfo.Find(mod.code);
            prop.param = mod.param;
            prop.value = Random.Range(mod.min, mod.max + 1);
            prop.min = mod.min;
            prop.max = mod.max;
            prop.classSpecific = affix.classSpecific;
            item.properties.Add(prop);
        }
    }

    public static void Drop(string code, Vector3 pos, int itemLevel)
    {
        Drop(code, pos, itemLevel, new QualityFactors());
    }

    static void Drop(string code, Vector3 pos, int itemLevel, QualityFactors qualityFactors = new QualityFactors())
    {
        var treasureClass = TreasureClass.Find(code);
        if (treasureClass == null)
        {
            var item = Item.Create(code);
            if (item == null)
            {
                Debug.LogWarning("Item wasn't spawned: " + code);
                return;
            }
            item.level = itemLevel;
            SelectItemQuality(item, qualityFactors, mf: 1000);
            if (item.quality != Item.Quality.HiQuality && 
                item.quality != Item.Quality.Normal &&
                item.quality != Item.Quality.LowQuality)
            {
                item.identified = false;
            }
            GenerateItemProperties(item);
            Pickup.Create(pos, item);
        }
        else
        {
            Drop(treasureClass, pos, itemLevel, qualityFactors);
        }
    }

    static void Drop(TreasureClass treasureClass, Vector3 pos, int itemLevel, QualityFactors qualityFactors = new QualityFactors())
    {
        qualityFactors.unique = Mathf.Max(qualityFactors.unique, treasureClass.unique);
        qualityFactors.set = Mathf.Max(qualityFactors.set, treasureClass.set);
        qualityFactors.rare = Mathf.Max(qualityFactors.rare, treasureClass.rare);
        qualityFactors.magic = Mathf.Max(qualityFactors.magic, treasureClass.magic);

        int noDrop = 0; // value is temporary just to increase drop for debugging purposes, should be treasureClass.noDrop
        for (int i = 0; i < treasureClass.picks; ++i)
        {
            int sample = Random.Range(0, treasureClass.probSum + noDrop);
            foreach (var node in treasureClass.nodeArray)
            {
                if (node.code == null)
                    break;

                if (sample < node.prob)
                {
                    Drop(node.code, pos, itemLevel, qualityFactors);
                    break;
                }

                sample -= node.prob;
            }
        }

        for (int i = 0; i < -treasureClass.picks; ++i)
        {
            var node = treasureClass.nodeArray[i];
            if (node.code == null)
                break;
            for (int j = 0; j < node.prob; ++j)
            {
                Drop(node.code, pos, itemLevel, qualityFactors);
            }
        }
    }
}
