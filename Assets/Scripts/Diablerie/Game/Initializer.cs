using System;
using System.IO;
using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Game.UI;
using Diablerie.Game.UI.Menu;
using UnityEngine;

namespace Diablerie.Game
{
    public class Initializer : MonoBehaviour
    {
        public MainMenu mainMenuPrefab;
        private DataLoader.LoadProgress loadProgress;
        private static DataLoader.Paths paths = new DataLoader.Paths
        {
            mpq = new []
            {
                new DataLoader.MpqLocation{filename="d2exp.mpq", optional=false}, 
                new DataLoader.MpqLocation{filename="d2data.mpq", optional=false}, 
                new DataLoader.MpqLocation{filename="d2char.mpq", optional=false}, 
                new DataLoader.MpqLocation{filename="d2sfx.mpq", optional=true}, 
                new DataLoader.MpqLocation{filename="d2music.mpq", optional=true}, 
                new DataLoader.MpqLocation{filename="d2xMusic.mpq", optional=true}, 
                new DataLoader.MpqLocation{filename="d2xtalk.mpq", optional=true}, 
                new DataLoader.MpqLocation{filename="d2speech.mpq", optional=true}, 
            },
            animData=@"data\global\animdata.d2",
        }; 
        
        void Awake()
        {
            Datasheet.SetLocation(typeof(BodyLoc), "data/global/excel/bodylocs.txt");
            Datasheet.SetLocation(typeof(SoundInfo), "data/global/excel/Sounds.txt");
            var dataLoader = new DataLoader(paths);
            loadProgress = dataLoader.LoadAll();
        }

        void Update()
        {
            if (loadProgress.finished)
            {
                if (loadProgress.exception != null)
                {
                    string message = BuildExceptionMessage(loadProgress.exception);
                    ScreenMessage.Show(message);
                }
                else
                {
                    ScreenMessage.Hide();
                    Instantiate(mainMenuPrefab);
                }
                Destroy(this);
            }
            else
            {
                ScreenMessage.Show("Loading... ");
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameManager.QuitGame();
            }
        }

        private string BuildExceptionMessage(Exception exception)
        {
            if (exception is FileNotFoundException)
            {
                return BuildMessage(exception.Message);
            }
            else
            {
                return exception.Message;
            }
        }

        private string BuildMessage(string missingFile)
        {
            string message = "File not found: " + missingFile;
            message += "\n\nBlizzard Diablo II resources are required";
            message += "\n\nCopy MPQ files to the Diablerie folder";
            return message;
        }
    }
}