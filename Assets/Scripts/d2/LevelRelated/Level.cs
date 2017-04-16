using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelInfo info;
    
    static Level current = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        if (current != null)
            LevelEntryTitle.Show("Entering " + info.levelName);

        current = this;
    }
}

