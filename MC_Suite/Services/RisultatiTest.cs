using Windows.UI;
using Windows.UI.Xaml.Media;


namespace MC_Suite.Services
{
    public class RisultatiTest
    {
        public static TestIO IO = new TestIO();
        public static TestSensore Sensore = new TestSensore();
        public static Test4_20mA AnOut = new Test4_20mA();
        public static TestSimulazione Simulazione = new TestSimulazione();
        public static TestICoil ICoil = new TestICoil();

        public enum Esito
        {
            DaEseguire,
            PASS_Verify,
            PASS_Def,
            FAIL
        }

        public class TestStep
        {
            public int ID { get; set; }
            public string Device { get; set; }
            public string Description { get; set; }
            public string Reference { get; set; }
            public string Reading { get; set; }
            public string Result { get; set; }
        }

        #region Test I/O

        public struct TestIO
        {
            //MC608******************************
            public Esito GP_OUT_on;
            public Esito GP_OUT_off;
            public Esito GP_FREQ_on;
            public Esito GP_FREQ_Off;
            public Esito GP_PULSE_on;
            public Esito GP_PULSE_off;
            public Esito GP_IN_on;
            public Esito GP_IN_off;
            public TestStep Test_GP_OUT_on;
            public TestStep Test_GP_OUT_off;
            public TestStep Test_GP_FREQ_on;
            public TestStep Test_GP_FREQ_Off;
            public TestStep Test_PULSE_on;
            public TestStep Test_PULSE_off;
            public TestStep Test_GP_IN_on;
            public TestStep Test_GP_IN_off;
            //MC406******************************
            public Esito PulsePosON;
            public Esito PulsePosOFF;
            public Esito PulseNegON;
            public Esito PulseNegOFF;
            public TestStep Test_PulsePosON;
            public TestStep Test_PulsePosOFF;
            public TestStep Test_PulseNegON;
            public TestStep Test_PulseNegOFF;
            //***********************************
            public Esito Esito;
            public string EsitoTxt;
            public Brush EsitoColor;
            public int TestID;
        }

        public static void Init_IO()
        {
            //MC608**************************************************
            IO.GP_IN_off    = Esito.DaEseguire;
            IO.GP_IN_on     = Esito.DaEseguire;
            IO.GP_OUT_off   = Esito.DaEseguire;
            IO.GP_OUT_on    = Esito.DaEseguire;
            IO.GP_PULSE_off = Esito.DaEseguire;
            IO.GP_PULSE_on  = Esito.DaEseguire;
            IO.GP_FREQ_Off  = Esito.DaEseguire;
            IO.GP_FREQ_on   = Esito.DaEseguire;

            IO.Test_GP_IN_off = new TestStep();
            IO.Test_GP_IN_off.Description = "Test Prog.In Off";
            IO.Test_GP_IN_on = new TestStep();
            IO.Test_GP_IN_on.Description = "Test Prog.In On";
            IO.Test_GP_OUT_off = new TestStep();
            IO.Test_GP_OUT_off.Description = "Test Prog.Out Off";
            IO.Test_GP_OUT_on = new TestStep();
            IO.Test_GP_OUT_on.Description = "Test Prog.Out On";
            IO.Test_PULSE_off = new TestStep();
            IO.Test_PULSE_off.Description = "Test Pulse Off";
            IO.Test_PULSE_on = new TestStep();
            IO.Test_PULSE_on.Description = "Test Pulse On";
            IO.Test_GP_FREQ_Off = new TestStep();
            IO.Test_GP_FREQ_Off.Description = "Test Freq Off";
            IO.Test_GP_FREQ_on = new TestStep();
            IO.Test_GP_FREQ_on.Description = "Test Freq On";

            //MC406**************************************************
            IO.PulsePosON   = Esito.DaEseguire;
            IO.PulsePosOFF  = Esito.DaEseguire;
            IO.PulseNegON   = Esito.DaEseguire;
            IO.PulseNegOFF  = Esito.DaEseguire;

            IO.Test_PulseNegOFF = new TestStep();
            IO.Test_PulseNegOFF.Description = "Test Pulse Neg OFF";
            IO.Test_PulseNegON  = new TestStep();
            IO.Test_PulseNegON.Description = "Test Pulse Neg ON";
            IO.Test_PulsePosOFF = new TestStep();
            IO.Test_PulsePosOFF.Description = "Test Pulse Pos OFF";
            IO.Test_PulsePosON  = new TestStep();
            IO.Test_PulsePosON.Description = "Test Pulse Pos ON";

            //*********************************************************

            IO.TestID           = 1;
            IO.Esito = Esito.DaEseguire;
            IO.EsitoTxt = "--";

        }

