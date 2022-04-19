using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace PipelineExtensions
{
    [ContentImporter(".txt", DisplayName = "LevelImporter", DefaultProcessor = "LevelProcessor")]
    public class LevelImporter : ContentImporter<string>
    {
        public override string Import(string filename, ContentImporterContext context)
        {
            return File.ReadAllText(filename);
        }
    }
}
