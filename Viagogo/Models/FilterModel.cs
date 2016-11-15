using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Viagogo.Models
{
    public class FilterModel
    {
        public List<Viagogo.Models.Event> lst {get; set;}
        public int? noofTickets { get; set; }
        public DateTimeOffset? filterDate { get; set; }
        public DateTimeOffset? filterDateE { get; set; }
    }
}