using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AirlyAPI.Interactions
{
    public class Installations : InteractionBase
    {
        public async Task<List<Installation>> Nearest(double lat, double lng, double maxDistance = 3, int maxResults = 1) => await api<List<Installation>>("installations/nearest", new
        {
            lat,
            lng,
            maxDistanceKM = maxDistance,
            maxResults
        });
        public async Task<Installation> Info(int id) => await api<Installation>($"installations/{id.ToString()}", new { });
    }
}
