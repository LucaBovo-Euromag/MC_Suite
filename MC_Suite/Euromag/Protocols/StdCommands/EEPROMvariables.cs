/* This is an automatic generated file,please don't modify it,
* changes will be overwritten
* 
* Created: 23/09/2022 11:26:10
*/

using System;
using System.Collections.Generic;
using Euromag.Utility.Endianness;

namespace MC_Suite.Euromag.Protocols.StdCommands
{
    #region EEPROM vars lists

    /// <summary>
    /// Lists EEPROM variables
    /// </summary>
    class AllEEPROMVarsLister
    {
        public static ICollection<ITargetWritable> getList()
        {
            List<ITargetWritable> list = new List<ITargetWritable>();
            list.Add(new MEAS_FREQ());
            list.Add(new UT_FLOW());
            list.Add(new TB_FLOW());
            list.Add(new UT_ACC());
            list.Add(new UT_PULSE());
            list.Add(new PULSE_VOL());
            list.Add(new CUTOFF());
            list.Add(new SENSOR_OFFSET());
            list.Add(new KA());
            list.Add(new DIAMETER());
            list.Add(new PULSE_LENGTH());
            list.Add(new EXC_PAUSE());
            list.Add(new OFFSET_TEMP());
            list.Add(new DAMPING());
            list.Add(new EPIPE_TH());
            list.Add(new EPIPE());
            list.Add(new NDEC_INST());
            list.Add(new NDEC_ACC());
            list.Add(new FS_MS());
            list.Add(new BYPASS());
            list.Add(new PEAK_CUT());
            list.Add(new BYPASS_COUNT());
            list.Add(new PEAKCUT_COUNT());
            list.Add(new SPEC_HEAT_sA());
            list.Add(new SPEC_HEAT_sB());
            list.Add(new SPEC_HEAT_sC());
            list.Add(new SPEC_HEAT_sD());
            list.Add(new DENSITY_cA());
            list.Add(new DENSITY_cB());
            list.Add(new DENSITY_cC());
            list.Add(new DENSITY_cD());
            list.Add(new DTMIN());
            list.Add(new TMEAS_TIME());
            list.Add(new PMEAS_TIME());
            list.Add(new ENERGY_OPTION());
            list.Add(new PRESS_OPTION());
            list.Add(new PLOG_TIME());
            list.Add(new CALIBR_TEMP());
            list.Add(new CALIBR_VOLT());
            list.Add(new OFFSET_REG());
            list.Add(new KALIGN());
            list.Add(new OFFALIGN());
            list.Add(new SENSOR_MODEL());
            list.Add(new OTHER_FEAT());
            list.Add(new CONV_ID());
            list.Add(new SENSOR_ID());
            list.Add(new CONV_SN());
            list.Add(new CAL_DATE());
            list.Add(new DEV_NAME());
            list.Add(new MANUFACTURER());
            list.Add(new PASSWORD());
            list.Add(new BASE_SEC());
            list.Add(new MCOUNT());
            list.Add(new uAh_TOT());
            list.Add(new uAh_LEFT());
            list.Add(new AWAKE_SEC());
            list.Add(new WAKEUP_SEC());
            list.Add(new PWD_TIMEOUT());
            list.Add(new BATT_AUTOSAVE());
            list.Add(new TMEAS_SD24());
            list.Add(new EPIPE_FREQ());
            list.Add(new EPIPE_RELEASE());
            list.Add(new MEAS_WD_EN());
            list.Add(new MEAS_WD_TH());
            list.Add(new TIMEOUT_TO_MAIN());
            list.Add(new ADC_GAIN());
            list.Add(new LINE_FREQ());
            list.Add(new INSERTION());
            list.Add(new MEAS_AWAKE_MS());
            list.Add(new EPIPE_FREQ_FAST());
            list.Add(new INPUT_STAGE_STAB());
            list.Add(new DAMPING_SLOW());
            list.Add(new GSMinstalled());
            list.Add(new GSM_SimPin());
            list.Add(new GSM_setSMS());
            list.Add(new GSM_setEMAIL());
            list.Add(new GSM_setEMAIL_ATTACH());
            list.Add(new GSM_setFTP_ATTACH());
            list.Add(new GSM_max_consecutive_rows());
            list.Add(new GSM_cfg_timeout());
            list.Add(new GSM_hard_timeout());
            list.Add(new GSM_timeSMS());
            list.Add(new GSM_timeEMAIL());
            list.Add(new GSM_timeEMAIL_ATTACH());
            list.Add(new GSM_timeFTP());
            list.Add(new GSM_hourSMS());
            list.Add(new GSM_hourEMAIL());
            list.Add(new GSM_hourEMAIL_ATTACH());
            list.Add(new GSM_hourFTP());
            list.Add(new GSM_zoneGMT());
            list.Add(new GSM_roaming());
            //list.Add(new GSM_ErrorLogDataStart());
            list.Add(new INTERP_sA_LO());
            list.Add(new INTERP_sB_LO());
            list.Add(new INTERP_sC_LO());
            list.Add(new INTERP_sD_LO());
            list.Add(new INTERP_sA_HI());
            list.Add(new INTERP_sB_HI());
            list.Add(new INTERP_sC_HI());
            list.Add(new INTERP_sD_HI());
            list.Add(new INTERP_TH());
            list.Add(new PRESS_PROBE_CAL());
            list.Add(new PRESS_CURRENT_CAL());
            list.Add(new SPECIAL_VISUAL());
            list.Add(new PRODUCT_VARIANT());
            list.Add(new PRESS_PROBE_CAL_OFFSET());
            list.Add(new ENABLE_BOARD_4_20_MA());
            list.Add(new CAL_LEV_4MA());
            list.Add(new CAL_LEV_20MA());
            list.Add(new MEAS_FOR_4_20_MA());
            list.Add(new ENABLE_REV_FLOW_ON_4_20MA());
            list.Add(new VEL_LEV_FOR_4MA());
            list.Add(new VEL_LEV_FOR_20MA());
            list.Add(new REV_FLOW_ERR_VAL_4_20MA_OUT());
            list.Add(new EPIPE_ERR_VAL_4_20MA_OUT());
            list.Add(new COIL_ERR_VAL_4_20MA_OUT());
            list.Add(new GEN_ERR_VAL_4_20MA_OUT());
            list.Add(new NEG_PULSE_OUTPUT_MODE());
            list.Add(new OUT_LOWER_LIMIT_4_20MA());
            list.Add(new OUT_UPPER_LIMIT_4_20MA());
            list.Add(new CUSTIMIZED_CONV_ID());
            list.Add(new CUSTIMIZED_SENSOR_ID());
            list.Add(new CUSTOMIZED_SENSOR_MODEL());
            list.Add(new CUSTOMIZED_DEV_NAME());
            list.Add(new CUSTOMIZED_INFO_ENABLE());
            //Scheda Bluetooth/RS485********************/
            list.Add(new ENABLE_BOARD_BT_RS485());
            list.Add(new BT_ON_INTERVAL());
            list.Add(new RS485_ON_INTERVAL());
            list.Add(new RS485_BAUDRATE());
            list.Add(new RS485_DATA_NUM_BIT());
            list.Add(new RS485_PARITY());
            list.Add(new RS485_STOP_BITS());
            list.Add(new RS485_MODBUS_MODE());
            list.Add(new RS485_MODBUS_ADDR());
            list.Add(new RS485_MODUBUS_BYTE_ORDER());
            list.Add(new RS_485_ON_DURATION());
            list.Add(new BT_ON_DURATION());
            list.Add(new RS485_MODUBUS_MAP());
            list.Add(new MAIN_PWR_INTERR_CNT());
            list.Add(new MAIN_PWR_INTERR_TOT_SEC());
            list.Add(new MODBUS_RESET_PERIOD());
            list.Add(new POWER_INTERR_SETTING());
            list.Add(new PROTOCOL_V());
            list.Add(new PROTOCOL_R());
            return list.AsReadOnly();
        }
    }

    #endregion EEPROM vars lists
    #region EEPROMvariable declaration

