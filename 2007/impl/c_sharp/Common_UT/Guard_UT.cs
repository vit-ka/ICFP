using System;
using System.IO;
using Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Common_UT
{
    [TestFixture]
    public class Guard_UT
    {
        [Test]
        public void ArgumentNotNull_UT()
        {
            // Проверка на не null.
            try
            {
                Guard.ArgumentNotNull(new object(), "some arg");
            }
            catch
            {
                Assert.Fail("Exception has been throws but value is not null");
            }

            // Проверка на null.
            try
            {
                Guard.ArgumentNotNull(null, "somevalue");
                Assert.Fail("Exception must be thrown but no one has been catched.");
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Text.Contains("somevalue"));
                Assert.IsInstanceOfType(typeof (ArgumentNullException), ex);
            }

            // Проверка на value-type.
            try
            {
                Guard.ArgumentNotNull(32, "int args");
            }
            catch
            {
                Assert.Fail("Exception has been throws but value is integer");
            }
        }

        [Test]
        public void FileExists_UT()
        {
            // Проверка на существующем файле.
            string fileName = Path.GetTempFileName();
            try
            {
                Guard.FileExists(fileName);
            }
            catch
            {
                Assert.Fail("Exception has been throws but file exists");
            }

            // Проверка на не существующем файле.
            fileName = Path.GetTempPath() + "\\somemegafile_withuniquename.some_ext";
            try
            {
                Guard.FileExists(fileName);
                Assert.Fail("Exception must be thrown but no one has been catched.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (InvalidDataException), ex);
                Assert.That(ex.Message, Text.Contains(fileName));
            }

            // Передаем вместо имени файла null
            try
            {
                Guard.FileExists(null);
                Assert.Fail("Exception must be thrown but no one has been catched.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (ArgumentNullException), ex);
            }

            // Передаем чушь вместо имени файла.
            try
            {
                Guard.FileExists(":::::");
                Assert.Fail("Exception must be thrown but no one has been catched. Incorrect path.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof (ArgumentException), ex);
            }
        }
    }
}