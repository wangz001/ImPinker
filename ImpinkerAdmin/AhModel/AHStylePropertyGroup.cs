using System;

namespace AhModel
{
    [Serializable]
    public class AhStylePropertyGroup
    {
        public AhStylePropertyGroup()
        { }
        #region Model
        private int _id;
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }
        #endregion Model
    }
}
