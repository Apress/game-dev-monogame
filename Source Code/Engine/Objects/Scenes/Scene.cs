using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Engine2D.Objects.Scenes
{
    public class Scene
    {
        private List<BaseLayer> _layers;

        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }

        public Scene()
        {
            _layers = new List<BaseLayer>();
            _layers.Add(new DefaultLayer());
        }

        public BaseLayer GetLayer(System.Type type)
        {
            foreach (var layer in _layers)
            {
                if (layer.GetType() == type)
                {
                    return layer;
                }
            }

            // if layer is not found, return an empty layer
            return new EmptyLayer();
        }

        public void AddGameObject(BaseGameObject obj)
        {
            var layer = GetLayer(typeof(DefaultLayer));
            AddGameObject(obj, layer);
        }

        public void AddGameObject(BaseGameObject obj, BaseLayer layer)
        {
            layer.GameObjects.Add(obj);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
            {
                return;
            }

            // draw all objects on all layers, ordered by depth

            var orderedLayers = _layers.OrderBy(l => l.Depth);
            foreach (var layer in orderedLayers)
            {
                foreach (var gameObject in layer.GameObjects)
                {
                    if (Debug.Instance.IsDebugMode)
                    {
                        gameObject.RenderBoundingBoxes(spriteBatch);
                    }

                    gameObject.Render(spriteBatch);
                }
            }
        }
    }
}
