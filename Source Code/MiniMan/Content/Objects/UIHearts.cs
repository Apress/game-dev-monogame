using Chapter6Game.Content.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace MiniMan.Content.Objects
{
   public  class UIHearts
    {
        public Player player = new Player();
        public Texture2D fullHeart;
        public Texture2D emptyHeart;

        public Vector2[] Positions = new Vector2[3];
        public void LoadContent(ContentManager Content)
        {
           emptyHeart = Content.Load<Texture2D>("UI/EmptyHeart");
            fullHeart = Content.Load<Texture2D>("UI/Heart");
        }

        public void Initialize()
        {
            Positions[0] = new Vector2(-795, 90);
            Positions[1] = new Vector2(-820, 90);
            Positions[2] = new Vector2(-845, 90);
        }
        public void Draw(SpriteBatch batch)
        {
            if (player.health == 3)
            {

                batch.Draw(fullHeart, Positions[0], Color.White);
                batch.Draw(fullHeart, Positions[1], Color.White);
                batch.Draw(fullHeart, Positions[2], Color.White);
            }
            else if (player.health == 2)
            {
                batch.Draw(fullHeart, Positions[0], Color.White);
                batch.Draw(fullHeart, Positions[1], Color.White);
                batch.Draw(emptyHeart, Positions[2], Color.White);
            }

            else if (player.health == 1)
            {
                batch.Draw(fullHeart, Positions[0], Color.White);
                batch.Draw(emptyHeart, Positions[1], Color.White);
                batch.Draw(emptyHeart,Positions[2], Color.White);
            }

            else if (player.health <= 0)
            {
                
                    batch.Draw(emptyHeart, Positions[0], Color.White);
                    batch.Draw(emptyHeart, Positions[1], Color.White);
                    batch.Draw(emptyHeart, Positions[2], Color.White);
                
            }
        }

    }
}
