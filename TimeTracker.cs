
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.VisualScripting;

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

        public static TimeTrackerSave timeTrackerSave = new TimeTrackerSave();
        public static TimeTrackerSettingsSave timeTrackerSettingsSave = new TimeTrackerSettingsSave();

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
                        string saveDataPath = Application.dataPath + "/TimeTracker/TimeTrackerSettingsSave.xml";

                        try
                        {
                            if (File.Exists(saveDataPath))
                            {
                                string timeTrackerSaveData = File.ReadAllText(saveDataPath);

                                if (!string.IsNullOrEmpty(timeTrackerSaveData))
                                {
                                    _settings = TimeTrackerSerializationHelper.Deserialize<TimeTrackerSettings>(timeTrackerSaveData);
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Save file not found, initializing empty TimeTrackerInfo.");
                                _settings = new TimeTrackerSettings(); // Default initialization
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error loading TimeTrackerInfo: {ex.Message}");
                            _settings = new TimeTrackerSettings(); // Initialize to a safe default
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
                try
                {
                    string saveDataPath = Application.dataPath + "/TimeTracker/TimeTrackerSettingsSave.xml";

                    // Serialize the TimeTrackerInfo to an XML string
                    string timeTrackerXmlSaveString = TimeTrackerSerializationHelper.Serialize(Settings);

                    // Write the serialized XML string to a file
                    File.WriteAllText(saveDataPath, timeTrackerXmlSaveString);

                    Debug.Log($"TimeTrackerInfo successfully saved to {saveDataPath}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error saving TimeTrackerInfo: {ex.Message}");
                }
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
                        string saveDataPath = Application.dataPath + "/TimeTracker/TimeTrackerSave.xml";

                        try
                        {
                            if (File.Exists(saveDataPath))
                            {
                                string timeTrackerSaveData = File.ReadAllText(saveDataPath);

                                if (!string.IsNullOrEmpty(timeTrackerSaveData))
                                {
                                    _timeTrackerInfo = TimeTrackerSerializationHelper.Deserialize<TimeTrackerInfo>(timeTrackerSaveData);
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Save file not found, initializing empty TimeTrackerInfo.");
                                _timeTrackerInfo = new TimeTrackerInfo(); // Default initialization
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error loading TimeTrackerInfo: {ex.Message}");
                            _timeTrackerInfo = new TimeTrackerInfo(); // Initialize to a safe default
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
                try
                {
                    string saveDataPath = Application.dataPath + "/TimeTracker/TimeTrackerSave.xml";

                    // Serialize the TimeTrackerInfo to an XML string
                    string timeTrackerXmlSaveString = TimeTrackerSerializationHelper.Serialize(TimeTrackerInfo);

                    // Write the serialized XML string to a file
                    File.WriteAllText(saveDataPath, timeTrackerXmlSaveString);

                    Debug.Log($"TimeTrackerInfo successfully saved to {saveDataPath}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error saving TimeTrackerInfo: {ex.Message}");
                }
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


    [System.Serializable]
    public class TimeTrackerSave
    {
        public string timeTrackerXMLSaveString;

    }

    [System.Serializable]
    public class TimeTrackerSettingsSave
    {
        public string timeTrackerXMLSaveSettingsString;
    }
}
#endif
