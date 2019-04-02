using System.Threading.Tasks;
using System.Windows;

namespace PromoSeeker
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        public Task ShowDialog<T>(T viewModel)
            where T : DialogWindowViewModel
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                DataContext = viewModel;
                ShowDialog();
            }
            finally
            {
                // Inform caller we've finished
                tcs.TrySetResult(true);
            }

            return tcs.Task;
        }
    }
}
