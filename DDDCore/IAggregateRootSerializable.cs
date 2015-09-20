using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DDDCore
{
    public interface IAggregateRootSerializable
    {
        void SerializeTo(Stream stream);
        void DeserializeFrom(Stream stream);
    }
}
