using Avalonia.Input;

namespace ScreenSearch.UI.HotKey;

public sealed class GlobalHotKeyListener
{
    private const Key HotKey = Key.M;
    private const KeyModifiers RequiredModifiers = KeyModifiers.Meta | KeyModifiers.Shift;

    public event Action? Triggered;

    public bool TryHandle(Key key, KeyModifiers modifiers)
    {
        var normalized = modifiers & (KeyModifiers.Meta | KeyModifiers.Shift);
        if (key != HotKey || normalized != RequiredModifiers)
        {
            return false;
        }

        Triggered?.Invoke();
        return true;
    }

    public void SimulateHotKeyPress()
    {
        Triggered?.Invoke();
    }
}
