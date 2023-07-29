using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GrabadorPicArduino
{
    public partial class Form1 : Form
    {
        private Programadores.IProgramer Grabador;
        private Controladores.IChip ChipControler;
        public Form1()
        {
            Grabador = null;
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //agrego los programadores que tiene soporte
            ComboGrabadores.Items.Add(new Programadores.CArduinoProgramer() );
            //agrego los pics que tiene soporte
            ComboChip.Items.Add(new Controladores.Pic18F2550.CPic18F2550Controler() );
        }

        private void ComboGrabadores_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboGrabadores.SelectedIndex == -1)
                return;
            Grabador = (Programadores.IProgramer)ComboGrabadores.SelectedItem;
            Contenedor.Controls.Clear();
            Contenedor.Controls.Add(Grabador.GetConfigurator());
            Contenedor.AutoSize = true;
            if (ChipControler != null)
                ChipControler.SetProgramer(Grabador);
        }

        private void ComboChip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboChip.SelectedIndex == -1)
                return;
            ChipControler = (Controladores.IChip)ComboChip.SelectedItem;
            PanelPrincipal.Controls.Clear();
            Control visor = ChipControler.GetVisor();
            visor.Dock = DockStyle.Fill;
            PanelPrincipal.Controls.Add(visor);
            if(Grabador!=null)
                ChipControler.SetProgramer(Grabador);
        }
    }
}
