using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Text.Core;
using Microsoft.ProjectOxford.Text.KeyPhrase;
using Microsoft.ProjectOxford.Text.Sentiment;
using Realms;
using Realms.Server;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Timers;

namespace Xamrealm.Backend
{
    public class TextAnalyticsHandler : RegexNotificationHandler
    {
        #region Fields

        private readonly SentimentClient sentimentClient;
        private readonly KeyPhraseClient keyPhraseClient;
        private readonly Subject<IChangeDetails> changesSubject;

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
            
            changesSubject = new Subject<IChangeDetails>();

            //WhenRealmChange = Observable.Create<IChangeDetails>(observer =>
            //{
            //    return Disposable.Empty;
            //});

            //WhenRealmChange = changesSubject.AsObservable();
            //changesSubject
            //    .Where(x => x.Changes.ContainsKey("Task"))
            //    .Select(x => x.Changes["Task"])
            //    .Where(x => x.Modifications.Any())
            //    .SelectMany(x => x.Modifications)
            //    .Where(m => m.CurrentObject != null && m.PreviousObject != null)
                

            //    .Subscribe(changeDetails => { },
            //    ex => { },
            //    () => { });
        }

        public override async Task HandleChangeAsync(IChangeDetails details)
        {
            changesSubject.OnNext(details);

            if (details.Changes.TryGetValue("Task", out var changeSetDetails) && changeSetDetails.Modifications.Any())
            {
                try
                {
                    //filter modifications only related to task's title if > 20 char
                    //otherwise we have a beautiful endless loop :-)
                    var modifiedTasks = (from m in changeSetDetails.Modifications
                                         where m.CurrentObject != null && m.PreviousObject != null
                                         group m by m.CurrentObject.Id into g
                                         let last = g.Last().CurrentObject
                                         let text = (string)last.Title
                                         where text.Length > 15
                                         select new { Id = (string)g.Key, Obj = last, Text = text })
                                         .ToDictionary(x => x.Id, y => new { y.Obj, y.Text });

                    if (!modifiedTasks.Any())
                        return;

                    var texts = modifiedTasks
                        .Select(t => new { Id = t.Key, t.Value.Text })
                        .ToList();

                    #region Analytics

                    Console.WriteLine($"Requesting sentiment score for {texts.Count} objects...");

                    var sentimentRequest = texts.Select(t => new SentimentDocument
                    {
                        Id = t.Id,
                        Text = t.Text,
                        Language = "en"
                    })
                    .Cast<IDocument>()
                    .ToList();

                    var sentimentResponse = await sentimentClient.GetSentimentAsync(new SentimentRequest
                    {
                        Documents = sentimentRequest
                    });

                    foreach (var error in sentimentResponse.Errors)
                    {
                        Console.WriteLine("Error from sentiment API: " + error.Message);
                    }

                    Console.WriteLine($"Requesting key phrases for {texts.Count} objects...");

                    var keyPhraseRequest = texts.Select(t => new KeyPhraseDocument
                    {
                        Id = t.Id,
                        Text = t.Text,
                        Language = "en"
                    })
                    .Cast<IDocument>()
                    .ToList();

                    var keyPhraseResponse = await keyPhraseClient.GetKeyPhrasesAsync(new KeyPhraseRequest
                    {
                        Documents = keyPhraseRequest
                    });

                    foreach (var error in keyPhraseResponse.Errors)
                    {
                        Console.WriteLine("Error from KeyPhrase API: " + error.Message);
                    }

                    var keyPhraseDictionary = keyPhraseResponse.Documents.ToDictionary(d => d.Id, d => d.KeyPhrases);

                    #endregion 

                    var toUpdate = sentimentResponse.Documents
                        .Select(doc =>
                        {
                            var obj = modifiedTasks[doc.Id].Obj;
                            var text = modifiedTasks[doc.Id].Text;

                            if (!keyPhraseDictionary.TryGetValue(doc.Id, out var keyPhrases) || keyPhrases == null)
                                keyPhrases = new List<string> { "Unknown" };

                            Console.WriteLine("------------------");
                            Console.WriteLine($"Analyzed: {text}");
                            Console.WriteLine($"Score: {doc.Score}");
                            Console.WriteLine($"KeyPhrases: {string.Join(", ", keyPhrases)}");
                            Console.WriteLine("------------------");

                            return new
                            {
                                doc.Score,
                                Reference = ThreadSafeReference.Create(obj),
                                KeyPhrases = keyPhrases
                            };
                        })
                        .ToList();

                    #region Update Realm

                    using (var realm = details.GetRealmForWriting())
                    {
                        var resolved = toUpdate.Select(t => new
                        {
                            Object = realm.ResolveReference(t.Reference),
                            t.Score,
                            t.KeyPhrases
                        })
                        .ToList();

                        realm.Write(() =>
                        {
                            foreach (var item in resolved)
                            {
                                item.Object.Score = item.Score;
                                item.Object.Tags = string.Join(" ", item.KeyPhrases);
                            }
                        });
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
