using MC_Suite.Modbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Suite.Services
{
    public class CustomDictionary
    {
        private static CustomDictionary _instance;
        public static CustomDictionary Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomDictionary();
                return _instance;
            }
        }

        private Dictionary<string, string> SensorModelsDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> ConverterModelsDictionary = new Dictionary<string, string>();
        public void InitDictionaries()
        {
            //Chemitec
            ConverterModelsDictionary.Add("MC608",  "CH608");
            ConverterModelsDictionary.Add("MC608A", "CH608A");
            ConverterModelsDictionary.Add("MC608B", "CH608B");
            ConverterModelsDictionary.Add("MC608I", "CH608I");
            ConverterModelsDictionary.Add("MC406",  "CH406");
            ConverterModelsDictionary.Add("MC406A", "CH406A");

            //Chemitec
            SensorModelsDictionary.Add("MUT500",    "CH500");
            SensorModelsDictionary.Add("MUT2200EL", "CH2200EL");
            SensorModelsDictionary.Add("MUT2400EL", "CH2400EL");
            SensorModelsDictionary.Add("MUT1000EL", "CH1000EL");
            SensorModelsDictionary.Add("MUT1100J",  "CH1100J");
            SensorModelsDictionary.Add("MUT4000",   "CH4000");
            SensorModelsDictionary.Add("MUT2300",   "CH2300");
            SensorModelsDictionary.Add("MUT1222",   "CH1222");
            SensorModelsDictionary.Add("MUT2660",   "CH2660");
            SensorModelsDictionary.Add("MUT2700",   "CH2700");            
            SensorModelsDictionary.Add("MUT770",    "CH2770");
        }

        public string ConverterModel(string _model)
        {
            string CustomModel = RemoveSpaces(_model);

            try
            {
                CustomModel = ConverterModelsDictionary[CustomModel];
            }
            catch
            {
                return _model;
            }

            return CustomModel;
        }

        public string SensorModel(string _model)
        {
            string CustomModel = RemoveSpaces(_model);

            try
            {
                CustomModel = SensorModelsDictionary[CustomModel];
            }
            catch
            {
                return _model;
            }

            return CustomModel;
        }

        private string RemoveSpaces(string _input)
        {
            Char[] StringArray = _input.ToCharArray();

            Char[] StringArrayNoSpaces = new Char[_input.Length];

            int i = 0;
            foreach (char c in StringArray)
            {
                if (c.Equals(' ') == false)
                {
                    StringArrayNoSpaces[i] = c;
                    i++;
                }
            }
            string returnstring = new String(StringArrayNoSpaces);

            int SpaceIndex = returnstring.IndexOf('\0');
            if (SpaceIndex > -1)
                return returnstring.Substring(0, SpaceIndex);
            else
                return returnstring;
        }
    }
}
