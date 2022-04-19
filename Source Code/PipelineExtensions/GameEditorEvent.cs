using System;

namespace PipelineExtensions
{
    public class GameEditorEvent
    {
        public int Y { get; set; }

        public static GameEditorEvent GetEvent(string typeName)
        {
            var fullyQualifiedName = $"PipelineExtensions.{typeName}";
            var eventType = Type.GetType(fullyQualifiedName);

            if (eventType != null)
            {
                return (GameEditorEvent) Activator.CreateInstance(eventType);
            }

            return null;
        }

    }

    public class GameEditorGenerate2Choppers : GameEditorEvent
    {
        public GameEditorGenerate2Choppers() { }
    }

    public class GameEditorGenerate4Choppers : GameEditorEvent
    {
        public GameEditorGenerate4Choppers() { }
    }

    public class GameEditorGenerate6Choppers : GameEditorEvent
    {
        public GameEditorGenerate6Choppers() { }
    }

    public class GameEditorStartLevel : GameEditorEvent
    {
        public GameEditorStartLevel() { }
    }

    public class GameEditorEndLevel : GameEditorEvent
    {
        public GameEditorEndLevel() { }
    }
}
