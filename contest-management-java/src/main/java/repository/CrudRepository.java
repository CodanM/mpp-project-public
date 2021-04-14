package repository;

import model.Entity;

public interface CrudRepository<ID, E extends Entity<ID>> {
    Iterable<E> findAll();
    E find(ID id);
    ID add(E entity);
    void remove(ID id);
    void update(E entity);
}
