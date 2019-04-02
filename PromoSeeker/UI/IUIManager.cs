using System.Threading.Tasks;

namespace PromoSeeker
{
    public interface IUIManager
    {
        Task MessageAsync(DialogWindowViewModel viewModel);
    }
}
