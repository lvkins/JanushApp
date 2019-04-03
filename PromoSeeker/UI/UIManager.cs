using System.Threading.Tasks;
using System.Windows;

namespace PromoSeeker
{
    public class UIManager : IUIManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task MessageAsync(DialogWindowViewModel viewModel)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return new PromptDialog().ShowDialog(viewModel);
            });
        }
    }
}