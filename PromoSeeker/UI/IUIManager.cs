using System.Threading.Tasks;

namespace PromoSeeker
{
    public interface IUIManager
    {
        /// <summary>
        /// Initializes the UI components.
        /// </summary>
        void Initialize();

        Task ShowMessageBoxAsync(MessageDialogViewModel viewModel);

        Task<string> ShowPromptMessageBoxAsync(PromptDialogViewModel viewModel);
    }
}
