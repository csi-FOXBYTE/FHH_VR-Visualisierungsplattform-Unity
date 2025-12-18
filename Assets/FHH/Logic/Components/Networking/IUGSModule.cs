using Cysharp.Threading.Tasks;

namespace FHH.Logic.Components.Networking
{
    /// <summary>
    /// Every UGS‐related sub‐service (Relay, Vivox, etc.) implements this.
    /// </summary>
    public interface IUGSModule
    {
        /// <summary>
        /// Called once UGSService has finished UnityServices.InitializeAsync()
        /// and (anonymous) Authentication. 
        /// </summary>
        UniTask InitAsync(UGSService context);

        /// <summary>
        /// Optional cleanup
        /// </summary>
        UniTask DisposeAsync();
    }
}