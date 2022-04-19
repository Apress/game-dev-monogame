namespace Engine2D.States
{
    public class BaseGameStateEvent 
    {
        public class Nothing : BaseGameStateEvent { }
        public class GameQuit : BaseGameStateEvent { }
        public class GameTick : BaseGameStateEvent { }
    }
}
