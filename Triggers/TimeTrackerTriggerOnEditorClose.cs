#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;

namespace UnityBase
{
    /// <summary>
    /// When the editor is closed is the last time we worked on something we trigger it to save time
    /// </summary>
    [InitializeOnLoad]
    public class TimeTrackerTriggerOnEditorClose
    {
        private static Task runningTask;

        static TimeTrackerTriggerOnEditorClose()
        {
            EditorApplication.wantsToQuit += WantsToQuit;
            if(runningTask != null)
            {
                return;
            }
            
        }
        static bool WantsToQuit()
        {
            TimeTracker.OnTrigger();
            return true;
        }
     
    }
}
#endif
