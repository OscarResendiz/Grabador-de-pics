using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Controladores
{
      /// <summary>
      /// interface que representa a un chip el cual controla todo el proceso de visualizacion y programacion
      /// </summary>
    public interface IChip
    {
        Control GetVisor();
        void SetProgramer(Programadores.IProgramer programador);
    }
}
