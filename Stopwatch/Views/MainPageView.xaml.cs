using Stopwatch.Actions;
using Stopwatch.Models;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static Stopwatch.States.ApplicationStateKey;

namespace Stopwatch.Views
{
    /// <inheritdoc />
    /// <summary>
    /// MainPageView.xaml の相互作用ロジック
    /// </summary>
    public partial class MainPageView : Page
    {
        private IDisposable _timerSubscription;
        public MainPageView()
        {
            InitializeComponent();

            App.Store.ObserveOnDispatcher().Subscribe(state =>
           {
                //Debug.WriteLine($"NowSpan:{state.GetState<TimeSpan>(NowSpan)}");
                txtNowSpan.Text = state.GetState<TimeSpan>(NowSpan).ToString(state.GetState<string>(DisplayFormat));
               btnStartStopReset.Content = state.GetState<string>(ButtonLabel);
               btnLap.IsEnabled = state.GetState<StopwatchMode>(Mode) == StopwatchMode.Start;
               lvLap.ItemsSource = state.GetState<ObservableCollection<LapTime>>(LapTimeList);
               if (state.GetState<StopwatchMode>(Mode) != StopwatchMode.Stop) return;
               var nowSpan = state.GetState<TimeSpan>(NowSpan);
               var maxLapTime = state.GetState<TimeSpan>(MaxLapTime);
               var minLapTime = state.GetState<TimeSpan>(MinLapTime);

               var r = MessageBox.Show($"All time: {nowSpan.ToString(state.GetState<string>(DisplayFormat))}\r\nMax laptime: {maxLapTime.TotalMilliseconds} ms\nMin laptime: { minLapTime.TotalMilliseconds}ms\n\nShow all lap result?", "Confirmation", MessageBoxButton.OKCancel);

               if (r != MessageBoxResult.OK) return;
               var w = Application.Current.MainWindow as NavigationWindow;
               w.Source = new Uri("ResultPageView.xaml", UriKind.Relative);
           });

            //表示切替チェックボックス
            chbIsShowed.Events().Checked.Subscribe(_ => App.Store.Dispatch(new TimeFormatAction() { Format = Constants.TimeSpanFormat }));
            chbIsShowed.Events().Unchecked.Subscribe(_ => App.Store.Dispatch(new TimeFormatAction() { Format = Constants.TimeSpanFormatNoMillsecond }));

            //start,stop,resetボタン
            btnStartStopReset.Events().Click.Subscribe(e =>
            {
                var mode = App.Store.GetState().GetState<StopwatchMode>(Mode);
                var scheduler = App.Store.GetState().TimerScheduler;
                switch (mode)
                {
                    case StopwatchMode.Init:
                        _timerSubscription = Observable.Interval(TimeSpan.FromMilliseconds(10), scheduler)
                            .Subscribe(_ =>
                            {
                                App.Store.Dispatch(new TimerAction() { Now = scheduler.Now.DateTime.ToLocalTime() });
                            });
                        break;
                    case StopwatchMode.Start:
                        _timerSubscription.Dispose();
                        _timerSubscription = null;
                        break;
                    case StopwatchMode.Stop:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                App.Store.Dispatch(new ChangeModeAction());

            });

            //lapボタン
            btnLap.Events().Click.Subscribe(_ =>
            {
                App.Store.Dispatch(new LapAction());
            });

        }
    }
}
