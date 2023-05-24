using System;
using System.Collections.Generic;
using System.Linq;

namespace Atheneum;
public class TrainingRecord
{
    public string ArticleTitle { get; set; }

    public string ArticleID { get; set; }

    public List<DateTime> TrainingDates { get; set; }

    /// <summary>
    /// Create a new training record
    /// </summary>
    public TrainingRecord() { }

    /// <summary>
    /// Create a new training record
    /// </summary>
    /// <param name="article">The article this training record is for</param>
    /// <param name="date">The date that this training was conducted</param>
    public TrainingRecord(Article article, DateTime date)
    {
        this.ArticleID = article.ID;
        this.ArticleTitle = article.Title;
        this.TrainingDates = new(); // This is require for JSON Serialization to work correctly
        this.TrainingDates.Add(date);
    }
    /// <summary>
    /// Create a new training record for Today
    /// </summary>
    /// <param name="article">The article this training record is for</param>
    public TrainingRecord(Article article)
    {
        this.ArticleID = article.ID;
        this.ArticleTitle = article.Title;
        DateTime _trainingRecordDate = DateTime.Now.Date;
        this.TrainingDates = new(); // This is require for JSON Serialization to work correctly
        this.TrainingDates.Add(_trainingRecordDate);
    }

    /// <summary>
    /// Add a training date for a specific Date
    /// </summary>
    public void AddTrainingDate(DateTime date)
    {
        DateTime _trainingRecordDate = DateTime.Now.Date;
        if (TrainingDates.Any(d => d.Date == _trainingRecordDate.Date))
        {
            return;
        }
        this.TrainingDates.Add(_trainingRecordDate);
    }
}