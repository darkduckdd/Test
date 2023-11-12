using IDosGames;
using System;

public class SidePanelTimer : Timer
{
    protected override void OnEnable()
    {
        base.OnEnable();
        TimerStopped += CLosePanel;
    }

    private void OnDisable()
    {
        TimerStopped -= CLosePanel;
    }
    public void UpdateTimer(float second)
    {
        DateTime endDate = DateTime.UtcNow;

        endDate = endDate.AddSeconds(second);

        Set(endDate);
    }

    private void CLosePanel()
    {
        SideAdvertising.ClosePanel?.Invoke();
    }
}
