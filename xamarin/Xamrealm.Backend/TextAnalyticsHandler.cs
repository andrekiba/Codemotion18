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
        private readonly Subject<ModifiedTask> modifiedTaskSubject;

        #endregion

        #region Properties

        public IObservable<IChangeDetails> WhenRealmChange { get; private set; }

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
            
            modifiedTaskSubject = new Subject<ModifiedTask>();

            //WhenRealmChange = Observable.Create<IChangeDetails>(observer =>
            //{
            //    return Disposable.Empty;
            //});

            //WhenRealmChange = modifiedTaskSubject.AsObservable();
            //modifiedTaskSubject
            //    .Where(x => x.Changes.ContainsKey("Task"))
            //    .Select(x => x.Changes["Task"])
            //    .Where(x => x.Modifications.Any())
            //    .SelectMany(x => x.Modifications)
            //    .Where(m => m.CurrentObject != null && m.PreviousObject != null)
                

            //    .Subscribe(changeDetails => { },
            //    ex => { },
            //    () => { 

            modifiedTaskSubject
                .Where(t => t.Text.Length > 10)
                .GroupBy(t => t.Id)
                .Select(g => g.Throttle(TimeSpan.FromSeconds(2)))
                .Subscribe(async t =>
                    {
                        #region Analytics

                        var sentimentResponse = await GetScores(t.ToEnumerable());
                        var keyPhraseResponse = await GetTags(t.ToEnumerable());

                        #endregion

                        #region Update Realm

                        //var toUpdate = sentimentResponse
                        //    .Select(doc =>
                        //    {
                        //        var obj = modifiedTasks[doc.Key].Obj;
                        //        var text = modifiedTasks[doc.Key].Text;

                        //        if (!keyPhraseResponse.TryGetValue(doc.Key, out var keyPhrases) || keyPhrases == null)
                        //            keyPhrases = new List<string> { "Unknown" };

                        //        Console.WriteLine("------------------");
                        //        Console.WriteLine($"Analyzed: {text}");
                        //        Console.WriteLine($"Score: {doc.Value}");
                        //        Console.WriteLine($"KeyPhrases: {string.Join(", ", keyPhrases)}");
                        //        Console.WriteLine("------------------");

                        //        return new
                        //        {
                        //            Reference = ThreadSafeReference.Create(obj),
                        //            Score = doc.Value,
                        //            KeyPhrases = keyPhrases
                        //        };
                        //    })
                        //    .ToList();

                        //using (var realm = details.GetRealmForWriting())
                        //{
                        //    var resolved = toUpdate.Select(t => new
                        //    {
                        //        Object = realm.ResolveReference(t.Reference),
                        //        t.Score,
                        //        t.KeyPhrases
                        //    })
                        //    .ToList();

                        //    realm.Write(() =>
                        //    {
                        //        foreach (var item in resolved)
                        //        {
                        //            item.Object.Score = item.Score;
                        //            item.Object.Tags = string.Join(" ", item.KeyPhrases);
                        //        }
                        //    });
                        //}

                        #endregion
                    },
                    ex => { },
                    () => { });
        }

        public override Task HandleChangeAsync(IChangeDetails details)
        {         
            if (details.Changes.TryGetValue("Task", out var changeSetDetails) && changeSetDetails.Modifications.Any())
            {
                try
                {
                    //filter modifications only related to task's title if > 15 char
                    //otherwise we have a beautiful endless loop :-)
                    var modifiedTasks = (from m in changeSetDetails.Modifications
                                         where m.CurrentObject != null && m.PreviousObject != null
                                         group m by m.CurrentObject.Id into g
                                         let obj = g.Last().CurrentObject
                                         let text = (string)obj.Title
                                         select new ModifiedTask{ Id = (string)g.Key, Obj = obj, Text = text })
                                         .ToList();
                                         //.ToDictionary(x => x.Id, y => new { y.Obj, y.Text });

                    modifiedTasks.ForEach(t => modifiedTaskSubject.OnNext(t));                                       
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return Task.CompletedTask;
        }

        private async Task<Dictionary<string, float>> GetScores(IEnumerable<ModifiedTask> modifiedTasks)
        {
            var sentimentRequest = modifiedTasks.Select(t => new SentimentDocument
            {
                Id = t.Id,
                Text = t.Text,
                Language = "en"
            }).Cast<IDocument>().ToList();

            Console.WriteLine($"Requesting sentiment score for {sentimentRequest.Count} objects...");

            var sentimentResponse = await sentimentClient.GetSentimentAsync(new SentimentRequest
            {
                Documents = sentimentRequest
            });
        

            foreach (var error in sentimentResponse.Errors)
            {
                Console.WriteLine("Error from sentiment API: " + error.Message);
            }

            return sentimentResponse.Documents.ToDictionary(d => d.Id, d => d.Score);
        }

        private async Task<Dictionary<string, List<string>>> GetTags(IEnumerable<ModifiedTask> modifiedTasks)
        {
            var keyPhraseRequest = modifiedTasks.Select(t => new KeyPhraseDocument
            {
                Id = t.Id,
                Text = t.Text,
                Language = "en"
            }).Cast<IDocument>().ToList();

            Console.WriteLine($"Requesting key phrases for {keyPhraseRequest.Count} objects...");

            var keyPhraseResponse = await keyPhraseClient.GetKeyPhrasesAsync(new KeyPhraseRequest
            {
                Documents = keyPhraseRequest
            });

            foreach (var error in keyPhraseResponse.Errors)
            {
                Console.WriteLine("Error from KeyPhrase API: " + error.Message);
            }

            return keyPhraseResponse.Documents.ToDictionary(d => d.Id, d => d.KeyPhrases);
        }

        private class ModifiedTask
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public dynamic Obj { get; set; }
        }
    }

}
