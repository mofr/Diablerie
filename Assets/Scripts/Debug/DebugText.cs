using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugText : MonoBehaviour
{

    public string text = null;
	
	void OnGUI ()
    {
        GUI.color = Color.white;
        var center = Camera.main.WorldToScreenPoint(Camera.main.transform.position);
        var pos = Camera.main.WorldToScreenPoint(transform.position);
        pos.z = 0;
        pos.y = center.y * 2 - pos.y;
        var renderText = (text == null) || text == "" ? gameObject.name : text;
        //var renderText = transform.position.x + " " + transform.position.y + "\n" + pos.x + " " + pos.y;
        GUI.Label(new Rect(pos, new Vector2(200, 200)), renderText);
	}
}
