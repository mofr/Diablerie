using System.Collections.Generic;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    public class WeaponHitClass
    {
        public static WeaponHitClass HandToHand;

        public string code;
        public SoundInfo sound;
        public SoundInfo hitSound;

        static Dictionary<string, WeaponHitClass> map = new Dictionary<string, WeaponHitClass>();

        static WeaponHitClass()
        {
            map.Add("hth", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_punch_1"),
                hitSound = SoundInfo.Find("impact_punch_1")
            });
            map.Add("1hss", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_1hs_small_1"),
                hitSound = SoundInfo.Find("impact_blade_swing_1")
            });
            map.Add("1hsl", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_1hs_large_1"),
                hitSound = SoundInfo.Find("impact_blade_swing_1")
            });
            map.Add("2hss", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_2hs_small_1"),
                hitSound = SoundInfo.Find("impact_blade_swing_1")
            });
            map.Add("2hsl", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_2hs_large_1"),
                hitSound = SoundInfo.Find("impact_blade_swing_1")
            });
            map.Add("1ht", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_1ht_1"),
                hitSound = SoundInfo.Find("impact_blade_thrust_1")
            });
            map.Add("2ht", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_2ht_1"),
                hitSound = SoundInfo.Find("impact_blade_thrust_1")
            });
            map.Add("club", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_1ht_1"),
                hitSound = SoundInfo.Find("impact_blunt_1")
            });
            map.Add("staf", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_staff_1"),
                hitSound = SoundInfo.Find("impact_blunt_1")
            });
            map.Add("bow", new WeaponHitClass()
            {
                sound = null,
                hitSound = null
            });
            map.Add("xbow", new WeaponHitClass()
            {
                sound = null,
                hitSound = null
            });
            map.Add("claw", new WeaponHitClass()
            {
                sound = SoundInfo.Find("weapon_punch_1"),
                hitSound = SoundInfo.Find("impact_claw_1")
            });

            foreach (var kv in map)
            {
                kv.Value.code = kv.Key;
            }

            HandToHand = map["hth"];
        }

        public static WeaponHitClass Find(string code)
        {
            return map.GetValueOrDefault(code);
        }
    }
}