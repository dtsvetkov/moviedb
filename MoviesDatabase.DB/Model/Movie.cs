using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesDatabase.DB.Model
{
    public enum AgeRestrictions
    {
        [Description("0+")]
        Age0,
        [Description("6+")]
        Age6,
        [Description("12+")]
        Age12,
        [Description("16+")]
        Age16,
        [Description("18+")]
        Age18,
    }

    public enum Genres
    {
        [Description("Комедия")]
        Comedy,
        [Description("Боевик")]
        Action,
        [Description("Драма")]
        Drama,
    }

    public class Movie
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual Genres Genre { get; set; }
        public virtual DateTime ReleaseDate { get; set; }
        public virtual AgeRestrictions AgeRestriction { get; set; }
        public virtual byte[] ImageFileData { get; set; }
    }
}
