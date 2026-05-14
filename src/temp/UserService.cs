using Oplink.Mims.Models.BusinessObjects;
using Oplink.Mims.Models.Exceptions;
using Oplink.Utilities;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Oplink.Mims.Services.Default
{
    internal class UserService : IUserService
    {
        private static readonly string _stiAdminLoginName = "oplink";
        private static readonly string _stiDefaultDepartmentName = "OPLink";
        private Person _superUser;

        /// <summary>
        /// 数据访问服务
        /// </summary>
        private IDataService _dataService;

        private ILogService _logService;

        /// <summary>
        /// 服务上下文
        /// </summary>
        private IMimsServiceContext _serviceContext;

        /// <summary>
        /// 用户密码加密方式
        /// </summary>
        private System.Security.Cryptography.MD5 _passwordEncryptor;

        public UserService()
        {
        }

        /// <summary>
        /// 获取字符串加密后的数据
        /// </summary>
        /// <param name="rawString">未加密字符串</param>
        /// <returns>加密后的数据</returns>
        private string GetEncryptString(string rawString)
        {
            var rawBytes = Encoding.ASCII.GetBytes(rawString);
            var encryptBytes = _passwordEncryptor.ComputeHash(rawBytes);

            //转化为十六进制格式
            var s = from b in encryptBytes select b.ToString("X2");
            return string.Concat(s.ToArray());
        }

        private ComponentResourceManager res = new ComponentResourceManager(typeof(Properties.Resources));

        #region IUserService 成员

        public void Initialize(IMimsServiceContext serviceContext)
        {
            var dataService = serviceContext.GetService<IDataService>();
            if (dataService == null)
            {
                string strInitUserServeErrMsg = res.GetString("InitUserServeErrMsg");
                throw new InitServiceFatalException(strInitUserServeErrMsg);
            }

            _passwordEncryptor = System.Security.Cryptography.MD5.Create();
            _serviceContext = serviceContext;
            _dataService = dataService;
            _logService = serviceContext.GetService<ILogService>();

            SetupRoles();
            SetupDepartments();
            SetupPersons();

            string strInitUserServeMsg = res.GetString("InitUserServeMsg");
            _logService.LogInfo(strInitUserServeMsg);
        }

        /// <summary>
        /// 初始化默认角色
        /// </summary>
        private void SetupRoles()
        {
            var dataSrv = _dataService;
            var existsRoles = dataSrv.RoleAccessor.GetRolesCount();
            if (existsRoles > 0) return;

            var roleDataList = from RoleKind role in Enum.GetValues(typeof(RoleKind))
                               select new { Name = role.GetName(), Description = role.GetDescription(), Kind = role };

            foreach (var data in roleDataList)
            {
                Role role = new Role();
                role.Name = data.Name;
                role.Description = data.Description;
                role.Kind = data.Kind;
                foreach (var p in Role.PresetRolePermissions[role.Kind])
                    role.AddPermission(p);

                dataSrv.RoleAccessor.CreateRole(role);
            }
        }

        /// <summary>
        /// 初始化默认部门
        /// </summary>
        private void SetupDepartments()
        {
            var dataSrv = _dataService;
            if (!dataSrv.DepartmentAccessor.HasName(_stiDefaultDepartmentName))
            {
                var dept = new Department();
                dept.Name = _stiDefaultDepartmentName;
                this.CreateDepartment(dept);
            }
        }

        /// <summary>
        /// 初始化默认管理员账户
        /// </summary>
        private void SetupPersons()
        {
            var dataSrv = _dataService;
            _superUser = dataSrv.PersonAccessor.GetPerson(_stiAdminLoginName);

            if (_superUser == null)
            {
                var superRole = dataSrv.RoleAccessor.GetAllRoles().Where(r => r.Kind == RoleKind.SuperUser).FirstOrDefault();
                var department = dataSrv.DepartmentAccessor.GetAllDepartments().Where(d => d.Name == _stiDefaultDepartmentName).FirstOrDefault();

                Debug.Assert(superRole != null);
                Debug.Assert(department != null);

                var person = new Person();
                person.LoginName = _stiAdminLoginName;
                person.Pass = _stiAdminLoginName;
                person.Name = "光联系统管理员";
                person.Role = superRole;
                person.Department = department;
                _superUser = this.CreatePerson(person);
            }
        }

        public void Dispose()
        {
            string strStopUserServeMsg = res.GetString("StopUserServeMsg");
            _logService.LogInfo(strStopUserServeMsg);
        }

        #region Person

        public bool PersonExsitbyLoginName(string loginName)
        {
            return _dataService.PersonAccessor.HasLoginName(loginName);
        }

        public Person GetPerson(string strloginName)
        {
            var person = _dataService.PersonAccessor.GetPerson(strloginName);

            return person;
        }

        public Person GetPerson(string loginName, string password, SystemOperateType loginType)
        {
            var encryptPassword = GetEncryptString(password);
            var person = _dataService.PersonAccessor.GetPerson(loginName, encryptPassword, loginType);

            return person;
        }

        private void PrepareSave(Person person)
        {
            string strLoginName = res.GetString("LoginName");
            MakeSureNotEmpty(person.LoginName, strLoginName);

            string strName = res.GetString("Name");
            MakeSureNotEmpty(person.Name, strName);

            MakeSureNotEmpty(person.Department, "所属部门");
            MakeSureNotEmpty(person.Role, "岗位");

            int tmp;
            if (person.LoginName != _stiAdminLoginName && !int.TryParse(person.LoginName, out tmp))
            {
                string strErrMsg = res.GetString("LoginNameErrMsg");
                throw new InvalidDataException(strErrMsg);
            }

            //检测登录名是否重复
            var originalPerson = _dataService.PersonAccessor.GetPerson(person.Id);
            //判断登录名是否有修改，如果没变化，则不检测重复
            if (originalPerson == null || originalPerson.LoginName != person.LoginName)
            {
                var hasLoginName = _dataService.PersonAccessor.HasLoginName(person.LoginName);
                if (hasLoginName)
                {
                    string strErrMsg = res.GetString("LoginNameErrMsg2");
                    throw new InvalidDataException(strErrMsg);
                }
            }
        }

        private void MakeSureNotEmpty(object value, string desc)
        {
            if (value == null)
            {
                string strNoEmptyMsg = res.GetString("NoEmptyMsg");
                throw new InvalidDataException(desc + strNoEmptyMsg);
            }
        }

        private void MakeSureNotEmpty(string value, string desc)
        {
            if (value.IsNullOrEmpty() || value.Trim().IsNullOrEmpty())
            {
                string strNoEmptyMsg = res.GetString("NoEmptyMsg");
                throw new InvalidDataException(desc + strNoEmptyMsg);
            }
        }

        public Role GetOperatorRoleByID()
        {
            var dataSrv = _dataService;
            var superRole = dataSrv.RoleAccessor.GetAllRoles().Where(r => r.Kind == RoleKind.Operator).FirstOrDefault();
            return superRole;
        }

        //public Person CreatePerson(Role role, string loginName, string password, string firstName, string secondName)
        public Person CreatePerson(Person person)
        {
            PrepareSave(person);

            var newPerson = person.Clone() as Person;
            newPerson.Pass = GetEncryptString(person.Pass);

            return _dataService.PersonAccessor.CreatePerson(newPerson);
        }

        ///// <summary>
        ///// 修改指定用户，不变更密码
        ///// </summary>
        ///// <param name="person"></param>
        //void UpdatePeronExceptPassword(Person person);
        public void UpdatePerson(Person person, bool updatePassword)
        {
            if (person.Id == _superUser.Id)
            {
                string strUpdateUserErrMsg = res.GetString("UpdateUserErrMsg");
                throw new InvalidDataException(strUpdateUserErrMsg);
            }

            PrepareSave(person);

            if (updatePassword)
            {
                person.Pass = GetEncryptString(person.Pass);
            }

            _dataService.PersonAccessor.UpdatePerson(person);
        }

        public void DeletePerson(Person person)
        {
            if (person.Id == _superUser.Id)
            {
                string strDeleteUserErrMsg = res.GetString("DeleteUserErrMsg");
                throw new InvalidDataException(strDeleteUserErrMsg);
            }

            _dataService.PersonAccessor.DeletePerson(person);
        }

        #endregion Person

        public Role[] GetAllRoles()
        {
            return _dataService.RoleAccessor.GetAllRoles();
        }

        public Role CreateRole(Role role)
        {
            return _dataService.RoleAccessor.CreateRole(role);
        }

        public void UpdateRole(Role role)
        {
            _dataService.RoleAccessor.UpdateRole(role);
        }

        public void DeleteRole(Role role)
        {
            _dataService.RoleAccessor.DeleteRole(role);
        }

        public Person[] GetAllPersons()
        {
            return _dataService.PersonAccessor.GetAllPersons();
        }

        #region Department

        private void PrepareSave(Department department)
        {
            var emptyName = department.Name.Trim().IsNullOrEmpty();
            if (emptyName)
                throw new InvalidDataException("部门名称不能空");

            if (department.Name.Length > 64)
                throw new InvalidDataException("部门名称不能超过64字符");

            var existsName = _dataService.DepartmentAccessor.HasName(department.Name);
            if (existsName)
                throw new InvalidDataException("部门名称重复");
        }

        private void PrepareRemove(Department department)
        {
            if (_dataService.DepartmentAccessor.HasPerson(department))
                throw new InvalidDataException("部门下存在用户，不能删除");
        }

        public Department[] GetAllDepartments()
        {
            return _dataService.DepartmentAccessor.GetAllDepartments();
        }

        public Department CreateDepartment(Department department)
        {
            PrepareSave(department);

            return _dataService.DepartmentAccessor.Create(department);
        }

        public void UpdateDepartment(Department department)
        {
            PrepareSave(department);

            _dataService.DepartmentAccessor.Update(department);
        }

        public void DeleteDepartment(Department department)
        {
            PrepareRemove(department);
            _dataService.DepartmentAccessor.Delete(department);
        }

        #endregion Department

        public Person[] ImportPersonsFromExcel(string excelFilePath)
        {
            var dataSet = GetExcelDataSet(excelFilePath);
            if (dataSet.Tables.Count == 0)
            {
                string strTabelErrMsg = res.GetString("TableErrMsg");
                throw new InvalidDataException(strTabelErrMsg);
            }

            var table = dataSet.Tables[0];
            if (table.Columns.Count < 2)
            {
                string strTableErrMsg2 = res.GetString("TableErrMsg2");
                throw new InvalidDataException(strTableErrMsg2);
            }

            if (table.Rows.Count < 1)
            {
                string strTableErrMsg = res.GetString("TableErrMsg");
                throw new InvalidDataException(strTableErrMsg);
            }

            var roles = this.GetAllRoles();
            var depts = this.GetAllDepartments();
            var persons = new Person[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int offset = 0;
                var row = table.Rows[i];

                var id = row[offset++].ToString();
                var name = row[offset++].ToString();
                var dept = row[offset++].ToString();
                var role = row[offset++].ToString();

                var person = new Person();
                person.Pass = id;
                person.LoginName = id;
                person.Name = name;
                person.Role = roles.Where(r => r.Name == role).FirstOrDefault();
                person.Department = depts.Where(d => d.Name == dept).FirstOrDefault();

                try
                {
                    this.PrepareSave(person);
                    persons[i] = person;
                }
                catch (Exception e)
                {
                    string strTableErrMsg = res.GetString("TableErrMsg3");
                    throw new InvalidDataException(strTableErrMsg + (i + 1) + Environment.NewLine + e.Message);
                }
            }

            return ImportPersons(persons);
        }

        private Person[] ImportPersons(Person[] persons)
        {
            return this._dataService.PersonAccessor.CreatePersons(persons);
        }

        /// <summary>
        /// 读取Excel文档
        /// </summary>
        /// <param name="excelFilePath">文件名称</param>
        /// <returns>返回一个数据集</returns>
        public DataSet GetExcelDataSet(string excelFilePath)
        {
            try
            {
                //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + excelFilePath + ";" + "Extended Properties=Excel 8.0;";
                // var connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + excelFilePath + "';Extended Properties='Excel 12.0;HDR=YES'";
                var connStr = "Provider=Microsoft.Jet.OleDb.4.0;Extended Properties=Excel 8.0;;Data Source='" + excelFilePath + "'";

                using (var conn = new System.Data.OleDb.OleDbConnection(connStr))
                {
                    conn.Open();
                    string sql = "select * from [sheet1$]";
                    var ds = new DataSet();
                    var adapter = new System.Data.OleDb.OleDbDataAdapter(sql, connStr);
                    adapter.Fill(ds, "table1");
                    return ds;
                }
            }
            catch (System.Data.OleDb.OleDbException)
            {
                throw;
            }
        }

        #endregion IUserService 成员
    }
}