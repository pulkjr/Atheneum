using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net.Mime;
using Atheneum.Enums;

namespace Atheneum;

/// <summary>
/// A Atheneum Contributors object
/// </summary>
public class Contributor
{
    public string SamAccountName { get; set; }

    public string Surname { get; set; }

    public string GivenName { get; set; }

    public string DisplayName { get; set; }

    /// <summary>
    /// The role that this contributor plays. i.e. Operator, Integrator...
    /// </summary>
    public Skillset Role { get; set; }

    public List<TrainingRecord> TrainingRecords { get; set; }

    //public List<TrainingRecord> TrainingRecords;

    /// <summary>
    /// Instantiate a new Contributor using Active Directory to populate the given users information
    /// </summary>
    /// <param name="userName">The username of the user</param>
    /// <param name="domain">The domain to search for the user</param>
    public Contributor(string userName, string domain)
    {
        PrincipalContext principalContext = new (ContextType.Domain, domain);
        UserPrincipal user = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, $"{domain}\\{userName}");
        SamAccountName = user.SamAccountName;
        Surname = user.Surname;
        GivenName = user.GivenName;
        DisplayName = user.DisplayName;
    }

    public Contributor() { }

    /// <summary>
    /// Add a training record to this user for today's date
    /// </summary>
    /// <param name="article">The article that was trained against</param>
    public void AddTrainingRecord(Article article)
    {
        DateTime _trainingRecordDate = DateTime.Now.Date;

        AddTrainingRecord(article, _trainingRecordDate);
    }

    public void AddTrainingRecord(Article article, DateTime date)
    {
        if (!TrainingRecords.Any(tr => tr.ArticleID == article.ID && tr.ArticleTitle == article.Title))
        {
            // No training record exist for this article on this Contributor add a new record.
            TrainingRecord trainingRecord = new(article, date);
            TrainingRecords.Add(trainingRecord);
        }
        else
        {
            TrainingRecord trainingRecord = TrainingRecords.Where(tr => tr.ArticleID == article.ID && tr.ArticleTitle == article.Title).FirstOrDefault<TrainingRecord>();

            trainingRecord.AddTrainingDate(date);
        }
        article.LastTrained = date;
    }
}

