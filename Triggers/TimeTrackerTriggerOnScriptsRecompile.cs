#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace UnityBase
{
    [InitializeOnLoad]
    public  class TimeTrackerTriggerOnScriptsRecompile 
    {
        static TimeTrackerTriggerOnScriptsRecompile()
        {
            TimeTracker.OnTrigger();
        }
    }
}
#endif