using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

using System.Resources;

namespace MC_Suite.Modbus
{
    public class MC608 : IDisposable

    {
        static ResourceManager UTValueRes = new ResourceManager("MC608.UT", Assembly.GetExecutingAssembly());
        static ResourceManager TBValueRes = new ResourceManager("MC608.TB", Assembly.GetExecutingAssembly());

        public bool cks_OK = false;

        //#define OUT3_OFF                        0
        //#define OUT3_REVERSE_FLOW               1                             //l'uscita si attiva se la portata è negativa
        //#define OUT3_FLOW_HI                    2
        //#define OUT3_FLOW_LO                    3
        //#define OUT3_FLOW_HI_LO                 4                            
        //#define OUT3_BATCHING                   5
        //#define OUT3_EXC_FAILURE                6
        //#define OUT3_EPIPE                      7
        //#define OUT3_ALL_ALARMS                 8


        public static string[] IO3setup = { "sDISABLED", "sREVERSE_FLOW", "sFLOW_HI", "sFLOW_LO", "sFLOW_HI_LO", "sBATCHING", "sEXC_FAILURE", "sEPIPE", "sALL_ALARMS" }; // Etichette tabella traduzioni
        public static string[] DINsetup = { "sDISABLED", "sPP_RES", "sPN_RES", "sPP_PN_RES" };
        public static string[] Lingue = { "English", "Italiano", "Español", "Português", "Francese" };

        public static byte[] Parametri_EE_byte = new byte[0x200];
        public const int START_EE_UTENTE = 0xAF;
        public const int STOP_EE_UTENTE = 0xE2;

        public static byte N_Strum_in_taratura;


        public static int[] MeansSpace = new int[21]; // indice tabella tempi tra due misure 3 modalità

        public enum ICoil
        {
            I_125mA = 2,
            I_62mA = 4,
            I_50mA = 5,
            I_31mA = 8,
            I_25mA = 10
        }
        public enum PowerLineMode
        {
            Alimentata = 1,
            Batterie = 2
        }

        public enum PowerMode
        {
            Alim_Ext = 0,
            Batterie = 1
        }
        public enum Funzionamento
        {
            Alimentata = 0,
            Batterie = 1,
            Ricaricabile
        }

