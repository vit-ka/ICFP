using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using RopeStrings;

namespace DnaRunner
{
    /// <summary>
    /// DNA processor. Reads input stream and process RNA commands to output stream.
    /// </summary>
    public class DnaRunner
    {
        private const int _commandRaiseEventLimit = 1;
        private const int _charsRaiseEventLimit = 50;
        private readonly StreamWriter _rnaWriter;
        private readonly object _runningMutex = new object();
        private int _lastRaisedCharsCountOverEvent;
        private int _lastRaisedCommandsCountOverEvent;

        private RopeString _runningDna;
        private Thread _runningThread;
        private RunningState _state;
        private int _totalCharsOfRna;
        private int _totalCommandProcessed;

        /// <summary>
        /// Constructor of DNA processor.
        /// </summary>
        /// <param name="inputStream">Input stream with source DNA.</param>
        /// <param name="outputStream">Output stream for produced RNA.</param>
        public DnaRunner(Stream inputStream, Stream outputStream)
        {
            if (inputStream.CanSeek)
                inputStream.Seek(0, SeekOrigin.Begin);

            _runningDna = new RopeString(new StreamReader(inputStream).ReadToEnd());

            RnaStream = outputStream;
            _rnaWriter = new StreamWriter(RnaStream);

            _state = RunningState.Stoped;
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
            lock (_runningMutex)
            {
                if (_state == RunningState.Running)
                    return;
                _state = RunningState.Running;
            }

            _runningThread = new Thread(ProcessDna);
            _runningThread.Start();
        }

        /// <summary>
        /// Main thread for processing DNA.
        /// </summary>
        private void ProcessDna()
        {
            bool flag = true;
            while (flag)
            {
                lock (_runningMutex)
                {
                    if (_state != RunningState.Running)
                    {
                        flag = false;
                        continue;
                    }
                }

                ++_totalCommandProcessed;

                PatternInfo pattern = DecodePattern();

                if (pattern == null)
                    continue;

                TemplateInfo template = DecodeTemplate();

                if (template == null)
                    continue;

                MatchReplace(pattern, template);

                // Raise event.
                if (_totalCommandProcessed - _lastRaisedCommandsCountOverEvent >= _commandRaiseEventLimit)
                {
                    InvokeSomeCommandOfDnaHasBeenProcessed(_totalCommandProcessed);
                    _lastRaisedCommandsCountOverEvent = _totalCommandProcessed;
                }
            }

            lock (_runningMutex)
            {
                if (_state != RunningState.Stoped)
                    return;
            }

            InvokeSomeCommandOfDnaHasBeenProcessed(_totalCommandProcessed);
            InvokeSomeCharsWrittenToRna(_totalCharsOfRna);
            InvokeDnaProcessingFinished();
        }

        private void MatchReplace(PatternInfo pattern, TemplateInfo template)
        {
            int index = 0;
            var environment = new List<RopeString>();
            var counters = new List<int>();

            foreach (PatternItemInfo pat in pattern)
            {
                if (pat.IsBase)
                {
                    if (_runningDna[index] == pat.Symbol)
                        ++index;
                    else
                        return;
                    continue;
                }

                if (pat.IsSkip)
                {
                    index += pat.SkipCount;
                    if (index > _runningDna.Length)
                        return;
                    continue;
                }

                if (pat.IsSearch)
                {
                    var patterIndex = _runningDna.IndexOf(pat.SearchPattern, index);
                    if (patterIndex != -1)
                        index = patterIndex + pat.SearchPattern.Length;
                    else
                        return;

                    continue;
                }

                if (pat.IsLevelUp)
                {
                    counters.Insert(0, index);
                    continue;
                }

                if (pat.IsLevelDown)
                {
                    environment.Add(_runningDna.Substring(counters[0], index - counters[0]));
                    counters.RemoveAt(0);
                    continue;
                }
            }

            _runningDna = _runningDna.Substring(index, _runningDna.Length - index);
            Replace(template, environment);
        }

