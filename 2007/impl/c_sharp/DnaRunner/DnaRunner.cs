using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace DnaRunner
{
    /// <summary>
    /// DNA processor. Reads input stream and process RNA commands to output stream.
    /// </summary>
    public class DnaRunner
    {
        private readonly object _runningMutex = new object();

        private Thread _runningThread;
        private string _runningDna;
        private readonly string _sourceDna;
        private RunningState _state;
        private int _totalCommandProcessed;
        private int _totalCharsOfRna;
        private StreamWriter _rnaWriter;
        private int _lastRaisedCharsCountOverEvent;
        private int _lastRaisedCommandsCountOverEvent;
        private const int _commandRaiseEventLimit = 1;
        private const int _charsRaiseEventLimit = 50;

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

            _state = RunningState.Stoped;
        }

        /// <summary>
        /// Prefix of DNA.
        /// </summary>
        public string Prefix
        {
            get; set;
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

            _rnaWriter = new StreamWriter(RnaStream);
            _runningDna = Prefix + _sourceDna;
            _totalCharsOfRna = 0;
            _totalCommandProcessed = 0;
            _lastRaisedCharsCountOverEvent = 0;
            _lastRaisedCommandsCountOverEvent = 0;

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

                var pattern = DecodePattern();
                
                if (pattern == null)
                {
                    continue;
                }

                var template = DecodeTemplate();

                if (template == null)
                {
                    continue;
                }

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
            var index = 0;
            var environment = new List<string>();
            var counters = new List<int>();

            foreach (var pat in pattern)
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
                    {
                        index = patterIndex + pat.SearchPattern.Length;
                    }
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

            _runningDna = _runningDna.Remove(0, index);
            Replace(template, environment);
        }

        private void Replace(TemplateInfo template, List<string> environment)
        {
            var newPrefix = string.Empty;

            foreach (var temp in template)
            {
                if (temp.IsBase)
                {
                    newPrefix += temp.Symbol;
                    continue;
                }

                if (temp.IsProtect)
                {
                    if (temp.Reference >= environment.Count)
                        continue;

                    newPrefix += Protect(temp.Level, environment[temp.Reference]);
                    continue;
                }

                if (temp.IsAsNat)
                {
                    if (temp.Reference >= environment.Count)
                    {
                        newPrefix += AsNat(0);
                        continue;
                    }

                    newPrefix += AsNat(environment[temp.Reference].Length);
                    continue;
                }
            }

            _runningDna = newPrefix + _runningDna;
        }

        private static string AsNat(int number)
        {
            if (number == 0)
                return "P";
            if (number % 2 == 0)
                return "I" + AsNat(number / 2);
            return "C" + AsNat(number / 2);
        }

        private static string Protect(int level, string str)
        {
            if (level == 0)
                return str;
            
            return Protect(level - 1, Quote(str));
        }

        private static string Quote(string str)
        {
            var result = new StringBuilder();

            for (int index = 0; index < str.Length; ++index )
            {
                switch (str[index])
                {
                    case 'I':
                        result.Append("C");
                        break;
                    case 'C':
                        result.Append("F");
                        break;
                    case 'F':
                        result.Append("P");
                        break;
                    case 'P':
                        result.Append("IC");
                        break;
                }
            }

            return result.ToString();
        }

        private TemplateInfo DecodeTemplate()
        {
            var template = new TemplateInfo();

            bool flag = true;
            while (flag)
            {
                if (_runningDna.StartsWith("C"))
                {
                    _runningDna = _runningDna.Remove(0, 1);
                    template.AppendBack('I');
                    continue;
                }

                if (_runningDna.StartsWith("F"))
                {
                    _runningDna = _runningDna.Remove(0, 1);
                    template.AppendBack('C');
                    continue;
                }

                if (_runningDna.StartsWith("P"))
                {
                    _runningDna = _runningDna.Remove(0, 1);
                    template.AppendBack('F');
                    continue;
                }

                if (_runningDna.StartsWith("IC"))
                {
                    _runningDna = _runningDna.Remove(0, 2);
                    template.AppendBack('P');
                    continue;
                }

                if (_runningDna.StartsWith("IF") || _runningDna.StartsWith("IP"))
                {
                    _runningDna = _runningDna.Remove(0, 2);
                    var level = DecodeNumber();
                    var reference = DecodeNumber();
                    template.AddReference(reference, level);
                    continue;
                }

                if (_runningDna.StartsWith("IIC") || _runningDna.StartsWith("IIF"))
                {
                    _runningDna = _runningDna.Remove(0, 3);
                    flag = false;
                    continue;
                }

                if (_runningDna.StartsWith("IIP"))
                {
                    _runningDna = _runningDna.Remove(0, 3);
                    var reference = DecodeNumber();
                    template.AddLengthOfReference(reference);
                    continue;
                }

                if (_runningDna.StartsWith("III"))
                {
                    string toRna = _runningDna.Substring(3, 7);
                    OutputToRna(toRna);
                    _runningDna = _runningDna.Remove(0, 10);
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
                    _runningDna = _runningDna.Remove(0, 1);
                    result.AppendBack('I');
                    continue;
                }

                if (_runningDna.StartsWith("F"))
                {
                    _runningDna = _runningDna.Remove(0, 1);
                    result.AppendBack('C');
                    continue;
                }

                if (_runningDna.StartsWith("P"))
                {
                    _runningDna = _runningDna.Remove(0, 1);
                    result.AppendBack('F');
                    continue;
                }

                if (_runningDna.StartsWith("IC"))
                {
                    _runningDna = _runningDna.Remove(0, 2);
                    result.AppendBack('P');
                    continue;
                }

                if (_runningDna.StartsWith("IP"))
                {
                    _runningDna = _runningDna.Remove(0, 2);
                    int number = DecodeNumber();
                    result.AppendSkip(number);
                    continue;
                }

                if (_runningDna.StartsWith("IF"))
                {
                    _runningDna = _runningDna.Remove(0, 3);
                    string consts = DecodeConsts();
                    result.AppendSearch(consts);
                    continue;
                }

                if (_runningDna.StartsWith("IIP"))
                {
                    _runningDna = _runningDna.Remove(0, 3);
                    ++level;
                    result.IncreaseLevel();
                    continue;
                }

                if (_runningDna.StartsWith("IIC") || _runningDna.StartsWith("IIF"))
                {
                    _runningDna = _runningDna.Remove(0, 3);
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
                    string toRna = _runningDna.Substring(3, 7);
                    OutputToRna(toRna);
                    _runningDna = _runningDna.Remove(0, 10);
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
                _runningDna = _runningDna.Remove(0, 1);
                var consts = DecodeConsts();
                return "I" + consts;
            }

            if (_runningDna.StartsWith("F"))
            {
                _runningDna = _runningDna.Remove(0, 1);
                var consts = DecodeConsts();
                return "C" + consts;
            }

            if (_runningDna.StartsWith("P"))
            {
                _runningDna = _runningDna.Remove(0, 1);
                var consts = DecodeConsts();
                return "F" + consts;
            }

            if (_runningDna.StartsWith("IC"))
            {
                _runningDna = _runningDna.Remove(0, 2);
                var consts = DecodeConsts();
                return "P" + consts;
            }

            return string.Empty;
        }

        private int DecodeNumber()
        {
            int result = 0;

            int firstPIndex = _runningDna.IndexOf('P');

            if (firstPIndex == -1)
            {
                lock (_runningMutex)
                {
                    _state = RunningState.Stoped;
                }
            }

            for (int index = firstPIndex - 1; index >= 0; --index)
            {
                if (_runningDna[index] == 'I' || _runningDna[index] == 'F')
                {
                    result *= 2;
                }
                else if (_runningDna[index] == 'C')
                {
                    result = result * 2 + 1;
                }
            }

            _runningDna = _runningDna.Remove(0, firstPIndex + 1);

            return result;
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