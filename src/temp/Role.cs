using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Oplink.Mims.Models.BusinessObjects
{
    /// <summary>
    /// 系统角色
    /// </summary>
    [System.Runtime.Serialization.DataContract]
    public class Role
    {
        #region 预置权限
        static Dictionary<RoleKind, Permission[]> _stiPresetRolePermissions;
        static Role()
        {

        }

        /// <summary>
        /// 预定义权限
        /// </summary>
        public static Dictionary<RoleKind, Permission[]> PresetRolePermissions
        {
            get
            {
                return _stiPresetRolePermissions;
            }
        }
        #endregion

        [DataMember]
        int _id;

        [DataMember]
        string _name;

        [DataMember]
        string _description;

        [DataMember]
        string _quxian;

        [DataMember]
        string _remark;

        /// <summary>
        /// 角色拥有的权限
        /// </summary>
        [DataMember]
        private HashSet<Permission> _permissions;

        #region 属性

        /// <summary>
        /// 角色Id
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public RoleKind Kind
        {
            get;
            set;
        }

        public string Quanxian
        {
            get
            { return _quxian; }
            set { _quxian = value; }
        }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// 角色描述
        /// </summary>

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }

        #endregion

        public Role()
        {
            _permissions = new HashSet<Permission>();
        }
        /// <summary>
        /// 获取角色是否拥有指定权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(Permission permission)
        {
            return _permissions.Contains(permission);
        }

        /// <summary>
        /// 为角色增加指定权限
        /// </summary>
        /// <param name="permission"></param>
        public void AddPermission(Permission permission)
        {
            _permissions.Add(permission);
        }

        /// <summary>
        /// 取消角色的指定权限
        /// </summary>
        /// <param name="permission"></param>
        public void RemovePermission(Permission permission)
        {
            _permissions.Remove(permission);
        }

        [System.Runtime.Serialization.IgnoreDataMember]
        /// <summary>
        /// 获取角色所拥有的所有权限
        /// </summary>
        public Permission[] Permissions
        {
            get
            {
                return _permissions.ToArray();
            }
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj == null) return false;
        //    if (ReferenceEquals(this, obj)) return true;

        //    var o = obj as Role;
        //    if (o == null) return false;
        //    return o._id == this._id;
        //}
    }

    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleKind
    {
        Operator = 0,
        Technician,
        Engineer,
        ProductManager,
        ProjectManager,
        UserManager,
        DataManager,
        SuperUser = 255,
    }
}
