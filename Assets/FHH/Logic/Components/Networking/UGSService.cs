using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Foxbyte.Core;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace FHH.Logic.Components.Networking
{
    /// <summary>
    /// Facade for all Unity Gaming Services.
    /// Holds a registry of IUGSModule instances (Relay, Vivox, etc.).
    /// Example usage:
    /// var relay = ServiceLocator.GetService<UGSService>().GetModule<RelayHandler>();
    /// string joinCode = await relay.HostAsync(8, "udp");
    /// var voice = ugs.GetModule<VoiceHandler>();
    /// await voice.JoinChannelAsync("room1");
    /// </summary>
    public sealed class UGSService : IAppServiceAsync
    {
        private readonly Dictionary<Type, IUGSModule> _modules = new();
        private UniTaskCompletionSource<bool> _readyTcs = new();
        private bool _initialized;
        private bool _initializing;
        private Exception _initError;

        /// <summary>
        /// Called from ServiceLocator during startup.
        /// </summary>
        public async UniTask InitServiceAsync()
        {
            if (_initialized) return;
            if (_initializing)
            {
                await _readyTcs.Task;
                return;
            }

            _initializing = true;
            _initError = null;
            _readyTcs = new UniTaskCompletionSource<bool>();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            try
            {
                await InitWithRetryAsync(async ct =>
                {
                    await UnityServices.InitializeAsync().AsUniTask().AttachExternalCancellation(ct);

                    if (!AuthenticationService.Instance.IsSignedIn)
                        await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask().AttachExternalCancellation(ct);

                }, totalBudget: TimeSpan.FromSeconds(25));

                //await UnityServices.InitializeAsync()
                //    .AsUniTask()
                //    .AttachExternalCancellation(cts.Token);

                //if (!AuthenticationService.Instance.IsSignedIn)
                //{
                //    await AuthenticationService.Instance.SignInAnonymouslyAsync()
                //        .AsUniTask()
                //        .AttachExternalCancellation(cts.Token);
                //}

                _initialized = true;
                _readyTcs.TrySetResult(true);
            }
            catch (OperationCanceledException oce) when (cts.IsCancellationRequested)
            {
                var ex = new TimeoutException("UGS initialization timed out.", oce);
                _initError = ex;
                _initialized = false;
                _readyTcs.TrySetException(ex);

                ULog.Error($"UGSService initialization failed: {ex}");
                throw; // back to service locator to mark offline
            }
            catch (Exception ex)
            {
                _initError = ex;
                _initialized = false;
                _readyTcs.TrySetException(ex);

                ULog.Error($"UGSService initialization failed: {ex}");
                throw; // back to service locator to mark offline
            }
            finally
            {
                _initializing = false;
            }
        }

        private async UniTask InitWithRetryAsync(Func<CancellationToken, UniTask> work, TimeSpan totalBudget)
        {
            using var budgetCts = new CancellationTokenSource(totalBudget);

            var delays = new[]
            {
                TimeSpan.FromMilliseconds(250),
                TimeSpan.FromMilliseconds(750),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(4),
            };

            Exception last = null;

            for (int attempt = 0; attempt < delays.Length + 1; attempt++)
            {
                try
                {
                    await work(budgetCts.Token);
                    return;
                }
                catch (OperationCanceledException) when (budgetCts.IsCancellationRequested)
                {
                    ULog.Error($"Operation was canceled due to exceeding the total budget.");
                }
                catch (Exception ex)
                {
                    last = ex;
                    if (attempt >= delays.Length)
                        throw;

                    await UniTask.Delay(delays[attempt], cancellationToken: budgetCts.Token);
                }
            }
        }

        /// <summary>
        /// Optional cleanup method.
        /// </summary>
        public async UniTask DisposeServiceAsync()
        {
            foreach (var module in _modules.Values)
            {
                await module.DisposeAsync();
            }

            _modules.Clear();
            //_readyTcs.TrySetResult(false);
            _initialized = false;
            _initializing = false;
            _initError = null;
            // Make any waiters fail fast rather than hang.
            _readyTcs.TrySetException(new ObjectDisposedException(nameof(UGSService)));
            _readyTcs = new UniTaskCompletionSource<bool>();
            ULog.Info("UGSService dispose finished.");
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Returns a UniTask that completes once InitServiceAsync() has finished.
        /// Any module or game code can await this to ensure UGS is “ready.”
        /// </summary>
        public async UniTask EnsureReadyAsync()
        {
            if (_initialized) return;
            await _readyTcs.Task;
            //return UniTask.FromResult(_readyTcs.Task.IsCompleted);
        }

        /// <summary>
        /// To register a new module before InitServiceAsync is called.
        /// </summary>
        public async UniTask RegisterModuleAsync(IUGSModule module)
        {
            if (!_initialized)
            {
                ULog.Error("UGSService not initialized. Call RegisterModuleAsync() after InitServiceAsync().");
                return;
            }
            var type = module.GetType();
            if (_modules.ContainsKey(type))
            {
                ULog.Error($"Module of type {type.Name} already registered.");
            }

            _modules[type] = module;
            await module.InitAsync(this);
        }

        /// <summary>
        /// Retrieve a registered module by its concrete type.
        /// Usage: var relay = ugsService.GetModule<RelayHandler>();
        /// </summary>
        public T GetModule<T>() where T : IUGSModule
        {
            if (_modules.TryGetValue(typeof(T), out var module))
                return (T)module;
            else return default(T);
            //throw new KeyNotFoundException($"Module {typeof(T).Name} not found. Did you RegisterModule()?");
        }

        /// <summary>
        /// Returns the Player ID of the currently signed-in user.
        /// </summary>
        /// <returns>
        /// string - The Player ID of the signed-in user.
        /// </returns>
        public string GetPlayerId()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
                ULog.Error("AuthenticationService is not signed in. Cannot get Player ID.");
            return AuthenticationService.Instance.PlayerId;
        }
    }
}