using System.Collections.Generic;
using contest_management_gui_cs.model;

namespace contest_management_gui_cs.repository
{
    public interface ICrudRepository<ID, TE> where TE : Entity<ID>
    {
        IEnumerable<TE> FindAll();
        TE Find(ID id);
        void Add(TE entity);
        void Remove(ID id);
        void Update(TE entity);
    }
}