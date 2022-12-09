using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;  // per comunicazione IrDA e COM

namespace MC_Suite.Modbus
{
    public partial class Map
    {

        public static bool Reverse_mdb = false; // registri letture EEPROM
        public static bool error_mdb = false;
        public static byte Slave_ID = 0;
        public const byte CMD3  = 3;
        public const byte CMD4  = 4;
        public const byte CMD16 = 16;

        public class Registri_CMD4
        {
            public static Float32_mdb Measure_m_s = new Float32_mdb(00, CMD4);
            public static Float32_mdb Measure_m3_s = new Float32_mdb(02, CMD4);
            public static Float32_mdb Measure_kg_s = new Float32_mdb(04, CMD4);
            public static Float32_mdb TPAcc_m3 = new Float32_mdb(06, CMD4);
            public static Float32_mdb TNAcc_m3 = new Float32_mdb(08, CMD4);
            public static Float32_mdb PPAcc_m3 = new Float32_mdb(10, CMD4);
            public static Float32_mdb PNAcc_m3 = new Float32_mdb(12, CMD4);
            public static Float32_mdb PesoSpec_Kg_m3 = new Float32_mdb(14, CMD4);
            public static Float32_mdb TPAcc_kg = new Float32_mdb(16, CMD4);
            public static Float32_mdb TNAcc_kg = new Float32_mdb(18, CMD4);
            public static Float32_mdb PPAcc_kg = new Float32_mdb(20, CMD4);
            public static Float32_mdb PNAcc_kg = new Float32_mdb(22, CMD4);
            public static Registro_mdb VflowUT = new Registro_mdb(24, CMD4);
            public static Registro_mdb TflowUT = new Registro_mdb(25, CMD4);
            public static Float32_mdb MeasureScaled = new Float32_mdb(26, CMD4);
            public static Registro_mdb VaccUT = new Registro_mdb(28, CMD4);
            public static Float32_mdb TPAccScaled = new Float32_mdb(29, CMD4);
            public static Float32_mdb TNAccScaled = new Float32_mdb(31, CMD4);
            public static Float32_mdb PPAccScaled = new Float32_mdb(33, CMD4);
            public static Float32_mdb PNAccScaled = new Float32_mdb(35, CMD4);
            public static Float32_mdb PQtyMain_m3 = new Float32_mdb(37, CMD4);
            public static Registro_mdb VpulseUT = new Registro_mdb(39, CMD4);
            public static Float32_mdb PQtyMain_Scaled = new Float32_mdb(40, CMD4);
            public static Float32_mdb TempPcbScaled = new Float32_mdb(42, CMD4);
            public static Float32_mdb VoltMainScaled = new Float32_mdb(44, CMD4);
            public static Registro_mdb GetPowerMode = new Registro_mdb(46, CMD4);
            public static Registro_mdb YearMem = new Registro_mdb(47, CMD4);
            public static Registro_mdb MonthMem = new Registro_mdb(48, CMD4);
            public static Registro_mdb DayMem = new Registro_mdb(49, CMD4);
            public static Registro_mdb HourMem = new Registro_mdb(50, CMD4);
            public static Registro_mdb MinuteMem = new Registro_mdb(51, CMD4);
            public static Registro_mdb MenuPos = new Registro_mdb(52, CMD4);
            public static Float32_mdb fullscale_scalato = new Float32_mdb(53, CMD4);
            public static Float32_mdb Portata_perc = new Float32_mdb(55, CMD4);
            public static Float32_mdb EXP_temp = new Float32_mdb(57, CMD4);
            public static Int32_mdb Last_Row_Datalog = new Int32_mdb(60, CMD4);
            public static Float32_mdb DCDCRead = new Float32_mdb(62, CMD4);
            public static Float32_mdb ExpPression = new Float32_mdb(64, CMD4);

