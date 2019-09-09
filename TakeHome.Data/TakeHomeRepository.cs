using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;

namespace TakeHome.Data
{
    public class TakeHomeRepository : ITakeHomeRepository
    {
        private readonly IDbConnection _db;
        public TakeHomeRepository(string connectionString)
        {
            _db = new NpgsqlConnection(connectionString);
        }

        public async Task<IEnumerable<Route>> GetRoutes(string origin, string destination)
        {
            var query = "select origin, destination FROM routes WHERE origin = @origin OR destination = @destination";

           return await RunQueryAsync<Route>(query, new { origin, destination });
        }

        public async Task<Dictionary<string, List<string>>> GetRoutesInRange(double midPointLatitude, double midPointLongitude, double range)
        {
            var query = "With airports_range AS (SELECT iata3 from airports " +
                "                                where |/((latitude - @midPointLatitude)^2 + (longitude - @midPointLongitude)^2) <= @range)" +
                        "SELECT origin, destination " +
                        "from routes " +
                        "where origin in (select iata3 from airports_range) " +
                        "and destination in (select iata3 from airports_range) " +
                        "group by origin, destination ";

            var routes = await RunQueryAsync<Route>(query, new { midPointLatitude, midPointLongitude, range });

            var map = new Dictionary<string, List<string>>();

            foreach (var route in routes)
            {
                if (!map.ContainsKey(route.Origin))
                    map.Add(route.Origin, new List<string>());

                map[route.Origin].Add(route.Destination);
            }

            return map;
        }

        public async Task<List<Airport>> GetAiports(string origin, string destination)
        {
            //Look for the 2 airports in Airports table using distinct to avoid possible duplicates
            var query = "SELECT iata3, name, city, country, latitude, longitude FROM airports WHERE iata3 = @origin or iata3 = @destination";

            var result = await RunQueryAsync<Airport, Coordinates>(query,
                (airport, coordinates) => {
                    airport.Coordinates = coordinates;
                    return airport;
                },
                new { origin, destination },
                splitOn: "latitude");

            return result?.ToList();
        }

        private async Task<IEnumerable<T>> RunQueryAsync<T>(string query, object parameters = null)
        {
            OpenConnectionAsync();

            var response = await _db.QueryAsync<T>(query, parameters);

            CloseConnection();

            return response;
        }

        private async Task<IEnumerable<T>> RunQueryAsync<T, TSecond>(string query, Func<T, TSecond, T> map, object parameters = null, string splitOn = "Id")
        {
            OpenConnectionAsync();

            var response = await _db.QueryAsync(query, map, parameters, splitOn: splitOn);

            CloseConnection();

            return response;
        }

        private void OpenConnectionAsync()
        {
            if (_db.State == ConnectionState.Closed)
                _db.Open();
        }

        private void CloseConnection()
        {
            if (_db.State != ConnectionState.Closed)
                _db.Close();
        }
    }
}
