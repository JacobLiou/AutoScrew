using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form712_ReportCurveAngTorq : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private UIReportStrc UI;

		private bool isSelecting = false;

		private Rectangle selectionRectangle;

		private int ISUSE_SWTORQ = 0;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private Button NextBn;

		private Button PrevBn;

		private Label CloseBn;

		private Chart chart1;

		private Label lab_Page;

		private Label lab_Chart1XY;

		private Button RstZoom1;

		public Form712_ReportCurveAngTorq(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV, UIReportStrc UI)
		{
			InitializeComponent();
			this.GB = GB;
			this.UI = UI;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			CreateGraph(true);
			ShowStageValue();
			chart1.MouseWheel += chart1_MouseWheel;
			chart1.MouseMove += chart1_MouseMove;
			chart1.MouseDown += chart1_MouseDown;
			chart1.MouseUp += chart1_MouseUp;
			chart1.Paint += chart1_Paint;
		}

		private void NextBn_Click(object sender, EventArgs e)
		{
			Form710_ReportInfo Form710 = new Form710_ReportInfo(GB, TCP, TrCSV, UI);
			Form710.Show();
			Close();
		}

		private void PrevBn_Click(object sender, EventArgs e)
		{
			Form711_ReportCurveTimeTorq Form711 = new Form711_ReportCurveTimeTorq(GB, TCP, TrCSV, UI);
			Form711.Show();
			Close();
		}

		private void Form712_ReportCurveAngTorq_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void CreateGraph(bool ForceRst)
		{
			bool IsScaleFromZero = false;
			uint TotalPoint = GB.ExFSReport.Scale[UI.AssignedRowNum].Curve_TotalPoint;
			ushort CoefUnit = GB.ExFSReport.Info[UI.AssignedRowNum].TorqueUnit;
			ushort CoefFWUnit = GB.ExFSReport.Info[UI.AssignedRowNum].FWSystemCoef;
			if (ForceRst)
			{
				IsScaleFromZero = GB.FSCtrlCurveScaleFromZero.Enable == 1;
				TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 10, (ushort)(CoefFWUnit * 100 + CoefUnit));
				TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 1, 0);
				TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 4, (ushort)(CoefFWUnit * 100 + CoefUnit));
				TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 5, (ushort)(CoefFWUnit * 100 + CoefUnit));
				TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 6, (ushort)(CoefFWUnit * 100 + CoefUnit));
				TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 7, (ushort)(CoefFWUnit * 100 + CoefUnit));
				TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 11, 0);
				if (TotalPoint > 2000)
				{
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 21, 0);
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 24, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 25, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 26, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 27, (ushort)(CoefFWUnit * 100 + CoefUnit));
				}
				if (TotalPoint > 4000)
				{
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 31, 0);
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 34, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 35, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 36, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 37, (ushort)(CoefFWUnit * 100 + CoefUnit));
				}
				if (TotalPoint > 6000)
				{
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 41, 0);
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 44, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 45, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 46, (ushort)(CoefFWUnit * 100 + CoefUnit));
					TCP.FSIDRead_ByTCP(751, 0, (ushort)(UI.AssignedRowNum + 1), (ushort)(UI.AssignedRowNum + 1 >> 16), 47, (ushort)(CoefFWUnit * 100 + CoefUnit));
				}
			}
			uint Length = GB.ExFSReport.Scale[UI.AssignedRowNum].Curve_TotalPoint;
			if (Length == 0)
			{
				return;
			}
			List<float> CurveAngle_f = new List<float>();
			List<float> CurveTorque_f = new List<float>();
			List<float> CurveTorqueRate_f = new List<float>();
			List<float> LimitStageH1_f = new List<float>();
			List<float> LimitStageV1_f = new List<float>();
			for (int j = 0; j < Length - 1; j++)
			{
				CurveAngle_f.Add(GB.ExFSReport.CurveAngle[j]);
				CurveTorque_f.Add((float)GB.ExFSReport.CurveTorque[j] / 1000f);
				CurveTorqueRate_f.Add((float)GB.ExFSReport.CurveTorqueRate[j] / 10000f);
			}
			string StrNull = "";
			ParamStucVer1[] ReportParam = new ParamStucVer1[1];
			ExParamStuc ExParam = default(ExParamStuc);
			TrCSV.TCPParamVSFSParam(false, 99, 0, ref StrNull, ref ReportParam, ref ExParam, ref GB.ExFSReport.ReportParam);
			ParamItemStucVer1[] ParamItem = new ParamItemStucVer1[6]
			{
				ReportParam[0].Item1,
				ReportParam[0].Item2,
				ReportParam[0].Item3,
				ReportParam[0].Item4,
				ReportParam[0].Item5,
				ReportParam[0].Item6
			};
			ISUSE_SWTORQ = GB.ParmIsUseSWTorqEn(ref ParamItem);
			LimitStageH1_f.Clear();
			LimitStageV1_f.Clear();
			float RstMinAngle = 0f;
			float RstMaxAngle = 0f;
			float CacheMode = 0f;
			float CacheTorqueRate_f = 0f;
			float RstTorqueRate_f = 0f;
			RstTorqueRate_f = -500f;
			for (int i = 0; i < Length - 1; i++)
			{
				if (RstMinAngle > (float)GB.ExFSReport.CurveAngle[i])
				{
					RstMinAngle = GB.ExFSReport.CurveAngle[i];
				}
				if (RstMaxAngle < (float)GB.ExFSReport.CurveAngle[i])
				{
					RstMaxAngle = GB.ExFSReport.CurveAngle[i];
				}
			}
			for (int k = 0; k < 6; k++)
			{
				CacheMode = (int)ParamItem[k].ControlMode_1;
				CacheTorqueRate_f = (float)ParamItem[k].TargetTorqueRate_DW_7 / 10000f;
				if (CacheMode == 2f)
				{
					RstTorqueRate_f = CacheTorqueRate_f;
				}
			}
			if (GB.ExFSReport.Scale[UI.AssignedRowNum].Stage1Angle == 0)
			{
				RstTorqueRate_f = -500f;
			}
			LimitStageH1_f.Add(RstMinAngle);
			LimitStageV1_f.Add(RstTorqueRate_f);
			LimitStageH1_f.Add(RstMaxAngle);
			LimitStageV1_f.Add(RstTorqueRate_f);
			Series series1 = new Series(MultiLanguage.GetStr("Form400_Results", "lab_Torque"));
			series1.ChartType = SeriesChartType.Line;
			series1.Color = Color.Purple;
			series1.BorderWidth = 2;
			series1.Points.DataBindXY(CurveAngle_f.ToArray(), CurveTorque_f.ToArray());
			series1.YAxisType = AxisType.Primary;
			Series series2 = new Series(MultiLanguage.GetStr("Form400_Results", "lab_TorqueRate"));
			series2.ChartType = SeriesChartType.Line;
			series2.Color = Color.Orange;
			series2.BorderWidth = 2;
			series2.Points.DataBindXY(CurveAngle_f.ToArray(), CurveTorqueRate_f.ToArray());
			series2.YAxisType = AxisType.Secondary;
			Series series3 = new Series(MultiLanguage.GetStr("Form400_Results", "lab_TorqueRateSetting"));
			series3.ChartType = SeriesChartType.Line;
			series3.Color = Color.FromArgb(255, 0, 0);
			series3.BorderWidth = 1;
			series3.Points.DataBindXY(LimitStageH1_f.ToArray(), LimitStageV1_f.ToArray());
			series3.YAxisType = AxisType.Secondary;
			double RstMaxAng = GB.ExFSReport.Scale[UI.AssignedRowNum].Curve_MaxAngle;
			double RstMinAng = GB.ExFSReport.Scale[UI.AssignedRowNum].Curve_MinAngle;
			double RstMaxTorq = (double)GB.ExFSReport.Scale[UI.AssignedRowNum].Curve_MaxTorque_DW / 1000.0;
			double RstMinTorq = (double)GB.ExFSReport.Scale[UI.AssignedRowNum].Curve_MinTorque_DW / 1000.0;
			double RstMaxTorqRate = (double)GB.ExFSReport.Scale[UI.AssignedRowNum].Curve_MaxTorqueRate_DW / 10000.0;
			double RstMinTorqRate = (double)GB.ExFSReport.Scale[UI.AssignedRowNum].Curve_MinTorqueRate_DW / 10000.0;
			double MaxAngle = ((RstMaxAng < 0.01) ? 0.01 : RstMaxAng);
			double MinAngle = ((RstMinAng >= 0.0) ? 0.0 : RstMinAng);
			double MaxTorque = ((RstMaxTorq < 0.01) ? 0.01 : RstMaxTorq);
			double MinTorque = ((IsScaleFromZero && GB.ExFSReport.Scale[UI.AssignedRowNum].Stage1Angle != 0) ? 0.0 : ((RstMinTorq >= 0.0) ? (-0.0010000000474974513) : RstMinTorq));
			double MaxTorqueRate = ((RstMaxTorqRate < 0.01) ? 0.01 : RstMaxTorqRate);
			double MinTorqueRate = ((RstMinTorqRate >= 0.0) ? (-0.0010000000474974513 * GB.TorqUnitcoef(1000 + CoefUnit)) : RstMinTorqRate);
			string TorqUnitStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.ExFSReport.Info[UI.AssignedRowNum].TorqueUnit);
			ChartArea chartArea = new ChartArea();
			chartArea.AxisX.Title = MultiLanguage.GetStr("Form400_Results", "lab_Angle");
			chartArea.AxisY.Title = MultiLanguage.GetStr("Form400_Results", "lab_Torque") + "(" + TorqUnitStr + ")";
			chartArea.AxisY2.Title = MultiLanguage.GetStr("Form400_Results", "lab_TorqueRate") + "(" + TorqUnitStr + "/°)";
			chartArea.AxisY2.Enabled = AxisEnabled.True;
			chartArea.AxisX.Minimum = Math.Floor(MinAngle * 100.0) / 100.0;
			chartArea.AxisX.Maximum = Math.Ceiling(MaxAngle * 100.0) / 100.0;
			chartArea.AxisX.Interval = (chartArea.AxisX.Maximum - chartArea.AxisX.Minimum) / 10.0;
			chartArea.AxisY.Minimum = Math.Floor(MinTorque * 100.0) / 100.0;
			chartArea.AxisY.Maximum = Math.Ceiling(MaxTorque * 100.0) / 100.0;
			chartArea.AxisY.Interval = (chartArea.AxisY.Maximum - chartArea.AxisY.Minimum) / 10.0;
			chartArea.AxisY2.Minimum = Math.Floor(MinTorqueRate * 1000.0) / 1000.0;
			chartArea.AxisY2.Maximum = Math.Ceiling(MaxTorqueRate * 1000.0) / 1000.0;
			chartArea.AxisY2.Interval = (chartArea.AxisY2.Maximum - chartArea.AxisY2.Minimum) / 10.0;
			chartArea.InnerPlotPosition.Auto = false;
			chartArea.InnerPlotPosition.Width = 75f;
			chartArea.InnerPlotPosition.Height = 80f;
			chartArea.InnerPlotPosition.X = 12f;
			chartArea.InnerPlotPosition.Y = 3f;
			chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
			chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
			chartArea.AxisX2.MajorGrid.LineColor = Color.LightGray;
			chartArea.AxisY2.MajorGrid.LineColor = Color.LightGray;
			chart1.Series.Clear();
			chart1.Series.Add(series1);
			chart1.Series.Add(series2);
			chart1.Series.Add(series3);
			chart1.ChartAreas.Clear();
			chart1.ChartAreas.Add(chartArea);
			chart1.ChartAreas[0].Position = new ElementPosition(0f, 10f, 100f, 90f);
		}

		private void ShowStageValue()
		{
			ReportInfoStuc Info = GB.ExFSReport.Info[UI.AssignedRowNum];
			ReportScaleStuc Scale = GB.ExFSReport.Scale[UI.AssignedRowNum];
			if (Info.Status == 3 || Info.Status == 4)
			{
				int Loop = 0;
				if (GB.ExFSReport.Scale[UI.AssignedRowNum].Loosening1Time > 0)
				{
					Loop = 0;
				}
				if (GB.ExFSReport.Scale[UI.AssignedRowNum].Loosening2Time > 0)
				{
					Loop = 1;
				}
				for (int i = 0; i <= Loop; i++)
				{
					TextAnnotation ChartText = new TextAnnotation();
					if (i == 0)
					{
						ChartText.Text = GB.AddStageRow(7, Scale.Loosening1Angle, Scale.Loosening1Torque_DW, Scale.Loosening1Time, 0, false, 1f);
					}
					else
					{
						ChartText.Text = GB.AddStageRow(8, Scale.Loosening2Angle, Scale.Loosening2Torque_DW, Scale.Loosening2Time, 0, false, 1f);
					}
					ChartText.X = 14.0;
					ChartText.Y = 21 + 6 * i;
					ChartText.Font = new Font("Arial", 10f, FontStyle.Regular);
					chart1.Annotations.Add(ChartText);
				}
				return;
			}
			int Loop2 = 0;
			if (GB.ExFSReport.Scale[UI.AssignedRowNum].Stage1Time > 0)
			{
				Loop2 = 0;
			}
			if (GB.ExFSReport.Scale[UI.AssignedRowNum].Stage2Time > 0)
			{
				Loop2 = 1;
			}
			if (GB.ExFSReport.Scale[UI.AssignedRowNum].Stage3Time > 0)
			{
				Loop2 = 2;
			}
			if (GB.ExFSReport.Scale[UI.AssignedRowNum].Stage4Time > 0)
			{
				Loop2 = 3;
			}
			if (GB.ExFSReport.Scale[UI.AssignedRowNum].Stage5Time > 0)
			{
				Loop2 = 4;
			}
			if (GB.ExFSReport.Scale[UI.AssignedRowNum].Stage6Time > 0)
			{
				Loop2 = 5;
			}
			TextAnnotation ChartText2 = new TextAnnotation();
			ChartText2.Text = GB.AddStageSnugRow(1, 1000, UI.AssignedRowNum);
			ChartText2.X = 14.0;
			ChartText2.Y = 1.0;
			ChartText2.Font = new Font("Arial", 10f, FontStyle.Regular);
			ChartText2.ForeColor = Color.Red;
			chart1.Annotations.Add(ChartText2);
			TextAnnotation ChartText3 = new TextAnnotation();
			ChartText3.Text = GB.AddStageSnugRow(1, 1001, UI.AssignedRowNum);
			ChartText3.X = 14.0;
			ChartText3.Y = 7.0;
			ChartText3.Font = new Font("Arial", 10f, FontStyle.Regular);
			ChartText3.ForeColor = Color.Blue;
			chart1.Annotations.Add(ChartText3);
			bool ShowSWTorqEn = ((ISUSE_SWTORQ > 0) ? true : false);
			for (int j = 0; j <= Loop2; j++)
			{
				TextAnnotation ChartText4 = new TextAnnotation();
				switch (j)
				{
				case 0:
					ChartText4.Text = GB.AddStageRow(1, Scale.Stage1Angle, Scale.Stage1Torque_DW, Scale.Stage1Time, Scale.Stage1SwitchTorq_DW, ShowSWTorqEn, 1f);
					break;
				case 1:
					ChartText4.Text = GB.AddStageRow(2, Scale.Stage2Angle, Scale.Stage2Torque_DW, Scale.Stage2Time, Scale.Stage2SwitchTorq_DW, ShowSWTorqEn, 1f);
					break;
				case 2:
					ChartText4.Text = GB.AddStageRow(3, Scale.Stage3Angle, Scale.Stage3Torque_DW, Scale.Stage3Time, Scale.Stage3SwitchTorq_DW, ShowSWTorqEn, 1f);
					break;
				case 3:
					ChartText4.Text = GB.AddStageRow(4, Scale.Stage4Angle, Scale.Stage4Torque_DW, Scale.Stage4Time, Scale.Stage4SwitchTorq_DW, ShowSWTorqEn, 1f);
					break;
				case 4:
					ChartText4.Text = GB.AddStageRow(5, Scale.Stage5Angle, Scale.Stage5Torque_DW, Scale.Stage5Time, Scale.Stage5SwitchTorq_DW, ShowSWTorqEn, 1f);
					break;
				default:
					ChartText4.Text = GB.AddStageRow(6, Scale.Stage6Angle, Scale.Stage6Torque_DW, Scale.Stage6Time, Scale.Stage6SwitchTorq_DW, ShowSWTorqEn, 1f);
					break;
				}
				ChartText4.X = 14.0;
				ChartText4.Y = 21 + 6 * j;
				ChartText4.Font = new Font("Arial", 10f, FontStyle.Regular);
				chart1.Annotations.Add(ChartText4);
			}
		}

		private void chart1_MouseWheel(object sender, MouseEventArgs e)
		{
			Axis xAxis = chart1.ChartAreas[0].AxisX;
			Axis yAxis = chart1.ChartAreas[0].AxisY;
			Axis y2Axis = chart1.ChartAreas[0].AxisY2;
			double xRange = xAxis.Maximum - xAxis.Minimum;
			double yRange = yAxis.Maximum - yAxis.Minimum;
			double y2Range = y2Axis.Maximum - y2Axis.Minimum;
			double xZoomFactor = ((e.Delta > 0) ? 0.9 : 1.1);
			double yZoomFactor = ((e.Delta > 0) ? 0.9 : 1.1);
			double xZoomOffset = xRange / 2.0 * (1.0 - xZoomFactor);
			double yZoomOffset = yRange / 2.0 * (1.0 - yZoomFactor);
			double y2ZoomOffset = y2Range / 2.0 * (1.0 - yZoomFactor);
			double newXMin = xAxis.Minimum + xZoomOffset;
			double newXMax = xAxis.Maximum - xZoomOffset;
			double newYMin = yAxis.Minimum + yZoomOffset;
			double newYMax = yAxis.Maximum - yZoomOffset;
			double newY2Min = y2Axis.Minimum + y2ZoomOffset;
			double newY2Max = y2Axis.Maximum - y2ZoomOffset;
			xAxis.Minimum = Math.Floor(newXMin * 1000.0) / 1000.0;
			xAxis.Maximum = Math.Ceiling(newXMax * 1000.0) / 1000.0;
			yAxis.Minimum = Math.Floor(newYMin * 1000.0) / 1000.0;
			yAxis.Maximum = Math.Ceiling(newYMax * 1000.0) / 1000.0;
			y2Axis.Minimum = Math.Floor(newY2Min * 1000.0) / 1000.0;
			y2Axis.Maximum = Math.Ceiling(newY2Max * 1000.0) / 1000.0;
		}

		private void chart1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isSelecting = true;
				selectionRectangle = new Rectangle(e.Location, default(Size));
				chart1.Cursor = Cursors.Cross;
			}
		}

		private void chart1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isSelecting = false;
				chart1.Cursor = Cursors.Default;
				if (selectionRectangle.Width > 0 && selectionRectangle.Height > 0)
				{
					Axis xAxis = chart1.ChartAreas[0].AxisX;
					Axis yAxis = chart1.ChartAreas[0].AxisY;
					Axis y2Axis = chart1.ChartAreas[0].AxisY2;
					double xMin = xAxis.PixelPositionToValue(selectionRectangle.Left);
					double xMax = xAxis.PixelPositionToValue(selectionRectangle.Right);
					double yMin = yAxis.PixelPositionToValue(selectionRectangle.Bottom);
					double yMax = yAxis.PixelPositionToValue(selectionRectangle.Top);
					double y2Min = y2Axis.PixelPositionToValue(selectionRectangle.Bottom);
					double y2Max = y2Axis.PixelPositionToValue(selectionRectangle.Top);
					xAxis.Minimum = Math.Floor(xMin * 1000.0) / 1000.0;
					xAxis.Maximum = Math.Ceiling(xMax * 1000.0) / 1000.0;
					yAxis.Minimum = Math.Floor(yMin * 1000.0) / 1000.0;
					yAxis.Maximum = Math.Ceiling(yMax * 1000.0) / 1000.0;
					y2Axis.Minimum = Math.Floor(y2Min * 1000.0) / 1000.0;
					y2Axis.Maximum = Math.Ceiling(y2Max * 1000.0) / 1000.0;
				}
				chart1.Refresh();
			}
		}

		private void chart1_MouseMove(object sender, MouseEventArgs e)
		{
			HitTestResult result = chart1.HitTest(e.X, e.Y);
			if (result.ChartElementType == ChartElementType.DataPoint)
			{
				DataPoint dataPoint = chart1.Series[result.Series.Name].Points[result.PointIndex];
				double xValue = dataPoint.XValue;
				double yValue = dataPoint.YValues[0];
				lab_Chart1XY.Text = result.Series.Name + "Ang:" + xValue.ToString("F0") + " Torq:" + yValue.ToString("F3");
			}
			if (isSelecting)
			{
				selectionRectangle.Width = e.X - selectionRectangle.X;
				selectionRectangle.Height = e.Y - selectionRectangle.Y;
				chart1.Refresh();
			}
		}

		private void chart1_Paint(object sender, PaintEventArgs e)
		{
			if (isSelecting)
			{
				using (Pen pen = new Pen(Color.Gray, 1f))
				{
					pen.DashStyle = DashStyle.Dot;
					e.Graphics.DrawRectangle(pen, selectionRectangle);
				}
			}
		}

		private void RstZoom1_Click(object sender, EventArgs e)
		{
			CreateGraph(false);
		}

		private void Form712_ReportCurveAngTorq_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.lab_Page = new System.Windows.Forms.Label();
			this.NextBn = new System.Windows.Forms.Button();
			this.PrevBn = new System.Windows.Forms.Button();
			this.lab_Chart1XY = new System.Windows.Forms.Label();
			this.RstZoom1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)this.chart1).BeginInit();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, -1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(947, 35);
			this.lab_HanderTitle.TabIndex = 58;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(916, 3);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 125;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			chartArea1.Name = "ChartArea1";
			chartArea1.Position.Auto = false;
			chartArea1.Position.Height = 90f;
			chartArea1.Position.Width = 100f;
			chartArea1.Position.Y = 10f;
			this.chart1.ChartAreas.Add(chartArea1);
			legend1.BackColor = System.Drawing.Color.Transparent;
			legend1.DockedToChartArea = "ChartArea1";
			legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
			legend1.Name = "Legend1";
			this.chart1.Legends.Add(legend1);
			this.chart1.Location = new System.Drawing.Point(44, 74);
			this.chart1.Name = "chart1";
			series1.ChartArea = "ChartArea1";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series1.Legend = "Legend1";
			series1.Name = "Series1";
			this.chart1.Series.Add(series1);
			this.chart1.Size = new System.Drawing.Size(858, 397);
			this.chart1.TabIndex = 165;
			this.chart1.Text = "chart1";
			this.lab_Page.BackColor = System.Drawing.Color.White;
			this.lab_Page.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lab_Page.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Page.Location = new System.Drawing.Point(428, 488);
			this.lab_Page.Name = "lab_Page";
			this.lab_Page.Size = new System.Drawing.Size(80, 31);
			this.lab_Page.TabIndex = 167;
			this.lab_Page.Text = "3";
			this.lab_Page.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.NextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.NextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.NextBn.FlatAppearance.BorderSize = 0;
			this.NextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NextBn.ForeColor = System.Drawing.Color.Transparent;
			this.NextBn.Location = new System.Drawing.Point(600, 488);
			this.NextBn.Name = "NextBn";
			this.NextBn.Size = new System.Drawing.Size(40, 40);
			this.NextBn.TabIndex = 60;
			this.NextBn.UseVisualStyleBackColor = true;
			this.NextBn.Click += new System.EventHandler(NextBn_Click);
			this.PrevBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.PrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.PrevBn.FlatAppearance.BorderSize = 0;
			this.PrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PrevBn.ForeColor = System.Drawing.Color.Transparent;
			this.PrevBn.Location = new System.Drawing.Point(295, 488);
			this.PrevBn.Name = "PrevBn";
			this.PrevBn.Size = new System.Drawing.Size(40, 40);
			this.PrevBn.TabIndex = 61;
			this.PrevBn.UseVisualStyleBackColor = true;
			this.PrevBn.Click += new System.EventHandler(PrevBn_Click);
			this.lab_Chart1XY.BackColor = System.Drawing.Color.White;
			this.lab_Chart1XY.Location = new System.Drawing.Point(682, 456);
			this.lab_Chart1XY.Name = "lab_Chart1XY";
			this.lab_Chart1XY.Size = new System.Drawing.Size(220, 15);
			this.lab_Chart1XY.TabIndex = 171;
			this.lab_Chart1XY.Text = "(0.0, 0.0)";
			this.lab_Chart1XY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.RstZoom1.BackgroundImage = SD3Soft.Properties.Resources.放大縮小;
			this.RstZoom1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstZoom1.FlatAppearance.BorderSize = 0;
			this.RstZoom1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstZoom1.Location = new System.Drawing.Point(862, 73);
			this.RstZoom1.Name = "RstZoom1";
			this.RstZoom1.Size = new System.Drawing.Size(40, 40);
			this.RstZoom1.TabIndex = 172;
			this.RstZoom1.UseVisualStyleBackColor = true;
			this.RstZoom1.Click += new System.EventHandler(RstZoom1_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(947, 545);
			base.Controls.Add(this.RstZoom1);
			base.Controls.Add(this.lab_Chart1XY);
			base.Controls.Add(this.lab_Page);
			base.Controls.Add(this.chart1);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.NextBn);
			base.Controls.Add(this.PrevBn);
			base.Controls.Add(this.lab_HanderTitle);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form712_ReportCurveAngTorq";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form712_ReportCurveAngTorq";
			base.Load += new System.EventHandler(Form712_ReportCurveAngTorq_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form712_ReportCurveAngTorq_Paint);
			((System.ComponentModel.ISupportInitialize)this.chart1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
