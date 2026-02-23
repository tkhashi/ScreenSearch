namespace ScreenSearch.UI.HotKey;

public sealed class GlobalHotKeyListener
{
    public event Action? Triggered;

    public void SimulateHotKeyPress()
    {
        Triggered?.Invoke();
    }
}
