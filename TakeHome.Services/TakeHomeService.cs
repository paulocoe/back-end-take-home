using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Data;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class TakeHomeService : ITakeHomeService
    {
        private readonly ITakeHomeRepository _repository;
        private int _shortestRouteConnections = int.MaxValue;
        private string _shortestRoute = "No Route";
        private Dictionary<string, List<string>> _mapOriginToDestinations;
        private string _finalDestination;
        private readonly int _minAmmountOfConnections = 2;

        public TakeHomeService(ITakeHomeRepository repositoryService)
        {
            this._repository = repositoryService;
        }

        public async Task<string> GetShortestRoute(Airport origin, Airport destination)
        {
            //Checks if there's a direct route and if it does, return it as shortest path
            IEnumerable<Route> routes = await _repository.GetRoutes(origin.Iata3, destination.Iata3);

            //Checks if both airports have at least 1 route and if they have a direct connection
            string validationMessage = ValidateRoutesAndCheckForConnection(routes, origin.Iata3, destination.Iata3);

            if (!string.IsNullOrEmpty(validationMessage))
                return validationMessage;

            _finalDestination = destination.Iata3;

            //Get Range
            var range = CalculateDistance(origin.Coordinates, destination.Coordinates);

            //Get All routes connecting airports found inside the range
            _mapOriginToDestinations = await _repository.GetRoutesInRange(origin.Coordinates.Latitude, origin.Coordinates.Longitude, range);

            await FindShortestPath(origin.Iata3, new List<string> { origin.Iata3 });

            return _shortestRoute;
        }

        public async Task<List<Airport>> GetAiports(string origin, string destination)
        {
            //Get origin and destination airports in DB
            return await _repository.GetAiports(origin, destination);
        }

        private async Task FindShortestPath(string origin, List<string> exceptions, string path = null, int connectionAmmount = 0)
        {
            //Aborts every recursion if the a shortest path with the minimmum 
            // amount of connections was found
            if (_shortestRouteConnections == _minAmmountOfConnections)
                return;

            //Adds current connection to temporary route
            path = FormatPath(path, origin);

            //Checks if the destinations was reached
            if (HasReachedFinalDestination(origin, path, connectionAmmount))
                return;

            //Checks if its still possible to find a shorter route than
            // the one found already
            if (!CanBeSmallerThanShortestRouteFound(connectionAmmount))
                return;

            if (!_mapOriginToDestinations.ContainsKey(origin))
                return;

            connectionAmmount++;

            int lastExceptionsIndex = exceptions.Count;

            //Get list of next destinations with exception of those who were or can be connected
            // by previous connections
            var destinations = _mapOriginToDestinations[origin].Except(exceptions).ToArray();

            //Adds the list of next destinations to exceptions for the next connection iteration
            exceptions.AddRange(_mapOriginToDestinations[origin]);

            foreach (var destination in destinations)
            {
                //Breaks the iteration if a short path with an
                // ammount of connections bigger than the current one by 1 has been found
                if (!CanBeSmallerThanShortestRouteFound(connectionAmmount))
                    break;

                await FindShortestPath(destination, exceptions, path, connectionAmmount);
            }

            exceptions.RemoveRange(lastExceptionsIndex, exceptions.Count - lastExceptionsIndex);
        }

        private string FormatPath(string path, string connection)
        {
            if (!string.IsNullOrEmpty(path))
                return $"{path} -> {connection}";

            return connection;

        }

        private double CalculateDistance(Coordinates origin, Coordinates destination)
        {
            double x = destination.Latitude - origin.Latitude;
            double y = destination.Longitude - origin.Longitude;

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        private Coordinates CalculateMiddlePoint(Coordinates origin, Coordinates destination)
        {
            double x = destination.Latitude + origin.Latitude;
            double y = destination.Longitude + origin.Longitude;

            return new Coordinates
            {
                Latitude = x / 2,
                Longitude = y / 2
            };
        }

        private string ValidateRoutesAndCheckForConnection(IEnumerable<Route> routes, string origin, string destination)
        {
            bool originFound = false;
            bool destinationFound = false;

            if (routes == null)
                return _shortestRoute;

            foreach (var route in routes)
            {
                if (route.Origin == origin)
                {
                    if (route.Destination == destination)
                        return FormatPath(origin, destination);

                    originFound = true;
                }
                else
                    destinationFound = true;
            }

            if (!originFound || !destinationFound)
                return _shortestRoute;

            return null;
        }

        private bool HasReachedFinalDestination(string airport, string path, int connection)
        {
            if (airport != _finalDestination)
                return false;

            _shortestRouteConnections = connection;
            _shortestRoute = path;
            return true;
        }

        private bool CanBeSmallerThanShortestRouteFound(int connection)
        {
           return connection < _shortestRouteConnections - 1;
        }
    }
}
