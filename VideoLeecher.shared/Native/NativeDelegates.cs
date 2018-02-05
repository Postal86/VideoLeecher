using System;


namespace VideoLeecher.shared.Native
{
   public static  class NativeDelegates
    {

        public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData);
    }
}
