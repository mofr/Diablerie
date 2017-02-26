using System.Linq;
using System.Collections.Generic;
using UnityEngine;

class COFAnimator : MonoBehaviour
{
    public COF cof;
    public ObjectInfo objectInfo;
    public int modeIndex = 0;
    public int direction = 0;

    void Start()
    {
        for (int i = 0; i < cof.layerCount; ++i)
        {
            int frameIndex = 0;
            int layerIndex = cof.priority[(direction * cof.framesPerDirection * cof.layerCount) + (frameIndex * cof.layerCount) + i];
            var layer = cof.layers[layerIndex];
            var dcc = DCC.Load(layer.dccFilename);

            IsoAnimation anim = ScriptableObject.CreateInstance<IsoAnimation>();
            anim.directionCount = cof.directionCount;
            anim.states = new IsoAnimation.State[1];
            IsoAnimation.State animState = new IsoAnimation.State();
            anim.states[0] = animState;
            animState.name = layer.name;
            if (objectInfo != null)
            {
                var sprites = dcc.sprites.Skip(objectInfo.start[modeIndex]);
                int frameCount = objectInfo.frameCount[modeIndex];
                if (frameCount != 0)
                    sprites = sprites.Take(frameCount);
                animState.sprites = sprites.ToArray();
                animState.loop = objectInfo.cycleAnim[modeIndex];
            }
            else
                animState.sprites = dcc.sprites.ToArray();

            GameObject layerObject = new GameObject();
            var spriteRenderer = layerObject.AddComponent<SpriteRenderer>();
            var animator = layerObject.AddComponent<IsoAnimator>();
            animator.direction = direction;
            animator.anim = anim;
            layerObject.name = layer.name;
            layerObject.transform.position = new Vector3(0, 0, -i * 0.1f);
            layerObject.transform.SetParent(gameObject.transform, false);
            spriteRenderer.sortingOrder = Iso.SortingOrder(gameObject.transform.position);
        }
    }
}
