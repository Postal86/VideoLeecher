using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static VideoLeecher.shared.Native.NativeDelegates;
using static VideoLeecher.shared.Native.NativeStructs;

namespace VideoLeecher.shared.Native
{
    public static class NativeMethods
    {
        [DllImport("user32.dll", ExactSpelling = true)]
        [ResourceExposure(ResourceScope.None)]
        private static extern bool EnumDisplayMonitors(HandleRef hdc, IntPtr rcClip, MonitorEnumProc lpfnEum, IntPtr dwData);

        public static bool EnumDisplayMonitorsNative(HandleRef hdc, IntPtr rcClip, MonitorEnumProc lpfnEnum, IntPtr dwData)
        {
            return EnumDisplayMonitors(hdc, rcClip, lpfnEnum, dwData);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        private static extern bool GetMonitorInfo(HandleRef hMonitor, [In, Out]MonitorInfoEx lpmi);

        public static bool GetMonitorInfoNative(HandleRef  hMonitor, [In, Out]MonitorInfoEx)

    }
}