        public MC608()
        {
            MeansSpace[0] = 0;
            MeansSpace[1] = 200; //250ms
            MeansSpace[2] = 300;
            MeansSpace[3] = 400;
            MeansSpace[4] = 500;
            MeansSpace[5] = 750;
            MeansSpace[6] = 1000;   //1s
            MeansSpace[7] = 2000;
            MeansSpace[8] = 5000;
            MeansSpace[9] = 10000;
            MeansSpace[10] = 15000;
            MeansSpace[11] = 30000;
            MeansSpace[12] = 45000;
            MeansSpace[13] = 60000; //1min
            MeansSpace[14] = 120000;    //2min
            MeansSpace[15] = 180000;    //3min
            MeansSpace[16] = 240000;    //4min
            MeansSpace[17] = 300000;    //5min
            MeansSpace[18] = 360000;    //6min
            MeansSpace[19] = 420000;    //7min
            MeansSpace[20] = 480000;    //8min	

        }
        private static MC608 _instance;
        public static MC608 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MC608();
                return _instance;
            }
        }

        public static void Clear_Parametri_Utente()
        {
            Array.Clear(Parametri_EE_byte, START_EE_UTENTE, STOP_EE_UTENTE - START_EE_UTENTE);
            Impulsi.Reset();
            Portata.Reset();
        }
        public static void Reset()
        {
            Clear_Parametri_Utente();
            Array.Clear(MC608.Parametri_EE_byte, 0, 255);
            Convertitore.Modello = "";
            Sensore.Modello = "";
            Sensore.Note = "";
            Release_FW.Reset();
            Release_HW.Reset();
            Convertitore.SN_Converitore = 0;
            Convertitore.Matricola = "";
            Sensore.Matricola = "";
            Auto_PowerOff_s = 0;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Seriale

        public enum Password
        {
            KEY_AZZERA_TOTALIZ = 0x55,
            KEY_AZZERA_PARZ = 0x56,
            KEY_WR_DATI_FABBRICA = 0xD6,
            KEY_AUTOZERO = 0x40,
            KEY_AZZERA_LOG = 0x7F,
            KEY_COLLAUDO = 0x88
        }

        public enum ADDRESS_EE
        {
            CMD = 0x0681,
            SIMULAZ = 0x0683,
            SET_IO = 0x0684,
            SET_BCK = 0x0685,
            MeasSpaceSet = 0x00D5,
            MeasSpaceCont = 0x00D6,
            MeasSpaceWakeup = 0x00D7,
            LOG_Intervallo = 0x00E6,
            Numero_KA = 0x0054,
            PowerMode = 0x00FB,
            Pag_FlashInUso = 0x00E4,
            Damping = 0x00D9,
            CutOff = 0x00DB,
            Bypass = 0x00DC,
            Peak_Cut = 0x00DD,
            Average = 0x00E1,
            Pulse_Neg = 0x00C0

        }

        public enum CMD
        {
            Ripristina_Fabbrica = 120,
            Ripristina_Utente = 110,
            CopiaUten_Fabb = 105,
            CalibraZero = 202,
            Azzera_ParzP = 15,
            Azzera_ParzN = 25,
            Reset = 44,
            AzzeraLog = 89,
            Azzera_Totaliz = 35,
            Azzera_LogSys = 75,
            Carica_Dati_Sensore = 70,
            Init_misura = 71,
            EEPROM_tot_init = 61,
            FLASH_CHECK = 199,
            EN_WR_MODBUS = 133,
            EELOAD_MDB = 178,
            INIT_COLLAUDO = 185,
            ENABLE_SIMUL = 144,
            SET_IO_MDB = 155,
            SET_BCK = 12,
            Init_Sensor_table = 111
        }

        public enum Comunicazione
        {
            Non_Connesso = 0,
            Connessione_persa = 1,
            Connesso_IrDA = 2,
            Connesso_ModBus = 3,
            IrDA = 0xDD, // 1101 1101
            Key_TX = 0xBB, // 1011 1011  
            Ping = 0x99, // 1001 1001
            Read_EEPROM_IrDA_cmd = 0x33, // 0011 0011
            Read_RAM_IrDA_cmd = 0x11, // 0001 0001
            Write_EEPROM_cmd = 0x77, // 0111 0111
            Write_RAM_cmd = 0x55  // 0101 0101
        }

        private static Comunicazione m_Tipo_Connessione = Comunicazione.Non_Connesso;

        public static Comunicazione Tipo_Connessione
        {
            set
            {
                m_Tipo_Connessione = value;
            }
            get
            {
                return m_Tipo_Connessione;
            }
        }
        #endregion

        public static byte GetPowerMode
        {
            get
            {
                return Map.Registri_CMD4.GetPowerMode.LSB_Byte;
            }
        }
        // *************  Aggiunti per Hart 19.09.2013

        public static ushort ResetMain_key = 0x2CD6;
        public static ushort ResetMain
        {
            set
            {
                Map.Registri_CMD3_16.ResetMain.Reg_value = value;
            }
        }

        public static byte GetErrorState
        {
            get
            {
                return Map.Registri_CMD4.ErrorState.LSB_Byte;
            }
        }

        public static byte GetEEPromChange
        {
            get
            {
                return Map.Registri_CMD4.GetEEPromChange.LSB_Byte;
            }
        }
        public static ushort ResetTotP_key = 0x0F56;
        public static ushort ResetTotP
        {
            set
            {
                Map.Registri_CMD3_16.ResetTotP.Reg_value = value;
            }
        }
        public static ushort ResetTotN_key = 0x1956;
        public static ushort ResetTotN
        {
            set
            {
                Map.Registri_CMD3_16.ResetTotN.Reg_value = value;
            }
        }

        public static byte DumpingSec
        {
            set
            {
                Map.Registri_CMD3_16.DumpingSec.Reg_value = value;
            }
            get
            {
                return Map.Registri_CMD3_16.DumpingSec.LSB_Byte;
            }

        }

        public static float Set_AnalogOut_Hart
        {
            set
            {
                Map.Registri_CMD3_16.Set_AnalogOut_Hart.Float_Modbus = value;
            }
            get
            {
                return Map.Registri_CMD3_16.Set_AnalogOut_Hart.Float_Modbus;
            }
        }

        public static float Send_AnalogRead_Hart
        {
            set
            {
                Map.Registri_CMD3_16.Send_AnalogRead_Hart.Float_Modbus = value;
            }
            get
            {
                return Map.Registri_CMD3_16.Send_AnalogRead_Hart.Float_Modbus;
            }
        }
        public static float Set_FixedMode_4_20mA
        {
            set
            {
                Map.Registri_CMD3_16.Set_FixedMode_4_20mA.Float_Modbus = value;
            }
            get
            {
                return Map.Registri_CMD3_16.Set_FixedMode_4_20mA.Float_Modbus;
            }
        }

        public static ushort Cmd_Cal_AnalogOut_Hart_Key = 0xAB56;
        public static ushort Cmd_Cal_AnalogOut_Hart
        {
            set
            {
                Map.Registri_CMD3_16.Cmd_Cal_AnalogOut_Hart.Reg_value = value;
            }
            get
            {
                return Map.Registri_CMD3_16.Cmd_Cal_AnalogOut_Hart.Reg_value;
            }
        }

        // *****************************************************
        public static class Orologio
        {
            public static String Data
            {
                get
                {
                    return Map.Registri_CMD4.DayMem.LSB_Byte.ToString() + '/' + Map.Registri_CMD4.MonthMem.LSB_Byte.ToString() + '/' + (Map.Registri_CMD4.YearMem.Reg_value + 2000).ToString();
                }
            }
            public static String Ora
            {
                get
                {
                    return Map.Registri_CMD4.HourMem.LSB_Byte.ToString() + ':' + Map.Registri_CMD4.MinuteMem.LSB_Byte.ToString("00");
                }
            }
        }
        public static class Analogiche
        {
            public static float Temp_EXT
            {
                get
                {
                    return Map.Registri_CMD4.EXP_temp.Float_Modbus;
                }
            }
            public static float Temp_PCB
            {
                get
                {
                    return Map.Registri_CMD4.TempPcbScaled.Float_Modbus;
                }
            }
            public static float Volt_PCB
            {
                get
                {
                    return Map.Registri_CMD4.VoltMainScaled.Float_Modbus;
                }
            }
            public static float Volt_DCDC
            {
                get
                {
                    return Map.Registri_CMD4.DCDCRead.Float_Modbus;
                }
            }

        }
        public static class ESPANSIONI
        {
            public enum EXP1_mode
            {
                disabilitato = 0,
                pressione = 4,
                temperatura = 5,
                press_temp = 6
            }
            public static ushort EXP1 // Addr 1090
            {
                get { return Map.Registri_CMD3_16.ExpSetup_1.Reg_value; }
                set { Map.Registri_CMD3_16.ExpSetup_1.Reg_value = value; }
            }
            public static ushort EXP2
            {
                get { return Map.Registri_CMD3_16.ExpSetup_2.Reg_value; }
                set { Map.Registri_CMD3_16.ExpSetup_2.Reg_value = value; }
            }
            public static ushort EXP3
            {
                get { return Map.Registri_CMD3_16.ExpSetup_3.Reg_value; }
                set { Map.Registri_CMD3_16.ExpSetup_3.Reg_value = value; }
            }
            public static float ExpPar1
            {
                get { return Map.Registri_CMD3_16.ExpPar_1.Float_Modbus; }
                set { Map.Registri_CMD3_16.ExpPar_1.Float_Modbus = value; }
            }
        }
        #region Digital IO
        public static class DIGITAL_IO
        {
            public static ushort data = Map.Registri_CMD4.Digital_IO.Reg_value;

            public static bool REED
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & 1) > 0)
                        return true;
                    else
                        return false;

                }
            }
            public static bool S1
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 1)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool S2
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 2)) > 0)
                        return true;
                    else
                        return false;
                }
            }

            public static bool S3
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 3)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool S4
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 4)) > 0)
                        return true;
                    else
                        return false;
                }
            }

            public static bool GP_IN
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 5)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool GP_OUT
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 6)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool FREQ_OUT
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 7)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool PULSES_OUT
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 8)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool Timer1
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 9)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool Timer2
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 10)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool Timer3
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 11)) > 0)
                        return true;
                    else
                        return false;
                }
            }
            public static bool Orologio
            {
                get
                {
                    if ((Map.Registri_CMD4.Digital_IO.Reg_value & (1 << 12)) > 0)
                        return true;
                    else
                        return false;
                }
            }
        }

        #endregion

        #region DATALOGGER  // ***********************************************
        public static class DATALOGGER
        {

            public static byte Intervallo_min // sistemare riorganizzando la tabella per la 608_6
            {
                set
                {
                    Intervallo_sec = value * 60;
                }
                get
                {
                    return (byte)(Intervallo_sec / 60);
                }

            }
            public const int Intervallo_minimo_sec = 1 * 4;
            public const int Intervallo_massimo_sec = 240 * 4;
            public const int Intervallo_minimo_min = 1;
            public const int Intervallo_massimo_min = 120;
            public static int Intervallo_sec // sistemare riorganizzando la tabella per la 608_6
            {
                set
                {
                    if (Release_FW.Versione >= 3)
                        value = (ushort)(value / 60);
                    else
                        value = (ushort)(value / 4);

                    Map.Registri_CMD3_16.LogInterval.Reg_value = (ushort)value;
                    Parametri_EE_byte[(int)ADDRESS_EE.LOG_Intervallo] = (byte)value;
                }
                get
                {
                    int ret_tmp = 0;

                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        ret_tmp = Parametri_EE_byte[(int)ADDRESS_EE.LOG_Intervallo];
                    else
                        ret_tmp = (byte)Map.Registri_CMD3_16.LogInterval.Reg_value;

                    if (Release_FW.Versione >= 3)
                        ret_tmp = ret_tmp * 60;
                    else
                        ret_tmp = ret_tmp * 4;

                    return ret_tmp;
                }

            }

            public static ushort Pagina_FlashInUso
            {
                set
                {
                    Map.Registri_CMD3_16.DLoggerPage.Reg_value = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, (int)ADDRESS_EE.Pag_FlashInUso, sizeof(ushort));
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, (int)ADDRESS_EE.Pag_FlashInUso);
                    else
                        return Map.Registri_CMD3_16.DLoggerPage.Reg_value;
                }
            }
            public static ushort GetMemoryModel
            {
                get
                {
                    return Map.Registri_CMD3_16.MemoryModel.Reg_value;
                }
            }

            public static DateTime[] Time;
            public static float[] Portata; // tutto in litri
            public static float[] TotP;
            public static float[] TotN;
            public static float[] Vbatt;
            public static float[] TempPCB;
            public static byte[] error;
            public static Int16[] SI1x;
            public static byte[] Battery;
            public static float[] F1x;
            public static float[] F2x;
            public static byte[] U5x;

            static byte Row_pack = 0;
            static byte byteXrow_pack = 0;
            static int byte_pack = 0;
            public static byte[] data_pack_M25P32_485; // header(3) + dati(2K)+ CRC16(2)

            public static void Crea_array_datalogger()
            {
                uint Flash_Size = 0;

                if (Release_FW.Versione >= 3)
                {
                    if (Release_FW.Revisione < 8)
                    {
                        Flash_Size = 221184;
                        byteXrow_pack = 20;
                        Row_pack = 100;
                    }
                    else
                    {
                        Flash_Size = 131072;
                        byteXrow_pack = 32;
                        Row_pack = 100;
                    }
                }
                else
                {
                    Flash_Size = 221211;
                    byteXrow_pack = 19;
                    Row_pack = 27;
                }

                Time = new DateTime[Flash_Size];
                Portata = new float[Flash_Size]; // tutto in litri
                TotP = new float[Flash_Size];
                TotN = new float[Flash_Size];
                Vbatt = new float[Flash_Size];
                TempPCB = new float[Flash_Size];
                error = new byte[Flash_Size];
                SI1x = new Int16[Flash_Size]; // Aggiunte dalla versione 3.09
                Battery = new byte[Flash_Size];
                F1x = new float[Flash_Size];
                F2x = new float[Flash_Size];
                U5x = new byte[Flash_Size];

                byte_pack = Row_pack * byteXrow_pack;
                data_pack_M25P32_485 = new byte[byte_pack + 5]; // header(3) + dati(2K)+ CRC16(2)
            }

            public static void Crea_array_dataloggerTEXAS()
            {
                uint Flash_Size = 0;

                Flash_Size = 131072;
                byteXrow_pack = 32;
                Row_pack = 10;

                Time = new DateTime[Flash_Size];
                Portata = new float[Flash_Size]; // tutto in litri
                TotP = new float[Flash_Size];
                TotN = new float[Flash_Size];
                Vbatt = new float[Flash_Size];
                TempPCB = new float[Flash_Size];
                error = new byte[Flash_Size];
                SI1x = new Int16[Flash_Size]; // Aggiunte dalla versione 3.09
                Battery = new byte[Flash_Size];
                F1x = new float[Flash_Size];
                F2x = new float[Flash_Size];
                U5x = new byte[Flash_Size];

                byte_pack = Row_pack * byteXrow_pack;
                data_pack_M25P32_485 = new byte[byte_pack + 5]; // header(3) + dati(2K)+ CRC16(2)
            }

            public static void Decodifica_dati(int start_row)
            {
                byte[] data_M25P32 = new byte[data_pack_M25P32_485.Length - 5];

                Array.Copy(data_pack_M25P32_485, 3, data_M25P32, 0, data_M25P32.Length); // tolgo header(3) e CRC16(2)

                for (int n_row = 0; n_row < Row_pack; n_row++) // funziona solo fino a 10 ..... !!!!!!!
                {
                    int linea_log = n_row + start_row;

                    if (linea_log >= Portata.Length) // Fine datalogger per non scrivere oltre l'array
                        break;

                    int index = n_row * byteXrow_pack;
                    Portata[linea_log] = BitConverter.ToSingle(data_M25P32, (index));
                    TotP[linea_log] = BitConverter.ToSingle(data_M25P32, (index + 4));
                    TotN[linea_log] = BitConverter.ToSingle(data_M25P32, (index + 8));

                    try
                    {
                        DateTime dataora_log = new DateTime((data_M25P32[index + 16] + 2000), data_M25P32[index + 15], data_M25P32[index + 14], data_M25P32[index + 12], data_M25P32[index + 13], 0);
                        Time[linea_log] = dataora_log;
                    }
                    catch (Exception)
                    {
                    }

                    TempPCB[linea_log] = data_M25P32[index + 17];
                    TempPCB[n_row + start_row] -= 50;

                    Vbatt[linea_log] = data_M25P32[index + 18];
                    Vbatt[linea_log] /= 10;

                    error[linea_log] = data_M25P32[index + 19];

                    if (MC608.Release_FW.Versione >= 3 && MC608.Release_FW.Revisione > 8)
                    {
                        SI1x[linea_log] = BitConverter.ToInt16(data_M25P32, (index + 20)); // Aggiunte dalla versione 3.09

                        F1x[linea_log] = BitConverter.ToSingle(data_M25P32, (index + 22));

                        if (F1x[linea_log] >= float.MaxValue || F1x[linea_log] == float.NaN)
                            F1x[linea_log] = 0;

                        F2x[n_row + start_row] = BitConverter.ToSingle(data_M25P32, (index + 26));

                        if (F2x[linea_log] >= float.MaxValue || F2x[linea_log] == float.NaN)
                            F2x[linea_log] = 0;

                        Battery[linea_log] = data_M25P32[index + 30];
                        U5x[linea_log] = data_M25P32[index + 31];
                    }
                }
            }
        }

        #endregion

        public static byte GetIrVersion
        {
            get
            {
                return Map.Registri_CMD3_16.GetIrVersion.LSB_Byte;
            }
        }

        #region DATI di FABBRICA

        public static byte PowerModeValue
        {
            set
            {
                Map.Registri_CMD3_16.power_mode.Reg_value = value;
            }
            get
            {
                return Map.Registri_CMD3_16.power_mode.LSB_Byte;
            }
        }

        public static byte EN_Ricaricabile
        {
            set
            {
                Map.Registri_CMD3_16.Ricaricabile_EN.Reg_value = value;
            }
            get
            {
                return Map.Registri_CMD3_16.Ricaricabile_EN.LSB_Byte;
            }
        }

        public static class Release_MDB
        {

            private static ushort _Release_MDB = 0x0000; // Valore trasferito dal modbus

            public static ushort Value_mdb
            {
                set { _Release_MDB = value; }
                get { return _Release_MDB; }
            }
            public static string Release_name
            {
                get
                {
                    return Versione.ToString() + "." + Revisione.ToString("00");
                }
            }

            public static byte Versione
            {
                get
                {
                    return Map.Registri_CMD3_16.Release_ModBus.MSB_Byte;
                }
            }
            public static byte Revisione
            {
                get
                {
                    return Map.Registri_CMD3_16.Release_ModBus.LSB_Byte;
                }
            }
        }

        public static class Release_FW
        {
            public static void Reset()
            {
                Value_mdb = 0;
            }

            public static ushort Value_mdb;

            public static string Release_name
            {
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[0xA].ToString() + "." + Parametri_EE_byte[0xB].ToString("00");
                    else
                        return Versione.ToString() + "." + Revisione.ToString("00");
                }
            }

            public static byte Versione
            {
                get
                {
                    return Map.Registri_CMD3_16.Release_FW.MSB_Byte;
                }
            }
            public static byte Revisione
            {
                get
                {
                    return Map.Registri_CMD3_16.Release_FW.LSB_Byte;
                }
            }
        }

        public static class Release_HW
        {
            public static void Reset()
            {
                Value_mdb = 0;
            }

            public static ushort Value_mdb;    // 0x0A 4 char

            public static string Release_name
            {
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[0xC].ToString() + "." + Parametri_EE_byte[0xD].ToString("00");
                    else
                        return Versione.ToString() + "." + Revisione.ToString("00");
                }
            }
            public static byte Versione
            {
                get
                {
                    return Map.Registri_CMD3_16.Release_HW.MSB_Byte;
                }
            }
            public static byte Revisione
            {
                get
                {
                    return Map.Registri_CMD3_16.Release_HW.LSB_Byte;
                }
            }
        }

        public static class Sensore
        {

            public static string Modello   // 0x0E 12 char
            {
                set
                {
                    Map.Registri_CMD3_16.Sensore.Text = value;
                    Encoding.ASCII.GetBytes(Formatta_nChar(value, 12), 0, 12, Parametri_EE_byte, 0x0E);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Leggi_stringa_EE(0x0E, 12);
                    else
                        return Map.Registri_CMD3_16.Sensore.Text;
                }
            }

            public static string Note   // 0xC1 20 char
            {
                set
                {
                    Map.Registri_CMD3_16.Note.Text = value;
                    Encoding.ASCII.GetBytes(Formatta_nChar(value, 20), 0, 20, Parametri_EE_byte, 0x001C);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Leggi_stringa_EE(0x1C, 20);
                    else
                        return Map.Registri_CMD3_16.Note.Text;
                }
            }

            public static string Matricola
            {
                set
                {
                    Map.Registri_CMD3_16.Sensore_matricola.Text = value;
                    Encoding.ASCII.GetBytes(Formatta_nChar(value, 9), 0, 9, Parametri_EE_byte, 0x3A);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Leggi_stringa_EE(0x3A, 9);
                    else
                        return Map.Registri_CMD3_16.Sensore_matricola.Text;
                }
            }

            public static ushort Diametro
            {
                set
                {
                    Map.Registri_CMD3_16.Diametro_sensore.Reg_value = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x001A, 2);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, 0x1A);
                    else
                        return Map.Registri_CMD3_16.Diametro_sensore.Reg_value;
                }
            }

            static public bool Tubo_vuoto
            {
                set
                {
                    Map.Registri_CMD3_16.Tubo_Vuoto.bool_value = value;
                    if (value)
                        Parametri_EE_byte[0x0030] = 1;
                    else
                        Parametri_EE_byte[0x0030] = 0;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        if (Parametri_EE_byte[0x0030] == 1)
                            return true;
                        else
                            return false;
                    else
                        return Map.Registri_CMD3_16.Tubo_Vuoto.bool_value;

                }
            }
            static public bool Inserzione
            {
                set
                {
                    Map.Registri_CMD3_16.Inserzione.bool_value = value;
                }
                get
                {
                    return Map.Registri_CMD3_16.Inserzione.bool_value;
                }
            }
        }

        public class Convertitore
        {

            public static string Modello // 0x02 8 char
            {
                set
                {
                    Map.Registri_CMD3_16.Convertitore.Text = value;
                    Encoding.ASCII.GetBytes(Formatta_nChar(value, 8), 0, 8, Parametri_EE_byte, 0x0002);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Leggi_stringa_EE(0x02, 8);
                    else
                        return Map.Registri_CMD3_16.Convertitore.Text;
                }
            }
            public static UInt32 SN_Converitore
            {
                set
                {
                    Map.Registri_CMD3_16.Serial_convert.Long32_Modbus = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x0044, 4);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt32(Parametri_EE_byte, 0x0044);
                    else
                        return Map.Registri_CMD3_16.Serial_convert.Long32_Modbus;
                }
            }

            public static string Matricola // 0x31 9 char
            {
                set
                {
                    Map.Registri_CMD3_16.Convert_matricola.Text = value;
                    Encoding.ASCII.GetBytes(Formatta_nChar(value, 9), 0, 9, Parametri_EE_byte, 0x031);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Leggi_stringa_EE(0x31, 9);
                    else
                        return Map.Registri_CMD3_16.Convert_matricola.Text;
                }
            }
        }

        public static class Taratura
        {
            public static ushort Dac_SET
            {
                get
                {
                    return Map.Registri_CMD4.DAC_SS1_KALIGN.Reg_value;
                }
            }
            public static float ICoil_125mA // float FONDO SCALA MISURATORE
            {
                set
                {
                    Map.Registri_CMD3_16.ICoil_125mA.Float_Modbus = value;

                }
                get
                {
                    return Map.Registri_CMD3_16.ICoil_125mA.Float_Modbus;
                }
            }
            public static string Data_Calibrazione    // 0x4C 8 char
            {
                set
                {
                    Map.Registri_CMD3_16.DAT_CALIB.Text = value;
                    Encoding.ASCII.GetBytes(Formatta_nChar(value, 8), 0, 8, Parametri_EE_byte, 0x4C);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Leggi_stringa_EE(0x4C, 8);
                    else
                        return Map.Registri_CMD3_16.DAT_CALIB.Text;
                }
            }
            public static float KA_Align
            {
                set
                {
                    Map.Registri_CMD3_16.KA_align.Float_Modbus = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x008B, 4);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToSingle(Parametri_EE_byte, 0x008B);
                    else
                        return Map.Registri_CMD3_16.KA_align.Float_Modbus;
                }
            }

            public static float KA1_main
            {
                set
                {
                    Map.Registri_CMD3_16.KA_main.Float_Modbus = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x0059, 4);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToSingle(Parametri_EE_byte, 0x0059);
                    else
                        return Map.Registri_CMD3_16.KA_main.Float_Modbus;
                }
            }

            public static float KA1_flow
            {
                set;
                get;

            }

            public static float KA_2
            {
                set { Map.Registri_CMD3_16.KA2.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA2.Float_Modbus; }
            }

            public static ushort KA_2_soglia
            {
                set { Map.Registri_CMD3_16.KA2_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA2_soglia.Reg_value; }
            }

            public static float KA_3
            {
                set { Map.Registri_CMD3_16.KA3.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA3.Float_Modbus; }
            }

            public static ushort KA_3_soglia
            {
                set { Map.Registri_CMD3_16.KA3_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA3_soglia.Reg_value; }
            }

            public static float KA_4
            {
                set { Map.Registri_CMD3_16.KA4.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA4.Float_Modbus; }
            }

            public static ushort KA_4_soglia
            {
                set { Map.Registri_CMD3_16.KA4_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA4_soglia.Reg_value; }
            }

            public static float KA_5
            {
                set { Map.Registri_CMD3_16.KA5.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA5.Float_Modbus; }
            }

            public static ushort KA_5_soglia
            {
                set { Map.Registri_CMD3_16.KA5_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA5_soglia.Reg_value; }
            }

            public static float KA_6
            {
                set { Map.Registri_CMD3_16.KA6.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA6.Float_Modbus; }
            }
            public static ushort KA_6_soglia
            {
                set { Map.Registri_CMD3_16.KA6_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA6_soglia.Reg_value; }
            }
            public static float KA_7
            {
                set { Map.Registri_CMD3_16.KA7.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA7.Float_Modbus; }
            }
            public static ushort KA_7_soglia
            {
                set { Map.Registri_CMD3_16.KA7_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA7_soglia.Reg_value; }
            }
            public static float KA_8
            {
                set { Map.Registri_CMD3_16.KA8.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA8.Float_Modbus; }
            }
            public static ushort KA_8_soglia
            {
                set { Map.Registri_CMD3_16.KA8_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA8_soglia.Reg_value; }
            }
            public static float KA_9
            {
                set { Map.Registri_CMD3_16.KA9.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA9.Float_Modbus; }
            }
            public static ushort KA_9_soglia
            {
                set { Map.Registri_CMD3_16.KA9_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA9_soglia.Reg_value; }
            }
            public static float KA_10
            {
                set { Map.Registri_CMD3_16.KA10.Float_Modbus = value; }
                get { return Map.Registri_CMD3_16.KA10.Float_Modbus; }
            }
            public static ushort KA_10_soglia
            {
                set { Map.Registri_CMD3_16.KA10_soglia.Reg_value = value; }
                get { return Map.Registri_CMD3_16.KA10_soglia.Reg_value; }
            }


            public static float ManualOffSet
            {
                set
                {
                    Map.Registri_CMD3_16.ManualOffset.Float_Modbus = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00A9, 4);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToSingle(Parametri_EE_byte, 0x00A9);
                    else
                        return Map.Registri_CMD3_16.ManualOffset.Float_Modbus;
                }
            }
            public static float AlignOffset
            {
                set
                {
                    Map.Registri_CMD3_16.AlignOffset.Float_Modbus = value;
                }
                get
                {
                    return Map.Registri_CMD3_16.AlignOffset.Float_Modbus;
                }
            }
        }

        #endregion

        #region Parametri utente
        public static byte TempUT_Set    // 0x4C 8 char
        {
            set
            {
                Map.Registri_CMD3_16.Temp_Unit.Reg_value = value;
            }
        }
        public static string TempUT_Get    // 0x4C 8 char
        {
            get
            {
                if (Map.Registri_CMD3_16.Temp_Unit.LSB_Byte == 1)
                    return "°F";
                else
                    return "°C";
            }
        }

        public string[] TB_Disponibili
        {
            get
            {
                string[] elencoTB = new string[40];
                byte i = 0;
                while (TBValueRes.GetString("TB" + i.ToString()) != null)
                {
                    elencoTB[i] = TBValueRes.GetString("TB" + i.ToString());
                    i++;
                }
                string[] elencoFinale = new string[i];
                Array.Copy(elencoTB, elencoFinale, i);
                return elencoFinale;
            }

        }

        #region Parametri

        public float Flow_ms
        {
            get { return Map.Registri_CMD4.Measure_m_s.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.Measure_m_s.Float_Modbus)
                {
                    Map.Registri_CMD4.Measure_m_s.Float_Modbus = value;
                    OnPropertyChanged("Flow_ms");
                }
            }
        }

        public float Flow_m3s
        {
            get { return Map.Registri_CMD4.Measure_m3_s.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.Measure_m3_s.Float_Modbus)
                {
                    Map.Registri_CMD4.Measure_m3_s.Float_Modbus = value;
                    OnPropertyChanged("Flow_m3s");
                }
            }
        }

        public float Flow_kgs
        {
            get { return Map.Registri_CMD4.Measure_kg_s.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.Measure_kg_s.Float_Modbus)
                {
                    Map.Registri_CMD4.Measure_kg_s.Float_Modbus = value;
                    OnPropertyChanged("Flow_kgs");
                }
            }
        }

        public float Flow_scaled
        {
            get { return Map.Registri_CMD4.MeasureScaled.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.MeasureScaled.Float_Modbus)
                {
                    Map.Registri_CMD4.MeasureScaled.Float_Modbus = value;
                    OnPropertyChanged("Flow_scaled");
                }
            }
        }
        
        public float Flow_perc
        {
            get { return Map.Registri_CMD4.Portata_perc.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.Portata_perc.Float_Modbus)
                {
                    Map.Registri_CMD4.Portata_perc.Float_Modbus = value;
                    OnPropertyChanged("Portata_perc");
                }
            }
        }

        public float TotalPositive
        {
            get { return Map.Registri_CMD4.TPAccScaled.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.TPAccScaled.Float_Modbus)
                {
                    Map.Registri_CMD4.TPAccScaled.Float_Modbus = value;
                    OnPropertyChanged("TotalPositive");
                }
            }
        }

        public float TotalNegative
        {
            get { return Map.Registri_CMD4.TNAccScaled.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.TNAccScaled.Float_Modbus)
                {
                    Map.Registri_CMD4.TNAccScaled.Float_Modbus = value;
                    OnPropertyChanged("TotalNegative");
                }
            }
        }

        public float PartialPositive
        {
            get { return Map.Registri_CMD4.PPAccScaled.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.PPAccScaled.Float_Modbus)
                {
                    Map.Registri_CMD4.PPAccScaled.Float_Modbus = value;
                    OnPropertyChanged("PartialPositive");
                }
            }
        }

        public float PartialNegative
        {
            get { return Map.Registri_CMD4.PNAccScaled.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.PNAccScaled.Float_Modbus)
                {
                    Map.Registri_CMD4.PNAccScaled.Float_Modbus = value;
                    OnPropertyChanged("PartialNegative");
                }
            }
        }

        public float FlowrateFS
        {
            get { return Map.Registri_CMD4.fullscale_scalato.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD4.fullscale_scalato.Float_Modbus)
                {
                    Map.Registri_CMD4.fullscale_scalato.Float_Modbus = value;
                    OnPropertyChanged("FlowrateFS");
                }
            }
        }

        private DateTime _convDateTime;
        public DateTime ConvDateTime
        {
            get {
                try
                { 
                    _convDateTime = new DateTime(Map.Registri_CMD4.YearMem.Reg_value + 2000,
                                                Map.Registri_CMD4.MonthMem.Reg_value,
                                                Map.Registri_CMD4.DayMem.Reg_value,
                                                Map.Registri_CMD4.HourMem.Reg_value,
                                                Map.Registri_CMD4.MinuteMem.Reg_value,
                                                0);
                }
                catch
                {
                    _convDateTime = new DateTime();
                }
                return _convDateTime;
            }
            set
            {
                if (value != _convDateTime)
                {
                    _convDateTime = value;
                    OnPropertyChanged("ConvDateTime");
                }
            }
        }

        public ushort FlowrateUnit
        {
            get { return Map.Registri_CMD3_16.VflowUT.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.VflowUT.Reg_value)
                {
                    Map.Registri_CMD3_16.VflowUT.Reg_value = value;
                    OnPropertyChanged("FlowrateUnit");
                }
            }
        }

        public ushort FlowrateTimebase
        {
            get { return Map.Registri_CMD3_16.TflowUT.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.TflowUT.Reg_value)
                {
                    Map.Registri_CMD3_16.TflowUT.Reg_value = value;
                    OnPropertyChanged("FlowrateTimebase");
                }
            }
        }
       

        public ushort CountersUnit
        {
            get { return Map.Registri_CMD3_16.VaccUT.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.VaccUT.Reg_value)
                {
                    Map.Registri_CMD3_16.VaccUT.Reg_value = value;
                    OnPropertyChanged("CountersUnit");
                }
            }
        }

        public ushort PulseOutUnit
        {
            get { return Map.Registri_CMD3_16.VpulseUT.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.VpulseUT.Reg_value)
                {
                    Map.Registri_CMD3_16.VpulseUT.Reg_value = value;
                    OnPropertyChanged("PulseOutUnit");
                }
            }
        }

        public float PulseOutVolume
        {
            get { return Map.Registri_CMD3_16.PQtyMain.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD3_16.PQtyMain.Float_Modbus)
                {
                    Map.Registri_CMD3_16.PQtyMain.Float_Modbus = value;
                    OnPropertyChanged("PulseOutVolume");
                }
            }
        }

        public ushort PulseOutWidth
        {
            get { return Map.Registri_CMD3_16.PONmain_ms.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.PONmain_ms.Reg_value)
                {
                    Map.Registri_CMD3_16.PONmain_ms.Reg_value = value;
                    OnPropertyChanged("PulseOutWidth");
                }
            }
        }

        public float FreqOutFS
        {
            get { return Map.Registri_CMD3_16.MaxPulsesFreq.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD3_16.MaxPulsesFreq.Float_Modbus)
                {
                    Map.Registri_CMD3_16.MaxPulsesFreq.Float_Modbus = value;
                    OnPropertyChanged("FreqOutFS");
                }
            }
        }

        public ushort ProgOutSetup
        {
            get { return Map.Registri_CMD3_16.OUTsetup.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.OUTsetup.Reg_value)
                {
                    Map.Registri_CMD3_16.OUTsetup.Reg_value = value;
                    OnPropertyChanged("ProgOutSetup");
                }
            }
        }

        public ushort ProgInSetup
        {
            get { return Map.Registri_CMD3_16.DINsetup.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.DINsetup.Reg_value)
                {
                    Map.Registri_CMD3_16.DINsetup.Reg_value = value;
                    OnPropertyChanged("ProgInSetup");
                }
            }
        }

        public ushort LCDContrast
        {
            get { return Map.Registri_CMD3_16.LcdContrast.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.LcdContrast.Reg_value)
                {
                    Map.Registri_CMD3_16.LcdContrast.Reg_value = value;
                    OnPropertyChanged("LCDContrast");
                }
            }
        }

        public ushort LCDBacklight
        {
            get { return Map.Registri_CMD3_16.Backlight_Perc.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.Backlight_Perc.Reg_value)
                {
                    Map.Registri_CMD3_16.Backlight_Perc.Reg_value = value;
                    OnPropertyChanged("LCDBacklight");
                }
            }
        }

        public ushort LCDBacklightTimeout
        {
            get { return Map.Registri_CMD3_16.Backlight_TimeOut.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.Backlight_TimeOut.Reg_value)
                {
                    Map.Registri_CMD3_16.Backlight_TimeOut.Reg_value = value;
                    OnPropertyChanged("LCDBacklightTimeout");
                }
            }
        }

        public ushort Language
        {
            get { return Map.Registri_CMD3_16.menu_language.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.menu_language.Reg_value)
                {
                    Map.Registri_CMD3_16.menu_language.Reg_value = value;
                    OnPropertyChanged("Language");
                }
            }
        }

        public ushort AutoPowerOff
        {
            get { return Map.Registri_CMD3_16.AutoPowerOffMem.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.AutoPowerOffMem.Reg_value)
                {
                    Map.Registri_CMD3_16.AutoPowerOffMem.Reg_value = value;
                    OnPropertyChanged("AutoPowerOff");
                }
            }
        }

        public ushort MaxFlowAlarm
        {
            get { return Map.Registri_CMD3_16.UP_FLOW_ALARM.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.UP_FLOW_ALARM.Reg_value)
                {
                    if (value < MinFlowAlarm)
                        value = MinFlowAlarm;
                    if (value > 200)
                        value = 255;
                    Map.Registri_CMD3_16.UP_FLOW_ALARM.Reg_value = value;
                    OnPropertyChanged("MaxFlowAlarm");
                }
            }
        }

        public ushort MinFlowAlarm
        {
            get { return Map.Registri_CMD3_16.LOW_FLOW_ALARM.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.LOW_FLOW_ALARM.Reg_value)
                {
                    if (value > MaxFlowAlarm)
                        value = MaxFlowAlarm;
                    if (value < 5)
                        value = 5;
                    Map.Registri_CMD3_16.LOW_FLOW_ALARM.Reg_value = value;
                    OnPropertyChanged("MinFlowAlarm");
                }
            }
        }

        public ushort PeakCutMin
        {
            get { return 1; }
        }
        public ushort PeakCutMax
        {
            get { return 25; }
        }
        public ushort PeakCut
        {
            get { return Map.Registri_CMD3_16.PeakCutTh.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.PeakCutTh.Reg_value)
                {
                    Map.Registri_CMD3_16.PeakCutTh.Reg_value = value;
                    OnPropertyChanged("PeakCut");
                }
            }
        }

        public ushort CutOffMin
        {
            get { return 0; }
        }
        public ushort CutOffMax
        {
            get { return 50; }
        }
        public ushort CutOff
        {
            get { return Map.Registri_CMD3_16.CutOffPercent.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.CutOffPercent.Reg_value)
                {
                    Map.Registri_CMD3_16.CutOffPercent.Reg_value = value;
                    OnPropertyChanged("CutOff");
                }
            }
        }

        public ushort FilterBypassMin
        {
            get { return 2; }
        }
        public ushort FilterBypassMax
        {
            get { return 95; }
        }
        public ushort FilterBypass
        {
            get { return Map.Registri_CMD3_16.FilterBypassTh.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.FilterBypassTh.Reg_value)
                {
                    Map.Registri_CMD3_16.FilterBypassTh.Reg_value = value;
                    OnPropertyChanged("FilterBypass");
                }
            }
        }

        public ushort DampingMin
        {
            get { return 5; }
        }
        public ushort DampingMax
        {
            get { return 500; }
        }
        public ushort Damping
        {
            get { return Map.Registri_CMD3_16.Damping.Reg_value; }
            set
            {
                if (value > DampingMax)
                    value = DampingMax;
                if (value < DampingMin)
                    value = DampingMin;

                if (value != Map.Registri_CMD3_16.Damping.Reg_value)
                {
                    Map.Registri_CMD3_16.Damping.Reg_value = value;
                    OnPropertyChanged("Damping");
                }
            }
        }

        public ushort AverageMin
        {
            get { return 1; }
        }
        public ushort AverageMax
        {
            get { return 500; }
        }
        public ushort Average
        {
            get { return Map.Registri_CMD3_16.AverageFilter.Reg_value; }
            set
            {
                if (value > AverageMax)
                    value = AverageMax;
                if (value < AverageMin)
                    value = AverageMin;

                if (value != Map.Registri_CMD3_16.AverageFilter.Reg_value)
                {
                    Map.Registri_CMD3_16.AverageFilter.Reg_value = value;
                    OnPropertyChanged("Average");
                }
            }
        }

        public ushort LineFrequency
        {
            get { return Map.Registri_CMD3_16.LineFreq.Reg_value; }
            set
            {
                if (value != Map.Registri_CMD3_16.LineFreq.Reg_value)
                {
                    Map.Registri_CMD3_16.LineFreq.Reg_value = value;
                    OnPropertyChanged("LineFrequency");
                }
            }
        }

        public float BatchingVolume
        {
            get { return Map.Registri_CMD3_16.BATCHING_VOLUME.Float_Modbus; }
            set
            {
                if (value != Map.Registri_CMD3_16.BATCHING_VOLUME.Float_Modbus)
                {
                    Map.Registri_CMD3_16.BATCHING_VOLUME.Float_Modbus = value;
                    OnPropertyChanged("BatchingVolume");
                }
            }
        }


        #endregion

        private IDictionary<ushort, String> _unitsList;
        public IDictionary<ushort, String> UnitsList
        {
            get
            {
                if (_unitsList == null)
                {
                    _unitsList = new Dictionary<ushort, String>();
                    _unitsList.Add(0, "--");
                    _unitsList.Add(1, "ml");
                    _unitsList.Add(2, "cl");
                    _unitsList.Add(3, "dl");
                    _unitsList.Add(4, "l");
                    _unitsList.Add(5, "dal");
                    _unitsList.Add(6, "hl");
                    _unitsList.Add(7, "m³");
                    _unitsList.Add(8, "Ml");
                    _unitsList.Add(9, "in³");
                    _unitsList.Add(10, "ft³");
                    _unitsList.Add(11, "gal");
                    _unitsList.Add(12, "bbl");
                    _unitsList.Add(13, "oz");
                }
                return _unitsList;
            }
        }

        private static IDictionary<byte, float> _convUnitsList;
        public static IDictionary<byte, float> ConvUnitsList
        {
            get
            {
                if (_convUnitsList == null)
                {
                    _convUnitsList = new Dictionary<byte, float>();
                    _convUnitsList.Add(0, 0);
                    _convUnitsList.Add(1, 1000);
                    _convUnitsList.Add(2, 100);
                    _convUnitsList.Add(3, 10);
                    _convUnitsList.Add(4, 1);
                    _convUnitsList.Add(5, 0.1f);
                    _convUnitsList.Add(6, 0.01f);
                    _convUnitsList.Add(7, 0.001f);
                    _convUnitsList.Add(8, 0.000001f);
                    _convUnitsList.Add(9, 61.0237f);
                    _convUnitsList.Add(10, 0.0353147f);
                    _convUnitsList.Add(11, 0.264172f);
                    _convUnitsList.Add(12, 0.00628981f);
                    _convUnitsList.Add(13, 33.814f);
                }
                return _convUnitsList;
            }
        }


        private IDictionary<ushort, String> _timebaseList;
        public IDictionary<ushort, String> TimebaseList
        {
            get
            {
                if (_timebaseList == null)
                {
                    _timebaseList = new Dictionary<ushort, String>();
                    _timebaseList.Add(0, "--");
                    _timebaseList.Add(1, "s");
                    _timebaseList.Add(2, "m");
                    _timebaseList.Add(3, "h");
                    _timebaseList.Add(4, "d");
                }
                return _timebaseList;
            }
        }

        private static IDictionary<byte, float> _convTimebaseList;
        public static IDictionary<byte, float> ConvTimebaseList
        {
            get
            {
                if (_convTimebaseList == null)
                {
                    _convTimebaseList = new Dictionary<byte, float>();
                    _convTimebaseList.Add(0, 0);
                    _convTimebaseList.Add(1, 0.0001f);
                    _convTimebaseList.Add(2, 0.006f);
                    _convTimebaseList.Add(3, 0.36f);
                    _convTimebaseList.Add(4, 8.64f);
                }
                return _convTimebaseList;
            }
        }


        private IDictionary<ushort, String> _progoutList;
        public IDictionary<ushort, String> ProgOutList
        {
            get
            {
                if (_progoutList == null)
                {
                    _progoutList = new Dictionary<ushort, String>();
                    _progoutList.Add(0, "Disabled");
                    _progoutList.Add(1, "Reverse Flow");
                    _progoutList.Add(2, "Max Flowrate Threshold");
                    _progoutList.Add(3, "Min Flowrate Threshold");
                    _progoutList.Add(4, "Min/Max Flowrate Threshold");
                    _progoutList.Add(5, "Batching");
                    _progoutList.Add(6, "EXC Failure");
                    _progoutList.Add(7, "Empty Pipe");
                    _progoutList.Add(8, "All Alarms");
                }
                return _progoutList;
            }
        }

        private IDictionary<ushort, String> _proginList;
        public IDictionary<ushort, String> ProgInList
        {
            get
            {
                if (_proginList == null)
                {
                    _proginList = new Dictionary<ushort, String>();
                    _proginList.Add(0, "Disabled");
                    _proginList.Add(1, "Reset Partial -");
                    _proginList.Add(2, "Reset Partial +");
                    _proginList.Add(3, "Reset Partial +/-");
                    _proginList.Add(4, "Partial Reset & Block");
                }
                return _proginList;
            }
        }
        
        private IDictionary<ushort, String> _languagesList;
        public IDictionary<ushort, String> LanguagesList
        {
            get
            {
                if (_languagesList == null)
                {
                    _languagesList = new Dictionary<ushort, String>();
                    _languagesList.Add(0, "English");
                    _languagesList.Add(1, "Italiano");
                    _languagesList.Add(2, "Español");
                    _languagesList.Add(3, "Português");
                    _languagesList.Add(4, "Français");
                }
                return _languagesList;
            }
        }

        public static class Logo
        {

            public static byte Logo_Indice
            {
                set
                {
                    Map.Registri_CMD3_16.Logo.Reg_value = value;
                    Parametri_EE_byte[0x4A] = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[0x4A];
                    else
                        return Map.Registri_CMD3_16.Logo.LSB_Byte;
                }
            }
        }
        public static byte   Lingua_Indice
        {
            set
            {
                Map.Registri_CMD3_16.menu_language.Reg_value = value;
                Parametri_EE_byte[0x00D0] = value;
            }
            get
            {
                if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                    return Parametri_EE_byte[0x00D0];
                else
                    return Map.Registri_CMD3_16.menu_language.LSB_Byte;
            }
        }
 
        public float MaxPulsesFreq // float massima frequenza impulsi a batterie
        {
            set
            {
                Map.Registri_CMD3_16.MaxPulsesFreq.Float_Modbus = value;
                byte[] byteArray = BitConverter.GetBytes(value);
                Array.Copy(byteArray, 0, Parametri_EE_byte, 0x0109, 4);
            }
            get
            {
                if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                    return BitConverter.ToSingle(Parametri_EE_byte, 0x0109);
                else
                    return Map.Registri_CMD3_16.MaxPulsesFreq.Float_Modbus;
            }
        }
 
