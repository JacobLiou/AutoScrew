using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ScrewTcpType
	{
		public const int Ver0 = 0;

		public const int Ver1 = 1;

		public const int None_0 = 0;

		public const int ClosePupWindowPage_5 = 5;

		public const int SubscriptionTCPStart_10 = 10;

		public const int SubscriptionTCPStop_11 = 11;

		public const int SubscriptionUDPStart_12 = 12;

		public const int SubscriptionUDPStop_13 = 13;

		public const int TCPFooterEndSignStart_14 = 14;

		public const int TCPFooterEndSignStop_15 = 15;

		public const int WriteTCPAddrOpreation_20 = 20;

		public const int WriteMappingSetting_21 = 21;

		public const int WriteActiveFeedbackTCPMode_30 = 30;

		public const int ReadRequest_50 = 50;

		public const int ReadTCPAddrOpreation_51 = 51;

		public const int ReadMappingSetting_52 = 52;

		public const int ReadLocalAddrInfo_53 = 53;

		public const int WriteLocalAddrInfo_54 = 54;

		public const int ReadCheckGetResultActive_60 = 60;

		public const int ReadActiveFeedbackTCPMode_80 = 80;

		public const int ReadReflashStatus_81 = 81;

		public const int ReadPerpendicularityVal_94 = 94;

		public const int WriteTheParameters_100 = 100;

		public const int WriteToDeleteParameters_110 = 110;

		public const int ReadTheParameters_150 = 150;

		public const int ReadParamIDIsUsed_160 = 160;

		public const int WriteTheSequence_200 = 200;

		public const int WriteTheNavigationCoordinates_201 = 201;

		public const int WriteTheNavigationPictureID_202 = 202;

		public const int WriteArmCoordinates_203 = 203;

		public const int WriteToDeleteSequence_210 = 210;

		public const int WritePictureBitMap_211 = 211;

		public const int ReadTheSequence_250 = 250;

		public const int ReadTheNavigationCoordinates_251 = 251;

		public const int ReadTheNavigationPictureID_252 = 252;

		public const int ReadArmCoordinates_253 = 253;

		public const int ReadSeqIDIsUsed_260 = 260;

		public const int ReadPictureBitMap_261 = 261;

		public const int ReadRemainSpaceSize_262 = 262;

		public const int WriteTheOperationModeAndSwitchingMethodOfSources_300 = 300;

		public const int WriteTheContentsOfSingleSourceSettings_301 = 301;

		public const int WriteTheParameterIDInManualMode_302 = 302;

		public const int WriteTheSequenceIDInManualMode_303 = 303;

		public const int WriteToDeleteSources_310 = 310;

		public const int ReadTheOperationModeAndSwitchingMethodOfSources_350 = 350;

		public const int ReadTheContentsOfSingleSourceSettings_351 = 351;

		public const int WriteTheSwitchingMethodOfSources_400 = 400;

		public const int WriteTheScannerString_401 = 401;

		public const int WriteToClearAllErrors_402 = 402;

		public const int WriteToResetTheOperationProgress_403 = 403;

		public const int WriteToExecuteThePreviousStep_404 = 404;

		public const int WriteToExecuteTheNextStep_405 = 405;

		public const int WriteToRestrictTighteningOperation_406 = 406;

		public const int WriteToRestrictLooseningOperation_407 = 407;

		public const int WriteTheScannerAdvancedSettings_408 = 408;

		public const int WriteToClearTheCountOfSingleNOKScrewTightening_409 = 409;

		public const int WriteToClearTheCountOfSingleNOKScrewLoosening_410 = 410;

		public const int WriteToResetRemainingOperationTime_411 = 411;

		public const int WriteToResetStatus_412 = 412;

		public const int ReadTheSwitchingMethodOfSources_450 = 450;

		public const int ReadTheScannerString_451 = 451;

		public const int ReadTheScannerAdvancedSettings_452 = 452;

		public const int ReadTheLEDStatus_453 = 453;

		public const int ReadTheCurrentSNReportScaleParam_498 = 498;

		public const int ReadTheCurrentCurve_499 = 499;

		public const int WriteTheRequestForPermissionsLogin_500 = 500;

		public const int WriteTheRequestForPasswordChange_501 = 501;

		public const int WriteTheRequestForPermissionsLogout_502 = 502;

		public const int WriteThePagePermissions_503 = 503;

		public const int WriteTheEthernetSettings_504 = 504;

		public const int WriteTheRequestForFactoryReset_505 = 505;

		public const int WriteTheBuzzerSoundPattern_506 = 506;

		public const int WriteTheDIDOFunctions_507 = 507;

		public const int WriteTheDIDOConversionTable_508 = 508;

		public const int WriteTheDefaultTorqueUnit_509 = 509;

		public const int WriteTheDefaultToolStartCondition_510 = 510;

		public const int WriteTheSingleDIDOFunction_511 = 511;

		public const int WriteToExportInformation_512 = 512;

		public const int WriteImportInformation_513 = 513;

		public const int WriteTwoStageModeUnderSelfDefinedTorqueControl_514 = 514;

		public const int WriteDisplayOperationWarningWindow_515 = 515;

		public const int WriteDisplayTheLimitsOfAllStagesForCurves_516 = 516;

		public const int WriteExportResultFileForEachScrew_517 = 517;

		public const int WriteSamplingRateForCurves_518 = 518;

		public const int WriteAlwaysMonitorTheToolCurrent_519 = 519;

		public const int WriteCompensationForToolTemperatureRise_520 = 520;

		public const int WriteComPortSetting_521 = 521;

		public const int WriteSendResultTCPForEachScrew_522 = 522;

		public const int WriteTighteningParameterDoesNotMatchToolCheck_523 = 523;

		public const int WriteTheDefaultAngleUnit_524 = 524;

		public const int WriteBuzzerVolume_525 = 525;

		public const int WriteDisplayHDMIMode_526 = 526;

		public const int WriteHomePage_527 = 527;

		public const int WriteRS485Function_528 = 528;

		public const int WriteCurveAlwaysPositive_529 = 529;

		public const int WriteDefaultLoosSPD_530 = 530;

		public const int WriteKeyboardCursorBlinkingInResults_531 = 531;

		public const int WriteTorqueRateReplaceByVelocityCurve_532 = 532;

		public const int WriteProhibitOperationNC_533 = 533;

		public const int WriteDIResponseFilterTime_534 = 534;

		public const int WriteCtrlModelName_535 = 535;

		public const int WriteTorqScaleFromZero_536 = 536;

		public const int WriteMCURangeDetect_537 = 537;

		public const int WriteDOTimer_538 = 538;

		public const int WriteEarlyWindowForm_539 = 539;

		public const int WriteCurveCutoffPoint_540 = 540;

		public const int WriteMCUDetectionSwitch_541 = 541;

		public const int WriteAlarmClearRemoteOrDIProhibit_542 = 542;

		public const int WriteSpeedLimitInTheFinalStage_543 = 543;

		public const int WriteToolHealthDiagnosis_544 = 544;

		public const int ReadTheEthernetSettings_550 = 550;

		public const int ReadThePagePermissions_551 = 551;

		public const int ReadTheFirmwareVersion_552 = 552;

		public const int ReadTheDIDOFunctions_553 = 553;

		public const int ReadTheDIDOConversionTable_554 = 554;

		public const int ReadTheDefaultTorqueUnit_555 = 555;

		public const int ReadTheDefaultToolStartCondition_556 = 556;

		public const int ReadTheSingleDIDOFunction_557 = 557;

		public const int ReadTwoStageModeUnderSelfDefinedTorqueControl_558 = 558;

		public const int ReadDisplayOperationWarningWindow_559 = 559;

		public const int ReadDisplayTheLimitsOfAlStagesForCurves_560 = 560;

		public const int ReadExportResultFileForEachScrew_561 = 561;

		public const int ReadSamplingRateForCurves_562 = 562;

		public const int ReadAlwaysMonitorTheToolCurrent_563 = 563;

		public const int ReadCompensationForToolTemperatureRise_564 = 564;

		public const int ReadComPortFunction_565 = 565;

		public const int ReadSendResultTCPForEachScrew_566 = 566;

		public const int ReadCheckThatTighteningParameterDoNotMatchToolSpec_567 = 567;

		public const int ReadAngleUnit_568 = 568;

		public const int ReadBuzzerVolume_569 = 569;

		public const int ReadDisplayHDMIMode_570 = 570;

		public const int ReadHomePage_571 = 571;

		public const int ReadRS485Function_572 = 572;

		public const int ReadCurveAlwaysPositive_573 = 573;

		public const int ReadDefaultLoosSPD_574 = 574;

		public const int ReadKeyboardCursorBlinkingInResults_575 = 575;

		public const int ReadTorqueRateReplaceByVelocityCurve_576 = 576;

		public const int ReadProhibitOperationNC_577 = 577;

		public const int ReadDIResponseFilterTime_578 = 578;

		public const int ReadMAC_579 = 579;

		public const int ReadCtrlModelName_580 = 580;

		public const int ReadTorqScaleFromZero_581 = 581;

		public const int ReadMCURangeDetect_582 = 582;

		public const int ReadDOTimer_583 = 583;

		public const int ReadEarlyWindowForm_584 = 584;

		public const int ReadBuzzerType_585 = 585;

		public const int ReadCurveCutoffPoint_586 = 586;

		public const int ReadMCUDetectionSwitch_587 = 587;

		public const int ReadAlarmClearRemoteOrDIProhibit_588 = 588;

		public const int ReadSpeedLimitInTheFinalStage_589 = 589;

		public const int ReadToolHealthDiagnosis_590 = 590;

		public const int ReadCtrlInfoSpec_599 = 599;

		public const int WriteToActivateTheTool_600 = 600;

		public const int WriteToEnableServiceReminder_601 = 601;

		public const int WriteTheLeverStartLevel_602 = 602;

		public const int WriteThePushStartLevel_603 = 603;

		public const int WriteTheWorkLightBrightness_604 = 604;

		public const int WriteTheLEDLightSettings_606 = 606;

		public const int WriteTheToolCalibration_607 = 607;

		public const int WriteToolRemindCnt_608 = 608;

		public const int WriteMaxAngForToolRotationDetect_609 = 609;

		public const int WriteToolTempLevel_610 = 610;

		public const int WriteToolLedDelayTmr_611 = 611;

		public const int ReadTheToolInformation_650 = 650;

		public const int ReadTheLeverStartLevel_651 = 651;

		public const int ReadThePushStartLevel_652 = 652;

		public const int ReadTheWorkLightBrightness_653 = 653;

		public const int ReadTheLEDLightSettings_655 = 655;

		public const int ReadTheToolCalibration_656 = 656;

		public const int ReadTheToolVersion_657 = 657;

		public const int ReadTheToolServiceReminder_658 = 658;

		public const int ReadTheToolActive_659 = 659;

		public const int ReadToolRemindCnt_660 = 660;

		public const int ReadToolTeachRecord_661 = 661;

		public const int ReadMaxAngForToolRotationDetect_662 = 662;

		public const int ReadToolTempLevel_663 = 663;

		public const int ReadToolLedDelayTmr_664 = 664;

		public const int ReadTheToolInfoSpec_699 = 699;

		public const int ClearTheProductionReportEntries_700 = 700;

		public const int ClearTheErrorAndWarningReportEntries_701 = 701;

		public const int ClearTheProductionReportFiles_702 = 702;

		public const int FindAndReadTheProductionReportEntries_750 = 750;

		public const int FindAndReadCurves_751 = 751;

		public const int FindAndReadTheErrorReportEntries_752 = 752;

		public const int FindAndReadTheWarningReportEntries_753 = 753;

		public const int FindAndReadTheButtonReportEntries_754 = 754;
	}
}
