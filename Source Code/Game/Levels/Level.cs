using Microsoft.Xna.Framework;
using System;
using PipelineExtensions;

namespace Game.Levels
{
    public class Level
    {
        private LevelReader _levelReader;
        private PipelineExtensions.Level _currentLevel;
        private int _currentLevelNumber;
        private int _currentLevelRow;

        private TimeSpan _startGameTime;
        private readonly TimeSpan TickTimeSpan = new TimeSpan(0, 0, 2);

        public event EventHandler<LevelEvent.GenerateEnemies> OnGenerateEnemies;
        public event EventHandler<LevelEvent.GenerateTurret> OnGenerateTurret;
        public event EventHandler<LevelEvent.StartLevel> OnLevelStart;
        public event EventHandler<LevelEvent.EndLevel> OnLevelEnd;
        public event EventHandler<LevelEvent.NoRowEvent> OnLevelNoRowEvent;

        public Level(LevelReader reader)
        {
            _levelReader = reader;
            _currentLevelNumber = 1;
            _currentLevelRow = 0;

            _currentLevel = _levelReader.LoadLevel(_currentLevelNumber);
        }

        public void LoadNextLevel()
        {
            _currentLevelNumber++;
            _currentLevel = _levelReader.LoadLevel(_currentLevelNumber);
        }

        public void Reset()
        {
            _currentLevelRow = 0;
        }

        public void GenerateLevelEvents(GameTime gameTime)
        {
            // only generate events every 2 seconds
            if (_startGameTime == null)
            {
                _startGameTime = gameTime.TotalGameTime;
            }

            // nothing to do until tick time
            if (gameTime.TotalGameTime - _startGameTime < TickTimeSpan)
            {
                return;
            }

            _startGameTime = gameTime.TotalGameTime;

            foreach (var e in _currentLevel.ProcessedLevel[_currentLevelRow])
            {
                switch (e) 
                {
                    case LevelEvent.GenerateEnemies g:
                        OnGenerateEnemies?.Invoke(this, g);
                        break;

                    case LevelEvent.GenerateTurret g:
                        OnGenerateTurret?.Invoke(this, g);
                        break;

                    case LevelEvent.StartLevel s:
                        OnLevelStart?.Invoke(this, s);
                        break;

                    case LevelEvent.EndLevel s:
                        OnLevelEnd?.Invoke(this, s);
                        break;

                    case LevelEvent.NoRowEvent n:
                        OnLevelNoRowEvent?.Invoke(this, n);
                        break;
                }
            }

            _currentLevelRow++;
        }
    }
}
