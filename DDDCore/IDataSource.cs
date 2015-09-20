using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDCore
{
    public interface IDataSource<TDto>
        where TDto : IDto
    {
    }
}
