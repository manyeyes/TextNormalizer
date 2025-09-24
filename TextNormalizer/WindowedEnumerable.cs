namespace TextNormalizer
{
    public class WindowedEnumerable<T> : IEnumerable<IEnumerable<T>>
    {
        private readonly IEnumerable<T> _source;
        private readonly int _size;
        private readonly T _fillValue;

        public WindowedEnumerable(IEnumerable<T> source, int size, T fillValue = default(T))
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");

            _source = source;
            _size = size;
            _fillValue = fillValue;
        }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            var buffer = new List<T>(_size);

            foreach (var item in _source)
            {
                buffer.Add(item);

                if (buffer.Count == _size)
                {
                    yield return buffer.ToList(); // 返回当前窗口的副本  
                    buffer.Clear(); // 清空缓冲区以准备下一个窗口  
                }
            }

            // 处理最后一个不完整的窗口（如果需要填充）  
            if (buffer.Any())
            {
                while (buffer.Count < _size)
                {
                    buffer.Add(_fillValue);
                }
                yield return buffer.ToList();
            }
            else if (buffer.Any()) // 如果不填充且存在剩余元素，也返回它们  
            {
                yield return buffer.ToList();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