        private void Replace(TemplateInfo template, List<RopeString> environment)
        {
            var newPrefix = new RopeString();

            foreach (TemplateItemInfo temp in template)
            {
                if (temp.IsBase)
                {
                    newPrefix.Append(temp.Symbol);
                    continue;
                }

                if (temp.IsProtect)
                {
                    newPrefix.Append(Protect(temp.Level, environment[temp.Reference]));
                    continue;
                }

                if (temp.IsAsNat)
                {
                    newPrefix.Append(AsNat(environment[temp.Reference].Length));
                    continue;
                }
            }

            _runningDna.AddFirst(newPrefix);
        }

        private static string AsNat(int number)
        {
            if (number == 0)
                return "P";
            if (number % 2 == 0)
                return "I" + AsNat(number / 2);
            return "C" + AsNat(number / 2);
        }

        private static RopeString Protect(int level, RopeString str)
        {
            if (level == 0)
                return str;

            return Protect(level - 1, Quote(str));
        }

        private static RopeString Quote(RopeString str)
        {
            if (str.StartsWith("I"))
            {
                str.RemoveFromBegin(1);
                var str2 = Quote(str);
                str2.AddFirst("C");
                return str2;
            }

            if (str.StartsWith("C"))
            {
                str.RemoveFromBegin(1);
                var str2 = Quote(str);
                str2.AddFirst("F");
                return str2;
            }
                       
            if (str.StartsWith("F"))
            {
                str.RemoveFromBegin(1);
                var str2 = Quote(str);
                str2.AddFirst("P");
                return str2;
            }

            if (str.StartsWith("P"))
            {
                str.RemoveFromBegin(1);
                var str2 = Quote(str);
                str2.AddFirst("IC");
                return str2;
            }

            return new RopeString();
        }

        private TemplateInfo DecodeTemplate()
        {
            var template = new TemplateInfo();

            bool flag = true;
            while (flag)
            {
                if (_runningDna.StartsWith("C"))
                {
                    _runningDna.RemoveFromBegin(1);
                    template.AppendBack('I');
                    continue;
                }

                if (_runningDna.StartsWith("F"))
                {
                    _runningDna.RemoveFromBegin(1);
                    template.AppendBack('C');
                    continue;
                }

                if (_runningDna.StartsWith("P"))
                {
                    _runningDna.RemoveFromBegin(1);
                    template.AppendBack('F');
                    continue;
                }

                if (_runningDna.StartsWith("IC"))
                {
                    _runningDna.RemoveFromBegin(2);
                    template.AppendBack('P');
                    continue;
                }

                if (_runningDna.StartsWith("IF") || _runningDna.StartsWith("IP"))
                {
                    _runningDna.RemoveFromBegin(2);
                    int level = DecodeNumber();
                    int reference = DecodeNumber();
                    template.AddReference(reference, level);
                    continue;
                }

                if (_runningDna.StartsWith("IIC") || _runningDna.StartsWith("IIF"))
                {
                    _runningDna.RemoveFromBegin(3);
                    flag = false;
                    continue;
                }

                if (_runningDna.StartsWith("IIP"))
                {
                    _runningDna.RemoveFromBegin(3);
                    int reference = DecodeNumber();
                    template.AddLengthOfReference(reference);
                    continue;
                }

                if (_runningDna.StartsWith("III"))
                {
                    RopeString toRna = _runningDna.Substring(3, 7);
                    OutputToRna(toRna.ToString());
                    _runningDna.RemoveFromBegin(10);
                    continue;
                }

                // Else stop.
                lock (_runningMutex)
                {
                    _state = RunningState.Stoped;
                    return null;
                }
            }

            return template;
        }

