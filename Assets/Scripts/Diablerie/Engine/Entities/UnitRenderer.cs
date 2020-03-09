using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    [RequireComponent(typeof(COFRenderer))]
    public class UnitRenderer : MonoBehaviour
    {
        private Unit _unit;
        private COFRenderer _renderer;
        
        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _renderer = GetComponent<COFRenderer>();
        }

        private void Update()
        {
            var mode = _unit.Mode;
            string weaponClass = _unit.weaponClass;
            if (mode == "DT" || mode == "DD")
            {
                weaponClass = "HTH";
            }

            _renderer.cof = COF.Load(_unit.basePath, _unit.token, weaponClass, mode);
            _renderer.direction = _unit.DirectionIndex;
            _renderer.frame = _unit.AnimationFrame;
        }
    }
}