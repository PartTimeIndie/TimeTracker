using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UnityBase
{
    /// <summary>
    /// This is the class that is saved to the player prefs
    /// It contains all the information about the time worked on the project
    /// </summary>
    public class TimeTrackerInfo
    {
        /// <summary>
        /// The last time the TimeTracker triggered. This is used to measure the time delta between the last two triggers
        /// </summary>
        public DateTime LastTrigger { get; set; }
        /// <summary>
        /// When did the stopwatch start. This is important to even have the stopwatch running if for
        /// some reason Unity Crashes etc.
        /// </summary>
        public DateTime StopWatchStart { get; set; }
        /// <summary>
        /// The index of the current task, we serialize the index to always return the same object
        /// If you deserialize, you will no longer get the reference to the class inside the all tasks list
        /// if you serialize the object itself
        /// </summary>
        public int CurrentTaskIndex { get; set; } = 0;
        /// <summary>
        /// Access the current task, we serialize the index to always return the same object
        /// If you deserialize, you will no longer get the reference to the class inside the all tasks list
        /// if you serialize the object itself
        /// </summary>
        [XmlIgnore]
        public TimeTrackerTask CurrentTask
        {
            get
            {
                if (AllTasks.Any())
                {
                    if(CurrentTaskIndex >= AllTasks.Count || CurrentTaskIndex < 0)
                    {
                        CurrentTaskIndex = 0;
                    }
                    return AllTasks[CurrentTaskIndex];
                }
                return new TimeTrackerTask() { TaskName = "Default"};
            }
            set
            {
                CurrentTaskIndex = AllTasks.IndexOf(value);
            }
        }
        /// <summary>
        /// The list of all user created tasks
        /// </summary>
        public List<TimeTrackerTask> AllTasks { get; set; } = new List<TimeTrackerTask>();
        /// <summary>
        /// I use the TimeTrackerTask class to keep track of the time worked on days
        /// maybe I need to change that in upcoming versions?
        /// </summary>
        public List<TimeTrackerTask> DailyTasks { get; set; } = new List<TimeTrackerTask>();
        /// <summary>
        /// The index of the current daily task, we serialize the index to always return the same object
        /// If you deserialize, you will no longer get the reference to the class inside the all daily tasks list
        /// if you serialize the object itself
        /// </summary>
        public int CurrentDailyTaskIndex { get; set; } = 0;
        /// <summary>
        /// Access the current current daily task, we serialize the index to always return the same object
        /// If you deserialize, you will no longer get the reference to the class inside the all daily tasks list
        /// if you serialize the object itself
        /// </summary>
        [XmlIgnore]
        public TimeTrackerTask CurrentDailyTask
        {
            get
            {
                if (CurrentDailyTaskIndex >= DailyTasks.Count || CurrentDailyTaskIndex < 0)
                {
                    CurrentTaskIndex = 0;
                }
                if (DailyTasks.Any())
                {
                    return DailyTasks[CurrentDailyTaskIndex];
                }             
                return GetDailyTask(DateTime.Now);
            }
            set
            {
                CurrentDailyTaskIndex = DailyTasks.IndexOf(value);
            }
        }

        /// <summary>
        /// Stores the last 100 triggers, you can use this information to change the MaximumMinutesBetweenTriggers
        /// </summary>
        public List<DateTime> Last100Triggers { get; set; } = new List<DateTime>();
        /// <summary>
        /// Is the stop watch currently running?
        /// </summary>
        public bool StopWatchRunning { get; set; }
        /// <summary>
        /// The time in ticks you spend on the project
        /// </summary>
        public long TotalTimeSpendOnProject { get; set; } = new TimeSpan(0, 0, 0).Ticks;

        /// <summary>
        /// Set the current daily task by passing a certain date
        /// </summary>
        /// <param name="dateTime"></param>
        public void SetCurrentTask(DateTime dateTime)
        {
            CurrentDailyTask = GetDailyTask(dateTime);
        }

        /// <summary>
        /// The Total Time Spend on the project as a Timespan
        /// </summary>
        public TimeSpan GetTotalTimeOnProject()
        {
            return new TimeSpan(TotalTimeSpendOnProject);
        }
        /// <summary>
        /// The Time spnd on the selected daily task as atimespan
        /// </summary>
        public TimeSpan GetCurrentDailyTimeOnProject()
        {
            if (CurrentDailyTask == null)
            {
                return new TimeSpan(0);
            }
            return new TimeSpan(CurrentDailyTask.TimeSpendOnTask);
        }

        /// <summary>
        /// Checks if the last trigger and this trigger is less than "maximalMinutesBetweenTriggers" apart 
        /// If it is it adds the time to the Project and Task you are working on.
        /// </summary>
        /// <param name="maximalMinutesBetweenTriggers" default="5"></param>
        public void NewTriggerReceived(double maximalMinutesBetweenTriggers)
        {
            var timeBetweenLastAndThisTrigger = (DateTime.Now - LastTrigger);
            if (timeBetweenLastAndThisTrigger.TotalMinutes <= maximalMinutesBetweenTriggers)
            {
                TotalTimeSpendOnProject = new TimeSpan(TotalTimeSpendOnProject).Add(timeBetweenLastAndThisTrigger).Ticks;
                if (this.CurrentTask != null)
                {
                    this.CurrentTask.TimeSpendOnTask = new TimeSpan(this.CurrentTask.TimeSpendOnTask).Add(timeBetweenLastAndThisTrigger).Ticks;
                }
                var dailyTask = GetDailyTask(DateTime.Now);
                dailyTask.TimeSpendOnTask = new TimeSpan(dailyTask.TimeSpendOnTask).Add(timeBetweenLastAndThisTrigger).Ticks;
            }
            LastTrigger = DateTime.Now;
            Last100Triggers.Add(LastTrigger);
            while (Last100Triggers.Count > 100)
            {
                Last100Triggers.RemoveAt(0);
            }
        }
        /// <summary>
        /// Gets or sets the daily task for the passed date
        /// </summary>
        /// <param name="dateTime">The date you want to get the daily task for</param>
        /// <returns>The daily task for that day</returns>
        private TimeTrackerTask GetDailyTask(DateTime dateTime)
        {
            var foundTask = DailyTasks.FirstOrDefault(x => x.TaskName == dateTime.Date.ToShortDateString());
            if (foundTask == null)
            {
                foundTask = new TimeTrackerTask() { TaskName = dateTime.Date.ToShortDateString() };
                DailyTasks.Add(foundTask);
            }
            return foundTask;
        }

     
    }
}
