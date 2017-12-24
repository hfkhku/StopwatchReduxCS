using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace Stopwatch.States
{
    public class ApplicationState
    {
        public ImmutableDictionary<string, object> States { get; }

        public T GetState<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!States.TryGetValue(name, out var value))
            {
                throw new KeyNotFoundException();
            }
            else
            {
                if (!(value is T))
                {
                    throw new InvalidCastException();
                }
                else
                {
                    return (T)value;
                }
            }
        }

        public ApplicationState(IScheduler scheduler)
        {
            States = ImmutableDictionary.Create<string, object>();
            TimerScheduler = scheduler;
        }

        //public ApplicationState():this(Scheduler.Default)
        //{
        //    //States = ImmutableDictionary.Create<string, object>();
        //}

        //public ApplicationState(ImmutableDictionary<string, object> states) : this(states, Scheduler.Default)
        //{
        //    //States = states;
        //}

        public ApplicationState(ImmutableDictionary<string, object> states, IScheduler scheduler) : this(scheduler)
        {
            States = states;
        }

        public IScheduler TimerScheduler { get; }

    }

    public static class ApplicationStateKey
    {
        public static readonly string DisplayFormat = "DisplayFormat";
        public static readonly string NowSpan = "NowSpan";
        public static readonly string Mode = "Mode";
        public static readonly string ButtonLabel = "ButtonLabel";
        public static readonly string StartTime = "StartTime";
        public static readonly string Now = "Now";
        public static readonly string LapTimeList = "LapTimeList";
        public static readonly string MaxLapTime = "MaxLapTime";
        public static readonly string MinLapTime = "MinLapTime";
    }

}

