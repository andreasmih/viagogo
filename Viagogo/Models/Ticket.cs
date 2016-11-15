using GogoKit.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Viagogo.Models
{
    public class Ticket
    {
        public Money price { get; set; }
        public TicketType description { get; set; }
        public int noOfTickets { get; set; }
    }
}