        public static bool Res_IO()
        {
            bool res = true;

            if (IO.PulseNegOFF == Esito.FAIL)
                res = false;
            if (IO.PulseNegON == Esito.FAIL)
                res = false;
            if (IO.PulsePosOFF != Esito.PASS_Verify)
                res = false;
            if (IO.PulsePosON != Esito.PASS_Verify)
                res = false;

            if (res)
            {
                IO.Esito = Esito.PASS_Verify;
                IO.EsitoTxt = "PASS";
                IO.EsitoColor = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                IO.Esito = Esito.FAIL;
                IO.EsitoTxt = "FAIL";
                IO.EsitoColor = new SolidColorBrush(Colors.Red);
            }
            return res;
        }

        public static bool Res_IO_608()
        {
            bool res = true;

            if (IO.GP_OUT_off == Esito.FAIL)
                res = false;
            if (IO.GP_OUT_on == Esito.FAIL)
                res = false;
            if (IO.GP_IN_off == Esito.FAIL)
                res = false;
            if (IO.GP_IN_on == Esito.FAIL)
                res = false;
            if (IO.GP_PULSE_off == Esito.FAIL)
                res = false;
            if (IO.GP_PULSE_on == Esito.FAIL)
                res = false;
            if (IO.GP_FREQ_Off == Esito.FAIL)
                res = false;
            if (IO.GP_FREQ_on == Esito.FAIL)
                res = false;

            if (res)
            {
                IO.Esito = Esito.PASS_Verify;
                IO.EsitoTxt = "PASS";
                IO.EsitoColor = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                IO.Esito = Esito.FAIL;
                IO.EsitoTxt = "FAIL";
                IO.EsitoColor = new SolidColorBrush(Colors.Red);
            }
            return res;
        }

        #endregion

        #region Test 4_20mA
        public struct Test4_20mA
        {
            public Esito I_OffSet;
            public float I_OffSet_read;
            public Esito I_4mA;
            public float I_4mA_read;
            public Esito I_12mA;
            public float I_12mA_read;
            public Esito I_20mA;
            public float I_20mA_read;
            public Esito Esito;
            public string EsitoTxt;
            public Brush EsitoColor;

            public int TestID;
            public TestStep Test_I_OffSet;
            public TestStep Test_I_4mA;
            public TestStep Test_I_12mA;
            public TestStep Test_I_20mA;
        }

        public static void Init_AnalogOut()
        {
            AnOut.I_OffSet = Esito.DaEseguire;
            AnOut.I_4mA = Esito.DaEseguire;
            AnOut.I_12mA = Esito.DaEseguire;
            AnOut.I_20mA = Esito.DaEseguire;

            AnOut.I_4mA_read  = 0;
            AnOut.I_12mA_read = 0;
            AnOut.I_20mA_read = 0;

            AnOut.Esito = Esito.DaEseguire;
            AnOut.EsitoTxt = "---";

            AnOut.Test_I_OffSet = new TestStep();
            AnOut.Test_I_OffSet.Description = "Offset Verification";

            AnOut.Test_I_4mA    = new TestStep();
            AnOut.Test_I_4mA.Description = "Verification 4mA";

            AnOut.Test_I_12mA   = new TestStep();
            AnOut.Test_I_12mA.Description = "Verification 12mA";

            AnOut.Test_I_20mA   = new TestStep();
            AnOut.Test_I_20mA.Description = "Verification 20mA";

            AnOut.TestID = 1;            
        }

