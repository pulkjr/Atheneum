using System;
using Atheneum;

namespace Atheneum.Tests;

[TestFixture]
public class ScribeSettingsTests
{
    [TestFixture]
    public class ScribeSettingsContributorsJsonPath
    {
        [Test]
        public void Should_Error_On_File_Type()
        {
            ScribeSettings _testSettings = new();

            Assert.Throws<NotSupportedException>(() => _testSettings.ContributorsJsonPath = new FileInfo("./Test.yaml"));
        }

        [Test]
        public void Should_Error_On_File_Not_Present()
        {
            ScribeSettings _testSettings = new();

            Assert.Throws<FileNotFoundException>(() => _testSettings.ContributorsJsonPath = new FileInfo("./Test.json"));
        }
    }
}