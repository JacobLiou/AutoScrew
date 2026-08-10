using System.Collections.ObjectModel;
using AutoScrew.Application.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class HistoryDashboardViewModel : ObservableObject
{
    private readonly ILockHistoryQuery _query;
    private readonly ILogger<HistoryDashboardViewModel> _logger;

    public HistoryDashboardViewModel(ILockHistoryQuery query, ILogger<HistoryDashboardViewModel> logger)
    {
        _query = query;
        _logger = logger;
        var today = DateTime.Today;
        FromDate = today;
        ToDate = today;
        ResultFilterOptions = ["", "OK", "NG"];
    }

    public IReadOnlyList<string> ResultFilterOptions { get; }

    [ObservableProperty]
    private DateTime? _fromDate;

    [ObservableProperty]
    private DateTime? _toDate;

    [ObservableProperty]
    private string _serialFilter = "";

    [ObservableProperty]
    private string _partFilter = "";

    [ObservableProperty]
    private string _resultFilter = "";

    [ObservableProperty]
    private int _jobTotal;

    [ObservableProperty]
    private int _jobOk;

    [ObservableProperty]
    private int _jobNg;

    [ObservableProperty]
    private int _screwTotal;

    [ObservableProperty]
    private int _screwOk;

    [ObservableProperty]
    private int _screwNg;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<LockHistoryJobRow> _jobs = [];

    [ObservableProperty]
    private LockHistoryJobRow? _selectedJob;

    [ObservableProperty]
    private ObservableCollection<LockHistoryScrewRow> _screws = [];

    partial void OnSelectedJobChanged(LockHistoryJobRow? value) => _ = LoadScrewsAsync(value);

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        StatusMessage = "";
        try
        {
            var (from, to) = ResolveRange();
            var summary = await _query.GetSummaryAsync(from, to).ConfigureAwait(true);
            JobTotal = summary.JobTotal;
            JobOk = summary.JobOk;
            JobNg = summary.JobNg;
            ScrewTotal = summary.ScrewTotal;
            ScrewOk = summary.ScrewOk;
            ScrewNg = summary.ScrewNg;

            var page = await _query.QueryJobsAsync(new LockHistoryJobFilter(
                from,
                to,
                string.IsNullOrWhiteSpace(SerialFilter) ? null : SerialFilter,
                string.IsNullOrWhiteSpace(PartFilter) ? null : PartFilter,
                string.IsNullOrWhiteSpace(ResultFilter) ? null : ResultFilter,
                Skip: 0,
                Take: 200)).ConfigureAwait(true);

            Jobs = new ObservableCollection<LockHistoryJobRow>(page.Items);
            TotalCount = page.TotalCount;
            SelectedJob = Jobs.FirstOrDefault();
            if (SelectedJob is null)
                Screws = [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "History dashboard refresh failed");
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task TodayAsync()
    {
        var today = DateTime.Today;
        FromDate = today;
        ToDate = today;
        return RefreshAsync();
    }

    private async Task LoadScrewsAsync(LockHistoryJobRow? job)
    {
        if (job is null)
        {
            Screws = [];
            return;
        }

        try
        {
            var list = await _query.GetJobScrewsAsync(job.Id).ConfigureAwait(true);
            Screws = new ObservableCollection<LockHistoryScrewRow>(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Load screws for lock record {Id} failed", job.Id);
            Screws = [];
            StatusMessage = ex.Message;
        }
    }

    private (DateTimeOffset From, DateTimeOffset ToExclusive) ResolveRange()
    {
        var fromLocal = (FromDate ?? DateTime.Today).Date;
        var toLocal = (ToDate ?? DateTime.Today).Date;
        if (toLocal < fromLocal)
            (fromLocal, toLocal) = (toLocal, fromLocal);

        var from = new DateTimeOffset(fromLocal);
        var toExclusive = new DateTimeOffset(toLocal.AddDays(1));
        return (from, toExclusive);
    }
}
