using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Atheneum;
using Atheneum.Enums;

namespace Atheneum.Tests;

[TestFixture]
public class ArticleTests
{
    private ScribeSettings ScribeSettings;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        ScribeSettings = new();
        ScribeSettings.DocumentationDirectory = new("./Resources");
    }

    [Test]
    public void ValidateYamlPropertyValues()
    {
        Scribe root = new Scribe(ScribeSettings);
        Assert.Multiple(() =>
        {
            Assert.That(root.Articles.Count(), Is.EqualTo(3));

            Article exchange = root.Articles.Where(a => a.Title == "Connect to Remote Exchange Server").FirstOrDefault();
            Assert.That(exchange.Type, Is.EqualTo(ArticleType.GettingStarted));
            Assert.That(exchange.Title, Is.EqualTo("Connect to Remote Exchange Server"));
            Assert.That(exchange.Abstract, Is.Null);
            Assert.That(exchange.Author, Does.Contain("Doe, John"));
            Assert.That(exchange.Difficulty, Is.EqualTo(Difficulty.InDevelopment));
            Assert.That(exchange.Skillset, Is.EqualTo(Skillset.Any));
            Assert.That(exchange.Technology, Is.EqualTo("PowerShell"));
            Assert.That(exchange.ID, Is.EqualTo("97f248a2-d5d7-4c66-b7c0-c46fbbf725ef"));
            Assert.That(exchange.Path.Name, Is.EqualTo("Getting Started - Connect to Remote Exchange Server.md"));

            Article vmSnapShot = root.Articles.Where(a => a.Title == "Create a VMWare Snapshot for a Virtual Machine").FirstOrDefault();
            Assert.That(vmSnapShot.Type, Is.EqualTo(ArticleType.Modify));
            Assert.That(vmSnapShot.Title, Is.EqualTo("Create a VMWare Snapshot for a Virtual Machine"));
            Assert.That(vmSnapShot.Abstract, Is.EqualTo("Create a VM SnapShot"));
            Assert.That(vmSnapShot.Author, Does.Contain("John Doe"));
            Assert.That(vmSnapShot.Difficulty, Is.EqualTo(Difficulty.Professional));
            Assert.That(vmSnapShot.Skillset, Is.EqualTo(Skillset.Operator));
            Assert.That(vmSnapShot.TrainingFrequency, Is.EqualTo(TrainingFrequency.SemiAnnually));
            Assert.That(vmSnapShot.Technology, Is.EqualTo("Windows"));
            Assert.That(vmSnapShot.ID, Is.EqualTo("05d54556-bc64-4e7d-a0a1-bafb2bb5a98e"));
            Assert.That(vmSnapShot.DisablePublication, Is.EqualTo(true));
            Assert.That(vmSnapShot.Keywords, Does.Contain("snapshot"));
            Assert.That(vmSnapShot.Keywords, Does.Contain("vm"));
            Assert.That(vmSnapShot.Keywords, Does.Contain("vmware"));
            Assert.That(vmSnapShot.Section, Is.EqualTo("Virtual Machine"));
            Assert.That(vmSnapShot.Order, Is.EqualTo(1));
            Assert.That(vmSnapShot.VerifiedMetaData, Is.EqualTo(DateTime.Parse("03/23/2023")));
            Assert.That(vmSnapShot.VerifiedContent, Is.EqualTo(DateTime.Parse("03/23/2023")));
            Assert.That(vmSnapShot.SME, Does.Contain("John Doe"));
            Assert.That(vmSnapShot.SME, Does.Contain("Jane Doe"));
            Assert.That(vmSnapShot.IsTrainingTopic, Is.True);
            Assert.That(vmSnapShot.LastTrained, Is.EqualTo(DateTime.Parse("03/23/2022")));
            Assert.That(vmSnapShot.IsTrainingRequired, Is.True);
            Assert.That(vmSnapShot.IsImpacting, Is.True);
            Assert.That(vmSnapShot.SoftwareVersion, Does.Contain("6.7"));
            Assert.That(vmSnapShot.Path.Name, Is.EqualTo("Modify.Virtual Machine - Create a VMWare Snapshot for a Virtual Machine.md"));
            Assert.That(vmSnapShot.References.Count, Is.EqualTo(2));
            Assert.That(vmSnapShot.References[0].Title, Is.EqualTo("A KB Article"));
            Assert.That(vmSnapShot.References[0].Link.ToString(), Is.EqualTo("https://www.google.com/"));
            Assert.That(vmSnapShot.References[1].Title, Is.EqualTo("Another"));
            Assert.That(vmSnapShot.References[1].Link.ToString(), Is.EqualTo("https://www.netapp.com/"));
            Assert.That(vmSnapShot.KmId, Is.EqualTo("KM123456"));
            Assert.That(vmSnapShot.KmLastSync, Is.EqualTo(DateTime.Parse("03/23/2022")));
            Assert.That(vmSnapShot.IsKmSyncRequired, Is.True);
        });
    }
}