    public enum EEPROMAddresses : uint
    {
        MEAS_FREQ_ADDR = 1,
        UT_FLOW_ADDR = 2,
        TB_FLOW_ADDR = 3,
        UT_ACC_ADDR = 4,
        UT_PULSE_ADDR = 5,
        PULSE_VOL_ADDR = 6,
        CUTOFF_ADDR = 7,
        SENSOR_OFFSET_ADDR = 8,
        KA_ADDR = 9,
        DIAMETER_ADDR = 10,
        PULSE_LENGTH_ADDR = 11,
        EXC_PAUSE_ADDR = 12,
        OFFSET_TEMP_ADDR = 13,
        DAMPING_ADDR = 14,
        EPIPE_TH_ADDR = 15,
        EPIPE_ADDR = 16,
        NDEC_INST_ADDR = 17,
        NDEC_ACC_ADDR = 18,
        FS_MS_ADDR = 19,
        BYPASS_ADDR = 20,
        PEAK_CUT_ADDR = 21,
        BYPASS_COUNT_ADDR = 22,
        PEAKCUT_COUNT_ADDR = 23,
        SPEC_HEAT_sA_ADDR = 24,
        SPEC_HEAT_sB_ADDR = 25,
        SPEC_HEAT_sC_ADDR = 26,
        SPEC_HEAT_sD_ADDR = 27,
        DENSITY_cA_ADDR = 28,
        DENSITY_cB_ADDR = 29,
        DENSITY_cC_ADDR = 30,
        DENSITY_cD_ADDR = 31,
        DTMIN_ADDR = 32,
        TMEAS_TIME_ADDR = 33,
        PMEAS_TIME_ADDR = 34,
        ENERGY_OPTION_ADDR = 35,
        PRESS_OPTION_ADDR = 36,
        PLOG_TIME_ADDR = 37,
        CALIBR_TEMP_ADDR = 38,
        CALIBR_VOLT_ADDR = 39,
        OFFSET_REG_ADDR = 40,
        KALIGN_ADDR = 41,
        OFFALIGN_ADDR = 42,
        SENSOR_MODEL_ADDR = 43,
        OTHER_FEAT_ADDR = 44,
        CONV_ID_ADDR = 45,
        SENSOR_ID_ADDR = 46,
        CONV_SN_ADDR = 47,
        CAL_DATE_ADDR = 48,
        DEV_NAME_ADDR = 49,
        MANUFACTURER_ADDR = 50,
        PASSWORD_ADDR = 51,
        BASE_SEC_ADDR = 52,
        MCOUNT_ADDR = 53,
        uAh_TOT_ADDR = 54,
        uAh_LEFT_ADDR = 55,
        AWAKE_SEC_ADDR = 56,
        WAKEUP_SEC_ADDR = 57,
        PWD_TIMEOUT_ADDR = 58,
        BATT_AUTOSAVE_ADDR = 59,
        TMEAS_SD24_ADDR = 60,
        EPIPE_FREQ_ADDR = 61,
        EPIPE_RELEASE_ADDR = 62,
        MEAS_WD_EN_ADDR = 63,
        MEAS_WD_TH_ADDR = 64,
        TIMEOUT_TO_MAIN_ADDR = 65,
        ADC_GAIN_ADDR = 66,
        LINE_FREQ_ADDR = 67,
        INSERTION_ADDR = 68,
        MEAS_AWAKE_MS_ADDR = 69,
        EPIPE_FREQ_FAST_ADDR = 70,
        INPUT_STAGE_STAB_ADDR = 71,
        DAMPING_SLOW_ADDR = 72,
        GSMinstalled_ADDR = 73,
        GSM_SimPin_ADDR = 74,
        GSM_setSMS_ADDR = 75,
        GSM_setEMAIL_ADDR = 76,
        GSM_setEMAIL_ATTACH_ADDR = 77,
        GSM_setFTP_ATTACH_ADDR = 78,
        GSM_max_consecutive_rows_ADDR = 79,
        GSM_cfg_timeout_ADDR = 80,
        GSM_hard_timeout_ADDR = 81,
        GSM_timeSMS_ADDR = 82,
        GSM_timeEMAIL_ADDR = 83,
        GSM_timeEMAIL_ATTACH_ADDR = 84,
        GSM_timeFTP_ADDR = 85,
        GSM_hourSMS_ADDR = 86,
        GSM_hourEMAIL_ADDR = 87,
        GSM_hourEMAIL_ATTACH_ADDR = 88,
        GSM_hourFTP_ADDR = 89,
        GSM_zoneGMT_ADDR = 90,
        GSM_roaming_ADDR = 91,
        GSM_ErrorLogDataStart_ADDR = 92,
        INTERP_sA_LO_ADDR = 93,
        INTERP_sB_LO_ADDR = 94,
        INTERP_sC_LO_ADDR = 95,
        INTERP_sD_LO_ADDR = 96,
        INTERP_sA_HI_ADDR = 97,
        INTERP_sB_HI_ADDR = 98,
        INTERP_sC_HI_ADDR = 99,
        INTERP_sD_HI_ADDR = 100,
        INTERP_TH_ADDR = 101,
        PRESS_PROBE_CAL_ADDR = 102,
        PRESS_CURRENT_CAL_ADDR = 103,
        SPECIAL_VISUAL_ADDR = 104,
        PRODUCT_VARIANT_ADDR = 105,
        PRESS_PROBE_CAL_OFFSET_ADDR = 106,
        ENABLE_BOARD_4_20_MA_ADDR = 107,
        CAL_LEV_4MA_ADDR = 108,
        CAL_LEV_20MA_ADDR = 109,
        MEAS_FOR_4_20_MA_ADDR = 110,
        ENABLE_REV_FLOW_ON_4_20MA_ADDR = 111,
        VEL_LEV_FOR_4MA_ADDR = 112,
        VEL_LEV_FOR_20MA_ADDR = 113,
        REV_FLOW_ERR_VAL_4_20MA_OUT_ADDR = 114,
        EPIPE_ERR_VAL_4_20MA_OUT_ADDR = 115,
        COIL_ERR_VAL_4_20MA_OUT_ADDR = 116,
        GEN_ERR_VAL_4_20MA_OUT_ADDR = 117,
        NEG_PULSE_OUTPUT_MODE_ADDR = 118,
        OUT_LOWER_LIMIT_4_20MA_ADDR = 119,
        OUT_UPPER_LIMIT_4_20MA_ADDR = 120,
        CUSTIMIZED_CONV_ID_ADDR = 121,
        CUSTIMIZED_SENSOR_ID_ADDR = 122,
        CUSTOMIZED_SENSOR_MODEL_ADDR = 123,
        CUSTOMIZED_DEV_NAME_ADDR = 124,
        CUSTOMIZED_INFO_ENABLE_ADDR = 125,
        ENABLE_BOARD_BT_RS485_ADDR = 126,
        BT_ON_INTERVAL_ADDR = 127,
        RS485_ON_INTERVAL_ADDR = 128,
        RS485_BAUDRATE_ADDR = 129,
        RS485_DATA_NUM_BIT_ADDR = 130,
        RS485_PARITY_ADDR = 131,
        RS485_STOP_BITS_ADDR = 132,
        RS485_MODBUS_MODE_ADDR = 133,
        RS485_MODBUS_ADDR_ADDR = 134,
        RS485_MODUBUS_BYTE_ORDER_ADDR = 135,
        RS_485_ON_DURATION_ADDR = 136,
        BT_ON_DURATION_ADDR = 137,
        RS485_MODUBUS_MAP_ADDR = 138,
        MAIN_PWR_INTERR_CNT_ADDR = 139,
        MAIN_PWR_INTERR_TOT_SEC_ADDR = 140,
        MODBUS_RESET_PERIOD_ADDR = 141,
        POWER_INTERR_SETTING_ADDR = 142,
        PROTOCOL_V_ADDR = 143,
        PROTOCOL_R_ADDR = 144,

    }

    /// <summary>
    /// Objects must implement IEEPROMvariable interface to be consideres EEPROM variables
    /// </summary>
    public interface IEEPROMvariable : ITargetWritable
    {
        UInt32 ID
        { get; }
        
        EEPROMAddresses Address
        { get; }
    }

    #endregion EEPROMvariable declaration
    #region EEPROM Variables

    /// <summary>
    /// Implemented EEPROM Variables
    /// </summary>

    #region Byte EEPROM vars

    public class MEAS_FREQ : WritableByteTargetVariable, IEEPROMvariable
    {
        public MEAS_FREQ()
            : base()
        {

        }

        public MEAS_FREQ(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "low power measuring period [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 122;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MEAS_FREQ_ADDR; }
        }

    }

    public class EXC_PAUSE : WritableByteTargetVariable, IEEPROMvariable
    {
        public EXC_PAUSE()
            : base()
        {

        }

        public EXC_PAUSE(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "coil excitation stabilization delay [ms]";
        }

        public UInt32 ID
        {
            get
            {
                return 117;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.EXC_PAUSE_ADDR; }
        }

    }

    public class NDEC_INST : WritableByteTargetVariable, IEEPROMvariable
    {
        public NDEC_INST()
            : base()
        {

        }

        public NDEC_INST(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "decimal digits count (flowrate)";
        }

        public UInt32 ID
        {
            get
            {
                return 130;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.NDEC_INST_ADDR; }
        }

    }

    public class NDEC_ACC : WritableByteTargetVariable, IEEPROMvariable
    {
        public NDEC_ACC()
            : base()
        {

        }

        public NDEC_ACC(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "decimal digits count (totalizers)";
        }

        public UInt32 ID
        {
            get
            {
                return 131;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.NDEC_ACC_ADDR; }
        }

    }

    public class FS_MS : WritableByteTargetVariable, IEEPROMvariable
    {
        public FS_MS()
            : base()
        {

        }

        public FS_MS(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "flowrate full-scale [m/s / 10]";
        }

        public UInt32 ID
        {
            get
            {
                return 132;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.FS_MS_ADDR; }
        }

    }

