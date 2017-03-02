using System.Collections.Generic;
using UnityEngine;

class Creature : Entity
{
    void OnRenderObject()
    {
        MouseSelection.Submit(this);
    }
}
