using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Programadores
{
    /// <summary>
    /// interfaces que representa a un programador hardware
    /// </summary>
    public interface IProgramer
    {
        void ErraceChip();
        void WriteProgramMemory(Byte HDir, Byte MDir, Byte LDir, Byte Hdata, Byte LData);
        void WriteDataMemory(Byte HDir, Byte LDir, Byte Data);
        void WriteConfigurationBits(Byte HConfig, Byte LConfig);
        byte ReadProgramMemory(Byte HDir, Byte MDir, Byte LDir);
        byte ReadDataMemory(Byte HDir, Byte LDir);
        UserControl GetConfigurator();
        bool IsReady();
        void WriteConfigMemory(Byte HDir, Byte MDir, Byte LDir, Byte Hdata, Byte LData);
    }
}
