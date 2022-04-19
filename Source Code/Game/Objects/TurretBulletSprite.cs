using Engine2D.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Objects
{
    public class TurretBulletSprite : BaseGameObject
    {
        private const float BULLET_SPEED = 18.0f;
        private Vector2 _bulletCenterPosition;

        public Segment CollisionSegment
        { 
            get
            {
                var segment = Direction * _texture.Height;
                return new Segment(_position, Vector2.Add(_position, segment));
            } 
        }

        public TurretBulletSprite(Texture2D texture) : base(texture)
        {
            _bulletCenterPosition = new Vector2(_texture.Width / 2, _texture.Height / 2);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Update()
        {
            Position = Position + Direction * BULLET_SPEED;
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, _texture.Bounds, Color.White, Angle, _bulletCenterPosition, 1f, SpriteEffects.None, 0f);
        }
    }
}
