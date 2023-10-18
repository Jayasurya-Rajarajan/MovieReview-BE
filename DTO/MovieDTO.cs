using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string MoviePoster { get; set; }
        public Nullable<int> MovieRating { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
    }
}
