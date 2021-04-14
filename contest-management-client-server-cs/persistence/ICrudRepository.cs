using System.Collections.Generic;
using model;

namespace persistence
{
    public interface ICrudRepository<TId, TE> where TE : Entity<TId>
    {
        IEnumerable<TE> FindAll();
        TE? Find(TId id);
        void Add(TE entity);
        void Remove(TId id);
        void Update(TE entity);
    }
}