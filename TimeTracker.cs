
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnityBase
{
    /// <summary>
    /// This class is the access point for all the triggers to shoot there events at.
    /// It also funcitonns as the access point for all the data
    /// </summary>
    public static class TimeTracker
    {
        /// <summary>
        /// The instance of the loaded time tracker info class
        /// </summary>
        private static TimeTrackerInfo _timeTrackerInfo = null;
        /// <summary>
        /// The instance of the loaded time tracker settings class
        /// </summary>
        private static TimeTrackerSettings _settings;

        /// <summary>
        /// The PlayerPrefsKey where the information about the project is stored.
        /// It uses the product name, so if you change it your data is lost
        /// </summary>
        private static readonly string TimeTrackerInfoPlayerPrefKey = PlayerSettings.productName + "TimeTrackerInfo";
        /// <summary>
        /// The PlayerPrefsKey where the settings is stored.
        /// It uses the product name, so if you change it your data is lost
        /// </summary>
        private static readonly string TimeTrackerSettingsPlayerPrefKey = PlayerSettings.productName + "TimeTrackerSettings";
   
        //Lock object for multithreading. Probably not needed because of unity single thread approach
        //More there to keep it save for the future
        private static object trackerInfoLockObject = new object();
        private static object trackerSettingsLockObject = new object();

        /// <summary>
        /// The settings class for the time tracker
        /// </summary>
        public static TimeTrackerSettings Settings
        {
            get 
            {
                if (_settings == null)
                {
                    lock (trackerSettingsLockObject)
                    {
                        try
                        {
                            _settings = TimeTrackerSerializationHelper.Deserialize<TimeTrackerSettings>(PlayerPrefs.GetString(TimeTrackerSettingsPlayerPrefKey, TimeTrackerSerializationHelper.Serialize(new TimeTrackerSettings())));
                        }
                        catch
                        {
                            _settings = new TimeTrackerSettings();
                            Debug.LogError("Your player pref file was corrupt, your settings have been reset.");
                            PlayerPrefs.DeleteKey(TimeTrackerSettingsPlayerPrefKey);
                            PlayerPrefs.Save();
                        }
                    }
                }
                return _settings;
            }
        }
        /// <summary>
        /// Saves the time tracker settings
        /// </summary>
        public static void SaveSettings()
        {
            lock (trackerSettingsLockObject)
            {
                PlayerPrefs.SetString(TimeTrackerSettingsPlayerPrefKey, TimeTrackerSerializationHelper.Serialize(Settings)); 
                PlayerPrefs.Save();
            }
        }


        /// <summary>
        /// Returns a copy of the TimeTrackerInfoClass
        /// </summary>
        public static TimeTrackerInfo TimeTrackerInfo
        {
            get 
            {
                if (_timeTrackerInfo == null)
                {
                
                    lock (trackerInfoLockObject)
                    {
                        try
                        {
                            _timeTrackerInfo = TimeTrackerSerializationHelper.Deserialize<TimeTrackerInfo>(PlayerPrefs.GetString(TimeTrackerInfoPlayerPrefKey, TimeTrackerSerializationHelper.Serialize(new TimeTrackerInfo())));
                        }
                        catch
                        {
                            _timeTrackerInfo = new TimeTrackerInfo();
                            Debug.LogError("Your player pref file was corrupt, you lost all your tracke data.");
                            PlayerPrefs.DeleteKey(TimeTrackerInfoPlayerPrefKey);
                            PlayerPrefs.Save();
                        }
                    }
                    
                }
                return _timeTrackerInfo; 
            }
        }
        /// <summary>
        /// Saves the TimeTrackerInfo class to disc
        /// </summary>
        public static void SaveTimeTrackerInfo()
        {
            lock (trackerInfoLockObject)
            {
                PlayerPrefs.SetString(TimeTrackerInfoPlayerPrefKey, TimeTrackerSerializationHelper.Serialize(TimeTrackerInfo));
                PlayerPrefs.Save();
            }
        }

   

       /// <summary>
       /// Checks if the last trigger and this trigger is less than 5 minutes apart (configurable)
       /// If it is it adds the time to the Project and Task you are working on.
       /// </summary>
        public static void OnTrigger()
        {
            if (TimeTrackerInfo.StopWatchRunning)
            {
                //If we have the stop watch running no auto stuff is running
                return;
            }
            TimeTrackerInfo.NewTriggerReceived(Settings.MaximumMinutesBetweenTriggers);
            SaveTimeTrackerInfo();

        }
    }
}
#endif