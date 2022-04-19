using System;

namespace Game.States.Gameplay
{
    public class ChopperGenerator
    {
        private bool _generateLeft = true;

        private System.Timers.Timer _timer;
        private Action<bool> _chopperHandler;
        private int _maxChoppers = 0;
        private int _choppersGenerated = 0;
        private bool _generating = false;

        public ChopperGenerator(Action<bool> handler)
        {
            _chopperHandler = handler;

            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += _timer_Elapsed;
        }

        public void GenerateChoppers(int nbChoppers)
        {
            if (_generating)
            {
                return;
            }

            _maxChoppers = nbChoppers;
            _choppersGenerated = 0;
            _timer.Start();
        }

        public void StopGenerating()
        {
            _timer.Stop();
            _generating = false;
            _generateLeft = true;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _chopperHandler(_generateLeft);

            _generateLeft = !_generateLeft;

            _choppersGenerated++;
            if (_choppersGenerated == _maxChoppers)
            {
                StopGenerating();
            }
        }
    }
}