            public static Registro_mdb ErrorState = new Registro_mdb(69, CMD4);
            public static Registro_mdb Digital_IO = new Registro_mdb(70, CMD4);
            public static Registro_mdb Simulazione = new Registro_mdb(71, CMD4);
            public static Registro_mdb DAC_SS1_KALIGN = new Registro_mdb(72, CMD4);
            public static Registro_mdb res_Offset_KA_Align = new Registro_mdb(73, CMD4);
            public static Registro_mdb ICoilSet_EXT = new Registro_mdb(74, CMD4);
            public static Registro_mdb res_ActivateCoil = new Registro_mdb(75, CMD4);
            public static Registro_mdb res_EmptyPipeCk = new Registro_mdb(76, CMD4);
            public static Float32_mdb res_AlignOffset = new Float32_mdb(77, CMD4);
            public static Registro_mdb GetEEPromChange = new Registro_mdb(79, CMD4); // BM 26.09.2013
        }

        public class Registri_CMD3_16
        {
            public static Registro_mdb Release_ModBus   = new Registro_mdb(1000, CMD3);
            public static Registro_mdb Release_FW       = new Registro_mdb(1001, CMD3);
            public static Registro_mdb Release_HW       = new Registro_mdb(1002, CMD3);
            public static Registro_mdb RS485_BaudRate   = new Registro_mdb(1003, CMD3);
            public static Registro_mdb ID_Device        = new Registro_mdb(1004, CMD3);
            public static Registro_mdb Reverse_Modbus   = new Registro_mdb(1005, CMD3);
            
            //public static Registro_mdb Free_1006 = new Registro_mdb(1006, 3);  // LIBERO ************* Futuro tipo protocollo

            public static Registro_mdb ModbusConnection = new Registro_mdb(1007, CMD3);
            public static Registro_mdb GetIrVersion     = new Registro_mdb(1008, CMD3);  // 13.02.2012
            public static Registro_mdb power_mode       = new Registro_mdb(1009, CMD3);
            public static Registro_mdb AutoPowerOffMem  = new Registro_mdb(1010, CMD3);
            public static Registro_mdb MeasSpaceSet     = new Registro_mdb(1011, CMD3);
            public static Registro_mdb MeasSpaceCont    = new Registro_mdb(1012, CMD3);
            public static Registro_mdb MeasSpaceWakeup  = new Registro_mdb(1013, CMD3);
            public static Stringa_mdb Produttore       = new Stringa_mdb(1014, 8,CMD3);
            public static Registro_mdb Logo             = new Registro_mdb(1018, CMD3); 
            public static Stringa_mdb Convertitore     = new Stringa_mdb(1019, 8,CMD3);
            public static Stringa_mdb Convert_matricola= new Stringa_mdb(1023, 9,CMD3);
            public static Int32_mdb    Serial_convert   = new Int32_mdb(1028,CMD3);
            public static Stringa_mdb Sensore          = new Stringa_mdb(1030, 12,CMD3);
            public static Registro_mdb GeneralEEpromLoad= new Registro_mdb(1035, CMD3); 
            public static Stringa_mdb Sensore_matricola= new Stringa_mdb(1036, 9,CMD3);
            public static Registro_mdb Diametro_sensore = new Registro_mdb(1041, CMD3);
            public static Registro_mdb Tubo_Vuoto        = new Registro_mdb(1042, CMD3);

            //public static Registro_mdb Free_1043 = new Registro_mdb(1043, CMD3); // LIBERO ********** futuro pressione sensore

            public static Stringa_mdb Note             = new Stringa_mdb(1044, 20,CMD3);
            public static Float32_mdb FullScale_m3      = new Float32_mdb(1054, CMD3);
            public static Registro_mdb Ricaricabile_EN  = new Registro_mdb(1056, CMD3); 

            //public static Registro_mdb Free_1057 = new Registro_mdb(1057, CMD3); // LIBERO ************************************
            //public static Registro_mdb Free_1058 = new Registro_mdb(1058, CMD3); // LIBERO ************************************
            //public static Registro_mdb Free_1059 = new Registro_mdb(1059, CMD3); // LIBERO ************************************

