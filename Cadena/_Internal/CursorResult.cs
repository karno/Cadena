using System;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena._Internal
{
    /// <summary>
    /// Create instance of ICursorResult object.
    /// </summary>
    internal static class CursorResult
    {
        public static ICursorResult<T> Create<T>([NotNull] T item,
            [NotNull] string prevCursor, [NotNull] string nextCursor)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (prevCursor == null) throw new ArgumentNullException(nameof(prevCursor));
            if (nextCursor == null) throw new ArgumentNullException(nameof(nextCursor));

            return Create(item, Int64.Parse(prevCursor), Int64.Parse(nextCursor));
        }

        public static ICursorResult<T> Create<T>([NotNull] T item,
            long prevCursor, long nextCursor)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

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
                get { return PreviousCursor != 0; }
            }

            public bool CanReadNext
            {
                get { return NextCursor != 0; }
            }
        }
    }
}
