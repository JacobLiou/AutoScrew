using System;

namespace Oplink.Mims.Models.BusinessObjects
{
    public class Person : ICloneable
    {
        //  string _firstName;
        //  string _secondName;
        string _comment;
        string _password;
        int _operatetype;
        string _onlinecheck;
        string _mfgpermanent;
        DateTime _mfg_validity_start;
        DateTime _mfg_validity_end;
        string _debugpermanent;
        DateTime _debug_validity_start;
        DateTime _debug_validity_end;
        string _rdpermanent;
        DateTime _rd_validity_start;
        DateTime _rd_validity_end;

        string _trialrunpermanent;
        DateTime _trialrun_validity_start;
        DateTime _trialrun_validity_end;

        string _mpassword;

        Role _role;

        public int Id
        {
            get;
            set;
        }

        public string LoginName { get; set; }

        public string Name { get; set; }

        public string Pass
        {
            get { return _password; }
            set { _password = value; }
        }

        public string mPass
        {
            get { return _mpassword; }
            set { _mpassword = value; }
        }
        public string IfLogin { get; set; }

        public string IfHasLogin { get; set; }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public string FullName
        {
            get { return Name; }
        }

        public int OperateType
        {
            get { return _operatetype; }
            set { _operatetype = value; }
        }

        public string OnlineCheck
        {
            get { return _onlinecheck; }
            set { _onlinecheck = value; }
        }


        public string MFG_Permanent
        {
            get { return _mfgpermanent; }
            set { _mfgpermanent = value; }
        }

        public DateTime MFG_Validity_Start
        {
            get { return _mfg_validity_start; }
            set { _mfg_validity_start = value; }
        }

        public DateTime MFG_Validity_End
        {
            get { return _mfg_validity_end; }
            set { _mfg_validity_end = value; }
        }


        public string Debug_Permanent
        {
            get { return _debugpermanent; }
            set { _debugpermanent = value; }
        }

        public DateTime Debug_Validity_Start
        {
            get { return _debug_validity_start; }
            set { _debug_validity_start = value; }
        }

        public DateTime Debug_Validity_End
        {
            get { return _debug_validity_end; }
            set { _debug_validity_end = value; }
        }

        public string RD_Permanent
        {
            get { return _rdpermanent; }
            set { _rdpermanent = value; }
        }

        public DateTime RD_Validity_Start
        {
            get { return _rd_validity_start; }
            set { _rd_validity_start = value; }
        }

        public DateTime RD_Validity_End
        {
            get { return _rd_validity_end; }
            set { _rd_validity_end = value; }
        }


        public string TrialRun_Permanent
        {
            get { return _trialrunpermanent; }
            set { _trialrunpermanent = value; }
        }


        public DateTime TrialRun_Validity_Start
        {
            get { return _trialrun_validity_start; }
            set { _trialrun_validity_start = value; }
        }

        public DateTime TrialRun_Validity_End
        {
            get { return _trialrun_validity_end; }
            set { _trialrun_validity_end = value; }
        }


        /// <summary>
        /// 用户所属角色
        /// </summary>
        public Role Role
        {
            get { return _role; }
            set { _role = value; }
        }

        Department _department;

        /// <summary>
        /// 用户所属部门
        /// </summary>
        public Department Department
        {
            get { return _department; }
            set { _department = value; }
        }

        #region ICloneable 成员

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}