
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnityBase
{
    /// <summary>
    /// Editor Window Class so that interacting with the time tracker is easy
    /// </summary>
    public class TimeTrackerEditorWindow : EditorWindow
    {
        //Needed so the window is scrollable
        private Vector2 scrollPosition;
        //New Task Information
        private string newTaskName;

        //Fooldout information
        private bool totalTimeFoldout = false;
        private bool dailyTimeFoldout = true;
        private bool timePeriodFoldout = false;
        private bool projectInfoFoldout = true;
        private bool settingsFoldout = false;
        private bool allTriggersFoldout = false;
        private bool externalTimeSpendFoldout = false;
        private bool triggerInformationFoldout = false;
        private bool taskFoldout = true;

        //Where should we add time when clicking the button
        private bool addTimeToTask = true;
        private bool addTimeToDay = true;
        private bool addTimeToProject = true;
        //How much time should we add
        private int minutesToAdd = 0;

        //How many days in the past should we calculate the time
        private int numberOfDays = 7;
        public TimeTrackerEditorWindow()
        {
            this.titleContent = new GUIContent("Time Tracker");
        }
        
        [MenuItem("Window/Time Tracker")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TimeTrackerEditorWindow));
        }
        void OnGUI()
        {

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            ProjectInfo();
            EditorGUILayout.Separator();
            TaskInformation();
            EditorGUILayout.Separator();
            ExternalInformation();
            EditorGUILayout.Separator();
            Settings();
            EditorGUILayout.Separator();
            TriggerInformation();
            EditorGUILayout.Separator();
            Help();
            EditorGUILayout.Separator();
            Support();
            EditorGUILayout.EndScrollView();
        }

        private static void Help()
        {
            EditorGUILayout.LabelField("Help", EditorStyles.boldLabel);
            if (GUILayout.Button("Documentation"))
            {
                Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "Assets\\TimeTracker\\TimeTrackerDocumentation.pdf"));
            }
        }

        private static void Support()
        {
            EditorGUILayout.LabelField("Want to support me?", EditorStyles.boldLabel);
            if (GUILayout.Button("Improve this asset on github"))
            {
                Process.Start("https://github.com/PartTimeIndie/TimeTracker");
            }
            if (GUILayout.Button("Check out my games"))
            {
                Process.Start("https://store.steampowered.com/curator/33183467");
            }
        }

        private void Settings()
        {
            settingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(settingsFoldout, "Settings");
            if (!settingsFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            EditorGUI.indentLevel++;
            double maximumTimeBetweenTriggers;
            EditorGUILayout.LabelField(new GUIContent("Maximum Time Between Triggers", "If two triggers are further apart than this time, the system will not count it towards you working on  the game. Default = 5 minutes, take a look at the mean time between triggers and add something extra"));
            if (Double.TryParse(EditorGUILayout.TextField("Minutes: ", TimeTracker.Settings.MaximumMinutesBetweenTriggers.ToString()), out maximumTimeBetweenTriggers))
            {
                if (maximumTimeBetweenTriggers != TimeTracker.Settings.MaximumMinutesBetweenTriggers)
                {
                    TimeTracker.Settings.MaximumMinutesBetweenTriggers = maximumTimeBetweenTriggers;
                    TimeTracker.SaveSettings();
                }
            }        

            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private void ExternalInformation()
        {
            externalTimeSpendFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(externalTimeSpendFoldout, "External Time Spend");
            if (externalTimeSpendFoldout)
            {
                EditorGUI.indentLevel++;
                AddTimeManually();
                StopWatchTracker();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private void StopWatchTracker()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stopwatch Tracker");

            if (TimeTracker.TimeTrackerInfo.StopWatchRunning)
            {
                EditorGUILayout.LabelField("Time in minutes: " + Math.Round((DateTime.Now - TimeTracker.TimeTrackerInfo.StopWatchStart).TotalMinutes, 2));
                if (GUILayout.Button("Stop and Add Time"))
                {
                    var timeToAdd = (DateTime.Now - TimeTracker.TimeTrackerInfo.StopWatchStart);
                    if (addTimeToProject)
                    {
                        TimeTracker.TimeTrackerInfo.TotalTimeSpendOnProject = new TimeSpan(TimeTracker.TimeTrackerInfo.TotalTimeSpendOnProject).Add(timeToAdd).Ticks;
                    }
                    if (addTimeToTask)
                    {
                        TimeTracker.TimeTrackerInfo.CurrentTask.TimeSpendOnTask = new TimeSpan(TimeTracker.TimeTrackerInfo.CurrentTask.TimeSpendOnTask).Add(timeToAdd).Ticks;
                    }
                    if (addTimeToDay)
                    {
                        TimeTracker.TimeTrackerInfo.CurrentDailyTask.TimeSpendOnTask = new TimeSpan(TimeTracker.TimeTrackerInfo.CurrentDailyTask.TimeSpendOnTask).Add(timeToAdd).Ticks;
                    }
                    TimeTracker.TimeTrackerInfo.StopWatchRunning = false;
                    TimeTracker.SaveTimeTrackerInfo();
                }

                if (GUILayout.Button("Cancel"))
                {

                    TimeTracker.TimeTrackerInfo.StopWatchRunning = false;
                    TimeTracker.SaveTimeTrackerInfo();
                }
            }
            else
            {
                if (GUILayout.Button("Start"))
                {
                    TimeTracker.TimeTrackerInfo.StopWatchRunning = true;
                    TimeTracker.TimeTrackerInfo.StopWatchStart = DateTime.Now;
                    TimeTracker.SaveTimeTrackerInfo();
                }
            }

            EditorGUILayout.EndHorizontal();
            if (TimeTracker.TimeTrackerInfo.StopWatchRunning)
            {
                EditorGUILayout.LabelField("While the stopwatch is running no other time is tracked!", EditorStyles.boldLabel);
            }
        }
        private void AddTimeManually()
        {
            addTimeToProject = EditorGUILayout.Toggle("Add To Project", addTimeToProject);
            addTimeToTask = EditorGUILayout.Toggle("Add To Task", addTimeToTask);
            addTimeToDay = EditorGUILayout.Toggle("Add To Day", addTimeToDay);
            EditorGUILayout.BeginHorizontal();
            Int32.TryParse(EditorGUILayout.TextField(new GUIContent("Minutes To Add"), minutesToAdd.ToString()), out minutesToAdd);
            var timeToAdd = new TimeSpan(0, minutesToAdd, 0);


            if (GUILayout.Button("Add Time"))
            {
                if (addTimeToProject)
                {
                    TimeTracker.TimeTrackerInfo.TotalTimeSpendOnProject = new TimeSpan(TimeTracker.TimeTrackerInfo.TotalTimeSpendOnProject).Add(timeToAdd).Ticks;
                }
                if (addTimeToTask)
                {
                    TimeTracker.TimeTrackerInfo.CurrentTask.TimeSpendOnTask = new TimeSpan(TimeTracker.TimeTrackerInfo.CurrentTask.TimeSpendOnTask).Add(timeToAdd).Ticks;
                }
                if (addTimeToDay)
                {
                    TimeTracker.TimeTrackerInfo.CurrentDailyTask.TimeSpendOnTask = new TimeSpan(TimeTracker.TimeTrackerInfo.CurrentDailyTask.TimeSpendOnTask).Add(timeToAdd).Ticks;
                }
                TimeTracker.SaveTimeTrackerInfo();
            }

            EditorGUILayout.EndHorizontal();
        }
        private void TaskInformation()
        {
            taskFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(taskFoldout, "Task");
            if (!taskFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            EditorGUI.indentLevel++;
            AddTask();
            ShowSelectedTask();

            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private static void ShowSelectedTask()
        {
            var index = TimeTracker.TimeTrackerInfo.AllTasks.Select(x => x.TaskName).ToList().IndexOf(TimeTracker.TimeTrackerInfo.CurrentTask.TaskName);
            EditorGUILayout.BeginHorizontal();
            var newIndex = EditorGUILayout.Popup(new GUIContent("Selected Task"), index, TimeTracker.TimeTrackerInfo.AllTasks.Select(x => x.TaskName).ToArray());
            if (GUILayout.Button("X", EditorStyles.miniButton))
            {
                TimeTracker.TimeTrackerInfo.AllTasks.Remove(TimeTracker.TimeTrackerInfo.CurrentTask);

                TimeTracker.SaveTimeTrackerInfo();
            }
            EditorGUILayout.EndHorizontal();
            if (index != newIndex)
            {
                TimeTracker.TimeTrackerInfo.CurrentTask = TimeTracker.TimeTrackerInfo.AllTasks[newIndex];

                TimeTracker.SaveTimeTrackerInfo();
            }

            if (TimeTracker.TimeTrackerInfo.CurrentTask != null)
            {
                var timeOnTask = TimeTracker.TimeTrackerInfo.CurrentTask.GetTimeSpendOnTask();
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Days: " + timeOnTask.Days);
                EditorGUILayout.LabelField("Hours: " + timeOnTask.Hours);
                EditorGUILayout.LabelField("Minutes: " + timeOnTask.Minutes);
                EditorGUILayout.LabelField("Seconds: " + timeOnTask.Seconds);
                EditorGUI.indentLevel--;
            }
        }
        private void AddTask()
        {
            EditorGUILayout.BeginHorizontal();
            newTaskName = EditorGUILayout.TextField("Name", newTaskName);
            if (GUILayout.Button("Add Task"))
            {
                if (TimeTracker.TimeTrackerInfo.AllTasks.Any(x => x.TaskName == newTaskName))
                {
                    newTaskName = "selected task name already exists";
                }
                else
                {
                    TimeTracker.TimeTrackerInfo.AllTasks.Add(new TimeTrackerTask() { TaskName = newTaskName });
                    TimeTracker.TimeTrackerInfo.CurrentTask = TimeTracker.TimeTrackerInfo.AllTasks.Last();
                    TimeTracker.SaveTimeTrackerInfo();
                }
            }


            EditorGUILayout.EndHorizontal();
        }
        private void ProjectInfo()
        {
            projectInfoFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(projectInfoFoldout, "Time Spend on Project");
            if (!projectInfoFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            EditorGUI.indentLevel++;

            TotalTime();

            DailyTimes();
            TimesOverPeriod();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();

        }
        private void TotalTime()
        {
            var totalTimeOnProject = TimeTracker.TimeTrackerInfo.GetTotalTimeOnProject();
            totalTimeFoldout = EditorGUILayout.Foldout(totalTimeFoldout, "Total Time", true);
            if (totalTimeFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Days: " + totalTimeOnProject.Days);
                EditorGUILayout.LabelField("Hours: " + totalTimeOnProject.Hours);
                EditorGUILayout.LabelField("Minutes: " + totalTimeOnProject.Minutes);
                EditorGUILayout.LabelField("Seconds: " + totalTimeOnProject.Seconds);

                EditorGUI.indentLevel--;
            }
        }
        private void DailyTimes()
        {
            dailyTimeFoldout = EditorGUILayout.Foldout(dailyTimeFoldout, "Daily Time", true);
            if (dailyTimeFoldout)
            {
                EditorGUI.indentLevel++;
                int oldIndex = TimeTracker.TimeTrackerInfo.CurrentDailyTaskIndex;
                EditorGUILayout.BeginHorizontal();
                TimeTracker.TimeTrackerInfo.CurrentDailyTaskIndex = EditorGUILayout.Popup(new GUIContent("Selected Day"), TimeTracker.TimeTrackerInfo.CurrentDailyTaskIndex, TimeTracker.TimeTrackerInfo.DailyTasks.Select(x => x.TaskName).ToArray());
                if (GUILayout.Button("X", EditorStyles.miniButton))
                {
                    TimeTracker.TimeTrackerInfo.DailyTasks.Remove(TimeTracker.TimeTrackerInfo.CurrentDailyTask);
                    TimeTracker.SaveTimeTrackerInfo();
                }
                EditorGUILayout.EndHorizontal();
                if (oldIndex != TimeTracker.TimeTrackerInfo.CurrentDailyTaskIndex)
                {
                    TimeTracker.SaveTimeTrackerInfo();
                }
                var dailyTime = TimeTracker.TimeTrackerInfo.GetCurrentDailyTimeOnProject();
                EditorGUILayout.LabelField("Days: " + dailyTime.Days);
                EditorGUILayout.LabelField("Hours: " + dailyTime.Hours);
                EditorGUILayout.LabelField("Minutes: " + dailyTime.Minutes);
                EditorGUILayout.LabelField("Seconds: " + dailyTime.Seconds);
                EditorGUI.indentLevel--;
            }
        }
        private void TimesOverPeriod()
        {
            timePeriodFoldout = EditorGUILayout.Foldout(timePeriodFoldout, "Time Perdiod", true);
            if (timePeriodFoldout)
            {
                EditorGUI.indentLevel++;

               Int32.TryParse(  EditorGUILayout.TextField("Last x Days", numberOfDays.ToString()), out numberOfDays);
                var timePeriodTotal = new TimeSpan(TimeTracker.TimeTrackerInfo.DailyTasks.Select(x => x.TimeSpendOnTask).Reverse().Take(numberOfDays).Sum(x => x));
                EditorGUILayout.LabelField("Days: " + timePeriodTotal.Days);
                EditorGUILayout.LabelField("Hours: " + timePeriodTotal.Hours);
                EditorGUILayout.LabelField("Minutes: " + timePeriodTotal.Minutes);
                EditorGUILayout.LabelField("Seconds: " + timePeriodTotal.Seconds);
                EditorGUI.indentLevel--;
            }
        }
        private void TriggerInformation()
        {
            triggerInformationFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(triggerInformationFoldout, "Trigger Information");
            if (!triggerInformationFoldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }
            List<double> timeBetweenAllTriggers = new List<double>();
            DateTime lastTrigger = DateTime.MinValue;
            foreach (var triggerTime in TimeTracker.TimeTrackerInfo.Last100Triggers)
            {
                if (lastTrigger != DateTime.MinValue)
                {
                    timeBetweenAllTriggers.Add((triggerTime - lastTrigger).TotalMinutes);
                }
                lastTrigger = triggerTime;
            }
            timeBetweenAllTriggers = timeBetweenAllTriggers.OrderBy(x => x).ToList();
            EditorGUI.indentLevel++;
            if (timeBetweenAllTriggers.Count > 1)
            {
                EditorGUILayout.LabelField("Mean Time Between Triggers: " + Math.Round(timeBetweenAllTriggers[(int)(timeBetweenAllTriggers.Count / 2)], 2) + " minutes");
            }
            else
            {

                EditorGUILayout.LabelField("Mean Time Between Triggers: n/a");
            }
            EditorGUILayout.LabelField("Number of Triggers Saved: " + TimeTracker.TimeTrackerInfo.Last100Triggers.Count);
            EditorGUILayout.LabelField("Last Trigger: " + TimeTracker.TimeTrackerInfo.LastTrigger.ToString());
            allTriggersFoldout = EditorGUILayout.Foldout(allTriggersFoldout, "All Saved Trigger Times", true);
            if (allTriggersFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (var value in timeBetweenAllTriggers)
                {
                    EditorGUILayout.LabelField(Math.Round(value, 2) + " minutes");
                }
                EditorGUI.indentLevel--;
            }


            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
#endif
