using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace Chapter6Game.Content.Objects
{

    
    public class RedEnemy
    {
     
      public Player player = new Player();
        public Vector2 Position;
        public float speed = 0.5f;
        public float gravity;
        public Rectangle mainBody, topofHead;
        
        Terrain terrain = new Terrain();
        public bool isDead;
        public SpriteAnimation anim;
        public bool ispatroling = false;
        public RedEnemy(GameRoot root)
        {
            Position = new Vector2(-500, 350);
         
        }
        public void initialize()
        {
            topofHead = new Rectangle((int)Position.X, (int)Position.Y -10, 30, 1);
            mainBody = new Rectangle((int)Position.X, (int)Position.Y, 40, 44);

           
            player.Initialize();
        }
        public void Update(GameTime gameTime)
        {
            
            // Every function talked about in the book

            // SinuSoid Motion
            //Enemy.pos.X -= Enemy.speed;
            // Enemy.pos.Y = (float)MathF.Sin((float)gameTime.TotalGameTime.TotalMilliseconds / 100);

            // Circular Motion
            //Position.X -= (float)MathF.Cos((float)gameTime.TotalGameTime.TotalMilliseconds / 900) * speed;
            //Position.Y += (float)MathF.Sin((float)gameTime.TotalGameTime.TotalMilliseconds / 900) * speed;




            // Calculates the distance betweeen player and Enemy
            float distance = MathHelper.Distance(player.position.X, Position.X) / (float)gameTime.TotalGameTime.TotalSeconds + 10;
            // Debug.WriteLine(distance);


            ////Non-Linear Movement Script That makes a staircase motion
            ///
            //float distancebtwnX = MathHelper.Distance(Position.X, origin.X);
            //float distancebtwnY = MathHelper.Distance(Position.Y, origin.Y) ;
            //Debug.WriteLine(distancebtwnX);
            //  Debug.WriteLine(distancebtwnY);
            //if (distancebtwnX <= 100) 
            //    Position.X -= speed;

            //else if (distancebtwnY <= 200)
            //{
            //    Position.Y -= speed + 2;
            //    Debug.WriteLine("Go Up");
            //} else if (distancebtwnY >= 199)
            //{

            //    Position.X -= speed + 2;
            //    Debug.WriteLine("Go Left Again");

            //}



            //Regular Movement
            if (distance <= 100)
            {
                ispatroling = true;
                Position.X -= speed;
                
            }


            mainBody.X = (int)Position.X;
            mainBody.Y = (int)Position.Y;
            topofHead.X = (int)Position.X;
            topofHead.Y = (int)Position.Y;
            if(mainBody.Intersects(player.playerRect))
            {
                player.health--;
                Debug.WriteLine("Collision");
            }
            if (mainBody.Intersects(terrain.collisionRect[0]))
            {
                gravity = 0;
            }

            else
            {
               gravity = 2;
            }

            if (topofHead.Intersects(player.playerRect) && player.hasjumped)
            {
                
                
                gravity = 0;
            }
            anim.Position = Position;
            anim.Update(gameTime);
           Position.Y += gravity;
        }
        
    }

    public class BlueEnemy
    {
        public Player player = new Player();
        public Rectangle mainBody, topofHead;
        Terrain terrain = new Terrain();
        public Vector2 Position;
        public bool IsPatrolling = false;
        public bool ishit = false;
        public SpriteAnimation anim;
        public float speed = 1;
        public float gravity = 2;
        
        public BlueEnemy(GameRoot root)
        {
            Position = new Vector2(500, -200);
        }
        public void initialize()
        {
            topofHead = new Rectangle((int)Position.X, (int)Position.Y - 50, 10, 20);
            mainBody = new Rectangle((int)Position.X-30, (int)Position.Y, 100, 46);
         
        }
        public void Update(GameTime gameTime)
        {
            float distance = MathHelper.Distance(player.position.X, Position.X) - (float)gameTime.TotalGameTime.TotalSeconds -775;
          //  Debug.WriteLine(distance);
            if (distance <= 700)
            {
                IsPatrolling = true;
                Position.X -= 0.5f;
            }
    initialize();

            mainBody.X = (int)Position.X;
            mainBody.Y = (int)Position.Y;
            topofHead.X = (int)Position.X;
            topofHead.Y = (int)Position.Y;
            anim.Position = Position;
            anim.Update(gameTime);
            Position.Y += gravity;
            
            HandleCollisions();
           
        }
        public void HandleCollisions()
        {
            if (mainBody.Intersects(player.fistRect))
            {
                Debug.WriteLine("hit!");
                mainBody = new Rectangle((int)Position.X,(int) Position.Y , 0, 0);
            }
            if (mainBody.Intersects(terrain.collisionRect[4]))
            {
                gravity = 0;
            }
            else
            {
                gravity = 2;
            }
            if (mainBody.Intersects(terrain.collisionRect[0]))
            {
                gravity = 0;
            }
           
            if (mainBody.Intersects(terrain.collisionRect[3]))
            {
                gravity = 0;
            }
            
            if (mainBody.Intersects(terrain.collisionRect[2]))
            {
                gravity = 0;
            }
            if (topofHead.Intersects(player.playerRect))
            {
                player.health -= 1;
                player.position.X -= 2;
                player.position.Y -= 3;
            }
            if (mainBody.Intersects(player.fistRect))
            {
                
                ishit = true;
                mainBody.Height = 0;
                mainBody.Width = 0;
            }
        }
    }
    
    public class SamuraiBoss
    {
       
        public Player Player = new Player();
        public bool isdead = false; 
        Terrain terrain = new Terrain();
        public int health = 3;
        public SpriteAnimation anim;
        public Vector2 Position;
        public Rectangle mainBody, SamuraiSlash;
       public bool walking = false;
        public bool flip = false;
        public bool isattacking = false;
        
        public SamuraiBoss(GameRoot root)
        {
            Position = new Vector2(700, 230);
        }
        public void Initialize()
        {
            mainBody = new Rectangle((int)Position.X, (int)Position.Y, 100, 48);
            SamuraiSlash = new Rectangle((int)Position.X - 10, (int)Position.Y, 0, 0);
        }
        public void Update(GameTime gameTime)
        {
            Initialize();
            if (health != 0)
            {
                WalkLeftAndRight(gameTime);
                
                Attack(gameTime);
            }
           if (health <= 0)
            {
                isattacking = false;
                walking = false;
                isdead = true;
                Position.Y += 2;
            }
            anim.Position = Position;
            anim.Update(gameTime);  
        }
        public void WalkLeftAndRight(GameTime gametime)
        {
            float x = MathF.Cos(0.8f * (float)gametime.TotalGameTime.TotalSeconds) * 2;

            Debug.WriteLine(Position);
                Position.X -= x;
                walking = true;
            /// Add Sprite Flipping Logic Here
           if (Position.X <= 685 )
            {
                flip = true;
            }
            else { flip = false; }
        }
        
        public void Attack(GameTime gameTime)
        {
          float distance = Vector2.Distance(Player.position, Position);
            Debug.WriteLine(distance);
            if (distance <= 1400)
            {
                walking = false;
                isattacking = true;
                SamuraiSlash.Width = 30;
                SamuraiSlash.Height = 30;
            }
            
        }
      
    }

}
