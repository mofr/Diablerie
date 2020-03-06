using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Game
{
    public class MissileFunctions : MonoBehaviour
    {
        public void Awake()
        {
            Events.MissileMoved += OnMissileMove;
            Events.MissileHit += OnMissileHit;
            Events.MissileLifetimeEnd += OnMissileLifetimeEnd;
        }

        private static void OnMissileMove(Missile missile)
        {
            var info = missile.Info;
            var lifeTime = missile.LifeTime;
            var originator = missile.Originator;
            var pos = missile.Iso.pos;
            
            if (missile.Info.serverDoFunc == 15)
            {
                // Frozen orb
                int frequency = info.parameters[0].value * 25;
                float spawnPeriod = 1.0f / frequency;
                float directionIncrement = info.parameters[1].value * 25 * Mathf.PI;
                int missilesToSpawn = (int)((lifeTime + Time.deltaTime) / spawnPeriod) - (int)(lifeTime / spawnPeriod);
                for (int i = 0; i < missilesToSpawn; ++i)
                {
                    var dir = new Vector2(1, 0);
                    var rot = Quaternion.AngleAxis(lifeTime * directionIncrement, new Vector3(0, 0, 1));
                    var offset = (Vector2) (rot * dir);
                    Missile.Create(info.clientSubMissileId[0], pos, pos + offset, originator);
                }
            }
        }

        private static void OnMissileHit(Missile missile, Vector2 hitPos, GameObject gameObject)
        {
            var info = missile.Info;
            var originator = missile.Originator;
            
            Unit hitUnit = null;
            if (gameObject != null)
            {
                hitUnit = gameObject.GetComponent<Unit>();
                if (hitUnit != null)
                {
                    int damage = CalcDamage(missile);
                    hitUnit.Hit(damage, originator);
                    if (info.progOverlay != null)
                        Overlay.Create(hitUnit.gameObject, info.progOverlay, false);
                }
            }
            if (info.explosionMissile != null)
                Missile.Create(info.explosionMissile, hitPos, hitPos, originator);

            AudioManager.instance.Play(info.hitSound, Iso.MapToWorld(hitPos));
            AudioManager.instance.Play(SoundInfo.GetHitSound(info.hitClass, hitUnit), Iso.MapToWorld(hitPos));

            if (info.clientHitFunc == "14")
            {
                // glacial spike, freezing arrow
                Missile.Create(info.clientHitSubMissileId[0], hitPos, hitPos, originator);
                int subMissileCount = Random.Range(3, 5);
                for (int i = 0; i < subMissileCount; ++i)
                {
                    var offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    Missile.Create(info.clientHitSubMissileId[1], hitPos, hitPos - offset, originator);
                }
            }
            else if (info.serverHitFunc == "29")
            {
                // Frozen orb
                Missile.CreateRadially(info.clientHitSubMissileId[0], missile.Iso.pos, originator, 16);
            }
            
            // todo pierce is actually is pierce skill with a chance to pierce
            if ((!info.pierce || hitUnit == null) && info.collideKill)
            {
                Destroy(missile.gameObject);
            }
        }

        private static int CalcDamage(Missile missile)
        {
            int damage = missile.weaponDamage;
            var info = missile.Info;
            var skillInfo = missile.SkillInfo;

            // todo take skill level into account
            if (missile.SkillInfo != null)
            {
                damage += Random.Range(skillInfo.eMin, skillInfo.eMax + 1) + Random.Range(skillInfo.minDamage, skillInfo.maxDamage + 1);
            }
            else
            {
                damage += Random.Range(info.eMin, info.eMax + 1) + Random.Range(info.minDamage, info.maxDamage + 1);
            }

            return damage;
        }

        private void OnMissileLifetimeEnd(Missile missile)
        {
            var info = missile.Info;
            var iso = missile.Iso;
            var originator = missile.Originator;
            
            if (info.serverHitFunc == "29")
            {
                Missile.CreateRadially(info.clientHitSubMissileId[0], iso.pos, originator, 16);
            }
        }
    }
}
