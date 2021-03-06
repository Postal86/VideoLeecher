﻿using System;

namespace VideoLeecher.core.Attributes
{
    public  class EnumDisplayNameAttribute  : Attribute 
    {
        public EnumDisplayNameAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public string Name { get; private set; }
    }
}
