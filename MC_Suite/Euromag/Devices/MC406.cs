using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MC_Suite.Euromag.Devices
{
    [DefaultPropertyAttribute("MC406 Configurazione")]
    public class MC406
    {

        //[Browsable(bool)]                         // this property should be visible
        //[ReadOnly(true)]                          // but just read only
        //[Description("sample hint1")]             // sample hint1
        //[Category("Category1")]                   // Category that I want
        //[DisplayName("Int for Displaying")]       // I want to say more, than just DisplayInt

        #region Enum
        public enum FreqRete
        {
            Rete_50Hz = 0,
            Rete_60Hz = 1
        }

        public enum Emptypype
        {
            Assente = 0,
            Presente = 1
        }

        public enum TipoSensore
        {
            Standard = 0,
            Inserzione = 1
        }

        public enum Collegamento
        {
            Compatto = 0,
            Separato = 1
        }

        public enum MatRivestimento
        {
            nd,
            Teflon,
            Rilsan,
            Ebanite
        }

        public enum MatSensore
        {
            nd,
            Lega_HC,
            Inox_316
        }

        public enum MatElettrodi
        {
            nd,
            Titanio,
            Platino,
            Astelloid,
            altro
        }

        public enum UT
        {
            mL = 1,
            L = 2,
            m3 = 3,
            gal = 4
        }

        public enum UT_Portata
        {
            ft,
            m
        }

        public enum TB_Portata
        {
            d,
            h,
            m,
            s
        }
        #endregion

        public void Init()
        {
        }

        const string CategoriaInfo = "CONVERTER";

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(false)]
        public string Manufacturer { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public int FW_Ver { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public int FW_Rev { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public int HW_Version { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public int BoardID { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public int Seriale { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public DateTime DataCalibr { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public string Modello_Covert { get; set; }

        [CategoryAttribute(CategoriaInfo), DisplayName("Matricola Convertitore"), ReadOnly(true), DescriptionAttribute("")]
        public string Matricola_Conv { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public float KA_Align { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public float KA_Main { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute(""), ReadOnly(true)]
        public float Offset { get; set; }

        [CategoryAttribute(CategoriaInfo), DescriptionAttribute("Temperatura PCB MC406"), ReadOnly(true)]
        public float TempPCB { get; set; }


        // ************************************************************************************************************************************

        const string CategoriaSensore = "SENSOR";

        [CategoryAttribute(CategoriaSensore), ReadOnly(true), DescriptionAttribute("")]
        public string Modello_Sensore { get; set; }

        [CategoryAttribute(CategoriaSensore), ReadOnly(true), DescriptionAttribute("")]
        public string Matricola_Sensore { get; set; }

        const string CategoriaAmbientale = "6-AMBIENTALE";

    }
}
