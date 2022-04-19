using Engine2D.Input;
using Engine2D.States;
using Game.Input;
using Game.Objects;
using Microsoft.Xna.Framework;

namespace Game.States
{
    public class SplashState : BaseGameState
    {
        public override void LoadContent()
        {
            var splash = new SplashImage(LoadTexture("Images/splash"));
            splash.Activate();
            AddGameObject(splash);
        }

        public override void HandleInput(GameTime gameTime)
        {
            InputManager.GetCommands(cmd =>
            {
                if (cmd is SplashInputCommand.GameSelect)
                {
                    SwitchState(new GameplayState());
                }
            });
        }

        public override void UpdateGameState(GameTime _) { }

        protected override void SetInputManager()
        {
            InputManager = new InputManager(new SplashInputMapper());
        }
    }
}