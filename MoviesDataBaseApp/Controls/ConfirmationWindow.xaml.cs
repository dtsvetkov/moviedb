using System.Windows;

namespace MoviesDataBaseGUI.Controls
{
    /// <summary>
    /// Interaction logic for ConfirmationWindow.xaml
    /// </summary>
    public partial class ConfirmationWindow
    {
        public static readonly DependencyProperty ConfirmationTemplateProperty =
            DependencyProperty.Register(
                "ConfirmationTemplate", typeof(DataTemplate), typeof(ConfirmationWindow), new PropertyMetadata(null));

        public ConfirmationWindow()
        {
            InitializeComponent();
            OKButton.Content = "Да";
            CancelButton.Content = "Нет";
        }

        public DataTemplate ConfirmationTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ConfirmationTemplateProperty);
            }
            set
            {
                SetValue(ConfirmationTemplateProperty, value);
            }
        }
    }
}
