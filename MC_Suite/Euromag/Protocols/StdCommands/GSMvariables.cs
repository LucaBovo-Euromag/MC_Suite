using Euromag.Utility.Endianness;
namespace MC_Suite.Euromag.Protocols.StdCommands
{
    using System;
    using System.Collections.Generic;

    /// <REVNOTES 16-02-2018 >
    ///  REVNOTES 16-02-2018 
    ///     Aggiornamneto offset varibli per aggiunta:
    ///     SMTP_PORT 
    ///     Sensor_ID
    ///     Converter_ID 
    ///  END 16-02-2018
    /// </REVNOTES 16-02-2018 >

    static class AllGSMVarsLister
    {
        public static ICollection<ITargetWritable> getList() 
        {
            List<ITargetWritable> list = new List<ITargetWritable>();
            
            list.Add(new Mobile_num_1());
            list.Add(new Mobile_num_2());
            list.Add(new Mobile_num_3());
            list.Add(new Mobile_num_4());
            list.Add(new Mobile_num_5());

            list.Add(new SMS_Service_center());

            list.Add(new Mail_address_1());
            list.Add(new Mail_address_2());
            list.Add(new Mail_address_3());
            list.Add(new Mail_address_4());
            list.Add(new Mail_address_5());

            list.Add(new Mail_address_sender());

            list.Add(new SMTP_server());
            list.Add(new SMTP_Port());
            list.Add(new SMTP_username());
            list.Add(new Smtp_password());

            list.Add(new APN());
            list.Add(new APN_username());
            list.Add(new APN_password());
         
            list.Add(new SIM_PIN());
            list.Add(new En_data_roaming());

            list.Add(new Hours_sms());
            list.Add(new Days_of_Week_sms());
            list.Add(new Days_of_Month_sms());

            list.Add(new Hours_email());
            list.Add(new Days_of_Week_email());
            list.Add(new Days_of_Month_email());

            list.Add(new Hours_email_att());
            list.Add(new Days_of_Week_email_att());
            list.Add(new Days_of_Month_email_att());

            list.Add(new Hours_Data());
            list.Add(new Days_of_Week_Data());
            list.Add(new Days_of_Month_Data());

            list.Add(new FTP_server());
            list.Add(new FTP_username());
            list.Add(new FTP_password());
            
            list.Add(new FTP_workdir_1());
            list.Add(new FTP_workdir_2());
            list.Add(new FTP_workdir_3());

            list.Add(new Email_send_by_ftp());

            list.Add(new Sensor_ID());
            list.Add(new Converter_ID());

            list.Add(new Enable_remote_update());

            return list.AsReadOnly();
        }
    }

    static class AllGSMTestVarsLister
    {
        public static ICollection<ITargetWritable> getList()
        {
            List<ITargetWritable> list = new List<ITargetWritable>();

            list.Add(new Operator_Connecteted());
            list.Add(new Signal_strength());
            list.Add(new Operator_name());
            list.Add(new Mail_Body());
            list.Add(new Mail_Attachment());
            list.Add(new Mail_Send());
            list.Add(new Mail_Send_Attach());

            list.Add(new SMS_Body());
            list.Add(new SMS_Send());
            list.Add(new SMS_Sending_Status());
            list.Add(new Mail_Sending_Status());
            list.Add(new FTP_Test_Connection());
            list.Add(new Data_Test_Connection());
            list.Add(new FTP_Test_Status());
            list.Add(new Data_Test_Status());
            list.Add(new Sync_Date_Time());
            list.Add(new Get_Date_Time());
            list.Add(new Modem_Reset());
            list.Add(new Modem_shutdown());
            list.Add(new GSM_Status());
            list.Add(new RESET_EEprom());

            return list.AsReadOnly();
        }
    }
    public enum GSMAddresses : uint
    {
        // rubrica
        Mobile_num_1 = 0x01,
        Mobile_num_2 = 0x02,
        Mobile_num_3 = 0x03,
        Mobile_num_4 = 0x04,
        Mobile_num_5 = 0x05,

