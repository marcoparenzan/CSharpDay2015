﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDDCore
{
    public interface IAggregateRoot<TId>
    {
        TId Id { get; }
    }
}
