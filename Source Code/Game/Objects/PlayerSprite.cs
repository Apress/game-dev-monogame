using Engine2D.Objects;
using Engine2D.Objects.Animations;
using Engine2D.PipelineExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Objects
{
    public class PlayerSprite : BaseGameObject
    {
        // Camera speed is -10
        private Vector2 _playerNormalUpSpeed = new Vector2(0, -8.0f);
        private Vector2 _playerBoostUpSpeed = new Vector2(0, -15.0f);
        private Vector2 _playerBoostDownSpeed = new Vector2(0, -6.0f);
        private const float PlayerHorizontalSpeed = 10.0f;

        private const int BB1PosX = 29;
        private const int BB1PosY = 2;
        private const int BB1Width = 57;
        private const int BB1Height = 147;

        private const int BB2PosX = 2;
        private const int BB2PosY = 77;
        private const int BB2Width = 111;
        private const int BB2Height = 37;

        private Animation _turnLeftAnimation;
        private Animation _turnRightAnimation;
        private Animation _leftToCenterAnimation;
        private Animation _rightToCenterAnimation;
        private const int AnimationCellWidth = 116;
        private const int AnimationCellHeight= 152;

        private Animation _currentAnimation;
        private Rectangle _idleRectangle;

        private bool _movingLeft = false;
        private bool _movingRight = false;

        public override int Height => AnimationCellHeight;
        public override int Width => AnimationCellWidth;

        public Vector2 CenterPosition
        {
            get
            {
                return Vector2.Add(_position, new Vector2(AnimationCellWidth / 2, AnimationCellHeight / 2));
            }
        }

        public Vector2 CurrentUpSpeed { get; private set; }

        public PlayerSprite(Texture2D texture, AnimationData turnLeftAnimation, AnimationData turnRightAnimation) : base(texture)
        {
            AddBoundingBox(new Engine2D.Objects.BoundingBox(new Vector2(BB1PosX, BB1PosY), BB1Width, BB1Height));
            AddBoundingBox(new Engine2D.Objects.BoundingBox(new Vector2(BB2PosX, BB2PosY), BB2Width, BB2Height));

            _idleRectangle = new Rectangle(348, 0, AnimationCellWidth, AnimationCellHeight);

            _turnLeftAnimation = new Animation(turnLeftAnimation);
            _turnRightAnimation = new Animation(turnRightAnimation);
            _leftToCenterAnimation = _turnLeftAnimation.ReverseAnimation;
            _rightToCenterAnimation = _turnRightAnimation.ReverseAnimation;

            CurrentUpSpeed = _playerNormalUpSpeed;
        }

        public void StopMoving()
        {
            if (_movingLeft)
            {
                _currentAnimation = _leftToCenterAnimation;
                _movingLeft = false;
            }

            if (_movingRight)
            {
                _currentAnimation = _rightToCenterAnimation;
                _movingRight = false;
            }
        }

        public void StopVerticalMoving()
        {
            CurrentUpSpeed = _playerNormalUpSpeed;
        }

        public void MoveLeft()
        {
            _movingLeft = true;
            _movingRight = false;
            _currentAnimation = _turnLeftAnimation;
            _leftToCenterAnimation.Reset();
            _turnRightAnimation.Reset();
            Position = new Vector2(Position.X - PlayerHorizontalSpeed, Position.Y);
        }

        public void MoveRight()
        {
            _movingRight = true;
            _movingLeft = false;
            _currentAnimation = _turnRightAnimation;
            _rightToCenterAnimation.Reset();
            _turnLeftAnimation.Reset();
            Position = new Vector2(Position.X + PlayerHorizontalSpeed, Position.Y);
        }

        public void MoveUp()
        {
            CurrentUpSpeed = _playerBoostUpSpeed;
        }

        public void MoveDown()
        {
            CurrentUpSpeed = _playerBoostDownSpeed;
        }

        public void Update(GameTime gametime)
        {
            Position += CurrentUpSpeed;

            if (_currentAnimation != null)
            {
                _currentAnimation.Update(gametime);
            }
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            var destinationRectangle = new Rectangle((int)_position.X, (int)_position.Y, AnimationCellWidth, AnimationCellHeight);
            var sourceRectangle = _idleRectangle;
            if (_currentAnimation != null)
            {
                var currentFrame = _currentAnimation.CurrentFrame;
                if (currentFrame != null)
                {
                    sourceRectangle = currentFrame.SourceRectangle;
                }
            }

            spriteBatch.Draw(_texture, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}