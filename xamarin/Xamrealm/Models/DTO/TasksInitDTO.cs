using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace Xamrealm.Models.DTO
{
    public class TasksInitDTO
    {
        public Realm Realm { get; set; }

        public string IdTaskList { get; set; }
    }
}
