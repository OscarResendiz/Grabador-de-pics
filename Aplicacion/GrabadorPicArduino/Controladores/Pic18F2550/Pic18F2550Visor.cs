using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controladores.Pic18F2550
{
    public delegate void Pic18F2550VisorDelegate();
    public partial class Pic18F2550Visor : UserControl
    {
        public event Pic18F2550VisorDelegate OnBorrar;
        public event Pic18F2550VisorDelegate OnGrabar;
        public event Pic18F2550VisorDelegate OnLeer;

        public Pic18F2550Visor()
        {
            InitializeComponent();
        }

        private void BAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK)
                    return;
                hexFileComponent1.ReadFile(openFileDialog1.FileName);
                visorHexFile1.SetData(hexFileComponent1);
                CargaConfiguracion();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BGuardar_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            hexFileComponent1.Guardar(saveFileDialog1.FileName);
        }

        private void BBorar_Click(object sender, EventArgs e)
        {
            if (OnBorrar != null)
                OnBorrar();
        }

        private void BGrabar_Click(object sender, EventArgs e)
        {
            LStatus.Text = "Grabando";
            if (OnGrabar != null)
                OnGrabar();
        }

        private void BLeer_Click(object sender, EventArgs e)
        {
            LStatus.Text = "Leyendo";

            if (OnLeer != null)
                OnLeer();
        }
        public bool EnableBootons
        {
            set
            {
                BBorar.Enabled = value;
                BGrabar.Enabled = value;
                BLeer.Enabled = value;

            }
        }
        public void ReportProgreso(int maximo, int valor)
        {
            BarraProgreso.Maximum = maximo;
            BarraProgreso.Value = valor;
        }
        public int TotalDatos
        {
            get
            {
                return hexFileComponent1.TotalDatos;
            }
        }
        public HexFile.CDatoHex GetDato(int pos)
        {
            return hexFileComponent1.GetDato(pos);
        }
        public void SetData(HexFile.HexFileComponent data)
        {
            hexFileComponent1 = data;
            visorHexFile1.SetData(hexFileComponent1);
        }
        #region palabras de configuracion
        #region CONFIG1L
        public byte CONFIG1L
        {
            get
            {
                byte b = (byte)ComboPLLDIV.SelectedIndex;
                switch (ComboCPUDIV.SelectedIndex)
                {
                    case 1:
                        b += 8;
                        break;
                    case 2:
                        b += 16;
                        break;
                    case 3:
                        b += 24;
                        break;
                }
                if (CONFIG1L_5.Checked)
                    b += 32;
                return b;
            }
            set
            {
                ComboPLLDIV.SelectedIndex = value & 0x07;

                switch(value & 0x18)
                {
                    case 0x00:
                        ComboCPUDIV.SelectedIndex = 0;
                        break;
                    case 0x08:
                        ComboCPUDIV.SelectedIndex = 1;
                        break;
                    case 0x10:
                        ComboCPUDIV.SelectedIndex = 2;
                        break;
                    case 0x18:
                        ComboCPUDIV.SelectedIndex = 3;
                        break;
                }

                if ((value & 0x20) == 0x20)
                    CONFIG1L_5.Checked = true;
                else
                    CONFIG1L_5.Checked = false;
                MuestraValores();
            }
        }
        #endregion
        #region CONFIG1H
        public byte CONFIG1H
        {
            get
            {
                byte b = 0;
                switch(ComboOcilador.SelectedIndex)
                {
                    case 0:
                        b = 0x0E;
                        break;
                    case 1:
                        b = 0X0C;
                        break;
                    case 2:
                        b = 0X0B;
                        break;
                    case 3:
                        b = 0X0A;
                        break;
                    case 4:
                        b = 0X09;
                        break;
                    case 5:
                        b = 0X08;
                        break;
                    case 6:
                        b = 0X07;
                        break;
                    case 7:
                        b = 0X06;
                        break;
                    case 8:
                        b =0X05 ;
                        break;
                    case 9:
                        b = 0X04;
                        break;
                    case 10:
                        b = 0X02;
                        break;
                    case 11:
                        b = 0X00;
                        break;
                }





                if (CONFIG1H_6.Checked)
                    b += 64;
                if (CONFIG1H_7.Checked)
                    b += 128;
                return b;
            }
            set
            {
                if ((value & 0x0E) == 0x0E)
                    ComboOcilador.SelectedIndex =0;
               else if ((value & 0X0C) == 0X0C)
                    ComboOcilador.SelectedIndex =1;
                else if ((value & 0X0B) == 0X0B)
                    ComboOcilador.SelectedIndex =2;
                else if ((value & 0X0A) == 0X0A)
                    ComboOcilador.SelectedIndex =3;
                else if ((value & 0X09) == 0X09)
                    ComboOcilador.SelectedIndex =4;
                else if ((value & 0X08) == 0X08)
                    ComboOcilador.SelectedIndex =5;
                else if ((value & 0X07) == 0X07)
                    ComboOcilador.SelectedIndex =6;
                else if ((value & 0X06) == 0X06)
                    ComboOcilador.SelectedIndex =7;
                else if ((value & 0X05) == 0X05)
                    ComboOcilador.SelectedIndex =8;
                else if ((value & 0X04) == 0X04)
                    ComboOcilador.SelectedIndex =9;
                else if ((value & 0X02) == 0X02)
                    ComboOcilador.SelectedIndex =10;
                else if ((value & 0X00) == 0X00)
                    ComboOcilador.SelectedIndex =11;

                if ((value & 0x40) == 0x40)
                    CONFIG1H_6.Checked = true;
                else
                    CONFIG1H_6.Checked = false;

                if ((value & 0x80) == 0x80)
                    CONFIG1H_7.Checked = true;
                else
                    CONFIG1H_7.Checked = false;
                MuestraValores();

            }
        }
        #endregion
        #region CONFIG2L
        public byte CONFIG2L
        {
            get
            {
                byte b = 0;
                if (CONFIG2L_0.Checked)
                    b = 1;
                switch(ComboBOREN.SelectedIndex)
                {
                    case 0:
                        b += 0x00;
                        break;
                    case 1:
                        b += 0x02;
                        break;
                    case 2:
                        b += 0x04;
                        break;
                    case 3:
                        b += 0x06;
                        break;
                }
                switch(ComboBORV.SelectedIndex)
                {
                    case 0:
                        b +=0;
                        break;
                    case 1:
                        b +=0x08;
                        break;
                    case 2:
                        b +=0x10;
                        break;
                    case 3:
                        b +=0x18;
                        break;
                }
                if (CONFIG2L_5.Checked)
                    b += 32;
                return b;
            }
            set
            {
                if ((value & 0x01) == 0x01)
                    CONFIG2L_0.Checked = true;
                else
                    CONFIG2L_0.Checked = false;

                switch(value&0x06)
                {
                    case 0x00:
                        ComboBOREN.SelectedIndex =0;
                        break;
                    case 0x02:
                        ComboBOREN.SelectedIndex =1;
                        break;
                    case 0x04:
                        ComboBOREN.SelectedIndex =2;
                        break;
                    case 0x06:
                        ComboBOREN.SelectedIndex =3;
                        break;
                }

                switch(value&0x18)
                {
                    case 0x00:
                        ComboBORV.SelectedIndex =0;
                        break;
                    case 0x08:
                        ComboBORV.SelectedIndex =1;
                        break;
                    case 0x10:
                        ComboBORV.SelectedIndex =2;
                        break;
                    case 0x18:
                        ComboBORV.SelectedIndex =3;
                        break;
                }

                if ((value & 0x20) == 0x20)
                    CONFIG2L_5.Checked = true;
                else
                    CONFIG2L_5.Checked = false;
                MuestraValores();
            }
        }
        #endregion
        #region CONFIG2H
        public byte CONFIG2H
        {
            get
            {
                byte b = 0;
                if (CONFIG2H_0.Checked)
                    b = 1;
                switch(ComboWDTPS.SelectedIndex)
                {
                    case 0:
                        b += 0x00;
                        break;
                    case 1:
                        b += 0x02;
                        break;
                    case 2:
                        b += 0x04;
                        break;
                    case 3:
                        b += 0x06;
                        break;
                    case 4:
                        b += 0x08;
                        break;
                    case 5:
                        b += 0x0A;
                        break;
                    case 6:
                        b += 0x0C;
                        break;
                    case 7:
                        b += 0x0E;
                        break;
                    case 8:
                        b += 0x10;
                        break;
                    case 9:
                        b += 0x12;
                        break;
                    case 10:
                        b += 0x14;
                        break;
                    case 11:
                        b += 0x16;
                        break;
                    case 12:
                        b += 0x18;
                        break;
                    case 13:
                        b += 0x1A;
                        break;
                    case 14:
                        b += 0x1C;
                        break;
                    case 15:
                        b += 0x1E;
                        break;
                }
                return b;
            }
            set
            {
                if ((value & 0x01) == 0x01)
                    CONFIG2H_0.Checked = true;
                else
                    CONFIG2H_0.Checked = false;
                switch(value&0x1E)
                {
                    case 0x00:
                        ComboWDTPS.SelectedIndex =0;
                        break;
                    case 0x02:
                        ComboWDTPS.SelectedIndex =1;
                        break;
                    case 0x04:
                        ComboWDTPS.SelectedIndex =2;
                        break;
                    case 0x06:
                        ComboWDTPS.SelectedIndex =3;
                        break;
                    case 0x08:
                        ComboWDTPS.SelectedIndex =4;
                        break;
                    case 0x0A:
                        ComboWDTPS.SelectedIndex =5;
                        break;
                    case 0x0C:
                        ComboWDTPS.SelectedIndex =6;
                        break;
                    case 0x0E:
                        ComboWDTPS.SelectedIndex =7;
                        break;
                    case 0x10:
                        ComboWDTPS.SelectedIndex =8;
                        break;
                    case 0x12:
                        ComboWDTPS.SelectedIndex =9;
                        break;
                    case 0x14:
                        ComboWDTPS.SelectedIndex =10;
                        break;
                    case 0x16:
                        ComboWDTPS.SelectedIndex =11;
                        break;
                    case 0x18:
                        ComboWDTPS.SelectedIndex =12;
                        break;
                    case 0x1A:
                        ComboWDTPS.SelectedIndex =13;
                        break;
                    case 0x1C:
                        ComboWDTPS.SelectedIndex =14;
                        break;
                    case 0x1E:
                        ComboWDTPS.SelectedIndex =15;
                        break;
                }
                MuestraValores();
            }
        }
        #endregion
        #region CONFIG3H
        public byte CONFIG3H
        {
            get
            {
                byte b = 0;
                if (CONFIG3H_0.Checked)
                    b = 1;
                if (CONFIG3H_1.Checked)
                    b += 2;
                if (CONFIG3H_2.Checked)
                    b += 4;
                if (CONFIG3H_7.Checked)
                    b += 128;
                return b;
            }
            set
            {
                if ((value & 0x01) == 0x01)
                    CONFIG3H_0.Checked = true;
                else
                    CONFIG3H_0.Checked = false;

                if ((value & 0x02) == 0x02)
                    CONFIG3H_1.Checked = true;
                else
                    CONFIG3H_1.Checked = false;

                if ((value & 0x04) == 0x04)
                    CONFIG3H_2.Checked = true;
                else
                    CONFIG3H_2.Checked = false;

                if ((value & 0x80) == 0x80)
                    CONFIG3H_7.Checked = true;
                else
                    CONFIG3H_7.Checked = false;
                MuestraValores();

            }
        }
        #endregion
        #region CONFIG4L
        public byte CONFIG4L
        {
            get
            {
                byte b = 0;
                if (CONFIG4L_0.Checked)
                    b = 1;
                if (CONFIG4L_2.Checked)
                    b += 4;
                if (CONFIG4L_5.Checked)
                    b += 32;
                if (CONFIG4L_6.Checked)
                    b += 64;
                if (CONFIG4L_7.Checked)
                    b += 128;
                return b;
            }
            set
            {
                if ((value & 0x01) == 0x01)
                    CONFIG4L_0.Checked = true;
                else
                    CONFIG4L_0.Checked = false;

                if ((value & 0x04) == 0x04)
                    CONFIG4L_2.Checked = true;
                else
                    CONFIG4L_2.Checked = false;
                if ((value & 0x20) == 0x20)
                    CONFIG4L_5.Checked = true;
                else
                    CONFIG4L_5.Checked = false;

                if ((value & 0x40) == 0x40)
                    CONFIG4L_6.Checked = true;
                else
                    CONFIG4L_6.Checked = false;

                if ((value & 0x80) == 0x80)
                    CONFIG4L_7.Checked = true;
                else
                    CONFIG4L_7.Checked = false;
                MuestraValores();

            }
        }
        #endregion
        #region CONFIG5L
        public byte CONFIG5L
        {
            get
            {
                byte b = 0;
                if (!CONFIG5L_0.Checked)
                    b = 1;
                if (!CONFIG5L_1.Checked)
                    b += 2;
                if (!CONFIG5L_2.Checked)
                    b += 4;
                if (!CONFIG5L_3.Checked)
                    b += 8;
                return b;
            }
            set
            {
                if ((value & 0x01) == 0x01)
                    CONFIG5L_0.Checked = false;
                else
                    CONFIG5L_0.Checked = true;

                if ((value & 0x02) == 0x02)
                    CONFIG5L_1.Checked = false;
                else
                    CONFIG5L_1.Checked = true;

                if ((value & 0x04) == 0x04)
                    CONFIG5L_2.Checked = false;
                else
                    CONFIG5L_2.Checked = true;

                if ((value & 0x08) == 0x08)
                    CONFIG5L_3.Checked = false;
                else
                    CONFIG5L_3.Checked = true;
                MuestraValores();
            }
        }
        #endregion
        #region CONFIG5H
        public byte CONFIG5H
        {
            get
            {
                byte b = 0;
                if (!CONFIG5H_6.Checked)
                    b += 64;
                if (!CONFIG5H_7.Checked)
                    b += 128;
                return b;
            }
            set
            {
                if ((value & 0x40) == 0x40)
                    CONFIG5H_6.Checked = false;
                else
                    CONFIG5H_6.Checked = true;

                if ((value & 0x80) == 0x80)
                    CONFIG5H_7.Checked = false;
                else
                    CONFIG5H_7.Checked = true;
                MuestraValores();

            }
        }
        #endregion
        #region CONFIG6L
        public byte CONFIG6L
        {
            get
            {
                byte b = 0;
                if (!CONFIG6L_0.Checked)
                    b = 1;
                if (!CONFIG6L_1.Checked)
                    b += 2;
                if (!CONFIG6L_2.Checked)
                    b += 4;
                if (!CONFIG6L_3.Checked)
                    b += 8;
                return b;
            }
            set
            {
                if ((value & 0x01) == 0x01)
                    CONFIG6L_0.Checked = false;
                else
                    CONFIG6L_0.Checked = true;

                if ((value & 0x02) == 0x02)
                    CONFIG6L_1.Checked = false;
                else
                    CONFIG6L_1.Checked = true;

                if ((value & 0x04) == 0x04)
                    CONFIG6L_2.Checked = false;
                else
                    CONFIG6L_2.Checked = true;

                if ((value & 0x08) == 0x08)
                    CONFIG6L_3.Checked = false;
                else
                    CONFIG6L_3.Checked = true;
                MuestraValores();
            }
        }
        #endregion
        #region CONFIG6H
        public byte CONFIG6H
        {
            get
            {
                byte b = 0;
                if (!CONFIG6H_5.Checked)
                    b += 32;
                if (!CONFIG6H_6.Checked)
                    b += 64;
                if (!CONFIG6H_7.Checked)
                    b += 128;
                return b;
            }
            set
            {
                if ((value & 0x20) == 0x20)
                    CONFIG6H_5.Checked = false;
                else
                    CONFIG6H_5.Checked = true;

                if ((value & 0x40) == 0x40)
                    CONFIG6H_6.Checked = false;
                else
                    CONFIG6H_6.Checked = true;

                if ((value & 0x80) == 0x80)
                    CONFIG6H_7.Checked = false;
                else
                    CONFIG6H_7.Checked = true;
                MuestraValores();

            }
        }
        #endregion
        #region CONFIG7L
        public byte CONFIG7L
        {
            get
            {
                byte b = 0;
                if (!CONFIG7L_0.Checked)
                    b = 1;
                if (!CONFIG7L_1.Checked)
                    b += 2;
                if (!CONFIG7L_2.Checked)
                    b += 4;
                if (!CONFIG7L_3.Checked)
                    b += 8;
                return b;
            }
            set
            {
                if ((value & 0x01) == 0x01)
                    CONFIG7L_0.Checked = false;
                else
                    CONFIG7L_0.Checked = true;

                if ((value & 0x02) == 0x02)
                    CONFIG7L_1.Checked = false;
                else
                    CONFIG7L_1.Checked = true;

                if ((value & 0x04) == 0x04)
                    CONFIG7L_2.Checked = false;
                else
                    CONFIG7L_2.Checked = true;

                if ((value & 0x08) == 0x08)
                    CONFIG7L_3.Checked = false;
                else
                    CONFIG7L_3.Checked = true;
                MuestraValores();
            }
        }
        #endregion
        #region CONFIG7H
        public byte CONFIG7H
        {
            get
            {
                byte b = 0;
                if (!CONFIG7H_6.Checked)
                    b += 64;
                return b;
            }
            set
            {
                if ((value & 0x40) == 0x40)
                    CONFIG7H_6.Checked = false;
                else
                    CONFIG7H_6.Checked = true;
                MuestraValores();
            }
        }
        #endregion
        #region DEVID1
        private byte FDEVID1;
        public byte DEVID1
        {
            get
            {
                return FDEVID1;
            }
            set
            {
                FDEVID1 = value;
                IdentificaPic();
            }
        }
        #endregion
        #region DEVID2
        private byte FDEVID2;
        public byte DEVID2
        {
            get
            {
                return FDEVID2;
            }
            set
            {
                FDEVID2 = value;
                IdentificaPic();
            }
        }
        #endregion
        #endregion
        public String Status
        {
            get
            {
                return LStatus.Text;
            }
            set
            {
                LStatus.Text = value;
            }
        }
        public Color StatusColor
        {
            get
            {
                return LStatus.ForeColor;
            }
            set
            {
                LStatus.ForeColor = value;
            }
        }
        /// <summary>
        /// carga la configuracion del archivo y la muestra en la pantalla
        /// </summary>
        private void CargaConfiguracion()
        {
            HexFile.CDatoHex d = null;
            d = hexFileComponent1.GetDatoDir(0x300000);
            if (d != null)
            {
                CONFIG1L = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x300001);
            if (d != null)
            {
                CONFIG1H = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x300002);
            if (d != null)
            {
                CONFIG2L = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x300003);
            if (d != null)
            {
                CONFIG2H = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x300005);
            if (d != null)
            {
                CONFIG3H = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x300006);
            if (d != null)
            {
                CONFIG4L = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x300008);
            if (d != null)
            {
                CONFIG5L = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x300009);
            if (d != null)
            {
                CONFIG5H = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x30000A);
            if (d != null)
            {
                CONFIG6L = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x30000B);
            if (d != null)
            {
                CONFIG6H = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x30000C);
            if (d != null)
            {
                CONFIG7L = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x30000D);
            if (d != null)
            {
                CONFIG7H = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x3FFFFE);
            if (d != null)
            {
                DEVID1 = d.HexDatoH;
            }

            d = hexFileComponent1.GetDatoDir(0x3FFFFF);
            if (d != null)
            {
                DEVID2 = d.HexDatoH;
            }
            MuestraValores();
        }
        private void MuestraValores()
        {
            LCONFIG1L.Text = CONFIG1L.ToString("x").ToUpper();
            LCONFIG1H.Text = CONFIG1H.ToString("x").ToUpper();
            LCONFIG2L.Text = CONFIG2L.ToString("x").ToUpper();
            LCONFIG2H.Text = CONFIG2H.ToString("x").ToUpper();
            LCONFIG3H.Text = CONFIG3H.ToString("x").ToUpper();
            LCONFIG4L.Text = CONFIG4L.ToString("x").ToUpper();
            LCONFIG5L.Text = CONFIG5L.ToString("x").ToUpper();
            LCONFIG5H.Text = CONFIG5H.ToString("x").ToUpper();
            LCONFIG6L.Text = CONFIG6L.ToString("x").ToUpper();
            LCONFIG6H.Text = CONFIG6H.ToString("x").ToUpper();
            LCONFIG7L.Text = CONFIG7L.ToString("x").ToUpper();
            LCONFIG7H.Text = CONFIG7H.ToString("x").ToUpper();
        }

        private void CONFIG1L_5_CheckedChanged(object sender, EventArgs e)
        {
            MuestraValores();
        }
        /// <summary>
        /// identifica el dispositivo dependiendo del valor de los registros DEVID1 y DEVID2
        /// </summary>
        private void IdentificaPic()
        {
            string dispositivo = "Desconocido";
            switch(DEVID2)
            {
                case 0x21:
                    switch(DEVID1&0xE0)
                    {
                        case 0x60:
                            dispositivo = "PIC18F2221";
                            break;
                        case 0x20:
                            dispositivo = "PIC18F2321";
                            break;
                        case 0x40:
                            dispositivo = "PIC18F4221";
                            break;
                        case 0x00:
                            dispositivo = "PIC18F4321";
                            break;
                    }
                    break;
                case 0x11:
                    switch(DEVID1 & 0xE0)
                    {
                        case 0x60:
                            dispositivo = "PIC18F2410";
                            break;
                        case 0x40:
                            if((DEVID1 & 0xF0)==0x40)
                                dispositivo = "PIC18F2420";
                            else
                                dispositivo = "PIC18F2423";
                            break;
                        case 0x20:
                            dispositivo = "PIC18F2510";
                            break;
                        case 0x00:
                            if ((DEVID1 & 0xF0) == 0x00)
                                dispositivo = "PIC18F2520";
                            else
                                dispositivo = "PIC18F2523";
                            break;
                    }
                    break;
                case 0x24:
                    switch (DEVID1 & 0xE0)
                    {
                        case 0x20:
                            dispositivo = "PIC18F2450";
                            break;
                        case 0x00:
                            dispositivo = "PIC18F4450";
                            break;
                    }
                    break;
                case 0x12:
                    switch (DEVID1 & 0xE0)
                    {
                        case 0x60:
                            dispositivo = "PIC18F2455";
                            break;
                        case 0x40:
                            dispositivo = "PIC18F2550";
                            break;
                        case 0x20:
                            dispositivo = "PIC18F4455";
                            break;
                        case 0x00:
                            dispositivo = "PIC18F4550";
                            break;
                    }
                    break;
                case 0x2A:
                    switch (DEVID1 & 0xE0)
                    {
                        case 0x60:
                            dispositivo = "PIC18F2455";
                            break;
                        case 0x40:
                            dispositivo = "PIC18F2553";
                            break;
                        case 0x20:
                            dispositivo = "PIC18F4458";
                            break;
                        case 0x00:
                            dispositivo = "PIC18F4553";
                            break;
                    }
                    break;
                case 0x1A:
                    switch (DEVID1 & 0xE0)
                    {
                        case 0xE0:
                            dispositivo = "PIC18F2480";
                            break;
                        case 0xC0:
                            dispositivo = "PIC18F2580";
                            break;
                        case 0xA0:
                            dispositivo = "PIC18F4480";
                            break;
                        case 0x80:
                            dispositivo = "PIC18F4580";
                            break;
                    }
                    break;
                case 0x0C:
                    switch (DEVID1 & 0xE0)
                    {
                        case 0xE0:
                            dispositivo = "PIC18F2515";
                            break;
                        case 0xC0:
                            dispositivo = "PIC18F2525";
                            break;
                        case 0xA0:
                            dispositivo = "PIC18F2610";
                            break;
                        case 0x80:
                            dispositivo = "PIC18F2620";
                            break;
                        case 0x60:
                            dispositivo = "PIC18F4515";
                            break;
                        case 0x40:
                            dispositivo = "PIC18F4525";
                            break;
                        case 0x20:
                            dispositivo = "PIC18F4610";
                            break;
                        case 0x00:
                            dispositivo = "PIC18F4620";
                            break;
                    }
                    break;
                case 0x0E:
                    switch (DEVID1 & 0xE0)
                    {
                        case 0xE0:
                            dispositivo = "PIC18F2585";
                            break;
                        case 0xC0:
                            dispositivo = "PIC18F2680";
                            break;
                        case 0xA0:
                            dispositivo = "PIC18F4585";
                            break;
                        case 0x80:
                            dispositivo = "PIC18F4680";
                            break;
                    }
                    break;
                case 0x27:
                    switch (DEVID1 & 0xE0)
                    {
                        case 0x00:
                            dispositivo = "PIC18F2682";
                            break;
                        case 0x20:
                            dispositivo = "PIC18F2685";
                            break;
                        case 0x40:
                            dispositivo = "PIC18F4682";
                            break;
                        case 0x60:
                            dispositivo = "PIC18F4685";
                            break;
                    }
                    break;
                case 0x10:
                    switch (DEVID1 & 0xE0)
                    {
                        case 0xE0:
                            dispositivo = "PIC18F4410";
                            break;
                        case 0xC0:
                            if((DEVID1 & 0xF0)==0xC0)
                                dispositivo = "PIC18F4420";
                            else
                                dispositivo = "PIC18F4423";
                            break;
                        case 0xA0:
                            dispositivo = "PIC18F4510";
                            break;
                        case 0x80:
                            if ((DEVID1 & 0xF0) == 0x80)
                                dispositivo = "PIC18F4520";
                            else
                                dispositivo = "PIC18F4523";
                            break;
                    }
                    break;
            }
            TDevId.Text = dispositivo;
        }

        private void ComboOcilador_SelectedIndexChanged(object sender, EventArgs e)
        {
            MuestraValores();
        }
    }
}
