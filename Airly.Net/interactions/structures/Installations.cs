using System.Threading.Tasks;
using System.Collections.Generic;

using AirlyNet.Utilities;
using AirlyNet.Common.Handling.Exceptions;
using AirlyNet.Common.Models;

namespace AirlyNet.Interactions.Structures
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
        public async Task<List<AirInstallation>> Nearest(AirInstallation installation, double maxDistance = 3, int maxResults = 1) => installation != null ? await Nearest(installation.Location, maxDistance, maxResults) : null;

        /// <summary>
        /// Getting nearest installations in the specified location. If no results, returning null or empty null.
        /// </summary>
        /// <param name="location">Location object</param>
        /// <param name="maxDistance">Max distance, deafult 3, double</param>
        /// <param name="maxResults">Max results, -1 = unlimited, deafult 1, integer</param>
        /// <returns>List of installations near the location</returns>
        public async Task<List<AirInstallation>> Nearest(AirLocation location, double maxDistance = 3, int maxResults = 1) => location != null ? await Nearest(location.Lat, location.Lng, maxDistance, maxResults) : null;

        private async Task<List<AirInstallation>> Nearest(double lat, double lng, double maxDistance = 3, int maxResults = 1) => await Api.GetInstallationsNearestAsync(lat, lng, maxDistance, maxResults);

        /// <summary>
        /// Getting informations about installation by providing the installation ID.
        /// </summary>
        /// <param name="id">Installation id, integer</param>
        /// <param name="redirect">If specified, redirecting to valid resource when the 301 returned</param>
        /// <returns>Single installation</returns>
        public async Task<AirInstallation> Info(int id, bool redirect = false) {
            if (!redirect) return await Api.GetInstallationByIdAsync(id);
            else
            {
                AirInstallation installationData;
                try
                {
                    installationData = await Info(id); // not recursive because passing redirect = false;
                }
                catch(ElementPermentlyReplacedException error)
                {
                    var newSuccesor = (int?) error.Data["succesorId"]; // null is begin handled in the RequestQueuer
                    var newInstallationData = await Info((int) newSuccesor); // not recursive because passing redirect = false;

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
        public async Task<List<AirInstallation>> Area(AirLocationArea area, int maxResults = 10)
        {
            double areaDistanceKilometers = GeoUtil.GetKmInArea(area);
            AirLocation barycenter = area.GetBarycenter();

            List<AirInstallation> installations = await Nearest(barycenter.Lat, barycenter.Lng, areaDistanceKilometers, maxResults);

            if (installations == null) return default;

            List<AirInstallation> installationsFiltred = installations.FindAll(installation => area.Contains(installation.Location));
            return installationsFiltred;
        }
    }
}
