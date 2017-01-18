using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usable : MonoBehaviour
{
    public bool active = true;

    public void Use() {
		SendMessage("OnUse");
    }
}
