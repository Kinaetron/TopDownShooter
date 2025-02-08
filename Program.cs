using MoonWorks;
using TopDownShooter;

internal class Program
{
    static void Main()
    {
        var windowCreateInfo = new WindowCreateInfo(
             "Shooter Game",
             640,
             360,
             ScreenMode.Windowed
         );

        var framePacingSettings = FramePacingSettings.CreateLatencyOptimized(60);

        var game = new ShooterGame(
            new AppInfo("Shooter Game", "ShooterGame"),
            windowCreateInfo,
            framePacingSettings,
            true);

        game.Run();
    }
}