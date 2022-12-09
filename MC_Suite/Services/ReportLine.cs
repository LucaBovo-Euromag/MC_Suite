using System;

namespace MC_Suite.Services
{
    public class ReportLine
    {
        public int ID { get; set; }
        public string Data_Test { get; set; }
        public string OperatoreTest { get; set; }
        public string Modello_Sensore { get; set; }
        public string Matricola_Sensore { get; set; }
        public string Modello_Convertitore { get; set; }
        public string Matricola_Convertitore { get; set; }
        public string KA { get; set; }
        public string FondoScala { get; set; }
        public string Impulsi { get; set; }

        #region Convertitore
        public string AnalogOut { get; set; }

        #region Simulazione
            public string Simulation { get; set; }
            public double Zero_read { get; set; }
            public double Hi_read { get; set; }
            public double LO_read { get; set; }
            public string EmptyPype { get; set; }
        #endregion

        #region Energy Coil
            public string EnergyCoil { get; set; }
            public double ICoil_Read { get; set; }
        #endregion

        #region IO
        public string IO { get; set; }
        #endregion

        public string TempPCB { get; set; }
        #endregion

        //Sensore***********************************************
        public string CoilResistance { get; set; }
        public string IsolationAC { get; set; }
        public string IsolationTC { get; set; }
        public string IsolationDC { get; set; }
        public string IsolationEC { get; set; }
        public string TestType { get; set; }
        //******************************************************
        public string Company { get; set; }
        public string CompanyLocation { get; set; }
        public string Customer { get; set; }
        public string CustomerLocation { get; set; }
        public string Note { get; set; }
        public string SW_Ver_Verificator { get; set; }
        public string SN_Verificator { get; set; }
        public string DataCalibrazione { get; set; }
        public string NuovaCalibrazione { get; set; }
    }
}
