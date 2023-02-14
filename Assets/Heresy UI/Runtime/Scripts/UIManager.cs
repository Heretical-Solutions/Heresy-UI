namespace HereticalSolutions.UI.Managers
{
    public class UIManager
    {
        //private HUDManager hudManager;

        //private PopupManager popupManager;

        private WindowManager windowManager;

        public UIManager(
            //HUDManager hudManager,
            //PopupManager popupManager,
            WindowManager windowManager)
        {
            //this.hudManager = hudManager;

            //this.popupManager = popupManager;

            this.windowManager = windowManager;
        }

        public void TearDown()
        {
            //screenManager.TearDown();

            //panelManager.TearDown();

            windowManager.TearDown();
        }
    }
}