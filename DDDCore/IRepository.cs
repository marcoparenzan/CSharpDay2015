using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDCore
{
    public interface IRepository<TAggregateRoot, TId>
        where TAggregateRoot: IAggregateRoot<TId>
    {
        Task<TAggregateRoot> GetByIdAsync(TId id);
        Task SetAsync(TAggregateRoot aggregateRoot);
    }
}
