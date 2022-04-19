using PipelineExtensions;
using System;

namespace GameEditor
{
    public class EventSelectedArgs : EventArgs
    {
        public GameEditorEvent GameEditorEvent { get; private set; }
        public EventSelectedArgs(object sender, GameEditorEvent e)
        {
            GameEditorEvent = e;
        }
    }
}
