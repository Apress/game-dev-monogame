using System.Collections.Generic;

namespace Engine2D.PipelineExtensions
{
    public class AnimationData
    {
        public int AnimationSpeed;
        public bool IsLooping;
        public List<AnimationFrameData> Frames;
    }

    public class AnimationFrameData
    {
        public int X;
        public int Y;
        public int CellWidth;
        public int CellHeight;
    }
}
