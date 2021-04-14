package model;

public class Entity<T> {
    private T id;

    public T getId() {
        return id;
    }

    public void setId(T id) {
        this.id = id;
    }

    @Override
    public String toString() {
        StringBuilder str = new StringBuilder(getClass().getSimpleName() + ": { ");
        for (var field : getClass().getDeclaredFields()) {
            field.setAccessible(true);
            try {
                str.append(field.getName()).append(": ").append(field.get(this)).append(", ");
            } catch (IllegalAccessException e) {
                e.printStackTrace();
            }
            field.setAccessible(false);
        }
        str.append("}");
        str.replace(str.length() - 3, str.length() - 2, "");
        return str.toString();
    }
}
