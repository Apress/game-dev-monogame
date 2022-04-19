using Microsoft.Xna.Framework.Graphics;

namespace GameEditor
{
    public class GameObject
    {
        public Texture2D Texture { get; set; }
        public float Scale { get; set; }
        public int Width
        {
            get
            {
                return (int) (Texture.Width * Scale);
            }
        }

        public int Height
        {
            get
            {
                return (int) (Texture.Height * Scale);
            }
        }

        public GameObject(Texture2D texture, float scale = 1.0f)
        {
            Texture = texture;
            Scale = scale;
        }
    }
}
