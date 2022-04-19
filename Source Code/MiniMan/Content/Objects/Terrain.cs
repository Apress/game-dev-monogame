using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter6Game.Content.Objects
{
   public class Terrain
    {
        public Vector2 Position;
        public Texture2D levelTerrain, levelTerrain2;
        public Rectangle[] collisionRect = new Rectangle[9];

        public Rectangle[] sideRect = new Rectangle[5];
        public Terrain()
        {
            Position = new Vector2(-300, 250);
            // All Ground Collision
            collisionRect[0] = new Rectangle(-800, 410, 475, 1);
            collisionRect[1] = new Rectangle(-418, 358, 40, 50);
            collisionRect[2] = new Rectangle(-250, 350, 10, 150);
            collisionRect[3] = new Rectangle(-95, 410, 104, 150);
            collisionRect[4] = new Rectangle(60, 412, 600, 160);
            collisionRect[5] = new Rectangle(575, 290, 340, 150);
            collisionRect[6] = new Rectangle(165, 360, 50, 20);
            collisionRect[7] = new Rectangle(300, 360, 50, 20);
            collisionRect[8] = new Rectangle(450, 360, 50, 20);

            // All side collision;
            sideRect[0] = new Rectangle(-430, 400, 50, 20);
            sideRect[1] = new Rectangle(160, 400, 45, 20);
            sideRect[2] = new Rectangle(300, 400, 45, 20);
            sideRect[3] = new Rectangle(450, 400, 50, 20);
            sideRect[4] = new Rectangle(575, 300, 50, 100);
        }


        public void Initialize()
        {
           

            
        }
        public void LoadContent(ContentManager content)
        {
          levelTerrain =  content.Load<Texture2D>("Terrain/Level");
           levelTerrain2 = content.Load<Texture2D>("Terrain/Level-2");
        }
        
           public void Draw( SpriteBatch sprite)
        {
            sprite.Draw(levelTerrain, Position, Color.White);
         sprite.Draw(levelTerrain2, new Vector2(-850, 175), Color.White);
        }        
    }
}
