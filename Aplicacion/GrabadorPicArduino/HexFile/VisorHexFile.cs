using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HexFile
{
    public partial class VisorHexFile : UserControl
    {
        private HexFileComponent DatosHex;
        public VisorHexFile()
        {
            InitializeComponent();
        }
        /// <summary>
        /// muestra la informacion de los datos
        /// </summary>
        /// <param name="datos"></param>
        public void SetData(HexFileComponent datos)
        {
            DatosHex = datos;
            dataSet1.Clear();
            DataTable tabla = dataSet1.Tables["Datos"];//me traigo la tabla de datos
            int total = DatosHex.TotalDatos;
            int pos = 0;
            DataRow dr = tabla.NewRow();
            int ultimaDir = -1;
            string ascii = "";
            for(int i=0;i<total;i++)
            {
                //me traigo el dato
                CDatoHex data = DatosHex.GetDato(i);
                if (ultimaDir==-1)
                {
                    //es la primer direccion a cargar
                    dr = tabla.NewRow();
                    dr["direccion"] = data.HexDireccion;
                    pos = 1;
                    ascii = "";
                }
                if (ultimaDir + 1 != data.Direccion) //la direccion actual no es la que sigue
                {
                    //hay que agregar una nueva linea
                    dr["ASCII"] = ascii;
                    tabla.Rows.Add(dr);
                    dr = tabla.NewRow();
                    dr["direccion"] = data.HexDireccion;
                    pos = 1;
                    ascii = "";
                }
                if (pos==17) ///llene la primer linea
                {
                    //hay que agregar una nueva linea porquer ya se lleno esta
                    dr["ASCII"] = ascii;
                    tabla.Rows.Add(dr);
                    dr = tabla.NewRow();
                    dr["direccion"] = data.HexDireccion;
                    pos = 1;
                    ascii = "";

                }
                dr[pos] = data.HexDato;
                ultimaDir = data.Direccion;
                ascii = ascii + (char)data.Dato;
                pos++;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
