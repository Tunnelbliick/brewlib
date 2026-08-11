using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Brewlib.Util
{
    public class ValueWindow<TValue> : ICollection<TValue>
        where TValue : INumber<TValue>
    {
        public int Count { get; private set; }
        public bool IsFull => Count >= data.Length;
        public bool IsReadOnly => false;

        private readonly TValue[] data;
        private readonly TValue defaultValue;
        private int headIndex = -1;

        public ValueWindow(int length, TValue defaultValue = default)
        {
            data = new TValue[length];
            this.defaultValue = defaultValue;
        }

        public TValue Last()
        {
            if (headIndex < 0)
                return defaultValue;
            return data[headIndex];
        }

        public double GetAverage()
        {
            if (Count == 0)
                return Convert.ToDouble(defaultValue);

            var sum = 0.0;
            foreach (var v in data.Take(Count))
                sum += Convert.ToDouble(v);
            return sum / Count;
        }

        public double GetVariance()
        {
            if (Count < 2)
                return 0;

            var average = GetAverage();
            var delta = 0.0;
            foreach (var v in data.Take(Count))
            {
                var diff = Convert.ToDouble(v) - average;
                delta += diff * diff;
            }
            return delta / Count;
        }

        public TValue GetMedian()
        {
            if (Count == 0)
                return defaultValue;

            var sortedData = data.Take(Count).Order().ToArray();

            var halfIndex = sortedData.Length / 2;
            return sortedData.Length % 2 == 1 ? sortedData[halfIndex] : (sortedData[halfIndex - 1] + sortedData[halfIndex]) / TValue.CreateChecked(2);
        }

        #region Collection

        public void Add(TValue value)
        {
            headIndex = (headIndex + 1) % data.Length;
            data[headIndex] = value;
            if (Count < data.Length)
                Count++;
        }

        public bool Remove(TValue item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            headIndex = -1;
            Count = 0;
        }

        public bool Contains(TValue item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            if (Count > 0)
                for (var i = 0; i < Count; i++)
                    yield return data[i];
            else yield return defaultValue;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}