using Redux;
using Stopwatch.Models;
using Stopwatch.States;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Immutable;
using static Stopwatch.States.ApplicationStateKey;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace Stopwatch
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IStore<ApplicationState> Store { get; private set; }

        private readonly IScheduler _scheduler = Scheduler.Default;
        public App()
        {
            var states = ImmutableDictionary.CreateRange(
                new Dictionary<string, object>()
                {
                    { DisplayFormat, Constants.TimeSpanFormatNoMillsecond },
                    { NowSpan, TimeSpan.Zero },
                    { Mode, StopwatchMode.Init },
                    { ButtonLabel, Constants.StartLabel },
                    { StartTime, new DateTime() },
                    { Now, new DateTime() },
                    { LapTimeList, new ObservableCollection<LapTime>() },
                    { MaxLapTime, TimeSpan.Zero },
                    { MinLapTime, TimeSpan.Zero },
                });
            var initialState = new ApplicationState(states, _scheduler);

            Store = new Store<ApplicationState>(Reducers.ReduceApplication, initialState);

        }
    }
}
