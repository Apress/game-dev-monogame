using Engine2D;
using Engine2D.Input;
using Engine2D.Objects;
using Engine2D.Objects.Collisions;
using Engine2D.PipelineExtensions;
using Engine2D.States;
using Game.Content;
using Game.Input;
using Game.Levels;
using Game.Objects;
using Game.Objects.Text;
using Game.States.Gameplay;
using Game.States.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.States
{
    public class GameplayState : BaseGameState
    {
        private const float SCROLLING_SPEED = 2.0f;

        private const string BackgroundTexture = "Sprites/Barren";
        private const string PlayerFighter = "Sprites/Animations/FighterSpriteSheet";
        private const string PlayerAnimationTurnLeft = "Sprites/Animations/FighterSpriteTurnLeft";
        private const string PlayerAnimationTurnRight = "Sprites/Animations/FighterSpriteTurnRight";
        private const string BulletTexture = "Sprites/bullet";
        private const string ExhaustTexture = "Sprites/Cloud";
        private const string MissileTexture = "Sprites/Missile";
        private const string ChopperTexture = "Sprites/Chopper";
        private const string ExplosionTexture = "Sprites/explosion";
        private const string TurretTexture = "Sprites/Turrets/Tower";
        private const string TurretMG2Texture = "Sprites/Turrets/MG2";
        private const string TurretBulletTexture = "Sprites/Turrets/Bullet_MG";

        private const string TextFont = "Fonts/Lives";
        private const string GameOverFont = "Fonts/GameOver";
        private const string StatsFont = "Fonts/Stats";

        private const string BulletSound = "Sounds/bulletSound";
        private const string MissileSound = "Sounds/missileSound";

        private const string Soundtrack1 = "Music/FutureAmbient_1";
        private const string Soundtrack2 = "Music/FutureAmbient_2";

        private const int StartingPlayerLives = 3;
        private int _playerLives = StartingPlayerLives;

        private const int MaxExplosionAge = 600; // 10 seconds
        private const int ExplosionActiveLength = 75; // emit particles for 1.2 seconds and let them fade out for 10 seconds

        private Texture2D _missileTexture;
        private Texture2D _exhaustTexture;
        private Texture2D _bulletTexture;
        private Texture2D _explosionTexture;
        private Texture2D _chopperTexture;
        private Texture2D _screenBoxTexture;

        private StatsObject _statsText;
        private LivesText _livesText;
        private GameOverText _levelStartEndText;
        private PlayerSprite _playerSprite;
        private bool _playerDead;
        private bool _gameOver = false;

        private bool _isShootingBullets;
        private bool _isShootingMissile;
        private TimeSpan _lastBulletShotAt;
        private TimeSpan _lastMissileShotAt;

        private GameObjectPool<BulletSprite> _bulletList = new GameObjectPool<BulletSprite>();
        private GameObjectPool<MissileSprite> _missileList = new GameObjectPool<MissileSprite>();
        private GameObjectPool<ExplosionEmitter> _explosionList = new GameObjectPool<ExplosionEmitter>();
        private GameObjectPool<ChopperSprite> _enemyList = new GameObjectPool<ChopperSprite>();
        private GameObjectPool<TurretBulletSprite> _turretBulletList = new GameObjectPool<TurretBulletSprite>();
        private GameObjectPool<TurretSprite> _turretList = new GameObjectPool<TurretSprite>();

        private ChopperGenerator _chopperGenerator;

        private Level _level;

        public override void LoadContent()
        {
            _missileTexture = LoadTexture(MissileTexture);
            _exhaustTexture = LoadTexture(ExhaustTexture);
            _bulletTexture = LoadTexture(BulletTexture);
            _explosionTexture = LoadTexture(ExplosionTexture);
            _chopperTexture = LoadTexture(ChopperTexture);

            var turnLeftAnimation = LoadAnimation(PlayerAnimationTurnLeft);
            var turnRightAnimation = LoadAnimation(PlayerAnimationTurnRight);
            _playerSprite = new PlayerSprite(LoadTexture(PlayerFighter), turnLeftAnimation, turnRightAnimation);

            _livesText = new LivesText(LoadFont(TextFont));
            _livesText.NbLives = StartingPlayerLives;
            _livesText.Position = new Vector2(10.0f, 690.0f);
            AddGameObject(_livesText);

            _statsText = new StatsObject(LoadFont(StatsFont));
            _statsText.Position = new Vector2(10, 10);

            if (Debug.Instance.IsDebugMode)
            {
                AddGameObject(_statsText);
            }

            _levelStartEndText = new GameOverText(LoadFont(GameOverFont));

            var background = new TerrainBackground(LoadTexture(BackgroundTexture), SCROLLING_SPEED);
            background.zIndex = -100;
            AddGameObject(background);

            // load sound effects and register in the sound manager
            var bulletSound = LoadSound(BulletSound);
            var missileSound = LoadSound(MissileSound);
            _soundManager.RegisterSound(new GameplayEvents.PlayerShootsBullets(), bulletSound);
            _soundManager.RegisterSound(new GameplayEvents.PlayerShootsMissile(), missileSound, 0.4f, -0.2f, 0.0f);

            // load soundtracks into sound manager
            var track1 = LoadSound(Soundtrack1).CreateInstance();
            var track2 = LoadSound(Soundtrack2).CreateInstance();
            _soundManager.SetSoundtrack(new List<SoundEffectInstance>() { track1, track2 });

            _chopperGenerator = new ChopperGenerator(AddChopper);

            var levelReader = new LevelReader(_contentManager, _viewportWidth);
            _level = new Level(levelReader);

            _level.OnGenerateEnemies += _level_OnGenerateEnemies;
            _level.OnGenerateTurret += _level_OnGenerateTurret;
            _level.OnLevelStart += _level_OnLevelStart;
            _level.OnLevelEnd += _level_OnLevelEnd;
            _level.OnLevelNoRowEvent += _level_OnLevelNoRowEvent;

            ResetGame();
        }

        public override void HandleInput(GameTime gameTime)
        {
            InputManager.GetCommands(cmd =>
            {
                if (cmd is GameplayInputCommand.GameExit)
                {
                    NotifyEvent(new BaseGameStateEvent.GameQuit());
                }

                if (cmd is GameplayInputCommand.PlayerMoveLeft && !_playerDead)
                {
                    _playerSprite.MoveLeft();
                    KeepPlayerInBounds();
                }

                if (cmd is GameplayInputCommand.PlayerMoveRight && !_playerDead)
                {
                    _playerSprite.MoveRight();
                    KeepPlayerInBounds();
                }

                if (cmd is GameplayInputCommand.PlayerStopsMoving && !_playerDead)
                {
                    _playerSprite.StopMoving();
                }

                if (cmd is GameplayInputCommand.PlayerMoveUp && !_playerDead)
                {
                    _playerSprite.MoveUp();
                    KeepPlayerInBounds();
                }

                if (cmd is GameplayInputCommand.PlayerMoveDown && !_playerDead)
                {
                    _playerSprite.MoveDown();
                    KeepPlayerInBounds();
                }

                if (cmd is GameplayInputCommand.PlayerShoots && !_playerDead)
                {
                    Shoot(gameTime);
                }
            });
        }

        public override void UpdateGameState(GameTime gameTime)
        {
            _playerSprite.Update(gameTime);

            _level.GenerateLevelEvents(gameTime);

            foreach (var bullet in _bulletList.ActiveObjects)
            {
                bullet.MoveUp();
            }

            foreach (var missile in _missileList.ActiveObjects)
            {
                missile.Update(gameTime);
            }

            foreach (var chopper in _enemyList.ActiveObjects)
            {
                chopper.Update();
            }

            foreach (var turret in _turretList.ActiveObjects)
            {
                turret.Update(gameTime, _playerSprite.CenterPosition);
                if (turret.Position.Y > 0 && turret.Position.Y < _viewportHeight)
                {
                    turret.CanAttack = true;
                }
            }

            foreach (var bullet in _turretBulletList.ActiveObjects)
            {
                bullet.Update();
            }

            UpdateExplosions(gameTime);
            RegulateShootingRate(gameTime);
            DetectCollisions();

            // deactivate game objects that have gone out of view
            DeactivateObjects(_bulletList);
            DeactivateObjects(_missileList);
            DeactivateObjects(_enemyList, chopper => chopper.Position.Y > _viewportHeight + 100);
            DeactivateObjects(_turretBulletList);
            DeactivateObjects(_turretList, turret => turret.Position.Y > _viewportHeight + 200);

            if (Debug.Instance.IsDebugMode)
            {
                _statsText.Update(gameTime);
            }
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            base.Render(spriteBatch);

            if (_gameOver)
            {
                // draw black rectangle at 30% transparency
                var screenBoxTexture = GetScreenBoxTexture(spriteBatch.GraphicsDevice);
                var viewportRectangle = new Rectangle(0, 0, _viewportWidth, _viewportHeight);
                spriteBatch.Draw(screenBoxTexture, viewportRectangle, Color.Black * 0.3f);
            }
        }

        private Texture2D GetScreenBoxTexture(GraphicsDevice graphicsDevice)
        {
            if (_screenBoxTexture == null)
            {
                _screenBoxTexture = new Texture2D(graphicsDevice, 1, 1);
                _screenBoxTexture.SetData<Color>(new Color[] { Color.White });
            }

            return _screenBoxTexture;
        }

        private void _level_OnLevelStart(object sender, PipelineExtensions.LevelEvent.StartLevel e)
        {
            _levelStartEndText.Text = Strings.GoodLuckPlayer1;
            _levelStartEndText.Position = new Vector2(350, 300);
            AddGameObject(_levelStartEndText);
        }

        private void _level_OnLevelEnd(object sender, PipelineExtensions.LevelEvent.EndLevel e)
        {
            _levelStartEndText.Text = Strings.YouEscaped;
            _levelStartEndText.Position = new Vector2(300, 300);
            AddGameObject(_levelStartEndText);
        }

        private void _level_OnLevelNoRowEvent(object sender, PipelineExtensions.LevelEvent.NoRowEvent e)
        {
            RemoveGameObject(_levelStartEndText);
        }

        private void _level_OnGenerateTurret(object sender, PipelineExtensions.LevelEvent.GenerateTurret e)
        {
            var turret = _turretList.GetOrCreate(() => new TurretSprite(LoadTexture(TurretTexture), LoadTexture(TurretMG2Texture)));

            // position the turret offscreen at the top
            turret.Position = new Vector2(e.XPosition, -100);
            turret.MoveSpeed = SCROLLING_SPEED;

            turret.OnTurretShoots += _turret_OnTurretShoots;
            turret.OnObjectChanged += _onObjectChanged;
            AddGameObject(turret);
        }

        private void _turret_OnTurretShoots(object sender, GameplayEvents.TurretShoots e)
        {
            var bulletPositions = new List<Vector2> { e.Bullet1Position, e.Bullet2Position };

            var bullet1 = _turretBulletList.GetOrCreate(() => new TurretBulletSprite(LoadTexture(TurretBulletTexture)));
            bullet1.Angle = e.Angle;
            bullet1.Direction = e.Direction;
            bullet1.Direction.Normalize();

            var bullet2 = _turretBulletList.GetOrCreate(() => new TurretBulletSprite(LoadTexture(TurretBulletTexture)));
            bullet2.Angle = e.Angle;
            bullet2.Direction = e.Direction;
            bullet2.Direction.Normalize();

            bullet1.Position = e.Bullet1Position;
            bullet1.zIndex = -10;

            bullet2.Position = e.Bullet2Position;
            bullet2.zIndex = -10;

            AddGameObject(bullet1);
            AddGameObject(bullet2);
        }

        private void _level_OnGenerateEnemies(object sender, PipelineExtensions.LevelEvent.GenerateEnemies e)
        {
            _chopperGenerator.GenerateChoppers(e.NbEnemies);
        }

        private void RegulateShootingRate(GameTime gameTime)
        {
            // can't shoot bullets more than every 0.2 second
            if (_lastBulletShotAt != null && gameTime.TotalGameTime - _lastBulletShotAt > TimeSpan.FromSeconds(0.2))
            {
                _isShootingBullets = false;
            }

            // can't shoot missiles more than every 1 second
            if (_lastMissileShotAt != null && gameTime.TotalGameTime - _lastMissileShotAt > TimeSpan.FromSeconds(1.0))
            {
                _isShootingMissile = false;
            }
        }

        private void DetectCollisions()
        {
            var bulletCollisionDetector = new AABBCollisionDetector<BulletSprite, BaseGameObject>(_bulletList.ActiveObjects);
            var missileCollisionDetector = new AABBCollisionDetector<MissileSprite, BaseGameObject>(_missileList.ActiveObjects);
            var playerCollisionDetector = new AABBCollisionDetector<ChopperSprite, PlayerSprite>(_enemyList.ActiveObjects);
            var turretBulletCollisionDetector = new SegmentAABBCollisionDetector<PlayerSprite>(_playerSprite);

            bulletCollisionDetector.DetectCollisions(_enemyList.ActiveObjects, (bullet, chopper) =>
            {
                var hitEvent = new GameplayEvents.ObjectHitBy(bullet);
                chopper.OnNotify(hitEvent);
                _soundManager.OnNotify(hitEvent);
                _bulletList.DeactivateObject(bullet, b => RemoveGameObject(b));
            });

            missileCollisionDetector.DetectCollisions(_enemyList.ActiveObjects, (missile, chopper) =>
            {
                HandleCollision(missile, chopper);
                _missileList.DeactivateObject(missile, m => RemoveGameObject(m));
            });

            bulletCollisionDetector.DetectCollisions(_turretList.ActiveObjects, (bullet, turret) =>
            {
                HandleCollision(bullet, turret);
                _bulletList.DeactivateObject(bullet, b => RemoveGameObject(b));
            });

            missileCollisionDetector.DetectCollisions(_turretList.ActiveObjects, (missile, turret) =>
            {
                HandleCollision(missile, turret);
                _missileList.DeactivateObject(missile, m => RemoveGameObject(m));
            });

            if (!_playerDead)
            {
                var segments = new List<Segment>();
                foreach (var bullet in _turretBulletList.ActiveObjects)
                {
                    segments.Add(bullet.CollisionSegment);
                }

                turretBulletCollisionDetector.DetectCollisions(segments, _ =>
                {
                    KillPlayer();
                });

                playerCollisionDetector.DetectCollisions(_playerSprite, (chopper, player) =>
                {
                    KillPlayer();
                });
            }
        }

        private void HandleCollision(BaseGameObject hitBy, BaseGameObject hitObject)
        {
            // only allow hitting objects if the projectile is still on the screen. Otherwise we can 
            // destroy turrets before they show up
            if (hitBy is IGameObjectWithDamage && hitBy.Position.Y >= 0)
            {
                var projectile = (IGameObjectWithDamage) hitBy;
                var hitEvent = new GameplayEvents.ObjectHitBy(projectile);

                hitObject.OnNotify(hitEvent);
                _soundManager.OnNotify(hitEvent);
            }
        }

        private void ResetGame()
        {
            if (_chopperGenerator != null)
            {
                _chopperGenerator.StopGenerating();
            }

            _bulletList.DeactivateAllObjects(obj => RemoveGameObject(obj));
            _missileList.DeactivateAllObjects(obj => RemoveGameObject(obj));
            _enemyList.DeactivateAllObjects(obj => RemoveGameObject(obj));
            _explosionList.DeactivateAllObjects(obj => RemoveGameObject(obj));
            _turretBulletList.DeactivateAllObjects(obj => RemoveGameObject(obj));
            _turretList.DeactivateAllObjects(obj => RemoveGameObject(obj));

            _playerSprite.Activate();
            AddGameObject(_playerSprite);

            // position the player in the middle of the screen, at the bottom, leaving a slight gap at the bottom
            var playerXPos = _viewportWidth / 2 - _playerSprite.Width / 2;
            var playerYPos = _viewportHeight - _playerSprite.Height - 30;
            _playerSprite.Position = new Vector2(playerXPos, playerYPos);

            _playerDead = false;
            _level.Reset();
        }

        private async void KillPlayer()
        {
            if (_indestructible || _playerDead)
            {
                return;
            }

            _playerDead = true;
            _playerLives -= 1;
            _livesText.NbLives = _playerLives;

            AddExplosion(_playerSprite.Position);
            RemoveGameObject(_playerSprite);

            await Task.Delay(TimeSpan.FromSeconds(2));

            if (_playerLives > 0)
            {
                ResetGame();
            }
            else
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            var font = LoadFont(GameOverFont);
            var gameOverText = new GameOverText(font);
            var textPositionOnScreen = new Vector2(460, 300);

            gameOverText.Position = textPositionOnScreen;
            AddGameObject(gameOverText);
            _gameOver = true;
        }

        private void AddChopper(bool generateOnLeftSide)
        {
            Vector2 leftVector = new Vector2(-1, 0);
            Vector2 rightVector = new Vector2(1, 0);
            Vector2 downLeftVector = new Vector2(-1, 1);
            Vector2 downRightVector = new Vector2(1, 1);
            downLeftVector.Normalize();
            downRightVector.Normalize();

            List<(int, Vector2)> path;
            Vector2 pos;
            if (generateOnLeftSide)
            {
                path = new List<(int, Vector2)>
                {
                    (0, rightVector),
                    (2 * 60, downRightVector),
                };

                pos = new Vector2(-200, 100);
            }
            else
            {
                path = new List<(int, Vector2)>
                {
                    (0, leftVector),
                    (2 * 60, downLeftVector),
                };

                pos = new Vector2(1500, 100);
            }

            var newChopper = _enemyList.GetOrCreate(() => new ChopperSprite(_chopperTexture));

            newChopper.Position = pos;
            newChopper.Path = path;
            newChopper.OnObjectChanged += _onObjectChanged;
            AddGameObject(newChopper);
        }

        private void DeactivateObjects<T>(GameObjectPool<T> objectList, Func<T, bool> predicate) where T : BaseGameObject
        {
            foreach(T item in objectList.ActiveObjects)
            {
                if (predicate(item))
                {
                    objectList.DeactivateObject(item, _ => RemoveGameObject(item));
                }
            }
        }

        private void DeactivateObjects<T>(GameObjectPool<T> objectList) where T : BaseGameObject
        {
            DeactivateObjects(objectList, item => 
                item.Position.Y < -50 || 
                item.Position.Y > _viewportHeight + 50 ||
                item.Position.X < -50 ||
                item.Position.X > _viewportWidth + 50
            );
        }

        private void _onObjectChanged(object sender, BaseGameStateEvent e)
        {
            var gameObject = (BaseGameObject)sender;
            switch (e)
            {
                case GameplayEvents.ObjectLostLife ge:
                    if (ge.CurrentLife <= 0)
                    {
                        AddExplosion(new Vector2(gameObject.Position.X - 40, gameObject.Position.Y - 40));

                        switch (gameObject)
                        {
                            case ChopperSprite c:
                                _enemyList.DeactivateObject(c);
                                break;

                            case TurretSprite t:
                                _turretList.DeactivateObject(t);
                                break;
                        }

                        RemoveGameObject(gameObject);
                    }
                    break;
            }
        }

        private void AddExplosion(Vector2 position)
        {
            var explosion = _explosionList.GetOrCreate(() => new ExplosionEmitter(_explosionTexture));
            explosion.Position = position;

            AddGameObject(explosion);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            foreach (var explosion in _explosionList.ActiveObjects)
            {
                explosion.Update(gameTime);
                
                if (explosion.Age > ExplosionActiveLength)
                {
                    explosion.Deactivate();
                }

                if (explosion.Age > MaxExplosionAge)
                {
                    RemoveGameObject(explosion);
                }
            }
        }
 
        private void Shoot(GameTime gameTime)
        {
            if (!_isShootingBullets)
            {
                CreateBullets();
                _isShootingBullets = true;
                _lastBulletShotAt = gameTime.TotalGameTime;

                NotifyEvent(new GameplayEvents.PlayerShootsBullets());
            }

            if (!_isShootingMissile)
            {
                CreateMissile();
                _isShootingMissile = true;
                _lastMissileShotAt = gameTime.TotalGameTime;

                NotifyEvent(new GameplayEvents.PlayerShootsMissile());
            }
        }

        private void CreateBullets()
        {
            var bulletY = _playerSprite.Position.Y + 30;
            var bulletLeftX = _playerSprite.Position.X + _playerSprite.Width / 2 - 40;
            var bulletRightX = _playerSprite.Position.X + _playerSprite.Width / 2 + 10;

            var bullet1 = _bulletList.GetOrCreate(() => new BulletSprite(_bulletTexture));
            var bullet2 = _bulletList.GetOrCreate(() => new BulletSprite(_bulletTexture));

            bullet1.Position = new Vector2(bulletLeftX, bulletY);
            bullet2.Position = new Vector2(bulletRightX, bulletY);

            AddGameObject(bullet1);
            AddGameObject(bullet2);
        }

        private void CreateMissile()
        {
            var missileSprite = _missileList.GetOrCreate(() => new MissileSprite(_missileTexture, _exhaustTexture));
            missileSprite.Position = new Vector2(_playerSprite.Position.X + 33, _playerSprite.Position.Y - 25);
            AddGameObject(missileSprite);
        }

        private void KeepPlayerInBounds()
        {
            if (_playerSprite.Position.X < 0)
            {
                _playerSprite.Position = new Vector2(0, _playerSprite.Position.Y);
            }

            if (_playerSprite.Position.X > _viewportWidth - _playerSprite.Width)
            {
                _playerSprite.Position = new Vector2(_viewportWidth - _playerSprite.Width, _playerSprite.Position.Y);
            }

            if (_playerSprite.Position.Y < 0)
            {
                _playerSprite.Position = new Vector2(_playerSprite.Position.X, 0);
            }

            if (_playerSprite.Position.Y > _viewportHeight - _playerSprite.Height)
            {
                _playerSprite.Position = new Vector2(_playerSprite.Position.X, _viewportHeight - _playerSprite.Height);
            }
        }

        protected override void SetInputManager()
        {
            InputManager = new InputManager(new GameplayInputMapper());
        }
    }
}