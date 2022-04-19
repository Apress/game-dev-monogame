using System.Collections.Generic;

namespace Engine2D.Objects.Scenes
{
    public class EmptyLayer : BaseLayer
    {
        public EmptyLayer() : base(0.0f)
        {
        }

        // force game object list to always be empty
        public override List<BaseGameObject> GameObjects => new List<BaseGameObject>();
    }
}
