using System;

namespace AhModel
{
    public class EntityBase
    {
        public int ID { set; get; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
