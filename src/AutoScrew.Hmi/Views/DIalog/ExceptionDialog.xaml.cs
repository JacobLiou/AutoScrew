using System.Reflection;
using System.Windows;

namespace AutoScrew.Hmi.Dialog
{
    /// <summary>
    /// ErrorDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ExceptionDialog
    {
        // private static readonly SynchronizationContext _syncContext = Application.Current.Dispatcher;

        public List<KeyValueInfo> Details { get; }

        public ExceptionDialog(Exception ex)
        {
            InitializeComponent();

            Details = new(128);

            CascadeError(ex, 1).ForEach(e =>
            {
                e.GetType()
                    .GetRuntimeProperties()
                    .ToList()
                    .ForEach(p =>
                    {
                        var value = p.GetIndexParameters().Length == 0 ? p.GetValue(e)?.ToString() : null;
                        Details.Add(new() { Key = p.Name, Value = value });
                    });
            });

            DataContext = this;
        }

        private List<Exception> CascadeError(Exception ex, int cascde)
        {
            Details.Add(new() { Key = "Message", Value = ex.Message });
            // Details.Add(new { Key = "Message", Value = ex.Message });
            var errs = new List<Exception>();
            var index = ex;
            var level = 0;
            do
            {
                errs.Add(index);
                index = index.InnerException;
            } while (++level < cascde && index != null);
            return errs;
        }

        public static void HandleException(Exception ex)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var dialog = new ExceptionDialog(ex);
                dialog.Owner = System.Windows.Application.Current.MainWindow;
                dialog.ShowDialog();
            });
            // _syncContext.Post(pl =>
            // {
            //     var dialog = new ExceptionDialog(ex);
            //     dialog.Owner = Application.Current.MainWindow;
            //     dialog.ShowDialog();
            // }, null);
        }
    }

    public class KeyValueInfo
    {
        public string? Key { get; set; } = "";

        public string? Value { get; set; } = "";
    }
}