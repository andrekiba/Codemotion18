using System;
using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class Board : RealmObject
    {
        [PrimaryKey]
        [MapTo("id")]
        public string Id { get; } = Guid.NewGuid().ToString();

        [MapTo("taskLists")]
        public IList<TaskList> TaskLists { get; }
    }
}
