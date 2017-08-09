using MoviesDatabase.DB.Model;
using System;
using System.Collections.Generic;

namespace MoviesDatabase.DAL
{
    public interface IDAL
    {
        IList<Movie> GetMovies();
        IList<Movie> GetMovies(string key);
        void SaveMovie(Movie movie);
        void UpdateMovie(Movie movie);
        void DeleteMovie(Movie movie);
    }
}
