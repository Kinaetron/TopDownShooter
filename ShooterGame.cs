using MoonWorks;
using MoonWorks.Graphics;
using TopDownShooter.GameStates;

namespace TopDownShooter;

public class ShooterGame : Game
{
    private DeathState _deathState;
    private GameplayState _gameplayState;

    private GameState _currentState;

    public ShooterGame(
        AppInfo appInfo,
        WindowCreateInfo windowCreateInfo,
        FramePacingSettings framePacingSettings,
        bool debugMode = false
        ) : base(
            appInfo,
            windowCreateInfo,
            framePacingSettings,
            ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.MSL | ShaderFormat.DXBC,
            debugMode)
    {
        ShaderCross.Initialize();

        _deathState = new DeathState(this, _gameplayState);
        _gameplayState = new GameplayState(this, _deathState);

        _deathState.SetTransitionState(_gameplayState);

        _currentState = _gameplayState;
        _currentState.Start();
    }

    protected override void Update(TimeSpan delta)
    {
        _currentState.Update(delta);
    }

    protected override void Draw(double alpha)
    {
        _currentState.Draw(alpha);
    }

    public void SetState(GameState gameState)
    {
        if(_currentState != null)
        {
            _currentState.End();
        }


        gameState.Start();
        _currentState = gameState;
    }
}
