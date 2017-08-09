using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using MoviesDataBaseGUI.Controls;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace MoviesDataBaseGUI.Interactions
{
    public class PopupModalWindowAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
            "ContentTemplate", typeof(DataTemplate), typeof(PopupModalWindowAction), new PropertyMetadata(null));

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            if (args == null)
            {
                return;
            }

            var childWindow = GetChildWindow(args.Context);

            var callback = args.Callback;
            EventHandler handler = null;
            handler = (o, e) =>
            {
                childWindow.Closed -= handler;
                callback();
            };
            childWindow.Closed += handler;

            childWindow.ShowDialog();
        }

        protected Window GetChildWindow(INotification notification)
        {
            var childWindow = CreateWindow(notification);
            childWindow.DataContext = notification;
            childWindow.Owner = Application.Current.MainWindow;

            return childWindow;
        }

        private Window CreateWindow(INotification notification)
        {
            return new ConfirmationWindow { ConfirmationTemplate = ContentTemplate };
            /*return notification is Confirmation
                ? new ConfirmationWindow { ConfirmationTemplate = ContentTemplate }
                : (Window)new NotificationWindow { NotificationTemplate = ContentTemplate };*/
        }
    }
}
