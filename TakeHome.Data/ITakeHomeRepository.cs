using System.Collections.Generic;
using System.Threading.Tasks;
using TakeHome.Models;

namespace TakeHome.Data
{
    public interface ITakeHomeRepository
    {
        Task<Dictionary<string, List<string>>> GetRoutesInRange(double midPointLatitude, double midPointLongitude, double radius);
        Task<IEnumerable<Route>> GetRoutes(string origin, string destination);
        Task<List<Airport>> GetAiports(string origin, string destination);
    }
}