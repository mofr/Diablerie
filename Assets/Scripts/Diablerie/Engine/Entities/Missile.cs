using Diablerie.Engine.Datasheets;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    public class Missile : MonoBehaviour
    {
        public int weaponDamage = 0;

        private SpriteRenderer _renderer;
        private SpriteAnimator _animator;
        private Iso _iso;
        private Vector2 _dir;
        private float _speed;
        private MissileInfo _info;
        private SkillInfo _skillInfo;
        private Unit _originator;
        private float _lifeTime;

        public Unit Originator => _originator;
        
        public Iso Iso => _iso;

        public MissileInfo Info => _info;
        
        public SkillInfo SkillInfo => _skillInfo;
        
        public float LifeTime => _lifeTime;

        public static Missile Create(string missileId, Vector3 target, Unit originator)
        {
            return Create(missileId, originator.iso.pos, target, originator);
        }

        public static Missile Create(string missileId, Vector3 start, Vector3 target, Unit originator)
        {
            var missileInfo = MissileInfo.Find(missileId);
            if (missileInfo == null)
            {
                Debug.LogWarning("missile not found: " + missileId);
                return null;
            }

            return Create(missileInfo, start, target, originator);
        }

        public static void CreateRadially(string missileId, Vector3 start, Unit originator, int missileCount)
        {
            var missileInfo = MissileInfo.Find(missileId);
            if (missileInfo == null)
            {
                Debug.LogWarning("missile not found: " + missileId);
            }

            CreateRadially(missileInfo, start, originator, missileCount);
        }

        public static void CreateRadially(MissileInfo missileInfo, Vector3 start, Unit originator, int missileCount)
        {
            float angle = 0;
            float angleStep = 360.0f / missileCount;
            var dir = new Vector3(1, 0);
            for (int i = 0; i < missileCount; ++i)
            {
                var rot = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
                Missile.Create(missileInfo, start, start + rot * dir, originator);
                angle += angleStep;
            }
        }

        public static Missile Create(MissileInfo missileInfo, Vector3 start, Vector3 target, Unit originator)
        {
            var gameObject = new GameObject("missile_" + missileInfo.missile);
            var missile = gameObject.AddComponent<Missile>();
            missile._animator = gameObject.AddComponent<SpriteAnimator>();
            missile._renderer = gameObject.GetComponent<SpriteRenderer>();
            missile._iso = gameObject.AddComponent<Iso>();
        
            missile._info = missileInfo;
            missile._originator = originator;
            missile._speed = missileInfo.velocity;
            missile._iso.pos = start;
            missile._dir = (target - start).normalized;
            missile._renderer.material = missileInfo.material;
            missile._skillInfo = SkillInfo.Find(missileInfo.skill);
            missile._lifeTime = 0;

            var spritesheet = Spritesheet.Load(missileInfo.spritesheetFilename);
            int direction = Iso.Direction(start, target, spritesheet.directionCount);
            missile._animator.SetSprites(spritesheet.GetSprites(direction));
            missile._animator.loop = missileInfo.loopAnim != 0;
            missile._animator.fps = missileInfo.fps;
        
            AudioManager.instance.Play(missileInfo.travelSound, missile.transform);

            return missile;
        }

        void Update()
        {
            _lifeTime += Time.deltaTime;
            if (_lifeTime > _info.lifeTime)
            {
                if (_info.serverHitFunc == "29")
                {
                    Missile.CreateRadially(_info.clientHitSubMissileId[0], _iso.pos, _originator, 16);
                }
                Destroy(gameObject);
            }

            _speed += Mathf.Clamp(_info.accel * Time.deltaTime, 0, _info.maxVelocity);
            float distance = _speed * Time.deltaTime;
            var posDiff = _dir * distance;
            var newPos = _iso.pos + posDiff;
            var hit = CollisionMap.Raycast(_iso.pos, newPos, distance, size: _info.size, ignore: _originator.gameObject);
            if (hit)
            {
                Events.InvokeMissileHit(this, hit.pos, hit.gameObject);
            }

            Events.InvokeMissileMoved(this);

            _iso.pos = newPos;
        }
    }
}