/*
    EEProm_copy[11] = EE_read_byte(MeasSpaceSet_ADDR);// indice tabella pausa tra due misure batteria
	EEProm_copy[12] = EE_read_byte(MeasSpaceCont_ADDR); // indice tabella pausa tra misure continua
	EEProm_copy[13] = EE_read_byte(MeasSpaceWakeup_ADDR); //intervallo misura a batterie quando viene svegliata	    
 */
        public ushort Pausa_TraMisure_Batt
        {
            set
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                Array.Copy(byteArray, 0, Parametri_EE_byte, (int)ADDRESS_EE.MeasSpaceSet, 2);
                Map.Registri_CMD3_16.MeasSpaceSet.Reg_value = value;
            }
            get
            {
                // ***** DA INSERIRE TABELLA *****
                if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                    return BitConverter.ToUInt16(Parametri_EE_byte, 0xD5);
                else
                    return Map.Registri_CMD3_16.MeasSpaceSet.Reg_value;
            }
        }
        public ushort Frequenza_FS // Hz
        {
            set
            {
                if (value > 10000)
                    value = 10000;
                byte[] byteArray = BitConverter.GetBytes(value);
                Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00CC, 2);
                Map.Registri_CMD3_16.FS_FREQ.Reg_value = value;
            }
            get
            {
                if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                    return (ushort)(BitConverter.ToInt16(Parametri_EE_byte, 0x00CC));
                else
                    return Map.Registri_CMD3_16.FS_FREQ.Reg_value;
            }
        } 
  
        #endregion

        #region Filtri

        public static class Filtri
        {
            // ********* DAMPING ***************************

            public static ushort Damping_min = 5;
            public static ushort Damping_max = 500;
            public static ushort Damping
            {
                set
                {
                    if (value > Damping_max)
                        value = Damping_max;
                    if (value < Damping_min)
                        value = Damping_min;
                 
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                    {
                        byte[] byteArray = BitConverter.GetBytes(value);
                        Array.Copy(byteArray, 0, Parametri_EE_byte, (int)ADDRESS_EE.Damping, 2);
                    }
                    else
                        Map.Registri_CMD3_16.Damping.Reg_value = value;
                }

                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return  BitConverter.ToUInt16(Parametri_EE_byte,(int) ADDRESS_EE.Damping);
                    else
                        return Map.Registri_CMD3_16.Damping.Reg_value;
                }
            }

            // *************** Average ***************************************

            public const ushort Average_min = 1;
            public const ushort Average_max = 500;
            public static ushort Average
            {
                set
                {
                    Map.Registri_CMD3_16.AverageFilter.Reg_value = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, (int)ADDRESS_EE.Average, 2);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, (int)ADDRESS_EE.Average);
                    else
                        return (byte)Map.Registri_CMD3_16.AverageFilter.Reg_value;
                }
            }
            // *************** Frequenza RETE ***************************************

            public static ushort LineFreq
            {
                set
                {
                    Map.Registri_CMD3_16.LineFreq.Reg_value = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00DE, 2);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, 0x00DE);
                    else
                        return (byte)Map.Registri_CMD3_16.LineFreq.Reg_value;
                }
            }
            // ******* Cut_Off ***************************************

            public static byte CutOff_min = 0;
            public static byte CutOff_max = 50;
            public static byte CutOff
            {
                set
                {
                    Parametri_EE_byte[(int)ADDRESS_EE.CutOff] = value;
                    Map.Registri_CMD3_16.CutOffPercent.Reg_value = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[(int)ADDRESS_EE.CutOff];
                    else
                        return (byte)Map.Registri_CMD3_16.CutOffPercent.Reg_value;

                }
            }

            // *************** ByPass ***************************************

            public static byte Bypass_min = 2;
            public static byte Bypass_max = 95;
            public static byte Bypass
            {
                set
                {
                    Map.Registri_CMD3_16.FilterBypassTh.Reg_value = value;
                    Parametri_EE_byte[(int)ADDRESS_EE.Bypass] = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[(int)ADDRESS_EE.Bypass];
                    else
                        return (byte)Map.Registri_CMD3_16.FilterBypassTh.Reg_value;
                }
            }

            // *************** Peak_Cut ***************************************

            public static byte Peak_Cut_min = 1;
            public static byte Peak_Cut_max = 25;
            public static byte Peak_Cut
            {
                set
                {
                    Map.Registri_CMD3_16.PeakCutTh.Reg_value = value;
                    Parametri_EE_byte[(int)ADDRESS_EE.Peak_Cut] = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[(int)ADDRESS_EE.Peak_Cut];
                    else
                        return (byte)Map.Registri_CMD3_16.PeakCutTh.Reg_value;
                }
            }
        }
       
        #endregion

        #region Impulsi
        public static class Impulsi
        {
            public static void Reset()
            {
                Map.Registri_CMD3_16.VpulseUT.Reg_value = 0;
                Array.Clear(Map.Registri_CMD3_16.PQtyMain.Reg_value, 0, Map.Registri_CMD3_16.PQtyMain.Reg_value.Length);
                Map.Registri_CMD3_16.PONmain_ms.Reg_value = 0;

            }

//   *******  uscita impulsi anche con portata negativa ****** 

            public static byte PULSES_NEG     
            {
                set
                {
                    Map.Registri_CMD3_16.PULSES_NEG.Reg_value = value;
                    Parametri_EE_byte[(int)ADDRESS_EE.Pulse_Neg] = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[(int)ADDRESS_EE.Pulse_Neg];
                    else
                        return (byte)Map.Registri_CMD3_16.PULSES_NEG.Reg_value;
                }
            }

            public static ushort UT_Indice
            {
                set
                {
                    Map.Registri_CMD3_16.VpulseUT.Reg_value = value;
                    Parametri_EE_byte[0xB3] = (byte)value; // IrDA
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[0xB3];
                    else
                        return Map.Registri_CMD3_16.VpulseUT.Reg_value;
                }
            }

            public static string UT_Name
            {
                get{return UTValueRes.GetString("UT" + Impulsi.UT_Indice.ToString());}
            }

            public static float UT_Scala
            {
                get{return Convert.ToSingle(UTValueRes.GetString("UT" + Impulsi.UT_Indice.ToString() + "S"), CultureInfo.CreateSpecificCulture("it-IT"));}
            }

            /// <summary>
            /// SET : valore Float nell'unità corrente
            /// GET : recupera valore float nell'unità corrente,
            /// 10.000 = 1L ver2.xx  1000 = 1L ver 3.xx
            /// </summary>
            public static float Volume_UT_corrente
            {
                set
                {
                    Volume_ml = (value / Impulsi.UT_Scala) * 1000;
                }
                get
                {
                    return Volume_ml / 1000 * Impulsi.UT_Scala;
                }
            }

            public static float Volume_ml
            {
                set
                {
                    if (MC608.Release_FW.Versione < 3)
                        value = value * 10;

                    Map.Registri_CMD3_16.PQtyMain.Float_Modbus = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00B4, 4);
                }
                get
                {
                    float tmp_volume = 0;
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        tmp_volume = BitConverter.ToSingle(Parametri_EE_byte, 0x00B4);
                    else
                        tmp_volume = Map.Registri_CMD3_16.PQtyMain.Float_Modbus;

                    if(Release_FW.Versione<3)
                        tmp_volume =  tmp_volume/10;

                    return tmp_volume;
                }
            }

            public static ushort Durata_ms // Secondi
            {
                set
                {
                    if (Release_FW.Versione >= 3)
                    {
                        if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        {
                            byte[] byteArray = BitConverter.GetBytes(value);
                            Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00B8, 2);
                        }
                        else
                            Map.Registri_CMD3_16.PONmain_ms.Reg_value = value;
                    }
                    else
                    {
                        if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        {
                            byte[] byteArray = BitConverter.GetBytes(value * 2);
                            Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00B8, 2);
                        }
                        else
                            Map.Registri_CMD3_16.PONmain_ms.Reg_value = (ushort)(value * 2);
                    }

                }
                get
                {
                    if (Release_FW.Versione >= 3)
                    {
                        if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                            return (ushort)(BitConverter.ToInt16(Parametri_EE_byte, 0x00B8));
                        else
                            return Map.Registri_CMD3_16.PONmain_ms.Reg_value;
                    }
                    else
                    {
                        if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                            return (ushort)(BitConverter.ToInt16(Parametri_EE_byte, 0x00B8)/2);
                        else
                            return (ushort)(Map.Registri_CMD3_16.PONmain_ms.Reg_value/2);
                    }
                }
            }

        } // end class Impulsi      
        #endregion

        #region Parametri bobine e decimatore
        public static class Bobine
        {
            public static byte I_Coil
            {
                set
                {
                    Map.Registri_CMD3_16.ICoil.Reg_value = value;
                    Parametri_EE_byte[0x0043] = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[0x0043];
                    else
                        return (byte)Map.Registri_CMD3_16.ICoil.Reg_value;    
                }
            }
            static float _I_Coil_set;
            public static float I_Coil_set // Scrittura corrente letta da collaudo
            {
                set
                {
                    _I_Coil_set = value;
                }
                get
                {
                    return _I_Coil_set;
                }
            }
            public static ushort Coil_on1
            {
                set
                {
                    Map.Registri_CMD3_16.COIL_ON_1_WAIT.Reg_value = value;

                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00EA, 2);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, 0xEA);
                    else
                        return (byte)Map.Registri_CMD3_16.COIL_ON_1_WAIT.Reg_value;
                }
            }

            public static ushort Coil_on2
            {
                set
                {
                    Map.Registri_CMD3_16.COIL_ON_2_WAIT.Reg_value = value;

                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00EC, 2);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, 0xEC);
                    else
                        return (byte)Map.Registri_CMD3_16.COIL_ON_2_WAIT.Reg_value;
                }
            }

            public static ushort Coil_Off
            {
                set
                {
                    Map.Registri_CMD3_16.COIL_OFF.Reg_value = value;

                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00EE, 2);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, 0xEE);
                    else
                        return (byte)Map.Registri_CMD3_16.COIL_OFF.Reg_value;
                }
            }

            public static ushort Coil_Pause_Inv
            {
                set
                {
                    Map.Registri_CMD3_16.T_INV_WAIT.Reg_value = value;

                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00F0, 2);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, 0xF0);
                    else
                        return (byte)Map.Registri_CMD3_16.T_INV_WAIT.Reg_value;
                }
            }

            public static ushort ADC_dec
            {
                set
                {
                    Map.Registri_CMD3_16.FilterADC.Reg_value = value;

                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x00FE, 2);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return BitConverter.ToUInt16(Parametri_EE_byte, 0xFE);
                    else
                        return (byte)Map.Registri_CMD3_16.FilterADC.Reg_value;
                }
            }
        }
        #endregion

        #region Gestione Portata 
        public static class Portata
        {
            public static void Reset()
            {
                Map.Registri_CMD3_16.VflowUT.Reg_value = 0;
                Map.Registri_CMD3_16.TflowUT.Reg_value = 0;
            }
            public static float Full_Scale_M3h // float FONDO SCALA MISURATORE
            {
                set
                {
                    Map.Registri_CMD3_16.FullScale_m3.Float_Modbus = value;
                    byte[] byteArray = BitConverter.GetBytes(value);
                    Array.Copy(byteArray, 0, Parametri_EE_byte, 0x0055, 4);
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                    {
                        if (BitConverter.ToSingle(Parametri_EE_byte, 0x0055) < 0)
                            return 0;
                        else
                            return BitConverter.ToSingle(Parametri_EE_byte, 0x0055);
                    }
                    else
                        return Map.Registri_CMD3_16.FullScale_m3.Float_Modbus;
                }
            }
            public static float FullScale_Scalato
            {
                get
                {
                    float fattore = 1;
                    switch (Portata.TB_Indice)
                    {
                        case 1:
                        fattore = 3600; // secondi
                        break;
                        case 2:
                        fattore = 60; // Minuti                                                                                                                                      
                        break;
                        case 3:
                        fattore = 1; // Ore
                        break;
                        case 4:
                        fattore = 1f / 24f; // giorni
                        break;
                    }
                    //                                            m3 = 0,001
                    float _Full_Scale = (Full_Scale_M3h * 1000 * Portata.UT_Scala_fattore) / fattore; // Porto tutto in litri

                    return _Full_Scale;

                }
            }

            public static byte TB_Indice
            {
                set
                {
                    Parametri_EE_byte[0xB0] = value;
                    Map.Registri_CMD3_16.TflowUT.Reg_value = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[0xB0];
                    else
                        return (byte)Map.Registri_CMD3_16.TflowUT.Reg_value;
                }
            }
            public static string TB_Name
            {
                get {return TBValueRes.GetString("TB" + Portata.TB_Indice.ToString());}
            }
            public static float TB_Scala
            {
                get 
                {
                    return ConvTimebaseList[Portata.TB_Indice];
                }
            }

            public static byte UT_Indice
            {
                set
                {
                    Parametri_EE_byte[0xAF] = value;
                    Map.Registri_CMD3_16.VflowUT.Reg_value = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[0xAF];
                    else
                        return (byte)Map.Registri_CMD3_16.VflowUT.Reg_value;
                }
            }
            public static string UT_Name
            {
                get
                {
                    return UTValueRes.GetString("UT" + Portata.UT_Indice.ToString());
                }
            }
            public static float UT_Scala_fattore
            {
                get
                {
                    return ConvUnitsList[Portata.UT_Indice];
                }
            } 
        }
 
        /// <summary>
        /// Coefficente conversione valore portata ist datalogger
        /// </summary>
        public static float K_main_datalogger
        {
            get
            {
                float fattore = 1;
                switch (Portata.TB_Indice)
                {
                    case 1:
                    fattore = 1f/3600f; // secondi
                    break;
                    case 2:
                    fattore = 1f/60f; // Minuti
                    break;
                    case 3:
                    fattore = 1; // Ore
                    break;
                    case 4:
                    fattore = 24; // giorni
                    break;
                }
                return (1000 * Portata.UT_Scala_fattore * fattore);
               
            }
        }

        public static ushort Auto_PowerOff_s
        {
            set
            {
                Map.Registri_CMD3_16.AutoPowerOffMem.Reg_value = value;
                Parametri_EE_byte[0x00BA] = (byte)value;
            }
            get
            {
                if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                    return Parametri_EE_byte[0x00BA];
                else
                    return Map.Registri_CMD3_16.AutoPowerOffMem.Reg_value;
            }
        }

        #endregion

        #region VaccUT Gestisco unità tecnica volume accumulatori

        public static class Totalizzatori
        {
            
            /// <summary>
            /// Restituisce Unità tecnica Volume
            /// </summary>
            public static string UT_name
            {
                get{return UTValueRes.GetString("UT" + UT_Indice.ToString());}
            }

            /// <summary>
            /// Restituisce Fattore conversione Volume
            /// </summary>
            public static float UT_Scala
            {
                get{ return Convert.ToSingle(UTValueRes.GetString("UT" + UT_Indice.ToString() + "S")); }
            }
            public static byte UT_Indice
            {
                set
                {
                    Map.Registri_CMD3_16.VaccUT.Reg_value = value;
                    Parametri_EE_byte[0xB1] = value;
                }
                get
                {
                    if (Tipo_Connessione == Comunicazione.Connesso_IrDA)
                        return Parametri_EE_byte[0xB1];
                    else
                        return (byte)Map.Registri_CMD3_16.VaccUT.Reg_value;
                }
            }
        }
        #endregion
 
        private static string Formatta_nChar(string String_in, byte Count)
        {
            string String_out = String_in;

            for (int i = 0; i < (Count - String_in.Length); i++)
            {
                String_out += " ";
            }
            return String_out;
        }
        private static string Leggi_stringa_EE(int address, byte Count)
        {
            char[] temp_char = new char[Count];
            Array.Copy(Parametri_EE_byte, address, temp_char, 0, temp_char.Length);
            return new string(temp_char, 0, temp_char.Length);
        }

        public static int MeansSpace_ms(byte indice)
        {
            if (indice > 0 || indice <= MeansSpace.Length)
                return MeansSpace[indice];
            else
                return 0;
        }

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
