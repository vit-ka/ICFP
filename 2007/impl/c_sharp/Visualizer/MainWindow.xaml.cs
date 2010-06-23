using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
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

        /// <summary>
        /// Main windows of application.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunDnaButtonClick(object sender, RoutedEventArgs e)
        {
            _dnaRunner.Prefix = _prefixTextBox.Text;
            _dnaRunner.Start();
        }


        private void RunRnaButtonClick(object sender, RoutedEventArgs e)
        {
            _rnaRunner.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dnaRunner =
                new DnaRunner.DnaRunner(
                    new FileStream(
                        Settings.Default.PathToEndoDNAFile.Trim(), FileMode.Open, FileAccess.Read, FileShare.None),
                    new FileStream(
                        "D:\\OutRNA.txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite));

            _dnaRunner.SomeCharsWrittenToRna += DnaRunnerSomeCharsWrittenToRna;
            _dnaRunner.SomeCommandOfDnaHasBeenProcessed += DnaRunnerSomeCommandOfDnaHasBeenProcessed;
            _dnaRunner.DnaProcessingFinished += DnaRunnerDnaProcessingFinished;

            _rnaRunner = new RnaRunner.RnaRunner(new FileStream(
                        @"D:\Projects\ICFP\2007\source\Rna\RNA_WithDefaultPrefix.txt", FileMode.Open, FileAccess.Read, FileShare.None));
            _rnaRunner.SomeDrawCommandsExecuted += RnaRunnerSomeDrawCommandsExecuted;
            _rnaRunner.ExecutionFinished += RnaRunnerExecutionFinished;

            _canvas.RnaRunner = _rnaRunner;
        }

        void RnaRunnerExecutionFinished(object sender, EventArgs e)
        {
            DrawBitmap();
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        const string message = "Обработка РНК завершена.";
                        _logListBox.Items.Add(message);
                        _logListBox.ScrollIntoView(message);
                    }));
        }

        private static DispatcherOperationCallback exitFrameCallback = new
                            DispatcherOperationCallback(ExitFrame);

        /// <summary>
        /// Processes all UI messages currently in the message queue.
        /// </summary>
        public static void DoEvents()
        {

            // Create new nested message pump.
            DispatcherFrame nestedFrame = new DispatcherFrame();



            // Dispatch a callback to the current message queue, when getting called, 
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than the that of UI event messages.
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
                                                  DispatcherPriority.Background, exitFrameCallback, nestedFrame);



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
                        ++_index;
                        _canvas.InvalidateVisual();
                        DoEvents();
                    }));
        }

        void RnaRunnerSomeDrawCommandsExecuted(object sender, EventArgs e)
        {
            DrawBitmap();
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                         string message = _index + ": Изменения в изображении.";
                        _logListBox.Items.Add(message);
                        _logListBox.ScrollIntoView(message);
                    }));
        }

        private void DnaRunnerDnaProcessingFinished(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        const string message = "Обработка ДНК завершена.";
                        _logListBox.Items.Add(message);
                        _logListBox.ScrollIntoView(message);
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
                        string message = string.Format("Обработано {0} команд ДНК.", e.TotalCommandProcessed);
                        _logListBox.Items.Add(message);
                        _logListBox.ScrollIntoView(message);
                    }));
        }

        private void DnaRunnerSomeCharsWrittenToRna(object sender, SomeCharsWrittenToRnaEventArgs e)
        {
            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new VoidDelegate(
                    () =>
                    {
                        string message = string.Format("Произведено {0} символов РНК.", e.TotalCharsCount);
                        _logListBox.Items.Add(message);
                        _logListBox.ScrollIntoView(message);
                    }));
        }

    }
}