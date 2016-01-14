using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FootballScores.Models
{
    public class Match
    {
        public string HomeTeam { get; private set; }
        public string AwayTeam { get; private set; }
        public int HomeScore { get; private set; }
        public int AwayScore { get; private set; }
        public int ElapsedMinutes { get; private set; }
        public string Status { get; private set; }

        public Match(string bbcPage, MatchStatus status)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(bbcPage);

            HomeTeam = doc.DocumentNode.SelectNodes("//span[@class='team-home']").Single().InnerText;
            AwayTeam = doc.DocumentNode.SelectNodes("//span[@class='team-away']").Single().InnerText;
            Status = Enum.GetName(typeof(MatchStatus), status);

            var scoreString = doc.DocumentNode.SelectNodes("//span[@class='score']").Single().InnerText.Trim();
            SetScores(scoreString, status);

            var elapsedTime = doc.DocumentNode.SelectNodes("//span[@class='elapsed-time']").Single().InnerText.Trim();
            ElapsedMinutes = GetElapsedMinutes(elapsedTime, status);
        }

        private void SetScores(string scoreString, MatchStatus status)
        {
            if (status != MatchStatus.Fixture)
            {
                var match = Regex.Match(scoreString, @"^(\d+)\s*\-\s*(\d+)$");
                if (match.Success)
                {
                    HomeScore = int.Parse(match.Groups[1].Value);
                    AwayScore = int.Parse(match.Groups[2].Value);
                }
            }
        }

        private int GetElapsedMinutes(string timeString, MatchStatus status)
        {
            switch (status)
            {
                case MatchStatus.Live:
                    var match = Regex.Match(timeString, @"^(\d+)\s*mins$");
                    if (match.Success)
                        return int.Parse(match.Groups[1].Value);
                    else
                        return 45;
                case MatchStatus.Report:
                    return 90;
                case MatchStatus.Fixture:
                    return 0;
            }
            return 0;
        }
    }
}