        public static bool Res_AnalogOut()
        {

            if (AnOut.I_OffSet == Esito.PASS_Verify && AnOut.I_4mA == Esito.PASS_Verify && AnOut.I_12mA == Esito.PASS_Verify && AnOut.I_20mA == Esito.PASS_Verify)
            {
                AnOut.Esito = Esito.PASS_Verify;
                AnOut.EsitoTxt = "PASS";
                AnOut.EsitoColor = new SolidColorBrush(Colors.LimeGreen);
                return true;
            }
            else
            {
                AnOut.Esito = Esito.FAIL;
                AnOut.EsitoTxt = "FAIL";
                AnOut.EsitoColor = new SolidColorBrush(Colors.Red);
                return false;
            }
        }
        #endregion

        #region Test Sensore
        public struct TestSensore
        {
            public bool dry;
            public float R_Coil;
            public float H_Coil;
            public Esito RL_AB;
            public Esito RL_DC;
            public Esito RL_TC;
            public Esito RL_EC;
            public Esito RH_AC;
            public float R_AC;
            public Esito RH_DC;
            public float R_DC;
            public Esito RH_TC;
            public float R_TC;
            public Esito RH_EC;
            public float R_EC;
            public Esito Esito;
            public string EsitoTxt;
            public Brush EsitoColor;
            public int TestID;
            public TestStep Test_R_Coil;
            public TestStep Test_RL_AB;
            public TestStep Test_RL_DC;
            public TestStep Test_RL_TC;
            public TestStep Test_RL_EC;
            public TestStep Test_RH_AC;
            public TestStep Test_RH_DC;
            public TestStep Test_RH_TC;
            public TestStep Test_RH_EC;
        }

        public static bool Res_Sensor_Wet()
        {
            bool res = true;

            if (Sensore.RL_AB == Esito.FAIL)
                res = false;

            if (Sensore.RH_AC == Esito.FAIL) // Attendo modifica FW enrico
                res = false;

            if (res)
            {
                Sensore.Esito = Esito.PASS_Verify;
                Sensore.EsitoTxt = "PASS";
            }
            else
            {
                Sensore.Esito = Esito.FAIL;
                Sensore.EsitoTxt = "FAIL";
            }

            return res;
        }

        public static bool Res_Sensor_Dry()
        {
            bool res = true;

            if (Sensore.RL_AB == Esito.FAIL)
                res = false;
            if (Sensore.RL_DC == Esito.FAIL)
                res = false;
            if (Sensore.RL_EC == Esito.FAIL)
                res = false;
            if (Sensore.RL_TC == Esito.FAIL)
                res = false;

            if (Sensore.RH_AC == Esito.FAIL)
                res = false;
            if (Sensore.RH_DC == Esito.FAIL)
                res = false;
            if (Sensore.RH_EC == Esito.FAIL)
                res = false;
            if (Sensore.RH_TC == Esito.FAIL)
                res = false;

            if (res)
            {
                if (Sensore.RL_AB == Esito.PASS_Verify)
                {
                    Sensore.Esito = Esito.PASS_Verify;
                    Sensore.EsitoTxt = "PASS";
                    Sensore.EsitoColor = new SolidColorBrush(Colors.LimeGreen);
                }
                if (Sensore.RL_AB == Esito.PASS_Def)
                {
                    Sensore.Esito = Esito.PASS_Def;
                    Sensore.EsitoTxt = "PASS (Default)";
                    Sensore.EsitoColor = new SolidColorBrush(Colors.YellowGreen);
                }
            }
            else
            {
                Sensore.Esito = Esito.FAIL;
                Sensore.EsitoTxt = "FAIL";
                Sensore.EsitoColor = new SolidColorBrush(Colors.Red);
            }

            return res;
        }


