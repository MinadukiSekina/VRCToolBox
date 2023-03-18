﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface ITweetRelatedPhotoViewModel
    {
        public string FullName { get; }

        public ReactivePropertySlim<int> Order { get; }
    }
}