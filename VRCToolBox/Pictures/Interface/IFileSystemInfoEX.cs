﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IFileSystemInfoEX
    {
        public ReactiveProperty<string> Name { get; }

        public string FullName { get; }

        public DateTime CreationTime { get; }
    }
}