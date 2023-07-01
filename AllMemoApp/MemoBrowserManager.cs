﻿using MemoMinder.SettingsApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoMinder
{
    internal class MemoBrowserManager
    {
        private static MemoBrowserManager? instance;
        private int activeWindowsCount;

        private MemoBrowserManager()
        {
            activeWindowsCount = 0;
        }

        public static MemoBrowserManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MemoBrowserManager();
                }
                return instance;
            }
        }

        public bool CanOpenAllMemoWindow()
        {
            return activeWindowsCount < 1;
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
