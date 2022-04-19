using Engine2D.States;
using Microsoft.Xna.Framework.Content;

namespace Game.Levels
{
    public class LevelReader
    {
        private int _viewportWidth;
        private ContentManager _content;

        public LevelReader(ContentManager content, int viewportWidth)
        {
            _viewportWidth = viewportWidth;
            _content = content;
        }

        public PipelineExtensions.Level LoadLevel(int nb)
        {
            var levelAssetName = $"Levels/Level{nb}";
            var level = _content.Load<PipelineExtensions.Level>(levelAssetName);
            level.ComputePositions(_viewportWidth);

            return level;
        }
    }
}
