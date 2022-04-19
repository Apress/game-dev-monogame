using System;
using System.Collections.Generic;
using System.Linq;

namespace PipelineExtensions
{
    public class LevelEvent
    {
        public class Nothing : LevelEvent { }

        public class GenerateEnemies : LevelEvent
        { 
            public int NbEnemies { get; private set; }
            public GenerateEnemies(int nbEnemies)
            {
                NbEnemies = nbEnemies;
            }
        }

        public class GenerateTurret : LevelEvent
        {
            public float XPosition { get; set; }
            public GenerateTurret(float xPosition)
            {
                XPosition = xPosition;
            }
        }

        public class StartLevel : LevelEvent { }

        public class EndLevel : LevelEvent { }

        public class NoRowEvent : LevelEvent { }
    }

    public class Level
    {
        public const int NB_ROWS = 11;
        public const int NB_TILE_ROWS = 10;

        public string LevelStringEncoding { get; }
        public List<List<LevelEvent>> ProcessedLevel { get; }

        public Level(string encoding)
        {
            LevelStringEncoding = encoding;
            ProcessedLevel = LoadLevel();
        }

        private List<List<LevelEvent>> LoadLevel()
        {
            if (LevelStringEncoding != null && LevelStringEncoding.Length > 0)
            {
                var rows = LevelStringEncoding.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var convertedRows = from r in rows
                                    select ToEventRow(r);

                return convertedRows.Reverse().ToList();
            }
            else
            {
                return new List<List<LevelEvent>>();
            }
        }

        private List<LevelEvent> ToEventRow(string rowString)
        {
            var elements = rowString.Split(',');

            var newRow = new List<LevelEvent>();
            for (int i = 0; i < NB_ROWS; i++)
            {
                newRow.Add(ToEvent(elements[i]));
            }

            return newRow;
        }

        private LevelEvent ToEvent(string input)
        {
            switch (input) 
            {
                case "0":
                    return new LevelEvent.Nothing();
                    
                case "_":
                    return new LevelEvent.NoRowEvent();

                case "1":
                    // turret x position must be computed later, after level is loaded from content manager
                    return new LevelEvent.GenerateTurret(0.0f);

                case "s":
                    return new LevelEvent.StartLevel();

                case "e":
                    return new LevelEvent.EndLevel();

                case string g when g.StartsWith("g"):
                    var nb = int.Parse(g.Substring(1));
                    return new LevelEvent.GenerateEnemies(nb);

                default:
                    return new LevelEvent.Nothing();
            }
        }

        public void ComputePositions(int viewportWidth)
        {
            for (int row = 0; row < ProcessedLevel.Count; row++)
            {
                for (int col = 0; col < ProcessedLevel[row].Count; col++)
                {
                    var levelEvent = ProcessedLevel[row][col];
                    switch (levelEvent)
                    {
                        case LevelEvent.GenerateTurret t:
                            var xPosition = col * viewportWidth / NB_TILE_ROWS;
                            t.XPosition = xPosition;
                            break;
                    }
                }
            }
        }
    }
}
