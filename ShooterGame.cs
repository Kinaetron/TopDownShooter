using MoonWorks;
using MoonWorks.Graphics;
using TopDownShooter.GameStates;

namespace TopDownShooter;

public class ShooterGame : Game
{
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

        _currentState = new GameplayState(this);
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
}
