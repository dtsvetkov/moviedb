using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Win32;
using MoviesDatabase.DB.Model;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive.Linq;

namespace MoviesDataBaseGUI.ViewModels
{
    public class MovieViewModel : ViewModelBase
    {
        private readonly Movie _movie;

        #region .ctor

        public MovieViewModel(Movie movie)
        {
            _movie = movie;

            SubscribeToPropertiesChanged();

            LoadFromModel();
        }

        private void SubscribeToPropertiesChanged()
        {
            this.ObservableForProperty(x => x.ImageFileData)
                       .Subscribe(_ =>
                       {
                           DeleteImageCommand.RaiseCanExecuteChanged();
                       });
        }

        #endregion

        #region Properties

        public Movie Movie { get { return _movie; } }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(x => x.Name, value); }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(x => x.Description, value); }
        }

        private DateTime _ReleaseDate;
        public DateTime ReleaseDate
        {
            get { return _ReleaseDate; }
            set { this.RaiseAndSetIfChanged(x => x.ReleaseDate, value); }
        }

        private Genres _Genre;
        public Genres Genre
        {
            get { return _Genre; }
            set { this.RaiseAndSetIfChanged(x => x.Genre, value); }
        }

        private AgeRestrictions _AgeRestriction;
        public AgeRestrictions AgeRestriction
        {
            get { return _AgeRestriction; }
            set { this.RaiseAndSetIfChanged(x => x.AgeRestriction, value); }
        }

        private byte[] _ImageFileData;
        public byte[] ImageFileData
        {
            get { return _ImageFileData; }
            set { this.RaiseAndSetIfChanged(x => x.ImageFileData, value); }
        }

        #endregion

        #region Commands

        #region DeleteMovie

        private DelegateCommand _deleteMovieCommand;
        public DelegateCommand DeleteMovieCommand
        {
            get
            {
                return _deleteMovieCommand ?? (_deleteMovieCommand = new DelegateCommand(DeleteMovieExecute));
            }
        }

        private void DeleteMovieExecute()
        {
            Removing?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Removing;

        #endregion

        #region ChangeImageCommand

        private DelegateCommand _changeImageCommand;
        public DelegateCommand ChangeImageCommand
        {
            get
            {
                return _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageExecute));
            }
        }

        private void ChangeImageExecute()
        {
            var openDlg = new OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "Файлы изображений (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            if (openDlg.ShowDialog() ?? false)
            {
                ImageFileData = File.ReadAllBytes(openDlg.FileName);
            }
        }

        #endregion

        #region DeleteImageCommand

        private DelegateCommand _deleteImageCommand;
        public DelegateCommand DeleteImageCommand
        {
            get
            {
                return _deleteImageCommand ?? (_deleteImageCommand = new DelegateCommand(DeleteImageExecute, DeleteImageCanExecute));
            }
        }

        private void DeleteImageExecute()
        {
            var notification = new Confirmation { Title = "Удаление Обложки", Content = string.Format("Удалить обложку фильма \"{0}\"?", Name) };

            _imageRemovingConfirmation.Raise(notification, confirmation =>
            {
                if (!confirmation.Confirmed)
                {
                    return;
                }

                ImageFileData = null;
            });
        }

        private bool DeleteImageCanExecute()
        {
            return ImageFileData != null;
        }

        #endregion

        #endregion

        #region InteractionRequest

        private InteractionRequest<Confirmation> _imageRemovingConfirmation;
        public IInteractionRequest ImageRemovingConfirmation
        {
            get
            {
                return _imageRemovingConfirmation ?? (_imageRemovingConfirmation = new InteractionRequest<Confirmation>());
            }
        }

        #endregion

        public void SaveChangesToModel()
        {
            _movie.Name = Name;
            _movie.Description = Description;
            _movie.ReleaseDate = ReleaseDate;
            _movie.AgeRestriction = AgeRestriction;
            _movie.Genre = Genre;
            _movie.ImageFileData = ImageFileData;
        }

        public void LoadFromModel()
        {
            Name = _movie.Name;
            Description = _movie.Description;
            ReleaseDate = _movie.ReleaseDate;
            AgeRestriction = _movie.AgeRestriction;
            Genre = _movie.Genre;
            ImageFileData = _movie.ImageFileData;
        }
    }
}
