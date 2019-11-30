using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;
using Diablerie.Engine.World;
using UnityEngine;

namespace Diablerie.Game.World
{
    public class Act1 : Act
    {
        public Act1()
        {
            var town = new LevelBuilder("Act 1 - Town");
            var bloodMoor = CreateBloodMoor();

            var townOffset = new Vector2i(bloodMoor.gridWidth * bloodMoor.gridX - town.gridWidth * town.gridX, bloodMoor.gridHeight * bloodMoor.gridY);
            var townGameObject = town.Instantiate(townOffset);
            var bloodMoorGameObject = bloodMoor.Instantiate(new Vector2i(0, 0));

            var doe = CreateDenOfEvil();
            var doeOffset = new Vector2i(120, 0);
            var doeGameObject = doe.Instantiate(doeOffset);

            root = new GameObject("Act 1");
            townGameObject.transform.SetParent(root.transform);
            bloodMoorGameObject.transform.SetParent(root.transform);
            doeGameObject.transform.SetParent(root.transform);

            entry = town.FindEntry() + townOffset;
        }

        static LevelBuilder CreateDenOfEvil()
        {
            var builder = new LevelBuilder("Act 1 - Cave 1");
            var palette = new Maze.Palette();
            palette.special = new LevelPreset[][] {
                new LevelPreset[] {
                    LevelPreset.Find("Act 1 - Cave Prev W"),
                    LevelPreset.Find("Act 1 - Cave Prev E"),
                    LevelPreset.Find("Act 1 - Cave Prev S"),
                    LevelPreset.Find("Act 1 - Cave Prev N")
                },
                new LevelPreset[] {
                    LevelPreset.Find("Act 1 - Cave Den Of Evil W"),
                    LevelPreset.Find("Act 1 - Cave Den Of Evil E"),
                    LevelPreset.Find("Act 1 - Cave Den Of Evil S"),
                    LevelPreset.Find("Act 1 - Cave Den Of Evil N")
                }
            };
            palette.rooms = new LevelPreset[16];
            for (int i = 0; i < 15; ++i)
                palette.rooms[i + 1] = LevelPreset.sheet[53 + i];
            palette.themedRooms = new LevelPreset[16];
            for (int i = 0; i < 15; ++i)
                palette.themedRooms[i + 1] = LevelPreset.sheet[68 + i];
            Maze.Generate(builder, palette);
            return builder;
        }

        static LevelBuilder CreateBloodMoor()
        {
            var bloodMoor = new LevelBuilder("Act 1 - Wilderness 1", 8, 8);
            var riverN = DS1.Load(@"data\global\tiles\act1\outdoors\UriverN.ds1");
            var uRiver = DS1.Load(@"data\global\tiles\act1\outdoors\Uriver.ds1");
            var lRiver = DS1.Load(@"data\global\tiles\act1\outdoors\Lriver.ds1");
            var bord1 = LevelPreset.Find("Act 1 - Wild Border 1");
            var bord2 = LevelPreset.Find("Act 1 - Wild Border 2");
            var bord3 = LevelPreset.Find("Act 1 - Wild Border 3");
            var bord5 = LevelPreset.Find("Act 1 - Wild Border 5");
            var bord6 = LevelPreset.Find("Act 1 - Wild Border 6");
            var bord9 = LevelPreset.Find("Act 1 - Wild Border 9");
            var cottage = LevelPreset.Find("Act 1 - Cottages 1");
            var denEntrance = LevelPreset.Find("Act 1 - DOE Entrance");

            for (int i = 0; i < bloodMoor.gridHeight; ++i)
                bloodMoor.Place(lRiver, new Vector2i(bloodMoor.gridWidth - 1, i));
            for (int i = 1; i < bloodMoor.gridHeight; ++i)
                bloodMoor.Place(uRiver, new Vector2i(bloodMoor.gridWidth - 2, i));
            bloodMoor.Place(riverN, new Vector2i(bloodMoor.gridWidth - 2, 0));

            for (int i = 1; i < bloodMoor.gridHeight - 1; ++i)
                bloodMoor.Place(bord2, new Vector2i(0, i), 0, 3);
            bloodMoor.Place(bord5, new Vector2i(0, bloodMoor.gridHeight - 1));

            for (int i = 1; i < 3; ++i)
                bloodMoor.Place(bord1, new Vector2i(i, bloodMoor.gridHeight - 1), 0, 3);
            bloodMoor.Place(bord9, new Vector2i(3, bloodMoor.gridHeight - 1));

            for (int i = 1; i < bloodMoor.gridWidth - 2; ++i)
                bloodMoor.Place(bord3, new Vector2i(i, 0), 0, 3);

            bloodMoor.Place(bord6, new Vector2i(0, 0));
            for (int i = 1; i < 5; ++i)
                bloodMoor.Place(cottage, new Vector2i(i, 4 + Random.Range(-1, 1)));
            bloodMoor.Place(denEntrance, new Vector2i(5, 7));

            return bloodMoor;
        }
    }
}
