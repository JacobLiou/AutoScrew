CREATE TABLE `mims_department` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) DEFAULT NULL,
  `parent_id` int(11) DEFAULT NULL,
  `Description` varchar(260) DEFAULT NULL,
  `Sits` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=gbk

CREATE TABLE `mims_person` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `password` varchar(64) DEFAULT NULL,
  `role_id` int(11) DEFAULT NULL,
  `department_id` int(11) DEFAULT NULL,
  `login_name` varchar(64) DEFAULT NULL,
  `comment` varchar(255) DEFAULT NULL,
  `name` varchar(64) DEFAULT NULL,
  `OperatorType` int(11) DEFAULT NULL,
  `IfLogin` varchar(64) DEFAULT NULL,
  `IfHasLogin` varchar(64) DEFAULT NULL,
  `role_id_debug` int(11) DEFAULT NULL,
  `role_id_rd` int(11) DEFAULT NULL,
  `role_id_eng` int(11) unsigned zerofill DEFAULT '00000000000',
  `LoginDate` varchar(255) DEFAULT NULL,
  `IfHasLoginDebug` varchar(64) DEFAULT NULL,
  `IfHasLoginRD` varchar(64) DEFAULT NULL,
  `PassWordSettingDate` varchar(255) DEFAULT NULL,
  `IfResetPassWord` varchar(64) DEFAULT NULL,
  `LoginIP` varchar(64) DEFAULT NULL,
  `LoginMode` varchar(16) DEFAULT NULL,
  `Email` varchar(128) DEFAULT NULL,
  `OnlineCheck` varchar(16) DEFAULT NULL COMMENT '否是具备二次校验',
  `autoupdatercheck` varchar(16) DEFAULT NULL,
  `MFG_Permanent` varchar(8) DEFAULT NULL,
  `MFG_Validity_Start` datetime DEFAULT NULL,
  `MFG_Validity_End` datetime DEFAULT NULL,
  `Debug_Permanent` varchar(8) DEFAULT NULL,
  `Debug_Validity_Start` datetime DEFAULT NULL,
  `Debug_Validity_End` datetime DEFAULT NULL,
  `RD_Permanent` varchar(8) DEFAULT NULL,
  `RD_Validity_Start` datetime DEFAULT NULL,
  `RD_Validity_End` datetime DEFAULT NULL,
  `TrialRun_Permanent` varchar(8) DEFAULT NULL,
  `TrialRun_Validity_Start` datetime DEFAULT NULL,
  `TrialRun_Validity_End` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10281 DEFAULT CHARSET=gbk

CREATE TABLE `mims_role` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `description` varchar(255) DEFAULT NULL,
  `permission` tinyblob,
  `name` varchar(32) DEFAULT NULL,
  `type` int(11) DEFAULT NULL,
  `Quanxian` varchar(2000) DEFAULT NULL,
  `Remark` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=gbk