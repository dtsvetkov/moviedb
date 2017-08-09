using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using MoviesDatabase.DAL;
using MoviesDatabase.DB.Model;
using MoviesDataBaseGUI.Constants;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace MoviesDataBaseGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDAL _dal;

        #region .ctor

        public MainWindowViewModel(IDAL dal)
        {
            _dal = dal;

            var allMovies = _dal.GetMovies();
            Movies.AddRange(allMovies.Select(CreateMovieViewModel));

            SubscribeToPropertiesChanged();
        }

        private MovieViewModel CreateMovieViewModel(Movie model)
        {
            var vm = new MovieViewModel(model);
            vm.Removing += this.OnMovieRemoving;
            return vm;
        }

        private void SubscribeToPropertiesChanged()
        {
            this.ObservableForProperty(x => x.SearchMovieText)
                       .Throttle(ReactiveConstants.TextChangedDelay)
                       .Select(x => x.GetValue())
                       .Subscribe(x =>
                       {
                           var movies = string.IsNullOrWhiteSpace(x) ? _dal.GetMovies() : _dal.GetMovies(x);

                           foreach (var vm in Movies)
                           {
                               vm.Removing -= this.OnMovieRemoving;
                           }

                           Application.Current.Dispatcher.Invoke(() =>
                           {
                               Movies.Clear();
                               Movies.AddRange(movies.Select(CreateMovieViewModel));
                           });
                       });

            Movies.ItemsAdded.Subscribe(x =>
            {
                x.Removing += this.OnMovieRemoving;
            });
            Movies.ItemsRemoved.Subscribe(x =>
            {
                x.Removing -= this.OnMovieRemoving;
            });
        }

        #endregion
        
        #region Properties

        private ReactiveCollection<MovieViewModel> _movies;
        public ReactiveCollection<MovieViewModel> Movies
        {
            get
            {
                return _movies ?? (_movies = new ReactiveCollection<MovieViewModel>());
            }
        }

        private void OnMovieRemoving(object sender, EventArgs e)
        {
            var movieVM = sender as MovieViewModel;
            if (movieVM == null) return;
            
            var notification = new Confirmation { Title = "Удаление фильма", Content = string.Format("Удалить фильм \"{0}\" из базы данных?", movieVM.Name) };

            _movieRemovingConfirmation.Raise(notification, confirmation =>
            {
                if (!confirmation.Confirmed)
                {
                    return;
                }

                var currentIndex = Movies.IndexOf(movieVM);
                _dal.DeleteMovie(movieVM.Movie);
                Movies.Remove(movieVM);

                if (Movies.Any())
                {
                    if (currentIndex >= Movies.Count)
                    {
                        SelectedMovie = Movies.Last();
                    }
                    else
                    {
                        SelectedMovie = Movies[currentIndex];
                    }
                }
                else
                {
                    SelectedMovie = null;
                }
            });
        }

        private MovieViewModel _SelectedMovie;
        public MovieViewModel SelectedMovie
        {
            get { return _SelectedMovie; }
            set { this.RaiseAndSetIfChanged(x => x.SelectedMovie, value); }
        }

        private string _SearchMovieText;
        public string SearchMovieText
        {
            get { return _SearchMovieText; }
            set { this.RaiseAndSetIfChanged(x => x.SearchMovieText, value); }
        }

        #endregion

        #region Commands

        #region AddNewMovie

        private DelegateCommand _addNewMovieCommand;
        public DelegateCommand AddNewMovieCommand
        {
            get
            {
                return _addNewMovieCommand ?? (_addNewMovieCommand = new DelegateCommand(AddNewMovieExecute));
            }
        }

        private void AddNewMovieExecute()
        {
            var movie = new Movie { Name = "Название фильма", Description = "Описание", ReleaseDate = DateTime.Now.Date };
            var movieVM = new MovieViewModel(movie);
            _dal.SaveMovie(movie);
            Movies.Add(movieVM);
            SelectedMovie = movieVM;
        }

        #endregion

        #region SaveChanges

        private DelegateCommand _saveChangesCommand;
        public DelegateCommand SaveChangesCommand
        {
            get
            {
                return _saveChangesCommand ?? (_saveChangesCommand = new DelegateCommand(SaveChangesExecute));
            }
        }

        private void SaveChangesExecute()
        {
            foreach (var movieVM in Movies)
            {
                movieVM.SaveChangesToModel();
                _dal.UpdateMovie(movieVM.Movie);
            }
        }

        #endregion

        #region CancelChanges

        private DelegateCommand _cancelChangesCommand;
        public DelegateCommand CancelChangesCommand
        {
            get
            {
                return _cancelChangesCommand ?? (_cancelChangesCommand = new DelegateCommand(CancelChangesExecute));
            }
        }

        private void CancelChangesExecute()
        {
            foreach (var movieVM in Movies)
            {
                movieVM.LoadFromModel();
            }
        }

        #endregion

        #endregion

        #region InteractionRequest

        private InteractionRequest<Confirmation> _movieRemovingConfirmation;
        public IInteractionRequest MovieRemovingConfirmation
        {
            get
            {
                return _movieRemovingConfirmation ?? (_movieRemovingConfirmation = new InteractionRequest<Confirmation>());
            }
        }

        #endregion
    }
}
