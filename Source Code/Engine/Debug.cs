namespace Engine2D
{
    public class Debug
    {

        public bool IsDebugMode { get; private set; }
        public static Debug Instance { get; internal set; }

        internal Debug(bool isDebugMode) 
        {
            IsDebugMode = isDebugMode;
        }

        public void Log(string message)
        {
            if (IsDebugMode)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
        }
    }
}
