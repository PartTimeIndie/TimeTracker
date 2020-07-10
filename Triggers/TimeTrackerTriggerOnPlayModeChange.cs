#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace UnityBase
{
    [InitializeOnLoadAttribute]
    public class TimeTrackerTriggerOnPlayModeChange
    {
        static TimeTrackerTriggerOnPlayModeChange()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
           
          
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            TimeTracker.OnTrigger();
        }      


    }
}
#endif