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
            var mode = _staticObject.ModeIndex;
            var modeName = COF.ModeNames[2][mode];

            if (!info.mode[mode])
            {
                _renderer.cof = null;
                return;
            }

            if (_renderer.cof == null || _renderer.cof.mode != modeName)
            {
                var cof = COF.Load(@"data\global\objects", info.token, "HTH", modeName);
                _renderer.shadow = info.blocksLight[mode];
                _renderer.cof = cof;
            }

            int frame = (int)(_staticObject.AnimationTime * info.frameCount[mode] / _staticObject.AnimationDuration);
            frame = Mathf.Min(frame, info.frameCount[mode]);
            frame += info.start[mode];
            _renderer.SetFrame(frame);
        }
    }
}
