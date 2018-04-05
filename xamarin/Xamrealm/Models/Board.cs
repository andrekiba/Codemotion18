using System.Collections.Generic;
using Realms;

namespace Xamrealm.Models
{
    public class Board : RealmObject
    {
        [PrimaryKey]
        [MapTo("id")]
        public int Id { get; set; }

        [MapTo("taskLists")]
        public IList<TaskList> TaskLists { get; }
    }
}
