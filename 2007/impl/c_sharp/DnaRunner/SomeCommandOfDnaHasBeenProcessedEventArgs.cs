using System;

namespace DnaRunner
{
    /// <summary>
    /// Event args for event of processing some command from DNA.
    /// </summary>
    public class SomeCommandOfDnaHasBeenProcessedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="totalCommandProcessed">Total count of processed commands from RNA.</param>
        public SomeCommandOfDnaHasBeenProcessedEventArgs(int totalCommandProcessed)
        {
            TotalCommandProcessed = totalCommandProcessed;
        }

        /// <summary>
        /// Total count of processed commands from RNA.
        /// </summary>
        public int TotalCommandProcessed { get; private set; }
    }
}