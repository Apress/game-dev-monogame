using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine2D.Objects
{
    public class StatsObject : BaseTextObject
    {
        public const int ROLLING_SIZE = 60;

        private Queue<float> _rollingFPS = new Queue<float>();

        public float FPS { get; set; }
        public float MinFPS { get; private set; }
        public float MaxFPS { get; private set; }
        public float AverageFPS { get; private set; }
        public bool IsRunningSlowly { get; set; }
        public int NbUpdateCalled { get; set; }
        public int NbDrawCalled { get; set; }

        public StatsObject(SpriteFont font) : base(font) 
        {
            NbUpdateCalled = 0;
            NbDrawCalled = 0;
        }

        public void Update(GameTime gameTime)
        {
            NbUpdateCalled++;
            FPS = 1.0f / (float) gameTime.ElapsedGameTime.TotalSeconds;

            _rollingFPS.Enqueue(FPS);

            if (_rollingFPS.Count > ROLLING_SIZE)
            {
                _rollingFPS.Dequeue();

                var sum = 0.0f;
                MaxFPS = int.MinValue;
                MinFPS = int.MaxValue;
                foreach (var fps in _rollingFPS.ToArray())
                {
                    sum += fps;
                    if (fps > MaxFPS)
                    {
                        MaxFPS = fps;
                    }

                    if (fps < MinFPS)
                    {
                        MinFPS = fps;
                    }
                }
                AverageFPS = sum / _rollingFPS.Count;
            }
            else
            {
                AverageFPS = FPS;
                MinFPS = FPS;
                MaxFPS = FPS;
            }

            Text = $"FPS: {FPS}" + System.Environment.NewLine;
            Text += $"Min FPS: {MinFPS}" + System.Environment.NewLine;
            Text += $"Max FPS: {MaxFPS}" + System.Environment.NewLine;
            Text += $"Avg FPS: {AverageFPS}" + System.Environment.NewLine;
            Text += $"Running Slowly: {IsRunningSlowly}" + System.Environment.NewLine;
            Text += $"Nb Updates: {NbUpdateCalled}" + System.Environment.NewLine;
            Text += $"Nb Draws: {NbDrawCalled}";
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            NbDrawCalled++;
            base.Render(spriteBatch);
        }
    }
}
