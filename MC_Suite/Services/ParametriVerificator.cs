using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Suite.Services
{
    class ParametriVerificator
    {
        public TestSens TestSensore = new TestSens();
        public I4_20mA Para4_20mA = new I4_20mA();
        public ParaICoil ICoil = new ParaICoil();
        public Simulazione Simul = new Simulazione();

        public ParametriVerificator()
        {
            TestSensore = new TestSens();
            Para4_20mA  = new I4_20mA();
            ICoil       = new ParaICoil();
            Simul       = new Simulazione();
        }

        public class Simulazione
        {
            public ushort DAC_Zero = 0;
            public float zero_Res_min = -0.1f;
            public float zero_Res_max = 0.1f;

            public ushort DAC_Low_608 = 5805;   //  5805 = 0,4430mV toll 0,0015
            public ushort DAC_Low_406 = 1370;       
            public float Low_Res_min = 2.10f;   //  2.18f;    //2.48f;
            public float Low_Res_max = 2.80f;   //  2.72f;    //2.52f;

            public ushort DAC_Hi_608 = 11578;   //  11578 = 0,8790mV toll 0,0015
            public ushort DAC_Hi_406 = 2760;        
            public float HI_Res_min = 4.20f;    //  4.65f; //4.95f;
            public float HI_Res_max = 5.40f;    //  5.35f; //5.05f;

            // Tolleranze rispetto alla misura trovata nel DB
            public float Tolleranza_MaxPerc = 2.0f;
            public float Tolleranza_MinPerc = 2.0f;
        }

        public class TestSens
        {
            public float RL_ABmin = 50;
            public float RL_ABmax = 250;
            public float RL_Min = 19999;        // limmite lelttura con test in bassatensione
            public float RH_Min = 100000000;    // isolamento minimo 100Mohm
            public float MaxDiffPerc = 5.0f;    // Tolleranze rispetto alla misura trovata nel DB
        }

        public class I4_20mA
        {
            public double Offset_min = -0.5d;
            public double Offset_max = 0.5d;
            public string Offset_limit = ">=-0.5/<=0.5";

            public double I4mA_min = 3.80d;
            public double I4mA_max = 4.20d;
            public string I4mA_limit = ">=3.8/<=4.2";

            public double I12mA_min = 11.80d;
            public double I12mA_max = 12.20d;
            public string I12mA_limit = ">=11.8/<=12.2";

            public double I20mA_min = 19.80d;
            public double I20mA_max = 20.20d;
            public string I20mA_limit = ">=19.8/<=20.2";

            // Valutare se tolleranze diverse per self powered
        }

        public class ParaICoil
        {
            // Base corrente 208mA con ref 1,25 e shunt 6ohm
            public float Offset_min = -2.0f;
            public float Offset_max = 2.0f;

            public float I_125mA_min = 95.0f;   // 208 / 2 = 104mA
            public float I_125mA_max = 130.0f;
            public float I_65mA_min = 35.0f;    // 208 / 4 = 52mA
            public float I_65mA_max = 70.0f;
            public float I_50mA_min = 25.0f;    // 208 / 5 = 41,6mA
            public float I_50mA_max = 55.0f;
            public float I_31mA_min = 10.0f;    // 208 / 8 = 26mA
            public float I_31mA_max = 35.0f;
            public float I_25mA_min = 5.0f;    // 208 / 10 = 20,8
            public float I_25mA_max = 30.0f;

            // Tolleranze rispetto alla misura trovata nel DB
            public float Tolleranza_MaxPerc = 1.0f;
            public float Tolleranza_MinPerc = 1.0f;
        }
    }
}
