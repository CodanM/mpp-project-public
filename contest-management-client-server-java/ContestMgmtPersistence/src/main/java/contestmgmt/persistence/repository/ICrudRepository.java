package contestmgmt.persistence.repository;

import contestmgmt.model.Entity;

public interface ICrudRepository<ID, E extends Entity<ID>> {
    Iterable<E> findAll();
    E find(ID id);
    ID add(E entity);
    void remove(ID id);
    void update(E entity);
}
