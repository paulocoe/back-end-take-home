using System.Collections.Generic;
using System.Threading.Tasks;
using TakeHome.Models;

namespace TakeHome.Services
{
    public interface ITakeHomeService
    {
        Task<string> GetShortestRoute(Airport origin, Airport destination);
        Task<List<Airport>> GetAiports(string origin, string destination);
    }
}