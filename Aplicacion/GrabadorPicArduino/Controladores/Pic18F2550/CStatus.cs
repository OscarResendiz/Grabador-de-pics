using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace Controladores.Pic18F2550
{
    public class CStatus
    {
        public int Maximo
        {
            get;
            set;
        }
        public int Valor
        {
            get;
            set;
        }
        public Color Color
        {
            get;
            set;
        }
        public string Mensaje
        {
            get;
            set;
        }
    }
}
