using System;

namespace DnaRunner
{
    /// <summary>
    /// Event args for event of writing some chars to RNA.
    /// </summary>
    public class SomeCharsWrittenToRnaEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="totalCharsCount">Total count of chars in RNA.</param>
        public SomeCharsWrittenToRnaEventArgs(int totalCharsCount)
        {
            TotalCharsCount = totalCharsCount;
        }

        /// <summary>
        /// Total count of chars in RNA.
        /// </summary>
        public int TotalCharsCount { get; private set; }
    }
}