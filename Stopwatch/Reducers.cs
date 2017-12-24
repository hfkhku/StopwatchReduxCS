using Redux;
using Stopwatch.Actions;
using Stopwatch.Models;
using Stopwatch.States;
using System;
using System.Collections.Generic;
using System.Linq;
using static Stopwatch.States.ApplicationStateKey;
using System.Collections.ObjectModel;

namespace Stopwatch
{
    public class Reducers
    {
        public static ApplicationState StopwatchReducer(ApplicationState previousState, IAction action)
        {

            if (action is TimeFormatAction formatAction)
            {
                return TimeFormatReducer(previousState, formatAction);
            }

            if (action is ChangeModeAction modeAction)
            {
                return ChangeModeReducer(previousState, modeAction);
            }

            if (action is TimerAction timerAction)
            {
                return TimerReducer(previousState, timerAction);
            }

            if (action is LapAction lapAction)
            {
                return LapReducer(previousState, lapAction);
            }

            return previousState;
        }

        private static ApplicationState LapReducer(ApplicationState previousState, LapAction action)
        {
            var timerScheduler = previousState.TimerScheduler;
            var startTime = previousState.GetState<DateTime>(StartTime);
            var lapTimeList = previousState.GetState<ObservableCollection<LapTime>>(LapTimeList);
            var now = previousState.GetState<DateTime>(Now);
            //Debug.WriteLine($"LapAction Now:{ now}");
            var prevLap = lapTimeList.Any() ? lapTimeList.Last().Time : startTime;
            lapTimeList.Add(new LapTime(now, timerScheduler.Now.DateTime.ToLocalTime() - prevLap));

            var max = lapTimeList.Max(s => s.Span);
            var min = lapTimeList.Min(s => s.Span);

            var nextStates = previousState.States.SetItems(new Dictionary<string, object>()
            {
                { LapTimeList, lapTimeList },
                { MaxLapTime, max },
                { MinLapTime, min },
            });

            return new ApplicationState(nextStates, previousState.TimerScheduler);
        }

        private static ApplicationState TimerReducer(ApplicationState previousState, TimerAction action)
        {
            //Debug.WriteLine($"TimerAction Now:{ ((TimerAction)action).Now}");
            var span = action.Now - previousState.GetState<DateTime>(StartTime);
            var nextStates = previousState.States.SetItems(new Dictionary<string, object>()
            {
                { NowSpan, span },
                { Now, action.Now },
            });

            return new ApplicationState(nextStates, previousState.TimerScheduler);
        }

        private static ApplicationState ChangeModeReducer(ApplicationState previousState, ChangeModeAction action)
        {
            var previousMode = previousState.GetState<StopwatchMode>(Mode);
            var timerScheduler = previousState.TimerScheduler;
            string buttonLabel;
            StopwatchMode nextMode;
            DateTime startTime;
            var lapTimeList = previousState.GetState<ObservableCollection<LapTime>>(LapTimeList);
            var now = previousState.GetState<DateTime>(Now);
            var l = new Dictionary<string, object>();
            TimeSpan max, min;
            switch (previousMode)
            {
                case StopwatchMode.Init:
                    nextMode = StopwatchMode.Start;
                    buttonLabel = Constants.StopLabel;
                    startTime = timerScheduler.Now.DateTime.ToLocalTime();
                    //Debug.WriteLine($"startTime:{startTime}");
                    l.Add(Mode, nextMode);
                    l.Add(ButtonLabel, buttonLabel);
                    l.Add(StartTime, startTime);
                    break;
                case StopwatchMode.Start:
                    //Debug.WriteLine($"ChangeModeAction Now:{ now}");
                    nextMode = StopwatchMode.Stop;
                    buttonLabel = Constants.ResetLabel;
                    startTime = previousState.GetState<DateTime>(StartTime);
                    var prevLap = lapTimeList.Any() ? lapTimeList.Last().Time : startTime;
                    //Debug.WriteLine($"ChangeModeAction prevLap:{ prevLap}");
                    lapTimeList.Add(new LapTime(time: now, span: timerScheduler.Now.DateTime.ToLocalTime() - prevLap));
                    max = lapTimeList.Max(s => s.Span);
                    min = lapTimeList.Min(s => s.Span);
                    l.Add(LapTimeList, lapTimeList);
                    l.Add(Mode, nextMode);
                    l.Add(ButtonLabel, buttonLabel);
                    l.Add(MaxLapTime, max);
                    l.Add(MinLapTime, min);
                    break;
                case StopwatchMode.Stop:
                    nextMode = StopwatchMode.Init;
                    buttonLabel = Constants.StartLabel;
                    startTime = timerScheduler.Now.DateTime.ToLocalTime();
                    lapTimeList.Clear();
                    max = TimeSpan.Zero;
                    min = TimeSpan.Zero;
                    l.Add(Mode, nextMode);
                    l.Add(ButtonLabel, buttonLabel);
                    l.Add(StartTime, startTime);
                    l.Add(NowSpan, TimeSpan.Zero);
                    l.Add(LapTimeList, lapTimeList);
                    l.Add(MaxLapTime, max);
                    l.Add(MinLapTime, min);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var nextStates = previousState.States.SetItems(l);

            return new ApplicationState(nextStates, previousState.TimerScheduler);
        }

        private static ApplicationState TimeFormatReducer(ApplicationState previousState, TimeFormatAction action)
        {
            var nextStates = previousState.States.SetItem(DisplayFormat, action.Format);

            return new ApplicationState(nextStates, previousState.TimerScheduler);
        }

        public static ApplicationState ReduceApplication(ApplicationState previousState, IAction action)
        {
            return StopwatchReducer(previousState, action);
        }
    }


}
