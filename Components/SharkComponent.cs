namespace GS.Lib.Components
{
    public abstract class SharkComponent
    {
        internal SharpShark Library { get; private set; }

        internal SharkComponent(SharpShark p_Library)
        {
            Library = p_Library;
        }
    }
}
