#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UnityBase
{
    /// <summary>
    /// Whenever we start a build we know that we have been working so we Track that
    /// </summary>
    public class TimeTrackerTriggerOnBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            TimeTracker.OnTrigger();
        }
    }
}
#endif
