namespace GS.Lib.Components
{
    internal abstract class SharkComponent
    {
        public SharpShark Library { get; protected set; }

        protected SharkComponent(SharpShark p_Library)
        {
            Library = p_Library;
        }
    }
}
