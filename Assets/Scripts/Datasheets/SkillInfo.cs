using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SkillInfo
{
    public static SkillInfo Attack;

    public enum Range
    {
        NoRestrictions,
        Melee,
        Ranged,
        Both
    }

    public string skill;
    public int id = -1;
    public string charClass;
    public string skillDesc;
    public int srvStartFunc;
    public int srvDoFunc;
    [Datasheet.Sequence(length = 54)]
    public string[] unused;
    public string summon;
    public string pettype;
    public string petmax; // formula
    public string summode;
    [Datasheet.Sequence(length = 10)]
    public string[] sumcalcs;
    public string sumumod;
    public string sumoverlay;
    public bool stsuccessonly;
    public string _stsound;
    [Datasheet.Sequence(length = 10)]
    public string[] unused2;
    public string castOverlayId;
    public string clientOverlayA;
    public string clientOverlayB;
    public int clientStartFunc;
    public int clientDoFunc;
    public int clientPrqFunc1;
    public int clientPrqFunc2;
    public int clientPrqFunc3;
    public string clientMissile;
    public string clientMissileA;
    public string clientMissileB;
    public string clientMissileC;
    public string clientMissileD;
    [Datasheet.Sequence(length = 6)]
    public string[] clientCalcs;
    public bool warp;
    public bool immediate;
    public bool enhanceable;
    public int attackRank;
    public bool noAmmo;
    public string _range;
    public int weapSel;
    public string itemTypeA1;
    public string itemTypeA2;
    public string itemTypeA3;
    public string eItemTypeA1;
    public string eItemTypeA2;
    public string itemTypeB1;
    public string itemTypeB2;
    public string itemTypeB3;
    public string eItemTypeB1;
    public string eItemTypeB2;
    public string anim;
    public string seqTrans;
    public string monAnim;
    public int seqNum;
    public string seqInput;
    public bool durability;
    public bool useAttackRate;
    public int lineOfSight;
    public bool targetableOnly;
    public bool searchEnemyXY;
    public bool searchEnemyNear;
    public bool searchOpenXY;
    public int selectProc;
    public bool targetCorpse;
    public bool targetPet;
    public bool targetAlly;
    public bool targetItem;
    [Datasheet.Sequence(length = 77)]
    public string[] unused3;
    public int hitShift;
    public int srcDamage;

    // todo move damage fields to separate structure and share it with MissileInfo
    public int minDamage;
    [Datasheet.Sequence(length = 5)]
    public int[] minDamagePerLevel;
    public int maxDamage;
    [Datasheet.Sequence(length = 5)]
    public int[] maxDamagePerLevel;
    public string damageSymPerCalc;
    public string eType;
    public int eMin;
    [Datasheet.Sequence(length = 5)]
    public int[] minEDamagePerLevel;
    public int eMax;
    [Datasheet.Sequence(length = 5)]
    public int[] maxEDamagePerLevel;
    public string eDamageSymPerCalc;

    public int eLen;
    [Datasheet.Sequence(length = 3)]
    public int[] eLenPerLevel;
    public string eLenSymPerCalc;
    public int aiType;
    public int aiBonus;
    public int costMult;
    public int costAdd;

    [System.NonSerialized]
    public OverlayInfo castOverlay;

    [System.NonSerialized]
    public SoundInfo startSound;

    [System.NonSerialized]
    public Range range;

    [System.NonSerialized]
    public string name;

    [System.NonSerialized]
    public string iconSpritesheetFilename;

    [System.NonSerialized]
    public int iconIndex;

    public static List<SkillInfo> sheet = Datasheet.Load<SkillInfo>("data/global/excel/Skills.txt");
    static Dictionary<string, SkillInfo> map = new Dictionary<string, SkillInfo>();
    static Dictionary<int, SkillInfo> idMap = new Dictionary<int, SkillInfo>();

    static SkillInfo()
    {
        int charOffset = 0;
        string charClass = null;

        foreach (var row in sheet)
        {
            if (row.id == -1)
                continue;

            if (charClass != row.charClass)
            {
                charClass = row.charClass;
                charOffset = row.id;
            }

            row.name = Translation.Find("skillname" + row.id);
            if (row.charClass != null)
                row.iconSpritesheetFilename = @"data\global\ui\spells\" + row.charClass.Substring(0, 2) + "Skillicon";
            else
                row.iconSpritesheetFilename = @"data\global\ui\spells\Skillicon";
            row.iconIndex = row.id - charOffset;
            row.castOverlay = OverlayInfo.Find(row.castOverlayId);
            row.startSound = SoundInfo.Find(row._stsound);
            if (row._range == "none")
                row.range = Range.NoRestrictions;
            else if (row._range == "h2h")
                row.range = Range.Melee;
            else if (row._range == "rng")
                row.range = Range.Ranged;
            else if (row._range == "both")
                row.range = Range.Both;
            else
                throw new System.Exception("Unknown skill range " + row._range);
            map.Add(row.skill, row);
            idMap.Add(row.id, row);
        }

        Attack = Find("Attack");
    }

    public static SkillInfo Find(string id)
    {
        if (id == null)
            return null;
        var result = map.GetValueOrDefault(id);
        if (result != null)
            return result;

        int intId;
        if (int.TryParse(id, out intId))
            return Find(intId);

        return null;
    }

    public static SkillInfo Find(int id)
    {
        return idMap.GetValueOrDefault(id);
    }

    public bool IsRangeOk(Character self, Character targetCharacter, Vector2 targetPoint)
    {
        if (targetCharacter != null)
        {
            targetPoint = targetCharacter.iso.pos;
        }

        float radius = self.attackRange + self.size / 2;
        if (targetCharacter != null)
            radius += targetCharacter.size / 2;

        return range == SkillInfo.Range.NoRestrictions ||
            Vector2.Distance(self.iso.pos, targetPoint) <= radius;
    }

    public void Do(Character self, Character targetCharacter, Vector3 target)
    {
        if (srvDoFunc == 27)
        {
            // teleport
            self.InstantMove(target);
        }

        if (srvDoFunc == 1)
        {
            Item weapon = self.equip == null ? null : self.equip.GetWeapon();
            int damage = 10;
            WeaponHitClass hitClass = WeaponHitClass.HandToHand;
            bool ranged = false;
            if (weapon != null)
            {
                WeaponInfo weaponInfo = weapon.info.weapon;
                hitClass = weaponInfo.hitClass;
                if (weaponInfo.twoHanded)
                    damage = Random.Range(weaponInfo.twoHandedMinDamage, weaponInfo.twoHandedMaxDamage + 1);
                else
                    damage = Random.Range(weaponInfo.minDamage, weaponInfo.maxDamage + 1);

                ranged = weapon.info.type.shoots != null;
            }

            if (ranged)
            {
                var missile = Missile.Create("arrow", target, self);
                missile.weaponDamage = damage;
            }
            else if (targetCharacter != null && IsRangeOk(self, targetCharacter, target))
            {
                AudioManager.instance.Play(hitClass.hitSound, targetCharacter.transform.position);
                targetCharacter.TakeDamage(damage, self);
            }
        }
        else if (srvDoFunc == 17)
        {
            // charged bold, bolt sentry
            int boltCount = 7;
            for (int i = 0; i < boltCount; ++i)
            {
                var offset = new Vector3(Random.Range(-boltCount / 2f, boltCount / 2f), Random.Range(-boltCount / 2f, boltCount / 2f));
                Missile.Create(clientMissileA, self.iso.pos, target + offset, self);
            }
        }
        else if (srvDoFunc == 22)
        {
            // nova, poison nova, howl
            int missileCount = 64;
            Missile.CreateRadially(clientMissileA, self.iso.pos, self, missileCount);
        }
        else if (srvDoFunc == 31)
        {
            // raise skeleton, raise skeletal mage
            var pos = Iso.MapToWorld(target);
            World.SpawnMonster(summon, pos);
            Missile.Create(clientMissileA, target, target, self);
        }
        else if (srvDoFunc == 68)
        {
            // barbarian howls
            int missileCount = 64;
            Missile.CreateRadially(clientMissileA, self.iso.pos, self, missileCount);
        }
        else
        {
            if (clientMissileA != null)
            {
                Missile.Create(clientMissileA, self.iso.pos, target, self);
            }
        }

        if (clientMissile != null)
        {
            Missile.Create(clientMissile, self.iso.pos, target, self);
        }
    }

    public Sprite GetIcon()
    {
        Sprite sprite = Spritesheet.Load(iconSpritesheetFilename).GetSprites(0)[iconIndex * 2];
        return sprite;
    }

    public Sprite GetPressedIcon()
    {
        Sprite sprite = Spritesheet.Load(iconSpritesheetFilename).GetSprites(0)[iconIndex * 2 + 1];
        return sprite;
    }
}
