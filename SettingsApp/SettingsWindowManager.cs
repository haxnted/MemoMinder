namespace MemoMinder
{
    public class SettingsWindowManager
    {
        private static SettingsWindowManager? instance;
        private int activeWindowsCount;

        private SettingsWindowManager() => activeWindowsCount = 0;

        public static SettingsWindowManager Instance
        {
            get
            {
                if (instance == null) instance = new SettingsWindowManager();
                
                return instance;
            }
        }
        public bool CanOpenSettingsWindow() { return activeWindowsCount < 1; }
        public void IncrementWindowCount() => activeWindowsCount++;
        public void DecrementWindowCount() => activeWindowsCount--;
        
    }
}
