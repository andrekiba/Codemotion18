using System;
using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class Board : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public IList<TaskList> TaskLists { get; }
    }
}
