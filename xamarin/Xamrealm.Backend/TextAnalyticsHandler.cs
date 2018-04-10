using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Text.Core;
using Microsoft.ProjectOxford.Text.KeyPhrase;
using Microsoft.ProjectOxford.Text.Sentiment;
using Realms;
using Realms.Server;

namespace Xamrealm.Backend
{
    public class TextAnalyticsHandler : RegexNotificationHandler
    {
        private readonly SentimentClient sentimentClient;
        private readonly KeyPhraseClient keyPhraseClient;

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
        }

        public override async Task HandleChangeAsync(IChangeDetails details)
        {
            if (details.Changes.TryGetValue("Task", out var changeSetDetails) &&
                changeSetDetails.Modifications.Length > 0)
            {
                try
                {
                    //filter modifications only related to task's title or description
                    //otherwise we have a beautiful endless loop :-)
                    var modifiedTasks = changeSetDetails.Modifications
                        .Where(m => m.CurrentObject.Title != null && m.CurrentObject.Title != string.Empty 
                                    && m.PreviousObject.Title != m.CurrentObject.Title)
                        .Select(x => x.CurrentObject)
                        .Select(t => new {Id = (string) t.Id, Obj = t})
                        .ToDictionary(x => x.Id, y => y.Obj);

                    var texts = modifiedTasks
                        .Select(t => new { Id = t.Key, Text = (string)t.Value.Title })
                        //.Select(t => new {Id = t.Key, Text = (string)(t.Value.Title + Environment.NewLine + t.Value.Description) } )
                        .Where(t => t.Text.Length > 20)
                        .ToList();

                    if (!texts.Any())
                        return;

                    #region Analytics

                    Console.WriteLine($"Requesting sentiment score for {texts.Count} objects...");

                    var sentimentRequest = texts.Select(t => new SentimentDocument
                    {
                        Id = t.Id,
                        Text = t.Text,
                        Language = "it"
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
                        Language = "it"
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
                            //var obj = changeSetDetails.Modifications[int.Parse(doc.Id)].CurrentObject;
                            var obj = modifiedTasks[doc.Id];

                            if (!keyPhraseDictionary.TryGetValue(doc.Id, out var keyPhrases) || keyPhrases == null)
                                keyPhrases = new List<string> { "Unknown" };

                            Console.WriteLine("------------------");
                            Console.WriteLine($"Analyzed: {obj.Title}");
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
