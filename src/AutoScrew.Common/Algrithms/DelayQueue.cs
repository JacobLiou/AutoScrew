namespace ProtocolSimulationTest.Common.Algrithms
{
    public class DelayQueue<T> where T : IDelayable
    {
        private readonly object _lock = new object();
        private readonly PriorityQueue<T, long> _queue = new PriorityQueue<T, long>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _cleanupTask;

        public DelayQueue()
        {
            _cleanupTask = Task.Run(Cleanup);
        }

        // 添加元素
        public void Put(T item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item, item.GetDelay());
                Monitor.Pulse(_lock); // 通知可能的等待线程
            }
        }

        // 获取元素（阻塞直到有可用元素）
        public T Take()
        {
            while (true)
            {
                lock (_lock)
                {
                    if (_queue.Count == 0)
                    {
                        Monitor.Wait(_lock);
                        continue;
                    }

                    var item = _queue.Peek();
                    var delay = item.GetDelay();

                    if (delay <= 0)
                    {
                        _queue.Dequeue();
                        return item;
                    }

                    Monitor.Wait(_lock, (int)delay);
                }
            }
        }

        // 清理过期元素的后台任务
        private async Task Cleanup()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                lock (_lock)
                {
                    if (_queue.Count > 0)
                    {
                        var item = _queue.Peek();
                        if (item.GetDelay() <= 0)
                        {
                            Monitor.Pulse(_lock);
                        }
                    }
                }
                await Task.Delay(100, _cts.Token);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cleanupTask.Wait();
            _cts.Dispose();
        }
    }

    // 需要实现的接口
    public interface IDelayable
    {
        long GetDelay(); // 返回剩余延迟时间(毫秒)
    }

    // 示例使用
    public class DelayItem : IDelayable
    {
        private readonly long _triggerTime;
        public string Data { get; }

        public DelayItem(string data, long delayMs)
        {
            Data = data;
            _triggerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + delayMs;
        }

        public long GetDelay()
        {
            return _triggerTime - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}