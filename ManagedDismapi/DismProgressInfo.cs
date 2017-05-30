namespace ManagedDismapi {
    /// <summary>
    /// The progress information passed during a DISM operation.
    /// </summary>
    public sealed class DismProgressInfo {
        internal DismProgressInfo(uint current, uint total) {
            Current = current;
            Total = total;
        }

        /// <summary>
        /// The current progress value.
        /// </summary>
        public uint Current { get; }

        /// <summary>
        /// The total progress value.
        /// </summary>
        public uint Total { get; }
    }
}
