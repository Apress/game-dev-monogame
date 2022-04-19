using Microsoft.Xna.Framework.Content;
using PipelineExtensions;
using System.Collections.Generic;

namespace GameEditor
{
    public class Level
    {
        public const int LEVEL_LENGTH = 100;
        public const int LEVEL_WIDTH = 10;

        public string[,] GroundGrid { get; set; }
        public List<GameEditorTileData> Buildings { get; set; }
        public List<GameEditorTileData> Objects { get; set; }
        public List<GameEditorEvent> LevelEvents { get; set; }


        public Level()
        {
            GroundGrid = new string[LEVEL_WIDTH, LEVEL_LENGTH];
            Buildings = new List<GameEditorTileData>();
            Objects = new List<GameEditorTileData>();
            LevelEvents = new List<GameEditorEvent>();
        }

        public void Save(int levelNb)
        {
            var levelData = new GameEditorLevelData(LEVEL_WIDTH, LEVEL_LENGTH, GroundGrid, Buildings, Objects, LevelEvents);
            levelData.Save(levelNb);
        }

        public void Load(ContentManager content, int currentLevel)
        {
            var levelData = content.Load<GameEditorLevelData>($"levels/Level{currentLevel}");
            GroundGrid = levelData.StringToArray(levelData.GroundGrid);
            Buildings = levelData.Buildings;
            Objects = levelData.Objects;
            LevelEvents = levelData.LevelEvents;
        }
    }
}
