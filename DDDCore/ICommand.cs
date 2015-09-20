using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDDCore
{
    public interface ICommand
    {
        TCommand As<TCommand>()
            where TCommand : ICommand;
        Guid CommandId { get; }
    }
}
