using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WTacticsLibrary.Model
{   
    public class Base
    {
        public Guid Guid { get; set; }

        public User Creator { get; set; }

        public DateTime? CreateTime { get; set; }

        public User LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
    }
}