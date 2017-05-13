using UnityEngine;

public class Missile : MonoBehaviour
{
    new SpriteRenderer renderer;
    SpriteAnimator animator;
    Iso iso;
    Vector2 dir;
    float speed;
    MissileInfo info;

    static public Missile Create(string missileId, Vector3 start, Vector3 target)
    {
        var missileInfo = MissileInfo.Find(missileId);
        if (missileInfo == null)
        {
            Debug.LogWarning("missile not found: " + missileId);
            return null;
        }

        return Create(missileInfo, start, target);
    }

    static public Missile Create(MissileInfo missileInfo, Vector3 start, Vector3 target)
    {
        var gameObject = new GameObject("missile");
        var missile = gameObject.AddComponent<Missile>();
        missile.info = missileInfo;
        missile.animator = gameObject.AddComponent<SpriteAnimator>();
        missile.renderer = gameObject.GetComponent<SpriteRenderer>();
        missile.renderer.material = missileInfo.material;
        missile.iso = gameObject.AddComponent<Iso>();
        missile.iso.pos = start;
        missile.dir = (target - start).normalized;
        missile.speed = missileInfo.velocity;
        var spritesheet = Spritesheet.Load(missileInfo.spritesheetFilename);
        int direction = Iso.Direction(start, target, spritesheet.directionCount);
        missile.animator.sprites = spritesheet.GetSprites(direction);
        missile.animator.loop = missileInfo.loopAnim != 0;
        missile.animator.fps = missileInfo.animSpeed;
        Destroy(gameObject, 3);
        return missile;
    }

    void Update()
    {
        speed += info.accel * Time.deltaTime;
        iso.pos += dir * speed * Time.deltaTime;
    }
}
