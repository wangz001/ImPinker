using System;

namespace AhModel
{
	/// <summary>
	/// AHStyleProperty:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class AhStyleProperty
	{
        public AhStyleProperty()
        { }
        #region Model
        private int _id;
        private int _propertygroupid;
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
        public int PropertyGroupID
        {
            set { _propertygroupid = value; }
            get { return _propertygroupid; }
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

