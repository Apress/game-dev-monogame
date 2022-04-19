using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Chapter6Game;
using System.Collections.Generic;
using System;

namespace Chapter6Game.Content.Objects
{  public class Player
    {
       
        public int health = 3;
        public bool isCollidingside = false;
      public Rectangle playerRect;
        public Rectangle fistRect;
        public Texture2D pixel;
        public Vector2 velocity;
        public Vector2 position;
        public float speed = 4;
        public float gravity = 2;
        
        
        public SpriteAnimation anim;
       
        public Vector2 Velocity;
        public SpriteEffects flip;
      
        public bool hasjumped =false;

        Terrain terrain = new Terrain();
       
        public bool iscolliding = false;
       
      
        public Player()
        {
           
            position = new Vector2(-700, 300);
        }

        public void Initialize()
        {
            
            playerRect = new Rectangle((int)position.X, (int)position.Y, 32, 40);
            fistRect = new Rectangle(0, 0, 0, 0);
        }

        public void HandleCollisions()
        {

        
            #region Terrain Collision
            if (playerRect.Intersects(terrain.collisionRect[0]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
             //  Debug.WriteLine("Collision Found");
            }
            else
            {
                float i = 1;
                gravity += 0.15f * i;
            }
            if (playerRect.Intersects(terrain.collisionRect[1]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
              
            }
            if (playerRect.Intersects(terrain.collisionRect[2]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
         
            }
            if (playerRect.Intersects(terrain.collisionRect[3]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
          
            }
            if (playerRect.Intersects(terrain.collisionRect[4]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
          //   Debug.WriteLine("Collision Found");
            }

            if (playerRect.Intersects(terrain.collisionRect[5]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
        //        Debug.WriteLine("Collision Found");
            }

            if (playerRect.Intersects(terrain.collisionRect[6]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
             //   Debug.WriteLine("Collision Found");
            }
            if (playerRect.Intersects(terrain.collisionRect[7]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
             
            }

            if (playerRect.Intersects(terrain.collisionRect[8]))
            {
                gravity = 0;
                hasjumped = false;
                iscolliding = true;
                Debug.WriteLine("Last Platform");
            }



            if (playerRect.Intersects(terrain.sideRect[0]))
            {
                isCollidingside = true;
            }
            else
            {
                isCollidingside = false;
            }
            if (playerRect.Intersects(terrain.sideRect[1]))
            {
                isCollidingside = true;
            }

            if (playerRect.Intersects(terrain.sideRect[2]))
            {
                isCollidingside = true;
            }
            if (playerRect.Intersects(terrain.sideRect[3]))
            {
                isCollidingside = true;
            }
            if (playerRect.Intersects(terrain.sideRect[4]))
            {
                isCollidingside = true;
            }
            #endregion

            
        }
        public void Update(GameTime gameTime)
        {

            Initialize();
            anim.Position = position;  
            anim.Update(gameTime);
            playerRect.X = (int)position.X;
            playerRect.Y = (int)position.Y;
            HandleCollisions();
           //Gravity Logic
             position.Y += gravity;
           
            if (hasjumped)
            {
                float i = 1;
                gravity += 0.15f * i;
            }
            
        }

    }
}
