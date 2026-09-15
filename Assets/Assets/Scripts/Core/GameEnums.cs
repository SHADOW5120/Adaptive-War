namespace Core
{
    public enum EntityType
    {
        Unit,
        Building
    }

    public enum UnitState
    {
        Idle,
        Walk,
        Attack,
        Die
    }

    public enum GameState
    {
        Playing,
        GameOver,
        Victory
    }
}