using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static VideoLeecher.shared.Native.NativeDelegates;
using static VideoLeecher.shared.Native.NativeDisplay;
using static VideoLeecher.shared.Native.NativeStructs;



namespace VideoLeecher.shared.Native
{
    public class NativeDisplay
    {
        #region ველები 

        private System.Windows.Rect _bounds;
        private IntPtr _handle;
        private bool _isPrimary;
        private string _name;
        private System.Windows.Rect _workingArea;

        #endregion ველები


        #region კონსტრუქტორი

        private NativeDisplay(IntPtr hMonitor, IntPtr hdc)
        {
            MonitorInfoEx info = new MonitorInfoEx();
            GetMonitorInfoNative(new HandleRef(null, hMonitor), info);

            _isPrimary = ((info.dwFlags & NativeFlags.MonitorinfofPrimary) != 0);
            _name = new string(info.szDevice).TrimEnd((char)0);
            _handle = hMonitor;

            _bounds = new System.Windows.Rect(
                info.rcMonitor.Left, info.rcMonitor.Top, 
                info.rcMonitor.Right - info.rcMonitor.Left, 
                info.rcMonitor.Bottom - info.rcMonitor.Top);

            _workingArea = new System.Windows.Rect(
                info.rcWork.Left, info.rcWork.Top, 
                info.rcWork.Right - info.rcWork.Left, 
                info.rcWork.Bottom - info.rcWork.Top);
        }



        #endregion კონსტრუქტორი

        #region თვისებები

        public  System.Windows.Rect Bounds
        {
            get
            {
                return _bounds;
            }
        }

        public IntPtr Handle
        {
           get
            {
                return _handle;
            }
        }

        public bool  IsPrimary
        {
            get
            {
                return _isPrimary;
            }

        }

        public string Name
        {
            get
            {
                return _name;
            }

        }

        public  System.Windows.Rect WorkingArea
        {
            get
            {
                return _workingArea;
            }
        }


        #endregion თვისებები


        #region მეთოდები

        public static ICollection<NativeDisplay> GetAllDisplay()
        {

            DisplayEnumCallback closure = new DisplayEnumCallback();
            MonitorEnumProc proc = new MonitorEnumProc(closure.Callback);
            EnumDisplayMonitorsNative(new HandleRef(null, IntPtr.Zero), IntPtr.Zero, proc, IntPtr.Zero);
            return closure.Display.Cast<NativeDisplay>();

        }

        public static NativeDisplay GetDisplayFromWindow(IntPtr handle)
        {
            IntPtr hMonitor = MonitorFromWindowNative(handle, NativeFlags.MONITOR_DEFAULTTONEAREST);

            return GetAllDisplay().Where(d => d.Handle == hMonitor).FirstOrDefault();
        }


        #endregion მეთოდები

        #region კლასები

        private class DisplayEnumCallback
        {
            #region კონსტრუქტორები

            public DisplayEnumCallback()
            {
                Displays = new ArrayList();
            }

            #endregion კონსტრუქტორები


            #region თვისებები

            public ArrayList Displays { get; private set; }

            #endregion თვისებები


            #region მეთოდები

            public bool Callback(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData)
            {
                Displays.Add(new NativeDisplay(hMonitor, hdcMonitor));
                return true;
            }

            #endregion მეთოდები


        }

          #endregion კლასები
    }
}