            public static Registro_mdb Backlight_Perc   = new Registro_mdb(1060, CMD3);
            public static Registro_mdb Backlight_TimeOut= new Registro_mdb(1061, CMD3);
            public static Registro_mdb LcdContrast      = new Registro_mdb(1062, CMD3);
            public static Registro_mdb menu_language    = new Registro_mdb(1063, CMD3);
            public static Registro_mdb LastRowOption    = new Registro_mdb(1064, CMD3);
            public static Registro_mdb VflowUT          = new Registro_mdb(1065, CMD3);
            public static Registro_mdb TflowUT          = new Registro_mdb(1066, CMD3);
            public static Registro_mdb VaccUT           = new Registro_mdb(1067, CMD3);
            public static Registro_mdb PESO_SPEC_KGM3   = new Registro_mdb(1068, CMD3);
            public static Registro_mdb Temp_Unit        = new Registro_mdb(1069, CMD3); // LIBERO ************************************

            // USCITE IMPULSI

            public static Registro_mdb VpulseUT = new Registro_mdb(1070, CMD3);

            /// <summary>
            /// Reg 1071 in ml
            /// </summary>
            public static Float32_mdb PQtyMain        = new Float32_mdb(1071, CMD3);
            public static Registro_mdb PONmain_ms     = new Registro_mdb(1073, CMD3);
            public static Float32_mdb MaxPulsesFreq   = new Float32_mdb(1074, CMD3);
            public static Registro_mdb PulseFreqMode  = new Registro_mdb(1076, CMD3);
            public static Registro_mdb PULSES_NEG     = new Registro_mdb(1077, CMD3); // 1 = ON 0 = OFF

            //public static Registro_mdb Free_1078 = new Registro_mdb(1078, CMD3); // LIBERO ************************************
            //public static Registro_mdb Free_1079 = new Registro_mdb(1079, CMD3); // LIBERO ************************************

            // USCITE PROGRAMMABILI
            public static Registro_mdb DINsetup   = new Registro_mdb(1080, CMD3);
            public static Registro_mdb OUTsetup   = new Registro_mdb(1081, CMD3);
            public static Registro_mdb FS_FREQ    = new Registro_mdb(1082, CMD3);
            public static Registro_mdb OUT3_LOGIC = new Registro_mdb(1083, CMD3); // Inversione logica per tutte le funzioni

            // DOSAGGIO
            public static Float32_mdb BATCHING_VOLUME = new Float32_mdb(1084, CMD3);
            public static Registro_mdb BATCHING_UT    = new Registro_mdb(1086, CMD3);

            //public static Registro_mdb Free_1087 = new Registro_mdb(1087, CMD3); // LIBERO ************************************
            //public static Registro_mdb Free_1088 = new Registro_mdb(1088, CMD3); // LIBERO ************************************
            //public static Registro_mdb Free_1089 = new Registro_mdb(1089, CMD3); // LIBERO ************************************

            // ESPANSIONI
            public static Registro_mdb ExpSetup_1 = new Registro_mdb(1090, CMD3);
            public static Registro_mdb ExpSetup_2 = new Registro_mdb(1091, CMD3);
            public static Registro_mdb ExpSetup_3 = new Registro_mdb(1092, CMD3);
            public static Registro_mdb ExpSetup_4 = new Registro_mdb(1093, CMD3);
            public static Float32_mdb  ExpPar_1   = new Float32_mdb(1094, CMD3);

            public static Registro_mdb GSM_Function   = new Registro_mdb(1096, CMD3);
            //public static Registro_mdb Free_1097    = new Registro_mdb(1097, CMD3); // LIBERO ************************************
            //public static Registro_mdb Free_1098    = new Registro_mdb(1098, CMD3); // LIBERO ************************************
            public static Registro_mdb DumpingSec     = new Registro_mdb(1099, CMD3); // LIBERO ************************************

