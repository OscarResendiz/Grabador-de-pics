using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Programadores.ArduinoProgramer
{
    public delegate void CArduinoProgramerConfiguratorEvent(string puerto, int velocidad);
    public partial class CArduinoProgramerConfigurator : UserControl
    {
        public event CArduinoProgramerConfiguratorEvent OnClickConnec;
        public CArduinoProgramerConfigurator()
        {
            InitializeComponent();
            ComboVelocidad.Items.Add("2400");
            ComboVelocidad.Items.Add("4800");
            ComboVelocidad.Items.Add("9600");
            ComboVelocidad.Items.Add("14400");
            ComboVelocidad.Items.Add("19200");
            ComboVelocidad.Items.Add("28800");
            ComboVelocidad.Items.Add("38400");
            ComboVelocidad.Items.Add("57600");
            ComboVelocidad.Items.Add("115200");
        }

        private void ComboSerial_DropDown(object sender, EventArgs e)
        {
            //cargo los puertos seriales
            ComboSerial.Items.Clear();
            string[] l = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string s in l)
            {
                ComboSerial.Items.Add(s);
            }

        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if(ComboSerial.Text=="")
            {
                MessageBox.Show(this,"Seleccione el puerto", "Error",  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (OnClickConnec != null)
                OnClickConnec(ComboSerial.Text,int.Parse(ComboVelocidad.Text));
        }
        public bool EnbleControls
        {
            get
            {
                return ComboSerial.Enabled;
            }
            set
            {
                ComboSerial.Enabled = value;
                ComboVelocidad.Enabled = value;
            }
        }
        public bool EnableConectButton
        {
            get
            {
                return toolStripButton5.Enabled;
            }
            set
            {
                toolStripButton5.Enabled = value;
            }
        }
    }
}
