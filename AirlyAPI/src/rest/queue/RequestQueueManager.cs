using System;
using System.Collections.Generic;
using System.Linq;

namespace AirlyAPI.Handling
{
    public class RequestQueueManager : Dictionary<string, RequestQueuer>, IDisposable // internal module with exposed parent
    {
        public int Handlers() => Count;
        public bool Inactive() => Count == 0;
        public bool RateLimited()
        {
            if (Count == 0) return false;

            List<bool> originLimits = new(Values.Select((e, b) => e.RateLimited));

            int limits = originLimits.Count;
            int limited = originLimits.FindAll((e) => e == true).Count;

            return limits == limited;
        }

        public RequestQueuer Get(string route)
        {
            TryGetValue(route, out RequestQueuer queuer);
            return queuer ?? null;
        }
        public void Set(string route, RequestQueuer queuer)
        {
            bool contains = ContainsKey(route);
            if (contains) Remove(route);
            Add(route, queuer);
        }
        public void Dispose()
        {
            foreach (var avaibleQueuer in this)
            {
                var queuer = avaibleQueuer.Value ?? null;
                if (queuer == null || string.IsNullOrEmpty(avaibleQueuer.Key)) continue;

                queuer.Dispose();
            }
            Clear();
        }
    }
}