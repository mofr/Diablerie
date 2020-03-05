using System.Collections.Generic;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;
using UnityEngine;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
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
        public string skillDescId;
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
        public string shortDescription;

        [System.NonSerialized]
        public string longDescription;

        [System.NonSerialized]
        public string shortName;

        [System.NonSerialized]
        private string _iconSpritesheetFilename;

        [System.NonSerialized]
        private SkillDescription _description;

        public static List<SkillInfo> sheet;
        static Dictionary<string, SkillInfo> map;
        static Dictionary<int, SkillInfo> idMap;

        public static void Load()
        {
            sheet = Datasheet.Load<SkillInfo>("data/global/excel/Skills.txt");
            map = new Dictionary<string, SkillInfo>();
            idMap = new Dictionary<int, SkillInfo>();
            foreach (var row in sheet)
            {
                if (row.id == -1)
                    continue;
            
                if (row.charClass != null)
                    row._iconSpritesheetFilename = @"data\global\ui\spells\" + row.charClass.Substring(0, 2) + "Skillicon";
                else
                    row._iconSpritesheetFilename = @"data\global\ui\spells\Skillicon";
                row.castOverlay = OverlayInfo.Find(row.castOverlayId);
                row.startSound = SoundInfo.Find(row._stsound);
                row._description = SkillDescription.Find(row.skillDescId);
                if (row._description != null)
                {
                    row.name = Translation.Find(row._description.strName);
                    row.shortDescription = Translation.Find(row._description.strShort);
                    row.longDescription = Translation.Find(row._description.strLong);
                    row.shortName = Translation.Find(row._description.strAlt);
                }
                else
                {
                    row.name = row.skill;
                }
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

        public bool IsRangeOk(Unit self, Unit targetUnit, Vector2 targetPoint)
        {
            if (targetUnit != null)
            {
                targetPoint = targetUnit.iso.pos;
            }

            float radius = self.attackRange + self.size / 2;
            if (targetUnit != null)
                radius += targetUnit.size / 2;

            return range == SkillInfo.Range.NoRestrictions ||
                   Vector2.Distance(self.iso.pos, targetPoint) <= radius;
        }

        public void Do(Unit self, Unit targetUnit, Vector3 target)
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
                else if (targetUnit != null && IsRangeOk(self, targetUnit, target))
                {
                    AudioManager.instance.Play(hitClass.hitSound, targetUnit.transform.position);
                    targetUnit.TakeDamage(damage, self);
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
                var monster = global::Diablerie.Game.World.WorldBuilder.SpawnMonster(summon, pos, summoner: self);
                monster.overrideMode = summode;
                Missile.Create(clientMissileA, target, target, self);
            }
            else if (srvDoFunc == 45)
            {
                // traps
                var pos = Iso.MapToWorld(target);
                var monster = global::Diablerie.Game.World.WorldBuilder.SpawnMonster(summon, pos, summoner: self);
                monster.overrideMode = summode;
                Missile.Create(clientMissileA, target, target, self);
            }
            else if (srvDoFunc == 56 || srvDoFunc == 57)
            {
                // 57 iron golem, 56 other golems
                var pos = Iso.MapToWorld(target);
                var monster = global::Diablerie.Game.World.WorldBuilder.SpawnMonster(summon, pos, summoner: self);
                monster.overrideMode = summode;
            }
            else if (srvDoFunc == 114)
            {
                // raven
                var pos = Iso.MapToWorld(target);
                var monster = global::Diablerie.Game.World.WorldBuilder.SpawnMonster(summon, pos, summoner: self);
                monster.overrideMode = summode;
            }
            else if (srvDoFunc == 119)
            {
                // summon grizzly, wolves, spirits
                var pos = Iso.MapToWorld(target);
                var monster = global::Diablerie.Game.World.WorldBuilder.SpawnMonster(summon, pos, summoner: self);
                monster.overrideMode = summode;
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
            if (_description == null)
                return null;
            Sprite sprite = Spritesheet.Load(_iconSpritesheetFilename).GetSprites(0)[_description.iconIndex];
            return sprite;
        }

        public Sprite GetPressedIcon()
        {
            if (_description == null)
                return null;
            Sprite sprite = Spritesheet.Load(_iconSpritesheetFilename).GetSprites(0)[_description.iconIndex + 1];
            return sprite;
        }
    }
}
