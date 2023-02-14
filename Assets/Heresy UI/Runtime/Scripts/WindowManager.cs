using System;
using System.Collections.Generic;
using HereticalSolutions.UI.Events;

namespace HereticalSolutions.UI.Managers
{
    public class WindowManager
    {
        private IRepository<Type, IWindow> windows;

        private Stack<IUIEvent> eventStack;

        private MessageBus uiBus;

        private IWindow currentOpenWindow;

        private bool pushInProgress = false;
        private bool popInProgress = false;

        public WindowManager(
            Dictionary<Type, IWindow> windows,
            Stack<IUIEvent> eventStack,
            MessagetBus uiBus)
        {
            this.windows = windows;
            this.eventStack = eventStack;
            this.uiBus = uiBus;

            currentOpenWindow = null;

            uiBus
                .SubscribeTo<OpenWindowEvent>(
                    @event => HandleOpenWindowEvent(@event, @event.GetType()))
                .AddTo(disposables);

            uiBus
                .SubscribeTo<CloseWindowEvent>(
                    @event => HandleCloseWindowEvent(@event))
                .AddTo(disposables);
        }

        public void TearDown()
        {
            if (disposables != null)
                disposables.Dispose();

            foreach (var window in windows.Values)
                window.TearDown();
        }

        private void HandleOpenWindowEvent(IUIEvent @event, Type windowType)
        {
            IWindow nextCurrentOpenWindow = null;

            bool windowObtained = windows.TryGetValue(windowType, out nextCurrentOpenWindow);

            if (!windowObtained)
                throw new Exception(string.Format("[WindowManager] NO WINDOW OF TYPE \"{0}\" FOUND", windowType.ToString()));

            if (!popInProgress)
            {
                if (currentOpenWindow != null)
                {
                    pushInProgress = true;

                    if (nextCurrentOpenWindow != currentOpenWindow)
                        currentOpenWindow.Hide();

                    pushInProgress = false;
                }

                eventStack.Push(@event);
            }

            currentOpenWindow = nextCurrentOpenWindow;

            currentOpenWindow.Show();
        }

        private void HandleCloseWindowEvent(CloseWindowEvent @event)
        {
            if (currentOpenWindow == null)
                return;

            currentOpenWindow.Hide();

            if (pushInProgress)
                return;

            if (@event.CloseAllWindows)
                eventStack.Clear();

            if (eventStack.Count == 0)
                return;

            eventStack.Pop();

            currentOpenWindow = null;

            if (eventStack.Count != 0)
            {
                var poppedEvent = eventStack.Peek();

                popInProgress = true;

                if (poppedEvent is OpenWindowEvent)
                {
                    uiBus
                        .PopMessage<OpenWindowEvent>(out var message) //TODO: fix, this generic will produce wrong type of message
                        .Write(message, new HUDShowEventArguments()
                        {
                        })
                        .SendImmediately(message);
                }

                popInProgress = false;
            }
        }
    }
}