        SMS_Service_Center = 0x06,

        Mail_address_1 = 0x07,
        Mail_address_2 = 0x08,
        Mail_address_3 = 0x09,
        Mail_address_4 = 0x10,
        Mail_address_5 = 0x11,
        
        //salto indirizzo  

        Mail_address_sender = 0x013,//0x13

        // SMTP Server
        Smtp_server   = 0x14,
        SMTP_Port     = 0x15,
        Smtp_username = 0x16,
        Smtp_password = 0x17,

        // APN  DATA 
        APN = 0x18,
        APN_username = 0x19,
        APN_password = 0x20,
        
        // SIM 
        SIM_PIN = 0x21,
        En_data_roaming = 0x22,

        // AutoReport
                                                     
        Hours_sms         = 0x23,
        Days_of_Week_sms  = 0x24,
        Days_of_Month_sms = 0x25,

        Hours_email         = 0x26,
        Days_of_Week_email  = 0x27,
        Days_of_Month_email = 0x28,

        Hours_email_att         = 0x29,
        Days_of_Week_email_att  = 0x30,
        Days_of_Month_email_att = 0x31,

        Hours_Data              = 0x32,
        Days_of_Week_Data       = 0x33,
        Days_of_Month_Data      = 0x34,

         
         // FTP Server

        FTP_server_name     = 0x35,
        FTP_username        = 0x36,
        FTP_password        = 0x37,
        FTP_workdir_1       = 0x38,
        FTP_workdir_2       = 0x39,
        FTP_workdir_3       = 0x40,

        Email_send_by_ftp   = 0x41,

        Sensor_ID           = 0x42,
        Conveter_ID         = 0x43,
        Enable_remote_update= 0x44,

        // TEST VAR

        Operator_Connecteted = 0x100,
        Signal_strength = 0x101,
        Operator_name   = 0x102,


        Mail_Body       = 0x103,
        Mail_Attachment = 0x104,
        Mail_Send       = 0x105,
        SMS_Body        = 0x106,
        SMS_Send        = 0x107,

        SMS_Sending_Status  = 0x108,
        Mail_Sending_Status = 0x109,
                             
        FTP_Test_Connection  = 0x110,
        Data_Test_Connection = 0x111,

        FTP_Test_Status     = 0x112,
        Data_Test_Status    = 0x113,

        Sync_Date_Time      = 0x114,
        Get_Date_Time       = 0x115,
        Modem_Reset         = 0x116,
        Modem_shutdown      = 0x117,
        GSM_Status          = 0x118,
        Mail_Send_Attach    = 0x119,
        RESET_EEPROM        = 0x120,
    }

    public enum GSM_UID : uint
    {
        // rubrica
        Mobile_num_1 = 0x01,
        Mobile_num_2 = 0x02,
        Mobile_num_3 = 0x03,
        Mobile_num_4 = 0x04,
        Mobile_num_5 = 0x05,

        SMS_Service_Center = 0x06,

        Mail_address_1 = 0x07,
        Mail_address_2 = 0x08,
        Mail_address_3 = 0x09,
        Mail_address_4 = 0x10,
        Mail_address_5 = 0x11,

        //salto indirizzo  

        Mail_address_sender = 0x013,//0x13

        // SMTP Server
        Smtp_server     = 0x14,
        SMTP_Port       = 0x15,
        Smtp_username   = 0x16,
        Smtp_password   = 0x17,

        // APN  DATA 
        APN             = 0x18,
        APN_username    = 0x19,
        APN_password    = 0x20,

        // SIM 
        SIM_PIN         = 0x21,
        En_data_roaming = 0x22,

        // AutoReport

        Hours_sms           = 0x23,
        Days_of_Week_sms    = 0x24,
        Days_of_Month_sms   = 0x25,

        Hours_email         = 0x26,
        Days_of_Week_email  = 0x27,
        Days_of_Month_email = 0x28,

