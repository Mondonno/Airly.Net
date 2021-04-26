using System.Threading.Tasks;
using System.Collections.Generic;

using AirlyAPI.Utilities;
using AirlyAPI.Handling.Errors;

// IMPORTANT DISTRIBUTION NOTE!
// What does do redirect?
// Redirect do the request to the api with the new id (`INSTALLATION_REPLACED` error) and returns them not error
// If you do not provide them our module give you error with succesor, message and code
// If the another installation is moved module give error (this prevents the infinity request loops and limits reaching)

namespace AirlyAPI.Interactions
{
    public class Installations : InteractionBase
    {
        public Installations(Airly airly) : base(airly) { }

        public async Task<List<Installation>> Nearest(Installation installation, double maxDistance = 3, int maxResults = 1) => installation != null ? await Nearest(installation.Location, maxDistance, maxResults) : null;
        public async Task<List<Installation>> Nearest(Location location, double maxDistance = 3, int maxResults = 1) => location != null ? await Nearest(location.Lat, location.Lng, maxDistance, maxResults) : null;
        private async Task<List<Installation>> Nearest(double lat, double lng, double maxDistance = 3, int maxResults = 1) => await Api.GetInstallationsNearestAsync(lat, lng, maxDistance, maxResults);

        // the redirect is untested because i can not get the replaced installation id :(
        public async Task<Installation> Info(int id, bool redirect = false) {
            if (!redirect) return await Api.GetInstallationByIdAsync(id);
            else
            {
                Installation installationData;
                try
                {
                    installationData = await Info(id);
                }
                catch(ElementPermentlyReplacedException error)
                {
                    var newSuccesor = (int?) error.Data["succesorId"];
                    if (newSuccesor == null) return default;

                    var newInstallationData = await Info((int) newSuccesor);

                    return newInstallationData;
                }
                catch { return default; }

                return installationData;
            }
        }

        public async Task<List<Installation>> Area(LocationArea area, int maxResults = 10)
        {
            double areaDistanceKilometers = GeoUtil.GetKmInArea(area);
            Location barycenter = area.GetBarycenter();

            List<Installation> installations = await Nearest(barycenter.Lat, barycenter.Lng, areaDistanceKilometers, maxResults);

            if (installations == null) return default;

            List<Installation> installationsFiltred = installations.FindAll(installation => area.Contains(installation.Location));
            return installationsFiltred;
        }
    }
}
