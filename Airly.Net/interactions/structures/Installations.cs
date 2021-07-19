using System.Threading.Tasks;
using System.Collections.Generic;

using AirlyNet.Utilities;
using AirlyNet.Handling.Errors;
using AirlyNet.Models;

namespace AirlyNet.Interactions
{
    public class Installations : InteractionBase
    {
        public Installations(Airly airly) : base(airly) { }

        /// <summary>
        /// Getting nearest installations near the specified installation
        /// </summary>
        /// <param name="installation">Installation object</param>
        /// <param name="maxDistance">Max distance, deafult 3, double</param>
        /// <param name="maxResults">Max results, -1 = unlimited, deafult 1, integer</param>
        /// <returns>List of installations near the installation</returns>
        public async Task<List<Installation>> Nearest(Installation installation, double maxDistance = 3, int maxResults = 1) => installation != null ? await Nearest(installation.Location, maxDistance, maxResults) : null;

        /// <summary>
        /// Getting nearest installations in the specified location. If no results, returning null or empty null.
        /// </summary>
        /// <param name="location">Location object</param>
        /// <param name="maxDistance">Max distance, deafult 3, double</param>
        /// <param name="maxResults">Max results, -1 = unlimited, deafult 1, integer</param>
        /// <returns>List of installations near the location</returns>
        public async Task<List<Installation>> Nearest(Location location, double maxDistance = 3, int maxResults = 1) => location != null ? await Nearest(location.Lat, location.Lng, maxDistance, maxResults) : null;

        private async Task<List<Installation>> Nearest(double lat, double lng, double maxDistance = 3, int maxResults = 1) => await Api.GetInstallationsNearestAsync(lat, lng, maxDistance, maxResults);

        /// <summary>
        /// Getting informations about installation by providing the installation ID.
        /// </summary>
        /// <param name="id">Installation id, integer</param>
        /// <param name="redirect">If specified, redirecting to valid resource when the 301 returned</param>
        /// <returns>Single installation</returns>
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

                    var newInstallationData = await Info((int) newSuccesor); // it will automaticly rethrow inside (loops preventing)
                    return newInstallationData;
                }
                catch { return default; }

                return installationData;
            }
        }

        /// <summary>
        /// Getting installations in the specified area of location's.
        /// </summary>
        /// <param name="area">The LocationArea object</param>
        /// <param name="maxResults">Max results of the installations, deafult 10</param>
        /// <returns>List of installations in the specified area</returns>
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