        Hours_email_att         = 0x29,
        Days_of_Week_email_att  = 0x30,
        Days_of_Month_email_att = 0x31,

        Hours_Data          = 0x32,
        Days_of_Week_Data   = 0x33,
        Days_of_Month_Data  = 0x34,

        // FTP Server

        FTP_server_name = 0x35,
        FTP_username    = 0x36,
        FTP_password    = 0x37,
        FTP_workdir_1   = 0x38,
        FTP_workdir_2   = 0x39,
        FTP_workdir_3   = 0x40,

        Email_send_by_ftp = 0x41,

        Sensor_ID            = 0x42,
        Conveter_ID          = 0x43,
        Enable_remote_update = 0x44,

        // TEST VAR

        Operator_Connecteted = 0x100,
        Signal_strength      = 0x101,
        Operator_name        = 0x102,

        Mail_Body           = 0x103,
        Mail_Attachment     = 0x104,
        Mail_Send           = 0x105,
        SMS_Body            = 0x106,
        SMS_Send            = 0x107,

        SMS_Sending_Status  = 0x108,
        Mail_Sending_Status = 0x109,

        FTP_Test_Connection  = 0x110,
        Data_Test_Connection = 0x111,

        FTP_Test_Status     = 0x112,
        Data_Test_Status    = 0x113,

        Sync_Date_Time      = 0x114,
        Get_Date_Time       = 0x115,
        Modem_Reset         = 0x116,
        Modem_shutdown      = 0x117,
        GSM_Status          = 0x118,
        Mail_Send_Attach    = 0x119,
        RESET_EEPROM        = 0x120,
    }


    public interface IGSMvariable : ITargetWritable
    {
        UInt32 ID
        {
            get;
        }

        GSMAddresses Address
        { get; }
    }


    #region GSM VARIABLES
    // Mobile NUM 


