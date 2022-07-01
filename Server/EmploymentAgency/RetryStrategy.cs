namespace EmploymentAgency;

public class RetryStrategy
{
    private readonly int _maxRetryCount;
    private readonly int _initialDelayMs;
    private readonly int _delayMultiplier;
    private readonly ILogger _logger;

    public RetryStrategy(int maxRetryCount, int initialDelayMs, int delayMultiplier, ILogger logger)
    {
        if (maxRetryCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetryCount));
        }
        if (initialDelayMs < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialDelayMs));
        }
        if (delayMultiplier < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delayMultiplier));
        }

        _maxRetryCount = maxRetryCount;
        _initialDelayMs = initialDelayMs;
        _delayMultiplier = delayMultiplier;
        _logger = logger;
    }

    public async Task ExecuteAsync<E>(Func<Task> callback, Predicate<E> isTransient)
        where E : Exception
    {
        int retry = 1;
        int delayMs = _initialDelayMs;

        for (; ; )
        {
            try
            {
                await callback();

                _logger.LogInformation($"Success{(retry == 1 ? "" : $" on retry #{retry}")}.");
                return;
            }
            catch (E exception) when (isTransient(exception))
            {
                _logger.LogWarning($"Failed on retry #{retry}.");
                retry++;

                if (retry > _maxRetryCount)
                {
                    _logger.LogWarning("No retries left.");
                    throw;
                }

                _logger.LogWarning($"Next retry will be in {delayMs} ms.");
                await Task.Delay(delayMs);
                delayMs *= _delayMultiplier;
            }
        }
    }
}
