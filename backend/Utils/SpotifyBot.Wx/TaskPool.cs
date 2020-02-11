using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyBot.Wx
{
    sealed class TaskPool
    {
        readonly SemaphoreSlim _semaphore;

        public TaskPool(int size) => _semaphore = new SemaphoreSlim(size, size);

        public async Task<T> Put<T>(Func<Task<T>> taskProvider)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await Task.Run(taskProvider);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