        private PatternInfo DecodePattern()
        {
            var result = new PatternInfo();

            int level = 0;

            bool flag = true;
            while (flag)
            {
                if (_runningDna.StartsWith("C"))
                {
                    _runningDna.RemoveFromBegin(1);
                    result.AppendBack('I');
                    continue;
                }

                if (_runningDna.StartsWith("F"))
                {
                    _runningDna.RemoveFromBegin(1);
                    result.AppendBack('C');
                    continue;
                }

                if (_runningDna.StartsWith("P"))
                {
                    _runningDna.RemoveFromBegin(1);
                    result.AppendBack('F');
                    continue;
                }

                if (_runningDna.StartsWith("IC"))
                {
                    _runningDna.RemoveFromBegin(2);
                    result.AppendBack('P');
                    continue;
                }

                if (_runningDna.StartsWith("IP"))
                {
                    _runningDna.RemoveFromBegin(2);
                    int number = DecodeNumber();
                    result.AppendSkip(number);
                    continue;
                }

                if (_runningDna.StartsWith("IF"))
                {
                    _runningDna.RemoveFromBegin(3);
                    string consts = DecodeConsts();
                    result.AppendSearch(consts);
                    continue;
                }

                if (_runningDna.StartsWith("IIP"))
                {
                    _runningDna.RemoveFromBegin(3);
                    ++level;
                    result.IncreaseLevel();
                    continue;
                }

                if (_runningDna.StartsWith("IIC") || _runningDna.StartsWith("IIF"))
                {
                    _runningDna.RemoveFromBegin(3);
                    if (level == 0)
                    {
                        flag = false;
                        continue;
                    }

                    --level;
                    result.DecreaseLevel();
                    continue;
                }

                if (_runningDna.StartsWith("III"))
                {
                    RopeString toRna = _runningDna.Substring(3, 7);
                    OutputToRna(toRna.ToString());
                    _runningDna.RemoveFromBegin(10);
                    continue;
                }

                // Else stop.
                lock (_runningMutex)
                {
                    _state = RunningState.Stoped;
                    return null;
                }
            }

            return result;
        }

        private string DecodeConsts()
        {
            if (_runningDna.StartsWith("C"))
            {
                _runningDna.RemoveFromBegin(1);
                string consts = DecodeConsts();
                return "I" + consts;
            }

            if (_runningDna.StartsWith("F"))
            {
                _runningDna.RemoveFromBegin(1);
                string consts = DecodeConsts();
                return "C" + consts;
            }

            if (_runningDna.StartsWith("P"))
            {
                _runningDna.RemoveFromBegin(1);
                string consts = DecodeConsts();
                return "F" + consts;
            }

            if (_runningDna.StartsWith("IC"))
            {
                _runningDna.RemoveFromBegin(2);
                string consts = DecodeConsts();
                return "P" + consts;
            }

            return string.Empty;
        }

        private int DecodeNumber()
        {
            if (_runningDna.StartsWith("P"))
            {
                _runningDna.RemoveFromBegin(1);
                return 0;
            }

            if (_runningDna.StartsWith("I") || _runningDna.StartsWith("F"))
            {
                _runningDna.RemoveFromBegin(1);
                int number = DecodeNumber();
                return number * 2;
            }

            if (_runningDna.StartsWith("C"))
            {
                _runningDna.RemoveFromBegin(1);
                int number = DecodeNumber();
                return number * 2 + 1;
            }

            if (_runningDna.Length == 0)
            {
                lock (_runningMutex)
                {
                    _state = RunningState.Stoped;
                }
            }

            return 0;
        }

        private void OutputToRna(string value)
        {
            _rnaWriter.Write(value);
            _totalCharsOfRna += value.Length;

            // Raise event.
            if (_totalCharsOfRna - _lastRaisedCharsCountOverEvent >= _charsRaiseEventLimit)
            {
                InvokeSomeCharsWrittenToRna(_totalCharsOfRna);
                _lastRaisedCharsCountOverEvent = _totalCharsOfRna;
            }
        }

        /// <summary>
        /// Stop DNA processing.
        /// </summary>
        public void Stop()
        {
            lock (_runningMutex)
            {
                if (_state == RunningState.Stoped)
                    return;
                _state = RunningState.Stoped;
            }

            _runningThread.Join();
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

        /// <summary>
        /// Raises when DNA processing has been finished.
        /// </summary>
        public event EventHandler<EventArgs> DnaProcessingFinished;

        private void InvokeDnaProcessingFinished()
        {
            EventHandler<EventArgs> handler = DnaProcessingFinished;
            if (handler != null)
                handler(this, new EventArgs());
        }

        #region Nested type: RunningState

        private enum RunningState
        {
            Running,
            Stoped
        }

        #endregion
    }
}