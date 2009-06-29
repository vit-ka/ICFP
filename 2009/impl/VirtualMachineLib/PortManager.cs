using System;
using System.Collections;
using System.Collections.Generic;

namespace ICFP2009.VirtualMachineLib
{
    public class PortManager
    {
        public PortManager()
        {
            Input = new PortsCollection();
            Output = new PortsCollection();
        }

        public PortsCollection Input { get; private set; }

        public PortsCollection Output { get; private set; }

        #region Nested type: PortsCollection

        public class PortsCollection : IEnumerable<KeyValuePair<Int16, Double>>
        {
            // ������� �������� I/O ������. ���� --- ����� �����. �������� --- �������� � �����.
            private readonly IDictionary<Int16, Double> _ports;

            public PortsCollection()
            {
                _ports = new Dictionary<short, double>();
            }

            public Double this[Int16 index]
            {
                get
                {
                    return _ports[index];
                }
                set
                {
                    if (!_ports.ContainsKey(index))
                        _ports.Add(index, 0.0);

                    _ports[index] = value;
                }
            }

            public IEnumerator<KeyValuePair<short, double>> GetEnumerator()
            {
                return _ports.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion
    }
}