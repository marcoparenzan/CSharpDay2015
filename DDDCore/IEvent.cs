using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDDCore
{
    public interface IEvent
    {
        TEvent As<TEvent>()
            where TEvent : IEvent;
        Guid EventId { get; }
    }
}
