namespace SD3_Graph
{
	public struct TcpStatus1
	{
		public ushort TighteningIDset_00;

		public ushort ParamSeqSet_01;

		public ushort SeqID_02;

		public ushort ParamID_03;

		public ushort TargetTorque_04;

		public ushort TargetAngle_05;

		public ushort ParameterProgress_06;

		public ushort CurrentParameter_L_07;

		public ushort CurrentParameter_H_08;

		public ushort CurrentSequence_L_09;

		public ushort CurrentSequence_H_10;

		public ushort TighteningOKCnt_L_11;

		public ushort TighteningOKCnt_H_12;

		public ushort TighteningNOKCnt_L_13;

		public ushort TighteningNOKCnt_H_14;

		public ushort LooseningOKCnt_L_15;

		public ushort LooseningOKCnt_H_16;

		public ushort LooseningNOKCnt_L_17;

		public ushort LooseningNOKCnt_H_18;

		public ushort FinalStageMaxTorque_19;

		public ushort FinalStageMinTorque_20;

		public ushort CurrenttorqueUnit_21;

		public ushort TighteningLooseningInProgress_22;

		public ushort BitID_23;

		public ushort OperationalStatusVersion_24;

		public ushort ClearTheFlag_25;

		public ushort TotalScrewQty_L_26;

		public ushort TotalScrewQty_H_27;

		public ushort ParameterQtyOfCurrentSequence_28;

		public ushort ScrewQtyOfCurrentParameter_L_29;

		public ushort ScrewQtyOfCurrentParameter_H_30;

		public ushort AllScrewsOfCurrentParameterFinished_31;

		public ushort CurrentParameterFinished_32;

		public ushort CurrentScrewFinished_33;

		public ushort Waiting_34;

		public ushort FinalAndPrevailTorque_35;

		public short ActualAngle_36;

		public ushort TighteningAngle_37;

		public ushort TighteningResult_38;

		public ushort LooseningResult_39;

		public ushort CurveCreationFinished_40;

		public ushort RestrictTighteningStatus_41;

		public ushort ParameterSettingsOKNOK_42;

		public ushort FinalTorque_43;

		public ushort PrevailTorque_44;

		public ushort FinalCurrent_45;

		public ushort CauseToRestrictTighteningOperation_46;

		public ushort CauseToRestrictTooseningOperation_47;

		public ushort RemainingOperationTime_L_48;

		public ushort RemainingOperationTime_H_49;
	}
}
