#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnityBase
{
    [InitializeOnLoad]
    public class TimeTrackerTriggerUnityEditorKeystrokes
    {
        private static DateTime lastTime =DateTime.MinValue;
        static TimeTrackerTriggerUnityEditorKeystrokes()
        {
            System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue(null);

            value += () =>
            {
                if (Event.current.type == EventType.KeyDown)
                {
                    //we only need to trigger it every 10 seconds
                    //doing to many triggers will just cause to many writes to disk without any need
                    if ((DateTime.Now - lastTime).TotalSeconds > 10)
                    {
                        lastTime = DateTime.Now;
                        TimeTracker.OnTrigger();
                    }
                }
            };

            info.SetValue(null, value);
        }
    }
}
#endif
