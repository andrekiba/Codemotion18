using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Text.Core;
using Microsoft.ProjectOxford.Text.KeyPhrase;
using Microsoft.ProjectOxford.Text.Sentiment;
using Realms;
using Realms.Server;
using System.Reactive.Subjects;

namespace Xamrealm.Backend
{
    public class TextAnalyticsHandler : RegexNotificationHandler
    {
        #region Fields

        private readonly SentimentClient sentimentClient;
        private readonly KeyPhraseClient keyPhraseClient;
        private readonly Subject<TaskItem> newTaskSubject;
        private readonly Subject<TaskItem> modifiedTaskSubject;
        
        #endregion 

        public TextAnalyticsHandler() : base($"^/{Constants.RealmName}$")
        {
            sentimentClient = new SentimentClient(Constants.TextAnalyticsApiKey)
            {
                Url = Constants.SentimentUrl
            };

            keyPhraseClient = new KeyPhraseClient(Constants.TextAnalyticsApiKey)
            {
                Url = Constants.KeyPhraseUrl
            };

            newTaskSubject = new Subject<TaskItem>();
            modifiedTaskSubject = new Subject<TaskItem>();

            newTaskSubject.Select(async t =>
                {
                    t.Score = await GetScore(t);
                    t.Tags = await GetTags(t);
                    return t;
                })
                .Concat()
                .Subscribe(
                    UpdateTask,
                    ex =>
                    {
                        Console.WriteLine(ex.Message);
                    });

            modifiedTaskSubject
                //.Where(t => t.Text.Length > 10)
                .GroupBy(t => t.Id)
                .SelectMany(g => g.Throttle(TimeSpan.FromSeconds(3)))
                .Select(async t =>
                {
                    t.Score = await GetScore(t);
                    t.Tags = await GetTags(t);
                    return t;
                })
                .Concat()
                .Subscribe(
                    UpdateTask,
                    ex =>
                    {
                        Console.WriteLine(ex.Message);
                    });
        }

        public override Task HandleChangeAsync(IChangeDetails details)
        {
            try
            {
                if (details.Changes.TryGetValue("Task", out var changeSetDetails))
                {
                    //if (changeSetDetails.Insertions.Any())
                    //{
                    //    var newTasks = (from m in changeSetDetails.Insertions
                    //        let obj = m.CurrentObject
                    //        let text = (string)obj.Title
                    //        select new TaskItem {
                    //            Id = (string)obj.Id,
                    //            Reference = ThreadSafeReference.Create(obj),
                    //            Text = text,
                    //            Realm = details.GetRealmForWriting()
                    //        })
                    //        .ToList();

                    //    newTasks.ForEach(t => newTaskSubject.OnNext(t));
                    //}

                    if (changeSetDetails.Modifications.Any())
                    {
                        var modifiedTasks = (from m in changeSetDetails.Modifications
                                 //where m.CurrentObject != null && m.PreviousObject != null
                                 //group m by m.CurrentObject.Id into g
                                 //let obj = g.Last().CurrentObject
                                 //let text = (string)obj.Title
                                 let obj = m.CurrentObject
                                 let text = (string)obj.Title
                                 select new TaskItem
                                 {
                                     Id = (string)obj.Id,
                                     Reference = ThreadSafeReference.Create(obj),
                                     Text = text,
                                     Realm = details.GetRealmForWriting()
                                 })
                                .ToList();

                        modifiedTasks.ForEach(t => modifiedTaskSubject.OnNext(t));
                    }                                       
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return Task.CompletedTask;
        }

        private async Task<float> GetScore(TaskItem task)
        {
            Console.WriteLine($"Requesting sentiment score for one object...");

            var sentimentResponse = await sentimentClient.GetSentimentAsync(new SentimentRequest
            {
                Documents = new List<IDocument> { new SentimentDocument
                {
                    Id = task.Id,
                    Text = task.Text,
                    Language = "en"
                } }
            });
        
            foreach (var error in sentimentResponse.Errors)
            {
                Console.WriteLine("Error from sentiment API: " + error.Message);
            }

            return sentimentResponse.Documents[0].Score;
        }

        private async Task<List<string>> GetTags(TaskItem task)
        {
            Console.WriteLine($"Requesting key phrases for one object...");

            var keyPhraseResponse = await keyPhraseClient.GetKeyPhrasesAsync(new KeyPhraseRequest
            {
                Documents = new List<IDocument> { new KeyPhraseDocument
                {
                    Id = task.Id,
                    Text = task.Text,
                    Language = "en"
                } }
            });

            foreach (var error in keyPhraseResponse.Errors)
            {
                Console.WriteLine("Error from KeyPhrase API: " + error.Message);
            }

            return keyPhraseResponse.Documents[0].KeyPhrases;
        }

        private static void UpdateTask(TaskItem task)
        {
            using (var realm = task.Realm)
            {
                var resolved = realm.ResolveReference(task.Reference);
                realm.Write(() =>
                {
                    resolved.Score = task.Score;
                    resolved.Tags = task.Tags;
                });
            }
        }

        private class TaskItem
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public dynamic Reference { get; set; }
            public Realm Realm { get; set; }
            public float Score { get; set; }
            public List<string> Tags { get; set; } = new List<string> { "Unknown" };
        }
    }

}
