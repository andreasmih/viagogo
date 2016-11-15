using GogoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace Viagogo.Controllers
{
    public class TicketController : Controller
    {
        public ActionResult Index()
        {
            return Content("MAMMA");
        }

        [HttpGet]
        public ActionResult Buy()
        {
            return RedirectToAction("Home","Index");
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Buy(string id)
        {
            var client = new ViagogoClient(new ProductHeaderValue("AwesomeApp", "1.0"), "TaRJnBcw1ZvYOXENCtj5",
                           "ixGDUqRA5coOHf3FQysjd704BPptwbk6zZreELW2aCYSmIT8XJ9ngvN1MuKV");
            var token = await client.OAuth2.GetClientAccessTokenAsync(/*List of scopes*/ new string[] { });
            await client.TokenStore.SetTokenAsync(token);

            int idasint;
            Int32.TryParse(id, out idasint);
            var listings = await client.Listings.GetAllByEventAsync(idasint);
            var listcollection = new List<Models.Ticket>();

            foreach (var res in listings)
            {
                 listcollection.Add(new Models.Ticket()
                 {
                     price = res.EstimatedTotalTicketPrice,
                     description = res.TicketType,
                     noOfTickets = (int)res.NumberOfTickets
                 });
                 
            }

            return View(listcollection);
        }
    }
}