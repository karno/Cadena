namespace Cadena.Data
{
    /// <summary>
    /// Get cursored entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICursorResult<out T>
    {
        /// <summary>
        /// Result of cursoring item
        /// </summary>
        T Result { get; }

        /// <summary>
        /// Previous cursoring ID
        /// </summary>
        long PreviousCursor { get; }

        /// <summary>
        /// Next cursoring ID
        /// </summary>
        long NextCursor { get; }

        /// <summary>
        /// Flag of existence of next page
        /// </summary>
        bool CanReadNext { get; }

        /// <summary>
        /// Flag of existence of previous page
        /// </summary>
        bool CanReadPrevious { get; }
    }
}