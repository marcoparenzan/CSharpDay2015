using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDCore
{
    public interface IEnvelope
    {
        string Username { get; }
        DateTime Timestamp { get; }
    }
}
