using Chapter6Game.Content.Objects;
using Microsoft.Xna.Framework;
using Chapter6Game;
using Chapter6Game.Content;
using System.Diagnostics;

namespace MiniMan.Content.Objects
{
  public class Coins
    {
        Player player = new Player();
      public   Vector2 Position;
        public Rectangle coinRect;
        public SpriteAnimation anim;
        

        public Coins(GameRoot root)
        {
            Position = new Vector2(-600, 390);
            player = root.player;
            
          // coinRect  = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }
        public void Update (GameTime gameTime)
        {
            anim.Update(gameTime);
            anim.Position = Position;
           
            HandleCollision();
        }
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position,  10 );
            }
        }
        
       public void HandleCollision()
        {
            if (BoundingCircle.Intersects(player.playerRect))
            {
                
                Debug.WriteLine("Circle Collision Found");
            }
        }
    }
}