            // FILTRI
            public static Registro_mdb Damping        = new Registro_mdb(1100, CMD3);
            public static Registro_mdb CutOffPercent  = new Registro_mdb(1101, CMD3);
            public static Registro_mdb FilterBypassTh = new Registro_mdb(1102, CMD3);
            public static Registro_mdb PeakCutTh      = new Registro_mdb(1103, CMD3);
            public static Registro_mdb LineFreq       = new Registro_mdb(1104, CMD3); // frequenza Rete 0 = 50Hz, 1 = 60Hz;
            public static Registro_mdb AverageFilter  = new Registro_mdb(1105, CMD3);
            public static Registro_mdb BYPASS_REP_ON  = new Registro_mdb(1106, CMD3); 
            public static Registro_mdb PEAK_REP_ON    = new Registro_mdb(1107, CMD3); 
            public static Registro_mdb BYPASS_REP_LP  = new Registro_mdb(1108, CMD3); 
            public static Registro_mdb PEAK_REP_LP    = new Registro_mdb(1109, CMD3); 

            // ALLARMI
            public static Registro_mdb UP_FLOW_ALARM  = new Registro_mdb(1110, CMD3);
            public static Registro_mdb LOW_FLOW_ALARM = new Registro_mdb(1111, CMD3);

            public static Float32_mdb Set_FixedMode_4_20mA    = new Float32_mdb(1112, CMD3); // LIBERO ************************************ 
            public static Float32_mdb Set_AnalogOut_Hart      = new Float32_mdb(1114, CMD3); 
            public static Float32_mdb Send_AnalogRead_Hart    = new Float32_mdb(1116, CMD3);
            public static Registro_mdb Cmd_Cal_AnalogOut_Hart = new Registro_mdb(1118, CMD3);

            public static Registro_mdb GSM_SMS_ErrorCounter   = new Registro_mdb(1119, CMD3);
            // ERRORI
            public static Registro_mdb ALARMS_MEMORY = new Registro_mdb(1120, CMD3);

            public static Registro_mdb ResetTotP = new Registro_mdb(1121, CMD3); 
            public static Registro_mdb ResetTotN = new Registro_mdb(1122, CMD3);

            //public static Registro_mdb Free_1123 = new Registro_mdb(1123, CMD3); 

            public static Registro_mdb GSM_Email_ErrorCounter     = new Registro_mdb(1124, CMD3);
            public static Registro_mdb GSM_EmailA_ErrorCounter    = new Registro_mdb(1125, CMD3);
            public static Registro_mdb GSM_FTP_ErrorCounter       = new Registro_mdb(1126, CMD3);
            public static Registro_mdb GSM_ModemCom_ErrorCounter  = new Registro_mdb(1127, CMD3);
            public static Registro_mdb GSM_Network_ErrorCounter   = new Registro_mdb(1127, CMD3);

            //public static Registro_mdb Free_1128 = new Registro_mdb(1128, CMD3); // LIBERO ************************************
            public static Registro_mdb ResetMain = new Registro_mdb(1129, CMD3); // LIBERO ************************************

            // DATALOGER

            public static Registro_mdb DLoggerPage = new Registro_mdb(1130, CMD3); // Pagina_FlashInUso per MC608_5
            public static Registro_mdb LogInterval = new Registro_mdb(1131, CMD3);
            public static Registro_mdb MemoryModel = new Registro_mdb(1132, CMD3);

            //public static Registro_mdb Free_1133 = new Registro_mdb(1133, CMD3); // LIBERO ************************************
            //public static Registro_mdb Free_1134 = new Registro_mdb(1134, CMD3); // LIBERO ************************************
            //public static Registro_mdb Free_1135 = new Registro_mdb(1135, CMD3); // LIBERO ************************************

            // TARATURA
            public static Stringa_mdb DAT_CALIB      = new Stringa_mdb(1136, 8,CMD3);
            public static Registro_mdb CALIBR_TEMP    = new Registro_mdb(1140, CMD3);
            public static Registro_mdb CALIBR_VOLTAGE = new Registro_mdb(1141, CMD3);

