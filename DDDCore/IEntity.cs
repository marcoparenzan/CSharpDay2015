using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDDCore
{
    public interface IEntity<TKey>
    {
        TKey Key { get; }
    }
}
