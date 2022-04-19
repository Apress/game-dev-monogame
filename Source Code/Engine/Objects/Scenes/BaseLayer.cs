using System.Collections.Generic;

namespace Engine2D.Objects.Scenes
{
    public abstract class BaseLayer
    {
        private List<BaseLayer> _interactsWith;

        /// <summary>
        /// Depth is between 0.0f and 1.0f. Can be used to order layers for drawing order and
        /// for parallax in conjunction with the MonoGame.extended orthographic camera
        /// </summary>
        public float Depth { get; private set; }

        /// <summary>
        /// List of all the game objects on this layer
        /// </summary>
        public virtual List<BaseGameObject> GameObjects { get; private set; }

        /// <summary>
        /// List of layers this layer can interact with. This will be used for collision detection. 
        /// Layers that don't interact with each other have objects that can't collide with each other. 
        /// Each layer has at least itself as part of this list
        /// </summary>
        public List<BaseLayer> InteractsWith 
        {
            get
            {
                if (_interactsWith.Count == 0)
                {
                    _interactsWith.Add(this);
                }

                return _interactsWith;
            }
        }

        public BaseLayer(float depth)
        {
            Depth = depth;
            GameObjects = new List<BaseGameObject>();
            _interactsWith = new List<BaseLayer>();
        }
    }
}