            // 4-20mA Versione MC608_5 FW<3 
            public static Float32_mdb OFFSET_420  = new Float32_mdb(1142, CMD3); // Versione _5
            public static Float32_mdb KCORR_420   = new Float32_mdb(1144, CMD3); // Versione _5

            // 4-20mA Versione MC608_6 FW>3
            public static Registro_mdb LOW_420    = new Registro_mdb(1142, CMD3); //  4mA
            public static Registro_mdb MID_420    = new Registro_mdb(1143, CMD3); // 12mA
            public static Registro_mdb HI_420     = new Registro_mdb(1144, CMD3); // 20mA

            public static Registro_mdb OUT420_SETUP = new Registro_mdb(1146, CMD3);

            // Parametri BOBINE
            public static Registro_mdb COIL_ON_1_WAIT = new Registro_mdb(1147, CMD3);
            public static Registro_mdb COIL_ON_2_WAIT = new Registro_mdb(1148, CMD3);
            public static Registro_mdb COIL_OFF       = new Registro_mdb(1149, CMD3);
            public static Registro_mdb T_INV_WAIT     = new Registro_mdb(1150, CMD3);
            public static Registro_mdb RES_COIL       = new Registro_mdb(1151, CMD3);
            public static Registro_mdb FilterADC      = new Registro_mdb(1152, CMD3);
            public static Registro_mdb ICoil          = new Registro_mdb(1153, CMD3);

            // BATTERIE
            public static Float32_mdb TotalmAhBatt    = new Float32_mdb(1154, CMD3);
            public static Float32_mdb LeftmAhBatt     = new Float32_mdb(1156, CMD3);
            public static Registro_mdb DayLastUpdate  = new Registro_mdb(1158, CMD3);

            // TARATURA
            public static Registro_mdb EPIPE_CALIBR   = new Registro_mdb(1159, CMD3);
            public static Float32_mdb ManualOffset    = new Float32_mdb(1160, CMD3);
            public static Float32_mdb KA_align        = new Float32_mdb(1162, CMD3);
            public static Float32_mdb KA_main         = new Float32_mdb(1164, CMD3);
            public static Float32_mdb KA2             = new Float32_mdb(1166, CMD3);
            public static Registro_mdb KA2_soglia     = new Registro_mdb(1168, CMD3);
            public static Float32_mdb KA3             = new Float32_mdb(1169, CMD3);
            public static Registro_mdb KA3_soglia     = new Registro_mdb(1171, CMD3);
            public static Float32_mdb KA4             = new Float32_mdb(1172, CMD3);
            public static Registro_mdb KA4_soglia     = new Registro_mdb(1174, CMD3);
            public static Float32_mdb KA5             = new Float32_mdb(1175, CMD3);
            public static Registro_mdb KA5_soglia     = new Registro_mdb(1177, CMD3);
            public static Float32_mdb KA6             = new Float32_mdb(1178, CMD3);
            public static Registro_mdb KA6_soglia     = new Registro_mdb(1180, CMD3);
            public static Float32_mdb KA7             = new Float32_mdb(1181, CMD3);
            public static Registro_mdb KA7_soglia     = new Registro_mdb(1183, CMD3);
            public static Float32_mdb KA8             = new Float32_mdb(1184, CMD3);
            public static Registro_mdb KA8_soglia     = new Registro_mdb(1186, CMD3);
            public static Float32_mdb KA9             = new Float32_mdb(1187, CMD3);
            public static Registro_mdb KA9_soglia     = new Registro_mdb(1189, CMD3);
            public static Float32_mdb KA10            = new Float32_mdb(1190, CMD3);
            public static Registro_mdb KA10_soglia    = new Registro_mdb(1192, CMD3);
            //public static Registro_mdb Free_1193    = new Registro_mdb(1193, CMD3); // LIBERO ************************************
            public static Float32_mdb AlignOffset     = new Float32_mdb(1194, CMD3);
            public static Float32_mdb ICoil_125mA     = new Float32_mdb(1196, CMD3);
            public static Registro_mdb Inserzione     = new Registro_mdb(1198, CMD3);
            //public static Registro_mdb Free_1199    = new Registro_mdb(1199, CMD3); // LIBERO ************************************


