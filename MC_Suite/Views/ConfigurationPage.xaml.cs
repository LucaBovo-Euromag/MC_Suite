﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using System.Windows.Input;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using MC_Suite.Services;
using MC_Suite.Properties;
using MC_Suite.Euromag.Devices;
using MC_Suite.Euromag.Protocols;
using MC_Suite.Euromag.Protocols.StdCommands;
using Microsoft.Toolkit.Uwp;
using Windows.Storage;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace MC_Suite.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class ConfigurationPage : Page 
    {
       
        public ConfigurationPage()
        {
            this.InitializeComponent();
            InitComboBoxes();
            InitUIElements();
        }

        #region Istanze

        public IrCOMPortManager IrConnection
        {
            get
            {
                return IrCOMPortManager.Instance;
            }
        }

        public TargetVariablesFields Fields
        {
            get
            {
                return TargetVariablesFields.Instance;
            }
        }

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
            }
        }

        #endregion

        #region FlowrateMeasurementOptions

        private ICommand _setTotalizerTUCmd;
        public ICommand SetTotalizerTUCmd
        {
            get
            {
                if (_setTotalizerTUCmd == null)
                {
                    _setTotalizerTUCmd = new RelayCommand(
                        param => SetTotalizerTU()
                            );
                }
                return _setTotalizerTUCmd;
            }
        }

        private void SetTotalizerTU()
        {
            Fields.AccumulatorsTechUnit.Value = (byte)(TotalizerTUbox.SelectedIndex + 2);
            UpdateParam(Fields.AccumulatorsTechUnit, Fields.AccumulatorsTechUnit.Value.ToString());
        }

        private ICommand _setFlowrateTBCmd;
        public ICommand SetFlowrateTBCmd
        {
            get
            {
                if (_setFlowrateTBCmd == null)
                {
                    _setFlowrateTBCmd = new RelayCommand(
                        param => SetFlowrateTB()
                            );
                }
                return _setFlowrateTBCmd;
            }
        }

        private void SetFlowrateTB()
        {
            Fields.FlowrateTimeBase.Value = (byte)(FlowrateTBbox.SelectedIndex + 1);
            UpdateParam(Fields.FlowrateTimeBase, Fields.FlowrateTimeBase.Value.ToString());
        }

        private ICommand _setFlowrateTUCmd;
        public ICommand SetFlowrateTUCmd
        {
            get
            {
                if (_setFlowrateTUCmd == null)
                {
                    _setFlowrateTUCmd = new RelayCommand(
                        param => SetFlowrateTU()
                            );
                }
                return _setFlowrateTUCmd;
            }
        }

        private void SetFlowrateTU()
        {
            Fields.FlowrateTechUnit.Value = (byte)(FlowrateTUbox.SelectedIndex + 1);
            UpdateParam(Fields.FlowrateTechUnit, Fields.FlowrateTechUnit.Value.ToString());
        }

        private ICommand _setFlowrateNdigitCmd;
        public ICommand SetFlowrateNdigitCmd
        {
            get
            {
                if (_setFlowrateNdigitCmd == null)
                {
                    _setFlowrateNdigitCmd = new RelayCommand(
                        param => SetFlowrateNdigit()
                            );
                }
                return _setFlowrateNdigitCmd;
            }
        }

        private void SetFlowrateNdigit()
        {
            Fields.FlowrateNdigit.Value = Convert.ToByte(FlowrateNdigitVal.Text);
            UpdateParam(Fields.FlowrateNdigit, Fields.FlowrateNdigit.Value.ToString());
        }

        private ICommand _setAccumulatorNdigitCmd;
        public ICommand SetAccumulatorNdigitCmd
        {
            get
            {
                if (_setAccumulatorNdigitCmd == null)
                {
                    _setAccumulatorNdigitCmd = new RelayCommand(
                        param => SetAccumulatorNdigit()
                            );
                }
                return _setAccumulatorNdigitCmd;
            }
        }

        private void SetAccumulatorNdigit()
        {
            Fields.AccumulatorNdigit.Value = Convert.ToByte(AccumulatorNdigitVal.Text);
            UpdateParam(Fields.AccumulatorNdigit, Fields.AccumulatorNdigit.Value.ToString());
        }

        private ICommand _setProcessLogPeriodCmd;
        public ICommand SetProcessLogPeriodCmd
        {
            get
            {
                if (_setProcessLogPeriodCmd == null)
                {
                    _setProcessLogPeriodCmd = new RelayCommand(
                        param => SetProcessLogPeriod()
                            );
                }
                return _setProcessLogPeriodCmd;
            }
        }

        private void SetProcessLogPeriod()
        {
            Fields.ProcessLogPeriod.Value = Convert.ToByte(ProcessLogPeriodVal.Text);
            UpdateParam(Fields.ProcessLogPeriod, Fields.ProcessLogPeriod.Value.ToString());
        }

        #endregion

        #region VisualizationsOptions

        private ICommand _setVisualizationOptCmd;
        public ICommand SetVisualizationOptCmd
        {
            get
            {
                if (_setVisualizationOptCmd == null)
                {
                    _setVisualizationOptCmd = new RelayCommand(
                        param => SetVisualizationOpt()
                            );
                }
                return _setVisualizationOptCmd;
            }
        }

        private void SetVisualizationOpt()
        {
            Fields.SpecialVisualization.Value = (byte)(VisualizationOptbox.SelectedIndex);
            UpdateParam(Fields.SpecialVisualization, Fields.SpecialVisualization.Value.ToString());
        }

        #endregion

        #region AcquisitionSetup

        private ICommand _setLowPowerMeasurePeriodCmd;
        public ICommand SetLowPowerMeasurePeriodCmd
        {
            get
            {
                if (_setLowPowerMeasurePeriodCmd == null)
                {
                    _setLowPowerMeasurePeriodCmd = new RelayCommand(
                        param => SetLowPowerMeasurePeriod()
                            );
                }
                return _setLowPowerMeasurePeriodCmd;
            }
        }

        private void SetLowPowerMeasurePeriod()
        {
            Fields.LowPowerMeasurePeriod.Value = Convert.ToByte(LowPowerMeasurePeriodVal.Text);
            UpdateParam(Fields.LowPowerMeasurePeriod, Fields.LowPowerMeasurePeriod.Value.ToString());
        }

        private ICommand _setExcitationPauseCmd;
        public ICommand SetExcitationPauseCmd
        {
            get
            {
                if (_setExcitationPauseCmd == null)
                {
                    _setExcitationPauseCmd = new RelayCommand(
                        param => SetExcitationPause()
                            );
                }
                return _setExcitationPauseCmd;
            }
        }

        private void SetExcitationPause()
        {
            Fields.ExcitationPause.Value = Convert.ToByte(ExcitationPauseVal.Text);
            UpdateParam(Fields.ExcitationPause, Fields.ExcitationPause.Value.ToString());
        }

        private ICommand _setOffsetRegCmd;
        public ICommand SetOffsetRegCmd
        {
            get
            {
                if (_setOffsetRegCmd == null)
                {
                    _setOffsetRegCmd = new RelayCommand(
                        param => SetOffsetReg()
                            );
                }
                return _setOffsetRegCmd;
            }
        }

        private void SetOffsetReg()
        {
            Fields.OffsetReg.Value = Convert.ToByte(OffsetRegVal.Text);
            UpdateParam(Fields.OffsetReg);
        }

        private ICommand _setAwakeMeasurePeriodCmd;
        public ICommand SetAwakeMeasurePeriodCmd
        {
            get
            {
                if (_setAwakeMeasurePeriodCmd == null)
                {
                    _setAwakeMeasurePeriodCmd = new RelayCommand(
                        param => SetAwakeMeasurePeriod()
                            );
                }
                return _setAwakeMeasurePeriodCmd;
            }
        }

        private void SetAwakeMeasurePeriod()
        {
            Fields.AwakeMeasurePeriod.Value = Convert.ToUInt16(AwakeMeasurePeriodVal.Text);
            UpdateParam(Fields.AwakeMeasurePeriod);
        }

        private ICommand _setMainsLineFrequencyCmd;
        public ICommand SetMainsLineFrequencyCmd
        {
            get
            {
                if (_setMainsLineFrequencyCmd == null)
                {
                    _setMainsLineFrequencyCmd = new RelayCommand(
                        param => SetMainsLineFrequency()
                            );
                }
                return _setMainsLineFrequencyCmd;
            }
        }

        private void SetMainsLineFrequency()
        {
            Fields.MainsLineFrequency.Value = (byte)(MainsLineFrequencybox.SelectedIndex);
            UpdateParam(Fields.MainsLineFrequency);
        }

        private ICommand _setAdcGainCmd;
        public ICommand SetAdcGainCmd
        {
            get
            {
                if (_setAdcGainCmd == null)
                {
                    _setAdcGainCmd = new RelayCommand(
                        param => SetAdcGain()
                            );
                }
                return _setAdcGainCmd;
            }
        }

        private void SetAdcGain()
        {
            Fields.AdcGain.Value = (byte)(AdcGainbox.SelectedIndex);
            UpdateParam(Fields.AdcGain);
        }

        private ICommand _setSD24SamplingIndexCmd;
        public ICommand SetSD24SamplingIndexCmd
        {
            get
            {
                if (_setSD24SamplingIndexCmd == null)
                {
                    _setSD24SamplingIndexCmd = new RelayCommand(
                        param => SetSD24SamplingIndex()
                            );
                }
                return _setSD24SamplingIndexCmd;
            }
        }

        private void SetSD24SamplingIndex()
        {
            Fields.SD24SamplingIndex.Value = (byte)(SD24SamplingIndexbox.SelectedIndex);
            UpdateParam(Fields.SD24SamplingIndex);
        }

        private ICommand _setInputStageStabTimeCmd;
        public ICommand SetInputStageStabTimeCmd
        {
            get
            {
                if (_setInputStageStabTimeCmd == null)
                {
                    _setInputStageStabTimeCmd = new RelayCommand(
                        param => SetInputStageStabTime()
                            );
                }
                return _setInputStageStabTimeCmd;
            }
        }

        private void SetInputStageStabTime()
        {
            Fields.InputStageStabTime.Value = Convert.ToUInt16(InputStageStabTimeVal.Text);
            UpdateParam(Fields.InputStageStabTime);
        }
        #endregion

        #region FiltersAndFullScale

        private ICommand _setCutoffCmd;
        public ICommand SetCutoffCmd
        {
            get
            {
                if (_setCutoffCmd == null)
                {
                    _setCutoffCmd = new RelayCommand(
                        param => SetCutoff()
                            );
                }
                return _setCutoffCmd;
            }
        }

        private void SetCutoff()
        {
            Fields.Cutoff.Value = Convert.ToUInt16(CutoffVal.Text);
            UpdateParam(Fields.Cutoff);
        }

        private ICommand _setDampingCmd;
        public ICommand SetDampingCmd
        {
            get
            {
                if (_setDampingCmd == null)
                {
                    _setDampingCmd = new RelayCommand(
                        param => SetDamping()
                            );
                }
                return _setDampingCmd;
            }
        }

        private void SetDamping()
        {
            Fields.Damping.Value = Convert.ToUInt16(DampingVal.Text);
            UpdateParam(Fields.Damping);
        }

        private ICommand _setDampingSlowCmd;
        public ICommand SetDampingSlowCmd
        {
            get
            {
                if (_setDampingSlowCmd == null)
                {
                    _setDampingSlowCmd = new RelayCommand(
                        param => SetDampingSlow()
                            );
                }
                return _setDampingSlowCmd;
            }
        }

        private void SetDampingSlow()
        {
            Fields.DampingSlow.Value = Convert.ToUInt16(DampingSlowVal.Text);
            UpdateParam(Fields.DampingSlow);
        }

        private ICommand _setBypassCmd;
        public ICommand SetBypassCmd
        {
            get
            {
                if (_setBypassCmd == null)
                {
                    _setBypassCmd = new RelayCommand(
                        param => SetBypass()
                            );
                }
                return _setBypassCmd;
            }
        }

        private void SetBypass()
        {
            Fields.Bypass.Value = Convert.ToByte(BypassVal.Text);
            UpdateParam(Fields.Bypass);
        }

        private ICommand _setBypassCountCmd;
        public ICommand SetBypassCountCmd
        {
            get
            {
                if (_setBypassCountCmd == null)
                {
                    _setBypassCountCmd = new RelayCommand(
                        param => SetBypassCount()
                            );
                }
                return _setBypassCountCmd;
            }
        }

        private void SetBypassCount()
        {
            Fields.BypassCount.Value = Convert.ToByte(BypassCountVal.Text);
            UpdateParam(Fields.BypassCount);
        }

        private ICommand _setPeakcutCmd;
        public ICommand SetPeakcutCmd
        {
            get
            {
                if (_setPeakcutCmd == null)
                {
                    _setPeakcutCmd = new RelayCommand(
                        param => SetPeakcut()
                            );
                }
                return _setPeakcutCmd;
            }
        }

        private void SetPeakcut()
        {
            Fields.Peakcut.Value = Convert.ToByte(PeakcutVal.Text);
            UpdateParam(Fields.Peakcut);
        }

        private ICommand _setPeakcutCountCmd;
        public ICommand SetPeakcutCountCmd
        {
            get
            {
                if (_setPeakcutCountCmd == null)
                {
                    _setPeakcutCountCmd = new RelayCommand(
                        param => SetPeakcutCount()
                            );
                }
                return _setPeakcutCountCmd;
            }
        }

        private void SetPeakcutCount()
        {
            Fields.PeakcutCount.Value = Convert.ToByte(PeakcutCountVal.Text);
            UpdateParam(Fields.PeakcutCount);
        }

        private ICommand _setFlowrateFullscaleCmd;
        public ICommand SetFlowrateFullscaleCmd
        {
            get
            {
                if (_setFlowrateFullscaleCmd == null)
                {
                    _setFlowrateFullscaleCmd = new RelayCommand(
                        param => SetFlowrateFullscale()
                            );
                }
                return _setFlowrateFullscaleCmd;
            }
        }

        private void SetFlowrateFullscale()
        {
            Fields.FlowrateFullscale.Value = Convert.ToByte(FlowrateFullscaleVal.Text);
            UpdateParam(Fields.FlowrateFullscale);
        }

        #endregion

        #region Sensor

        private ICommand _setSensorOffsetCmd;
        public ICommand SetSensorOffsetCmd
        {
            get
            {
                if (_setSensorOffsetCmd == null)
                {
                    _setSensorOffsetCmd = new RelayCommand(
                        param => SetSensorOffset()
                            );
                }
                return _setSensorOffsetCmd;
            }
        }

        private void SetSensorOffset()
        {
            Fields.SensorOffset.Value = (float)Convert.ToDouble(SensorOffsetVal.Text, System.Globalization.CultureInfo.InvariantCulture);
            UpdateParam(Fields.SensorOffset);
        }

        private ICommand _setKaRatioCmd;
        public ICommand SetKaRatioCmd
        {
            get
            {
                if (_setKaRatioCmd == null)
                {
                    _setKaRatioCmd = new RelayCommand(
                        param => SetKaRatio()
                            );
                }
                return _setKaRatioCmd;
            }
        }

        private void SetKaRatio()
        {
            Fields.KaRatio.Value = (float)Convert.ToDouble(KaRatioVal.Text, System.Globalization.CultureInfo.InvariantCulture);
            UpdateParam(Fields.KaRatio);
        }

        private ICommand _setSensorDiameterCmd;
        public ICommand SetSensorDiameterCmd
        {
            get
            {
                if (_setSensorDiameterCmd == null)
                {
                    _setSensorDiameterCmd = new RelayCommand(
                        param => SetSensorDiameter()
                            );
                }
                return _setSensorDiameterCmd;
            }
        }

        private void SetSensorDiameter()
        {
            Fields.SensorDiameter.Value = Convert.ToUInt16(SensorDiameterVal.Text);
            UpdateParam(Fields.SensorDiameter);
        }

        private ICommand _setSensorModelCmd;
        public ICommand SetSensorModelCmd
        {
            get
            {
                if (_setSensorModelCmd == null)
                {
                    _setSensorModelCmd = new RelayCommand(
                        param => SetSensorModel()
                            );
                }
                return _setSensorModelCmd;
            }
        }

        private void SetSensorModel()
        {
            Fields.SensorModel.Value = SensorModelVal.Text;
            UpdateParam(Fields.SensorDiameter);
        }

        private ICommand _setSensorIdCmd;
        public ICommand SetSensorIdCmd
        {
            get
            {
                if (_setSensorIdCmd == null)
                {
                    _setSensorIdCmd = new RelayCommand(
                        param => SetSensorId()
                            );
                }
                return _setSensorIdCmd;
            }
        }

        private void SetSensorId()
        {
            Fields.SensorId.Value = SensorIdVal.Text;
            UpdateParam(Fields.SensorId);
        }

        private ICommand _setSensorIsInsertionCmd;
        public ICommand SetSensorIsInsertionCmd
        {
            get
            {
                if (_setSensorIsInsertionCmd == null)
                {
                    _setSensorIsInsertionCmd = new RelayCommand(
                        param => SetSensorIsInsertion()
                            );
                }
                return _setSensorIsInsertionCmd;
            }
        }

        private void SetSensorIsInsertion()
        {
            Fields.SensorIsInsertion.Value = (byte)(SensorIsInsertionbox.SelectedIndex);
            UpdateParam(Fields.SensorIsInsertion);
        }

        private ICommand _setInsertion_sA_LOCmd;
        public ICommand SetInsertion_sA_LOCmd
        {
            get
            {
                if (_setInsertion_sA_LOCmd == null)
                {
                    _setInsertion_sA_LOCmd = new RelayCommand(
                        param => SetInsertion_sA_LO()
                            );
                }
                return _setInsertion_sA_LOCmd;
            }
        }

        private void SetInsertion_sA_LO()
        {
            Fields.Insertion_sA_LO.Value = (float)Convert.ToDouble(Insertion_sA_LOVal.Text);
            UpdateParam(Fields.Insertion_sA_LO);
        }

        private ICommand _setInsertion_sB_LOCmd;
        public ICommand SetInsertion_sB_LOCmd
        {
            get
            {
                if (_setInsertion_sB_LOCmd == null)
                {
                    _setInsertion_sB_LOCmd = new RelayCommand(
                        param => SetInsertion_sB_LO()
                            );
                }
                return _setInsertion_sB_LOCmd;
            }
        }

        private void SetInsertion_sB_LO()
        {
            Fields.Insertion_sB_LO.Value = (float)Convert.ToDouble(Insertion_sB_LOVal.Text);
            UpdateParam(Fields.Insertion_sB_LO);
        }

        private ICommand _setInsertion_sC_LOCmd;
        public ICommand SetInsertion_sC_LOCmd
        {
            get
            {
                if (_setInsertion_sC_LOCmd == null)
                {
                    _setInsertion_sC_LOCmd = new RelayCommand(
                        param => SetInsertion_sC_LO()
                            );
                }
                return _setInsertion_sC_LOCmd;
            }
        }

        private void SetInsertion_sC_LO()
        {
            Fields.Insertion_sC_LO.Value = (float)Convert.ToDouble(Insertion_sC_LOVal.Text);
            UpdateParam(Fields.Insertion_sC_LO);
        }

        private ICommand _setInsertion_sD_LOCmd;
        public ICommand SetInsertion_sD_LOCmd
        {
            get
            {
                if (_setInsertion_sD_LOCmd == null)
                {
                    _setInsertion_sD_LOCmd = new RelayCommand(
                        param => SetInsertion_sD_LO()
                            );
                }
                return _setInsertion_sD_LOCmd;
            }
        }

        private void SetInsertion_sD_LO()
        {
            Fields.Insertion_sD_LO.Value = (float)Convert.ToDouble(Insertion_sD_LOVal.Text);
            UpdateParam(Fields.Insertion_sD_LO);
        }

        private ICommand _setInsertion_sA_HICmd;
        public ICommand SetInsertion_sA_HICmd
        {
            get
            {
                if (_setInsertion_sA_HICmd == null)
                {
                    _setInsertion_sA_HICmd = new RelayCommand(
                        param => SetInsertion_sA_HI()
                            );
                }
                return _setInsertion_sA_HICmd;
            }
        }

        private void SetInsertion_sA_HI()
        {
            Fields.Insertion_sA_HI.Value = (float)Convert.ToDouble(Insertion_sA_HIVal.Text);
            UpdateParam(Fields.Insertion_sA_HI);
        }

        private ICommand _setInsertion_sB_HICmd;
        public ICommand SetInsertion_sB_HICmd
        {
            get
            {
                if (_setInsertion_sB_HICmd == null)
                {
                    _setInsertion_sB_HICmd = new RelayCommand(
                        param => SetInsertion_sB_HI()
                            );
                }
                return _setInsertion_sB_HICmd;
            }
        }

        private void SetInsertion_sB_HI()
        {
            Fields.Insertion_sB_HI.Value = (float)Convert.ToDouble(Insertion_sB_HIVal.Text);
            UpdateParam(Fields.Insertion_sB_HI);
        }

        private ICommand _setInsertion_sC_HICmd;
        public ICommand SetInsertion_sC_HICmd
        {
            get
            {
                if (_setInsertion_sC_HICmd == null)
                {
                    _setInsertion_sC_HICmd = new RelayCommand(
                        param => SetInsertion_sC_HI()
                            );
                }
                return _setInsertion_sC_HICmd;
            }
        }

        private void SetInsertion_sC_HI()
        {
            Fields.Insertion_sC_HI.Value = (float)Convert.ToDouble(Insertion_sC_HIVal.Text);
            UpdateParam(Fields.Insertion_sC_HI);
        }

        private ICommand _setInsertion_sD_HICmd;
        public ICommand SetInsertion_sD_HICmd
        {
            get
            {
                if (_setInsertion_sD_HICmd == null)
                {
                    _setInsertion_sD_HICmd = new RelayCommand(
                        param => SetInsertion_sD_HI()
                            );
                }
                return _setInsertion_sD_HICmd;
            }
        }

        private void SetInsertion_sD_HI()
        {
            Fields.Insertion_sD_HI.Value = (float)Convert.ToDouble(Insertion_sD_HIVal.Text);
            UpdateParam(Fields.Insertion_sD_HI);
        }

        private ICommand _setInsertion_Interp_ThCmd;
        public ICommand SetInsertion_Interp_ThCmd
        {
            get
            {
                if (_setInsertion_Interp_ThCmd == null)
                {
                    _setInsertion_Interp_ThCmd = new RelayCommand(
                        param => SetInsertion_Interp_Th()
                            );
                }
                return _setInsertion_Interp_ThCmd;
            }
        }

        private void SetInsertion_Interp_Th()
        {
            Fields.Insertion_Interp_Th.Value = (float)Convert.ToDouble(Insertion_Interp_ThVal.Text);
            UpdateParam(Fields.Insertion_Interp_Th);
        }

        #endregion

        #region Converter

        private ICommand _setAlignmentCalibrationFactorCmd;
        public ICommand SetAlignmentCalibrationFactorCmd
        {
            get
            {
                if (_setAlignmentCalibrationFactorCmd == null)
                {
                    _setAlignmentCalibrationFactorCmd = new RelayCommand(
                        param => SetAlignmentCalibrationFactor()
                            );
                }
                return _setAlignmentCalibrationFactorCmd;
            }
        }

        private void SetAlignmentCalibrationFactor()
        {
            Fields.AlignmentCalibrationFactor.Value = (float)Convert.ToDouble(AlignmentCalibrationFactorVal.Text, System.Globalization.CultureInfo.InvariantCulture);
            UpdateParam(Fields.AlignmentCalibrationFactor);
        }

        private ICommand _setAlignmentOffsetCmd;
        public ICommand SetAlignmentOffsetCmd
        {
            get
            {
                if (_setAlignmentOffsetCmd == null)
                {
                    _setAlignmentOffsetCmd = new RelayCommand(
                        param => SetAlignmentOffset()
                            );
                }
                return _setAlignmentOffsetCmd;
            }
        }

        private void SetAlignmentOffset()
        {
            Fields.AlignmentOffset.Value = (float)Convert.ToDouble(AlignmentOffsetVal.Text, System.Globalization.CultureInfo.InvariantCulture);
            UpdateParam(Fields.AlignmentOffset);
        }

        private ICommand _setTemperatureOffsetCmd;
        public ICommand SetTemperatureOffsetCmd
        {
            get
            {
                if (_setTemperatureOffsetCmd == null)
                {
                    _setTemperatureOffsetCmd = new RelayCommand(
                        param => SetTemperatureOffset()
                            );
                }
                return _setTemperatureOffsetCmd;
            }
        }

        private void SetTemperatureOffset()
        {
            Fields.TemperatureOffset.Value = Convert.ToInt16(TemperatureOffsetVal.Text);
            UpdateParam(Fields.TemperatureOffset);
        }

        private ICommand _setConverterIdCmd;
        public ICommand SetConverterIdCmd
        {
            get
            {
                if (_setConverterIdCmd == null)
                {
                    _setConverterIdCmd = new RelayCommand(
                        param => SetConverterId()
                            );
                }
                return _setConverterIdCmd;
            }
        }

        private void SetConverterId()
        {
            Fields.ConverterId.Value = ConverterIdVal.Text;
            UpdateParam(Fields.ConverterId);
        }

        private ICommand _setConverterSerialNumberCmd;
        public ICommand SetConverterSerialNumberCmd
        {
            get
            {
                if (_setConverterSerialNumberCmd == null)
                {
                    _setConverterSerialNumberCmd = new RelayCommand(
                        param => SetConverterSerialNumber()
                            );
                }
                return _setConverterSerialNumberCmd;
            }
        }

        private void SetConverterSerialNumber()
        {
            Fields.ConverterSerialNumber.Value = Convert.ToUInt32(ConverterSerialNumberVal.Text);
            UpdateParam(Fields.ConverterSerialNumber);
        }

        private ICommand _setDeviceNameCmd;
        public ICommand SetDeviceNameCmd
        {
            get
            {
                if (_setDeviceNameCmd == null)
                {
                    _setDeviceNameCmd = new RelayCommand(
                        param => SetDeviceName()
                            );
                }
                return _setDeviceNameCmd;
            }
        }

        private void SetDeviceName()
        {
            Fields.DeviceName.Value = DeviceNameVal.Text;
            UpdateParam(Fields.DeviceName);
        }

        #endregion

        #region PulseOutput

        private ICommand _setSetPulseLengthCmd;
        public ICommand SetPulseLengthCmd
        {
            get
            {
                if (_setSetPulseLengthCmd == null)
                {
                    _setSetPulseLengthCmd = new RelayCommand(
                        param => SetPulseLength()
                            );
                }
                return _setSetPulseLengthCmd;
            }
        }

        private void SetPulseLength()
        {
            Fields.PulseLength.Value = Convert.ToUInt16(PulseLengthVal.Text);
            UpdateParam(Fields.PulseLength);
        }

        private ICommand _setPulseOutputTechUnitCmd;
        public ICommand SetPulseOutputTechUnitCmd
        {
            get
            {
                if (_setPulseOutputTechUnitCmd == null)
                {
                    _setPulseOutputTechUnitCmd = new RelayCommand(
                        param => SetPulseOutputTechUnit()
                            );
                }
                return _setPulseOutputTechUnitCmd;
            }
        }

        private void SetPulseOutputTechUnit()
        {
            Fields.PulseOutputTechUnit.Value = (byte)(PulseOutputTechUnitbox.SelectedIndex + 1);
            UpdateParam(Fields.PulseOutputTechUnit);
        }

        private ICommand _setPulseOutputVolumeCmd;
        public ICommand SetPulseOutputVolumeCmd
        {
            get
            {
                if (_setPulseOutputVolumeCmd == null)
                {
                    _setPulseOutputVolumeCmd = new RelayCommand(
                        param => SetPulseOutputVolume()
                            );
                }
                return _setPulseOutputVolumeCmd;
            }
        }

        private void SetPulseOutputVolume()
        {
            Fields.PulseOutputVolume.Value = Convert.ToUInt16(PulseOutputVolumeVal.Text);
            UpdateParam(Fields.PulseOutputVolume);
        }

        #endregion

        #region Calibration

        private ICommand _setCalibrationTemperatureCmd;
        public ICommand SetCalibrationTemperatureCmd
        {
            get
            {
                if (_setCalibrationTemperatureCmd == null)
                {
                    _setCalibrationTemperatureCmd = new RelayCommand(
                        param => SetCalibrationTemperature()
                            );
                }
                return _setCalibrationTemperatureCmd;
            }
        }

        private void SetCalibrationTemperature()
        {
            Fields.CalibrationTemperature.Value = Convert.ToByte(CalibrationTemperatureVal.Text);
            UpdateParam(Fields.CalibrationTemperature);
        }

        private ICommand _setCalibrationVoltageCmd;
        public ICommand SetCalibrationVoltageCmd
        {
            get
            {
                if (_setCalibrationVoltageCmd == null)
                {
                    _setCalibrationVoltageCmd = new RelayCommand(
                        param => SetCalibrationVoltage()
                            );
                }
                return _setCalibrationVoltageCmd;
            }
        }

        private void SetCalibrationVoltage()
        {
            Fields.CalibrationVoltage.Value = Convert.ToByte(CalibrationVoltageVal.Text);
            UpdateParam(Fields.CalibrationVoltage);
        }

        private ICommand _setCalibrationDateCmd;
        public ICommand SetCalibrationDateCmd
        {
            get
            {
                if (_setCalibrationDateCmd == null)
                {
                    _setCalibrationDateCmd = new RelayCommand(
                        param => SetCalibrationDate()
                            );
                }
                return _setCalibrationDateCmd;
            }
        }

        private void SetCalibrationDate()
        {
            Fields.CalibrationDate.Value = CalibrationDateVal.Text;
            UpdateParam(Fields.CalibrationDate);
        }

        private ICommand _setManufacturerCmd;
        public ICommand SetManufacturerCmd
        {
            get
            {
                if (_setManufacturerCmd == null)
                {
                    _setManufacturerCmd = new RelayCommand(
                        param => SetManufacturer()
                            );
                }
                return _setManufacturerCmd;
            }
        }

        private void SetManufacturer()
        {
            Fields.Manufacturer.Value = ManufacturerVal.Text;
            UpdateParam(Fields.Manufacturer);
        }

        private ICommand _setOtherFeaturesCmd;
        public ICommand SetOtherFeaturesCmd
        {
            get
            {
                if (_setOtherFeaturesCmd == null)
                {
                    _setOtherFeaturesCmd = new RelayCommand(
                        param => SetOtherFeatures()
                            );
                }
                return _setOtherFeaturesCmd;
            }
        }

        private void SetOtherFeatures()
        {
            Fields.OtherFeatures.Value = OtherFeaturesVal.Text;
            UpdateParam(Fields.OtherFeatures);
        }

        #endregion

        #region EmptyPipeDetection

        private ICommand _setEmptyPipeCfgCmd;
        public ICommand SetEmptyPipeCfgCmd
        {
            get
            {
                if (_setEmptyPipeCfgCmd == null)
                {
                    _setEmptyPipeCfgCmd = new RelayCommand(
                        param => SetEmptyPipeCfg()
                            );
                }
                return _setEmptyPipeCfgCmd;
            }
        }

        private void SetEmptyPipeCfg()
        {
            Fields.EmptyPipeCfg.Value = (byte)(EmptyPipeCfgbox.SelectedIndex);
            UpdateParam(Fields.EmptyPipeCfg);
        }

        private ICommand _setEmptyPipeThresholdCmd;
        public ICommand SetEmptyPipeThresholdCmd
        {
            get
            {
                if (_setEmptyPipeThresholdCmd == null)
                {
                    _setEmptyPipeThresholdCmd = new RelayCommand(
                        param => SetEmptyPipeThreshold()
                            );
                }
                return _setEmptyPipeThresholdCmd;
            }
        }

        private void SetEmptyPipeThreshold()
        {
            Fields.EmptyPipeThreshold.Value = Convert.ToUInt16(EmptyPipeThresholdVal.Text);
            UpdateParam(Fields.EmptyPipeThreshold);
        }

        private ICommand _setEmptyPipeReleaseCmd;
        public ICommand SetEmptyPipeReleaseCmd
        {
            get
            {
                if (_setEmptyPipeReleaseCmd == null)
                {
                    _setEmptyPipeReleaseCmd = new RelayCommand(
                        param => SetEmptyPipeRelease()
                            );
                }
                return _setEmptyPipeReleaseCmd;
            }
        }

        private void SetEmptyPipeRelease()
        {
            Fields.EmptyPipeRelease.Value = Convert.ToByte(EmptyPipeReleaseVal.Text);
            UpdateParam(Fields.EmptyPipeRelease);
        }

        private ICommand _setEmptyPipeFreqCmd;
        public ICommand SetEmptyPipeFreqCmd
        {
            get
            {
                if (_setEmptyPipeFreqCmd == null)
                {
                    _setEmptyPipeFreqCmd = new RelayCommand(
                        param => SetEmptyPipeFreq()
                            );
                }
                return _setEmptyPipeReleaseCmd;
            }
        }

        private void SetEmptyPipeFreq()
        {
            Fields.EmptyPipeFreq.Value = Convert.ToUInt16(EmptyPipeReleaseVal.Text);
            UpdateParam(Fields.EmptyPipeFreq);
        }

        private ICommand _setEmptyPipeFreqFastCmd;
        public ICommand SetEmptyPipeFreqFastCmd
        {
            get
            {
                if (_setEmptyPipeFreqFastCmd == null)
                {
                    _setEmptyPipeFreqFastCmd = new RelayCommand(
                        param => SetEmptyPipeFreqFast()
                            );
                }
                return _setEmptyPipeFreqFastCmd;
            }
        }

        private void SetEmptyPipeFreqFast()
        {
            Fields.EmptyPipeFreqFast.Value = Convert.ToByte(EmptyPipeFreqFastVal.Text);
            UpdateParam(Fields.EmptyPipeFreqFast);
        }

        private ICommand _setWaterDetectMeasEnableCmd;
        public ICommand SetWaterDetectMeasEnableCmd
        {
            get
            {
                if (_setWaterDetectMeasEnableCmd == null)
                {
                    _setWaterDetectMeasEnableCmd = new RelayCommand(
                        param => SetWaterDetectMeasEnable()
                            );
                }
                return _setWaterDetectMeasEnableCmd;
            }
        }

        private void SetWaterDetectMeasEnable()
        {
            Fields.WaterDetectMeasEnable.Value = (byte)(WaterDetectMeasEnablebox.SelectedIndex);
            UpdateParam(Fields.WaterDetectMeasEnable);
        }

        private ICommand _setWaterDetectMeasThresholdCmd;
        public ICommand SetWaterDetectMeasThresholdCmd
        {
            get
            {
                if (_setWaterDetectMeasThresholdCmd == null)
                {
                    _setWaterDetectMeasThresholdCmd = new RelayCommand(
                        param => SetWaterDetectMeasThreshold()
                            );
                }
                return _setWaterDetectMeasThresholdCmd;
            }
        }

        private void SetWaterDetectMeasThreshold()
        {
            Fields.WaterDetectMeasThreshold.Value = Convert.ToByte(WaterDetectMeasThresholdVal.Text);
            UpdateParam(Fields.WaterDetectMeasThreshold);
        }

        #endregion

        #region BatteryEnergy

        private ICommand _setTotalTimeInLowPowerCmd;
        public ICommand SetTotalTimeInLowPowerCmd
        {
            get
            {
                if (_setTotalTimeInLowPowerCmd == null)
                {
                    _setTotalTimeInLowPowerCmd = new RelayCommand(
                        param => SetTotalTimeInLowPower()
                            );
                }
                return _setTotalTimeInLowPowerCmd;
            }
        }

        private void SetTotalTimeInLowPower()
        {
            Fields.TotalTimeInLowPower.Value = Convert.ToUInt32(TotalTimeInLowPowerVal.Text);
            UpdateParam(Fields.TotalTimeInLowPower);
        }

        private ICommand _setTotalMeasuresCountCmd;
        public ICommand SetTotalMeasuresCountCmd
        {
            get
            {
                if (_setTotalMeasuresCountCmd == null)
                {
                    _setTotalMeasuresCountCmd = new RelayCommand(
                        param => SetTotalMeasuresCount()
                            );
                }
                return _setTotalMeasuresCountCmd;
            }
        }

        private void SetTotalMeasuresCount()
        {
            Fields.TotalMeasuresCount.Value = Convert.ToUInt32(TotalMeasuresCountVal.Text);
            UpdateParam(Fields.TotalMeasuresCount);
        }

        private ICommand _setTotalTimeAwakeCmd;
        public ICommand SetTotalTimeAwakeCmd
        {
            get
            {
                if (_setTotalTimeAwakeCmd == null)
                {
                    _setTotalTimeAwakeCmd = new RelayCommand(
                        param => SetTotalTimeAwake()
                            );
                }
                return _setTotalTimeAwakeCmd;
            }
        }

        private void SetTotalTimeAwake()
        {
            Fields.TotalTimeAwake.Value = Convert.ToUInt32(TotalTimeAwakeVal.Text);
            UpdateParam(Fields.TotalTimeAwake);
        }

        private ICommand _setTotaluAhCmd;
        public ICommand SetTotaluAhCmd
        {
            get
            {
                if (_setTotaluAhCmd == null)
                {
                    _setTotaluAhCmd = new RelayCommand(
                        param => SetTotaluAh()
                            );
                }
                return _setTotaluAhCmd;
            }
        }

        private void SetTotaluAh()
        {
            Fields.TotaluAh.Value = Convert.ToUInt32(TotaluAhVal.Text);
            UpdateParam(Fields.TotaluAh);
        }

        private ICommand _setLeftuAhCmd;
        public ICommand SetLeftuAhCmd
        {
            get
            {
                if (_setLeftuAhCmd == null)
                {
                    _setLeftuAhCmd = new RelayCommand(
                        param => SetLeftuAh()
                            );
                }
                return _setLeftuAhCmd;
            }
        }

        private void SetLeftuAh()
        {
            Fields.LeftuAh.Value = Convert.ToUInt32(LeftuAhVal.Text);
            UpdateParam(Fields.LeftuAh);
        }

        private ICommand _setBatteryAutosaveCmd;
        public ICommand SetBatteryAutosaveCmd
        {
            get
            {
                if (_setBatteryAutosaveCmd == null)
                {
                    _setBatteryAutosaveCmd = new RelayCommand(
                        param => SetBatteryAutosave()
                            );
                }
                return _setBatteryAutosaveCmd;
            }
        }

        private void SetBatteryAutosave()
        {
            Fields.BatteryAutosave.Value = Convert.ToUInt16(BatteryAutosaveVal.Text);
            UpdateParam(Fields.BatteryAutosave);
        }

        #endregion

        #region PressureMeasure

        private ICommand _setPressureOptionCmd;
        public ICommand SetPressureOptionCmd
        {
            get
            {
                if (_setPressureOptionCmd == null)
                {
                    _setPressureOptionCmd = new RelayCommand(
                        param => SetPressureOption()
                            );
                }
                return _setPressureOptionCmd;
            }
        }

        private void SetPressureOption()
        {
            Fields.PressureOption.Value = (byte)(PressureOptionbox.SelectedIndex);
            UpdateParam(Fields.PressureOption);
        }

        private ICommand _setPressureMeasurePeriodCmd;
        public ICommand SetPressureMeasurePeriodCmd
        {
            get
            {
                if (_setPressureMeasurePeriodCmd == null)
                {
                    _setPressureMeasurePeriodCmd = new RelayCommand(
                        param => SetPressureMeasurePeriod()
                            );
                }
                return _setPressureMeasurePeriodCmd;
            }
        }

        private void SetPressureMeasurePeriod()
        {
            Fields.PressureMeasurePeriod.Value = Convert.ToByte(PressureMeasurePeriodVal.Text);
            UpdateParam(Fields.PressureMeasurePeriod);
        }

        #endregion

        #region TemperatureAndEnergyMeter

        private ICommand _setEnergyOptionCmd;
        public ICommand SetEnergyOptionCmd
        {
            get
            {
                if (_setEnergyOptionCmd == null)
                {
                    _setEnergyOptionCmd = new RelayCommand(
                        param => SetEnergyOption()
                            );
                }
                return _setEnergyOptionCmd;
            }
        }

        private void SetEnergyOption()
        {
            Fields.EnergyOption.Value = (byte)(EnergyOptionbox.SelectedIndex);
            UpdateParam(Fields.EnergyOption);
        }

        private ICommand _setTemperatureMeasurePeriodCmd;
        public ICommand SetTemperatureMeasurePeriodCmd
        {
            get
            {
                if (_setTemperatureMeasurePeriodCmd == null)
                {
                    _setTemperatureMeasurePeriodCmd = new RelayCommand(
                        param => SetTemperatureMeasurePeriod()
                            );
                }
                return _setTemperatureMeasurePeriodCmd;
            }
        }

        private void SetTemperatureMeasurePeriod()
        {
            Fields.TemperatureMeasurePeriod.Value = Convert.ToByte(TemperatureMeasurePeriodVal.Text);
            UpdateParam(Fields.TemperatureMeasurePeriod);
        }

        private ICommand _setDeltaTempMinCmd;
        public ICommand SetDeltaTempMinCmd
        {
            get
            {
                if (_setDeltaTempMinCmd == null)
                {
                    _setDeltaTempMinCmd = new RelayCommand(
                        param => SetDeltaTempMin()
                            );
                }
                return _setDeltaTempMinCmd;
            }
        }

        private void SetDeltaTempMin()
        {
            Fields.DeltaTempMin.Value = Convert.ToByte(DeltaTempMinVal.Text);
            UpdateParam(Fields.DeltaTempMin);
        }

        private ICommand _setSpecificHeatCoeffACmd;
        public ICommand SetSpecificHeatCoeffACmd
        {
            get
            {
                if (_setSpecificHeatCoeffACmd == null)
                {
                    _setSpecificHeatCoeffACmd = new RelayCommand(
                        param => SetSpecificHeatCoeffA()
                            );
                }
                return _setSpecificHeatCoeffACmd;
            }
        }

        private void SetSpecificHeatCoeffA()
        {
            Fields.SpecificHeatCoeffA.Value = (float)Convert.ToDouble(SpecificHeatCoeffAVal.Text);
            UpdateParam(Fields.SpecificHeatCoeffA);
        }

        private ICommand _setSpecificHeatCoeffBCmd;
        public ICommand SetSpecificHeatCoeffBCmd
        {
            get
            {
                if (_setSpecificHeatCoeffBCmd == null)
                {
                    _setSpecificHeatCoeffBCmd = new RelayCommand(
                        param => SetSpecificHeatCoeffB()
                            );
                }
                return _setSpecificHeatCoeffBCmd;
            }
        }

        private void SetSpecificHeatCoeffB()
        {
            Fields.SpecificHeatCoeffB.Value = (float)Convert.ToDouble(SpecificHeatCoeffBVal.Text);
            UpdateParam(Fields.SpecificHeatCoeffB);
        }

        private ICommand _setSpecificHeatCoeffCCmd;
        public ICommand SetSpecificHeatCoeffCCmd
        {
            get
            {
                if (_setSpecificHeatCoeffCCmd == null)
                {
                    _setSpecificHeatCoeffCCmd = new RelayCommand(
                        param => SetSpecificHeatCoeffC()
                            );
                }
                return _setSpecificHeatCoeffCCmd;
            }
        }

        private void SetSpecificHeatCoeffC()
        {
            Fields.SpecificHeatCoeffC.Value = (float)Convert.ToDouble(SpecificHeatCoeffCVal.Text);
            UpdateParam(Fields.SpecificHeatCoeffC);
        }

        private ICommand _setSpecificHeatCoeffDCmd;
        public ICommand SetSpecificHeatCoeffDCmd
        {
            get
            {
                if (_setSpecificHeatCoeffDCmd == null)
                {
                    _setSpecificHeatCoeffDCmd = new RelayCommand(
                        param => SetSpecificHeatCoeffD()
                            );
                }
                return _setSpecificHeatCoeffDCmd;
            }
        }

        private void SetSpecificHeatCoeffD()
        {
            Fields.SpecificHeatCoeffD.Value = (float)Convert.ToDouble(SpecificHeatCoeffDVal.Text);
            UpdateParam(Fields.SpecificHeatCoeffD);
        }

        private ICommand _setDensityCoeffACmd;
        public ICommand SetDensityCoeffACmd
        {
            get
            {
                if (_setDensityCoeffACmd == null)
                {
                    _setDensityCoeffACmd = new RelayCommand(
                        param => SetDensityCoeffA()
                            );
                }
                return _setDensityCoeffACmd;
            }
        }

        private void SetDensityCoeffA()
        {
            Fields.DensityCoeffA.Value = (float)Convert.ToDouble(DensityCoeffAVal.Text);
            UpdateParam(Fields.DensityCoeffA);
        }

        private ICommand _setDensityCoeffBCmd;
        public ICommand SetDensityCoeffBCmd
        {
            get
            {
                if (_setDensityCoeffBCmd == null)
                {
                    _setDensityCoeffBCmd = new RelayCommand(
                        param => SetDensityCoeffB()
                            );
                }
                return _setDensityCoeffBCmd;
            }
        }

        private void SetDensityCoeffB()
        {
            Fields.DensityCoeffA.Value = (float)Convert.ToDouble(DensityCoeffBVal.Text);
            UpdateParam(Fields.DensityCoeffB);
        }

        private ICommand _setDensityCoeffCCmd;
        public ICommand SetDensityCoeffCCmd
        {
            get
            {
                if (_setDensityCoeffCCmd == null)
                {
                    _setDensityCoeffCCmd = new RelayCommand(
                        param => SetDensityCoeffC()
                            );
                }
                return _setDensityCoeffCCmd;
            }
        }

        private void SetDensityCoeffC()
        {
            Fields.DensityCoeffC.Value = (float)Convert.ToDouble(DensityCoeffCVal.Text);
            UpdateParam(Fields.DensityCoeffC);
        }

        private ICommand _setDensityCoeffDCmd;
        public ICommand SetDensityCoeffDCmd
        {
            get
            {
                if (_setDensityCoeffDCmd == null)
                {
                    _setDensityCoeffDCmd = new RelayCommand(
                        param => SetDensityCoeffD()
                            );
                }
                return _setDensityCoeffDCmd;
            }
        }

        private void SetDensityCoeffD()
        {
            Fields.DensityCoeffD.Value = (float)Convert.ToDouble(DensityCoeffDVal.Text);
            UpdateParam(Fields.DensityCoeffD);
        }

        #endregion

        #region TimeoutAndPassword

        private ICommand _setTimeoutToMainCmd;
        public ICommand SetTimeoutToMainCmd
        {
            get
            {
                if (_setTimeoutToMainCmd == null)
                {
                    _setTimeoutToMainCmd = new RelayCommand(
                        param => SetTimeoutToMain()
                            );
                }
                return _setTimeoutToMainCmd;
            }
        }

        private void SetTimeoutToMain()
        {
            Fields.TimeoutToMain.Value = Convert.ToUInt16(TimeoutToMainVal.Text);
            UpdateParam(Fields.TimeoutToMain);
        }

        private ICommand _setWakeupPoweroffCmd;
        public ICommand SetWakeupPoweroffCmd
        {
            get
            {
                if (_setWakeupPoweroffCmd == null)
                {
                    _setWakeupPoweroffCmd = new RelayCommand(
                        param => SetWakeupPoweroff()
                            );
                }
                return _setWakeupPoweroffCmd;
            }
        }

        private void SetWakeupPoweroff()
        {
            Fields.WakeupPoweroff.Value = (byte)(WakeupPoweroffbox.SelectedIndex);
            UpdateParam(Fields.WakeupPoweroff);
        }

        private ICommand _setPasswordTimeoutCmd;
        public ICommand SetPasswordTimeoutCmd
        {
            get
            {
                if (_setPasswordTimeoutCmd == null)
                {
                    _setPasswordTimeoutCmd = new RelayCommand(
                        param => SetPasswordTimeout()
                            );
                }
                return _setPasswordTimeoutCmd;
            }
        }

        private void SetPasswordTimeout()
        {
            Fields.PasswordTimeout.Value = Convert.ToUInt16(PasswordTimeoutVal.Text);
            UpdateParam(Fields.PasswordTimeout);
        }

        #endregion

        #region GsmParams

        private ICommand _setGSM_InstalledCmd;
        public ICommand SetGSM_InstalledCmd
        {
            get
            {
                if (_setGSM_InstalledCmd == null)
                {
                    _setGSM_InstalledCmd = new RelayCommand(
                        param => SetGSM_Installed()
                            );
                }
                return _setGSM_InstalledCmd;
            }
        }

        private void SetGSM_Installed()
        {
            Fields.GSM_Installed.Value = (byte)(GSM_Installedbox.SelectedIndex);
            UpdateParam(Fields.GSM_Installed);
        }

        #endregion

        #region ChangePassword

        private ICommand _checkOldPasswordCmd;
        public ICommand CheckOldPasswordCmd
        {
            get
            {
                if (_checkOldPasswordCmd == null)
                {
                    _checkOldPasswordCmd = new RelayCommand(
                        param => CheckOldPassword()
                            );
                }
                return _checkOldPasswordCmd;
            }
        }

        async void CheckOldPassword()
        {
            
            if (String.Compare(OldPassword.Text, Fields.Password.ValAsString) == 0)
            {
                NewPassword.IsEnabled = true;
                NewPasswordBtn.IsEnabled = true;
            }
            else
            {
                var PasswordErr = new MessageDialog("Wrong Password");
                await PasswordErr.ShowAsync();
                OldPassword.Text = "";
                NewPassword.IsEnabled = false;
                NewPasswordBtn.IsEnabled = false;
            }
        }

        private ICommand _setNewPasswordCmd;
        public ICommand SetNewPasswordCmd
        {
            get
            {
                if (_setNewPasswordCmd == null)
                {
                    _setNewPasswordCmd = new RelayCommand(
                        param => SetNewPassword()
                            );
                }
                return _setNewPasswordCmd;
            }
        }

        public uint NewPassvordValue;
        async void SetNewPassword()
        {
            string passwordLenghTxt = "Password must be 6 numbers";
            string passwordErrorTxt = "Password must be numeric value";
            string passwordSuccessTxt = "Password Changed";

            try
            {
                NewPassvordValue = Convert.ToUInt32(NewPassword.Text);
                if ((NewPassword.Text.Length == 6) || (NewPassvordValue == 0))
                {
                    Fields.Password.Value = NewPassvordValue;
                    UpdateParam(Fields.Password);

                    var PasswordSuccess = new MessageDialog(passwordSuccessTxt);
                    await PasswordSuccess.ShowAsync();
                }
                else
                {
                    var PasswordLenghtErr = new MessageDialog(passwordLenghTxt);
                    await PasswordLenghtErr.ShowAsync();
                }
            }
            catch
            {
                var PasswordErr = new MessageDialog(passwordErrorTxt);
                await PasswordErr.ShowAsync();
            }

            NewPassword.Text = "";
            OldPassword.Text = "";
            NewPassword.IsEnabled = false;
            NewPasswordBtn.IsEnabled = false;
        }

        #endregion

        #region UpdateClock

        private ICommand _updateClock;
        public ICommand UpdateClock
        {
            get
            {
                if (_updateClock == null)
                {
                    _updateClock = new RelayCommand(
                        param => SendSetTimeCommand()
                            );
                }
                return _updateClock;
            }
        }

        private DateTime NewDateTime;
        private void SendSetTimeCommand()
        {
            int Year, Month, Day, Hour, Minute;            

            if((DatePicker.Value == null) && (TimePicker.Value == null))
            {
                return;
            }
            else
            {
                SetDateTime setClockCmd = new SetDateTime(IrConnection.portHandler);

                if (DatePicker.Value != null)
                {
                    Year = DatePicker.Value.Value.Year;
                    Month = DatePicker.Value.Value.Month;
                    Day = DatePicker.Value.Value.Day;
                }
                else
                {
                    Year = Fields.ConvDateTime.Year;
                    Month = Fields.ConvDateTime.Month;
                    Day = Fields.ConvDateTime.Day;
                }

                if (TimePicker.Value != null)
                {
                    Hour = TimePicker.Value.Value.Hour;
                    Minute = TimePicker.Value.Value.Minute;
                }
                else
                {
                    Hour = Fields.ConvDateTime.Hour;
                    Minute = Fields.ConvDateTime.Minute;
                }

                NewDateTime = new DateTime(Year, Month, Day, Hour, Minute, 0, 0);
                setClockCmd.DateTimeToSet = NewDateTime;
                setClockCmd.CommandCompleted += SetClockCmd_CommandCompleted;
                //IrCOMPortManager.Instance.CommandList.Add(setClockCmd);
                setClockCmd.send();
            }
        }

        private ICommand _updateClockToNow;
        public ICommand UpdateClockToNow
        {
            get
            {
                if (_updateClockToNow == null)
                {
                    _updateClockToNow = new RelayCommand(
                        param => SendSetTimeNowCommand()
                            );
                }
                return _updateClockToNow;
            }
        }

        private void SendSetTimeNowCommand()
        {
            SetDateTime setClockCmd = new SetDateTime(IrConnection.portHandler);
            NewDateTime = DateTime.Now;
            setClockCmd.DateTimeToSet = NewDateTime;
            setClockCmd.CommandCompleted += SetClockCmd_CommandCompleted;
            //IrConnection.CommandList.Add(setClockCmd);
            setClockCmd.send();
        }

        private async void SetClockCmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SetDateTime cmd = sender as SetDateTime;
                if (cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
                {
                    Fields.ConvDateTime = NewDateTime;
                }
                //IrConnection.ExtCommandCompleted = IrCOMPortManager.CommandState.WaitForNew;
                //IrConnection.CommandList.Remove(cmd);
            });
        }

        #endregion

        #region ConfigurationFile

        private ICommand _saveCfgCmd;
        public ICommand SaveCfgCmd
        {
            get
            {
                if (_saveCfgCmd == null)
                {
                    _saveCfgCmd = new RelayCommand(
                        param => SaveConfigToFile()
                            );
                }
                return _saveCfgCmd;
            }
        }

        public async void SaveConfigToFile()
        {
            List<VariableImage> ParList = new List<VariableImage>();

            //FlowrateMeasOption***************************************************************************************************************************************
            ParList.Add(FileManager.GetVariableParams(Fields.FlowrateTechUnit, (UInt32)(Fields.FlowrateTechUnit.Value)));
            ParList.Add(FileManager.GetVariableParams(Fields.FlowrateTimeBase, (UInt32)(Fields.FlowrateTimeBase.Value)));
            ParList.Add(FileManager.GetVariableParams(Fields.AccumulatorsTechUnit, (UInt32)(Fields.AccumulatorsTechUnit.Value)));
            ParList.Add(FileManager.GetVariableParams(Fields.FlowrateNdigit, (UInt32)(Fields.FlowrateNdigit.Value)));
            ParList.Add(FileManager.GetVariableParams(Fields.AccumulatorNdigit, (UInt32)(Fields.AccumulatorNdigit.Value)));
            ParList.Add(FileManager.GetVariableParams(Fields.ProcessLogPeriod, (UInt32)(Fields.ProcessLogPeriod.Value)));

            //Save File************************************************************************************************************************************************            
            FileManager.SavedFileName = string.Format("{0}_{1}_{2}.{3}", TargetVariablesFields.Instance.ConverterId.ValAsString, TargetVariablesFields.Instance.DeviceName.ValAsString, DateTime.Now.ToString("yyyyMMddhhmmss"), "cfg");
            SerializableStorage<VariableImage>.Save(FileManager.SavedFileName, FileManager.CurrentFolder.Path, ParList);

            //Aggiorno lista file**************************************************************************************************************************************        
            await FileManager.UpdateFileList(FileManager.CurrentFolder);
            //*********************************************************************************************************************************************************
            var SuccessMsg = new MessageDialog( string.Format("{0}{1}{2}", "File ", FileManager.SavedFileName, " saved") );
            await SuccessMsg.ShowAsync();
        }

        private ICommand _loadCfgCmd;
        public ICommand LoadCfgCmd
        {
            get
            {
                if (_loadCfgCmd == null)
                {
                    _loadCfgCmd = new RelayCommand(
                        param => LoadCfg()
                            );
                }
                return _loadCfgCmd;
            }
        }

        private void LoadCfg()
        {
            FileSelectorPnl.Visibility = Visibility.Visible;
            SaveCfgBtn.Visibility = Visibility.Collapsed;
            LoadCfgBtn.Visibility = Visibility.Collapsed;
            EditCfgBtn.Visibility = Visibility.Collapsed;
            SaveEditBtn.Visibility = Visibility.Collapsed;
            ExitEditBtn.Visibility = Visibility.Collapsed;
            SelectCfgBtn.Visibility = Visibility.Visible;
            CancelCfgBtn.Visibility = Visibility.Visible;
            FileBox.SelectedIndex = 0;
        }

        private ICommand _exitLoadCmd;
        public ICommand ExitLoadCmd
        {
            get
            {
                if (_exitLoadCmd == null)
                {
                    _exitLoadCmd = new RelayCommand(
                        param => ExitLoad()
                            );
                }
                return _exitLoadCmd;
            }
        }

        private void ExitLoad()
        {
            FileSelectorPnl.Visibility = Visibility.Collapsed;
            SaveCfgBtn.Visibility = Visibility.Visible;
            LoadCfgBtn.Visibility = Visibility.Visible;
            EditCfgBtn.Visibility = Visibility.Visible;
            SaveEditBtn.Visibility = Visibility.Collapsed;
            ExitEditBtn.Visibility = Visibility.Collapsed;
            SelectCfgBtn.Visibility = Visibility.Collapsed;
            CancelCfgBtn.Visibility = Visibility.Collapsed;
        }

        private ICommand _loadFileCmd;
        public ICommand LoadFileCmd
        {
            get
            {
                if (_loadFileCmd == null)
                {
                    _loadFileCmd = new RelayCommand(
                        param => LoadConfigFromFile()
                            );
                }
                return _loadFileCmd;
            }
        }

        private FileData _selectedFile;
        public FileData SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (value != _selectedFile)
                {
                    _selectedFile = value;
                    OnPropertyChanged("SelectedFile");
                }
            }
        }

        private async void LoadConfigFromFile()
        {
            List<VariableImage> ParList = new List<VariableImage>();

            ParList = await SerializableStorage<VariableImage>.Load(SelectedFile.Name, SelectedFile.FullPath);

            Fields.FlowrateTechUnit.Value = (byte)ParList[0].Value;
            Fields.WriteEepromVariable(Fields.FlowrateTechUnit);

            Fields.FlowrateTimeBase.Value = (byte)ParList[1].Value;
            Fields.WriteEepromVariable(Fields.FlowrateTimeBase);

            Fields.AccumulatorsTechUnit.Value = (byte)ParList[2].Value;
            Fields.WriteEepromVariable(Fields.AccumulatorsTechUnit);

            Fields.FlowrateNdigit.Value = (byte)ParList[3].Value;
            Fields.WriteEepromVariable(TargetVariablesFields.Instance.FlowrateNdigit);

            Fields.AccumulatorNdigit.Value = (byte)ParList[4].Value;
            Fields.WriteEepromVariable(TargetVariablesFields.Instance.AccumulatorNdigit);

            Fields.ProcessLogPeriod.Value = (byte)ParList[5].Value;
            Fields.WriteEepromVariable(TargetVariablesFields.Instance.ProcessLogPeriod);

            FileSelectorPnl.Visibility = Visibility.Collapsed;
            SaveCfgBtn.Visibility = Visibility.Visible;
            LoadCfgBtn.Visibility = Visibility.Visible;
            EditCfgBtn.Visibility = Visibility.Visible;
            SaveEditBtn.Visibility = Visibility.Collapsed;
            ExitEditBtn.Visibility = Visibility.Collapsed;
            SelectCfgBtn.Visibility = Visibility.Collapsed;
            CancelCfgBtn.Visibility = Visibility.Collapsed;

            var SuccessMsg = new MessageDialog(string.Format("{0}{1}{2}", "File ", FileManager.SavedFileName, " loaded"));
            await SuccessMsg.ShowAsync();

        }

        private List<VariableImage> EditList = new List<VariableImage>();

        private bool _editingOn;
        public bool EditingOn
        {
            get { return _editingOn; }
            set
            {
                if (value != _editingOn)
                {
                    _editingOn = value;
                    OnPropertyChanged("EditingOn");
                }
            }
        }

        private ICommand _editCfgCmd;
        public ICommand EditCfgCmd
        {
            get
            {
                if (_editCfgCmd == null)
                {
                    _editCfgCmd = new RelayCommand(
                        param => EditConfigFile()
                            );
                }
                return _editCfgCmd;
            }
        }

        private void EditConfigFile()
        {
            Fields.EditCfgMessage       = "Editor Mode";
            FileSelectorPnl.Visibility  = Visibility.Collapsed;
            SaveCfgBtn.Visibility       = Visibility.Collapsed;
            LoadCfgBtn.Visibility       = Visibility.Collapsed;
            EditCfgBtn.Visibility       = Visibility.Collapsed;
            SaveEditBtn.Visibility      = Visibility.Visible;
            ExitEditBtn.Visibility      = Visibility.Visible;
            SelectCfgBtn.Visibility     = Visibility.Collapsed;
            CancelCfgBtn.Visibility     = Visibility.Collapsed;
            EditingOn                   = true;
        }       

        private ICommand _saveEditCmd;
        public ICommand SaveEditCmd
        {
            get
            {
                if (_saveEditCmd == null)
                {
                    _saveEditCmd = new RelayCommand(
                        param => SaveEdit()
                            );
                }
                return _saveEditCmd;
            }
        }

        private void SaveEdit()
        {
            Fields.EditCfgMessage = "";
            FileSelectorPnl.Visibility = Visibility.Collapsed;
            SaveCfgBtn.Visibility = Visibility.Visible;
            LoadCfgBtn.Visibility = Visibility.Visible;
            EditCfgBtn.Visibility = Visibility.Visible;
            SaveEditBtn.Visibility = Visibility.Collapsed;
            ExitEditBtn.Visibility = Visibility.Collapsed;
            SelectCfgBtn.Visibility = Visibility.Collapsed;
            CancelCfgBtn.Visibility = Visibility.Collapsed;
            SaveEditFile();
        }

        private async void SaveEditFile()
        {
            //Save File************************************************************************************************************************************************            
            FileManager.SavedFileName = string.Format("ConfigFile_{0}.cfg", DateTime.Now.ToString("yyyyMMddhhmmss") );
            SerializableStorage<VariableImage>.Save(FileManager.SavedFileName, FileManager.CurrentFolder.Path, EditList);
            //Aggiorno lista file**************************************************************************************************************************************
            await FileManager.UpdateFileList(FileManager.CurrentFolder);
            //*********************************************************************************************************************************************************
            var SuccessMsg = new MessageDialog(string.Format("{0}{1}{2}", "File ", FileManager.SavedFileName, " saved"));
            await SuccessMsg.ShowAsync();
        }

        private ICommand _exitEditCmd;
        public ICommand ExitEditCmd
        {
            get
            {
                if (_exitEditCmd == null)
                {
                    _exitEditCmd = new RelayCommand(
                        param => ExitEdit()
                            );
                }
                return _exitEditCmd;
            }
        }

        private void ExitEdit()
        {
            Fields.EditCfgMessage = "";
            FileSelectorPnl.Visibility = Visibility.Collapsed;
            SaveCfgBtn.Visibility = Visibility.Visible;
            LoadCfgBtn.Visibility = Visibility.Visible;
            EditCfgBtn.Visibility = Visibility.Visible;
            SaveEditBtn.Visibility = Visibility.Collapsed;
            ExitEditBtn.Visibility = Visibility.Collapsed;
            SelectCfgBtn.Visibility = Visibility.Collapsed;
            CancelCfgBtn.Visibility = Visibility.Collapsed;
            EditingOn = false;
        }

        #endregion

        #region Read Bundle

        private ICommand _bundleReadCmd;
        public ICommand BundleReadCmd
        {
            get
            {
                if (_bundleReadCmd == null)
                {
                    _bundleReadCmd = new RelayCommand(
                        param => BundleRead()
                            );
                }
                return _bundleReadCmd;
            }
        }

        private void BundleRead()
        {
            IrConnection.ReadBundle((byte)BundleSelectBox.SelectedValue);
            IrConnection.ReadBundleCompleted += IrConnection_ReadBundleCompleted;
            BundleReadState.Text = "Reading...";
        }

        private void IrConnection_ReadBundleCompleted(object sender, PropertyChangedEventArgs e)
        {
            IrCOMPortManager cmd = sender as IrCOMPortManager;
            BundleReadState.Text = cmd.ReadBundle_CommandResult.Message;
        }

        #endregion

        #region Reset Contatori

        private ICommand _resetTotalPositive;
        public ICommand ResetTotalPositive
        {
            get
            {
                if (_resetTotalPositive == null)
                {
                    _resetTotalPositive = new RelayCommand(
                        param => ResetTotalPositiveAccumulatorCheck()
                            );
                }
                return _resetTotalPositive;
            }
        }

        private async void ResetTotalPositiveAccumulatorCheck()
        {
            var messageDialog = new MessageDialog("Are you sure to reset the accumulator?");

            messageDialog.Title = "Reset Total Positive Accumulator";
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(ResetTotalPositiveAccumulatorConfirm)));
            messageDialog.Commands.Add(new UICommand("Cancel"));
            messageDialog.DefaultCommandIndex = 0;
            await messageDialog.ShowAsync();
        }

        private void ResetTotalPositiveAccumulatorConfirm(IUICommand command)
        {
            SendOperationCommand(BoardOperation.operationResetPositiveTotalizer);
        }

        private ICommand _resetTotalNegative;
        public ICommand ResetTotalNegative
        {
            get
            {
                if (_resetTotalNegative == null)
                {
                    _resetTotalNegative = new RelayCommand(
                        param => ResetTotalNegativeAccumulatorCheck()
                            );
                }
                return _resetTotalNegative;
            }
        }

        private async void ResetTotalNegativeAccumulatorCheck()
        {
            var messageDialog = new MessageDialog("Are you sure to reset the accumulator?");

            messageDialog.Title = "Reset Total Negative Accumulator";
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(ResetTotalNegativeAccumulatorConfirm)));
            messageDialog.Commands.Add(new UICommand("Cancel"));
            messageDialog.DefaultCommandIndex = 0;
            await messageDialog.ShowAsync();
        }

        private void ResetTotalNegativeAccumulatorConfirm(IUICommand command)
        {
            SendOperationCommand(BoardOperation.operationResetNegativeTotalizer);
        }


        private ICommand _resetPartialPositive;
        public ICommand ResetPartialPositive
        {
            get
            {
                if (_resetPartialPositive == null)
                {
                    _resetPartialPositive = new RelayCommand(
                        param => ResetPartialPositiveAccumulatorCheck()
                            );
                }
                return _resetPartialPositive;
            }
        }

        private async void ResetPartialPositiveAccumulatorCheck()
        {
            var messageDialog = new MessageDialog("Are you sure to reset the accumulator?");

            messageDialog.Title = "Reset Partial Positive Accumulator";
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(ResetPartialPositiveAccumulatorConfirm)));
            messageDialog.Commands.Add(new UICommand("Cancel"));
            messageDialog.DefaultCommandIndex = 0;
            await messageDialog.ShowAsync();
        }

        private void ResetPartialPositiveAccumulatorConfirm(IUICommand command)
        {
            SendOperationCommand(BoardOperation.operationResetPositiveAccumulator);
        }

        private ICommand _resetPartialNegative;
        public ICommand ResetPartialNegative
        {
            get
            {
                if (_resetPartialNegative == null)
                {
                    _resetPartialNegative = new RelayCommand(
                        param => ResetNegativeAccumulatorCheck()
                            );
                }
                return _resetPartialNegative;
            }
        }

        private async void ResetNegativeAccumulatorCheck()
        {
            var messageDialog = new MessageDialog("Are you sure to reset the accumulator?");

            messageDialog.Title = "Reset Partial Negative Accumulator";
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(ResetPartialNegativeAccumulatorConfirm)));
            messageDialog.Commands.Add(new UICommand("Cancel"));
            messageDialog.DefaultCommandIndex = 0;
            await messageDialog.ShowAsync();
        }

        private void ResetPartialNegativeAccumulatorConfirm(IUICommand command)
        {
            SendOperationCommand(BoardOperation.operationResetNegativeAccumulator);
        }

        private void SendOperationCommand(BoardOperation op)
        {
            Operation opCmd = new Operation( IrCOMPortManager.Instance.portHandler );
            opCmd.OperationCode = op;
            opCmd.CommandCompleted += OpCmd_CommandCompleted;
            opCmd.send();
        }

        private async void OpCmd_CommandCompleted(object sender, PropertyChangedEventArgs e)
        {
            Operation cmd = sender as Operation;

            if (cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
                Settings.Instance.OtherMeasuresChanged = true;

            var messageDialog = new MessageDialog(cmd.Result.Message);
            await messageDialog.ShowAsync();
        }

        #endregion

        #region Metodi

        private async void InitComboBoxes()
        {
            //ComboBoxes Init -------------------------------------------------------------

            await FileManager.UpdateFileList(FileManager.CurrentFolder);

            FlowrateTUbox.ItemsSource = Fields.FlowrateTechUnit.Options;
            FlowrateTUbox.SelectedIndex = Fields.FlowrateTechUnit.Value - 1;

            FlowrateTBbox.ItemsSource = Fields.FlowrateTimeBase.Options;
            FlowrateTBbox.SelectedIndex = Fields.FlowrateTimeBase.Value - 1;

            TotalizerTUbox.ItemsSource = Fields.AccumulatorsTechUnit.Options;
            if (Fields.AccumulatorsTechUnit.Value != 0)
                TotalizerTUbox.SelectedIndex = Fields.AccumulatorsTechUnit.Value - 2;

            VisualizationOptbox.ItemsSource = Fields.SpecialVisualization.Options;
            VisualizationOptbox.SelectedIndex = Fields.SpecialVisualization.Value;

            MainsLineFrequencybox.ItemsSource = Fields.MainsLineFrequency.Options;
            MainsLineFrequencybox.SelectedIndex = Fields.MainsLineFrequency.Value;

            AdcGainbox.ItemsSource = Fields.AdcGain.Options;
            AdcGainbox.SelectedIndex = Fields.AdcGain.Value;

            SD24SamplingIndexbox.ItemsSource = Fields.SD24SamplingIndex.Options;
            SD24SamplingIndexbox.SelectedIndex = Fields.SD24SamplingIndex.Value;

            SensorIsInsertionbox.ItemsSource = Fields.SensorIsInsertion.Options;
            SensorIsInsertionbox.SelectedIndex = Fields.SensorIsInsertion.Value;

            PulseOutputTechUnitbox.ItemsSource = Fields.PulseOutputTechUnit.Options;
            PulseOutputTechUnitbox.SelectedIndex = Fields.PulseOutputTechUnit.Value - 1;

            EmptyPipeCfgbox.ItemsSource = Fields.EmptyPipeCfg.Options;
            EmptyPipeCfgbox.SelectedIndex = Fields.EmptyPipeCfg.Value;

            WaterDetectMeasEnablebox.ItemsSource = Fields.WaterDetectMeasEnable.Options;
            WaterDetectMeasEnablebox.SelectedIndex = Fields.WaterDetectMeasEnable.Value;

            PressureOptionbox.ItemsSource = Fields.PressureOption.Options;
            PressureOptionbox.SelectedIndex = Fields.PressureOption.Value;

            EnergyOptionbox.ItemsSource = Fields.EnergyOption.Options;
            EnergyOptionbox.SelectedIndex = Fields.EnergyOption.Value;

            GSM_Installedbox.ItemsSource = Fields.GSM_Installed.Options;
            GSM_Installedbox.SelectedIndex = Fields.GSM_Installed.Value;

            WakeupPoweroffbox.ItemsSource = Fields.WakeupPoweroff.Options;
            WakeupPoweroffbox.SelectedIndex = Fields.WakeupPoweroff.Value;

            BundleSelectBox.SelectedValue = 1;
        }

        private void InitUIElements()
        {
            Settings.Instance.UpdateRunning = false;
            NewPassword.IsEnabled = false;
            NewPasswordBtn.IsEnabled = false;
            Fields.EditCfgMessage = "";

            FileSelectorPnl.Visibility = Visibility.Collapsed;
            SaveCfgBtn.Visibility = Visibility.Visible;
            LoadCfgBtn.Visibility = Visibility.Visible;
            EditCfgBtn.Visibility = Visibility.Visible;
            SaveEditBtn.Visibility = Visibility.Collapsed;
            ExitEditBtn.Visibility = Visibility.Collapsed;
            SelectCfgBtn.Visibility = Visibility.Collapsed;
            CancelCfgBtn.Visibility = Visibility.Collapsed;
        }

        private void UpdateParam(ITargetWritable var, string NewValue)
        {
            if(EditingOn)
            {
                IEEPROMvariable NewVar = var as IEEPROMvariable;
                if(NewVar.DataType == TargetDataType.TYPE_STR)
                    EditList.Add(FileManager.GetVariableParams(NewVar, 0));
                else
                    EditList.Add(FileManager.GetVariableParams(NewVar, Convert.ToUInt32(NewValue)));
            }
            else
            {
                //Fields.WriteEepromVariable((IEEPROMvariable)var);
                IrCOMPortManager.Instance.WriteParam((IEEPROMvariable)var );
                IrCOMPortManager.Instance.WriteParamCompleted += Instance_WriteParamCompleted;
            }
        }

        private async void Instance_WriteParamCompleted(object sender, PropertyChangedEventArgs e)
        {
            IrCOMPortManager cmd = sender as IrCOMPortManager;

            var messageDialog = new MessageDialog(cmd.WriteParam_CommandResult.Message);

            messageDialog.Title = "Param Write";
            await messageDialog.ShowAsync();
        }

        private void UpdateParam(ITargetWritable var)
        {
            //Fields.WriteEepromVariable((IEEPROMvariable)var);
            IrCOMPortManager.Instance.WriteParam((IEEPROMvariable)var);
            IrCOMPortManager.Instance.WriteParamCompleted += Instance_WriteParamCompleted;
        }

        #endregion

        #region ObservableObject

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
