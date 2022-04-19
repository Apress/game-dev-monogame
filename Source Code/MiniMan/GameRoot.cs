using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using Chapter6Game.Content;
using System.Diagnostics;
using MonoGame.Extended;
using System.Collections.Generic;
using Chapter6Game.Content.Objects;
using MonoGame.Extended.ViewportAdapters;
using MiniMan.Content.Objects;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Chapter6Game
{
    public class GameRoot : Game
    {

        #region Objects
        bool gameStarted = false;

        OrthographicCamera camera;


      
        bool playerisDead = false;
        bool punchisActive = false;
        bool playerWon = false;

        ParticleEngine particleEngine;

        public SamuraiBoss samurai;
        public RedEnemy redEnemy;
        public BlueEnemy blueEnemy;



        InputManager Input;
        GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

        GamePadCapabilities[] controllers = new GamePadCapabilities[4];

        
        
        float punchDelay = 0;

        // Put Camera at X: 745 when at end of level
        public Vector2 CameraPos;
        public Player player = new Player();
        

        Terrain terrain = new Terrain();
        Coins coin;
        
        public SpriteFont font;

        bool flip = false;
        
        bool paused = false;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        #endregion
        #region Textures
        private Texture2D background { get; set; }

        /// <summary>
        ///  Player Sprites and Animation Field
        /// </summary>
        int AnimState;
        //Player Animation
        private Texture2D idle, Jump, run, Damaged, punch, Fist;
        //coin animations
        private Texture2D coinIdle, coinSpark;

        //Red Enemy
        private Texture2D Redidle, patrol, stomp;
        //Blue Enemy
        private Texture2D BlueIdle, Bluepatrol, blueHit;
        // Samurai Boss 
        private Texture2D Samuraiidle, Slash, samuraiRun, samuraihit;

        public SpriteAnimation[] animations = new SpriteAnimation[5];
        public SpriteAnimation[] coinAnims = new SpriteAnimation[2];
        public SpriteAnimation[] blueAnimations = new SpriteAnimation[3];
        public SpriteAnimation[] redAnimations = new SpriteAnimation[3];
        public SpriteAnimation[] samuraiAnimations = new SpriteAnimation[4];

        bool hasHitPlayer = false;

        UIHearts hearts = new UIHearts();
        public Effect effect;

        #endregion

        #region Audio
        SoundEffect jumpSnd, AttackSound, getHit, EnemyHitSound, CoinSnd, SamuraiSlash; 
        Song[] songs = new Song[4];
            Song  main, endSong;
        
        #endregion
        public GameRoot()
        {
            Input = new InputManager(this);
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            coin = new Coins(this);
            redEnemy = new RedEnemy(this);
            blueEnemy = new BlueEnemy(this);
            samurai = new SamuraiBoss(this);
        }

        protected override void Initialize()
        {
            this.Components.Add(Input);
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 360, 360);
            camera = new OrthographicCamera(viewportAdapter);

           

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            player.Initialize();
            terrain.Initialize();
            hearts.Initialize();
            redEnemy.initialize();
            blueEnemy.initialize();
            samurai.Initialize();



            CameraPos = player.position -= new Vector2(-35, 50);
            base.Initialize();
        }

        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Player Sprites
            idle = Content.Load<Texture2D>("Characters/Player/PlayerIdle");
            run = Content.Load<Texture2D>("Characters/Player/PlayerRun");
            punch = Content.Load<Texture2D>("Characters/Player/PlayerPunch");
            Fist = Content.Load<Texture2D>("Characters/Player/PlayerFist");
            Jump = Content.Load<Texture2D>("Characters/Player/PlayerJump");
            Damaged = Content.Load<Texture2D>("Characters/Player/PlayerDamaged");
            #endregion

            List<Texture2D> textures = new List<Texture2D>(); // Loads in our particle textures to be used for the particle engine
            textures.Add(Content.Load<Texture2D>("Particles/diamond"));
            textures.Add(Content.Load<Texture2D>("Particles/star"));
            particleEngine = new ParticleEngine(textures, player.position); // Used to draw and update textures and location of particle engine


            coinIdle = Content.Load<Texture2D>("Collectibles/Coins");
            coinSpark = Content.Load<Texture2D>("Collectibles/CoinSpark");
            // Red Enemy
            Redidle = Content.Load<Texture2D>("Characters/Enemies/RedIdle");
            patrol = Content.Load<Texture2D>("Characters/Enemies/RedPatrol");
            stomp = Content.Load<Texture2D>("Characters/Enemies/RedStomped");


            #region Enemy Sprites
            //Blue Enemy
            BlueIdle = Content.Load<Texture2D>("Characters/Enemies/BlueIdle");
            blueHit = Content.Load<Texture2D>("Characters/Enemies/BlueIdle");
            Bluepatrol = Content.Load<Texture2D>("Characters/Enemies/BluePatrol");

            //Samurai Boss
            Samuraiidle = Content.Load<Texture2D>("Characters/Enemies/SamuraiIdle");
            Slash = Content.Load<Texture2D>("Characters/Enemies/SamuraiAttack");
            samuraiRun = Content.Load<Texture2D>("Characters/Enemies/SamuraiRun");
            samuraihit = Content.Load<Texture2D>("Characters/Enemies/SamuraiDamaged");

            animations[0] = new SpriteAnimation(idle, 4, 8);
            animations[1] = new SpriteAnimation(run, 3, 8);
            animations[2] = new SpriteAnimation(Jump, 2, 1);
            animations[3] = new SpriteAnimation(punch, 4, 8);
            animations[4] = new SpriteAnimation(Damaged, 1, 1);

            coinAnims[0] = new SpriteAnimation(coinIdle, 4, 8);
            coinAnims[1] = new SpriteAnimation(coinSpark, 4, 7);
            coinAnims[1].IsLooping = false;
            #endregion


            #region Audio Load
            jumpSnd = Content.Load<SoundEffect>("Fx/Jump");
            CoinSnd = Content.Load<SoundEffect>("Fx/Coin");
            getHit = Content.Load<SoundEffect>("Fx/GetHit");
            EnemyHitSound = Content.Load<SoundEffect>("Fx/EnemyHit"
               );
            AttackSound = Content.Load<SoundEffect>("Fx/Punch");
            SamuraiSlash = Content.Load<SoundEffect>("Fx/Slash");


            main =  Content.Load<Song>("Music/On My Way");
           
            endSong = Content.Load<Song>("Music/Victory");
            #endregion

            // red enemy Animations
            redAnimations[0] = new SpriteAnimation(Redidle, 2, 2);
            redAnimations[1] = new SpriteAnimation(patrol, 6, 5);
            redAnimations[2] = new SpriteAnimation(stomp, 2, 1);

            redAnimations[2].IsLooping = false;

            // blue Animation
            blueAnimations[0] = new SpriteAnimation(BlueIdle, 2, 2);
            blueAnimations[1] = new SpriteAnimation(Bluepatrol, 6, 2);
            blueAnimations[2] = new SpriteAnimation(blueHit, 2, 2);
            blueAnimations[2].IsLooping = false;

            // Samurai Animation
            samuraiAnimations[0] = new SpriteAnimation(Samuraiidle, 3, 4);
            samuraiAnimations[1] = new SpriteAnimation(samuraiRun, 4, 8);
            samuraiAnimations[2] = new SpriteAnimation(Slash, 7, 10);
            samuraiAnimations[3] = new SpriteAnimation(samuraihit, 1, 1);

            // Sets the default animation to Idle
            player.anim = animations[0];
            coin.anim = coinAnims[0];

            redEnemy.anim = redAnimations[0];
            blueEnemy.anim = redAnimations[0];


            samurai.anim = samuraiAnimations[0];
            font = Content.Load<SpriteFont>("Font");

            background = Content.Load<Texture2D>("Terrain/Sky");
            terrain.LoadContent(Content);
            hearts.LoadContent(Content);

            effect = Content.Load<Effect>("PixelShader");

        }


        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gameStarted = true;
           //     MediaPlayer.Play(main);
            }
            
            
            punchDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (samurai.isdead)
            {
                MediaPlayer.Play(endSong);
            }


            if (gameStarted)
                {

                #region Pausing
               

                if (!paused)
                    {
                        camera.LookAt(CameraPos);
                    }
                    Input.Update(gameTime);
                    if (Input.IsPressed(Keys.P))
                    {
                        paused = true;
                    }
                    else if (Input.IsPressed(Keys.O))
                    {
                        paused = false;
                    }

                #endregion
                redEnemy.Update(gameTime);
                blueEnemy.Update(gameTime);
                samurai.Update(gameTime);

                if (player.position.Y > 450 || player.health <= 0)
                {
                    playerisDead = true;
                }
                if (playerisDead == true)
                {
                    player.playerRect = new Rectangle((int)player.position.X, (int)player.position.Y, 0, 0);
                    player.anim = animations[4];
                }

                #region Keyboard Input
                if (Input.IsPressed(Keys.W) && !player.hasjumped)
                    {
                    jumpSnd.Play();
                        player.position.Y -= 14;
                        player.gravity = -7.5f;
                        player.hasjumped = true;
                   
                }
                

                if (Input.IsPressed(Keys.K) && punchDelay >= 0.65f)
                    {
                    punchisActive = true;
                    punchDelay = 0;
                    AttackSound.Play();
                }
                else
                {
                    punchisActive = false;
                }
               
                
                if (punchisActive)
                {
                    player.fistRect = new Rectangle((int)player.position.X , (int)player.position.Y, 25, 20);
                    Debug.WriteLine("Is Punch Active?: " + punchisActive + player.fistRect);
                    Debug.WriteLine(blueEnemy.mainBody.X);
                    Debug.WriteLine(blueEnemy.Position.X);

                }
                if (flip && Input.IsPressed(Keys.K))
                {
                    player.fistRect = new Rectangle((int)player.position.X - 10, (int)player.position.Y, 0, 0);
                }
                else if (Input.IsPressed(Keys.K))
                {
                    player.fistRect = new Rectangle((int)player.position.X + 10, (int)player.position.Y, 0, 0);
                }
                
                if (Input.IsPressed(Keys.K))
                {
                    AnimState = 3;
                }
                
                    else
                    {
                        AnimState = 0;
                    
                    }

                    if (Input.IsPressed(Keys.A) && !player.isCollidingside && !playerisDead)
                    {

                        flip = true;
                        AnimState = 1;
                    player.position.X -= player.speed;

                    
                    if (player.position.X <700)
                    {
                        CameraPos.X -= player.speed;

                        hearts.Positions[0].X -= player.speed;
                        hearts.Positions[1].X -= player.speed;
                        hearts.Positions[2].X -= player.speed;
                    }
                    }
                 

                if (Input.IsPressed(Keys.D) && !player.isCollidingside && !playerisDead)
                {
                    player.position.X = player.position.X + player.speed;
                    flip = false;
                    if (player.position.X < 745)
                    {
                        CameraPos.X += player.speed;


                        hearts.Positions[0].X += player.speed;
                        hearts.Positions[1].X += player.speed;
                        hearts.Positions[2].X += player.speed;
                        AnimState = 1;
                    }
                }
                else
                {

                }

                    HandleAnimationCollisions();
                #endregion
                
                    if (!paused || !playerisDead || !playerWon)
                    {
                    
                   
                        coin.Update(gameTime);
                        AnimStates();

                    if (!playerisDead || !playerWon)
                    {
                        player.Update(gameTime);
                    }
                    }


                    particleEngine.EmitterLoc = new Vector2(player.position.X + 21, player.position.Y + 30);
                    particleEngine.Update();

                    #region Controller Input
                    if (capabilities.IsConnected)
                    {
                        AnimStates();

                        GamePadState state = GamePad.GetState(PlayerIndex.One);
                        if (state.IsButtonDown(Buttons.B))
                        {
                            AnimState = 3;
                        }
                        if (state.IsButtonDown(Buttons.Y))
                        {
                            AnimState = 3;
                        }
                        if (state.IsButtonDown(Buttons.A) && !player.hasjumped)
                        {

                            player.position.Y -= 14;
                            player.gravity = -7.5f;
                            player.hasjumped = true;

                        }
                        if (state.IsButtonDown(Buttons.X) && !player.hasjumped)
                        {

                            player.position.Y -= 14;
                            player.gravity = -7.5f;
                            player.hasjumped = true;

                        }

                        if (state.IsButtonDown(Buttons.DPadLeft) && !player.isCollidingside)
                        {
                            flip = true;
                            AnimState = 1;
                            player.position.X -= player.speed;
                            CameraPos.X -= player.speed;
                            hearts.Positions[0].X -= player.speed;
                            hearts.Positions[1].X -= player.speed;
                            hearts.Positions[2].X -= player.speed;
                         //   Debug.WriteLine("Animation Position: " + player.anim.Position);
                        }


                        if (state.IsButtonDown(Buttons.DPadRight) && !player.isCollidingside)
                        {
                        
                            flip = false;
                            AnimState = 1;
                            player.position.X += player.speed;
                            CameraPos.X += player.speed;
                            hearts.Positions[0].X += player.speed;
                            hearts.Positions[1].X += player.speed;
                            hearts.Positions[2].X += player.speed;
                        }


                        if (capabilities.HasLeftXThumbStick)
                        {
                            //Moves player with the Thumbstick
                            if (state.ThumbSticks.Left.X < -0.5f && !player.isCollidingside)
                            {
                                flip = true;
                                AnimState = 1;
                                player.position.X -= player.speed;
                                CameraPos.X -= player.speed;
                                hearts.Positions[0].X -= player.speed;
                                hearts.Positions[1].X -= player.speed;
                                hearts.Positions[2].X -= player.speed;
                            }
                            if (state.ThumbSticks.Left.X > .5f && !player.isCollidingside)
                            {
                                flip = false;
                                AnimState = 1;
                                player.position.X += player.speed;
                                CameraPos.X += player.speed;
                                hearts.Positions[0].X += player.speed;
                                hearts.Positions[1].X += player.speed;
                                hearts.Positions[2].X += player.speed;
                            }
                        }
                    }

                    #endregion
                }
            
            base.Update(gameTime);
        }
        #region debug
        public void DebugPlayer()
        {


            if (Input.IsHeld(Keys.NumPad0))
            {
                player.speed -= 1;
                Debug.WriteLine(player.speed);
            }
            if (Input.IsHeld(Keys.NumPad1))
            {
                player.speed += 1;
                Debug.WriteLine(player.speed);
            }
            if (Input.IsPressed(Keys.NumPad2))
            {
                player.gravity--;
                Debug.WriteLine(player.gravity);
            }
            if (Input.IsPressed(Keys.NumPad3))
            {
                player.gravity++;
                Debug.WriteLine(player.gravity);
            }

            if (Input.IsHeld(Keys.NumPad4))
            {
                player.playerRect.Width--;
            }

            if (Input.IsHeld(Keys.NumPad5))
            {
                player.playerRect.Width++;
                Debug.WriteLine(player.playerRect.Width);
            }

            if (Input.IsHeld(Keys.NumPad6))
            {
                player.playerRect.Height--;
                Debug.WriteLine(player.playerRect.Height);
            }
            if (Input.IsHeld(Keys.NumPad7))
            {
                player.playerRect.Height++;
                Debug.WriteLine(player.playerRect.Height);
            }
        }
        #endregion
        public void HandleAnimationCollisions()
        {
            if (player.playerRect.Intersects(redEnemy.mainBody))
            {
                Debug.WriteLine("collision");
                getHit.Play();
                //  player.health--;
                //player.anim = animations[4];

            }
            if (samurai.mainBody.Intersects(player.playerRect))
            {
                player.speed = 0;
            }
            else
            {
                player.speed = 4;
            }
            if (player.fistRect.Intersects(samurai.mainBody))
            {
                Debug.WriteLine("Boss Hit");
                player.fistRect = Rectangle.Empty;
                EnemyHitSound.Play();
                samurai.health -= 1;
            }
            if (player.playerRect.Intersects(blueEnemy.topofHead))
            {
                //player.health--;
                player.position.X -= player.speed * 1.5f;
                getHit.Play();
               
            }

            if(player.playerRect.Intersects(redEnemy.topofHead) && player.hasjumped)
            {

                // Makes all of the enemies' rect properties 0 to prevent continuous sound calls
                EnemyHitSound.Play();
                redEnemy.mainBody.Height = 0;
                redEnemy.mainBody.Width = 0;
                redEnemy.topofHead.Width = 0;
                redEnemy.topofHead.Height = 0;
                player.position.Y -= player.speed * 6;
                redEnemy.anim = redAnimations[2];
                redEnemy.speed = 0;
                
            }
            if (player.fistRect.Intersects(blueEnemy.mainBody) && Input.IsPressed(Keys.K))
            {
           
               EnemyHitSound.Play();
                blueEnemy.ishit = true; 
                blueEnemy.speed = 0;
                player.fistRect.Width = 0;
                player.fistRect.Height = 0;
            }

            if (player.playerRect.Intersects(samurai.SamuraiSlash)){
                Debug.WriteLine("Slashed");
                //player.health -= 1;
                player.position.X -= 3;
                hasHitPlayer = true;
            }
            
            if (samurai.isdead)
            {
                samurai.anim = samuraiAnimations[3];
            }
          if (samurai.isattacking)
            {
                samurai.anim = samuraiAnimations[2];
            }
            
            
            if (coin.BoundingCircle.Intersects(player.playerRect))
            {
               
              
                CoinSnd.Play();
                coin.Position = new Vector2(0, 0);
                coin.anim.Position = new Vector2(-600, 390);
                coin.anim = coinAnims[1];
            }
            
          
          
            if (samurai.walking)
            {
                samurai.anim = samuraiAnimations[1];
            }
            if (redEnemy.ispatroling)
            {
                redEnemy.anim = redAnimations[1];
            }
            if (blueEnemy.IsPatrolling)
            {
                blueEnemy.anim = blueAnimations[1];
            }
            if (blueEnemy.ishit) 
            {
                blueEnemy.anim = blueAnimations[2];
            }
        }


        public void AnimStates()
        {
            switch (AnimState)
            {
                case 1:
                    AnimState = 1;
                    player.anim = animations[1];
                    break;
                case 2:
                    AnimState = 2;
                    player.anim = animations[4];
                    break;
                case 3:
                    AnimState = 3;
                    player.anim = animations[3];
                    break;
                default:
                    AnimState = 0;
                    player.anim = animations[0];
                    break;
            }

            
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var transFormMatrix = camera.GetViewMatrix();
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, transformMatrix: transFormMatrix);

            if (gameStarted)
            {
                #region Textures and Shader
               // effect.CurrentTechnique.Passes[0].Apply();

                _spriteBatch.Draw(background, new Vector2(-1413, 50), Color.White);
                _spriteBatch.Draw(background, new Vector2(-942, 50), Color.White);
                _spriteBatch.Draw(background, new Vector2(-471, 50), Color.White);
                _spriteBatch.Draw(background, new Vector2(0, 50), Color.White);
                _spriteBatch.Draw(background, new Vector2(471, 50), Color.White);
                _spriteBatch.Draw(background, new Vector2(942, 50), Color.White);
                #endregion

                terrain.Draw(_spriteBatch);
                if (!playerisDead)
                {
                    particleEngine.Draw(_spriteBatch);
                }
                
                if (!blueEnemy.ishit)
                {
                    blueEnemy.anim.Draw(_spriteBatch, SpriteEffects.None);
                }
                redEnemy.anim.Draw(_spriteBatch, SpriteEffects.None);
                if (flip)
                {
                    player.anim.Draw(_spriteBatch, SpriteEffects.FlipHorizontally);
                }
                else
                {
                    player.anim.Draw(_spriteBatch, SpriteEffects.None);
                }
                if(samurai.flip)
                {
                    samurai.anim.Draw(_spriteBatch, SpriteEffects.None);
                } else
                samurai.anim.Draw(_spriteBatch, SpriteEffects.FlipHorizontally);
                coin.anim.Draw(_spriteBatch, SpriteEffects.None);
                hearts.Draw(_spriteBatch);
            }

            if (!gameStarted)
            {
                string title = "Mini Man";
                string startText = "Press Enter To Start";
                _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                
                _spriteBatch.DrawString(font, title, new Vector2(150, 200), Color.Black);

                _spriteBatch.DrawString(font, startText, new Vector2(115, 220), Color.Black);

            }
          
           


            if (playerisDead)
            {
               
                MediaPlayer.Play(endSong);
                string gameOver = "Game Over";
               
                _spriteBatch.DrawString(font, gameOver, CameraPos- new Vector2(10, 30 ), Color.Black);
            }

            if (playerWon)
            {
                string Won = "Congrats, you won!";
                _spriteBatch.DrawString(font, Won, CameraPos, Color.Black);
            }

          if  (samurai.health <= 0)
           {
                string conText = "Congrats, you won!";

               _spriteBatch.DrawString(font, conText, CameraPos, Color.Black);
           }

            _spriteBatch.End();

            base.Draw(gameTime);

        }
    }
}
