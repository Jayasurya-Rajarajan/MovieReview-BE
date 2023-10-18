using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public Nullable<int> MovieId { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
