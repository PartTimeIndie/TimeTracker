#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace UnityBase
{
    /// <summary>
    /// This trigger is used so we get notified when we change a file outside of unity.
    /// E.g.: Writing code
    /// </summary>
    [InitializeOnLoad]
    public class TimerTrackerTriggerFileSystem
    {
        static FileSystemWatcher watcher;
        static TimerTrackerTriggerFileSystem()
        {
            if(watcher != null)
            {
                return;
            }

            using (watcher = new FileSystemWatcher())
            {
                watcher.Path = System.IO.Directory.GetCurrentDirectory();

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                // Only watch text files.
                watcher.Filter = "*.txt";

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

               
            }
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            TimeTracker.OnTrigger();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            TimeTracker.OnTrigger();
        }

    }
}
#endif