    public class BYPASS : WritableByteTargetVariable, IEEPROMvariable
    {
        public BYPASS()
            : base()
        {

        }

        public BYPASS(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "bypass filter threshold [%full scale]";
        }

        public UInt32 ID
        {
            get
            {
                return 133;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.BYPASS_ADDR; }
        }

    }

    public class PEAK_CUT : WritableByteTargetVariable, IEEPROMvariable
    {
        public PEAK_CUT()
            : base()
        {

        }

        public PEAK_CUT(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "peak cut filter threshold [%full scale]";
        }

        public UInt32 ID
        {
            get
            {
                return 134;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PEAK_CUT_ADDR; }
        }

    }

    public class BYPASS_COUNT : WritableByteTargetVariable, IEEPROMvariable
    {
        public BYPASS_COUNT()
            : base()
        {

        }

        public BYPASS_COUNT(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "bypass filter minimum consecutive events";
        }

        public UInt32 ID
        {
            get
            {
                return 135;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.BYPASS_COUNT_ADDR; }
        }

    }

    public class PEAKCUT_COUNT : WritableByteTargetVariable, IEEPROMvariable
    {
        public PEAKCUT_COUNT()
            : base()
        {

        }

        public PEAKCUT_COUNT(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "peak-cut filter maximum consecutive events";
        }

        public UInt32 ID
        {
            get
            {
                return 136;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PEAKCUT_COUNT_ADDR; }
        }

    }

    public class DTMIN : WritableByteTargetVariable, IEEPROMvariable
    {
        public DTMIN()
            : base()
        {

        }

        public DTMIN(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "minimum dT for energy calculation (2 probes) [C]";
        }

        public UInt32 ID
        {
            get
            {
                return 11;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DTMIN_ADDR; }
        }

    }

    public class TMEAS_TIME : WritableByteTargetVariable, IEEPROMvariable
    {
        public TMEAS_TIME()
            : base()
        {

        }

        public TMEAS_TIME(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "external temperature probes measuring period [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 12;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.TMEAS_TIME_ADDR; }
        }

    }

    public class PMEAS_TIME : WritableByteTargetVariable, IEEPROMvariable
    {
        public PMEAS_TIME()
            : base()
        {

        }

        public PMEAS_TIME(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "external pressure probe measuring period [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 13;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PMEAS_TIME_ADDR; }
        }

    }

    public class PLOG_TIME : WritableByteTargetVariable, IEEPROMvariable
    {
        public PLOG_TIME()
            : base()
        {

        }

        public PLOG_TIME(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "process log line storing period [minutes]";
        }

        public UInt32 ID
        {
            get
            {
                return 16;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PLOG_TIME_ADDR; }
        }

    }

    public class CALIBR_TEMP : WritableByteTargetVariable, IEEPROMvariable
    {
        public CALIBR_TEMP()
            : base()
        {

        }

        public CALIBR_TEMP(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "internal temperature registred during the calibration [C]";
        }

        public UInt32 ID
        {
            get
            {
                return 17;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CALIBR_TEMP_ADDR; }
        }

    }

    public class CALIBR_VOLT : WritableByteTargetVariable, IEEPROMvariable
    {
        public CALIBR_VOLT()
            : base()
        {

        }

        public CALIBR_VOLT(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "internal voltage registred during the calibration [V]";
        }

        public UInt32 ID
        {
            get
            {
                return 18;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CALIBR_VOLT_ADDR; }
        }

    }

    public class OFFSET_REG : WritableByteTargetVariable, IEEPROMvariable
    {
        public OFFSET_REG()
            : base()
        {

        }

        public OFFSET_REG(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "input stage bias compensation switch closing time [ms]";
        }

        public UInt32 ID
        {
            get
            {
                return 137;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.OFFSET_REG_ADDR; }
        }

    }

    public class EPIPE_RELEASE : WritableByteTargetVariable, IEEPROMvariable
    {
        public EPIPE_RELEASE()
            : base()
        {

        }

        public EPIPE_RELEASE(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "skip-measures time after an empty pipe detection (4th electrode) [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 140;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.EPIPE_RELEASE_ADDR; }
        }

    }

    public class EPIPE_FREQ_FAST : WritableByteTargetVariable, IEEPROMvariable
    {
        public EPIPE_FREQ_FAST()
            : base()
        {

        }

        public EPIPE_FREQ_FAST(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "empty pipe measurment period during empty-pipe condition (4th electrode) [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 146;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.EPIPE_FREQ_FAST_ADDR; }
        }

    }

    public class GSM_cfg_timeout : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_cfg_timeout()
            : base()
        {

        }

        public GSM_cfg_timeout(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "timeout modem configuration mode(minutes)";
        }

