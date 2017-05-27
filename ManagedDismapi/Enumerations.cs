namespace ManagedDismapi {
    /// <summary>
    /// The logging level used by DISM operations.
    /// </summary>
    public enum LogLevel {
        /// <summary>
        /// Log only errors.
        /// </summary>
        Errors = 0,
        /// <summary>
        /// Log only errors and warnings.
        /// </summary>
        ErrorsWarnings = 1,
        /// <summary>
        /// Log errors, warnings, and additional information.
        /// </summary>
        ErrorsWarningsInfo = 2
    }
}
