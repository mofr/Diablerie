using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level current = null;
    public LevelInfo info;

    public delegate void LevelChangeHandler(Level level, Level previous);
    public static event LevelChangeHandler OnLevelChange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        if (current != null)
            LevelEntryTitle.Show("Entering " + info.levelName);

        var previous = current;
        current = this;
        if (OnLevelChange != null)
            OnLevelChange(current, previous);
    }
}
