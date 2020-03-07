using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    [RequireComponent(typeof(COFRenderer))]
    public class StaticObjectRenderer : MonoBehaviour
    {
        private static readonly string[] Equip = { "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT" };
        
        private StaticObject _staticObject;
        private COFRenderer _renderer;

        private void Awake()
        {
            _staticObject = GetComponent<StaticObject>();
            _renderer = GetComponent<COFRenderer>();
            _renderer.equip = Equip;
        }

        private void Update()
        {
            var info = _staticObject.info;
            var mode = _staticObject.Mode;

            if (!info.mode[mode.index])
            {
                _renderer.cof = null;
                return;
            }

            if (_renderer.cof == null || _renderer.cof.mode != mode.token)
            {
                var cof = COF.Load(@"data\global\objects", info.token, "HTH", mode.token);
                _renderer.shadow = info.blocksLight[mode.index];
                _renderer.cof = cof;
            }

            int frame = (int)(_staticObject.AnimationTime * info.frameCount[mode.index] / _staticObject.AnimationDuration);
            frame = Mathf.Min(frame, info.frameCount[mode.index]);
            frame = Mathf.Max(frame, 0);
            frame += info.start[mode.index];
            _renderer.SetFrame(frame);
        }
    }
}
