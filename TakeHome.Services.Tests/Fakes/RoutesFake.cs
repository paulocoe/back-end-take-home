using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Models;

namespace TakeHome.Services.Tests.Fakes
{
    public class RoutesFake
    {
        public static Dictionary<string, List<string>> RoutesInRange()
        {
            return new Dictionary<string, List<string>>
            {
                { "YYZ", new List<string> { "JFK" } },
                { "JFK", new List<string> { "YYZ", "LAX", "RKV", "YBG"} },
                { "LAX", new List<string> { "YVR", "JFK" } },
                { "YVR", new List<string> { "LAX" }},
                { "RKV", new List<string> { "YVR", "JFK", "YBR" }},
                { "YAM", new List<string> { "YVR", "RKV", "YDL" }},
                { "YBG", new List<string> { "YVR", "LAX", "JFK", "YFR" }},
                { "YFR", new List<string> { "YJT"}},
                { "GRU", new List<string> { "ALT"}},
                { "ALT", new List<string> { "GRU"} }
            };
        }

        public static IEnumerable<Route> RoutesA()
        {
            return new List<Route>
            {
                new Route{ Origin = "JFK", Destination = "YYZ" },
                new Route{ Origin = "JFK", Destination = "LAX" },
                new Route{ Origin = "JFK", Destination = "RKV" },
                new Route{ Origin = "JFK", Destination = "YBG" }                
            };
        }

        public static IEnumerable<Route> RoutesB()
        {
            return new List<Route>
            {
                new Route{ Origin = "JFK", Destination = "YYZ" },
                new Route{ Origin = "JFK", Destination = "LAX" },
                new Route{ Origin = "JFK", Destination = "RKV" },
                new Route{ Origin = "JFK", Destination = "YBG" }
            };
        }

        public static IEnumerable<Route> RoutesC()
        {
            return new List<Route>
            {
                new Route{ Origin = "YYZ", Destination = "JFK" },
                new Route{ Origin = "YFR", Destination = "YJT" }                
            };
        }

        public static IEnumerable<Route> RoutesD()
        {
            return new List<Route>
            {
                new Route{ Origin = "YYZ", Destination = "JFK" },
                new Route{ Origin = "ALT", Destination = "GRU" }
            };
        }

        public static IEnumerable<Route> RoutesE()
        {
            return new List<Route>
            {
                new Route{ Origin = "YYZ", Destination = "JFK" }                
            };
        }

    }
}
