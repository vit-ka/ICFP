using System;
using System.Collections.Generic;
using Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Common_UT
{
    [TestFixture]
    public class RoapList_UT
    {
        [Test]
        public void Add_UT()
        {
            var list = new RoapList<int>();

            Assert.AreEqual(0, list.Count);

            list.Add(12);
            list.Add(12);
            list.Add(13);
            list.Add(14);
            Assert.AreEqual(4, list.Count);

            list.Add(16);
            Assert.AreEqual(5, list.Count);

            list.Add(int.MaxValue);
            Assert.AreEqual(6, list.Count);

            list.Add(int.MinValue);
            Assert.AreEqual(7, list.Count);

            const int cycleCount = 1000000;
            for (int i = 0; i < cycleCount; ++i)
                list.Add(i);

            Assert.AreEqual(7 + cycleCount, list.Count);
        }

        [Test]
        public void Add_UT2()
        {
            var list = new RoapList<int>();
            var expectedList = new List<int>();

            const int cycleCount = 1000000;
            for (int i = 0; i < cycleCount; ++i)
            {
                list.Add(i);
                expectedList.Add(i);
            }

            Assert.AreEqual(cycleCount, list.Count);
            Assert.That(list, Is.Unique);
            Assert.That(list, Is.EquivalentTo(expectedList));
        }

        [Test]
        public void AddRange_UT()
        {
            var list = new RoapList<int>();
            var expectedList = new List<int>();

            const int cycleCount = 1000000;
            for (int i = 0; i < cycleCount; ++i)
                expectedList.Add(i);

            list.AddRange(expectedList);

            Assert.AreEqual(cycleCount, list.Count);
            Assert.That(list, Is.Unique);
            Assert.That(list, Is.EquivalentTo(expectedList));
        }

        [Test]
        public void Indexer_UT()
        {
            var list = new RoapList<int>();
            list.AddRange(new[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
                });

            Assert.AreEqual(5, list[6]);
            Assert.AreEqual(10, list[11]);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(20, list[19]);

            try
            {
                int a = list[-1];
                Assert.Fail("Indexer with index lesser left bound must throws exception");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (IndexOutOfRangeException), ex);
            }

            try
            {
                int a = list[20];
                Assert.Fail("Indexer with index greater right bound must throws exception");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (IndexOutOfRangeException), ex);
            }
        }

        [Test]
        public void Indexer_UT2()
        {
            var list = new RoapList<int>();
            list.AddRange(new[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
                });

            list[6] = 21;
            Assert.AreEqual(21, list[6]);

            list[0] = 47;
            Assert.AreEqual(47, list[0]);

            list[19] = 88;
            Assert.AreEqual(88, list[19]);

            list[12] = -3;
            Assert.AreEqual(-3, list[12]);

            try
            {
                list[-1] = 2;
                Assert.Fail("Indexer with index lesser left bound must throws exception");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (IndexOutOfRangeException), ex);
            }

            try
            {
                list[20] = 2;
                Assert.Fail("Indexer with index greater right bound must throws exception");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (IndexOutOfRangeException), ex);
            }
        }

        [Test]
        public void RemoveAt_UT()
        {
            var list = new RoapList<int>();
            list.AddRange(new[]
                {
                    1, 2, 3, 4, 5, 6
                });

            list.RemoveAt(3);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                1, 2, 3, 5, 6
                            }));

            list.RemoveAt(0);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                2, 3, 5, 6
                            }));

            list.RemoveAt(4);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                2, 3, 5
                            }));

            try
            {
                list.RemoveAt(4);
                Assert.Fail("Method must be thrown exception while processed index out of right bound.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (IndexOutOfRangeException), ex);
            }

            try
            {
                list.RemoveAt(-1);
                Assert.Fail("Method must be thrown exception while processed index out of left bound.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (IndexOutOfRangeException), ex);
            }
        }

        [Test]
        public void RemoveRange_UT()
        {
            var list = new RoapList<int>();
            list.AddRange(new[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
                });

            list.RemoveRange(0, 0);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
                            }));

            list.RemoveRange(0, 1);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
                            }));

            list.RemoveRange(0, 2);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
                            }));

            list.RemoveRange(5, 3);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                4, 5, 6, 7, 8, 12, 13, 14, 15, 16, 17, 18, 19, 20
                            }));

            list.RemoveRange(13, 5);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                4, 5, 6, 7, 8, 12, 13, 14, 15, 16, 17, 18
                            }));

            list.RemoveRange(-1, 3);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                6, 7, 8, 12, 13, 14, 15, 16, 17, 18
                            }));

            list.RemoveRange(3, -2);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                6, 7, 13, 14, 15, 16, 17, 18
                            }));

            list.RemoveRange(0, -2);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                7, 13, 14, 15, 16, 17, 18
                            }));

            list.RemoveRange(8, -2);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                7, 13, 14, 15, 16, 17
                            }));

            list.RemoveRange(-1, -2);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                7, 13, 14, 15, 16, 17
                            }));


            list.RemoveRange(7, 2);

            Assert.That(list,
                        Is.EquivalentTo(new[]
                            {
                                7, 13, 14, 15, 16, 17
                            }));
        }
    }
}