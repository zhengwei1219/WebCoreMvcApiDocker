using System;
using System.Collections.Generic;
using System.Text;

namespace Zhengwei.Project.Domain.SeedWork
{
   public class IRepository<T> where T:IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
