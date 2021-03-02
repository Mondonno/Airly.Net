using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AirlyAPI.Interactions
{
    public class Installations : InteractionBase
    {
        public Installations(Airly airly, RESTManager rest) : base(airly, rest) { }

        public async Task<List<Installation>> Nearest(double lat, double lng, double maxDistance = 3, int maxResults = 1) => await api<List<Installation>>("installations/nearest", new
        {
            lat,
            lng,
            maxDistanceKM = maxDistance,
            maxResults
        });

        public async Task<List<Installation>> Nearest(Location location, double maxDistance = 3, int maxResults = 1) => await Nearest(location.lat, location.lng, maxDistance, maxResults);

        // What does do redirect?
        // Redirect do the request to the api with the new id (`INSTALLATION_REPLACED` error) and returns them not error
        // If you do not provide them our module give you error with succesor, message and code
        // If the another installation is moved module give error (this prevents the infinity request loops)
        public async Task<Installation> Info(int id, bool redirect = false) => await api<Installation>($"installations/{id}", new { });

        public async Task<Installation> Area(LocationArea area)
        {
            double km = GeoUtil.GetKmInArea(area);
            Location barycenter = area.GetBarycenter();

            List<Installation> installations = await Nearest(barycenter.lat, barycenter.lng, km, 100);
            if (installations.Count == 0) return null;

            List<Installation> installationsFiltred = installations.FindAll((installation) => area.Contains(installation.location));
            if (installationsFiltred.Count == 0) return null;

            Installation installationResult = installationsFiltred.GetRange(0, 1)[0];
            return installationResult;
        }
    }
}
