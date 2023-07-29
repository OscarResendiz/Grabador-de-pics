using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Programadores;
using System.Drawing;
namespace Controladores.Pic18F2550
{
    /// <summary>
    /// controlador del pic 18F2550
    /// </summary>
    public partial class CPic18F2550Controler : Component, IChip
    {
        private CStatus Status = new CStatus();
        private IProgramer Programador;
        private Pic18F2550.Pic18F2550Visor Visor;
        List<HexFile.CDatoHex> ListaDatos = new List<HexFile.CDatoHex>();
        bool errores = false;
        private byte CONFIG1L;
        private byte CONFIG1H;
        private byte CONFIG2L ;
        private byte CONFIG2H ;
        private byte CONFIG3H ;
        private byte CONFIG4L ;
        private byte CONFIG5L ;
        private byte CONFIG5H ;
        private byte CONFIG6L ;
        private byte CONFIG6H ;
        private byte CONFIG7L ;
        private byte CONFIG7H ;
        private byte DEVID1 ;
        private byte DEVID2 ;

        private bool testconfig = false;

        public CPic18F2550Controler()
        {
            Visor = null;
            Programador = null;
            InitializeComponent();
        }

        public CPic18F2550Controler(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public Control GetVisor()
        {
            if (Visor == null)
            {
                Visor = new Pic18F2550Visor();
                Visor.OnBorrar += new Pic18F2550VisorDelegate(Borrar);
                Visor.OnGrabar += new Pic18F2550VisorDelegate(Grabar);
                Visor.OnLeer += new Pic18F2550VisorDelegate(Leer);

            }
            return Visor;
        }
        public void SetProgramer(IProgramer programador)
        {
            Programador = programador;
            if (Programador == null)
                return;
            if (Visor == null)
            {
                Visor = new Pic18F2550Visor();
                Visor.OnBorrar += new Pic18F2550VisorDelegate(Borrar);
                Visor.OnGrabar += new Pic18F2550VisorDelegate(Grabar);
                Visor.OnLeer += new Pic18F2550VisorDelegate(Leer);
            }
            Visor.EnableBootons = true;
        }
        public override string ToString()
        {
            return "Pic18F2550";
        }
        private void Grabar()
        {
            if(Programador.IsReady()==false)
            {
                MessageBox.Show(Visor, "El programador no esta listo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Visor.EnableBootons = false;
            Status.Maximo = Visor.TotalDatos;
            Status.Valor = 0;
            Status.Color = Color.YellowGreen;
            Status.Mensaje = "Grabando";
            Visor.ReportProgreso(Status.Maximo, 0);
            Visor.Status = Status.Mensaje;
            Visor.StatusColor = Status.Color;
            //me traigo las configuraciones
            CONFIG1L = Visor.CONFIG1L;
            CONFIG1H = Visor.CONFIG1H;
            CONFIG2L = Visor.CONFIG2L;
            CONFIG2H = Visor.CONFIG2H;
            CONFIG3H = Visor.CONFIG3H;
            CONFIG4L = Visor.CONFIG4L;
            CONFIG5L = Visor.CONFIG5L;
            CONFIG5H = Visor.CONFIG5H;
            CONFIG6L = Visor.CONFIG6L;
            CONFIG6H = Visor.CONFIG6H;
            CONFIG7L = Visor.CONFIG7L;
            CONFIG7H = Visor.CONFIG7H;
            DEVID1 = Visor.DEVID1;
            DEVID2 = Visor.DEVID2;
            BackProgam.RunWorkerAsync();

        }
        private void Leer()
        {
            if (Programador.IsReady() == false)
            {
                MessageBox.Show(Visor, "El programador no esta listo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Visor.EnableBootons = false;
            Status.Maximo = 0x7fff;
            Status.Color = Color.Green;
            Status.Mensaje = "Leyendo";
            Status.Valor = 0;
            Visor.ReportProgreso(Status.Maximo, 0);
            Visor.Status = Status.Mensaje;
            Visor.StatusColor = Status.Color;
            BackReader.RunWorkerAsync();
        }
        private void Borrar()
        {
            if (Programador.IsReady() == false)
            {
                MessageBox.Show(Visor, "El programador no esta listo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show(Visor, "Seguro que decea borrar el chip", "Borrar", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
            {
                return;
            }
            Programador.ErraceChip();
            Visor.Status = "Chip Borrado";
            Visor.StatusColor = Color.Red;
            MessageBox.Show(Visor, "Chip Borrado");

        }

        private void BackProgam_DoWork(object sender, DoWorkEventArgs e)
        {
            int pos = 0;
            errores = false;
            Programador.ErraceChip();
            Status.Mensaje = "Grabando Memoria de Codigo";
            if (testconfig == false)
            {
                do
                {
                    HexFile.CDatoHex dato;
                    HexFile.CDatoHex dato2;
                    Status.Valor = pos;
                    BackProgam.ReportProgress(pos);
                    dato = Visor.GetDato(pos);
                    pos++;
                    if (pos >= Status.Maximo)
                        break;
                    dato2 = Visor.GetDato(pos);
                    if (dato.Direccion <= 0x7fff)
                    {
                        Programador.WriteProgramMemory(dato.HexDireccionH, dato.HexDireccionM, dato.HexDireccionL, dato2.HexDatoH, dato.HexDatoH);
                    }
                    pos++;

                }
                while (pos < Status.Maximo);
                //ahora verifico
                Status.Mensaje = "Verificando Memoria de Codigo";
                Status.Color = Color.Blue;
                pos = 0;
                do
                {
                    HexFile.CDatoHex dato;
                    Status.Valor = pos;
                    BackProgam.ReportProgress(pos);
                    dato = Visor.GetDato(pos);
                    byte dator = Programador.ReadProgramMemory(dato.HexDireccionH, dato.HexDireccionM, dato.HexDireccionL);
                    if (dato.Direccion <= 0x7fff)
                    {
                        if (dator != dato.HexDatoH)
                        {
                            Status.Mensaje = "Error al verificar la direccion: " + dato.Direccion.ToString("x")/*.HexDireccionH.ToString()+ dato.HexDireccionM.ToString()+ dato.HexDireccionL.ToString()*/+ " se grabó " + dato.HexDatoH.ToString() + " y se leyó: " + dator.ToString();
                            Status.Color = Color.Red;
                            errores = true;
                            BackProgam.ReportProgress(pos);
                            return;
                        }
                    }
                    pos++;
                }
                while (pos < Status.Maximo);
            }
            //ahora grabo las configuraciones
            Status.Mensaje = "Grabando configuraciones";
            BackProgam.ReportProgress(0);
            Programador.WriteConfigMemory(0x30, 0x00, 0x00, CONFIG1L, CONFIG1L);
            Programador.WriteConfigMemory(0x30, 0x00, 0x01, CONFIG1H, CONFIG1H);
            Programador.WriteConfigMemory(0x30, 0x00, 0x02, CONFIG2L, CONFIG2L);
            Programador.WriteConfigMemory(0x30, 0x00, 0x03, CONFIG2H, CONFIG2H);
            Programador.WriteConfigMemory(0x30, 0x00, 0x05, CONFIG3H, CONFIG3H);
            Programador.WriteConfigMemory(0x30, 0x00, 0x06, CONFIG4L, CONFIG4L);
            Programador.WriteConfigMemory(0x30, 0x00, 0x08, CONFIG5L, CONFIG5L);
            Programador.WriteConfigMemory(0x30, 0x00, 0x09, CONFIG5H, CONFIG5H);
            Programador.WriteConfigMemory(0x30, 0x00, 0x0A, CONFIG6L, CONFIG6L);
            Programador.WriteConfigMemory(0x30, 0x00, 0x0B, CONFIG6H, CONFIG6H);
            Programador.WriteConfigMemory(0x30, 0x00, 0x0C, CONFIG7L, CONFIG7L);
            Programador.WriteConfigMemory(0x30, 0x00, 0x0D, CONFIG7H, CONFIG7H);

            //Programador.WriteConfigurationBits(CONFIG1H, CONFIG1L);
        }

        private void BackProgam_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Visor.ReportProgreso(Status.Maximo, Status.Valor);
            Visor.Status = Status.Mensaje;
            Visor.StatusColor = Status.Color;
        }

        private void BackProgam_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Visor.ReportProgreso(Status.Maximo, 0);
            Visor.EnableBootons = true;
            if (errores == false)
            {
                Visor.Status = "Grabació terminada";
            }
            MessageBox.Show(Visor, "Grabació terminada");
        }

        private void BackReader_DoWork(object sender, DoWorkEventArgs e)
        {
            Status.Mensaje = "Leyendo";
            ListaDatos = new List<HexFile.CDatoHex>();
            if (testconfig == false)
            {
                for (int i = 0; i < 0x7fff; i++)
                {
                    string s = i.ToString("x");
                    string s2 = "";
                    for (int j = 0; j < 8 - s.Length; j++)
                        s2 = s2 + "0";
                    s2 = s2 + s;
                    byte dh, dm, dl;
                    dh = (byte)Convert.ToInt32(s2.Substring(2, 2), 16);
                    dm = (byte)Convert.ToInt32(s2.Substring(4, 2), 16);
                    dl = (byte)Convert.ToInt32(s2.Substring(6, 2), 16);
                    byte d = Programador.ReadProgramMemory(dh, dm, dl);
                    ListaDatos.Add(new HexFile.CDatoHex
                    {
                        HexDato = d.ToString("x"),
                        HexDireccion = s2
                    });
                    Status.Valor = i;
                    Status.Mensaje = "Leyendo " + s2.ToUpper();// dh.ToString()+dm.ToString()+dl.ToString();
                    BackReader.ReportProgress(i);
                }
            }
            //cargo los bits de configuracion
            CONFIG1L = Programador.ReadProgramMemory(0x30, 0x00, 0x00);
            CONFIG1H = Programador.ReadProgramMemory(0x30, 0x00, 0x01);
            CONFIG2L = Programador.ReadProgramMemory(0x30, 0x00, 0x02);
            CONFIG2H = Programador.ReadProgramMemory(0x30, 0x00, 0x03);
            CONFIG3H = Programador.ReadProgramMemory(0x30, 0x00, 0x05);
            CONFIG4L = Programador.ReadProgramMemory(0x30, 0x00, 0x06);
            CONFIG5L = Programador.ReadProgramMemory(0x30, 0x00, 0x08);
            CONFIG5H = Programador.ReadProgramMemory(0x30, 0x00, 0x09);
            CONFIG6L = Programador.ReadProgramMemory(0x30, 0x00, 0x0A);
            CONFIG6H = Programador.ReadProgramMemory(0x30, 0x00, 0x0B);
            CONFIG7L = Programador.ReadProgramMemory(0x30, 0x00, 0x0C);
            CONFIG7H = Programador.ReadProgramMemory(0x30, 0x00, 0x0D);
            DEVID1 = Programador.ReadProgramMemory(0x3F, 0xFF, 0xFE);
            DEVID2 = Programador.ReadProgramMemory(0x3F, 0xFF, 0xFF);
            BackReader.ReportProgress(0x300000, CONFIG1L);
        }

        private void BackReader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch(e.ProgressPercentage)
            {
                case 0x300000:
                    Visor.CONFIG1L = CONFIG1L;
                    Visor.CONFIG1H = CONFIG1H;
                    Visor.CONFIG2L = CONFIG2L;
                    Visor.CONFIG2H = CONFIG2H;
                    Visor.CONFIG3H = CONFIG3H;
                    Visor.CONFIG4L = CONFIG4L;
                    Visor.CONFIG5L = CONFIG5L;
                    Visor.CONFIG5H = CONFIG5H;
                    Visor.CONFIG6L = CONFIG6L;
                    Visor.CONFIG6H = CONFIG6H;
                    Visor.CONFIG7L = CONFIG7L;
                    Visor.CONFIG7H = CONFIG7H;
                    Visor.DEVID1 = DEVID1;
                    Visor.DEVID2 = DEVID2;
                    break;
                default:
                    Visor.ReportProgreso(Status.Maximo, e.ProgressPercentage);
                    Visor.Status = Status.Mensaje;
                    Visor.StatusColor = Status.Color;
                    break;
            }
        }

        private void BackReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HexFile.HexFileComponent data = new HexFile.HexFileComponent();
            foreach (HexFile.CDatoHex d in ListaDatos)
            {
                data.Add(d);
            }
            Visor.SetData(data);
            Visor.EnableBootons = true;
            Visor.ReportProgreso(Status.Maximo, 0);
            Visor.Status = "";
        }
    }
}
