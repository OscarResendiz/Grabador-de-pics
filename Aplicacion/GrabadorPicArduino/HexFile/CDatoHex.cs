using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexFile
{
    /// <summary>
    /// representa una ceda de memoria
    /// </summary>
    public class CDatoHex
    {
        private string FHexDato="";
        /// <summary>
        /// direccion de memoria
        /// </summary>
        public int Direccion
        {
            get
            {
                return Convert.ToInt32(HexDireccion, 16);
            }
        }
        /// <summary>
        /// Dato 
        /// </summary>
        public int Dato
        {
            get
            {
                return Convert.ToInt32(HexDato, 16);

            }
        }
        /// <summary>
        /// direccion en exadecimal
        /// </summary>
        public string HexDireccion
        {
            get;
            set;
        }
        /// <summary>
        /// dato en hexadecimal
        /// </summary>
        public string HexDato
        {
            get
            {
                return FHexDato;
            }
            set
            {
                if (value.Length == 1)
                    FHexDato = "0" + value.ToUpper();
                else
                    FHexDato =value.ToUpper();
            }
        }
        public byte HexDireccionH
        {
            get
            {
                string s = HexDireccion.Substring(2, 2);
                return (byte)Convert.ToInt32(s, 16);
            }
        }
        public byte HexDireccionM
        {
            get
            {
                string s = HexDireccion.Substring(4, 2);
                return (byte)Convert.ToInt32(s, 16);
            }
        }
        public byte HexDireccionL
        {
            get
            {
                string s = HexDireccion.Substring(6, 2);
                return (byte)Convert.ToInt32(s, 16);
            }
        }
        public byte HexDatoH
        {
            get
            {
                string s = HexDato.Substring(0, 2);
                return (byte) Convert.ToInt32(s, 16);

            }
        }
    }
}
