using System;
using Atheneum;
using Atheneum.Enums;
namespace Atheneum.Tests;

[TestFixture]
public class AtheneumFunctionalTests
{
    private DirectoryInfo DocumentationPath;

    private FileInfo ContributorsJson;


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        DocumentationPath = new DirectoryInfo(".//Resources");
        ContributorsJson = new FileInfo($".//Resources//Contributors.json");
    }
    [Test, Description("Validate that a Contributors can be deserialized correctly"), Order(0)]
    public void ValidateNewContributorFromFile()
    {
        Atheneum atheneum = new();
        atheneum.ImportContributors(ContributorsJson);
        Assert.Multiple(() =>
        {
            Assert.That(atheneum.Contributors[0].GivenName, Is.EqualTo("John"));
            Assert.That(atheneum.Contributors[0].DisplayName, Is.EqualTo("Doe, John"));
            Assert.That(atheneum.Contributors[0].Surname, Is.EqualTo("Doe"));
            Assert.That(atheneum.Contributors[0].Role, Is.EqualTo(Skillset.Integrator));
        });
    }

    [Test, Description("Validate Errors are working")]
    public void ValidateNewContributorFileType()
    {
        Atheneum atheneum = new();
        Assert.Throws<NotSupportedException>(() => atheneum.ImportContributors((new FileInfo("./Test.yaml"))));

    }
    [Test, Description("Validate Errors are working")]
    public void ValidateNewContributorFileExists()
    {
        Atheneum atheneum = new();
        Assert.Throws<FileNotFoundException>(() => atheneum.ImportContributors((new FileInfo("./Test.json"))));

    }
    [Test, Description("Validate Save Team Members is working"), Order(1)]
    public void ValidateExportContributors()
    {
        Atheneum atheneum = new(DocumentationPath);

        atheneum.ImportContributors(ContributorsJson);

        Contributor john = atheneum.Contributors.Where(tm => tm.SamAccountName == "john.doe").FirstOrDefault();

        atheneum.Articles.ForEach(a => john.AddTrainingRecord(a));

        FileInfo _tempContributorsPath = new($"{ContributorsJson.Directory.FullName}/ExportTest.json");

        Console.Write(_tempContributorsPath.FullName);

        _ = atheneum.ExportContributors(_tempContributorsPath);

        Assert.That(_tempContributorsPath, Does.Exist);
    }
    [Test, Description("Validate Save Article Changes")]
    public void ValidateSaveArticleUpdates()
    {
        Atheneum atheneum = new(DocumentationPath);

        atheneum.ImportContributors(ContributorsJson);

        Article article = atheneum.Articles.Where(a => a.ID == "05d54556-bc64-4e7d-a0a1-bafb2bb5a98e").FirstOrDefault();

        Contributor john = atheneum.Contributors.Where(tm => tm.SamAccountName == "john.doe").FirstOrDefault();

        john.AddTrainingRecord(article);

        atheneum.Articles.ForEach(a => a.Save());
    }
}
