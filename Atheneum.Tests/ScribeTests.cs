using System;
using Atheneum;
using Atheneum.Enums;
namespace Atheneum.Tests;

[TestFixture]
public class ScribeTests
{
    private ScribeSettings ScribeSettings;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        ScribeSettings = new();
        ScribeSettings.DocumentationDirectory = new("./Resources");
        ScribeSettings.ContributorsJsonPath = new FileInfo($".//Resources//Contributors.json");
    }
    [Test, Description("Validate that a Contributors can be deserialized correctly"), Order(0)]
    public void ValidateNewContributorFromFile()
    {
        ScribeSettings _testSettings = new();
        _testSettings.ContributorsJsonPath = ScribeSettings.ContributorsJsonPath;
        Scribe atheneum = new(_testSettings);
        Assert.Multiple(() =>
        {
            Assert.That(atheneum.Contributors[0].GivenName, Is.EqualTo("John"));
            Assert.That(atheneum.Contributors[0].DisplayName, Is.EqualTo("Doe, John"));
            Assert.That(atheneum.Contributors[0].Surname, Is.EqualTo("Doe"));
            Assert.That(atheneum.Contributors[0].Role, Is.EqualTo(Skillset.Integrator));
        });
    }
    [Test, Description("Validate Save Team Members is working"), Order(1)]
    public void ValidateExportContributors()
    {
        Scribe atheneum = new(ScribeSettings);

        Contributor john = atheneum.Contributors.Where(tm => tm.SamAccountName == "john.doe").FirstOrDefault();

        atheneum.Articles.ForEach(a => john.AddTrainingRecord(a));

        FileInfo _tempContributorsPath = new($"{ScribeSettings.ContributorsJsonPath.Directory.FullName}/ExportTest.json");

        Console.Write(_tempContributorsPath.FullName);

        _ = atheneum.ExportContributors(_tempContributorsPath);

        Assert.That(_tempContributorsPath, Does.Exist);
    }
    [Test, Description("Validate Save Article Changes")]
    public void ValidateSaveArticleUpdates()
    {
        Scribe atheneum = new(ScribeSettings);

        Article article = atheneum.Articles.Where(a => a.ID == "05d54556-bc64-4e7d-a0a1-bafb2bb5a98e").FirstOrDefault();

        Contributor john = atheneum.Contributors.Where(tm => tm.SamAccountName == "john.doe").FirstOrDefault();

        john.AddTrainingRecord(article);

        atheneum.Articles.ForEach(a => a.Save());
    }
}
