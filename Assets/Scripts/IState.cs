namespace SGS
{
    public interface IState
    {
        // Tick must be called once every frame
        public void Tick();
    }
}