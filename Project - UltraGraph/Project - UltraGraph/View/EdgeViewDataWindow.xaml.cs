using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MarkusRezai.Project.UltraGraph.View
{
    /// <summary>
    /// Interaction logic for EdgeArrowLinePropertiesWindow.xaml
    /// </summary>
    public partial class EdgeViewDataWindow : MetroWindow
    {
        public string ViewDataName { get { return tbName.Text; } set { tbName.Text = value; } }

        public byte StrokeBrushColorA { get { return byte.Parse(tbStrokeBrushColorA.Text); } set { tbStrokeBrushColorA.Text = value.ToString(); } }
        public byte StrokeBrushColorR { get { return byte.Parse(tbStrokeBrushColorR.Text); } set { tbStrokeBrushColorR.Text = value.ToString(); } }
        public byte StrokeBrushColorG { get { return byte.Parse(tbStrokeBrushColorG.Text); } set { tbStrokeBrushColorG.Text = value.ToString(); } }
        public byte StrokeBrushColorB { get { return byte.Parse(tbStrokeBrushColorB.Text); } set { tbStrokeBrushColorB.Text = value.ToString(); } }

        public double StrokeThickness { get { return double.Parse(tbStrokeThickness.Text); } set { tbStrokeThickness.Text = value.ToString(); } }
        public double ArrowAngle { get { return double.Parse(tbArrowAngle.Text); } set { tbArrowAngle.Text = value.ToString(); } }
        public double ArrowLength { get { return double.Parse(tbArrowLength.Text); } set { tbArrowLength.Text = value.ToString(); } }
        public bool IsArrowHeadClosed { get { return cbIsArrowHeadClosed.IsChecked.Value; } set { cbIsArrowHeadClosed.IsChecked = value; } }

        public EdgeViewDataWindow()
        {
            InitializeComponent();
        }

        private void OkayButtonClick_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;

            e.Handled = true;
        }

        private void OkayButtonClick_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            byte ignoreMeByte;
            double ignoreMeDouble;

            e.CanExecute = tbStrokeBrushColorA != null &&
                           tbStrokeBrushColorR != null &&
                           tbStrokeBrushColorG != null &&
                           tbStrokeBrushColorB != null &&
                           tbStrokeThickness != null &&
                           tbArrowAngle != null &&
                           tbArrowLength != null &&
                           cbIsArrowHeadClosed != null &&
                           byte.TryParse(tbStrokeBrushColorA.Text, out ignoreMeByte) &&
                           byte.TryParse(tbStrokeBrushColorR.Text, out ignoreMeByte) &&
                           byte.TryParse(tbStrokeBrushColorG.Text, out ignoreMeByte) &&
                           byte.TryParse(tbStrokeBrushColorB.Text, out ignoreMeByte) &&
                           double.TryParse(tbStrokeThickness.Text, out ignoreMeDouble) &&
                           double.TryParse(tbArrowAngle.Text, out ignoreMeDouble) &&
                           double.TryParse(tbArrowLength.Text, out ignoreMeDouble) &&
                           cbIsArrowHeadClosed.IsChecked != null;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            e.Handled = true;
        }
    }
}
