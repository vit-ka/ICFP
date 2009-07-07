using System;
using System.Collections.Generic;
using System.IO;

namespace ICFP2009.VirtualMachineLib
{
    internal class InstructionManager
    {
        private readonly Int32[] _instructions;
        private Int16 _currentIndex;
        private bool _statusRegister;

        public InstructionManager(List<Int32> instructions)
        {
            _instructions = instructions.ToArray();
        }

        public void RunOneStep()
        {
            MemoryManager memory = VirtualMachine.Instance.Memory;
            PortManager ports = VirtualMachine.Instance.Ports;

            _currentIndex = -1;

            while (_currentIndex + 1 < _instructions.Length)
            {
                ++_currentIndex;

                Int32 currentInstruction = _instructions[_currentIndex];

                // С 31 по 28 биты. 4 бита.
                var opCode = (byte) ((currentInstruction & 0xF0000000) >> 28);

                // Значит S-Type
                if (opCode == 0)
                {
                    // С 27 по 24 биты. 4 бита.
                    var sTypeOpCode = (byte) ((currentInstruction & 0x0F000000) >> 24);

                    // C 13 по 0 биты. 14 бит.
                    var r1 = (Int16) (currentInstruction & 0x00003FFF);

                    switch (sTypeOpCode)
                    {
                            // Noop. Ничего не делаем.
                        case 0x00:
                            //Console.WriteLine("Noop");
                            break;

                            // Cmpz. Операция сравнения.
                        case 0x01:
                            // С 23 по 21 биты. 10 бит.
                            var immediate = (byte) ((currentInstruction & 0x00E00000) >> 21);

                            // Тип сравнения.
                            switch (immediate)
                            {
                                    // LTZ. Меньше чем.
                                case 0x00:
                                    _statusRegister = memory[r1] < 0.0;
                                    break;

                                    // LEZ. Меньше или равно.
                                case 0x01:
                                    _statusRegister = memory[r1] < 0.0 || memory[r1] == 0.0;
                                    break;

                                    // EQZ. Равно.
                                case 0x02:
                                    _statusRegister = memory[r1] == 0;
                                    break;

                                    // GEZ. Больше или равно.
                                case 0x03:
                                    _statusRegister = memory[r1] > 0.0 && memory[r1] == 0.0;
                                    break;

                                    // GTZ. Больше.
                                case 0x04:
                                    _statusRegister = memory[r1] > 0.0;
                                    break;

                                default:
                                    throw new InvalidDataException(
                                        string.Format("Immediate \"0x{0:x}\" does not exists.", immediate));
                            }
                            break;

                            // Sqrt. Квадратный корень.
                        case 0x02:
                            memory[_currentIndex] = Math.Sqrt(memory[r1]);
                            break;

                            // Copy. Копируем из одной области памяти в другую.
                        case 0x03:
                            memory[_currentIndex] = memory[r1];
                            break;

                            // Input. Читаем значение из порта.
                        case 0x04:
                            memory[_currentIndex] = ports.Input[r1];
                            break;

                        default:
                            throw new InvalidDataException(
                                string.Format("S-Type op code \"0x{0:x}\" does not exists.", sTypeOpCode));
                    }
                }
                    // Значит D-Type
                else
                {
                    // C 27 по 14 биты. 14 бит.
                    var r1 = (Int16) ((currentInstruction & 0x0FFFC000) >> 14);

                    // C 13 по 0 биты. 14 бит.
                    var r2 = (Int16)  (currentInstruction & 0x00003FFF);

                    switch (opCode)
                    {
                            // Add. Сложение.
                        case 0x01:
                            memory[_currentIndex] = memory[r1] + memory[r2];
                           // Console.WriteLine("{0} + {1} == {2}", memory[r1], memory[r2], memory[_currentIndex]);
                            break;

                            // Sub. Вычитание.
                        case 0x02:
                            memory[_currentIndex] = memory[r1] - memory[r2];
                            //Console.WriteLine("{0} - {1} == {2}", memory[r1], memory[r2], memory[_currentIndex]);
                            break;

                            // Mult. Умножение.
                        case 0x03:
                            memory[_currentIndex] = memory[r1] * memory[r2];
                            break;

                            // Div. Деление.
                        case 0x04:
                            if (memory[r2] == 0)
                                memory[_currentIndex] = 0.0;
                            else
                                memory[_currentIndex] = memory[r1] / memory[r2];
                            break;

                            // Output. Вывод в порт.
                        case 0x05:
                            ports.Output[r1] = memory[r2];
                            break;

                            // Phi. Что-то страшное.
                        case 0x06:
                            if (_statusRegister)
                                memory[_currentIndex] = memory[r1];
                            else
                                memory[_currentIndex] = memory[r2];
                            break;

                        default:
                            throw new InvalidDataException(
                                string.Format("D-Type op code \"0x{0:x}\" does not exists.", opCode));
                    }
                }
            }
        }
    }
}