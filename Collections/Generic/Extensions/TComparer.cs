using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Collections.Generic.Extensions
{
    public class TComparer
    {
        public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equalityFunc)
        {
            if (equalityFunc == null)
                throw new NotImplementedException();
            return new PComparer<T>(equalityFunc, null);
        }
        public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equalityFunc, Func<T, int> getHashCodeFunc)
        {
            if (equalityFunc == null)
                throw new NotImplementedException();
            return new PComparer<T>(equalityFunc, getHashCodeFunc);
        }
        private class PComparer<T> : IEqualityComparer<T>
        {
            readonly Func<T, int> _getHashCodeFunc;
            readonly Func<T, T, bool> _equalityFunc;
            public PComparer(Func<T, T, bool> equalityFunc, Func<T, int> getHashCodeFunc)
            {
                _equalityFunc = equalityFunc;
                _getHashCodeFunc = getHashCodeFunc;
            }
            public bool Equals(T x, T y)
            {
                return _equalityFunc.Invoke(x, y);
            }

            public int GetHashCode(T obj)
            {
                if (_getHashCodeFunc == null)
                    return base.GetHashCode();
                return _getHashCodeFunc.Invoke(obj);
            }
        }
    }
}
