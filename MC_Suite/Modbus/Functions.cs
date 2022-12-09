using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;  // per comunicazione IrDA e COM
using System.Threading.Tasks;


namespace MC_Suite.Modbus
{
    public partial class Functions
    {        

        public static ushort[,] Array_Register = new ushort[25, 1792]; // registri cmd,value reg

        public Functions()
        {
            Array.Clear(Array_Register, 0, Array_Register.Length);
        }

        public static void Forza_EEload(byte Address)
        {
            //await CMD_MC608(Address, 0x682, (ushort)MC608.CMD.EELOAD_MDB, (ushort)MC608.Password.KEY_WR_DATI_FABBRICA, 3);
        }

        public static void Read_OffSet(byte Address)
        {
            ushort[] Offset = new ushort[4];
//            Protocol.Read_Timeout = TimeSpan.FromMilliseconds(2000);

            Functions.Forza_EEload(Address);

            if (Functions.Read(Address, CMD.CMD03, 1160, 1161, 3))
            {
                MC608.Taratura.ManualOffSet = Map.Registri_CMD3_16.ManualOffset.Float_Modbus;
            }
            else
            {
                MC608.Taratura.ManualOffSet = float.NaN;
                return;
            }
        }


        public static void Reset()
        {
            Array.Clear(Array_Register, 0, Array_Register.Length);
        }

 
        public static void wait_EELoad()
        {
            System.Threading.Thread.Sleep(2000);
        }

        public enum CMD
        { 
            CMD03 = 03,
            CMD04 = 04,
            CMD16 = 16
        }
        public static bool Read(byte Address, CMD command, ushort reg_start, ushort reg_stop, byte tentativi)
        {
            ushort[] tmp = new ushort[reg_stop - reg_start + 1];
            byte comando = (byte)command;

            for (int i = 0; i < tentativi; i++)
            {
                //tmp = await Protocol.Instance.SendFc(Address, comando, reg_start, (ushort)tmp.Length);
                if (tmp != null)
                {
                    for (int a = 0; a < tmp.Length; a++)
                    {
                        Array_Register[comando, reg_start + a] = tmp[a];
                    }
                    Map.error_mdb = false;
                    return true;
                }
            }
            Map.error_mdb = true;
            return false;
        }
 
        /*public static bool CMD_MC608(byte Address, ushort reg_cmd, ushort CMD, ushort PSW, byte tentativi)
        {
            ushort[] comando = { (ushort)((PSW << 8) + CMD) };

            for (int i = 0; i < tentativi; i++)
            {
                //if (await Protocol.Instance.SendFc16(Address, reg_cmd, (ushort)1, comando))
                {
                    Map.error_mdb = false;
                    return true;
                }
            }
            Map.error_mdb = true;
            return false;
        }*/

        public static bool SendCMD(byte Address, Command_mdb _Comando, byte tentativi)
        {
            /*try
            {
                ushort[] cmd_mdb = new ushort[1];
                cmd_mdb[0] = _Comando.reg_value;
                byte command = _Comando.cmd;
                for (int i = 0; i < tentativi; i++)
                {
                    if (await Protocol.Instance.SendFc16(Address, _Comando.address, (ushort)cmd_mdb.Length, cmd_mdb))
                    {
                        Map.error_mdb = false;
                        return true;
                    }
                }
                Map.error_mdb = true;
                return false;
            }
            catch (Exception)
            {
                throw;
            }*/
            return false;
        }
        
        public static bool CMD_MC608_EN_WR_ModBus(byte Address, byte tentativi)
        {
            /*short[] comando = { (ushort)(((int)MC608.Password.KEY_WR_DATI_FABBRICA << 8) + MC608.CMD.EN_WR_MODBUS) };

            for (int i = 0; i < tentativi; i++)
            {
                if (await Protocol.Instance.SendFc16(Address, 0x0682, (ushort)1, comando))
                {
                    Map.error_mdb = false;
                    return true;
                }
            }
            Map.error_mdb = true;*/
            return false;
        }     
        public static bool CMD_MC608_RESET(byte Address, byte tentativi)
        {
            /*ushort[] comando = { (ushort)(((int)MC608.Password.KEY_WR_DATI_FABBRICA << 8) + MC608.CMD.Reset) };

            for (int i = 0; i < tentativi; i++)
            {
                if (await Protocol.Instance.SendFc16(Address, (ushort)MC608.ADDRESS_EE.CMD, (ushort)1, comando))
                {
                    Map.error_mdb = false;
                    return true;
                }
            }
            Map.error_mdb = true;*/
            return false;
        }

