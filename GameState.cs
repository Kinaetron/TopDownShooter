namespace TopDownShooter;

public abstract class GameState
{
    public abstract void Start();
    public abstract void Update(TimeSpan delta);
    public abstract void Draw(double alpha);
    public abstract void End();
}