            public static Registro_mdb GSM_Enable     = new Registro_mdb(1200, CMD3);
            public static Registro_mdb GSM_SIM_PIN    = new Registro_mdb(1201, CMD3);
            public static Registro_mdb GSM_SMS        = new Registro_mdb(1202, CMD3);
            public static Registro_mdb GSM_Email      = new Registro_mdb(1203, CMD3);
            public static Registro_mdb GSM_EmailA     = new Registro_mdb(1204, CMD3);
            public static Registro_mdb GSM_FTP        = new Registro_mdb(1205, CMD3);

            //1206 > H: giorno della settimana (1-7) ; L: giorno del mese (1-31)
            //1207 > H: mese (1-12); L: anno (14-99)
            //1208 > H: ora (0:23); L: GMT (53-148 offset=+100)
            //1209 > H:minuti (0-59); L: secondi (0-59)
            public static Registro_mdb TIME_Week_Day      = new Registro_mdb(1206, CMD3);
            public static Registro_mdb TIME_Month_Year    = new Registro_mdb(1207, CMD3);
            public static Registro_mdb TIME_Hour_GMT      = new Registro_mdb(1208, CMD3);
            public static Registro_mdb TIME_Minute_Second = new Registro_mdb(1209, CMD3);

            public static Int32_mdb    GSM_MaxRows        = new Int32_mdb(1210, CMD3); //1-100.000
            
            //Timeout
            public static Registro_mdb GSM_SOFT_TimOut = new Registro_mdb(1213, CMD3);
            public static Registro_mdb GSM_HARD_TimOut = new Registro_mdb(1214, CMD3);
            //Xvar
            public static Registro_mdb GSM_SMS_X      = new Registro_mdb(1215, CMD3);
            public static Registro_mdb GSM_Email_X    = new Registro_mdb(1216, CMD3);
            public static Registro_mdb GSM_EmailA_X   = new Registro_mdb(1217, CMD3);
            public static Registro_mdb GSM_FTP_X      = new Registro_mdb(1218, CMD3);
            //Yvar
            public static Registro_mdb GSM_SMS_Y          = new Registro_mdb(1219, CMD3);
            public static Registro_mdb GSM_Email_Y        = new Registro_mdb(1220, CMD3);
            public static Registro_mdb GSM_EmailA_Y       = new Registro_mdb(1221, CMD3);
            public static Registro_mdb GSM_FTP_Y          = new Registro_mdb(1222, CMD3);
            public static Registro_mdb GSM_EnableRoaming  = new Registro_mdb(1223, CMD3);
        }

        public class Comandi
        {
            
