using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityBase
{
    /// <summary>
    /// Settings Class right now it only contains the Time between triggers,
    /// might change in the future, that is why I created a class, to maintain compatability between versions
    /// </summary>
    public class TimeTrackerSettings 
    {
        public double MaximumMinutesBetweenTriggers { get; set; } = 5;
    }
}
