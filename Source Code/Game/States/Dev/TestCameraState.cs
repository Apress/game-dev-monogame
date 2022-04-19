using Engine2D;
using Engine2D.Input;
using Engine2D.States;
using Game.Input;
using Game.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace Game.States
{
    /// <summary>
    /// Used to test out new things, like particle engines and shooting missiles
    /// </summary>
    public class TestCameraState : BaseGameState
    {
        private const string PlayerAnimationTurnLeft = "Sprites/Animations/FighterSpriteTurnLeft";
        private const string PlayerAnimationTurnRight = "Sprites/Animations/FighterSpriteTurnRight";
        private const string PlayerFighter = "Sprites/Animations/FighterSpriteSheet";
        private PlayerSprite _playerSprite;

        private OrthographicCamera _camera;

        private const float CAMERA_SPEED = 10.0f;

        public override void LoadContent()
        {
            var viewportAdapter = new DefaultViewportAdapter(_graphicsDevice);
            _camera = new OrthographicCamera(viewportAdapter);

            _playerSprite = new PlayerSprite(LoadTexture(PlayerFighter),
                                             LoadAnimation(PlayerAnimationTurnLeft),
                                             LoadAnimation(PlayerAnimationTurnRight));

            _playerSprite.Position = new Vector2(0, 0);
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            // parallax parameter affects how movement of camera is applied.
            // with Zero, the position of the camera doesn't change
            // with One, it moves as desired (ie 10 units per update, as written below)
            // with Two, it moves twice as fast
            //var transformMatrix = _camera.GetViewMatrix(Vector2.Zero);
            //var transformMatrix = _camera.GetViewMatrix(Vector2.One); // default
            //var transformMatrix = _camera.GetViewMatrix(Vector2.One * 2);

            var transformMatrix = _camera.GetViewMatrix();

            spriteBatch.Begin(transformMatrix: transformMatrix);
                _playerSprite.Render(spriteBatch);
            spriteBatch.End();
        }

        public override void HandleInput(GameTime gameTime)
        {
            InputManager.GetCommands(cmd =>
            {
                if (cmd is DevInputCommand.DevQuit)
                {
                    NotifyEvent(new BaseGameStateEvent.GameQuit());
                }

                if (cmd is DevInputCommand.DevCamLeft)
                {
                    _camera.Position += new Vector2(-10.0f, 0);
                }

                if (cmd is DevInputCommand.DevCamRight)
                {
                    _camera.Position += new Vector2(10.0f, 0);
                }

                if (cmd is DevInputCommand.DevCamUp)
                {
                    _camera.Position += new Vector2(0, -10.0f);
                }

                if (cmd is DevInputCommand.DevCamDown)
                {
                    _camera.Position += new Vector2(0, 10.0f);
                }

                if (cmd is DevInputCommand.DevCamRotateLeft)
                {
                    _camera.Rotate(-0.05f);
                }

                if (cmd is DevInputCommand.DevCamRotateRight)
                {
                    _camera.Rotate(0.05f);
                }

                if (cmd is DevInputCommand.DevPlayerUp)
                {
                    _playerSprite.MoveUp();
                }

                if (cmd is DevInputCommand.DevPlayerDown)
                {
                    _playerSprite.MoveDown();
                }

                if (cmd is DevInputCommand.DevPlayerRight)
                {
                    _playerSprite.MoveRight();
                }

                if (cmd is DevInputCommand.DevPlayerLeft)
                {
                    _playerSprite.MoveLeft();
                }

                if (cmd is DevInputCommand.DevPlayerStopsMovingHorizontal)
                {
                    _playerSprite.StopMoving();
                }

                if (cmd is DevInputCommand.DevPlayerStopsMovingVertical)
                {
                    _playerSprite.StopVerticalMoving();
                }

                KeepPlayerInBounds();

                if (cmd is DevInputCommand.DevShoot)
                {
                }
            });
        }

        private void KeepPlayerInBounds()
        {
            if (_playerSprite.Position.X < _camera.BoundingRectangle.Left)
            {
                _playerSprite.Position = new Vector2(0, _playerSprite.Position.Y);
            }

            if (_playerSprite.Position.X + _playerSprite.Width > _camera.BoundingRectangle.Right)
            {
                _playerSprite.Position = new Vector2(_camera.BoundingRectangle.Right - _playerSprite.Width, _playerSprite.Position.Y);
            }

            if (_playerSprite.Position.Y < _camera.BoundingRectangle.Top)
            {
                _playerSprite.Position = new Vector2(_playerSprite.Position.X, _camera.BoundingRectangle.Top);
            }

            if (_playerSprite.Position.Y + _playerSprite.Height > _camera.BoundingRectangle.Bottom )
            {
                _playerSprite.Position = new Vector2(_playerSprite.Position.X, _camera.BoundingRectangle.Bottom - _playerSprite.Height);
            }
        }

        public override void UpdateGameState(GameTime gameTime) 
        {
            _playerSprite.Update(gameTime);
            _camera.Position += new Vector2(0, -CAMERA_SPEED);
        }

        protected override void SetInputManager()
        {
            InputManager = new InputManager(new DevInputMapper());
        }
    }
}