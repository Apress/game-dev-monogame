using Engine2D.Objects;
using Game.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Objects.Text
{
    public class LivesText : BaseTextObject
    {
        private int _nbLives = -1;

        public int NbLives {
            get
            {
                return _nbLives;
            }
            set
            {
                _nbLives = value;
                Text = $"{Strings.Lives}: {_nbLives}";
            }
        }

        public LivesText(SpriteFont font) : base(font)
        {
            _font = font;
        }
    }
}
