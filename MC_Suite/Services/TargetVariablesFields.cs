﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MC_Suite.Euromag.Protocols;
using MC_Suite.Euromag.Protocols.StdCommands;
using Windows.UI.Xaml;
using System.Collections.Generic;


namespace MC_Suite.Services
{
    public class TargetVariablesFields: INotifyPropertyChanged
    {
        private static TargetVariablesFields _instance;
        public static TargetVariablesFields Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TargetVariablesFields();
                return _instance;
            }
        }

        protected TargetVariablesFields()
        {
            InitVarBundles();            
        }

        #region RAM measures vars

        private FRATE_MS _flowrateMS;
        public FRATE_MS FlowRateMS
        {
            get
            {
                if (_flowrateMS == null)
                {
                    _flowrateMS = new FRATE_MS();
                    AddVarToBundle(ref _ramMeasBundleCmd, _flowrateMS);
                }
                return _flowrateMS;
            }

        }

        private FRATE_MS_VERIF _flowrateMS_Verif;
        public FRATE_MS_VERIF FlowRateMS_Verif
        {
            get
            {
                if (_flowrateMS_Verif == null)
                {
                    _flowrateMS_Verif = new FRATE_MS_VERIF();
                    //AddVarToBundle(ref _ramMeasBundleCmd, _flowrateMS);
                }
                return _flowrateMS_Verif;
            }

        }


        private FRATE_UTTB _flowrateTU;
        public FRATE_UTTB FlowRateTU
        {
            get
            {
                if (_flowrateTU == null)
                {
                    _flowrateTU = new FRATE_UTTB();
                    AddVarToBundle(ref _ramMeasBundleCmd, _flowrateTU);
                }
                return _flowrateTU;
            }
        }

        private FRATE_PERC _flowRatePERC;
        public FRATE_PERC FlowRatePERC
        {
            get
            {
                if (_flowRatePERC == null)
                {
                    _flowRatePERC = new FRATE_PERC();
                    AddVarToBundle(ref _ramMeasBundleCmd, _flowRatePERC);
                }
                return _flowRatePERC;
            }
        }

        private TPACC_M3 _tpAccM3;
        public TPACC_M3 TotalPositiveM3
        {
            get
            {
                if (_tpAccM3 == null)
                {
                    _tpAccM3 = new TPACC_M3();
                    AddVarToBundle(ref _ramMeasBundleCmd, _tpAccM3);
                }
                return _tpAccM3;
            }
        }

        private TNACC_M3 _tnAccM3;
        public TNACC_M3 TotalNegativeM3
        {
            get
            {
                if (_tnAccM3 == null)
                {
                    _tnAccM3 = new TNACC_M3();
                    AddVarToBundle(ref _ramMeasBundleCmd, _tnAccM3);
                }
                return _tnAccM3;
            }
        }

        private PPACC_M3 _ppAccM3;
        public PPACC_M3 PartialPositiveM3
        {
            get
            {
                if (_ppAccM3 == null)
                {
                    _ppAccM3 = new PPACC_M3();
                    AddVarToBundle(ref _ramMeasBundleCmd, _ppAccM3);
                }
                return _ppAccM3;
            }
        }

        private PNACC_M3 _pnAccM3;
        public PNACC_M3 PartialNegativeM3
        {
            get
            {
                if (_pnAccM3 == null)
                {
                    _pnAccM3 = new PNACC_M3();
                    AddVarToBundle(ref _ramMeasBundleCmd, _pnAccM3);
                }
                return _pnAccM3;
            }
        }

        #endregion // RAM measures vars

        #region RAM others vars

        private EVENTS_COUNT _eventsCount;
        public EVENTS_COUNT EventsCount
        {
            get
            {
                if (_eventsCount == null)
                {
                    _eventsCount = new EVENTS_COUNT();
                    AddVarToBundle(ref _ramOthersBundleCmd, _eventsCount);
                }
                return _eventsCount;
            }
        }

        private LOG_LAST_ROW _logLastRow;
        public LOG_LAST_ROW LogLastRow
        {
            get
            {
                if (_logLastRow == null)
                {
                    _logLastRow = new LOG_LAST_ROW();
                    AddVarToBundle(ref _ramOthersBundleCmd, _logLastRow);
                }
                return _logLastRow;
            }
        }

        private FW_VER _fwVersion;
        public FW_VER FwVersion
        {
            get
            {
                if (_fwVersion == null)
                {
                    _fwVersion = new FW_VER();
                    AddVarToBundle(ref _ramOthersBundleCmd, _fwVersion);
                }
                return _fwVersion;
            }
        }

        private FW_REV _fwRevision;
        public FW_REV FwRevision
        {
            get
            {
                if (_fwRevision == null)
                {
                    _fwRevision = new FW_REV();
                    AddVarToBundle(ref _ramOthersBundleCmd, _fwRevision);
                }
                return _fwRevision;
            }
        }

        private HW_INFO _hW_Version;
        public HW_INFO HW_Version
        {
            get
            {
                if (_hW_Version == null)
                {
                    _hW_Version = new HW_INFO();
                    //AddVarToBundle(ref _ramOthersBundleCmd, _hW_Version);
                }
                return _hW_Version;
            }
        }

        private ICOIL_MA _icoil_ma;
        public ICOIL_MA ICoil_ma
        {
            get
            {
                if (_icoil_ma == null)
                {
                    _icoil_ma = new ICOIL_MA();
                }
                return _icoil_ma;
            }
        }

        #endregion // RAM others vars

        #region RAM Temperature & Pressure Vars

        private TEMP_T1 _temperatureT1;
        public TEMP_T1 TemperatureT1
        {
            get
            {
                if (_temperatureT1 == null)
                {
                    _temperatureT1 = new TEMP_T1();
                    AddVarToBundle(ref _ramT1T2PressBundleCmd, _temperatureT1);
                }
                return _temperatureT1;
            }

        }

        private TEMP_T2 _temperatureT2;
        public TEMP_T2 TemperatureT2
        {
            get
            {
                if (_temperatureT2 == null)
                {
                    _temperatureT2 = new TEMP_T2();
                    AddVarToBundle(ref _ramT1T2PressBundleCmd, _temperatureT2);
                }
                return _temperatureT2;
            }

        }

        private PRESSURE _pressureProbe;
        public PRESSURE PressureProbe
        {
            get
            {
                if (_pressureProbe == null)
                {
                    _pressureProbe = new PRESSURE();
                    AddVarToBundle(ref _ramT1T2PressBundleCmd, _pressureProbe);
                }
                return _pressureProbe;
            }

        }

        #endregion

        #region RAM Bluetooth/RS485 vars

        private BT_HW_VER _bluetoothRS485_HW_Version;
        public BT_HW_VER BluetoothRS485_HW_Version
        {
            get
            {
                if (_bluetoothRS485_HW_Version == null)
                {
                    _bluetoothRS485_HW_Version = new BT_HW_VER();
                    AddVarToBundle(ref _ramBluetoothRS485Cmd, _bluetoothRS485_HW_Version);
                }
                return _bluetoothRS485_HW_Version;
            }
        }

        private BT_FW_VER _bluetoothRS485_FW_Version;
        public BT_FW_VER BluetoothRS485_FW_Version
        {
            get
            {
                if (_bluetoothRS485_FW_Version == null)
                {
                    _bluetoothRS485_FW_Version = new BT_FW_VER();
                    AddVarToBundle(ref _ramBluetoothRS485Cmd, _bluetoothRS485_FW_Version);
                }
                return _bluetoothRS485_FW_Version;
            }
        }

        private BT_FW_REV _bluetoothRS485_FW_Revision;
        public BT_FW_REV BluetoothRS485_FW_Revision
        {
            get
            {
                if (_bluetoothRS485_FW_Revision == null)
                {
                    _bluetoothRS485_FW_Revision = new BT_FW_REV();
                    AddVarToBundle(ref _ramBluetoothRS485Cmd, _bluetoothRS485_FW_Revision);
                }
                return _bluetoothRS485_FW_Revision;
            }
        }
        
        private BT_STATUS _bluetoothRS485_Status;
        public BT_STATUS BluetoothRS485_Status
        {
            get
            {
                if (_bluetoothRS485_Status == null)
                {
                    _bluetoothRS485_Status = new BT_STATUS();
                    AddVarToBundle(ref _ramBluetoothRS485Cmd, _bluetoothRS485_Status);
                }
                return _bluetoothRS485_Status;
            }
        }

        #endregion

        #region IO RAM Variables

        private INFO_BOARD_MODEL _info_BoardModel;
        public INFO_BOARD_MODEL Info_BoardModel
        {
            get
            {
                if (_info_BoardModel == null)
                {
                    _info_BoardModel = new INFO_BOARD_MODEL();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _info_BoardModel);
                }
                return _info_BoardModel;
            }
        }

        private INFO_FW_VER _info_FwVersion;
        public INFO_FW_VER Info_FwVersion
        {
            get
            {
                if (_info_FwVersion == null)
                {
                    _info_FwVersion = new INFO_FW_VER();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _info_FwVersion);
                }
                return _info_FwVersion;
            }
        }

        private INFO_FW_REV _info_FwRevision;
        public INFO_FW_REV Info_FwRevision
        {
            get
            {
                if (_info_FwRevision == null)
                {
                    _info_FwRevision = new INFO_FW_REV();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _info_FwRevision);
                }
                return _info_FwRevision;
            }
        }

        private INFO_FW_DATE _info_FwDate;
        public INFO_FW_DATE Info_FwDate
        {
            get
            {
                if (_info_FwDate == null)
                {
                    _info_FwDate = new INFO_FW_DATE();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _info_FwDate);
                }
                return _info_FwDate;
            }
        }

        private WEB_IP_ADDRESS _web_IPAddress;
        public WEB_IP_ADDRESS Web_IPAddress
        {
            get
            {
                if (_web_IPAddress == null)
                {
                    _web_IPAddress = new WEB_IP_ADDRESS();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _web_IPAddress);
                }
                return _web_IPAddress;
            }
        }

        private WEB_MAC_ADDRESS _web_MACAddress;
        public WEB_MAC_ADDRESS Web_MACAddress
        {
            get
            {
                if (_web_MACAddress == null)
                {
                    _web_MACAddress = new WEB_MAC_ADDRESS();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _web_MACAddress);
                }
                return _web_MACAddress;
            }
        }

        private HMS_MODULE_STATE _hms_ModuleState;
        public HMS_MODULE_STATE HMS_ModuleState
        {
            get
            {
                if (_hms_ModuleState == null)
                {
                    _hms_ModuleState = new HMS_MODULE_STATE();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _hms_ModuleState);
                }
                return _hms_ModuleState;
            }
        }

        private HMS_CONN_STATE _hms_ConnectionState;
        public HMS_CONN_STATE HMS_ConnectionState
        {
            get
            {
                if (_hms_ConnectionState == null)
                {
                    _hms_ConnectionState = new HMS_CONN_STATE();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _hms_ConnectionState);
                }
                return _hms_ConnectionState;
            }
        }

        private DAC161_STATE _daC161_State;
        public DAC161_STATE DAC161_State
        {
            get
            {
                if (_daC161_State == null)
                {
                    _daC161_State = new DAC161_STATE();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _daC161_State);
                }
                return _daC161_State;
            }
        }

        private DAC161_OUT_mA _daC161_Out_mA;
        public DAC161_OUT_mA DAC161_Out_mA
        {
            get
            {
                if (_daC161_Out_mA == null)
                {
                    _daC161_Out_mA = new DAC161_OUT_mA();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _daC161_Out_mA);
                }
                return _daC161_Out_mA;
            }
        }

        private SYSTEM_TASK_STATES _system_TaskStates;
        public SYSTEM_TASK_STATES System_TaskStates
        {
            get
            {
                if (_system_TaskStates == null)
                {
                    _system_TaskStates = new SYSTEM_TASK_STATES();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _system_TaskStates);
                }
                return _system_TaskStates;
            }
        }

        private SYSTEM_TASK_ERRORS _system_TaskErrors;
        public SYSTEM_TASK_ERRORS System_TaskErrors
        {
            get
            {
                if (_system_TaskErrors == null)
                {
                    _system_TaskErrors = new SYSTEM_TASK_ERRORS();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _system_TaskErrors);
                }
                return _system_TaskErrors;
            }
        }

        private SYSTEM_DATE_TIME _system_DateTime;
        public SYSTEM_DATE_TIME System_DateTime
        {
            get
            {
                if (_system_DateTime == null)
                {
                    _system_DateTime = new SYSTEM_DATE_TIME();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _system_DateTime);
                }
                return _system_DateTime;
            }
        }

        private SYSTEM_MB_LOGS _system_MB_Logs;
        public SYSTEM_MB_LOGS System_MB_Logs
        {
            get
            {
                if (_system_MB_Logs == null)
                {
                    _system_MB_Logs = new SYSTEM_MB_LOGS();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _system_MB_Logs);
                }
                return _system_MB_Logs;
            }
        }

        private SYSTEM_MB_EVENTS _system_MB_Events;
        public SYSTEM_MB_EVENTS System_MB_Events
        {
            get
            {
                if (_system_MB_Events == null)
                {
                    _system_MB_Events = new SYSTEM_MB_EVENTS();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _system_MB_Events);
                }
                return _system_MB_Events;
            }
        }

        private FILE_SYSTEM_FLAGS _fileSystem_Flags;
        public FILE_SYSTEM_FLAGS FileSystem_Flags
        {
            get
            {
                if (_fileSystem_Flags == null)
                {
                    _fileSystem_Flags = new FILE_SYSTEM_FLAGS();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _fileSystem_Flags);
                }
                return _fileSystem_Flags;
            }
        }

        private FILE_SYSTEM_FILE_COUNT _fileSystem_FileCount;
        public FILE_SYSTEM_FILE_COUNT FileSystem_FileCount
        {
            get
            {
                if (_fileSystem_FileCount == null)
                {
                    _fileSystem_FileCount = new FILE_SYSTEM_FILE_COUNT();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _fileSystem_FileCount);
                }
                return _fileSystem_FileCount;
            }
        }

        private FILE_SYSTEM_SAVED_LOGS _fileSystem_SavedLogs;
        public FILE_SYSTEM_SAVED_LOGS FileSystem_SavedLogs
        {
            get
            {
                if (_fileSystem_SavedLogs == null)
                {
                    _fileSystem_SavedLogs = new FILE_SYSTEM_SAVED_LOGS();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _fileSystem_SavedLogs);
                }
                return _fileSystem_SavedLogs;
            }
        }

        private FILE_SYSTEM_SAVED_EVENTS _fileSystem_SavedEvents;
        public FILE_SYSTEM_SAVED_EVENTS FileSystem_SavedEvents
        {
            get
            {
                if (_fileSystem_SavedEvents == null)
                {
                    _fileSystem_SavedEvents = new FILE_SYSTEM_SAVED_EVENTS();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _fileSystem_SavedEvents);
                }
                return _fileSystem_SavedEvents;
            }
        }

        private BLUETOOTH_STATE _bluetooth_Connection_State;
        public BLUETOOTH_STATE Bluetooth_Connection_State
        {
            get
            {
                if (_bluetooth_Connection_State == null)
                {
                    _bluetooth_Connection_State = new BLUETOOTH_STATE();
                    AddVarToBundle(ref _ioVariablesBundleCmd, _bluetooth_Connection_State);
                }
                return _bluetooth_Connection_State;
            }
        }

        #endregion

        #region EEPROM parameters

        private PULSE_LENGTH _pulseLength;
        public PULSE_LENGTH PulseLength
        {
            get
            {
                if (_pulseLength == null)
                {
                    _pulseLength = new PULSE_LENGTH();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pulseLength);
                }
                return _pulseLength;
            }

        }

        private OFFSET_TEMP _temperatureOffset;
        public OFFSET_TEMP TemperatureOffset
        {
            get
            {
                if (_temperatureOffset == null)
                {
                    _temperatureOffset = new OFFSET_TEMP();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _temperatureOffset);
                }
                return _temperatureOffset;
            }
        }

        private SPEC_HEAT_sA _specificHeatCoeffA;
        public SPEC_HEAT_sA SpecificHeatCoeffA
        {
            get
            {
                if (_specificHeatCoeffA == null)
                {
                    _specificHeatCoeffA = new SPEC_HEAT_sA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _specificHeatCoeffA);
                }
                return _specificHeatCoeffA;
            }
        }

        private SPEC_HEAT_sB _specificHeatCoeffB;
        public SPEC_HEAT_sB SpecificHeatCoeffB
        {
            get
            {
                if (_specificHeatCoeffB == null)
                {
                    _specificHeatCoeffB = new SPEC_HEAT_sB();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _specificHeatCoeffB);
                }
                return _specificHeatCoeffB;
            }
        }

        private SPEC_HEAT_sC _specificHeatCoeffC;
        public SPEC_HEAT_sC SpecificHeatCoeffC
        {
            get
            {
                if (_specificHeatCoeffC == null)
                {
                    _specificHeatCoeffC = new SPEC_HEAT_sC();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _specificHeatCoeffC);
                }
                return _specificHeatCoeffC;
            }
        }

        private SPEC_HEAT_sD _specificHeatCoeffD;
        public SPEC_HEAT_sD SpecificHeatCoeffD
        {
            get
            {
                if (_specificHeatCoeffD == null)
                {
                    _specificHeatCoeffD = new SPEC_HEAT_sD();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _specificHeatCoeffD);
                }
                return _specificHeatCoeffD;
            }
        }

        private DENSITY_cA _densityCoeffA;
        public DENSITY_cA DensityCoeffA
        {
            get
            {
                if (_densityCoeffA == null)
                {
                    _densityCoeffA = new DENSITY_cA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _densityCoeffA);
                }
                return _densityCoeffA;
            }
        }

        private DENSITY_cB _densityCoeffB;
        public DENSITY_cB DensityCoeffB
        {
            get
            {
                if (_densityCoeffB == null)
                {
                    _densityCoeffB = new DENSITY_cB();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _densityCoeffB);
                }
                return _densityCoeffB;
            }
        }

        private DENSITY_cC _densityCoeffC;
        public DENSITY_cC DensityCoeffC
        {
            get
            {
                if (_densityCoeffC == null)
                {
                    _densityCoeffC = new DENSITY_cC();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _densityCoeffC);
                }
                return _densityCoeffC;
            }
        }

        private DENSITY_cD _densityCoeffD;
        public DENSITY_cD DensityCoeffD
        {
            get
            {
                if (_densityCoeffD == null)
                {
                    _densityCoeffD = new DENSITY_cD();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _densityCoeffD);
                }
                return _densityCoeffD;
            }
        }

        private DTMIN _deltaTempMin;
        public DTMIN DeltaTempMin
        {
            get
            {
                if (_deltaTempMin == null)
                {
                    _deltaTempMin = new DTMIN();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _deltaTempMin);
                }
                return _deltaTempMin;
            }
        }

        private TMEAS_TIME _temperatureMeasurePeriod;
        public TMEAS_TIME TemperatureMeasurePeriod
        {
            get
            {
                if (_temperatureMeasurePeriod == null)
                {
                    _temperatureMeasurePeriod = new TMEAS_TIME();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _temperatureMeasurePeriod);
                }
                return _temperatureMeasurePeriod;
            }
        }

        private PMEAS_TIME _pressureMeasurePeriod;
        public PMEAS_TIME PressureMeasurePeriod
        {
            get
            {
                if (_pressureMeasurePeriod == null)
                {
                    _pressureMeasurePeriod = new PMEAS_TIME();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pressureMeasurePeriod);
                }
                return _pressureMeasurePeriod;
            }
        }

        private ENERGY_OPTION _energyOption;
        public ENERGY_OPTION EnergyOption
        {
            get
            {
                if (_energyOption == null)
                {
                    _energyOption = new ENERGY_OPTION();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _energyOption);
                }
                return _energyOption;
            }
        }

        private PRESS_OPTION _pressureOption;
        public PRESS_OPTION PressureOption
        {
            get
            {
                if (_pressureOption == null)
                {
                    _pressureOption = new PRESS_OPTION();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pressureOption);
                }
                return _pressureOption;
            }
        }

        private PLOG_TIME _processLogPeriod;
        public PLOG_TIME ProcessLogPeriod
        {
            get
            {
                if (_processLogPeriod == null)
                {
                    _processLogPeriod = new PLOG_TIME();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _processLogPeriod);
                }
                return _processLogPeriod;
            }
        }

        private CALIBR_TEMP _calibrationTemperature;
        public CALIBR_TEMP CalibrationTemperature
        {
            get
            {
                if (_calibrationTemperature == null)
                {
                    _calibrationTemperature = new CALIBR_TEMP();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _calibrationTemperature);
                }
                return _calibrationTemperature;
            }
        }

        private CALIBR_VOLT _calibrationVoltage;
        public CALIBR_VOLT CalibrationVoltage
        {
            get
            {
                if (_calibrationVoltage == null)
                {
                    _calibrationVoltage = new CALIBR_VOLT();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _calibrationVoltage);
                }
                return _calibrationVoltage;
            }
        }

        private WAKEUP_SEC _wakeupPoweroff;
        public WAKEUP_SEC WakeupPoweroff
        {
            get
            {
                if (_wakeupPoweroff == null)
                {
                    _wakeupPoweroff = new WAKEUP_SEC();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _wakeupPoweroff);
                }
                return _wakeupPoweroff;
            }
        }

        private PWD_TIMEOUT _passwordTimeout;
        public PWD_TIMEOUT PasswordTimeout
        {
            get
            {
                if (_passwordTimeout == null)
                {
                    _passwordTimeout = new PWD_TIMEOUT();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _passwordTimeout);
                }
                return _passwordTimeout;
            }
        }

        private BATT_AUTOSAVE _batteryAutosave;
        public BATT_AUTOSAVE BatteryAutosave
        {
            get
            {
                if (_batteryAutosave == null)
                {
                    _batteryAutosave = new BATT_AUTOSAVE();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _batteryAutosave);
                }
                return _batteryAutosave;
            }
        }

        private TIMEOUT_TO_MAIN _timeoutToMain;
        public TIMEOUT_TO_MAIN TimeoutToMain
        {
            get
            {
                if (_timeoutToMain == null)
                {
                    _timeoutToMain = new TIMEOUT_TO_MAIN();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _timeoutToMain);
                }
                return _timeoutToMain;
            }
        }
        
        private PRESS_PROBE_CAL _pressProbeCalFact;
        public PRESS_PROBE_CAL PressProbeCalFact
        {
            get
            {
                if (_pressProbeCalFact == null)
                {
                    _pressProbeCalFact = new PRESS_PROBE_CAL();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pressProbeCalFact);
                }
                return _pressProbeCalFact;
            }
        }
        
        private PRESS_CURRENT_CAL _pressCurrCalFact;
        public PRESS_CURRENT_CAL PressCurrCalFact
        {
            get
            {
                if (_pressCurrCalFact == null)
                {
                    _pressProbeCalFact = new PRESS_PROBE_CAL();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pressCurrCalFact);
                }
                return _pressCurrCalFact;
            }
        }

        private SPECIAL_VISUAL _specialVisualization;
        public SPECIAL_VISUAL SpecialVisualization
        {
            get
            {
                if (_specialVisualization == null)
                {
                    _specialVisualization = new SPECIAL_VISUAL();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _specialVisualization);
                }
                return _specialVisualization;
            }
        }

        private CAL_LEV_4MA _calLev4mA;
        public CAL_LEV_4MA CalLev4mA
        {
            get
            {
                if (_calLev4mA == null)
                {
                    _calLev4mA = new CAL_LEV_4MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _calLev4mA);
                }
                return _calLev4mA;
            }
        }

        private CAL_LEV_20MA _calLev20mA;
        public CAL_LEV_20MA CalLev20mA
        {
            get
            {
                if (_calLev20mA == null)
                {
                    _calLev20mA = new CAL_LEV_20MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _calLev20mA);
                }
                return _calLev20mA;
            }
        }

        private MEAS_FOR_4_20_MA _measFor420mA;
        public MEAS_FOR_4_20_MA MeasFor420mA
        {
            get
            {
                if (_measFor420mA == null)
                {
                    _measFor420mA = new MEAS_FOR_4_20_MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _measFor420mA);
                }
                return _measFor420mA;
            }
        }

        private ENABLE_REV_FLOW_ON_4_20MA _enableRevFlowOn420mA;
        public ENABLE_REV_FLOW_ON_4_20MA EnableRevFlowOn420mA
        {
            get
            {
                if (_enableRevFlowOn420mA == null)
                {
                    _enableRevFlowOn420mA = new ENABLE_REV_FLOW_ON_4_20MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _enableRevFlowOn420mA);
                }
                return _enableRevFlowOn420mA;
            }
        }

        private VEL_LEV_FOR_4MA _velLevFor4mA;
        public VEL_LEV_FOR_4MA VelLevFor4mA
        {
            get
            {
                if (_velLevFor4mA == null)
                {
                    _velLevFor4mA = new VEL_LEV_FOR_4MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _velLevFor4mA);
                }
                return _velLevFor4mA;
            }
        }

        private VEL_LEV_FOR_20MA _velLevFor20mA;
        public VEL_LEV_FOR_20MA VelLevFor20mA
        {
            get
            {
                if (_velLevFor20mA == null)
                {
                    _velLevFor20mA = new VEL_LEV_FOR_20MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _velLevFor20mA);
                }
                return _velLevFor20mA;
            }
        }

        private PRESS_PROBE_CAL_OFFSET _pressProbeCalOffset;
        public PRESS_PROBE_CAL_OFFSET PressProbeCalOffset
        {
            get
            {
                if (_pressProbeCalOffset == null)
                {
                    _pressProbeCalOffset = new PRESS_PROBE_CAL_OFFSET();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pressProbeCalOffset);
                }
                return _pressProbeCalOffset;
            }
        }

        private ENABLE_BOARD_4_20_MA _enableBoard420mA;
        public ENABLE_BOARD_4_20_MA EnableBoard420mA
        {
            get
            {
                if (_enableBoard420mA == null)
                {
                    _enableBoard420mA = new ENABLE_BOARD_4_20_MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _enableBoard420mA);
                }
                return _enableBoard420mA;
            }
        }

        private REV_FLOW_ERR_VAL_4_20MA_OUT _revFlowErrVal420mA;
        public REV_FLOW_ERR_VAL_4_20MA_OUT RevFlowErrVal420mA
        {
            get
            {
                if (_revFlowErrVal420mA == null)
                {
                    _revFlowErrVal420mA = new REV_FLOW_ERR_VAL_4_20MA_OUT();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _revFlowErrVal420mA);
                }
                return _revFlowErrVal420mA;
            }
        }

        private EPIPE_ERR_VAL_4_20MA_OUT _epipeErrVal420mA;
        public EPIPE_ERR_VAL_4_20MA_OUT EpipeErrVal420mA
        {
            get
            {
                if (_epipeErrVal420mA == null)
                {
                    _epipeErrVal420mA = new EPIPE_ERR_VAL_4_20MA_OUT();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _epipeErrVal420mA);
                }
                return _epipeErrVal420mA;
            }
        }

        private COIL_ERR_VAL_4_20MA_OUT _coilErrVal420mA;
        public COIL_ERR_VAL_4_20MA_OUT CoilErrVal420mA
        {
            get
            {
                if (_coilErrVal420mA == null)
                {
                    _coilErrVal420mA = new COIL_ERR_VAL_4_20MA_OUT();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _coilErrVal420mA);
                }
                return _coilErrVal420mA;
            }
        }

        private GEN_ERR_VAL_4_20MA_OUT _genErrVal420mA;
        public GEN_ERR_VAL_4_20MA_OUT GenErrVal420mA
        {
            get
            {
                if (_genErrVal420mA == null)
                {
                    _genErrVal420mA = new GEN_ERR_VAL_4_20MA_OUT();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _genErrVal420mA);
                }
                return _genErrVal420mA;
            }
        }

        private OUT_LOWER_LIMIT_4_20MA _outLowerLimit420mA;
        public OUT_LOWER_LIMIT_4_20MA OutLowerLimit420mA
        {
            get
            {
                if (_outLowerLimit420mA == null)
                {
                    _outLowerLimit420mA = new OUT_LOWER_LIMIT_4_20MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _outLowerLimit420mA);
                }
                return _outLowerLimit420mA;
            }
        }

        private OUT_UPPER_LIMIT_4_20MA _outUpperLimit420mA;
        public OUT_UPPER_LIMIT_4_20MA OutUpperLimit420mA
        {
            get
            {
                if (_outUpperLimit420mA == null)
                {
                    _outUpperLimit420mA = new OUT_UPPER_LIMIT_4_20MA();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _outUpperLimit420mA);
                }
                return _outUpperLimit420mA;
            }
        }

        private NEG_PULSE_OUTPUT_MODE _negpulseOutMode;
        public NEG_PULSE_OUTPUT_MODE NegPulseOutMode
        {
            get
            {
                if (_negpulseOutMode == null)
                {
                    _negpulseOutMode = new NEG_PULSE_OUTPUT_MODE();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _negpulseOutMode);
                }
                return _negpulseOutMode;
            }
        }
        
        private PRESS_PROBE_CAL _pressProbeCalGain;
        public PRESS_PROBE_CAL PressProbeCalGain
        {
            get
            {
                if (_pressProbeCalGain == null)
                {
                    _pressProbeCalGain = new PRESS_PROBE_CAL();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pressProbeCalGain);
                }
                return _pressProbeCalGain;
            }
        }

        private PULSE_VOL _pulseOutputVolume;
        public PULSE_VOL PulseOutputVolume
        {
            get
            {
                if (_pulseOutputVolume == null)
                {
                    _pulseOutputVolume = new PULSE_VOL();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pulseOutputVolume);
                }
                return _pulseOutputVolume;
            }

        }

        private UT_PULSE _pulseOutputTechUnit;
        public UT_PULSE PulseOutputTechUnit
        {
            get
            {
                if (_pulseOutputTechUnit == null)
                {
                    _pulseOutputTechUnit = new UT_PULSE();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _pulseOutputTechUnit);
                }
                return _pulseOutputTechUnit;
            }

        }
        
        private PROTOCOL_V _protocolVersion;
        public PROTOCOL_V ProtocolVersion
        {
            get
            {
                if (_protocolVersion == null)
                {
                    _protocolVersion = new PROTOCOL_V();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _protocolVersion);
                }
                return _protocolVersion;
            }

        }
        
        private PROTOCOL_R _protocolRevision;
        public PROTOCOL_R ProtocolRevision
        {
            get
            {
                if (_protocolRevision == null)
                {
                    _protocolVersion = new PROTOCOL_V();
                    AddVarToBundle(ref _eepParamsPageBundleCmd, _protocolRevision);
                }
                return _protocolRevision;
            }

        }

        #endregion // EEPROM parameters

        private GSMinstalled _gsm_Installed;
        public GSMinstalled GSM_Installed
        {
            get
            {
                if (_gsm_Installed == null)
                {
                    _gsm_Installed = new GSMinstalled();
                }
                return _gsm_Installed;
            }
        }

        #region Custom Info

        private CUSTIMIZED_CONV_ID _customConvID;
        public CUSTIMIZED_CONV_ID CustomConvID
        {
            get
            {
                if (_customConvID == null)
                {
                    _customConvID = new CUSTIMIZED_CONV_ID();
                    AddVarToBundle(ref _eepCustomizedDeviceInfoCmd, _customConvID);
                }
                return _customConvID;
            }
        }

        private CUSTIMIZED_SENSOR_ID _customSensorID;
        public CUSTIMIZED_SENSOR_ID CustomSensorID
        {
            get
            {
                if (_customSensorID == null)
                {
                    _customSensorID = new CUSTIMIZED_SENSOR_ID();
                    AddVarToBundle(ref _eepCustomizedDeviceInfoCmd, _customSensorID);
                }
                return _customSensorID;
            }
        }

        private CUSTOMIZED_SENSOR_MODEL _customSensorModel;
        public CUSTOMIZED_SENSOR_MODEL CustomSensorModel
        {
            get
            {
                if (_customSensorModel == null)
                {
                    _customSensorModel = new CUSTOMIZED_SENSOR_MODEL();
                    AddVarToBundle(ref _eepCustomizedDeviceInfoCmd, _customSensorModel);
                }
                return _customSensorModel;
            }
        }
        
        private CUSTOMIZED_DEV_NAME _customDeviceName;
        public CUSTOMIZED_DEV_NAME CustomDeviceName
        {
            get
            {
                if (_customDeviceName == null)
                {
                    _customDeviceName = new CUSTOMIZED_DEV_NAME();
                    AddVarToBundle(ref _eepCustomizedDeviceInfoCmd, _customDeviceName);
                }
                return _customDeviceName;
            }
        }
        
        private CUSTOMIZED_INFO_ENABLE _customInfoEnable;
        public CUSTOMIZED_INFO_ENABLE CustomInfoEnable
        {
            get
            {
                if (_customInfoEnable == null)
                {
                    _customInfoEnable = new CUSTOMIZED_INFO_ENABLE();
                    AddVarToBundle(ref _eepCustomizedDeviceInfoCmd, _customInfoEnable);
                }
                return _customInfoEnable;
            }
        }
        #endregion

        #region EEPROM info

        private SENSOR_MODEL _sensorModel;
        public SENSOR_MODEL SensorModel
        {
            get
            {
                if (_sensorModel == null)
                {
                    _sensorModel = new SENSOR_MODEL();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _sensorModel);
                }

                if (_sensorModel.Value != "")
                {
                    try
                    { 
                        _sensorModel.Value = CustomDictionary.Instance.SensorModel(_sensorModel.Value);
                    }
                    catch {
                        return _sensorModel;
                    }
                }
                return _sensorModel;
            }
        }

        private OTHER_FEAT _otherFeat;
        public OTHER_FEAT OtherFeatures
        {
            get
            {
                if (_otherFeat == null)
                {
                    _otherFeat = new OTHER_FEAT();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _otherFeat);

                }
                return _otherFeat;
            }
        }

        private CONV_ID _convId;
        public CONV_ID ConverterId
        {
            get
            {
                if (_convId == null)
                {
                    _convId = new CONV_ID();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _convId);
                }
                return _convId;
            }
        }

        private SENSOR_ID _sensorId;
        public SENSOR_ID SensorId
        {
            get
            {
                if (_sensorId == null)
                {
                    _sensorId = new SENSOR_ID();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _sensorId);
                }
                return _sensorId;
            }
        }


        private CONV_SN _conSN;
        public CONV_SN ConverterSerialNumber
        {
            get
            {
                if (_conSN == null)
                {
                    _conSN = new CONV_SN();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _conSN);
                }
                return _conSN;
            }
        }

        private CAL_DATE _calibrationDate;
        public CAL_DATE CalibrationDate
        {
            get
            {
                if (_calibrationDate == null)
                {
                    _calibrationDate = new CAL_DATE();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _calibrationDate);
                }
                return _calibrationDate;
            }
        }

        private DEV_NAME _deviceName;
        public DEV_NAME DeviceName
        {
            get
            {
                if (_deviceName == null)
                {
                    _deviceName = new DEV_NAME();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _deviceName);
                }

                if(_deviceName.Value != "")
                { 
                    try
                    { 
                        _deviceName.Value = CustomDictionary.Instance.ConverterModel(_deviceName.Value);
                    }
                    catch
                    {
                        return _deviceName;
                    }
                }

                return _deviceName;
            }
        }

        private MANUFACTURER _manufacturer;
        public MANUFACTURER Manufacturer
        {
            get
            {
                if (_manufacturer == null)
                {
                    _manufacturer = new MANUFACTURER();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _manufacturer);
                }
                return _manufacturer;
            }
        }

        private PRODUCT_VARIANT _productVariant;
        public PRODUCT_VARIANT ProductVariant
        {
            get
            {
                if (_productVariant == null)
                {
                    _productVariant = new PRODUCT_VARIANT();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _productVariant);
                }
                return _productVariant;
            }
        }

        private PASSWORD _password;
        public PASSWORD Password
        {
            get
            {
                if (_password == null)
                {
                    _password = new PASSWORD();
                    AddVarToBundle(ref _eepInfoPageBundleCmd, _password);
                }
                return _password;
            }
        }

        #endregion // EEPROM info

        #region EEPROM register

        private BASE_SEC _totalTimeInLowPower;
        public BASE_SEC TotalTimeInLowPower
        {
            get
            {
                if (_totalTimeInLowPower == null)
                {
                    _totalTimeInLowPower = new BASE_SEC();
                    AddVarToBundle(ref _eepRegisterPageBundleCmd, _totalTimeInLowPower);
                }
                return _totalTimeInLowPower;
            }
        }

        private MCOUNT _totalMeasuresCount;
        public MCOUNT TotalMeasuresCount
        {
            get
            {
                if (_totalMeasuresCount == null)
                {
                    _totalMeasuresCount = new MCOUNT();
                    AddVarToBundle(ref _eepRegisterPageBundleCmd, _totalMeasuresCount);
                }
                return _totalMeasuresCount;
            }
        }

        private uAh_TOT _totaluAh;
        public uAh_TOT TotaluAh
        {
            get
            {
                if (_totaluAh == null)
                {
                    _totaluAh = new uAh_TOT();
                    AddVarToBundle(ref _eepRegisterPageBundleCmd, _totaluAh);
                }
                return _totaluAh;
            }
        }

        private uAh_LEFT _leftuAh;
        public uAh_LEFT LeftuAh
        {
            get
            {
                if (_leftuAh == null)
                {
                    _leftuAh = new uAh_LEFT();
                    AddVarToBundle(ref _eepRegisterPageBundleCmd, _leftuAh);
                }
                return _leftuAh;
            }
        }

        private AWAKE_SEC _totalTimeAwake;
        public AWAKE_SEC TotalTimeAwake
        {
            get
            {
                if (_totalTimeAwake == null)
                {
                    _totalTimeAwake = new AWAKE_SEC();
                    AddVarToBundle(ref _eepRegisterPageBundleCmd, _totalTimeAwake);
                }
                return _totalTimeAwake;
            }
        }

        #endregion

        #region EEPROM calibration

        private KA _kaRatio;
        public KA KaRatio
        {
            get
            {
                if (_kaRatio == null)
                {
                    _kaRatio = new KA();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _kaRatio);
                }
                return _kaRatio;
            }
        }

        private DIAMETER _sensorDiameter;
        public DIAMETER SensorDiameter
        {
            get
            {
                if (_sensorDiameter == null)
                {
                    _sensorDiameter = new DIAMETER();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _sensorDiameter);
                }
                return _sensorDiameter;
            }
        }

        private SENSOR_OFFSET _sensorOffset;
        public SENSOR_OFFSET SensorOffset
        {
            get
            {
                if (_sensorOffset == null)
                {
                    _sensorOffset = new SENSOR_OFFSET();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _sensorOffset);
                }
                return _sensorOffset;
            }
        }

        private EXC_PAUSE _excitationPause;
        public EXC_PAUSE ExcitationPause
        {
            get
            {
                if (_excitationPause == null)
                {
                    _excitationPause = new EXC_PAUSE();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _excitationPause);
                }
                return _excitationPause;
            }
        }

        private KALIGN _alignmentCalibrationFactor;
        public KALIGN AlignmentCalibrationFactor
        {
            get
            {
                if (_alignmentCalibrationFactor == null)
                {
                    _alignmentCalibrationFactor = new KALIGN();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _alignmentCalibrationFactor);
                }
                return _alignmentCalibrationFactor;
            }
        }

        private OFFALIGN _alignmentOffset;
        public OFFALIGN AlignmentOffset
        {
            get
            {
                if (_alignmentOffset == null)
                {
                    _alignmentOffset = new OFFALIGN();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _alignmentOffset);
                }
                return _alignmentOffset;
            }
        }

        private CUTOFF _cutoff;
        public CUTOFF Cutoff
        {
            get
            {
                if (_cutoff == null)
                {
                    _cutoff = new CUTOFF();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _cutoff);
                }
                return _cutoff;
            }
        }

        private MEAS_FREQ _lowPowerMeasurePeriod;
        public MEAS_FREQ LowPowerMeasurePeriod
        {
            get
            {
                if (_lowPowerMeasurePeriod == null)
                {
                    _lowPowerMeasurePeriod = new MEAS_FREQ();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _lowPowerMeasurePeriod);
                }
                return _lowPowerMeasurePeriod;
            }
        }

        private DAMPING _damping;
        public DAMPING Damping
        {
            get
            {
                if (_damping == null)
                {
                    _damping = new DAMPING();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _damping);
                }
                return _damping;
            }
        }

        private UT_FLOW _flowrateTechUnit;
        public UT_FLOW FlowrateTechUnit
        {
            get
            {
                if (_flowrateTechUnit == null)
                {
                    _flowrateTechUnit = new UT_FLOW();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _flowrateTechUnit);
                }
                return _flowrateTechUnit;
            }

        }

        private TB_FLOW _flowrateTimeBase;
        public TB_FLOW FlowrateTimeBase
        {
            get
            {
                if (_flowrateTimeBase == null)
                {
                    _flowrateTimeBase = new TB_FLOW();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _flowrateTimeBase);
                }
                return _flowrateTimeBase;
            }

        }

        private UT_ACC _accumulatorsTechUnit;
        public UT_ACC AccumulatorsTechUnit
        {
            get
            {
                if (_accumulatorsTechUnit == null)
                {
                    _accumulatorsTechUnit = new UT_ACC();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _accumulatorsTechUnit);
                }
                return _accumulatorsTechUnit;
            }

        }

        private EPIPE_TH _emptyPipeThreshold;
        public EPIPE_TH EmptyPipeThreshold
        {
            get
            {
                if (_emptyPipeThreshold == null)
                {
                    _emptyPipeThreshold = new EPIPE_TH();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _emptyPipeThreshold);
                }
                return _emptyPipeThreshold;
            }
        }

        private EPIPE _emptyPipeCfg;
        public EPIPE EmptyPipeCfg
        {
            get
            {
                if (_emptyPipeCfg == null)
                {
                    _emptyPipeCfg = new EPIPE();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _emptyPipeCfg);
                }
                return _emptyPipeCfg;
            }
        }

        private NDEC_INST _flowrateNdigit;
        public NDEC_INST FlowrateNdigit
        {
            get
            {
                if (_flowrateNdigit == null)
                {
                    _flowrateNdigit = new NDEC_INST();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _flowrateNdigit);
                }
                return _flowrateNdigit;
            }
        }

        private NDEC_ACC _accumulatorNdigit;
        public NDEC_ACC AccumulatorNdigit
        {
            get
            {
                if (_accumulatorNdigit == null)
                {
                    _accumulatorNdigit = new NDEC_ACC();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _accumulatorNdigit);
                }
                return _accumulatorNdigit;
            }
        }

        private FS_MS _flowrateFullscale;
        public FS_MS FlowrateFullscale
        {
            get
            {
                if (_flowrateFullscale == null)
                {
                    _flowrateFullscale = new FS_MS();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _flowrateFullscale);
                }
                return _flowrateFullscale;
            }
        }

        private BYPASS _bypass;
        public BYPASS Bypass
        {
            get
            {
                if (_bypass == null)
                {
                    _bypass = new BYPASS();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _bypass);
                }
                return _bypass;
            }
        }

        private PEAK_CUT _peakcut;
        public PEAK_CUT Peakcut
        {
            get
            {
                if (_peakcut == null)
                {
                    _peakcut = new PEAK_CUT();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _peakcut);
                }
                return _peakcut;
            }
        }

        private BYPASS_COUNT _bypassCount;
        public BYPASS_COUNT BypassCount
        {
            get
            {
                if (_bypassCount == null)
                {
                    _bypassCount = new BYPASS_COUNT();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _bypassCount);
                }
                return _bypassCount;
            }
        }

        private PEAKCUT_COUNT _peakcutCount;
        public PEAKCUT_COUNT PeakcutCount
        {
            get
            {
                if (_peakcutCount == null)
                {
                    _peakcutCount = new PEAKCUT_COUNT();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _peakcutCount);
                }
                return _peakcutCount;
            }
        }

        private OFFSET_REG _offsetReg;
        public OFFSET_REG OffsetReg
        {
            get
            {
                if (_offsetReg == null)
                {
                    _offsetReg = new OFFSET_REG();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _offsetReg);
                }
                return _offsetReg;
            }
        }

        private TMEAS_SD24 _sD24SamplingIndex;
        public TMEAS_SD24 SD24SamplingIndex
        {
            get
            {
                if (_sD24SamplingIndex == null)
                {
                    _sD24SamplingIndex = new TMEAS_SD24();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _sD24SamplingIndex);
                }
                return _sD24SamplingIndex;
            }
        }

        private EPIPE_FREQ _emptyPipeFreq;
        public EPIPE_FREQ EmptyPipeFreq
        {
            get
            {
                if (_emptyPipeFreq == null)
                {
                    _emptyPipeFreq = new EPIPE_FREQ();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _emptyPipeFreq);
                }
                return _emptyPipeFreq;
            }
        }

        private EPIPE_RELEASE _emptyPipeRelease;
        public EPIPE_RELEASE EmptyPipeRelease
        {
            get
            {
                if (_emptyPipeRelease == null)
                {
                    _emptyPipeRelease = new EPIPE_RELEASE();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _emptyPipeRelease);
                }
                return _emptyPipeRelease;
            }
        }

        private MEAS_WD_EN _waterDetectMeasThreshold;
        public MEAS_WD_EN WaterDetectMeasThreshold
        {
            get
            {
                if (_waterDetectMeasThreshold == null)
                {
                    _waterDetectMeasThreshold = new MEAS_WD_EN();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _waterDetectMeasThreshold);
                }
                return _waterDetectMeasThreshold;
            }
        }
        
        private MEAS_WD_EN _waterDetectMeasEnable;
        public MEAS_WD_EN WaterDetectMeasEnable
        {
            get
            {
                if (_waterDetectMeasEnable == null)
                {
                    _waterDetectMeasEnable = new MEAS_WD_EN();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _waterDetectMeasEnable);
                }
                return _waterDetectMeasEnable;
            }
        }

        private ADC_GAIN _adcGain;
        public ADC_GAIN AdcGain
        {
            get
            {
                if (_adcGain == null)
                {
                    _adcGain = new ADC_GAIN();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _adcGain);
                }
                return _adcGain;
            }
        }

        private INSERTION _sensorIsInsertion;
        public INSERTION SensorIsInsertion
        {
            get
            {
                if (_sensorIsInsertion == null)
                {
                    _sensorIsInsertion = new INSERTION();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _sensorIsInsertion);
                }
                return _sensorIsInsertion;
            }
        }

        private MEAS_AWAKE_MS _awakeMeasurePeriod;
        public MEAS_AWAKE_MS AwakeMeasurePeriod
        {
            get
            {
                if (_awakeMeasurePeriod == null)
                {
                    _awakeMeasurePeriod = new MEAS_AWAKE_MS();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _awakeMeasurePeriod);
                }
                return _awakeMeasurePeriod;
            }
        }

        private EPIPE_FREQ_FAST _emptyPipeFreqFast;
        public EPIPE_FREQ_FAST EmptyPipeFreqFast
        {
            get
            {
                if (_emptyPipeFreqFast == null)
                {
                    _emptyPipeFreqFast = new EPIPE_FREQ_FAST();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _emptyPipeFreqFast);
                }
                return _emptyPipeFreqFast;
            }
        }

        private INPUT_STAGE_STAB _inputStageStabTime;
        public INPUT_STAGE_STAB InputStageStabTime
        {
            get
            {
                if (_inputStageStabTime == null)
                {
                    _inputStageStabTime = new INPUT_STAGE_STAB();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _inputStageStabTime);
                }
                return _inputStageStabTime;
            }
        }

        private DAMPING_SLOW _dampingSlow;
        public DAMPING_SLOW DampingSlow
        {
            get
            {
                if (_dampingSlow == null)
                {
                    _dampingSlow = new DAMPING_SLOW();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _dampingSlow);
                }
                return _dampingSlow;
            }
        }

        private LINE_FREQ _mainsLineFrequency;
        public LINE_FREQ MainsLineFrequency
        {
            get
            {
                if (_mainsLineFrequency == null)
                {
                    _mainsLineFrequency = new LINE_FREQ();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _mainsLineFrequency);
                }
                return _mainsLineFrequency;
            }

        }

        private INTERP_sA_LO _insertion_sA_LO;
        public INTERP_sA_LO Insertion_sA_LO
        {
            get
            {
                if (_insertion_sA_LO == null)
                {
                    _insertion_sA_LO = new INTERP_sA_LO();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_sA_LO);
                }
                return _insertion_sA_LO;
            }
        }

        private INTERP_sB_LO _insertion_sB_LO;
        public INTERP_sB_LO Insertion_sB_LO
        {
            get
            {
                if (_insertion_sB_LO == null)
                {
                    _insertion_sB_LO = new INTERP_sB_LO();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_sB_LO);
                }
                return _insertion_sB_LO;
            }
        }

        private INTERP_sC_LO _insertion_sC_LO;
        public INTERP_sC_LO Insertion_sC_LO
        {
            get
            {
                if (_insertion_sC_LO == null)
                {
                    _insertion_sC_LO = new INTERP_sC_LO();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_sC_LO);
                }
                return _insertion_sC_LO;
            }
        }

        private INTERP_sD_LO _insertion_sD_LO;
        public INTERP_sD_LO Insertion_sD_LO
        {
            get
            {
                if (_insertion_sD_LO == null)
                {
                    _insertion_sD_LO = new INTERP_sD_LO();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_sD_LO);
                }
                return _insertion_sD_LO;
            }
        }

        private INTERP_sA_HI _insertion_sA_HI;
        public INTERP_sA_HI Insertion_sA_HI
        {
            get
            {
                if (_insertion_sA_HI == null)
                {
                    _insertion_sA_HI = new INTERP_sA_HI();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_sA_HI);
                }
                return _insertion_sA_HI;
            }
        }

        private INTERP_sB_HI _insertion_sB_HI;
        public INTERP_sB_HI Insertion_sB_HI
        {
            get
            {
                if (_insertion_sB_HI == null)
                {
                    _insertion_sB_HI = new INTERP_sB_HI();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_sB_HI);
                }
                return _insertion_sB_HI;
            }
        }

        private INTERP_sC_HI _insertion_sC_HI;
        public INTERP_sC_HI Insertion_sC_HI
        {
            get
            {
                if (_insertion_sC_HI == null)
                {
                    _insertion_sC_HI = new INTERP_sC_HI();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_sC_HI);
                }
                return _insertion_sC_HI;
            }
        }

        private INTERP_sD_HI _insertion_sD_HI;
        public INTERP_sD_HI Insertion_sD_HI
        {
            get
            {
                if (_insertion_sD_HI == null)
                {
                    _insertion_sD_HI = new INTERP_sD_HI();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_sD_HI);
                }
                return _insertion_sD_HI;
            }
        }

        private INTERP_TH _insertion_Interp_Th;
        public INTERP_TH Insertion_Interp_Th
        {
            get
            {
                if (_insertion_Interp_Th == null)
                {
                    _insertion_Interp_Th = new INTERP_TH();
                    AddVarToBundle(ref _eepCalibPageBundleCmd, _insertion_Interp_Th);
                }
                return _insertion_Interp_Th;
            }
        }

        #endregion // EEPROM calibration

        #region Bluetooth/Modbus

        private ENABLE_BOARD_BT_RS485 _enableBoardBtRs485;
        public ENABLE_BOARD_BT_RS485 EnableBoardBtRs485
        {
            get
            {
                if (_enableBoardBtRs485 == null)
                {
                    _enableBoardBtRs485 = new ENABLE_BOARD_BT_RS485();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _enableBoardBtRs485);
                }
                return _enableBoardBtRs485;
            }
        }
        
        private BT_ON_INTERVAL _bluetoothOnInterval;
        public BT_ON_INTERVAL BluetoothOnInterval
        {
            get
            {
                if (_bluetoothOnInterval == null)
                {
                    _bluetoothOnInterval = new BT_ON_INTERVAL();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _bluetoothOnInterval);
                }
                return _bluetoothOnInterval;
            }
        }

        private RS485_ON_INTERVAL _rs485OnInterval;
        public RS485_ON_INTERVAL Rs485_OnInterval
        {
            get
            {
                if (_rs485OnInterval == null)
                {
                    _rs485OnInterval = new RS485_ON_INTERVAL();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485OnInterval);
                }
                return _rs485OnInterval;
            }
        }
        
        private RS485_BAUDRATE _rs485Baudrate;
        public RS485_BAUDRATE Rs485_Baudrate
        {
            get
            {
                if (_rs485Baudrate == null)
                {
                    _rs485Baudrate = new RS485_BAUDRATE();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485Baudrate);
                }
                return _rs485Baudrate;
            }
        }
        
        private RS485_DATA_NUM_BIT _rs485DataLenght;
        public RS485_DATA_NUM_BIT Rs485_DataLenght
        {
            get
            {
                if (_rs485DataLenght == null)
                {
                    _rs485DataLenght = new RS485_DATA_NUM_BIT();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485DataLenght);
                }
                return _rs485DataLenght;
            }
        }

        private RS485_PARITY _rs485Parity;
        public RS485_PARITY Rs485_Parity
        {
            get
            {
                if (_rs485Parity == null)
                {
                    _rs485Parity = new RS485_PARITY();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485Parity);
                }
                return _rs485Parity;
            }
        }

        private RS485_STOP_BITS _rs485StopBits;
        public RS485_STOP_BITS Rs485_StopBits
        {
            get
            {
                if (_rs485StopBits == null)
                {
                    _rs485StopBits = new RS485_STOP_BITS();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485StopBits);
                }
                return _rs485StopBits;
            }
        }
        
        private RS485_MODBUS_MODE _rs485ModbusMode;
        public RS485_MODBUS_MODE Rs485_ModbusMode
        {
            get
            {
                if (_rs485ModbusMode == null)
                {
                    _rs485ModbusMode = new RS485_MODBUS_MODE();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485ModbusMode);
                }
                return _rs485ModbusMode;
            }
        }
        
        private RS485_MODBUS_ADDR _rs485ModbusAddress;
        public RS485_MODBUS_ADDR Rs485_ModbusAddress
        {
            get
            {
                if (_rs485ModbusAddress == null)
                {
                    _rs485ModbusAddress = new RS485_MODBUS_ADDR();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485ModbusAddress);
                }
                return _rs485ModbusAddress;
            }
        }

        
        private RS485_MODUBUS_BYTE_ORDER _rs485ModbusByteOrder;
        public RS485_MODUBUS_BYTE_ORDER Rs485_ModbusByteOrder
        {
            get
            {
                if (_rs485ModbusByteOrder == null)
                {
                    _rs485ModbusByteOrder = new RS485_MODUBUS_BYTE_ORDER();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485ModbusByteOrder);
                }
                return _rs485ModbusByteOrder;
            }
        }
        
        private RS_485_ON_DURATION _rs485OnDuration;
        public RS_485_ON_DURATION Rs485_OnDuration
        {
            get
            {
                if (_rs485OnDuration == null)
                {
                    _rs485OnDuration = new RS_485_ON_DURATION();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485OnDuration);
                }
                return _rs485OnDuration;
            }
        }
        
        private BT_ON_DURATION _bluetoothOnDuration;
        public BT_ON_DURATION Bluetooth_OnDuration
        {
            get
            {
                if (_bluetoothOnDuration == null)
                {
                    _bluetoothOnDuration = new BT_ON_DURATION();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _bluetoothOnDuration);
                }
                return _bluetoothOnDuration;
            }
        }
        
        private RS485_MODUBUS_MAP _rs485ModbusMap;
        public RS485_MODUBUS_MAP Rs485ModbusMap
        {
            get
            {
                if (_rs485ModbusMap == null)
                {
                    _rs485ModbusMap = new RS485_MODUBUS_MAP();
                    AddVarToBundle(ref _eepBluetoothModbusDeviceInfoCmd, _rs485ModbusMap);
                }
                return _rs485ModbusMap;
            }
        }
        #endregion
                  
        #region GSM Parameters

        #region GSM Variables
        private Mobile_num_1 _gsm_Mobile_num_1;
        public Mobile_num_1 GSM_Mobile_num_1
        {
            get
            {
                if (_gsm_Mobile_num_1 == null)
                {
                    _gsm_Mobile_num_1 = new Mobile_num_1();                    
                }
                return _gsm_Mobile_num_1;
            }
        }

        private Mobile_num_2 _gsm_Mobile_num_2;
        public Mobile_num_2 GSM_Mobile_num_2
        {
            get
            {
                if (_gsm_Mobile_num_2 == null)
                {
                    _gsm_Mobile_num_2 = new Mobile_num_2();
                }
                return _gsm_Mobile_num_2;
            }
        }

        private Mobile_num_3 _gsm_Mobile_num_3;
        public Mobile_num_3 GSM_Mobile_num_3
        {
            get
            {
                if (_gsm_Mobile_num_3 == null)
                {
                    _gsm_Mobile_num_3 = new Mobile_num_3();
                }
                return _gsm_Mobile_num_3;
            }
        }

        private Mobile_num_4 _gsm_Mobile_num_4;
        public Mobile_num_4 GSM_Mobile_num_4
        {
            get
            {
                if (_gsm_Mobile_num_4 == null)
                {
                    _gsm_Mobile_num_4 = new Mobile_num_4();
                }
                return _gsm_Mobile_num_4;
            }
        }

        private Mobile_num_5 _gsm_Mobile_num_5;
        public Mobile_num_5 GSM_Mobile_num_5
        {
            get
            {
                if (_gsm_Mobile_num_5 == null)
                {
                    _gsm_Mobile_num_5 = new Mobile_num_5();
                }
                return _gsm_Mobile_num_5;
            }
        }

        private SMS_Service_center _gsm_SMS_Service_center;
        public SMS_Service_center GSM_SMS_Service_center
        {
            get
            {
                if (_gsm_SMS_Service_center == null)
                {
                    _gsm_SMS_Service_center = new SMS_Service_center();
                }
                return _gsm_SMS_Service_center;
            }
        }

        private Mail_address_1 _gsm_Mail_address_1;
        public Mail_address_1 GSM_Mail_address_1
        {
            get
            {
                if (_gsm_Mail_address_1 == null)
                {
                    _gsm_Mail_address_1 = new Mail_address_1();
                }
                return _gsm_Mail_address_1;
            }
        }

        private Mail_address_2 _gsm_Mail_address_2;
        public Mail_address_2 GSM_Mail_address_2
        {
            get
            {
                if (_gsm_Mail_address_2 == null)
                {
                    _gsm_Mail_address_2 = new Mail_address_2();
                }
                return _gsm_Mail_address_2;
            }
        }

        private Mail_address_3 _gsm_Mail_address_3;
        public Mail_address_3 GSM_Mail_address_3
        {
            get
            {
                if (_gsm_Mail_address_3 == null)
                {
                    _gsm_Mail_address_3 = new Mail_address_3();
                }
                return _gsm_Mail_address_3;
            }
        }

        private Mail_address_4 _gsm_Mail_address_4;
        public Mail_address_4 GSM_Mail_address_4
        {
            get
            {
                if (_gsm_Mail_address_4 == null)
                {
                    _gsm_Mail_address_4 = new Mail_address_4();
                }
                return _gsm_Mail_address_4;
            }
        }

        private Mail_address_5 _gsm_Mail_address_5;
        public Mail_address_5 GSM_Mail_address_5
        {
            get
            {
                if (_gsm_Mail_address_5 == null)
                {
                    _gsm_Mail_address_5 = new Mail_address_5();
                }
                return _gsm_Mail_address_5;
            }
        }

        private Mail_address_sender _gsm_Mail_address_sender;
        public Mail_address_sender GSM_Mail_address_sender
        {
            get
            {
                if (_gsm_Mail_address_sender == null)
                {
                    _gsm_Mail_address_sender = new Mail_address_sender();
                }
                return _gsm_Mail_address_sender;
            }
        }

        private SMTP_server _gsm_SMTP_server;
        public SMTP_server GSM_SMTP_server
        {
            get
            {
                if (_gsm_SMTP_server == null)
                {
                    _gsm_SMTP_server = new SMTP_server();
                }
                return _gsm_SMTP_server;
            }
        }

        private SMTP_Port _gsm_SMTP_Port;
        public SMTP_Port GSM_SMTP_Port
        {
            get
            {
                if (_gsm_SMTP_Port == null)
                {
                    _gsm_SMTP_Port = new SMTP_Port();
                }
                return _gsm_SMTP_Port;
            }
        }

        private SMTP_username _gsm_SMTP_username;
        public SMTP_username GSM_SMTP_username
        {
            get
            {
                if (_gsm_SMTP_username == null)
                {
                    _gsm_SMTP_username = new SMTP_username();
                }
                return _gsm_SMTP_username;
            }
        }

        private Smtp_password _gsm_SMTP_password;
        public Smtp_password GSM_SMTP_password
        {
            get
            {
                if (_gsm_SMTP_password == null)
                {
                    _gsm_SMTP_password = new Smtp_password();
                }
                return _gsm_SMTP_password;
            }
        }

        private APN _gsm_APN_Name;
        public APN GSM_APN_Name
        {
            get
            {
                if (_gsm_APN_Name == null)
                {
                    _gsm_APN_Name = new APN();
                }
                return _gsm_APN_Name;
            }
        }

        private APN_username _gsm_APN_username;
        public APN_username GSM_APN_username
        {
            get
            {
                if (_gsm_APN_username == null)
                {
                    _gsm_APN_username = new APN_username();
                }
                return _gsm_APN_username;
            }
        }

        private APN_password _gsm_APN_password;
        public APN_password GSM_APN_password
        {
            get
            {
                if (_gsm_APN_password == null)
                {
                    _gsm_APN_password = new APN_password();
                }
                return _gsm_APN_password;
            }
        }

        private SIM_PIN _gsm_SIM_PIN;
        public SIM_PIN GSM_SIM_PIN
        {
            get
            {
                if (_gsm_SIM_PIN == null)
                {
                    _gsm_SIM_PIN = new SIM_PIN();
                }
                return _gsm_SIM_PIN;
            }
        }

        private En_data_roaming _gsm_En_data_roaming;
        public En_data_roaming GSM_En_data_roaming
        {
            get
            {
                if (_gsm_En_data_roaming == null)
                {
                    _gsm_En_data_roaming = new En_data_roaming();
                }
                return _gsm_En_data_roaming;
            }
        }

        private Hours_sms _gsm_Hours_sms;
        public Hours_sms GSM_Hours_sms
        {
            get
            {
                if (_gsm_Hours_sms == null)
                {
                    _gsm_Hours_sms = new Hours_sms();
                }
                return _gsm_Hours_sms;
            }
        }

        private Days_of_Week_sms _gsm_Days_of_Week_sms;
        public Days_of_Week_sms GSM_Days_of_Week_sms
        {
            get
            {
                if (_gsm_Days_of_Week_sms == null)
                {
                    _gsm_Days_of_Week_sms = new Days_of_Week_sms();
                }
                return _gsm_Days_of_Week_sms;
            }
        }

        private Days_of_Month_sms _gsm_Days_of_Month_sms;
        public Days_of_Month_sms GSM_Days_of_Month_sms
        {
            get
            {
                if (_gsm_Days_of_Month_sms == null)
                {
                    _gsm_Days_of_Month_sms = new Days_of_Month_sms();
                }
                return _gsm_Days_of_Month_sms;
            }
        }

        private Hours_email _gsm_Hours_email;
        public Hours_email GSM_Hours_email
        {
            get
            {
                if (_gsm_Hours_email == null)
                {
                    _gsm_Hours_email = new Hours_email();
                }
                return _gsm_Hours_email;
            }
        }

        private Days_of_Week_email _gsm_Days_of_Week_email;
        public Days_of_Week_email GSM_Days_of_Week_email
        {
            get
            {
                if (_gsm_Days_of_Week_email == null)
                {
                    _gsm_Days_of_Week_email = new Days_of_Week_email();
                }
                return _gsm_Days_of_Week_email;
            }
        }

        private Days_of_Month_email _gsm_Days_of_Month_email;
        public Days_of_Month_email GSM_Days_of_Month_email
        {
            get
            {
                if (_gsm_Days_of_Month_email == null)
                {
                    _gsm_Days_of_Month_email = new Days_of_Month_email();
                }
                return _gsm_Days_of_Month_email;
            }
        }

        private Hours_email_att _gsm_Hours_email_att;
        public Hours_email_att GSM_Hours_email_att
        {
            get
            {
                if (_gsm_Hours_email_att == null)
                {
                    _gsm_Hours_email_att = new Hours_email_att();
                }
                return _gsm_Hours_email_att;
            }
        }

        private Days_of_Week_email_att _gsm_Days_of_Week_email_att;
        public Days_of_Week_email_att GSM_Days_of_Week_email_att
        {
            get
            {
                if (_gsm_Days_of_Week_email_att == null)
                {
                    _gsm_Days_of_Week_email_att = new Days_of_Week_email_att();
                }
                return _gsm_Days_of_Week_email_att;
            }
        }

        private Days_of_Month_email_att _gsm_Days_of_Month_email_att;
        public Days_of_Month_email_att GSM_Days_of_Month_email_att
        {
            get
            {
                if (_gsm_Days_of_Month_email_att == null)
                {
                    _gsm_Days_of_Month_email_att = new Days_of_Month_email_att();
                }
                return _gsm_Days_of_Month_email_att;
            }
        }

        private Hours_Data _gsm_Hours_Data;
        public Hours_Data GSM_Hours_Data
        {
            get
            {
                if (_gsm_Hours_Data == null)
                {
                    _gsm_Hours_Data = new Hours_Data();
                }
                return _gsm_Hours_Data;
            }
        }

        private Days_of_Week_Data _gsm_Days_of_Week_Data;
        public Days_of_Week_Data GSM_Days_of_Week_Data
        {
            get
            {
                if (_gsm_Days_of_Week_Data == null)
                {
                    _gsm_Days_of_Week_Data = new Days_of_Week_Data();
                }
                return _gsm_Days_of_Week_Data;
            }
        }

        private Days_of_Month_Data _gsm_Days_of_Month_Data;
        public Days_of_Month_Data GSM_Days_of_Month_Data
        {
            get
            {
                if (_gsm_Days_of_Month_Data == null)
                {
                    _gsm_Days_of_Month_Data = new Days_of_Month_Data();
                }
                return _gsm_Days_of_Month_Data;
            }
        }

        private FTP_server _gsm_FTP_server;
        public FTP_server GSM_FTP_server
        {
            get
            {
                if (_gsm_FTP_server == null)
                {
                    _gsm_FTP_server = new FTP_server();
                }
                return _gsm_FTP_server;
            }
        }

        private FTP_username _gsm_FTP_username;
        public FTP_username GSM_FTP_username
        {
            get
            {
                if (_gsm_FTP_username == null)
                {
                    _gsm_FTP_username = new FTP_username();
                }
                return _gsm_FTP_username;
            }
        }

        private FTP_password _gsm_FTP_password;
        public FTP_password GSM_FTP_password
        {
            get
            {
                if (_gsm_FTP_password == null)
                {
                    _gsm_FTP_password = new FTP_password();
                }
                return _gsm_FTP_password;
            }
        }

        private FTP_workdir_1 _gsm_FTP_workdir_1;
        public FTP_workdir_1 GSM_FTP_workdir_1
        {
            get
            {
                if (_gsm_FTP_workdir_1 == null)
                {
                    _gsm_FTP_workdir_1 = new FTP_workdir_1();
                }
                return _gsm_FTP_workdir_1;
            }
        }

        private FTP_workdir_2 _gsm_FTP_workdir_2;
        public FTP_workdir_2 GSM_FTP_workdir_2
        {
            get
            {
                if (_gsm_FTP_workdir_2 == null)
                {
                    _gsm_FTP_workdir_2 = new FTP_workdir_2();
                }
                return _gsm_FTP_workdir_2;
            }
        }

        private FTP_workdir_3 _gsm_FTP_workdir_3;
        public FTP_workdir_3 GSM_FTP_workdir_3
        {
            get
            {
                if (_gsm_FTP_workdir_3 == null)
                {
                    _gsm_FTP_workdir_3 = new FTP_workdir_3();
                }
                return _gsm_FTP_workdir_3;
            }
        }

        private Email_send_by_ftp _gsm_Email_send_by_ftp;
        public Email_send_by_ftp GSM_Email_send_by_ftp
        {
            get
            {
                if (_gsm_Email_send_by_ftp == null)
                {
                    _gsm_Email_send_by_ftp = new Email_send_by_ftp();
                }
                return _gsm_Email_send_by_ftp;
            }
        }

        private Sensor_ID _gsm_Sensor_ID;
        public Sensor_ID GSM_Sensor_ID
        {
            get
            {
                if (_gsm_Sensor_ID == null)
                {
                    _gsm_Sensor_ID = new Sensor_ID();
                }
                return _gsm_Sensor_ID;
            }
        }

        private Converter_ID _gsm_Converter_ID;
        public Converter_ID GSM_Converter_ID
        {
            get
            {
                if (_gsm_Converter_ID == null)
                {
                    _gsm_Converter_ID = new Converter_ID();
                }
                return _gsm_Converter_ID;
            }
        }

        private Enable_remote_update _gsm_Enable_remote_update;
        public Enable_remote_update GSM_Enable_remote_update
        {
            get
            {
                if (_gsm_Enable_remote_update == null)
                {
                    _gsm_Enable_remote_update = new Enable_remote_update();
                }
                return _gsm_Enable_remote_update;
            }
        }

        #endregion

        #region GSM Test

        private Operator_Connecteted _gsm_Operator_Connecteted;
        public Operator_Connecteted GSM_Operator_Connecteted
        {
            get
            {
                if (_gsm_Operator_Connecteted == null)
                {
                    _gsm_Operator_Connecteted = new Operator_Connecteted();
                }
                return _gsm_Operator_Connecteted;
            }
        }

        private Signal_strength _gsm_Signal_strength;
        public Signal_strength GSM_Signal_strength
        {
            get
            {
                if (_gsm_Signal_strength == null)
                {
                    _gsm_Signal_strength = new Signal_strength();
                }
                return _gsm_Signal_strength;
            }
        }

        private Operator_name _gsm_Operator_name;
        public Operator_name GSM_Operator_name
        {
            get
            {
                if (_gsm_Operator_name == null)
                {
                    _gsm_Operator_name = new Operator_name();
                }
                return _gsm_Operator_name;
            }
        }

        private Mail_Body _gsm_Mail_Body;
        public Mail_Body GSM_Mail_Body
        {
            get
            {
                if (_gsm_Mail_Body == null)
                {
                    _gsm_Mail_Body = new Mail_Body();
                }
                return _gsm_Mail_Body;
            }
        }

        private Mail_Attachment _gsm_Mail_Attachment;
        public Mail_Attachment GSM_Mail_Attachment
        {
            get
            {
                if (_gsm_Mail_Attachment == null)
                {
                    _gsm_Mail_Attachment = new Mail_Attachment();
                }
                return _gsm_Mail_Attachment;
            }
        }

        private Mail_Send _gsm_Mail_Send;
        public Mail_Send GSM_Mail_Send
        {
            get
            {
                if (_gsm_Mail_Send == null)
                {
                    _gsm_Mail_Send = new Mail_Send();
                }
                return _gsm_Mail_Send;
            }
        }

        private Mail_Send_Attach _gsm_Send_Attach;
        public Mail_Send_Attach GSM_Send_Attach
        {
            get
            {
                if (_gsm_Send_Attach == null)
                {
                    _gsm_Send_Attach = new Mail_Send_Attach();
                }
                return _gsm_Send_Attach;
            }
        }

        private SMS_Body _gsm_SMS_Body;
        public SMS_Body GSM_SMS_Body
        {
            get
            {
                if (_gsm_SMS_Body == null)
                {
                    _gsm_SMS_Body = new SMS_Body();
                }
                return _gsm_SMS_Body;
            }
        }

        private SMS_Send _gsm_SMS_Send;
        public SMS_Send GSM_SMS_Send
        {
            get
            {
                if (_gsm_SMS_Send == null)
                {
                    _gsm_SMS_Send = new SMS_Send();
                }
                return _gsm_SMS_Send;
            }
        }

        private SMS_Sending_Status _gsm_SMS_Sending_Status;
        public SMS_Sending_Status GSM_SMS_Sending_Status
        {
            get
            {
                if (_gsm_SMS_Sending_Status == null)
                {
                    _gsm_SMS_Sending_Status = new SMS_Sending_Status();
                }
                return _gsm_SMS_Sending_Status;
            }
        }

        private Mail_Sending_Status _gsm_Mail_Sending_Status;
        public Mail_Sending_Status GSM_Mail_Sending_Status
        {
            get
            {
                if (_gsm_Mail_Sending_Status == null)
                {
                    _gsm_Mail_Sending_Status = new Mail_Sending_Status();
                }
                return _gsm_Mail_Sending_Status;
            }
        }

        private FTP_Test_Connection _gsm_FTP_Test_Connection;
        public FTP_Test_Connection GSM_FTP_Test_Connection
        {
            get
            {
                if (_gsm_FTP_Test_Connection == null)
                {
                    _gsm_FTP_Test_Connection = new FTP_Test_Connection();
                }
                return _gsm_FTP_Test_Connection;
            }
        }

        private Data_Test_Connection _gsm_Data_Test_Connection;
        public Data_Test_Connection GSM_Data_Test_Connection
        {
            get
            {
                if (_gsm_Data_Test_Connection == null)
                {
                    _gsm_Data_Test_Connection = new Data_Test_Connection();
                }
                return _gsm_Data_Test_Connection;
            }
        }

        private FTP_Test_Status _gsm_FTP_Test_Status;
        public FTP_Test_Status GSM_FTP_Test_Status
        {
            get
            {
                if (_gsm_FTP_Test_Status == null)
                {
                    _gsm_FTP_Test_Status = new FTP_Test_Status();
                }
                return _gsm_FTP_Test_Status;
            }
        }

        private Data_Test_Status _gsm_Data_Test_Status;
        public Data_Test_Status GSM_Data_Test_Status
        {
            get
            {
                if (_gsm_Data_Test_Status == null)
                {
                    _gsm_Data_Test_Status = new Data_Test_Status();
                }
                return _gsm_Data_Test_Status;
            }
        }

        private Sync_Date_Time _gsm_Sync_Date_Time;
        public Sync_Date_Time GSM_Sync_Date_Time
        {
            get
            {
                if (_gsm_Sync_Date_Time == null)
                {
                    _gsm_Sync_Date_Time = new Sync_Date_Time();
                }
                return _gsm_Sync_Date_Time;
            }
        }

        private Get_Date_Time _gsm_Get_Date_Time;
        public Get_Date_Time GSM_Get_Date_Time
        {
            get
            {
                if (_gsm_Get_Date_Time == null)
                {
                    _gsm_Get_Date_Time = new Get_Date_Time();
                }
                return _gsm_Get_Date_Time;
            }
        }

        private Modem_Reset _gsm_Modem_Reset;
        public Modem_Reset GSM_Modem_Reset
        {
            get
            {
                if (_gsm_Modem_Reset == null)
                {
                    _gsm_Modem_Reset = new Modem_Reset();
                }
                return _gsm_Modem_Reset;
            }
        }

        private Modem_shutdown _gsm_Modem_shutdown;
        public Modem_shutdown GSM_Modem_shutdown
        {
            get
            {
                if (_gsm_Modem_shutdown == null)
                {
                    _gsm_Modem_shutdown = new Modem_shutdown();
                }
                return _gsm_Modem_shutdown;
            }
        }
        
        private RESET_EEprom _gsm_Reset_Eeprom;
        public RESET_EEprom GSM_Reset_Eeprom
        {
            get
            {
                if (_gsm_Reset_Eeprom == null)
                {
                    _gsm_Reset_Eeprom = new RESET_EEprom();
                }
                return _gsm_Reset_Eeprom;
            }
        }

        private GSM_Status _gsm_Status;
        public GSM_Status GSM_Status
        {
            get
            {
                if (_gsm_Status == null)
                {
                    _gsm_Status = new GSM_Status();
                }
                return _gsm_Status;
            }
        }

        #endregion

        #endregion

        #region UI_Parameters

        private string _fwRelease;
        public string FwRelease
        {
            get { return _fwRelease; }
            set
            {
                if (value != _fwRelease)
                {
                    _fwRelease = value;
                    OnPropertyChanged("FwRelease");
                }
            }
        }

        private string _flowRateTU_str;
        public string FlowRateTU_str
        {
            get { return _flowRateTU_str; }
            set
            {
                if (value != _flowRateTU_str)
                {
                    _flowRateTU_str = value;
                    OnPropertyChanged("FlowRateTU_str");
                }
            }
        }

        private string _locFlowFullScale_Str;
        public string LocFlowFullScale_Str
        {
            get { return _locFlowFullScale_Str; }
            set
            {
                if (value != _locFlowFullScale_Str)
                {
                    _locFlowFullScale_Str = value;
                    OnPropertyChanged("LocFlowFullScale_Str");
                }
            }
        }

        private string _flowRatePERC_str;
        public string FlowRatePERC_str
        {
            get { return _flowRatePERC_str; }
            set
            {
                if (value != _flowRatePERC_str)
                {
                    _flowRatePERC_str = value;
                    OnPropertyChanged("FlowRatePERC_str");
                }
            }
        }

        private string _flowRateMS_str;
        public string FlowRateMS_str
        {
            get { return _flowRateMS_str; }
            set
            {
                if (value != _flowRateMS_str)
                {
                    _flowRateMS_str = value;
                    OnPropertyChanged("FlowRateMS_str");
                }
            }
        }

        private string _totalPositiveM3_str;
        public string TotalPositiveM3_str
        {
            get { return _totalPositiveM3_str; }
            set
            {
                if (value != _totalPositiveM3_str)
                {
                    _totalPositiveM3_str = value;
                    OnPropertyChanged("TotalPositiveM3_str");
                }
            }
        }

        private string _totalNegativeM3_str;
        public string TotalNegativeM3_str
        {
            get { return _totalNegativeM3_str; }
            set
            {
                if (value != _totalNegativeM3_str)
                {
                    _totalNegativeM3_str = value;
                    OnPropertyChanged("TotalNegativeM3_str");
                }
            }
        }

        private string _partialPositiveM3_str;
        public string PartialPositiveM3_str
        {
            get { return _partialPositiveM3_str; }
            set
            {
                if (value != _partialPositiveM3_str)
                {
                    _partialPositiveM3_str = value;
                    OnPropertyChanged("PartialPositiveM3_str");
                }
            }
        }

        private string _partialNegativeM3_str;
        public string PartialNegativeM3_str
        {
            get { return _partialNegativeM3_str; }
            set
            {
                if (value != _partialNegativeM3_str)
                {
                    _partialNegativeM3_str = value;
                    OnPropertyChanged("PartialNegativeM3_str");
                }
            }
        }

        private DateTime _convDateTime;
        public DateTime ConvDateTime
        {
            get { return _convDateTime; }
            set
            {
                if (value != _convDateTime)
                {
                    _convDateTime = value;
                    OnPropertyChanged("ConvDateTime");
                }
            }
        }

        private string _editCfgMessage;
        public string EditCfgMessage
        {
            get { return _editCfgMessage; }
            set
            {
                if (value != _editCfgMessage)
                {
                    _editCfgMessage = value;
                    OnPropertyChanged("EditCfgMessage");
                }
            }
        }

        #endregion

        #region Logs

        private ObservableCollection<DataLogLine> _rowDatabase;
        public ObservableCollection<DataLogLine> RowDatabase
        {
            get
            {
                if (_rowDatabase == null)
                {
                    _rowDatabase = new ObservableCollection<DataLogLine>();
                }
                return _rowDatabase;
            }
            set {; }
        }

        private Visibility _downloadRingVisibility;
        public Visibility DownloadRingVisibility
        {
            get { return _downloadRingVisibility; }
            set
            {
                if (value != _downloadRingVisibility)
                {
                    _downloadRingVisibility = value;
                    OnPropertyChanged("DownloadRingVisibility");
                }
            }
        }

        /*private BeginInvokeOC<EventLogLine> _events;
        public BeginInvokeOC<EventLogLine> Events
        {
            get
            {
                if (_events == null)
                {
                    _events = new BeginInvokeOC<EventLogLine>(Application.Current.Dispatcher);
                }
                return _events;
            }
        }

        private BeginInvokeOC<DataLogLine> _dataRecords;
        public BeginInvokeOC<DataLogLine> DataRecords
        {
            get
            {
                if (_dataRecords == null)
                {
                    _dataRecords = new BeginInvokeOC<DataLogLine>(Application.Current.Dispatcher);
                }
                return _dataRecords;
            }
        }

        private BeginInvokeOC<BslLogLine> _BsdlRecords;
        public BeginInvokeOC<BslLogLine> BsdlRecords
        {
            get
            {
                if (_BsdlRecords == null)
                {
                    _BsdlRecords = new BeginInvokeOC<BslLogLine>(Application.Current.Dispatcher);
                }
                return _BsdlRecords;
            }
        }*/

        #endregion // logs

        #region methods

        public void WriteEepromVariable(IEEPROMvariable variable)
        {
            WriteEEPROM Cmd = new WriteEEPROM(IrCOMPortManager.Instance.portHandler);
            Cmd.Variable = variable;
            Cmd.CommandCompleted += WriteEepromVariable_Completed;
            IrCOMPortManager.Instance.CommandList.Add(Cmd);
        }

        private void WriteEepromVariable_Completed(object sender, PropertyChangedEventArgs e)
        {
            WriteEEPROM cmd = sender as WriteEEPROM;
            if (cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
            {
                IrCOMPortManager.Instance.ExtCommandCompleted = IrCOMPortManager.CommandState.WaitForNew;
                IrCOMPortManager.Instance.CommandList.Remove(cmd);
            }
        }

        public void ReadEepromVariable(IEEPROMvariable variable)
        {
            ReadEEPROM Cmd = new ReadEEPROM(IrCOMPortManager.Instance.portHandler);
            Cmd.Variable = variable;
            Cmd.CommandCompleted += ReadEepromVariable_Completed;
            Cmd.send();
            //IrCOMPortManager.Instance.CommandList.Add(Cmd);
        }

        private void ReadEepromVariable_Completed(object sender, PropertyChangedEventArgs e)
        {
            ReadEEPROM cmd = sender as ReadEEPROM;
            if (cmd.Result.Outcome == CommandResultOutcomes.CommandSuccess)
            {
                //IrCOMPortManager.Instance.ExtCommandCompleted = IrCOMPortManager.CommandState.WaitForNew;
                //IrCOMPortManager.Instance.CommandList.Remove(cmd);
            }
        }

        public void ReadRamVariable(IRAMvariable variable)
        {

        }

        public void ReadGSMPar(IGSMvariable variable)
        {

        }

        public void WriteGSMPar(IGSMvariable variable)
        {

        }

        public void ReadGSMTest(IGSMvariable variable)
        {

        }

        public void WriteGSMTest(IGSMvariable variable)
        {

        }

        private ReadRAM CreateNewReadRam(IRAMvariable var)
        {
            ReadRAM cmd = new ReadRAM();
            cmd.Variable = var;
            return cmd;
        }

        private ReadEEPROM CreateNewReadEeprom(IEEPROMvariable var)
        {
            ReadEEPROM cmd = new ReadEEPROM();
            cmd.Variable = var;
            return cmd;
        }

        private WriteEEPROM CreateNewWriteEeprom(IEEPROMvariable var)
        {
            WriteEEPROM cmd = new WriteEEPROM();
            cmd.Variable = var;
            return cmd;
        }

        private ReadGSMPar CreateNewReadGSMPar(IGSMvariable var)
        {
            ReadGSMPar cmd = new ReadGSMPar();
            cmd.Variable = var;
            return cmd;
        }

        private WriteGSMPar CreateNewWriteGSMPar(IGSMvariable var)
        {
            WriteGSMPar cmd = new WriteGSMPar();
            cmd.Variable = var;
            return cmd;
        }

        private ReadGSMTest CreateNewReadGSMTest(IGSMvariable var)
        {
            ReadGSMTest cmd = new ReadGSMTest();
            cmd.Variable = var;
            return cmd;
        }

        private WriteGSMTest CreateNewWriteGSMTest(IGSMvariable var)
        {
            WriteGSMTest cmd = new WriteGSMTest();
            cmd.Variable = var;
            return cmd;
        }

        private void InitVarBundles()
        {
            _ramMeasBundleCmd = new ReadVarBundle();
            _ramMeasBundleCmd.BundleId = ReadVarBundle.RAM_MEASURE_BUNDLE;            

            _ramOthersBundleCmd = new ReadVarBundle();
            _ramOthersBundleCmd.BundleId = ReadVarBundle.RAM_OTHERS_BUNDLE;

            _ramT1T2PressBundleCmd = new ReadVarBundle();
            _ramT1T2PressBundleCmd.BundleId = ReadVarBundle.RAM_BUNDLE_T1_T2_PRESS;

            _ramBluetoothRS485Cmd = new ReadVarBundle();
            _ramBluetoothRS485Cmd.BundleId = ReadVarBundle.RAM_BUNDLE_BT_RS485;

            _ioVariablesBundleCmd = new ReadVarBundle();
            _ioVariablesBundleCmd.BundleId = ReadVarBundle.IO_VARIABLES_BUNDLE_BASE;            

            _eepParamsPageBundleCmd = new ReadVarBundle();
            _eepParamsPageBundleCmd.BundleId = ReadVarBundle.EEPROM_BUNDLE_BASE + 0;            

            _eepInfoPageBundleCmd = new ReadVarBundle();
            _eepInfoPageBundleCmd.BundleId = ReadVarBundle.EEPROM_BUNDLE_BASE + 1;            

            _eepRegisterPageBundleCmd = new ReadVarBundle();
            _eepRegisterPageBundleCmd.BundleId = ReadVarBundle.EEPROM_BUNDLE_BASE + 2;

            _eepCustomizedDeviceInfoCmd = new ReadVarBundle();
            _eepCustomizedDeviceInfoCmd.BundleId = ReadVarBundle.EEPROM_BUNDLE_BASE + 101;

            _eepCalibPageBundleCmd = new ReadVarBundle();
            _eepCalibPageBundleCmd.BundleId = ReadVarBundle.EEPROM_BUNDLE_BASE + 512;

            _eepCoilStabilizationCmd = new ReadVarBundle();
            _eepCoilStabilizationCmd.BundleId = ReadVarBundle.EEPROM_BUNDLE_BASE + 521;

            _ioParamsBundleCmd = new ReadVarBundle();
            _ioParamsBundleCmd.BundleId = ReadVarBundle.IO_PARAMS_BUNDLE_BASE;
        }

        private void AddVarToBundle(ref ReadVarBundle varBundleCmd, ITargetVariable tVar, UInt32 onCreateBundleId = 0, Boolean canReplace = false)
        {
            if (varBundleCmd == null)
            {
                varBundleCmd = new ReadVarBundle();
                varBundleCmd.BundleId = onCreateBundleId;
            }

            Type tVarType = tVar.GetType();

            if (!varBundleCmd.BundleSet.ContainsKey(tVarType))
            {
                varBundleCmd.BundleSet.Add(tVarType, tVar);
            }
            else if (canReplace)
            {
                varBundleCmd.BundleSet[tVarType] = tVar;
            }
        }

        #endregion // methods

        #region fields

        private ReadVarBundle _ramMeasBundleCmd;
        public ReadVarBundle RamMeasBundleCmd
        {
            get
            {
                return _ramMeasBundleCmd;
            }
            set
            {
                if (value != _ramMeasBundleCmd)
                {
                    _ramMeasBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _ramOthersBundleCmd;
        public ReadVarBundle RamOthersBundleCmd
        {
            get
            {
                return _ramOthersBundleCmd;
            }
            set
            {
                if (value != RamOthersBundleCmd)
                {
                    RamOthersBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _eepParamsPageBundleCmd;
        public ReadVarBundle EepParamsPageBundleCmd
        {
            get
            {
                return _eepParamsPageBundleCmd;
            }
            set
            {
                if (value != _eepParamsPageBundleCmd)
                {
                    _eepParamsPageBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _eepInfoPageBundleCmd;
        public ReadVarBundle EepInfoPageBundleCmd
        {
            get
            {
                return _eepInfoPageBundleCmd;
            }
            set
            {
                if (value != _eepInfoPageBundleCmd)
                {
                    _eepInfoPageBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _eepRegisterPageBundleCmd;
        public ReadVarBundle EepRegisterPageBundleCmd
        {
            get
            {
                return _eepRegisterPageBundleCmd;
            }
            set
            {
                if (value != _eepRegisterPageBundleCmd)
                {
                    _eepRegisterPageBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _eepCalibPageBundleCmd;
        public ReadVarBundle EepCalibPageBundleCmd
        {
            get
            {
                return _eepCalibPageBundleCmd;
            }
            set
            {
                if (value != _eepCalibPageBundleCmd)
                {
                    _eepCalibPageBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _ioParamsBundleCmd;
        public ReadVarBundle IoParamsBundleCmd
        {
            get
            {
                return _ioParamsBundleCmd;
            }
            set
            {
                if (value != _ioParamsBundleCmd)
                {
                    _ioParamsBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _ioVariablesBundleCmd;
        public ReadVarBundle IoVariablesBundleCmd
        {
            get
            {
                return _ioVariablesBundleCmd;
            }
            set
            {
                if (value != _ioVariablesBundleCmd)
                {
                    _ioVariablesBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _ramT1T2PressBundleCmd;
        public ReadVarBundle RamT1T2PressBundleCmd
        {
            get
            {
                return _ramT1T2PressBundleCmd;
            }
            set
            {
                if (value != _ramT1T2PressBundleCmd)
                {
                    _ramT1T2PressBundleCmd = value;
                }
            }
        }

        private ReadVarBundle _eepCustomizedDeviceInfoCmd;
        public ReadVarBundle EepCustomizedDeviceInfoCmd
        {
            get
            {
                return _eepCustomizedDeviceInfoCmd;
            }
            set
            {
                if (value != _eepCustomizedDeviceInfoCmd)
                {
                    _eepCustomizedDeviceInfoCmd = value;
                }
            }
        }
        
        private ReadVarBundle _ramBluetoothRS485Cmd;
        public ReadVarBundle RAMBluetoothRS485Cmd
        {
            get
            {
                return _ramBluetoothRS485Cmd;
            }
            set
            {
                if (value != _ramBluetoothRS485Cmd)
                {
                    _ramBluetoothRS485Cmd = value;
                }
            }
        }

        

        private ReadVarBundle _eepBluetoothModbusDeviceInfoCmd;
        public ReadVarBundle EEPBluetoothModbusDeviceInfoCmd
        {
            get
            {
                return _eepBluetoothModbusDeviceInfoCmd;
            }
            set
            {
                if (value != _eepBluetoothModbusDeviceInfoCmd)
                {
                    _eepBluetoothModbusDeviceInfoCmd = value;
                }
            }
        }
        
        private ReadVarBundle _eepCoilStabilizationCmd;
        public ReadVarBundle EEPCoilStabilizationCmd
        {
            get
            {
                return _eepCoilStabilizationCmd;
            }
            set
            {
                if (value != _eepCoilStabilizationCmd)
                {
                    _eepCoilStabilizationCmd = value;
                }
            }
        }

        #endregion // fields        

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
