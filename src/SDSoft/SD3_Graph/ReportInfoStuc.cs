using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct ReportInfoStuc
	{
		[FieldOffset(0)]
		public unsafe fixed ushort Data16[300];

		[FieldOffset(0)]
		public unsafe fixed ushort SaveStr[100];

		[FieldOffset(200)]
		public ushort Year;

		[FieldOffset(202)]
		public ushort Month;

		[FieldOffset(204)]
		public ushort Day;

		[FieldOffset(206)]
		public ushort Hour;

		[FieldOffset(208)]
		public ushort Min;

		[FieldOffset(210)]
		public ushort Sec;

		[FieldOffset(212)]
		public ushort Tool;

		[FieldOffset(214)]
		public uint ScrewNo;

		[FieldOffset(218)]
		public ushort SeqID;

		[FieldOffset(220)]
		public ushort ParmID;

		[FieldOffset(222)]
		public ushort TargetTorque;

		[FieldOffset(224)]
		public ushort TargetAngle;

		[FieldOffset(226)]
		public ushort TargetTorqueRate;

		[FieldOffset(228)]
		public ushort FinalTorque;

		[FieldOffset(230)]
		public ushort TighteningAngle;

		[FieldOffset(232)]
		public short TotalAngle;

		[FieldOffset(234)]
		public ushort Status;

		[FieldOffset(236)]
		public ushort CT_Time;

		[FieldOffset(238)]
		public ushort ErrorCode;

		[FieldOffset(240)]
		public ushort MaxTighteningAngle;

		[FieldOffset(242)]
		public ushort MinTighteningAngle;

		[FieldOffset(244)]
		public ushort MaxTorque;

		[FieldOffset(246)]
		public ushort MinTorque;

		[FieldOffset(248)]
		public ushort TorqueUnit;

		[FieldOffset(250)]
		public ushort ToolMaxTorque_NM;

		[FieldOffset(252)]
		public ushort ToolProtectTorque;

		[FieldOffset(254)]
		public ushort PreTighteningTorque;

		[FieldOffset(256)]
		public ushort SetMaxTime;

		[FieldOffset(258)]
		public ushort SetMaxAngle;

		[FieldOffset(260)]
		public ushort FinalStage_SetMaxTorque;

		[FieldOffset(262)]
		public ushort FinalStage_SetMinTorque;

		[FieldOffset(264)]
		public ushort FinalStage_SetMaxAngle;

		[FieldOffset(266)]
		public ushort FinalStage_SetMinAngle;

		[FieldOffset(268)]
		public ushort FinalStage_SetMaxTime;

		[FieldOffset(270)]
		public ushort FinalStage_SetMinTime;

		[FieldOffset(272)]
		public ushort PrevailTorque;

		[FieldOffset(274)]
		public ushort AppliedTorque;

		[FieldOffset(276)]
		public ushort FinalCurrent;

		[FieldOffset(278)]
		public ushort ClampTorque;

		[FieldOffset(280)]
		public ushort SetMaxClampTorque;

		[FieldOffset(282)]
		public ushort SetMinClampTorque;

		[FieldOffset(284)]
		public ushort ClampAngle;

		[FieldOffset(286)]
		public ushort SetMaxClampAngle;

		[FieldOffset(288)]
		public ushort SetMinClampAngle;

		[FieldOffset(290)]
		public ushort SetMinAngle;

		[FieldOffset(292)]
		public ushort UserID;

		[FieldOffset(294)]
		public ushort FWSystemCoef;

		[FieldOffset(296)]
		public ushort TargetYield;

		[FieldOffset(298)]
		public unsafe fixed ushort Rreserve[4];

		[FieldOffset(306)]
		public uint TargetTorque_DW;

		[FieldOffset(310)]
		public uint TargetTorqueRate_DW;

		[FieldOffset(314)]
		public uint FinalTorque_DW;

		[FieldOffset(318)]
		public uint MaxTorque_DW;

		[FieldOffset(322)]
		public uint MinTorque_DW;

		[FieldOffset(326)]
		public uint PreTighteningTorque_DW;

		[FieldOffset(330)]
		public uint FinalStage_SetMaxTorque_DW;

		[FieldOffset(334)]
		public uint FinalStage_SetMinTorque_DW;

		[FieldOffset(338)]
		public uint PrevailTorque_DW;

		[FieldOffset(342)]
		public uint AppliedTorque_DW;

		[FieldOffset(346)]
		public uint ClampTorque_DW;

		[FieldOffset(350)]
		public uint SetMaxClampTorque_DW;

		[FieldOffset(354)]
		public uint SetMinClampTorque_DW;
	}
}
