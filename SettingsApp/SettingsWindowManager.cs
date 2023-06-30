using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoMinder.SettingsApp
{
    public class SettingsWindowManager
    {
        private static SettingsWindowManager instance;
        private int activeWindowsCount;

        private SettingsWindowManager()
        {
            activeWindowsCount = 0;
        }

        public static SettingsWindowManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingsWindowManager();
                }
                return instance;
            }
        }

        public bool CanOpenSettingsWindow()
        {
            return activeWindowsCount < 2;
        }

        public void IncrementWindowCount()
        {
            activeWindowsCount++;
        }

        public void DecrementWindowCount()
        {
            activeWindowsCount--;
        }
    }
}
