using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace HexFile
{
    public partial class HexFileComponent : Component
    {
        private string FileName = "";//nombre del archivo a abrir
//        private List<CDatoHex> Datos;
        private Dictionary<int, CDatoHex> Datos;
        public HexFileComponent()
        {
            Datos = new Dictionary<int, CDatoHex>();// List<CDatoHex>();
            InitializeComponent();
        }

        public HexFileComponent(IContainer container)
        {
            Datos = new Dictionary<int, CDatoHex>();// List<CDatoHex>();

            container.Add(this);

            InitializeComponent();
        }
        /// <summary>
        /// lee el archivo y genera el mapa de memoria
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public void ReadFile(string filename)
        {
            FileName = filename;
            int nlinea = 0;
            int nbytes = 0;
            string direccion = "00000000";
            string dirExt = "0000";
            int tipoRegistro = 0;
            int pos = 0;
            Datos.Clear();
            if (!File.Exists(FileName))
            {
                throw new Exception("No existe el archivo: " + FileName);
            }
            //abro el archivo
            StreamReader sr = File.OpenText(FileName);
            //leo el archivo mientras tenga datos
            while (sr.EndOfStream == false)
            {
                //me traigo la linea del archivo
                string Linea = sr.ReadLine();
                pos = 0;
                nlinea++;
                if (Linea.Length == 0)
                {
                    if (nlinea == 0)
                    {
                        throw new Exception("El archivo esta vacio");
                    }
                    //al pareccer ya termine de leer el archivo
                    return;
                }
                //verifico si el primer caracter es ':'
                if (Linea[pos] != ':')
                {
                    throw new Exception("Falta el caracter de inicio : en la linea " + nlinea.ToString());
                }
                pos++;
                //me traigo el numero de bytes que contiene la linea
                nbytes = Convert.ToInt32(Linea.Substring(pos, 2), 16);
                //me traigo la direccion
                pos += 2;
                direccion = dirExt + Linea.Substring(pos, 4); //a la direccion actual le agrego la direccion extendida
                //me traigo el tipo de registro
                pos += 4;
                tipoRegistro = Convert.ToInt32(Linea.Substring(pos, 2), 16);
                if (tipoRegistro == 1)
                {
                    //fin del archivo
                    sr.Close();
                    return;
                }
                if (tipoRegistro == 4)
                {
                    //contiene un direccion extendida
                    //leo la direccion extendida
                    pos += 2;
                    dirExt = Linea.Substring(pos, 4);
                }
                if (tipoRegistro == 0)
                {
                    //es una linea que contiene datos
                    //leo la informacion y la almaceno en el mapa de memoria
                    pos += 2;
                    for (int i = 0; i < nbytes; i++)
                    {
                        string data = Linea.Substring(pos, 2);
                        Datos.Add(int.Parse(direccion, System.Globalization.NumberStyles.HexNumber),new CDatoHex
                        {
                            HexDireccion = direccion,
                            HexDato = data
                        });
                        pos += 2;
                        string s = (Convert.ToInt32(direccion, 16) + 1).ToString("X");
                        string s2 = "";
                        for (int j = 0; j < 8 - s.Length; j++)
                            s2 = s2 + "0";
                        direccion = s2 + s; //incremento la direccion
                    }
                }
            }
        }
        /// <summary>
        /// regresa el numero total de datos almaceados
        /// </summary>
        public int TotalDatos
        {
            get
            {
                return Datos.Count();
            }
        }
        /// <summary>
        /// regresa el dato que se encuentra en la posicion pos
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public CDatoHex GetDato(int pos)
        {
            return Datos.ElementAt(pos).Value;//[direccion];
        }
        public CDatoHex GetDatoDir(int Dir)
        {
            if (Datos.ContainsKey(Dir))
                return Datos[Dir];
            return null;
        }
        public void Add(CDatoHex d)
        {
            if (Datos == null)
                Datos = new Dictionary<int, CDatoHex>();// List<CDatoHex>();
            Datos.Add(d.Direccion,d);
        }
        #region formato del archivo
        /*
            La estructura de una línea es:
             Start code Un caracter, ":" para líneas con contenido, ";" para comentarios.
             Byte count Dos caracteres HEX que indican el número de datos en Data.
             Address Cuatro caracteres HEX para la dirección del primer dato en Data.
             Record type Dos caracteres HEX que indican el tipo de línea, de 00 a 05. (ver mas abajo)
             Data Secuencia de 2 * n caracteres HEX correspondientes a los Byte count datos definidos antes.
             Checksum Dos caracteres HEX de Checksum calculado según el contenido anterior de la línea en la forma: El byte
            menos significativo del complemento a dos de la suma de los valores anteriores expresados como enteros los
            caracteres hexadecimales (sin incluir ni el Start code ni al él mismo)
        
            Cada línea puede expresar según su Record type:

             00 data record: Línea de datos, contiene la dirección del primer dato y la secuencia de datos a partir de ésa.
             01 End Of File record: Línea de Fin del Fichero HEX. Indica que se han acabado las líneas de datos. Usualmente es ":00000001FF"
             02 Extended Segment Address Record: Usado para procesadores 80x86 (No nos interesa aquí)
             03 Start Segment Address Record: Usado para procesadores 80x86 (No nos interesa aquí)
             04 Extended Linear Address Record: Si las líneas de datos que sigan a ésta necesitan una dirección mayor que la de
            16 bits ésta línea aporta los otros 16 bits para completar una dirección completa de 32 bits. Todas las líneas que
            sigan a esta deben completar su dirección con hasta los 32 bits con el contenido de la última línea de tipo 04
             05 Start Linear Address Record: Usado para procesadores 80386 o superiores (No nos interesa aquí)
             */
        #endregion
        /// <summary>
        /// conviert el byte a su equivalente en string a dos digitos
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private string ByteToString(byte b)
        {
            string s = b.ToString("X");
            string s2 = "";
            for (int j = 0; j < 2 - s.Length; j++)
                s2 = s2 + "0";
            s2 += s;
            return s2;
        }
        /// <summary>
        /// escribe una linea de codigo
        /// </summary>
        private void WriteLine(StreamWriter sw, byte Dir1, byte Dir0, byte type, List<byte> data)
        {
            byte count = (byte)data.Count;
            string cadena2 = "";
            //hago el calculo del checksum
            byte checksum = (byte)(count + Dir1 + Dir0);
            int checksum2 = count + Dir1 + Dir0;
            foreach (byte b in data)
            {
                checksum += b;
                checksum2 += b;
                cadena2 = cadena2 + ByteToString(b);
            }
            //hago la negacion de los bits
            checksum = (byte)~checksum;
            //y le sumo 1 para hacer el complemento a 2
            checksum += (byte)1;
            //escribo la linea
            string cadena = ":" + ByteToString(count) + ByteToString(Dir1) + ByteToString(Dir0) + ByteToString(type)+ cadena2 + ByteToString(checksum);
            sw.WriteLine(cadena);
        }
        /// <summary>
        /// pasa el contenido a un archivo con formato.hex
        /// </summary>
        /// <param name="fileName"></param>
        public void Guardar(string fileName)
        {
            byte d3 = 0, d2 = 0, d1 = 0, d0 = 0; //direcciones
            List<byte> buffer = new List<byte>();
            int n = 0;
            StreamWriter sw = File.CreateText(fileName);
            //grabo la primer linea
            sw.WriteLine(":020000040000FA");

            foreach (var d in Datos)
            {
                if (d2 != d.Value.HexDireccionH)
                {
                    //hubo cambio de direccion por lo que hay que iniciar una nueva linea
                    WriteLine(sw, d1, d0, 0x00, buffer);
                    //ahora escribo la nueva linea con el cambio de direccion
                    d2 = d.Value.HexDireccionH;
                    buffer.Clear();
                    buffer.Add(d3);
                    buffer.Add(d2);
                    WriteLine(sw, 0x00, 0x00, 0x04, buffer);
                    buffer.Clear();
                    n = 0;
                }
                if(n==0)
                {
                    //asigno la direccion inicial de la linea
                    d1 = d.Value.HexDireccionM;
                    d0 = d.Value.HexDireccionL;
                    //agrego el dato al buffer
                    buffer.Add(d.Value.HexDatoH);
                    n++;
                }
                else if(n==0x0f)
                {
                    //se llego al final de una linea
                    WriteLine(sw, d1, d0, 0x00, buffer);
                    n = 0; //reinicio el conteo de lineas
                    buffer.Clear();
                    buffer.Add(d.Value.HexDatoH);


                }
                else
                {
                    buffer.Add(d.Value.HexDatoH);
                    n++;
                }
            }
            //ya se termino de guardar ls datos
            //escribo la ultima linea
            sw.WriteLine(":00000001FF");
            sw.Close();
        }
    }
}