        public UInt32 ID
        {
            get
            {
                return 78;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_cfg_timeout_ADDR; }
        }

    }

    public class GSM_hard_timeout : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_hard_timeout()
            : base()
        {

        }

        public GSM_hard_timeout(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "timeout modem power-off (minutes); forced turn-off via hardware";
        }

        public UInt32 ID
        {
            get
            {
                return 79;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_hard_timeout_ADDR; }
        }

    }

    public class GSM_timeSMS : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_timeSMS()
            : base()
        {

        }

        public GSM_timeSMS(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "time parameter X SMS send";
        }

        public UInt32 ID
        {
            get
            {
                return 80;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_timeSMS_ADDR; }
        }

    }

    public class GSM_timeEMAIL : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_timeEMAIL()
            : base()
        {

        }

        public GSM_timeEMAIL(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "time parameter X EMAIL send";
        }

        public UInt32 ID
        {
            get
            {
                return 81;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_timeEMAIL_ADDR; }
        }

    }

    public class GSM_timeEMAIL_ATTACH : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_timeEMAIL_ATTACH()
            : base()
        {

        }

        public GSM_timeEMAIL_ATTACH(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "time parameter X EMAIL ATTACHMENT send";
        }

        public UInt32 ID
        {
            get
            {
                return 82;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_timeEMAIL_ATTACH_ADDR; }
        }

    }

    public class GSM_timeFTP : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_timeFTP()
            : base()
        {

        }

        public GSM_timeFTP(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "time parameter X FTP send";
        }

        public UInt32 ID
        {
            get
            {
                return 83;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_timeFTP_ADDR; }
        }

    }

    public class GSM_hourSMS : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_hourSMS()
            : base()
        {

        }

        public GSM_hourSMS(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "SMS sending hour (not used in case of \"send every X hours\")";
        }

        public UInt32 ID
        {
            get
            {
                return 84;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_hourSMS_ADDR; }
        }

    }

    public class GSM_hourEMAIL : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_hourEMAIL()
            : base()
        {

        }

        public GSM_hourEMAIL(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "EMAIL sending hour (not used in case of \"send every X hours\")";
        }

        public UInt32 ID
        {
            get
            {
                return 85;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_hourEMAIL_ADDR; }
        }

    }

    public class GSM_hourEMAIL_ATTACH : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_hourEMAIL_ATTACH()
            : base()
        {

        }

        public GSM_hourEMAIL_ATTACH(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "EMAIL with ATTACHMENT sending hour (not used in case of \"send every X hours\")";
        }

        public UInt32 ID
        {
            get
            {
                return 86;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_hourEMAIL_ATTACH_ADDR; }
        }

    }

    public class GSM_hourFTP : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_hourFTP()
            : base()
        {

        }

        public GSM_hourFTP(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "FTP sending hour (not used in case of \"send every X hours\")";
        }

        public UInt32 ID
        {
            get
            {
                return 87;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_hourFTP_ADDR; }
        }

    }

    public class GSM_zoneGMT : WritableByteTargetVariable, IEEPROMvariable
    {
        public GSM_zoneGMT()
            : base()
        {

        }

        public GSM_zoneGMT(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "GMT zone (quarter per hour - offset +100)";
        }

        public UInt32 ID
        {
            get
            {
                return 88;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_zoneGMT_ADDR; }
        }

    }

    public class ENABLE_BOARD_4_20_MA : WritableByteTargetVariable, IEEPROMvariable
    {
        public ENABLE_BOARD_4_20_MA()
            : base()
        {

        }

        public ENABLE_BOARD_4_20_MA(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "FLAG TO ENABLE 4-20MA BOARD";
        }

        public UInt32 ID
        {
            get
            {
                return 33;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.ENABLE_BOARD_4_20_MA_ADDR; }
        }

    }

    public class ENABLE_REV_FLOW_ON_4_20MA : WritableByteTargetVariable, IEEPROMvariable
    {
        public ENABLE_REV_FLOW_ON_4_20MA()
            : base()
        {

        }

        public ENABLE_REV_FLOW_ON_4_20MA(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "enable output 2-20mA for revers flow";
        }

        public UInt32 ID
        {
            get
            {
                return 29;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.ENABLE_REV_FLOW_ON_4_20MA_ADDR; }
        }

    }

    public class CUSTOMIZED_INFO_ENABLE : WritableByteTargetVariable, IEEPROMvariable
    {
        public CUSTOMIZED_INFO_ENABLE()
            : base()
        {

        }

        public CUSTOMIZED_INFO_ENABLE(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "enable customized info";
        }

        public UInt32 ID
        {
            get
            {
                return 95;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CUSTOMIZED_INFO_ENABLE_ADDR; }
        }

    }

    public class RS485_MODBUS_ADDR : WritableByteTargetVariable, IEEPROMvariable
    {
        public RS485_MODBUS_ADDR()
            : base()
        {

        }

        public RS485_MODBUS_ADDR(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "rs485 modbus addrs - slave mode";
        }

        public UInt32 ID
        {
            get
            {
                return 104;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_MODBUS_ADDR_ADDR; }
        }

    }

    public class RS_485_ON_DURATION : WritableByteTargetVariable, IEEPROMvariable
    {
        public RS_485_ON_DURATION()
            : base()
        {

        }

        public RS_485_ON_DURATION(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "rs485 on duration";
        }

        public UInt32 ID
        {
            get
            {
                return 106;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS_485_ON_DURATION_ADDR; }
        }

    }

    public class BT_ON_DURATION : WritableByteTargetVariable, IEEPROMvariable
    {
        public BT_ON_DURATION()
            : base()
        {

        }

        public BT_ON_DURATION(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "bt on duration";
        }

        public UInt32 ID
        {
            get
            {
                return 107;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.BT_ON_DURATION_ADDR; }
        }

    }

    public class MODBUS_RESET_PERIOD : WritableByteTargetVariable, IEEPROMvariable
    {
        public MODBUS_RESET_PERIOD()
            : base()
        {

        }

        public MODBUS_RESET_PERIOD(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "MODBUS RESET PERIOD";
        }

        public UInt32 ID
        {
            get
            {
                return 111;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MODBUS_RESET_PERIOD_ADDR; }
        }

    }

    public class POWER_INTERR_SETTING : WritableByteTargetVariable, IEEPROMvariable
    {
        public POWER_INTERR_SETTING()
            : base()
        {

        }

        public POWER_INTERR_SETTING(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "EE_POWER_INTERR_SETTING";
        }

        public UInt32 ID
        {
            get
            {
                return 112;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.POWER_INTERR_SETTING_ADDR; }
        }

    }

    public class PROTOCOL_V : WritableByteTargetVariable, IEEPROMvariable
    {
        public PROTOCOL_V()
            : base()
        {

        }

        public PROTOCOL_V(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "communication protocol ver";
        }

        public UInt32 ID
        {
            get
            {
                return 46;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PROTOCOL_V_ADDR; }
        }

    }

    public class PROTOCOL_R : WritableByteTargetVariable, IEEPROMvariable
    {
        public PROTOCOL_R()
            : base()
        {

        }

        public PROTOCOL_R(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "communication protocol rel";
        }

        public UInt32 ID
        {
            get
            {
                return 47;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PROTOCOL_R_ADDR; }
        }

    }

    #endregion Byte EEPROM vars

    #region Enumerator EEPROM vars

    public class UT_FLOW : WritableEnumTargetVariable, IEEPROMvariable
    {
        public UT_FLOW()
            : base()
        {

        }

        public UT_FLOW(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "flowrate technical unit index";
        }

        public UInt32 ID
        {
            get
            {
                return 124;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.UT_FLOW_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(1, "meter [m]");
                    _optionList.Add(2, "cubic meter [m�]");
                    _optionList.Add(3, "liter [L]");
                    _optionList.Add(4, "mega liter [ML]");
                    _optionList.Add(5, "cubic foot [ft�]");
                    _optionList.Add(6, "US liquid gallon [gal]");
                    _optionList.Add(7, "US Oil Barrel [BBL]");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class TB_FLOW : WritableEnumTargetVariable, IEEPROMvariable
    {
        public TB_FLOW()
            : base()
        {

        }

        public TB_FLOW(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "flowrate time base index";
        }

        public UInt32 ID
        {
            get
            {
                return 125;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.TB_FLOW_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(1, "second [s]");
                    _optionList.Add(2, "minute [m]");
                    _optionList.Add(3, "hour [h]");
                    _optionList.Add(4, "day [d]");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class UT_ACC : WritableEnumTargetVariable, IEEPROMvariable
    {
        public UT_ACC()
            : base()
        {

        }

        public UT_ACC(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "totalizers technical unit index";
        }

        public UInt32 ID
        {
            get
            {
                return 126;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.UT_ACC_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(2, "cubic meter [m�]");
                    _optionList.Add(3, "liter [L]");
                    _optionList.Add(4, "mega liter [ML]");
                    _optionList.Add(5, "US liquid gallon [gal]");
                    _optionList.Add(6, "US Oil Barrel [BBL]");
                    _optionList.Add(7, "Acre foot [a-ft]");
                    _optionList.Add(8, "Acre inch [a-in]");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class UT_PULSE : WritableEnumTargetVariable, IEEPROMvariable
    {
        public UT_PULSE()
            : base()
        {

        }

        public UT_PULSE(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "pulse volume entry technical unit";
        }

        public UInt32 ID
        {
            get
            {
                return 42;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.UT_PULSE_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(1, "milliliter [mL]");
                    _optionList.Add(2, "liter [L]");
                    _optionList.Add(3, "cubic meter [m�]");
                    _optionList.Add(4, "US liquid gallon [gal]");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class EPIPE : WritableEnumTargetVariable, IEEPROMvariable
    {
        public EPIPE()
            : base()
        {

        }

        public EPIPE(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "empty pipe configuration";
        }

        public UInt32 ID
        {
            get
            {
                return 123;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.EPIPE_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "detect disabled");
                    _optionList.Add(1, "detect enabled (4th electrode)");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class ENERGY_OPTION : WritableEnumTargetVariable, IEEPROMvariable
    {
        public ENERGY_OPTION()
            : base()
        {

        }

        public ENERGY_OPTION(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "external temperature probes / energy metering configuration";
        }

        public UInt32 ID
        {
            get
            {
                return 14;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.ENERGY_OPTION_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "external temperature probes disabled");
                    _optionList.Add(1, "one probe enabled (no energy calculation possible)");
                    _optionList.Add(2, "two probes enabled (energy measurement available)");
                    _optionList.Add(3, "two probes enabled (energy measurement disabled)");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class PRESS_OPTION : WritableEnumTargetVariable, IEEPROMvariable
    {
        public PRESS_OPTION()
            : base()
        {

        }

        public PRESS_OPTION(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "external pressure probe configuration";
        }

        public UInt32 ID
        {
            get
            {
                return 15;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PRESS_OPTION_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "pressure measure disabled");
                    _optionList.Add(1, "pressure measure enabled");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class WAKEUP_SEC : WritableEnumTargetVariable, IEEPROMvariable
    {
        public WAKEUP_SEC()
            : base()
        {

        }

        public WAKEUP_SEC(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "auto power-off timeout [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 19;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.WAKEUP_SEC_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "20 s");
                    _optionList.Add(1, "1 min");
                    _optionList.Add(2, "3 min");
                    _optionList.Add(3, "15 min");
                    _optionList.Add(4, "1 h");
                    _optionList.Add(5, "6 h");
                    _optionList.Add(6, "12 h");
                    _optionList.Add(7, "18 h");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class TMEAS_SD24 : WritableEnumTargetVariable, IEEPROMvariable
    {
        public TMEAS_SD24()
            : base()
        {

        }

        public TMEAS_SD24(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "flow measurment internal 24bit sigma-delta decimator sampling index";
        }

        public UInt32 ID
        {
            get
            {
                return 138;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.TMEAS_SD24_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "Single length");
                    _optionList.Add(1, "Double length");
                    _optionList.Add(2, "Triple length");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class MEAS_WD_EN : WritableEnumTargetVariable, IEEPROMvariable
    {
        public MEAS_WD_EN()
            : base()
        {

        }

        public MEAS_WD_EN(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "measuring electrodes dry condition detect configuration";
        }

        public UInt32 ID
        {
            get
            {
                return 141;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MEAS_WD_EN_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "disabled");
                    _optionList.Add(1, "enabled");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class ADC_GAIN : WritableEnumTargetVariable, IEEPROMvariable
    {
        public ADC_GAIN()
            : base()
        {

        }

        public ADC_GAIN(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "24bit sigma-delta gain index (flow measure)";
        }

        public UInt32 ID
        {
            get
            {
                return 143;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.ADC_GAIN_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "gain 1");
                    _optionList.Add(1, "gain 2");
                    _optionList.Add(2, "gain 4");
                    _optionList.Add(3, "gain 8");
                    _optionList.Add(4, "gain 16");
                    _optionList.Add(5, "gain 32");
                    _optionList.Add(6, "gain 64");
                    _optionList.Add(7, "gain 128");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class LINE_FREQ : WritableEnumTargetVariable, IEEPROMvariable
    {
        public LINE_FREQ()
            : base()
        {

        }

        public LINE_FREQ(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "power line frequency (50/60Hz)";
        }

        public UInt32 ID
        {
            get
            {
                return 149;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.LINE_FREQ_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "50 Hz");
                    _optionList.Add(1, "60 Hz");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class INSERTION : WritableEnumTargetVariable, IEEPROMvariable
    {
        public INSERTION()
            : base()
        {

        }

        public INSERTION(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "sensor insertion type option";
        }

        public UInt32 ID
        {
            get
            {
                return 144;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INSERTION_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "normal sensor");
                    _optionList.Add(1, "insertion type");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class GSMinstalled : WritableEnumTargetVariable, IEEPROMvariable
    {
        public GSMinstalled()
            : base()
        {

        }

        public GSMinstalled(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "GSM installed";
        }

        public UInt32 ID
        {
            get
            {
                return 69;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSMinstalled_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "not insalled");
                    _optionList.Add(1, "installed");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class GSM_setSMS : WritableEnumTargetVariable, IEEPROMvariable
    {
        public GSM_setSMS()
            : base()
        {

        }

        public GSM_setSMS(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "SMS send mode";
        }

        public UInt32 ID
        {
            get
            {
                return 71;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_setSMS_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "disabled");
                    _optionList.Add(1, "every X (1-24) hours");
                    _optionList.Add(2, "every day at hour X (0-23)");
                    _optionList.Add(3, "every X (1-7) day of week");
                    _optionList.Add(4, "every X (1-28) day of month");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class GSM_setEMAIL : WritableEnumTargetVariable, IEEPROMvariable
    {
        public GSM_setEMAIL()
            : base()
        {

        }

        public GSM_setEMAIL(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "EMAIL send mode";
        }

        public UInt32 ID
        {
            get
            {
                return 72;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_setEMAIL_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "disabled");
                    _optionList.Add(1, "every X (1-24) hours");
                    _optionList.Add(2, "every day at hour X (0-23)");
                    _optionList.Add(3, "every X (1-7) day of week");
                    _optionList.Add(4, "every X (1-28) day of month");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class GSM_setEMAIL_ATTACH : WritableEnumTargetVariable, IEEPROMvariable
    {
        public GSM_setEMAIL_ATTACH()
            : base()
        {

        }

        public GSM_setEMAIL_ATTACH(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "EMAIL with attachment send mode";
        }

        public UInt32 ID
        {
            get
            {
                return 73;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_setEMAIL_ATTACH_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "disabled");
                    _optionList.Add(1, "every X (1-24) hours");
                    _optionList.Add(2, "every day at hour X (0-23)");
                    _optionList.Add(3, "every X (1-7) day of week");
                    _optionList.Add(4, "every X (1-28) day of month");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class GSM_setFTP_ATTACH : WritableEnumTargetVariable, IEEPROMvariable
    {
        public GSM_setFTP_ATTACH()
            : base()
        {

        }

        public GSM_setFTP_ATTACH(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "FTP with attachment send mode";
        }

        public UInt32 ID
        {
            get
            {
                return 74;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_setFTP_ATTACH_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "disabled");
                    _optionList.Add(1, "every X (1-24) hours");
                    _optionList.Add(2, "every day at hour X (0-23)");
                    _optionList.Add(3, "every X (1-7) day of week");
                    _optionList.Add(4, "every X (1-28) day of month");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class GSM_roaming : WritableEnumTargetVariable, IEEPROMvariable
    {
        public GSM_roaming()
            : base()
        {

        }

        public GSM_roaming(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "data roaming enable ";
        }

        public UInt32 ID
        {
            get
            {
                return 89;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_roaming_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "roaming disabled");
                    _optionList.Add(1, "roaming enabled");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class SPECIAL_VISUAL : WritableEnumTargetVariable, IEEPROMvariable
    {
        public SPECIAL_VISUAL()
            : base()
        {

        }

        public SPECIAL_VISUAL(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "special visualization options";
        }

        public UInt32 ID
        {
            get
            {
                return 25;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.SPECIAL_VISUAL_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "default");
                    _optionList.Add(1, "scroll T+ only");
                    _optionList.Add(2, "scroll T NET only - scroll flow only");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class PRODUCT_VARIANT : WritableEnumTargetVariable, IEEPROMvariable
    {
        public PRODUCT_VARIANT()
            : base()
        {

        }

        public PRODUCT_VARIANT(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "product variant info";
        }

        public UInt32 ID
        {
            get
            {
                return 57;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PRODUCT_VARIANT_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "standard");
                    _optionList.Add(1, "MC4061 pulses only");
                    _optionList.Add(2, "MC4061_full");
                    _optionList.Add(3, "MC4062_full");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class MEAS_FOR_4_20_MA : WritableEnumTargetVariable, IEEPROMvariable
    {
        public MEAS_FOR_4_20_MA()
            : base()
        {

        }

        public MEAS_FOR_4_20_MA(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "measurement to output 4-20mA";
        }

        public UInt32 ID
        {
            get
            {
                return 28;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MEAS_FOR_4_20_MA_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "map flow to 4-20mA output");
                    _optionList.Add(1, "map FS percentage to 4-20mA output");
                    _optionList.Add(2, "map press to 4-20mA output");
                    _optionList.Add(3, "map temperature1 to 4-20mA output");
                    _optionList.Add(4, "map temperature2 to 4-20mA output");
                    _optionList.Add(5, "disabled out 4-20mA");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class NEG_PULSE_OUTPUT_MODE : WritableEnumTargetVariable, IEEPROMvariable
    {
        public NEG_PULSE_OUTPUT_MODE()
            : base()
        {

        }

        public NEG_PULSE_OUTPUT_MODE(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "mode of pulse output (PN or only PP)";
        }

        public UInt32 ID
        {
            get
            {
                return 40;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.NEG_PULSE_OUTPUT_MODE_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "negative pulse output");
                    _optionList.Add(1, "positive pulse output");
                    _optionList.Add(2, "error output");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class ENABLE_BOARD_BT_RS485 : WritableEnumTargetVariable, IEEPROMvariable
    {
        public ENABLE_BOARD_BT_RS485()
            : base()
        {

        }

        public ENABLE_BOARD_BT_RS485(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "flags to enable BT, RS485 settings";
        }

        public UInt32 ID
        {
            get
            {
                return 96;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.ENABLE_BOARD_BT_RS485_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "all disabled");
                    _optionList.Add(1, "bt enabled");
                    _optionList.Add(2, "rs485 enabled");
                    _optionList.Add(3, "all enabled");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class BT_ON_INTERVAL : WritableEnumTargetVariable, IEEPROMvariable
    {
        public BT_ON_INTERVAL()
            : base()
        {

        }

        public BT_ON_INTERVAL(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "bluetooth on interval";
        }

        public UInt32 ID
        {
            get
            {
                return 97;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.BT_ON_INTERVAL_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "always off");
                    _optionList.Add(1, "always on");
                    _optionList.Add(2, "on if MC406 awake");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class RS485_ON_INTERVAL : WritableEnumTargetVariable, IEEPROMvariable
    {
        public RS485_ON_INTERVAL()
            : base()
        {

        }

        public RS485_ON_INTERVAL(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "rs485on interval";
        }

        public UInt32 ID
        {
            get
            {
                return 98;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_ON_INTERVAL_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "always off");
                    _optionList.Add(1, "on every 5 sec");
                    _optionList.Add(2, "on every 10 sec");
                    _optionList.Add(3, "on every 15 sec");
                    _optionList.Add(4, "on every 30 sec");
                    _optionList.Add(5, "on every 1 min");
                    _optionList.Add(6, "on every 2 min");
                    _optionList.Add(7, "on every 5 min");
                    _optionList.Add(8, "on every 10 min");
                    _optionList.Add(9, "on every 15 min");
                    _optionList.Add(10, "on every 30 min");
                    _optionList.Add(11, "on every 60 min");
                    _optionList.Add(12, "on every 4h");
                    _optionList.Add(13, "on every 8h");
                    _optionList.Add(14, "on every 12h");
                    _optionList.Add(15, "on every 24h");
                    _optionList.Add(16, "always on");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class RS485_BAUDRATE : WritableEnumTargetVariable, IEEPROMvariable
    {
        public RS485_BAUDRATE()
            : base()
        {

        }

        public RS485_BAUDRATE(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "rs485 baudrate";
        }

        public UInt32 ID
        {
            get
            {
                return 99;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_BAUDRATE_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "2400");
                    _optionList.Add(1, "4800");
                    _optionList.Add(2, "9600");
                    _optionList.Add(3, "19200");
                    _optionList.Add(4, "38400");
                    _optionList.Add(5, "57600");
                    _optionList.Add(6, "115200");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class RS485_DATA_NUM_BIT : WritableEnumTargetVariable, IEEPROMvariable
    {
        public RS485_DATA_NUM_BIT()
            : base()
        {

        }

        public RS485_DATA_NUM_BIT(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "rs485 data num bit";
        }

        public UInt32 ID
        {
            get
            {
                return 100;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_DATA_NUM_BIT_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "8 bit data");
                    _optionList.Add(1, "7 bit data");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class RS485_PARITY : WritableEnumTargetVariable, IEEPROMvariable
    {
        public RS485_PARITY()
            : base()
        {

        }

        public RS485_PARITY(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "rs485 parity";
        }

        public UInt32 ID
        {
            get
            {
                return 101;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_PARITY_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "parity none");
                    _optionList.Add(1, "parity odd");
                    _optionList.Add(2, "parity even");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class RS485_STOP_BITS : WritableEnumTargetVariable, IEEPROMvariable
    {
        public RS485_STOP_BITS()
            : base()
        {

        }

        public RS485_STOP_BITS(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "rs485 stop bits";
        }

        public UInt32 ID
        {
            get
            {
                return 102;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_STOP_BITS_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "1 bit stop");
                    _optionList.Add(1, "2 bit stop");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class RS485_MODBUS_MODE : WritableEnumTargetVariable, IEEPROMvariable
    {
        public RS485_MODBUS_MODE()
            : base()
        {

        }

        public RS485_MODBUS_MODE(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "rs485 modbus mode";
        }

        public UInt32 ID
        {
            get
            {
                return 103;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_MODBUS_MODE_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "modbus slave");
                    _optionList.Add(1, "modbus master");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class RS485_MODUBUS_BYTE_ORDER : WritableEnumTargetVariable, IEEPROMvariable
    {
        public RS485_MODUBUS_BYTE_ORDER()
            : base()
        {

        }

        public RS485_MODUBUS_BYTE_ORDER(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "modbus byte order";
        }

        public UInt32 ID
        {
            get
            {
                return 105;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_MODUBUS_BYTE_ORDER_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "little endian");
                    _optionList.Add(1, "big endian");
                    _optionList.Add(2, "little endian byte swap");
                    _optionList.Add(3, "big endian byte swap");
                    _optionList.Add(4, "custom 1");
                    _optionList.Add(5, "custom 2");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    public class RS485_MODUBUS_MAP : WritableEnumTargetVariable, IEEPROMvariable
    {
        public RS485_MODUBUS_MAP()
            : base()
        {

        }

        public RS485_MODUBUS_MAP(Byte val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "modbus map";
        }

        public UInt32 ID
        {
            get
            {
                return 108;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.RS485_MODUBUS_MAP_ADDR; }
        }

        public override IDictionary<int, String> Options
        {
            get 
            { 
                if (_optionList == null)
                {
                    _optionList = new Dictionary<int, String>();
                    _optionList.Add(0, "euromag modbus map");
                    _optionList.Add(1, "sofrel modbus map");
                }
                return _optionList; 
            }
        }

        private Dictionary<int, String> _optionList;

    }

    #endregion Enumerator EEPROM vars

    #region UInt16 EEPROM vars

    public class PULSE_VOL : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public PULSE_VOL()
            : base()
        {

        }

        public PULSE_VOL(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "pulse volume";
        }

        public UInt32 ID
        {
            get
            {
                return 41;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PULSE_VOL_ADDR; }
        }

    }

    public class CUTOFF : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public CUTOFF()
            : base()
        {

        }

        public CUTOFF(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "flowrate cut-off filter [m/s / 100]";
        }

        public UInt32 ID
        {
            get
            {
                return 120;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CUTOFF_ADDR; }
        }

    }

    public class DIAMETER : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public DIAMETER()
            : base()
        {

        }

        public DIAMETER(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "sensor nominal internal diameter [mm]";
        }

        public UInt32 ID
        {
            get
            {
                return 115;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DIAMETER_ADDR; }
        }

    }

    public class PULSE_LENGTH : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public PULSE_LENGTH()
            : base()
        {

        }

        public PULSE_LENGTH(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "pulse \"ON\" width [ms]";
        }

        public UInt32 ID
        {
            get
            {
                return 1;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PULSE_LENGTH_ADDR; }
        }

    }

    public class DAMPING : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public DAMPING()
            : base()
        {

        }

        public DAMPING(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "average count (flow measure samples)";
        }

        public UInt32 ID
        {
            get
            {
                return 123;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DAMPING_ADDR; }
        }

    }

    public class EPIPE_TH : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public EPIPE_TH()
            : base()
        {

        }

        public EPIPE_TH(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "empty pipe detection threshold (on the 4th electrode)";
        }

        public UInt32 ID
        {
            get
            {
                return 128;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.EPIPE_TH_ADDR; }
        }

    }

    public class PWD_TIMEOUT : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public PWD_TIMEOUT()
            : base()
        {

        }

        public PWD_TIMEOUT(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "password expiry timout [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 20;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PWD_TIMEOUT_ADDR; }
        }

    }

    public class BATT_AUTOSAVE : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public BATT_AUTOSAVE()
            : base()
        {

        }

        public BATT_AUTOSAVE(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "energy drain and residual battery energy eeprom update [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 21;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.BATT_AUTOSAVE_ADDR; }
        }

    }

    public class EPIPE_FREQ : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public EPIPE_FREQ()
            : base()
        {

        }

        public EPIPE_FREQ(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "empty pipe measurment period during full-pipe condition (4th electrode) [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 139;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.EPIPE_FREQ_ADDR; }
        }

    }

    public class MEAS_WD_TH : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public MEAS_WD_TH()
            : base()
        {

        }

        public MEAS_WD_TH(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "dry condition detect threshold (measuring electrodes)";
        }

        public UInt32 ID
        {
            get
            {
                return 142;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MEAS_WD_TH_ADDR; }
        }

    }

    public class TIMEOUT_TO_MAIN : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public TIMEOUT_TO_MAIN()
            : base()
        {

        }

        public TIMEOUT_TO_MAIN(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "automatic return to the main screen timeout [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 22;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.TIMEOUT_TO_MAIN_ADDR; }
        }

    }

    public class MEAS_AWAKE_MS : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public MEAS_AWAKE_MS()
            : base()
        {

        }

        public MEAS_AWAKE_MS(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "continuous mode sampling period [ms]";
        }

        public UInt32 ID
        {
            get
            {
                return 145;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MEAS_AWAKE_MS_ADDR; }
        }

    }

    public class INPUT_STAGE_STAB : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public INPUT_STAGE_STAB()
            : base()
        {

        }

        public INPUT_STAGE_STAB(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "input stage stabilization time [ms]";
        }

        public UInt32 ID
        {
            get
            {
                return 147;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INPUT_STAGE_STAB_ADDR; }
        }

    }

    public class DAMPING_SLOW : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public DAMPING_SLOW()
            : base()
        {

        }

        public DAMPING_SLOW(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "average count when sleeping (flow measure samples)";
        }

        public UInt32 ID
        {
            get
            {
                return 148;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DAMPING_SLOW_ADDR; }
        }

    }

    public class GSM_SimPin : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public GSM_SimPin()
            : base()
        {

        }

        public GSM_SimPin(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "SIM card PIN code";
        }

        public UInt32 ID
        {
            get
            {
                return 70;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_SimPin_ADDR; }
        }

    }

    public class MAIN_PWR_INTERR_CNT : WritableUInt16TargetVariable, IEEPROMvariable
    {
        public MAIN_PWR_INTERR_CNT()
            : base()
        {

        }

        public MAIN_PWR_INTERR_CNT(UInt16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "Main power interruption counter";
        }

        public UInt32 ID
        {
            get
            {
                return 109;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MAIN_PWR_INTERR_CNT_ADDR; }
        }

    }

    #endregion UInt16 EEPROM vars

    #region Float EEPROM vars

    public class SENSOR_OFFSET : WritableFloatTargetVariable, IEEPROMvariable
    {
        public SENSOR_OFFSET()
            : base()
        {

        }

        public SENSOR_OFFSET(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "sensor offset";
        }

        public UInt32 ID
        {
            get
            {
                return 116;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.SENSOR_OFFSET_ADDR; }
        }

    }

    public class KA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public KA()
            : base()
        {

        }

        public KA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "sensor calibration factor ";
        }

        public UInt32 ID
        {
            get
            {
                return 114;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.KA_ADDR; }
        }

    }

    public class SPEC_HEAT_sA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public SPEC_HEAT_sA()
            : base()
        {

        }

        public SPEC_HEAT_sA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "specific heat A(x^3) interpolation coefficient";
        }

        public UInt32 ID
        {
            get
            {
                return 3;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.SPEC_HEAT_sA_ADDR; }
        }

    }

    public class SPEC_HEAT_sB : WritableFloatTargetVariable, IEEPROMvariable
    {
        public SPEC_HEAT_sB()
            : base()
        {

        }

        public SPEC_HEAT_sB(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "specific heat B(x^2) interpolation coefficient";
        }

        public UInt32 ID
        {
            get
            {
                return 4;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.SPEC_HEAT_sB_ADDR; }
        }

    }

    public class SPEC_HEAT_sC : WritableFloatTargetVariable, IEEPROMvariable
    {
        public SPEC_HEAT_sC()
            : base()
        {

        }

        public SPEC_HEAT_sC(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "specific heat Cx interpolation coefficient";
        }

        public UInt32 ID
        {
            get
            {
                return 5;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.SPEC_HEAT_sC_ADDR; }
        }

    }

    public class SPEC_HEAT_sD : WritableFloatTargetVariable, IEEPROMvariable
    {
        public SPEC_HEAT_sD()
            : base()
        {

        }

        public SPEC_HEAT_sD(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "specific heat D interpolation coefficient";
        }

        public UInt32 ID
        {
            get
            {
                return 6;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.SPEC_HEAT_sD_ADDR; }
        }

    }

    public class DENSITY_cA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public DENSITY_cA()
            : base()
        {

        }

        public DENSITY_cA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "density A(x^3) interpolation coefficient";
        }

        public UInt32 ID
        {
            get
            {
                return 7;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DENSITY_cA_ADDR; }
        }

    }

    public class DENSITY_cB : WritableFloatTargetVariable, IEEPROMvariable
    {
        public DENSITY_cB()
            : base()
        {

        }

        public DENSITY_cB(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "density B(x^2) interpolation coefficient";
        }

        public UInt32 ID
        {
            get
            {
                return 8;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DENSITY_cB_ADDR; }
        }

    }

    public class DENSITY_cC : WritableFloatTargetVariable, IEEPROMvariable
    {
        public DENSITY_cC()
            : base()
        {

        }

        public DENSITY_cC(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "density Cx interpolation coefficient";
        }

        public UInt32 ID
        {
            get
            {
                return 9;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DENSITY_cC_ADDR; }
        }

    }

    public class DENSITY_cD : WritableFloatTargetVariable, IEEPROMvariable
    {
        public DENSITY_cD()
            : base()
        {

        }

        public DENSITY_cD(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "density D interpolation coefficient";
        }

        public UInt32 ID
        {
            get
            {
                return 10;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DENSITY_cD_ADDR; }
        }

    }

    public class KALIGN : WritableFloatTargetVariable, IEEPROMvariable
    {
        public KALIGN()
            : base()
        {

        }

        public KALIGN(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "converter alignment calibration factor";
        }

        public UInt32 ID
        {
            get
            {
                return 118;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.KALIGN_ADDR; }
        }

    }

    public class OFFALIGN : WritableFloatTargetVariable, IEEPROMvariable
    {
        public OFFALIGN()
            : base()
        {

        }

        public OFFALIGN(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "converter alignment offset";
        }

        public UInt32 ID
        {
            get
            {
                return 119;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.OFFALIGN_ADDR; }
        }

    }

    public class INTERP_sA_LO : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_sA_LO()
            : base()
        {

        }

        public INTERP_sA_LO(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "A(x^3) interpolation coefficient - low velocity";
        }

        public UInt32 ID
        {
            get
            {
                return 150;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_sA_LO_ADDR; }
        }

    }

    public class INTERP_sB_LO : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_sB_LO()
            : base()
        {

        }

        public INTERP_sB_LO(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "B(x^2) interpolation coefficient - low velocity";
        }

        public UInt32 ID
        {
            get
            {
                return 151;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_sB_LO_ADDR; }
        }

    }

    public class INTERP_sC_LO : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_sC_LO()
            : base()
        {

        }

        public INTERP_sC_LO(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "Cx interpolation coefficient - low velocity";
        }

        public UInt32 ID
        {
            get
            {
                return 152;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_sC_LO_ADDR; }
        }

    }

    public class INTERP_sD_LO : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_sD_LO()
            : base()
        {

        }

        public INTERP_sD_LO(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "D interpolation coefficient - low velocity";
        }

        public UInt32 ID
        {
            get
            {
                return 153;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_sD_LO_ADDR; }
        }

    }

    public class INTERP_sA_HI : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_sA_HI()
            : base()
        {

        }

        public INTERP_sA_HI(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "A(x^3) interpolation coefficient - high velocity";
        }

        public UInt32 ID
        {
            get
            {
                return 154;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_sA_HI_ADDR; }
        }

    }

    public class INTERP_sB_HI : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_sB_HI()
            : base()
        {

        }

        public INTERP_sB_HI(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "B(x^2) interpolation coefficient - high velocity";
        }

        public UInt32 ID
        {
            get
            {
                return 155;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_sB_HI_ADDR; }
        }

    }

    public class INTERP_sC_HI : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_sC_HI()
            : base()
        {

        }

        public INTERP_sC_HI(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "Cx interpolation coefficient - high velocity";
        }

        public UInt32 ID
        {
            get
            {
                return 156;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_sC_HI_ADDR; }
        }

    }

    public class INTERP_sD_HI : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_sD_HI()
            : base()
        {

        }

        public INTERP_sD_HI(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "D interpolation coefficient - high velocity";
        }

        public UInt32 ID
        {
            get
            {
                return 157;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_sD_HI_ADDR; }
        }

    }

    public class INTERP_TH : WritableFloatTargetVariable, IEEPROMvariable
    {
        public INTERP_TH()
            : base()
        {

        }

        public INTERP_TH(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "interpolation between low and high curves";
        }

        public UInt32 ID
        {
            get
            {
                return 158;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.INTERP_TH_ADDR; }
        }

    }

    public class PRESS_PROBE_CAL : WritableFloatTargetVariable, IEEPROMvariable
    {
        public PRESS_PROBE_CAL()
            : base()
        {

        }

        public PRESS_PROBE_CAL(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "pressure probe calibration factor";
        }

        public UInt32 ID
        {
            get
            {
                return 23;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PRESS_PROBE_CAL_ADDR; }
        }

    }

    public class PRESS_CURRENT_CAL : WritableFloatTargetVariable, IEEPROMvariable
    {
        public PRESS_CURRENT_CAL()
            : base()
        {

        }

        public PRESS_CURRENT_CAL(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "pressure current feedback calibration factor";
        }

        public UInt32 ID
        {
            get
            {
                return 24;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PRESS_CURRENT_CAL_ADDR; }
        }

    }

    public class PRESS_PROBE_CAL_OFFSET : WritableFloatTargetVariable, IEEPROMvariable
    {
        public PRESS_PROBE_CAL_OFFSET()
            : base()
        {

        }

        public PRESS_PROBE_CAL_OFFSET(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "calibration offset for pressure probe";
        }

        public UInt32 ID
        {
            get
            {
                return 32;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PRESS_PROBE_CAL_OFFSET_ADDR; }
        }

    }

    public class CAL_LEV_4MA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public CAL_LEV_4MA()
            : base()
        {

        }

        public CAL_LEV_4MA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "cal level for output 4 mA";
        }

        public UInt32 ID
        {
            get
            {
                return 26;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CAL_LEV_4MA_ADDR; }
        }

    }

    public class CAL_LEV_20MA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public CAL_LEV_20MA()
            : base()
        {

        }

        public CAL_LEV_20MA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "cal level for output 20 mA";
        }

        public UInt32 ID
        {
            get
            {
                return 27;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CAL_LEV_20MA_ADDR; }
        }

    }

    public class VEL_LEV_FOR_4MA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public VEL_LEV_FOR_4MA()
            : base()
        {

        }

        public VEL_LEV_FOR_4MA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "velocity level for an 4 mA output";
        }

        public UInt32 ID
        {
            get
            {
                return 30;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.VEL_LEV_FOR_4MA_ADDR; }
        }

    }

    public class VEL_LEV_FOR_20MA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public VEL_LEV_FOR_20MA()
            : base()
        {

        }

        public VEL_LEV_FOR_20MA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "velocity level for an 20 mA output";
        }

        public UInt32 ID
        {
            get
            {
                return 31;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.VEL_LEV_FOR_20MA_ADDR; }
        }

    }

    public class REV_FLOW_ERR_VAL_4_20MA_OUT : WritableFloatTargetVariable, IEEPROMvariable
    {
        public REV_FLOW_ERR_VAL_4_20MA_OUT()
            : base()
        {

        }

        public REV_FLOW_ERR_VAL_4_20MA_OUT(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "current level to indicate  rev flow error";
        }

        public UInt32 ID
        {
            get
            {
                return 34;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.REV_FLOW_ERR_VAL_4_20MA_OUT_ADDR; }
        }

    }

    public class EPIPE_ERR_VAL_4_20MA_OUT : WritableFloatTargetVariable, IEEPROMvariable
    {
        public EPIPE_ERR_VAL_4_20MA_OUT()
            : base()
        {

        }

        public EPIPE_ERR_VAL_4_20MA_OUT(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "current output on epipe detection";
        }

        public UInt32 ID
        {
            get
            {
                return 35;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.EPIPE_ERR_VAL_4_20MA_OUT_ADDR; }
        }

    }

    public class COIL_ERR_VAL_4_20MA_OUT : WritableFloatTargetVariable, IEEPROMvariable
    {
        public COIL_ERR_VAL_4_20MA_OUT()
            : base()
        {

        }

        public COIL_ERR_VAL_4_20MA_OUT(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "current output on coil error";
        }

        public UInt32 ID
        {
            get
            {
                return 36;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.COIL_ERR_VAL_4_20MA_OUT_ADDR; }
        }

    }

    public class GEN_ERR_VAL_4_20MA_OUT : WritableFloatTargetVariable, IEEPROMvariable
    {
        public GEN_ERR_VAL_4_20MA_OUT()
            : base()
        {

        }

        public GEN_ERR_VAL_4_20MA_OUT(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "current output on error";
        }

        public UInt32 ID
        {
            get
            {
                return 37;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GEN_ERR_VAL_4_20MA_OUT_ADDR; }
        }

    }

    public class OUT_LOWER_LIMIT_4_20MA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public OUT_LOWER_LIMIT_4_20MA()
            : base()
        {

        }

        public OUT_LOWER_LIMIT_4_20MA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "output current lower limit";
        }

        public UInt32 ID
        {
            get
            {
                return 38;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.OUT_LOWER_LIMIT_4_20MA_ADDR; }
        }

    }

    public class OUT_UPPER_LIMIT_4_20MA : WritableFloatTargetVariable, IEEPROMvariable
    {
        public OUT_UPPER_LIMIT_4_20MA()
            : base()
        {

        }

        public OUT_UPPER_LIMIT_4_20MA(float val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "output current upper limit";
        }

        public UInt32 ID
        {
            get
            {
                return 39;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.OUT_UPPER_LIMIT_4_20MA_ADDR; }
        }

    }

    #endregion Float EEPROM vars

    #region Int16 EEPROM vars

    public class OFFSET_TEMP : WritableInt16TargetVariable, IEEPROMvariable
    {
        public OFFSET_TEMP()
            : base()
        {

        }

        public OFFSET_TEMP(Int16 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "internal temperature offset [C]";
        }

        public UInt32 ID
        {
            get
            {
                return 2;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.OFFSET_TEMP_ADDR; }
        }

    }

    #endregion Int16 EEPROM vars

    #region String EEPROM vars

    public class SENSOR_MODEL : WritableStringTargetVariable, IEEPROMvariable
    {
        public SENSOR_MODEL()
            : base()
        {

        }

        public SENSOR_MODEL(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "coupled sensor model";
        }

        public UInt32 ID
        {
            get
            {
                return 48;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 12;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.SENSOR_MODEL_ADDR; }
        }

    }

    public class OTHER_FEAT : WritableStringTargetVariable, IEEPROMvariable
    {
        public OTHER_FEAT()
            : base()
        {

        }

        public OTHER_FEAT(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "additional features";
        }

        public UInt32 ID
        {
            get
            {
                return 49;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 20;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.OTHER_FEAT_ADDR; }
        }

    }

    public class CONV_ID : WritableStringTargetVariable, IEEPROMvariable
    {
        public CONV_ID()
            : base()
        {

        }

        public CONV_ID(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "converter ID";
        }

        public UInt32 ID
        {
            get
            {
                return 50;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 9;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CONV_ID_ADDR; }
        }

    }

    public class SENSOR_ID : WritableStringTargetVariable, IEEPROMvariable
    {
        public SENSOR_ID()
            : base()
        {

        }

        public SENSOR_ID(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "coupled sensor ID";
        }

        public UInt32 ID
        {
            get
            {
                return 51;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 9;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.SENSOR_ID_ADDR; }
        }

    }

    public class CAL_DATE : WritableStringTargetVariable, IEEPROMvariable
    {
        public CAL_DATE()
            : base()
        {

        }

        public CAL_DATE(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "meter calibration date";
        }

        public UInt32 ID
        {
            get
            {
                return 53;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 8;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CAL_DATE_ADDR; }
        }

    }

    public class DEV_NAME : WritableStringTargetVariable, IEEPROMvariable
    {
        public DEV_NAME()
            : base()
        {

        }

        public DEV_NAME(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "converter model";
        }

        public UInt32 ID
        {
            get
            {
                return 54;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 8;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.DEV_NAME_ADDR; }
        }

    }

    public class MANUFACTURER : WritableStringTargetVariable, IEEPROMvariable
    {
        public MANUFACTURER()
            : base()
        {

        }

        public MANUFACTURER(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "manufacturing company";
        }

        public UInt32 ID
        {
            get
            {
                return 55;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 10;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MANUFACTURER_ADDR; }
        }

    }

    public class CUSTIMIZED_CONV_ID : WritableStringTargetVariable, IEEPROMvariable
    {
        public CUSTIMIZED_CONV_ID()
            : base()
        {

        }

        public CUSTIMIZED_CONV_ID(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "converter id for customized devices";
        }

        public UInt32 ID
        {
            get
            {
                return 91;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 24;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CUSTIMIZED_CONV_ID_ADDR; }
        }

    }

    public class CUSTIMIZED_SENSOR_ID : WritableStringTargetVariable, IEEPROMvariable
    {
        public CUSTIMIZED_SENSOR_ID()
            : base()
        {

        }

        public CUSTIMIZED_SENSOR_ID(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "sensor id for customized devices";
        }

        public UInt32 ID
        {
            get
            {
                return 92;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 24;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CUSTIMIZED_SENSOR_ID_ADDR; }
        }

    }

    public class CUSTOMIZED_SENSOR_MODEL : WritableStringTargetVariable, IEEPROMvariable
    {
        public CUSTOMIZED_SENSOR_MODEL()
            : base()
        {

        }

        public CUSTOMIZED_SENSOR_MODEL(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "sensor model for customized devices";
        }

        public UInt32 ID
        {
            get
            {
                return 93;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 24;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CUSTOMIZED_SENSOR_MODEL_ADDR; }
        }

    }

    public class CUSTOMIZED_DEV_NAME : WritableStringTargetVariable, IEEPROMvariable
    {
        public CUSTOMIZED_DEV_NAME()
            : base()
        {

        }

        public CUSTOMIZED_DEV_NAME(String val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "dev name for customized devices";
        }

        public UInt32 ID
        {
            get
            {
                return 94;
            }
        }

        public override Int32 Size
        {
            get
            {
                return 24;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CUSTOMIZED_DEV_NAME_ADDR; }
        }

    }

    #endregion String EEPROM vars

    #region UInt32 EEPROM vars

    public class CONV_SN : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public CONV_SN()
            : base()
        {

        }

        public CONV_SN(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "converter serial number";
        }

        public UInt32 ID
        {
            get
            {
                return 52;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.CONV_SN_ADDR; }
        }

    }

    public class PASSWORD : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public PASSWORD()
            : base()
        {

        }

        public PASSWORD(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "menu access password";
        }

        public UInt32 ID
        {
            get
            {
                return 58;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.PASSWORD_ADDR; }
        }

    }

    public class BASE_SEC : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public BASE_SEC()
            : base()
        {

        }

        public BASE_SEC(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "total operation time in low power mode [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 59;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.BASE_SEC_ADDR; }
        }

    }

    public class MCOUNT : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public MCOUNT()
            : base()
        {

        }

        public MCOUNT(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "total number of measure samples aquired ";
        }

        public UInt32 ID
        {
            get
            {
                return 60;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MCOUNT_ADDR; }
        }

    }

    public class uAh_TOT : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public uAh_TOT()
            : base()
        {

        }

        public uAh_TOT(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "nominal battery pack capacity [uAh]";
        }

        public UInt32 ID
        {
            get
            {
                return 61;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.uAh_TOT_ADDR; }
        }

    }

    public class uAh_LEFT : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public uAh_LEFT()
            : base()
        {

        }

        public uAh_LEFT(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "residual battery pack energy [uAh]";
        }

        public UInt32 ID
        {
            get
            {
                return 62;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.uAh_LEFT_ADDR; }
        }

    }

    public class AWAKE_SEC : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public AWAKE_SEC()
            : base()
        {

        }

        public AWAKE_SEC(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "total operation time in awake mode [s]";
        }

        public UInt32 ID
        {
            get
            {
                return 63;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.AWAKE_SEC_ADDR; }
        }

    }

    public class GSM_max_consecutive_rows : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public GSM_max_consecutive_rows()
            : base()
        {

        }

        public GSM_max_consecutive_rows(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "max number of consecutive rows transmittable during a connection";
        }

        public UInt32 ID
        {
            get
            {
                return 77;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.GSM_max_consecutive_rows_ADDR; }
        }

    }

    public class MAIN_PWR_INTERR_TOT_SEC : WritableUInt32TargetVariable, IEEPROMvariable
    {
        public MAIN_PWR_INTERR_TOT_SEC()
            : base()
        {

        }

        public MAIN_PWR_INTERR_TOT_SEC(UInt32 val)
            : base(val)
        {

        }

        public override string ToString()
        {
            return "Main power interruption total seconds";
        }

        public UInt32 ID
        {
            get
            {
                return 110;
            }
        }

        public EEPROMAddresses Address
        {
            get
            { return EEPROMAddresses.MAIN_PWR_INTERR_TOT_SEC_ADDR; }
        }

    }

    #endregion UInt32 EEPROM vars

    #endregion EEPROM Variables
}

