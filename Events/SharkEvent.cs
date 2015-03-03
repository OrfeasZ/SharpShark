namespace GS.Lib.Events
{
    public abstract class SharkEvent : SharkObject
    {
        public T As<T>()
            where T : SharkEvent
        {
            return (T) this;
        }
    }
}
