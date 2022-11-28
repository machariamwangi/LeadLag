using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    class Program
    {
        void Main()
        {
            var l = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            var p = new List<IEnumerable<int>>() { l.Take(1), l.Take(2), l.Take(3) };
            l.extns.AsLLEStruct().Dump();
            p.extns.AstLLERef().Dump();
        }

        public static class extns
        {
            public static LagLeadEnumerable<Nullable<T>> AsLLEStruct<T>( IEnumerable<T> enumerable) where T : struct
            {
                return new LagLeadEnumerable<Nullable<T>>(enumerable.Cast<T?>());
            }

            public static LagLeadEnumerable<T> AstLLERef<T>( IEnumerable<T> enumerable) where T : class
            {
                return new LagLeadEnumerable<T>(enumerable);
            }
        }


        public class LagLeadEnumerable<T> : IEnumerable<ILagLeader<T>>
        {
            IEnumerable<T> _enumer;

            public LagLeadEnumerable(IEnumerable<T> enumerable)
            {
                _enumer = enumerable;
            }

            public IEnumerator<ILagLeader<T>> GetEnumerator()
            {
                return new LagLeadEnumerator<T>(_enumer.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class LagLeadEnumerator<T> : IEnumerator<LagLeadEnumerator<T>>, ILagLeader<T>
        {
            IEnumerator<T> _enumerator;
            List<object> _history = new List<object>();
            bool _lastMove = false;
            bool _atEnd = false;

            public LagLeadEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
                Init();
            }

            object IEnumerator.Current
            {
                get
                {
                    return this;
                }
            }

            LagLeadEnumerator<T> IEnumerator<LagLeadEnumerator<T>>.Current
            {
                get
                {
                    return this;
                }
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                _history.Add(_enumerator.Current);
                _lastMove = _atEnd;
                if (!_lastMove)
                {
                    _atEnd = !_enumerator.MoveNext();
                }
                return !(_atEnd & _lastMove);
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            void Init()
            {
                _history.Clear();
                _enumerator.MoveNext();
                _history.Add(null);
                _history.Add(null);
            }

            public T Lead
            {
                get
                {
                    if (!_atEnd)
                    {
                        return _enumerator.Current;
                    }
                    return default(T);
                }
            }

            public T Lag
            {
                get
                {
                    return (T)_history[_history.Count - 2];
                }
            }


            public T Current
            {
                get
                {
                    return (T)_history[_history.Count - 1];
                }
            }

            T ILagLeader<T>.Lead => throw new NotImplementedException();

            T ILagLeader<T>.Lag => throw new NotImplementedException();
        }

        public interface ILagLeader<T>
        {
            T Lead { get; }
            T Current { get; }
            T Lag { get; }
        }
    }
}
