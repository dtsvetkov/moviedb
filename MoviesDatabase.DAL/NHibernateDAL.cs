using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesDatabase.DB.Model;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Collections.ObjectModel;

namespace MoviesDatabase.DAL
{
    public class NHibernateDAL : IDAL
    {
        private readonly ISessionFactory _sessionFactory;

        public NHibernateDAL(string configFile)
        {
            var cfg = new NHibernate.Cfg.Configuration().Configure(configFile);

            new SchemaUpdate(cfg).Execute(false, true);

            _sessionFactory = cfg.BuildSessionFactory();
        }

        public void DeleteMovie(Movie movie)
        {
            Delete(movie);
        }

        public IList<Movie> GetMovies()
        {
            var movies = GetAll<Movie>().ToList();
            return new ReadOnlyCollection<Movie>(movies);
        }

        public IList<Movie> GetMovies(string key)
        {
            var movies = GetAll<Movie>().Where(m => m.Name.Contains(key) || m.Description.Contains(key)).ToList();
            return new ReadOnlyCollection<Movie>(movies);
        }

        public void SaveMovie(Movie movie)
        {
            Save(movie);
        }

        public void UpdateMovie(Movie movie)
        {
            Update(movie);
        }

        private IEnumerable<T> GetAll<T>() where T : class
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var result = session.QueryOver<T>().List();

                return result;
            }
        }

        private void Save<T>(T obj)
        {
            InTransaction<T>(session => session.Save(obj));
        }

        private void Update<T>(T obj)
        {
            InTransaction<T>(session => session.Update(obj));
        }

        private void Delete<T>(T obj)
        {
            InTransaction<T>(session => session.Delete(obj));
        }

        private void InTransaction<T>(Action<ISession> action)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    action(session);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
