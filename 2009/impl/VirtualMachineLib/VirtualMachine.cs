using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace ICFP2009.VirtualMachineLib
{
    public class VirtualMachine
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (VirtualMachine));
        private static readonly object _virtualMachineMutex = new object();
        private static VirtualMachine _instance;

        private InstructionManager _instructionManager;

        /// <summary>
        /// Чтобы никто не уволок.
        /// </summary>
        private VirtualMachine()
        {
        }

        public static VirtualMachine Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_virtualMachineMutex)
                        if (_instance == null)
                            _instance = new VirtualMachine();
                }

                return _instance;
            }
        }

        public void LoadBinary(Stream binaryStream)
        {
            var instructions = new List<int>();
            var initialMemory = new List<double>();

            var binaryReader = new BinaryReader(binaryStream);

            _log.InfoFormat("Reading {0} bytes as image file...", binaryStream.Length);

            // Читаем тут данные из файла
            int frameIndex = 0;
            BinaryFrame frame = ReadFrame(binaryReader, frameIndex);
            while (frame != null)
            {
                instructions.Add(frame.Instruction);
                initialMemory.Add(frame.Memory);

                frame = ReadFrame(binaryReader, ++frameIndex);
            }

            _log.InfoFormat("Reader {0} frames.", frameIndex);

            _instructionManager = new InstructionManager(instructions);
            Memory = new MemoryManager(initialMemory);
            Ports = new PortManager();
        }

        internal MemoryManager Memory { get; private set; }
        public PortManager Ports { get; private set; }

        public void RunOneStep()
        {
            _instructionManager.RunOneStep();
        }

        private static BinaryFrame ReadFrame(BinaryReader binaryReader, int frameIndex)
        {
            var frame = new BinaryFrame();

            if (binaryReader.BaseStream.Position >= binaryReader.BaseStream.Length)
                return null;

            // Если четный индекс, то сначала идет значение памяти, а потом инструкция кода
            // иначе наоборот.
            if (frameIndex % 2 == 0)
            {
                frame.Memory = binaryReader.ReadInt64();
                frame.Instruction = binaryReader.ReadInt32();
            }
            else
            {
                frame.Instruction = binaryReader.ReadInt32();
                frame.Memory = binaryReader.ReadInt64();
            }

            return frame;
        }

        #region Nested type: BinaryFrame

        private class BinaryFrame
        {
            public int Instruction { get; set; }

            public double Memory { get; set; }
        }

        #endregion
    }
}