            public static Command_mdb COPY_UT_FABBR = new Command_mdb(1, Command_mdb.CMD.CMD_COPY_UT_FABBR, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb COPY_UTENTE = new Command_mdb(1, Command_mdb.CMD.CMD_COPY_UTENTE, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb COPY_FABBRIC = new Command_mdb(1, Command_mdb.CMD.CMD_COPY_FABBRIC, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb ZERO_CAL = new Command_mdb(1, Command_mdb.CMD.CMD_ZERO_CAL, Command_mdb.KEY.KEY_AUTOZERO);
            public static Command_mdb RESET_PARZ_P = new Command_mdb(1, Command_mdb.CMD.CMD_RESET_PARZ_P, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb RESET_PARZ_N = new Command_mdb(1, Command_mdb.CMD.CMD_RESET_PARZ_P, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb RESET_TOT = new Command_mdb(1, Command_mdb.CMD.CMD_RESET_TOT, Command_mdb.KEY.KEY_AZZERA_TOTALIZ);
            public static Command_mdb RESET_MAIN = new Command_mdb(1, Command_mdb.CMD.CMD_RESET_MAIN, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb RESET_DATALOG = new Command_mdb(1, Command_mdb.CMD.CMD_RESET_DATALOG, Command_mdb.KEY.KEY_AZZERA_LOG);
            public static Command_mdb FLASH_CHECK = new Command_mdb(1, Command_mdb.CMD.CMD_FLASH_CHECK, Command_mdb.KEY.KEY_AZZERA_LOG);
            public static Command_mdb RESET_SYSLOG = new Command_mdb(1, Command_mdb.CMD.CMD_RESET_SYSLOG, Command_mdb.KEY.KEY_AZZERA_LOG);
            public static Command_mdb LOAD_DATA_SENS = new Command_mdb(1, Command_mdb.CMD.CMD_LOAD_DATA_SENS, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb INIT_MISURA = new Command_mdb(1, Command_mdb.CMD.CMD_INIT_MISURA, 0);
            public static Command_mdb EEPROM_TOT_INIT = new Command_mdb(1, Command_mdb.CMD.CMD_EEPROM_TOT_INIT, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb INIT_SENSOR_TABLE = new Command_mdb(1, Command_mdb.CMD.CMD_INIT_SENSOR_TABLE, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb EN_WR_MODBUS = new Command_mdb(2, Command_mdb.CMD.CMD_EN_WR_MODBUS, Command_mdb.KEY.KEY_WR_DATI_FABBRICA);
            public static Command_mdb EELOAD_MDB = new Command_mdb(2, Command_mdb.CMD.CMD_EELOAD_MDB, 0);
            public static Command_mdb ENABLE_SIMUL = new Command_mdb(3, Command_mdb.CMD.CMD_ENABLE_SIMUL, 0);
            public static Command_mdb DISABLE_SIMUL = new Command_mdb(3, 0, 0);
            public static Command_mdb SET_IO_MDB = new Command_mdb(4, Command_mdb.CMD.CMD_SET_IO_MDB, 0) ;
            public static Command_mdb SET_BCK = new Command_mdb(5, Command_mdb.CMD.CMD_SET_BCK, 0);
            public static Command_mdb ALIGN_KA = new Command_mdb(6, Command_mdb.CMD.CMD_ALIGN_KA, Command_mdb.KEY.KEY_COLLAUDO);
            public static Command_mdb OFFS_KA = new Command_mdb(7, Command_mdb.CMD.CMD_OFFS_KA, Command_mdb.KEY.KEY_COLLAUDO);
            public static Command_mdb TEST_MODE = new Command_mdb(8, Command_mdb.CMD.CMD_TEST_MODE, Command_mdb.KEY.KEY_COLLAUDO);
            public static Command_mdb CMD_ACT_COIL = new Command_mdb(9, Command_mdb.CMD.CMD_ACT_COIL, Command_mdb.KEY.KEY_COLLAUDO);
            public static Command_mdb CMD_TEST_EMPTP = new Command_mdb(10, Command_mdb.CMD.CMD_TEST_EMPTP, Command_mdb.KEY.KEY_COLLAUDO);
            public static Command_mdb CMD_SWITCH_COM = new Command_mdb(11, Command_mdb.CMD.CMD_SWITCH_COM, 0);
            public static Command_mdb CMD_ACT_COIL_set = new Command_mdb(12, Command_mdb.CMD.CMD_ACT_COIL, Command_mdb.KEY.KEY_COLLAUDO);
            public static Command_mdb CMD_ENTER_VERIFICATION = new Command_mdb(2, Command_mdb.CMD.CMD_ENTER_VERIFICATION, 0);
            public static Command_mdb CMD_EXIT_VERIFICATION = new Command_mdb(2, Command_mdb.CMD.CMD_EXIT_VERIFICATION, 0);
        }
    }
}
