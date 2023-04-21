using SNShien.Common.ProcessTools;

public class SwitchSceneEvent : IArchitectureEvent
{
    public string RepositionActionKey { get; }

    public SwitchSceneEvent(string repositionActionKey)
    {
        RepositionActionKey = repositionActionKey;
    }
}