using MoviesDatabase.DAL;
using MoviesDataBaseGUI.ViewModels;
using MoviesDataBaseGUI.Views;
using System.Windows;

namespace MoviesDataBaseApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var dal = new NHibernateDAL("NHibernate.cfg.xml");

            var mainViewModel = new MainWindowViewModel(dal);

            var mainView = new MainWindow();
            mainView.DataContext = mainViewModel;

            Current.MainWindow = mainView;
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            //AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

            mainView.Show();
        }        
    }
}
