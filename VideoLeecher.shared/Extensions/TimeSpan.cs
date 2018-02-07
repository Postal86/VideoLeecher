﻿using System;


namespace VideoLeecher.shared.Extensions
{
   public static  class TimeSpanExtensions
    {
        public static string ToDaylesString(this TimeSpan  value)
        {
            return string.Format("{0}:{1}:{2}", ((value.Days * 24) + value.Hours).ToString("00"), value.Minutes.ToString("00"),  value.Seconds.ToString("00"));
        }

        public static int GetDaysInHours(this TimeSpan value)
        {
            return (value.Days * 24) + value.Hours;
        }
    }
}