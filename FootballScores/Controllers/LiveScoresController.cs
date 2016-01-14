using FootballScores.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FootballScores.Controllers
{
    public class LiveScoresController : ApiController
    {
        // GET api/livescores
        public IEnumerable<Match> Get()
        {
            string bbcSportLiveScores = ConfigurationManager.AppSettings["bbcSportLiveScores"];
            Task<string> fullPageTask = new HttpClient().GetStringAsync(bbcSportLiveScores);

            var matches = new List<Match>();
            var doc = new HtmlDocument();

            var fullPage = fullPageTask.Result;
            doc.LoadHtml(fullPage);

            matches.AddRange(GetMatches(doc, MatchStatus.Fixture));
            matches.AddRange(GetMatches(doc, MatchStatus.Live));
            matches.AddRange(GetMatches(doc, MatchStatus.Report));

            return matches;
        }

        private IEnumerable<Match> GetMatches(HtmlDocument doc, MatchStatus status)
        {
            string statusString = GetStatusString(status);
            var searchString = string.Format("//tr[@class='{0}']", statusString);

            HtmlNodeCollection matchRows = doc.DocumentNode.SelectNodes(searchString);
            if (matchRows != null)
            {
                foreach (HtmlNode matchRow in matchRows)
                {
                    yield return new Match(matchRow.OuterHtml, status);
                }
            }
        }

        private string GetStatusString(MatchStatus status)
        {
            switch (status)
            {
                case MatchStatus.Fixture:
                    return "fixture";
                case MatchStatus.Live:
                    return "live";
                case MatchStatus.Report:
                    return "report";
            }
            return String.Empty;
        }
    }
}
