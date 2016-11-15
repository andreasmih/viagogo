using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GogoKit;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Viagogo.Models;
using GogoKit.Models.Response;

namespace Viagogo.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(int? id)
        {
            Session["found"] = " ";
            if (id == 1)
            {
                Session["found"] = "No events found for your query :( !";
            }
            return View();
        }
        private string conv(Money money)
        {
            if (money != null)
                return money.Display;
            else
                return "N/A";
        }

        public ActionResult Lookup()
        {
            return RedirectToAction("Index");
        }
        public DateTime transform(DateTimeOffset? date)
        {
            DateTime d = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, date.Value.Hour, date.Value.Minute, date.Value.Second);
            return d;
        }
        public bool dateisin(DateTimeOffset? ed, DateTimeOffset? ld, DateTimeOffset? eventd)
        {
            if (DateTime.Compare(transform(ed), transform(eventd)) < 0 && DateTime.Compare(transform(eventd), transform(ld)) < 0)
                return true;
            else
                return false;
        }

        [HttpPost]
        public ActionResult Search(FilterModel model)
        {
            List<Models.Event> filteredlist = (List<Models.Event>)Session["Events"]; 
            if(model.noofTickets != null)
            {
                var helper = new List<Models.Event>();
                foreach (var ev in filteredlist)
                {
                    if (ev.noofTickets >= model.noofTickets) 
                       helper.Add(ev);
                }
                filteredlist = helper;
            }
            if (model.filterDate != null && model.filterDateE != null)
            {
                var helper = new List<Models.Event>();
                foreach (var ev in filteredlist)
                {
                    if (dateisin(model.filterDate, model.filterDateE, ev.eventDate))
                        helper.Add(ev);
                }
                filteredlist = helper;
            }
            FilterModel filterobject = new FilterModel();
            filterobject.lst = filteredlist;

            return View("Lookup",filterobject);
        }

        [HttpPost]
        public async Task<ActionResult> Lookup(SearchString search)
        {
            
            search.searchStr = search.searchStr == null ? "EMPTY SEARCH" : search.searchStr;
            if (search.searchStr == "EMPTY SEARCH")
                return RedirectToAction("Index", new { id = 1 });

            var existsEvent = false;
            var client = new ViagogoClient(new ProductHeaderValue("AwesomeApp", "1.0"), "TaRJnBcw1ZvYOXENCtj5",
                           "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV");
            var token = await client.OAuth2.GetClientAccessTokenAsync(/*List of scopes*/ new string[] { });
            await client.TokenStore.SetTokenAsync(token);
            var searchResults = await client.Search.GetAllAsync(search.searchStr);

            List<Models.Event> eventcollection = new List<Models.Event>();

            foreach (var res in searchResults)
            {
                if (res.Type == "Event")
                {
                    var currEvent = await client.Hypermedia.GetAsync<GogoKit.Models.Response.Event>(res.EventLink);
                    eventcollection.Add(new Models.Event()
                    {
                        eventName = res.Title,
                        minticketprice = conv(currEvent.MinTicketPrice),
                        eventDate = currEvent.StartDate,
                        noofTickets = currEvent.NumberOfTickets,
                        id = currEvent.Id
                    });
                    existsEvent = true;
                }
            }

            Session["events"] = eventcollection;
            if (existsEvent)
            {
                FilterModel filterobject = new FilterModel();
                filterobject.lst = eventcollection;
                return View(filterobject);
            }
            else
                return RedirectToAction("Index", new { id = 1 });
            
        }
    }
}