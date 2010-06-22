using System;
using System.IO;

namespace DnaRunner
{
    /// <summary>
    /// DNA processor. Reads input stream and process RNA commands to output stream.
    /// </summary>
    public class DnaRunner
    {
        private string _sourceDna;

        /// <summary>
        /// Constructor of DNA processor.
        /// </summary>
        /// <param name="inputStream">Input stream with source DNA.</param>
        /// <param name="outputStream">Output stream for produced RNA.</param>
        public DnaRunner(Stream inputStream, Stream outputStream)
        {
            if (inputStream.CanSeek)
                inputStream.Seek(0, SeekOrigin.Begin);

            _sourceDna = new StreamReader(inputStream).ReadToEnd();

            RnaStream = outputStream;
        }

        ///<summary>
        /// Stream for produced RNA.
        ///</summary>
        public Stream RnaStream { get; private set; }

        /// <summary>
        /// Start DNA processing.
        /// </summary>
        public void Start()
        {
            InvokeSomeCommandOfDnaHasBeenProcessed(new Random().Next());
            InvokeSomeCharsWrittenToRna(new Random().Next()+1);
        }

        /// <summary>
        /// Stop DNA processing.
        /// </summary>
        public void Stop()
        {
        }

        ///<summary>
        /// Raises when some chars has been written to RNA.
        ///</summary>
        public event EventHandler<SomeCharsWrittenToRnaEventArgs> SomeCharsWrittenToRna;

        private void InvokeSomeCharsWrittenToRna(int totalCharsCount)
        {
            EventHandler<SomeCharsWrittenToRnaEventArgs> handler = SomeCharsWrittenToRna;
            if (handler != null)
                handler(this, new SomeCharsWrittenToRnaEventArgs(totalCharsCount));
        }

        ///<summary>
        /// Raises when some commands of DNA has been processed.
        ///</summary>
        public event EventHandler<SomeCommandOfDnaHasBeenProcessedEventArgs> SomeCommandOfDnaHasBeenProcessed;

        private void InvokeSomeCommandOfDnaHasBeenProcessed(int totalCommandProcessed)
        {
            EventHandler<SomeCommandOfDnaHasBeenProcessedEventArgs> handler = SomeCommandOfDnaHasBeenProcessed;
            if (handler != null)
                handler(this, new SomeCommandOfDnaHasBeenProcessedEventArgs(totalCommandProcessed));
        }
    }
}