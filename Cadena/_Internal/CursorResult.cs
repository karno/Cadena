using System;
using Cadena.Data;

namespace Cadena._Internal
{
    public static class CursorResult
    {
        public static ICursorResult<T> Create<T>(T item, string prevCursor, string nextCursor)
        {
            return Create(item, Int64.Parse(prevCursor), Int64.Parse(nextCursor));
        }

        public static ICursorResult<T> Create<T>(T item, long prevCursor, long nextCursor)
        {
            return new CursorResultImpl<T>(item, prevCursor, nextCursor);
        }

        /// <summary>
        /// Describe cursored result of API Access.
        /// </summary>
        /// <typeparam name="T">Contains result</typeparam>
        private sealed class CursorResultImpl<T> : ICursorResult<T>
        {
            public CursorResultImpl(T result, long previousCursor, long nextCursor)
            {
                Result = result;
                PreviousCursor = previousCursor;
                NextCursor = nextCursor;
            }

            public T Result { get; }

            public long PreviousCursor { get; }

            public long NextCursor { get; }

            public bool CanReadPrevious
            {
                get { return this.PreviousCursor != 0; }
            }

            public bool CanReadNext
            {
                get { return this.NextCursor != 0; }
            }
        }
    }
}
