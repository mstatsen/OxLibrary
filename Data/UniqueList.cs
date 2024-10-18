namespace OxLibrary.Data
{
    public class UniqueList<T> : List<T>
    {
        public new void Add(T item)
        {
            //Remove(item);
            base.Add(item);
        }
    }
}