    public class Mobile_num_1 : WritableStringTargetVariable, IGSMvariable
    {
        public Mobile_num_1()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mobile Number N 1";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mobile_num_1;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mobile_num_1;
            }
        }

    }
    public class Mobile_num_2 : WritableStringTargetVariable, IGSMvariable
    {
        public Mobile_num_2()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mobile Number N 2";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mobile_num_2;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mobile_num_2;
            }
        }
    }
    public class Mobile_num_3 : WritableStringTargetVariable, IGSMvariable
    {
        public Mobile_num_3()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mobile Number N 3";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mobile_num_3;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mobile_num_3;
            }
        }
    }
    public class Mobile_num_4 : WritableStringTargetVariable, IGSMvariable
    {
        public Mobile_num_4()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mobile Number N 4";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mobile_num_4;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mobile_num_4;
            }
        }
    }
    public class Mobile_num_5 : WritableStringTargetVariable, IGSMvariable
    {
        public Mobile_num_5()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mobile Number N 5";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mobile_num_5;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mobile_num_5;
            }
        }
    }

    public class SMS_Service_center : WritableStringTargetVariable, IGSMvariable
    {
        public SMS_Service_center()
            : base()
        {

        }

        public override string ToString()
        {
            return "SMS_Service_center";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.SMS_Service_Center;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.SMS_Service_Center;
            }
        }
    }

    //Mail_ adresses
    public class Mail_address_1 : WritableStringTargetVariable, IGSMvariable
    {
        public Mail_address_1()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mail Address  N 1";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_address_1;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_address_1;
            }
        }
    }
    public class Mail_address_2 : WritableStringTargetVariable, IGSMvariable
    {
        public Mail_address_2()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mail Address  N 2";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_address_2;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_address_2;
            }
        }
    }
    public class Mail_address_3 : WritableStringTargetVariable, IGSMvariable
    {
        public Mail_address_3()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mail Address  N 3";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_address_3;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_address_3;
            }
        }
    }
    public class Mail_address_4 : WritableStringTargetVariable, IGSMvariable
    {
        public Mail_address_4()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mail Address  N 4";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_address_4;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_address_4;
            }
        }
    }
    public class Mail_address_5 : WritableStringTargetVariable, IGSMvariable
    {
        public Mail_address_5()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mail Address  N 5";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_address_5;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_address_5;
            }
        }
    }

    public class Mail_address_sender : WritableStringTargetVariable, IGSMvariable
    {
        public Mail_address_sender()
            : base()
        {

        }

        public override string ToString()
        {
            return "Mail Address  sender";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_address_sender;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_address_sender;
            }
        }
    }

    //SMTP 
    public class SMTP_server : WritableStringTargetVariable, IGSMvariable
    {
        public SMTP_server()
            : base()
        {

        }

        public override string ToString()
        {
            return "SMTP Server name";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Smtp_server;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Smtp_server;
            }
        }
    }
    public class SMTP_Port : WritableUInt16TargetVariable, IGSMvariable
    {
        public SMTP_Port()
            : base()
        {

        }

        public override string ToString()
        {
            return "SMTP Server Port";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.SMTP_Port;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.SMTP_Port;
            }
        }
    } 
    public class SMTP_username : WritableStringTargetVariable, IGSMvariable
    {
        public SMTP_username()
            : base()
        {

        }

        public override string ToString()
        {
            return "SMTP Server  username";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Smtp_username;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Smtp_username;
            }
        }
    }
    public class Smtp_password : WritableStringTargetVariable, IGSMvariable
    {
        public Smtp_password()
            : base()
        {

        }

        public override string ToString()
        {
            return "SMTP Server password";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Smtp_password;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Smtp_password;
            }
        }
    }
   
    //APN
    public class APN : WritableStringTargetVariable, IGSMvariable
    {
        public APN()
            : base()
        {

        }

        public override string ToString()
        {
            return "APN Access point";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.APN;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.APN;
            }
        }
    }
    public class APN_username : WritableStringTargetVariable, IGSMvariable
    {
        public APN_username()
            : base()
        {

        }

        public override string ToString()
        {
            return "APN user name";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.APN_username;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.APN_username;
            }
        }
    }
    public class APN_password : WritableStringTargetVariable, IGSMvariable
    {
        public APN_password()
            : base()
        {

        }

        public override string ToString()
        {
            return "APN password";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.APN_password;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.APN_password;
            }
        }
    }

    public class SIM_PIN : WritableStringTargetVariable, IGSMvariable
    {
        public SIM_PIN()
            : base()
        {

        }

        public override string ToString()
        {
            return "SIM PIN";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.SIM_PIN;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.SIM_PIN;
            }
        }
    }
    public class En_data_roaming: WritableByteTargetVariable, IGSMvariable
    {
        public En_data_roaming()
            : base()
        {

        }

        public override string ToString()
        {
            return "Enable data roaming";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.En_data_roaming;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.En_data_roaming;
            }
        }
    }


    //AUTO-Report 
    //sms
    public class Hours_sms : WritableUInt32TargetVariable, IGSMvariable
    {
        public Hours_sms()
            : base()
        {

        }

        public override string ToString()
        {
            return "SMS Hours Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Hours_sms;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Hours_sms;
            }
        }
    }
    public class Days_of_Week_sms : WritableByteTargetVariable, IGSMvariable
    {
        public Days_of_Week_sms()
            : base()
        {

        }

        public override string ToString()
        {
            return "SMS Days of Week Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Days_of_Week_sms;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Days_of_Week_sms;
            }
        }
    }
    public class Days_of_Month_sms : WritableUInt32TargetVariable, IGSMvariable
    {
        public Days_of_Month_sms()
            : base()
        {

        }

        public override string ToString()
        {
            return "SMS Days of Month Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Days_of_Month_sms;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Days_of_Month_sms;
            }
        }
    }
    //email
    public class Hours_email : WritableUInt32TargetVariable, IGSMvariable
    {
        public Hours_email()
            : base()
        {

        }

        public override string ToString()
        {
            return "Email Hours Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Hours_email;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Hours_email;
            }
        }
    }
    public class Days_of_Week_email : WritableByteTargetVariable, IGSMvariable
    {
        public Days_of_Week_email()
            : base()
        {

        }

        public override string ToString()
        {
            return "Email Days of Week Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Days_of_Week_email;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Days_of_Week_email;
            }
        }
    }
    public class Days_of_Month_email : WritableUInt32TargetVariable, IGSMvariable
    {
        public Days_of_Month_email()
            : base()
        {

        }

        public override string ToString()
        {
            return "Email Days of Month Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Days_of_Month_email;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Days_of_Month_email;
            }
        }
    }
    //email_Attchment 
    public class Hours_email_att : WritableUInt32TargetVariable, IGSMvariable
    {
        public Hours_email_att()
            : base()
        {

        }

        public override string ToString()
        {
            return "Email Att. Hours Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Hours_email_att;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Hours_email_att;
            }
        }
    }
    public class Days_of_Week_email_att : WritableByteTargetVariable, IGSMvariable
    {
        public Days_of_Week_email_att()
            : base()
        {

        }

        public override string ToString()
        {
            return "Email Att. Days of Week Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Days_of_Week_email_att;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Days_of_Week_email_att;
            }
        }
    }
    public class Days_of_Month_email_att : WritableUInt32TargetVariable, IGSMvariable
    {
        public Days_of_Month_email_att()
            : base()
        {

        }

        public override string ToString()
        {
            return "Email Att. Days of Month Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Days_of_Month_email_att;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Days_of_Month_email_att;
            }
        }
    }

    //Data Euromag Server 
    public class Hours_Data : WritableUInt32TargetVariable, IGSMvariable
    {
        public Hours_Data()
            : base()
        {

        }

        public override string ToString()
        {
            return "Data Hours Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Hours_Data;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Hours_Data;
            }
        }
    }
    public class Days_of_Week_Data : WritableByteTargetVariable, IGSMvariable
    {
        public Days_of_Week_Data()
            : base()
        {

        }

        public override string ToString()
        {
            return "Data Days of Week Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Days_of_Week_Data;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Days_of_Week_Data;
            }
        }
    }
    public class Days_of_Month_Data : WritableUInt32TargetVariable, IGSMvariable
    {
        public Days_of_Month_Data()
            : base()
        {

        }

        public override string ToString()
        {
            return "Data Days of Month Selection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Days_of_Month_Data;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Days_of_Month_Data;
            }
        }
    }
      


    //FTP server 

    public class FTP_server : WritableStringTargetVariable, IGSMvariable
    {
        public FTP_server()
            : base()
        {

        }

        public override string ToString()
        {
            return "FTP Server name";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.FTP_server_name;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.FTP_server_name;
            }
        }
    }
    public class FTP_username : WritableStringTargetVariable, IGSMvariable
    {
        public FTP_username()
            : base()
        {

        }

        public override string ToString()
        {
            return "FTP Server user name";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.FTP_username;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.FTP_username;
            }
        }
    }
    public class FTP_password : WritableStringTargetVariable, IGSMvariable
    {
        public FTP_password()
            : base()
        {

        }

        public override string ToString()
        {
            return "FTP Server password";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.FTP_password;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.FTP_password;
            }
        }
    }
    public class FTP_workdir_1 : WritableStringTargetVariable, IGSMvariable
    {
        public FTP_workdir_1()
            : base()
        {

        }

        public override string ToString()
        {
            return "FTP work directory 1";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.FTP_workdir_1;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.FTP_workdir_1;
            }
        }
    }
    public class FTP_workdir_2 : WritableStringTargetVariable, IGSMvariable
    {
        public FTP_workdir_2()
            : base()
        {

        }

        public override string ToString()
        {
            return "FTP work directory 2";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.FTP_workdir_2;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.FTP_workdir_2;
            }
        }
    }
    public class FTP_workdir_3 : WritableStringTargetVariable, IGSMvariable
    {
        public FTP_workdir_3()
            : base()
        {

        }

        public override string ToString()
        {
            return "FTP work directory 3";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.FTP_workdir_3;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.FTP_workdir_3;
            }
        }
    }

    public class Email_send_by_ftp : WritableByteTargetVariable, IGSMvariable
    {
        public Email_send_by_ftp()
            : base()
        {

        }

        public override string ToString()
        {
            return "Email Send By ftp server";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Email_send_by_ftp;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Email_send_by_ftp;
            }
        }
    }

    public class Sensor_ID : WritableStringTargetVariable, IGSMvariable
    {
        public Sensor_ID()
            : base()
        {

        }

        public override string ToString()
        {
            return "Sensor_ID";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Sensor_ID;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Sensor_ID;
            }
        }
    }
    public class Converter_ID : WritableStringTargetVariable, IGSMvariable
    {
        public Converter_ID()
            : base()
        {

        }

        public override string ToString()
        {
            return "Converter_ID";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Conveter_ID;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Conveter_ID;
            }
        }
    }

    public class Enable_remote_update : WritableByteTargetVariable, IGSMvariable
    {
        public Enable_remote_update()
            : base()
        {

        }

        public override string ToString()
        {
            return "Enable Firmware Update Check";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Enable_remote_update;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Enable_remote_update;
            }
        }
    }

    #endregion


    #region GSM_TEST_Variables

    public class Operator_Connecteted : WritableInt32TargetVariable, IGSMvariable
    {
        public Operator_Connecteted()
            : base()
        {

        }

        public override string ToString()
        {
            return "Connesso all Operatore";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Operator_Connecteted;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Operator_Connecteted;
            }
        }
    }
    public class Signal_strength : WritableByteTargetVariable, IGSMvariable
    {
        public Signal_strength()
            : base()
        {

        }

        public override string ToString()
        {
            return "Forza Segnale GSM";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Signal_strength;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Signal_strength;
            }
        }
    }
    public class Operator_name : WritableStringTargetVariable, IGSMvariable
    {
        public Operator_name()
            : base()
        {

        }

        public override string ToString()
        {
            return "Nome Operatore";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Operator_name;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Operator_name;
            }
        }

    }
    public class Mail_Body : WritableStringTargetVariable, IGSMvariable
    {
        public Mail_Body()
            : base()
        {

        }

        public override string ToString()
        {
            return "Corpo della Mail";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_Body;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_Body;
            }
        }

    }
    public class Mail_Attachment : WritableDataTargetVariable, IGSMvariable
    {
        public Mail_Attachment()
            : base()
        {

        }

        public override string ToString()
        {
            return "Allegato Mail";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_Attachment;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_Attachment;
            }
        }

    }
    public class Mail_Send : WritableByteTargetVariable, IGSMvariable
    {
        public Mail_Send()
            : base()
        {

        }

        public override string ToString()
        {
            return "Invia Mail al Contato n?";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_Send;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_Send;
            }
        }
    }
    public class Mail_Send_Attach : WritableByteTargetVariable, IGSMvariable
    {
        public Mail_Send_Attach()
            : base()
        {

        }

        public override string ToString()
        {
            return "Invia Mail_Attach al Contato n?";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_Send_Attach;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_Send_Attach;
            }
        }
    }


    public class SMS_Body : WritableStringTargetVariable, IGSMvariable
    {
        public SMS_Body()
            : base()
        {

        }

        public override string ToString()
        {
            return "Messagio dell SMS";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.SMS_Body;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.SMS_Body;
            }
        }

    }
    public class SMS_Send : WritableByteTargetVariable, IGSMvariable
    {
        public SMS_Send()
            : base()
        {

        }

        public override string ToString()
        {
            return "Invia SMS al Contato n?";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.SMS_Send;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.SMS_Send;
            }
        }
    }
    public class SMS_Sending_Status : WritableByteTargetVariable, IGSMvariable
    {
        public SMS_Sending_Status()
            : base()
        {

        }

        public override string ToString()
        {
            return "Stato Invio SMS";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.SMS_Sending_Status;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.SMS_Sending_Status;
            }
        }
    }
    public class Mail_Sending_Status : WritableByteTargetVariable, IGSMvariable
    {
        public Mail_Sending_Status()
            : base()
        {

        }

        public override string ToString()
        {
            return "Stato Invio Mail";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Mail_Sending_Status;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Mail_Sending_Status;
            }
        }
    }
    public class FTP_Test_Connection : WritableByteTargetVariable, IGSMvariable
    {
        public FTP_Test_Connection()
            : base()
        {

        }

        public override string ToString()
        {
            return "FTP Test  Connection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.FTP_Test_Connection;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.FTP_Test_Connection;
            }
        }
    }
    public class Data_Test_Connection : WritableByteTargetVariable, IGSMvariable
    {
        public Data_Test_Connection()
            : base()
        {

        }

        public override string ToString()
        {
            return "Data Test  Connection";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Data_Test_Connection;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Data_Test_Connection;
            }
        }
    }
    public class FTP_Test_Status : WritableByteTargetVariable, IGSMvariable
    {
        public FTP_Test_Status()
            : base()
        {

        }

        public override string ToString()
        {
            return "FTP Test  Connection Status";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.FTP_Test_Status;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.FTP_Test_Status;
            }
        }
    }
    public class Data_Test_Status : WritableByteTargetVariable, IGSMvariable
    {
        public Data_Test_Status()
            : base()
        {

        }

        public override string ToString()
        {
            return "Data Test  Connection Status";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Data_Test_Status;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Data_Test_Status;
            }
        }
    }
    public class Sync_Date_Time : WritableStringTargetVariable, IGSMvariable
    {
        public Sync_Date_Time()
            : base()
        {

        }

        public override string ToString()
        {
            return "Sincronisa Data Ora";
        }

        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Sync_Date_Time;
            }
        }

        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Sync_Date_Time;
            }
        }

    }
    public class Get_Date_Time : WritableStringTargetVariable, IGSMvariable
    {
        public Get_Date_Time()
            : base()
        {

        }

        public override string ToString()
        {
            return "Get_Date_Time Modem GSM";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Get_Date_Time;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Get_Date_Time;
            }
        }
    }

    public class Modem_Reset : WritableUInt16TargetVariable, IGSMvariable
    {
        public Modem_Reset()
            : base()
        {

        }

        public override string ToString()
        {
            return "Modem_Reset key = 18373(0x47C5)";
        }

        public new virtual UInt16 Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                _value = value;
            }
        }
            
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Modem_Reset;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Modem_Reset;
            }
        }
    }
    public class Modem_shutdown : WritableUInt16TargetVariable, IGSMvariable
    {
            public Modem_shutdown()
            : base()
        {

        }

        public override string ToString()
        {
            return "Modem_shutdown key = 22483 (0x57D3)";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.Modem_shutdown;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.Modem_shutdown;
            }
        }
    }
    public class GSM_Status : WritableDataTargetVariable, IGSMvariable
    {
        public GSM_Status()
            : base()
        {

        }

        public override string ToString()
        {
            return "Stato GSM";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.GSM_Status;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.GSM_Status;
            }
        }
    }
    public class RESET_EEprom : WritableUInt16TargetVariable, IGSMvariable
    {
        public RESET_EEprom()
            : base()
        {

        }

        public override string ToString()
        {
            return "RESET EEPROM  key = 26993 (0x6971)";
        }
        public UInt32 ID
        {
            get
            {
                return (uint)GSM_UID.RESET_EEPROM;
            }
        }
        public GSMAddresses Address
        {
            get
            {
                return GSMAddresses.RESET_EEPROM;
            }
        }
    }


   #endregion 



}
