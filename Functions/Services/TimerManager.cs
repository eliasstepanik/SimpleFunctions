using System.Net;
using System.Timers;
using Functions.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Timer = System.Timers.Timer;

namespace Functions.Services;
public class JobExecutedEventArgs : EventArgs {}

public class TimerManager : IDisposable
{
    private readonly ILoadManager _loadManager;

    public TimerManager(ILoadManager loadManager)
    {
        _loadManager = loadManager;
    }

    public event EventHandler<JobExecutedEventArgs> JobExecuted;

    void OnJobExecuted()
    {
        JobExecuted?.Invoke(this, new JobExecutedEventArgs());
    }

    System.Timers.Timer _timer;
    bool _running;

    public void StartExecuting()
    {
        if (!_running)
        {
            //TimerLogic();
            //initiate a timer
            _timer = new Timer();
            _timer.Interval = 1000;
            //_Timer.Interval = 60000;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += HandleTimer;
            _running = true;
        }
    }

    void HandleTimer(object source, ElapsedEventArgs e)
    {
        _loadManager.Update();

        //Execute required job
        //notify any subscibers to the event
        OnJobExecuted();
    }

    public void Dispose()
    {
        if (_running)
        {
            //clear up the timer
            _timer.Dispose();
        }
    }
}