        public class Register
        {

            private ushort _address;
            public ushort address
            {
                set
                {
                    _address = value;
                }
                get
                {
                    return _address;
                }
            }
            private ushort _address_end;
            public ushort address_end
            {
                set
                {
                    _address_end = value;
                }
                get
                {
                    return _address_end;
                }
            }

            byte m_cmd = 0;
            public byte cmd
            {
                set
                {
                    m_cmd = value;
                }
                get
                {
                    return m_cmd;
                }
            }    
        }
        public class Float32_mdb : Register
        {
            public bool reverse;
            
            public Float32_mdb(ushort _addr, byte _cmd)
            {
                address = _addr;
                address_end = (ushort)(address + 1);
                cmd = _cmd;
                Array.Clear(Reg_value, 0, Reg_value.Length);
            }


            public ushort[] Reg_value
            {
                set
                {
                        Array_Register[cmd,address] = value[0];
                        Array_Register[cmd,address + 1] = value[1];
                }
                get
                {
                    ushort[] _long_val = { Array_Register[cmd,address], Array_Register[cmd,address + 1] };
                    return _long_val;
                }
            }
            public float Float_Modbus
            {
                set
                {
                    Scrivi_float_ModBus(ref Array_Register, address, value);
                }
                get
                {
                    return Leggi_float_ModBus(ref Array_Register, address);
                }
            }

            private void Scrivi_float_ModBus(ref ushort[,] mdb_array, ushort address, float valore_float)
            {
                byte[] byte_array = BitConverter.GetBytes(valore_float);
                ushort word_LO = BitConverter.ToUInt16(byte_array, 0);
                ushort word_HI = BitConverter.ToUInt16(byte_array, 2);

                if (Map.Reverse_mdb)
                {
                    mdb_array[cmd,address + 1] = (ushort)(word_LO);
                    mdb_array[cmd,address] = (ushort)(word_HI);
                }
                else
                {
                    mdb_array[cmd,address + 1] = (ushort)(word_HI);
                    mdb_array[cmd,address] = (ushort)(word_LO);
                }
            }

            private float Leggi_float_ModBus(ref ushort[,] mdb_array, ushort address)
            {
                byte[] word_LO;
                byte[] word_HI;

                if (Map.Reverse_mdb)
                {
                    word_HI = BitConverter.GetBytes(mdb_array[cmd,address]);
                    word_LO = BitConverter.GetBytes(mdb_array[cmd,address + 1]);
                }
                else
                {
                    word_LO = BitConverter.GetBytes(mdb_array[cmd,address]);
                    word_HI = BitConverter.GetBytes(mdb_array[cmd,address + 1]);
                }

                byte[] flow_byte = { word_LO[0], word_LO[1], word_HI[0], word_HI[1] };
                float misura = BitConverter.ToSingle(flow_byte, 0);
                return misura;
            }
        }

        public class Int32_mdb : Register
        {
            public Int32_mdb(ushort _addr, byte _cmd)
            {
                address = _addr;
                address_end = (ushort)(address + 1);
                cmd = _cmd;
                Array.Clear(Reg_value, 0, Reg_value.Length);
            }

            public ushort[] Reg_value
            {

                set
                {
                    Array_Register[cmd, address] = value[0];
                    Array_Register[cmd,address + 1] = value[1];
                }
                get
                {
                    ushort[] _long_val = { Array_Register[cmd,address], Array_Register[cmd,address + 1] };
                    return _long_val;
                }
            }
            public UInt32 Long32_Modbus
            {
                set
                {
                    Scrivi_uint_ModBus(ref Array_Register, address, value);
                }
                get
                {
                    return Leggi_uint_ModBus(ref Array_Register, address);
                }
            }

