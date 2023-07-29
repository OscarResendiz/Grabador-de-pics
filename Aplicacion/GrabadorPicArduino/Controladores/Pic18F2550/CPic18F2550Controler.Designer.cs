namespace Controladores.Pic18F2550
{
    partial class CPic18F2550Controler
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.BackProgam = new System.ComponentModel.BackgroundWorker();
            this.BackReader = new System.ComponentModel.BackgroundWorker();
            // 
            // BackProgam
            // 
            this.BackProgam.WorkerReportsProgress = true;
            this.BackProgam.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackProgam_DoWork);
            this.BackProgam.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackProgam_ProgressChanged);
            this.BackProgam.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackProgam_RunWorkerCompleted);
            // 
            // BackReader
            // 
            this.BackReader.WorkerReportsProgress = true;
            this.BackReader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackReader_DoWork);
            this.BackReader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackReader_ProgressChanged);
            this.BackReader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackReader_RunWorkerCompleted);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker BackProgam;
        private System.ComponentModel.BackgroundWorker BackReader;
    }
}
