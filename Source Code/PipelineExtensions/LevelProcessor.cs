using Microsoft.Xna.Framework.Content.Pipeline;

namespace PipelineExtensions
{
    [ContentProcessor(DisplayName = "LevelProcessor")]
    public class LevelProcessor : ContentProcessor<string, Level>
    {
        public override Level Process(string input, ContentProcessorContext context)
        {
            return new Level(input);
        }
    }
}
