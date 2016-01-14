using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FootballScores.Controllers
{
    public class GetPageController : ApiController
    {
        // GET api/livescores
        public string Get()
        {
            Task<string> fullPageTask = new HttpClient().GetStringAsync("http://www.bbc.co.uk/sport/football/live-scores");

            return fullPageTask.Result;
        }
    }
}
