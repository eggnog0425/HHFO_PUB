﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HHFO.Models
{
    public interface IAuth
    {
        public bool HasAuthenticated { get; protected set; }
    }
}