            private void Scrivi_uint_ModBus(ref ushort[,] mdb_array, ushort address, UInt32 valore_32)
            {
                byte[] byte_array = BitConverter.GetBytes(valore_32);
                ushort word_LO = BitConverter.ToUInt16(byte_array, 0);
                ushort word_HI = BitConverter.ToUInt16(byte_array, 2);
                if (Map.Reverse_mdb)
                {
                    mdb_array[cmd,address] = (ushort)(word_HI);
                    mdb_array[cmd,address + 1] = (ushort)(word_LO);
                }
                else
                {
                    mdb_array[cmd,address] = (ushort)(word_LO);
                    mdb_array[cmd,address + 1] = (ushort)(word_HI);
                }
            }

            private UInt32 Leggi_uint_ModBus(ref ushort[,] mdb_array, ushort address)
            {
                UInt32 word_LO;
                UInt32 word_HI;

                if (Map.Reverse_mdb)
                {
                    word_LO = mdb_array[cmd,address + 1];
                    word_HI = mdb_array[cmd, address];
                }
                else
                {
                    word_HI = mdb_array[cmd, address + 1];
                    word_LO = mdb_array[cmd, address];
                }
                return ((word_HI << 16) + word_LO);
            }
        }

        public class Registro_mdb : Register
        {
            public Registro_mdb(ushort _addr, byte _cmd)
            {
                address = _addr;
                address_end = _addr;
                cmd = _cmd;
                Reg_value = 0;
                bool_value = false;
            }

            public bool bool_value
            {
                set
                {
                    if (value)
                        Array_Register[cmd, address] = 1;
                    else
                        Array_Register[cmd, address] = 0;
                }
                get
                {
                    ushort res = Array_Register[cmd, address];
                    if (res == 1)
                        return true;
                    else
                        return false;
                }
            }


            public ushort Reg_value
            {
                set
                {
                    Array_Register[cmd,address] = value;
                }
                get
                {
                    return Array_Register[cmd,address];
                }
            }
            public byte LSB_Byte
            {
                get
                {
                    return (byte)Array_Register[cmd,address];
                }
                set
                {
                    Array_Register[cmd, address] = (ushort)((ushort)(value) | (Array_Register[cmd, address] & 0xFF00));
                }
            }
            public byte MSB_Byte
            {
                get
                {
                    return (byte)(Array_Register[cmd,address]>>8);
                }
                set
                {
                    Array_Register[cmd, address] = (ushort)((ushort)(value << 8) | (Array_Register[cmd, address] & 0x00FF));
                }
            }

        }
      
        public class Command_mdb : Register
        {
            private ushort _reg_value = 0;


            public enum KEY
            {
                 KEY_AZZERA_TOTALIZ	    = 0x55,
                 KEY_AZZERA_PARZ        = 0x56,
                 KEY_WR_DATI_FABBRICA   = 0xD6, // Chiave da mettere nel byte HI per accedere alla scrittura
                 KEY_AUTOZERO		    = 0x40,
                 KEY_AZZERA_LOG		    = 0x7F,
                 KEY_COLLAUDO		    = 0x88
            }
            public enum CMD
            {
                 CMD_RESET_MAIN 	=	44,
                 CMD_RESET_TOT 		=   35,
                 CMD_RESET_PARZ_N   =	25, // Utente
                 CMD_RESET_PARZ_P   =	15,
                 CMD_ZERO_CAL 	   	=   202,
                 CMD_COPY_UTENTE    =   110,
                 CMD_COPY_FABBRIC   =   120,
                 CMD_COPY_UT_FABBR  =	105,
                 CMD_RESET_DATALOG  =   89,	//Utente
                 CMD_RESET_SYSLOG	=   75,
                 CMD_LOAD_DATA_SENS =   70,
                 CMD_INIT_MISURA    =   71,
                 CMD_CALIBRATION_420=	54, //MenuPos=1530
                 CMD_EEPROM_TOT_INIT=   61,
                 CMD_FLASH_CHECK	=	199,
                 CMD_ALIGN_KA	    =	185,
                 CMD_OFFS_KA		=   186,
                 CMD_ENABLE_SIMUL   =	144,
                 CMD_EN_WR_MODBUS   =	133,
                 CMD_EELOAD_MDB     =   178,
                 CMD_SET_IO_MDB     =   155,
                 CMD_SET_BCK		=	12,
                 CMD_INIT_SENSOR_TABLE= 111,
                 CMD_TEST_MODE 	    =	210,
                 CMD_TEST_EMPTP     =	220,
                 CMD_ACT_COIL	    =	169,
                 CMD_SWITCH_COM     =   176,
                 CMD_ENTER_VERIFICATION = 230,
                 CMD_EXIT_VERIFICATION = 231
            }

