using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Programadores
{

    public partial class CArduinoProgramer : Component, IProgramer
    {
        ArduinoProgramer.CArduinoProgramerConfigurator Configurator;
        public CArduinoProgramer()
        {
            InitializeComponent();
        }

        public CArduinoProgramer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            
        }
        #region expongo las propiedades del puerto serial
        public int BaudRate
        {
            get
            {
                return serialPort1.BaudRate;
            }
            set
            {
                serialPort1.BaudRate = value;                
            }
        }
        public int DataBits
        {
            get
            {
                return serialPort1.DataBits;
            }
            set
            {
                serialPort1.DataBits = value;                
            }
        }
        public string PortName
        {
            get
            {
                return serialPort1.PortName;
            }
            set
            {
                serialPort1.PortName = value;
            }
        }
        public System.IO.Ports.StopBits StopBits
        {
            get
            {
                return serialPort1.StopBits;
            }
            set
            {
                serialPort1.StopBits = value;
            }
        }
        public void Open()
        {
            serialPort1.Open();
        }
        public void Close()
        {
            serialPort1.Close();
        }
        #endregion
        #region Api del puerto serial
        private byte getData()
        {
            int status = 0;
            string s = "";
            string s2 = "";
            byte b;
            do
            {
                System.Threading.Thread.Sleep(1);
                //s2 = "";
                s = serialPort1.ReadExisting();
                //quito la basura
                for (int i = 0; i < s.Length; i++)
                {
                    switch (status)
                    {
                        case 0: //espero el caracter <
                            if (s[i] == '<')
                                status = 1;
                            break;
                        case 1: //recibiendo los datos hasta encontrar el caracter >
                            if (s[i] == '>')
                                status = 3;
                            else
                                s2 = s2 + s[i];
                            break;
                    }
                }
                //s = s2;
            }
            while (status<3);
            b = (byte)Convert.ToInt32(s2, 16);
            return b;

        }
        /// <summary>
        /// borra el chip
        /// </summary>
        public void ErraceChip()
        {
            byte b = 0;
            do
            {
                serialPort1.WriteLine(":D;");
                b = getData();
            } while (b == 0);
        }
        /// <summary>
        /// escibe en la memoria de programa
        /// </summary>
        /// <param name="HDir"></param>
        /// <param name="MDir"></param>
        /// <param name="LDir"></param>
        /// <param name="Hdata"></param>
        /// <param name="LData"></param>
        public void WriteProgramMemory(Byte HDir, Byte MDir, Byte LDir, Byte Hdata, Byte LData)
        {
            byte b = 0;
            do
            {
                serialPort1.WriteLine(":WP," + HDir.ToString("x") + "," + MDir.ToString("x") + "," + LDir.ToString("x") + "," + Hdata.ToString("x") + "," + LData.ToString("x") + ";");
                //espero la respuesta
                b = getData();
            }
            while (b == 0);
        }
        public void WriteConfigMemory(Byte HDir, Byte MDir, Byte LDir, Byte Hdata, Byte LData)
        {
            byte b = 0;
            do
            {
                serialPort1.WriteLine(":WC," + HDir.ToString("x") + "," + MDir.ToString("x") + "," + LDir.ToString("x") + "," + Hdata.ToString("x") + "," + LData.ToString("x") + ";");
                //espero la respuesta
                b = getData();
            }
            while (b == 0);
        }
        /// <summary>
        /// escribe en la memoria de la EEPROM
        /// </summary>
        /// <param name="HDir"></param>
        /// <param name="LDir"></param>
        /// <param name="Data"></param>
        public void WriteDataMemory(Byte HDir, Byte LDir, Byte Data)
        {
            byte b = 0;
            do
            {
                serialPort1.WriteLine(":WD," + HDir.ToString("x") + "," + LDir.ToString("x") + "," + Data.ToString("x") + ";");
                b = getData();
            }
            while (b == 0);
        }
        /// <summary>
        /// Escribe la palabra de configuracion
        /// </summary>
        /// <param name="HConfig"></param>
        /// <param name="LConfig"></param>
        public void WriteConfigurationBits(Byte HConfig, Byte LConfig)
        {
            byte b = 0;
            do
            {
                serialPort1.WriteLine(":C,"+ HConfig .ToString("x")+ ","+ LConfig.ToString("x") + ";");
                b = getData();
            }
            while (b == 0);
        }
        /// <summary>
        /// regresa el dato almacenado en la direccion de la memoria de programa
        /// </summary>
        /// <param name="HDir"></param>
        /// <param name="MDir"></param>
        /// <param name="LDir"></param>
        /// <returns></returns>
        public byte ReadProgramMemory(Byte HDir, Byte MDir, Byte LDir)
        {
            bool ok = true;
            byte b = 0;
            do
            {
                ok = true;
                try
                {
                    b = 0;
                    serialPort1.WriteLine(":RP," + HDir.ToString("x") + "," + MDir.ToString("x") + "," + LDir.ToString("x") + ";");
                    b = getData();
                }
                catch(System.Exception)
                {
                    ok = false;
                }
            }
            while (ok == false);
            return b;
        }
        /// <summary>
        /// regresa el dato almacenado en la direccion de la memoria EEPROM
        /// </summary>
        /// <param name="HDir"></param>
        /// <param name="LDir"></param>
        /// <returns></returns>
        public byte ReadDataMemory(Byte HDir, Byte LDir)
        {
            byte b = 0;
            serialPort1.WriteLine(":RD," + HDir.ToString("x") + "," +  LDir.ToString("x") + ";");
            b = getData();
            return b;
        }
        public UserControl GetConfigurator()
        {
            if (Configurator == null)
            {
                Configurator = new ArduinoProgramer.CArduinoProgramerConfigurator();
                Configurator.OnClickConnec += new ArduinoProgramer.CArduinoProgramerConfiguratorEvent(ClickConnect);
            }
            return Configurator;
        }
        private void ClickConnect(string puerto, int velocidad)
        {
            if(serialPort1.IsOpen)
            {
                //esta abierto, por lo que hay que cerrarlo
                serialPort1.Close();
                Configurator.EnbleControls = true;
            }
            else
            {
                //hay que conectar
                PortName = puerto;
                BaudRate =  velocidad;
                serialPort1.Open();
                Configurator.EnbleControls = false;
            }
        }
        #endregion
        public override string ToString()
        {
            return "Arduino como grabador";
        }
        public bool IsReady()
        {
            return serialPort1.IsOpen;
        }
    }
}
