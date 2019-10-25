using UnityEngine;

public class Missile : MonoBehaviour
{
    public int weaponDamage = 0;

    new SpriteRenderer renderer;
    SpriteAnimator animator;
    Iso iso;
    Vector2 dir;
    float speed;
    MissileInfo info;
    SkillInfo skillInfo;
    Character originator;

    static public Missile Create(string missileId, Vector3 target, Character originator)
    {
        return Create(missileId, originator.iso.pos, target, originator);
    }

    static public Missile Create(string missileId, Vector3 start, Vector3 target, Character originator)
    {
        var missileInfo = MissileInfo.Find(missileId);
        if (missileInfo == null)
        {
            Debug.LogWarning("missile not found: " + missileId);
            return null;
        }

        return Create(missileInfo, start, target, originator);
    }

    static public Missile Create(MissileInfo missileInfo, Vector3 start, Vector3 target, Character originator)
    {
        var gameObject = new GameObject("missile_" + missileInfo.missile);
        var missile = gameObject.AddComponent<Missile>();
        missile.animator = gameObject.AddComponent<SpriteAnimator>();
        missile.renderer = gameObject.GetComponent<SpriteRenderer>();
        missile.iso = gameObject.AddComponent<Iso>();
        Destroy(gameObject, missileInfo.lifeTime);
        
        missile.info = missileInfo;
        missile.originator = originator;
        missile.speed = missileInfo.velocity;
        missile.iso.pos = start;
        missile.dir = (target - start).normalized;
        missile.renderer.material = missileInfo.material;
        missile.skillInfo = SkillInfo.Find(missileInfo.skill);

        var spritesheet = Spritesheet.Load(missileInfo.spritesheetFilename);
        int direction = Iso.Direction(start, target, spritesheet.directionCount);
        missile.animator.sprites = spritesheet.GetSprites(direction);
        missile.animator.loop = missileInfo.loopAnim != 0;
        missile.animator.fps = missileInfo.fps;
        
        AudioManager.instance.Play(missileInfo.travelSound, missile.transform);

        return missile;
    }

    int CalcDamage()
    {
        int damage = weaponDamage;

        // todo take skill level into account
        if (skillInfo != null)
        {
            damage += Random.Range(skillInfo.eMin, skillInfo.eMax + 1) + Random.Range(skillInfo.minDamage, skillInfo.maxDamage + 1);
        }
        else
        {
            damage += Random.Range(info.eMin, info.eMax + 1) + Random.Range(info.minDamage, info.maxDamage + 1);
        }

        return damage;
    }

    void Update()
    {
        speed += Mathf.Clamp(info.accel * Time.deltaTime, 0, info.maxVelocity);
        float distance = speed * Time.deltaTime;
        var posDiff = dir * distance;
        var newPos = iso.pos + posDiff;
        var hit = CollisionMap.Raycast(iso.pos, newPos, distance, size: info.size, ignore: originator.gameObject);
        if (hit)
        {
            Character hitCharacter = null;
            if (hit.gameObject != null)
            {
                hitCharacter = hit.gameObject.GetComponent<Character>();
                if (hitCharacter != null)
                {
                    int damage = CalcDamage();
                    hitCharacter.TakeDamage(damage, originator);
                    if (info.progOverlay != null)
                        Overlay.Create(hitCharacter.gameObject, info.progOverlay);
                }
            }
            if (info.explosionMissile != null)
                Missile.Create(info.explosionMissile, hit.pos, hit.pos, originator);

            AudioManager.instance.Play(info.hitSound, Iso.MapToWorld(hit.pos));
            AudioManager.instance.Play(SoundInfo.GetHitSound(info.hitClass, hitCharacter), Iso.MapToWorld(hit.pos));

            if (info.clientHitFunc == "14")
            {
                // glacial spike, freezing arrow
                Missile.Create(info.clientHitSubMissileId[0], hit.pos, hit.pos, originator);
                int subMissileCount = Random.Range(3, 5);
                for (int i = 0; i < subMissileCount; ++i)
                {
                    var offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    Missile.Create(info.clientHitSubMissileId[1], hit.pos, hit.pos - offset, originator);
                }
            }
            
            // todo pierce is actually is pierce skill with a chance to pierce
            if ((!info.pierce || hitCharacter == null) && info.collideKill)
            {
                Destroy(gameObject);
            }
        }

        iso.pos = newPos;
    }
}
