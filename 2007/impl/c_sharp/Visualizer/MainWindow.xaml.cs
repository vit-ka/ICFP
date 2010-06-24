using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using DnaRunner;
using Visualizer.Properties;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DnaRunner.DnaRunner _dnaRunner;
        private RnaRunner.RnaRunner _rnaRunner;
        private int _drawCoeficient;
        private bool _waitOnImportantCommands;

        /// <summary>
        /// Main windows of application.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunDnaButtonClick(object sender, RoutedEventArgs e)
        {
            _dnaRunner =
                new DnaRunner.DnaRunner(
                    new FileStream(
                        Settings.Default.PathToEndoDNAFile.Trim(), FileMode.Open, FileAccess.Read, FileShare.None),
                    new FileStream(
                        "D:\\RNA_"+_prefixTextBox.Text+".txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite));

            _dnaRunner.SomeCharsWrittenToRna += DnaRunnerSomeCharsWrittenToRna;
            _dnaRunner.SomeCommandOfDnaHasBeenProcessed += DnaRunnerSomeCommandOfDnaHasBeenProcessed;
            _dnaRunner.DnaProcessingFinished += DnaRunnerDnaProcessingFinished;

            _dnaRunner.Prefix = _prefixTextBox.Text;
            _dnaRunner.Start();
        }


        private void RunRnaButtonClick(object sender, RoutedEventArgs e)
        {
            var fileInfo = _rnaFileComboBox.SelectedItem as string;

            if (fileInfo == null)
                return;

            _drawCoeficient = (int) _drawCoefficientSlider.Value;
            _waitOnImportantCommands = _waitOnImportantDrawCommandCheckBox.IsChecked.Value;

            _rnaRunner = new RnaRunner.RnaRunner(new FileStream(fileInfo, FileMode.Open, FileAccess.Read, FileShare.None));
            _rnaRunner.SomeDrawCommandsExecuted += RnaRunnerSomeDrawCommandsExecuted;
            _rnaRunner.ExecutionFinished += RnaRunnerExecutionFinished;
            _rnaRunner.AfterImportantDrawCommand += RnaRunnerAfterImportantDrawCommand;
            _rnaRunner.BeforeImportantDrawCommand += RnaRunnerBeforeImportantDrawCommand;

            _rnaRunner.Start();
        }

        void RnaRunnerBeforeImportantDrawCommand(object sender, EventArgs e)
        {
            if (_waitOnImportantCommands)
            {
                DrawBitmap();
                Thread.Sleep(500);
            }
        }

        void RnaRunnerAfterImportantDrawCommand(object sender, EventArgs e)
        {
            if (_waitOnImportantCommands)
            {
                DrawBitmap();
                Thread.Sleep(500);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var files = Directory.GetFiles(
                Settings.Default.PathToRnaFolder.Trim(), "*.rna", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                _rnaFileComboBox.Items.Add(file);
            }
        }

        void RnaRunnerExecutionFinished(object sender, EventArgs e)
        {
            DrawBitmap();
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        _rnaProcessingFinishedLabel.Content = "Отрисовка РНК завершена";
                    }));
        }

        private static readonly DispatcherOperationCallback _exitFrameCallback = ExitFrame;

        /// <summary>
        /// Processes all UI messages currently in the message queue.
        /// </summary>
        public static void DoEvents()
        {

            // Create new nested message pump.
            var nestedFrame = new DispatcherFrame();


            // Dispatch a callback to the current message queue, when getting called, 
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than the that of UI event messages.
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
                                                  DispatcherPriority.Background, _exitFrameCallback, nestedFrame);


            // pump the nested message loop, the nested message loop will 
            // immediately process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);


            // If the "exitFrame" callback doesn't get finished, Abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;

            // Exit the nested message loop.
            frame.Continue = false;
            return null;
        }

        private int _index;
        private void DrawBitmap()
        {
            Dispatcher.Invoke(
                new VoidDelegate(
                    () =>
                    {
                            _image.Source = BitmapConverter.Convert(_rnaRunner.PixelMap);
                            //_canvas.InvalidateVisual();
                            DoEvents();
                    }));
        }

        void RnaRunnerSomeDrawCommandsExecuted(object sender, EventArgs e)
        {
            ++_index;
            if (_index % _drawCoeficient == 0)
                DrawBitmap();
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        _processedRnaCommandLabel.Content = _index;
                    }));
        }

        private void DnaRunnerDnaProcessingFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        _dnaProcessingFinishedLabel.Content = "Обработка ДНК завершена.";
                    }));
        }

        private void DnaRunnerSomeCommandOfDnaHasBeenProcessed(object sender,
                                                               SomeCommandOfDnaHasBeenProcessedEventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        _dnaCommandsProcessedLabel.Content = e.TotalCommandProcessed;
                    }));
        }

        private void DnaRunnerSomeCharsWrittenToRna(object sender, SomeCharsWrittenToRnaEventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        _rnaCharsWrittenLabel.Content = e.TotalCharsCount;
                    }));
        }
    }
}