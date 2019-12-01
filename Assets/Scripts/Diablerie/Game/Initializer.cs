using Diablerie.Engine.IO.D2Formats;
using Diablerie.Game.UI;
using UnityEngine;

namespace Diablerie.Game
{
    public class Initializer : MonoBehaviour
    {
        public MainMenu mainMenuPrefab;
        
        public void Awake()
        {
            try
            {
                Mpq.AddArchive("d2exp.mpq");
                Mpq.AddArchive("d2data.mpq");
                Mpq.AddArchive("d2char.mpq");
                Mpq.AddArchive("d2sfx.mpq", optional: true);
                Mpq.AddArchive("d2music.mpq", optional: true);
                Mpq.AddArchive("d2xMusic.mpq", optional: true);
                Mpq.AddArchive("d2xtalk.mpq", optional: true);
                Mpq.AddArchive("d2speech.mpq", optional: true);
            }
            catch (System.IO.FileNotFoundException e)
            {
                // TODO Show the error to the user
                Debug.Log(e);
                return;
            }
            
            Instantiate(mainMenuPrefab);
        }
    }
}
