namespace HereticalSolutions.UI.Events
{
    public class CloseWindowEvent : IUIEvent
    {
        public bool CloseAllWindows { get; private set; }
    }
}
