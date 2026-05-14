using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oplink.Mims.Models.BusinessObjects
{
    /// <summary>
    /// 部门信息
    /// </summary>
    public class Department : ICloneable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Department Parent { get; set; }

        #region ICloneable 成员

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
