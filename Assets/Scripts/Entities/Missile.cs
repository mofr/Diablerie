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
    private float lifeTime;

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

    static public void CreateRadially(string missileId, Vector3 start, Character originator, int missileCount)
    {
        var missileInfo = MissileInfo.Find(missileId);
        if (missileInfo == null)
        {
            Debug.LogWarning("missile not found: " + missileId);
        }

        CreateRadially(missileInfo, start, originator, missileCount);
    }

    static public void CreateRadially(MissileInfo missileInfo, Vector3 start, Character originator, int missileCount)
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

    static public Missile Create(MissileInfo missileInfo, Vector3 start, Vector3 target, Character originator)
    {
        var gameObject = new GameObject("missile_" + missileInfo.missile);
        var missile = gameObject.AddComponent<Missile>();
        missile.animator = gameObject.AddComponent<SpriteAnimator>();
        missile.renderer = gameObject.GetComponent<SpriteRenderer>();
        missile.iso = gameObject.AddComponent<Iso>();
        
        missile.info = missileInfo;
        missile.originator = originator;
        missile.speed = missileInfo.velocity;
        missile.iso.pos = start;
        missile.dir = (target - start).normalized;
        missile.renderer.material = missileInfo.material;
        missile.skillInfo = SkillInfo.Find(missileInfo.skill);
        missile.lifeTime = 0;

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
        lifeTime += Time.deltaTime;
        if (lifeTime > info.lifeTime)
        {
            if (info.serverHitFunc == "29")
            {
                Missile.CreateRadially(info.clientHitSubMissileId[0], iso.pos, originator, 16);
            }
            Destroy(gameObject);
        }

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
                        Overlay.Create(hitCharacter.gameObject, info.progOverlay, false);
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
            else if (info.serverHitFunc == "29")
            {
                // Frozen orb
                Missile.CreateRadially(info.clientHitSubMissileId[0], iso.pos, originator, 16);
            }
            
            // todo pierce is actually is pierce skill with a chance to pierce
            if ((!info.pierce || hitCharacter == null) && info.collideKill)
            {
                Destroy(gameObject);
            }
        }

        if (info.serverDoFunc == 15)
        {
            // Frozen orb
            int frequency = info.parameters[0].value * 25;
            float spawnPeriod = 1.0f / frequency;
            float directionIncrement = info.parameters[1].value * 25 * Mathf.PI;
            int missileToSpawn = (int)((lifeTime + Time.deltaTime) / spawnPeriod) - (int)(lifeTime / spawnPeriod);
            for (int i = 0; i < missileToSpawn; ++i)
            {
                var dir = new Vector2(1, 0);
                var rot = Quaternion.AngleAxis(lifeTime * directionIncrement, new Vector3(0, 0, 1));
                var offset = (Vector2) (rot * dir);
                Missile.Create(info.clientSubMissileId[0], iso.pos, iso.pos + offset, originator);
            }
        }

        iso.pos = newPos;
    }
}