            public Command_mdb(ushort _addr, CMD _cmd, KEY _value)
            {
                const ushort Start_CMD = 0x0680;
                address = (ushort)(Start_CMD + _addr);
                address_end = address;

                cmd = 03;
                _reg_value = 0;
                Valore = (byte)_value;
                Comando = (byte)_cmd;

            }

            public byte Comando // LSB
            {
                set
                {
                    _reg_value = (ushort)(_reg_value & 0xFF00);
                    _reg_value = (ushort)(_reg_value | value);

                }
            }
            public byte Valore // MSB
            {
                set
                {
                    _reg_value = (ushort)(_reg_value & 0x00FF);
                    _reg_value = (ushort)(_reg_value | (value << 8));
                }
            }
            public ushort reg_value
            {
                get
                {
                    return _reg_value;
                }
            }
        }
        public class Stringa_mdb : Register
        {
            /// <summary>
            /// inserisce stringa nell'array modbus
            /// </summary>
            /// <param name="_addr">registro</param>
            /// <param name="num_char">numero caratteri, deve essere pari</param>
            public Stringa_mdb(ushort _addr, byte num_char, byte _cmd)
            {
                address = _addr;
                cmd = _cmd;
                n_char = num_char;

                if (num_char % 2 == 0) // n_char deve essere sempre pari
                    address_end = (ushort)(address + (ushort)(num_char / 2));
                else
                    address_end = (ushort)(address + (ushort)((num_char +1 ) / 2));

                Text = "".PadLeft(num_char, '-');
            }

            public byte n_char;

            public string Text
            {
                set
                {
                    Scrivi_stringa_ModBus(ref Array_Register, address, value, n_char);
                }
                get
                {
                    return Leggi_stringa_ModBus(Array_Register, address, n_char);
                }

            }
            private void Scrivi_stringa_ModBus(ref ushort[,] mdb_array, ushort address, string stringa, byte n_char)
            {
                if (stringa.Length > n_char)
                    stringa = stringa.Remove(n_char);
                else
                    stringa = stringa.PadRight(n_char);

                byte count = n_char;

                if (n_char % 2 != 0) // n_char deve essere sempre pari
                    count ++;

                stringa = stringa.PadRight(count); // aggiungo spazio se caratteri dispari
                byte[] temp_byte = Encoding.ASCII.GetBytes(stringa);

                for (int i = 0; i < (count / 2); i++)
                {
                    mdb_array[cmd,address + i] = (ushort)(temp_byte[(i * 2)] << 8);
                    mdb_array[cmd,address + i] += (ushort)(temp_byte[(i * 2 + 1)]);
                }
            }
            private string Leggi_stringa_ModBus(ushort[,] mdb_array, ushort address, byte char_count)
            {
                char[] temp_char;

                if (char_count % 2 != 0) // n_char deve essere sempre pari
                    temp_char = new char[char_count+1];
                else
                    temp_char = new char[char_count];

                for (int i = 0; i < temp_char.Length / 2; i++)
                {
                    byte[] byteArray = BitConverter.GetBytes(mdb_array[cmd,i + address]);
                    temp_char[ (i *2)] = Convert.ToChar(byteArray[1]);
                    temp_char[ (i *2) + 1] = Convert.ToChar(byteArray[0]);
                }
                string testo = new string(temp_char, 0, temp_char.Length);

                if (testo.Length > char_count)
                    return testo.Remove(char_count);
                else if (testo.Length < char_count)
                    return testo.PadRight(char_count);
                else
                    return testo;
            }
        }
        public enum BitField
        {
            GP_out = 1 << 0,
            Freq_Out = 1 << 1,
            Pulse_Out = 1 << 2,
            nc = 1 << 3,
            EXP1 = 1 << 4,
            EXP2 = 1 << 5,
            EXP3 = 1 << 6,
            EXP4 = 1 << 7,

        }
    }
 
}
