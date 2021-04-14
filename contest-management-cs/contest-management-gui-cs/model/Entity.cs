namespace contest_management_gui_cs.model
{
    public class Entity<T>
    {
        public T Id { get; set; }

        public override string ToString()
        {
            string str = $"{GetType().Name} {{ ";
            foreach (var prop in GetType().GetProperties())
                str += $"{prop.Name}: {prop.GetValue(this)}, ";
            str = str.Remove(str.Length - 2, 1);
            str += "}";
            return str;
        }
    }
}