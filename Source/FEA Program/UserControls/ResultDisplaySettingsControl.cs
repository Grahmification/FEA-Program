using FEA_Program.Models;
using FEA_Program.UI;

namespace FEA_Program.UserControls
{
    internal partial class ResultDisplaySettingsControl : UserControl
    {
        public NodeDrawManager NodeDrawManager { get; private set; } = new();
        public bool DisplacementEnabled { get => checkBox_displaced.Checked; set => checkBox_displaced.Checked = value; }


        public ResultDisplaySettingsControl()
        {
            InitializeComponent();
        }
        public void SetDrawManager(NodeDrawManager nodeDrawManager)
        {
            NodeDrawManager = nodeDrawManager;
            DisplacementEnabled = nodeDrawManager.DrawDisplaced;
            trackBar_displace.Value = (int)nodeDrawManager.DisplacePercentage;
            numericUpDown_scalingfactor.Value = (decimal)nodeDrawManager.DisplaceScaling;
        }

        // ----------------------- Event handlers --------------------------------
        private void checkBox_displaced_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                NodeDrawManager.DrawDisplaced = DisplacementEnabled;
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void trackBar_displace_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                TrackBar bar = (TrackBar)sender;
                NodeDrawManager.DisplacePercentage = bar.Value;
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void numericUpDown_scalingfactor_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                NumericUpDown box = (NumericUpDown)sender;
                NodeDrawManager.DisplaceScaling = (double)box.Value;
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
    }
}