        public static void Init_Sensore()
        {
            Sensore.dry = true;
            Sensore.R_Coil = 0f;
            Sensore.H_Coil = 0f;
            Sensore.RL_AB = Esito.DaEseguire;
            Sensore.RL_DC = Esito.DaEseguire;
            Sensore.RL_TC = Esito.DaEseguire;
            Sensore.RL_EC = Esito.DaEseguire;
            Sensore.RH_AC = Esito.DaEseguire;
            Sensore.RH_DC = Esito.DaEseguire;
            Sensore.RH_TC = Esito.DaEseguire;
            Sensore.RH_EC = Esito.DaEseguire;

            Sensore.R_AC = 0;
            Sensore.R_DC = 0;
            Sensore.R_TC = 0;
            Sensore.R_EC = 0;

            Sensore.Esito = Esito.DaEseguire;
            Sensore.EsitoTxt = "---";
            Sensore.EsitoColor = new SolidColorBrush(Colors.Yellow);

            Sensore.Test_R_Coil = new TestStep();
            Sensore.Test_R_Coil.Description = "Coil Resistance";

            Sensore.Test_RL_AB = new TestStep();
            Sensore.Test_RL_AB.Description = "RL AB";

            Sensore.Test_RL_DC = new TestStep();
            Sensore.Test_RL_DC.Description = "RL DC";

            Sensore.Test_RL_TC = new TestStep();
            Sensore.Test_RL_TC.Description = "RL TC";

            Sensore.Test_RL_EC = new TestStep();
            Sensore.Test_RL_EC.Description = "RL EC";

            Sensore.Test_RH_AC = new TestStep();
            Sensore.Test_RH_AC.Description = "RH AC";

            Sensore.Test_RH_DC = new TestStep();
            Sensore.Test_RH_DC.Description = "RH DC";

            Sensore.Test_RH_TC = new TestStep();
            Sensore.Test_RH_TC.Description = "RH TC";

            Sensore.Test_RH_EC = new TestStep();
            Sensore.Test_RH_EC.Description = "RH EC";

            Sensore.TestID = 1;
        }
        #endregion

        #region Test Simulazione
        public struct TestSimulazione
        {
            public Esito Zero;
            public double Zero_read;
            public Esito LO;
            public double LO_read;
            public Esito Hi;
            public double Hi_read;
            public Esito EmptyPype;
            public string Empy_Read;
            public Esito FullPype;
            public string Full_Read;
            public string EsitoTxt;
            public Esito Esito;
            public Brush EsitoColor;
            public Esito EpipeTest;

            public TestStep Test_Zero;
            public TestStep Test_LO;
            public TestStep Test_Hi;
            public TestStep Test_EmptyPype;
            public TestStep Test_FullPype;
            public int TestID;

        }

        public static void Init_Simulazione()
        {
            Simulazione.Zero = Esito.DaEseguire;
            Simulazione.Zero_read = 0;
            Simulazione.LO = Esito.DaEseguire;
            Simulazione.LO_read = 0;
            Simulazione.Hi = Esito.DaEseguire;
            Simulazione.Hi_read = 0;
            Simulazione.EmptyPype = Esito.DaEseguire;
            Simulazione.FullPype = Esito.DaEseguire;

            Simulazione.EsitoTxt = "--";
            Simulazione.Esito = Esito.DaEseguire;

            Simulazione.Test_Zero                   = new TestStep();
            Simulazione.Test_Zero.Description       = "Test Flow 0";
            
            Simulazione.Test_LO                     = new TestStep();
            Simulazione.Test_LO.Description         = "Test Flow Mid";

            Simulazione.Test_Hi                     = new TestStep();
            Simulazione.Test_Hi.Description         = "Test Flow Hi";

            Simulazione.Test_EmptyPype              = new TestStep();
            Simulazione.Test_EmptyPype.Description  = "Test Empty Pipe";

            Simulazione.Test_FullPype               = new TestStep();
            Simulazione.Test_FullPype.Description   = "Test Full Pipe";

            Simulazione.TestID = 1;
    }

        public static bool Res_Simulazione()
        {
            if (Simulazione.Zero == Esito.PASS_Verify && Simulazione.LO == Esito.PASS_Verify && Simulazione.Hi == Esito.PASS_Verify && Simulazione.EmptyPype == Esito.PASS_Verify && Simulazione.FullPype == Esito.PASS_Verify)
            {
                Simulazione.Esito = Esito.PASS_Verify;
                Simulazione.EsitoTxt = "PASS";
                Simulazione.EsitoColor = new SolidColorBrush(Colors.LimeGreen);
                return true;
            }
            if (Simulazione.Zero == Esito.PASS_Verify && (Simulazione.LO == Esito.PASS_Def || Simulazione.Hi == Esito.PASS_Def && Simulazione.EmptyPype == Esito.PASS_Verify && Simulazione.FullPype == Esito.PASS_Verify))
            {
                Simulazione.Esito = Esito.PASS_Verify;
                Simulazione.EsitoTxt = "PASS (Default)";
                Simulazione.EsitoColor = new SolidColorBrush(Colors.YellowGreen);
                return true;
            }
            else
            {
                Simulazione.Esito = Esito.FAIL;
                Simulazione.EsitoTxt = "FAIL";
                Simulazione.EsitoColor = new SolidColorBrush(Colors.Red);
                return false;
            }
        }
        #endregion

        #region Test ICOIL
        public struct TestICoil
        {
            public Esito Zero;
            public Esito PosNeg;
            public float ICoil_Read;
            public float Offset_I;
            public Esito Pos;
            public float Pos_I;
            public Esito Neg;
            public float Neg_I;
            public Esito Esito;
            public string EsitoTxt;
            public Brush EsitoColor;
            public int TestID;
            public TestStep Test_Zero;
            public TestStep Test_PosNeg;
            public TestStep Test_Pos;
            public TestStep Test_Neg;
        }


        public static void Init_ICoil()
        {
            ICoil.Zero = Esito.DaEseguire;
            ICoil.PosNeg = Esito.DaEseguire;
            ICoil.Neg = Esito.DaEseguire;
            ICoil.Pos = Esito.DaEseguire;            
            ICoil.Esito = Esito.DaEseguire;
            ICoil.EsitoTxt = "---";
            ICoil.EsitoColor = new SolidColorBrush(Colors.Yellow);

            ICoil.ICoil_Read = 0;
            ICoil.Neg_I = 0;
            ICoil.Pos_I = 0;

            ICoil.TestID = 1;
            ICoil.Test_Zero = new TestStep();
            ICoil.Test_Zero.Description = "Test ICoil Zero";

            ICoil.Test_PosNeg = new TestStep();
            ICoil.Test_PosNeg.Description = "Test ICoil Pos/Neg";

            ICoil.Test_Pos = new TestStep();
            ICoil.Test_Pos.Description = "Test ICoil Pos";

            ICoil.Test_Neg = new TestStep();
            ICoil.Test_Neg.Description = "Test ICoil Neg";

        }
        public static bool Res_ICoil()
        {
            if (ICoil.Zero == Esito.PASS_Verify && ICoil.PosNeg == Esito.PASS_Verify)
            {
                ICoil.Esito = Esito.PASS_Verify;
                ICoil.EsitoTxt = "PASS";
                ICoil.EsitoColor = new SolidColorBrush(Colors.LimeGreen);
                return true;
            }
            else if (ICoil.Zero == Esito.PASS_Verify && ICoil.PosNeg == Esito.PASS_Def)
            {
                ICoil.Esito = Esito.PASS_Def;
                ICoil.EsitoTxt = "PASS (Default)";
                ICoil.EsitoColor = new SolidColorBrush(Colors.YellowGreen);
                return true;
            }
            else
            {
                ICoil.Esito = Esito.FAIL;
                ICoil.EsitoTxt = "FAIL";
                ICoil.EsitoColor = new SolidColorBrush(Colors.Red);
                return false;
            }
        }

        public static bool Res_ICoil608()
        {
            if (ICoil.Zero == Esito.PASS_Verify && ICoil.Pos == Esito.PASS_Verify && ICoil.Neg == Esito.PASS_Verify)
            {
                ICoil.Esito = Esito.PASS_Verify;
                ICoil.EsitoTxt = "PASS";
                ICoil.EsitoColor = new SolidColorBrush(Colors.LimeGreen);
                return true;
            }
            else if ( ((ICoil.Zero == Esito.PASS_Def) || (ICoil.Zero == Esito.PASS_Verify)) &&
                      ((ICoil.Pos == Esito.PASS_Def) || (ICoil.Pos == Esito.PASS_Verify)) &&
                      ((ICoil.Neg == Esito.PASS_Def) || (ICoil.Neg == Esito.PASS_Verify)) )
            {
                ICoil.Esito = Esito.PASS_Def;
                ICoil.EsitoTxt = "PASS (Default)";
                ICoil.EsitoColor = new SolidColorBrush(Colors.YellowGreen);
                return true;
            }
            else
            {
                ICoil.Esito = Esito.FAIL;
                ICoil.EsitoTxt = "FAIL";
                ICoil.EsitoColor = new SolidColorBrush(Colors.Red);
                return false;
            }
        }
        #endregion
    }
}
