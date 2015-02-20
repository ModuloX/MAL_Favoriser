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
    /// Interaction logic for VertexEllipsePropertiesWindow.xaml
    /// </summary>
    public partial class VertexViewDataWindow : MetroWindow
    {
        public string ViewDataName { get { return tbName.Text; } set { tbName.Text = value; } }

        public byte FillBrushColorA { get { return byte.Parse(tbFillBrushColorA.Text); } set { tbFillBrushColorA.Text = value.ToString(); } }
        public byte FillBrushColorR { get { return byte.Parse(tbFillBrushColorR.Text); } set { tbFillBrushColorR.Text = value.ToString(); } }
        public byte FillBrushColorG { get { return byte.Parse(tbFillBrushColorG.Text); } set { tbFillBrushColorG.Text = value.ToString(); } }
        public byte FillBrushColorB { get { return byte.Parse(tbFillBrushColorB.Text); } set { tbFillBrushColorB.Text = value.ToString(); } }

        public byte StrokeBrushColorA { get { return byte.Parse(tbStrokeBrushColorA.Text); } set { tbStrokeBrushColorA.Text = value.ToString(); } }
        public byte StrokeBrushColorR { get { return byte.Parse(tbStrokeBrushColorR.Text); } set { tbStrokeBrushColorR.Text = value.ToString(); } }
        public byte StrokeBrushColorG { get { return byte.Parse(tbStrokeBrushColorG.Text); } set { tbStrokeBrushColorG.Text = value.ToString(); } }
        public byte StrokeBrushColorB { get { return byte.Parse(tbStrokeBrushColorB.Text); } set { tbStrokeBrushColorB.Text = value.ToString(); } }

        public double StrokeThickness { get { return double.Parse(tbStrokeThickness.Text); } set { tbStrokeThickness.Text = value.ToString(); } }
        public DoubleCollection StrokeDashes
        {
            get
            {
                DoubleCollection doubleCollection = new DoubleCollection();

                foreach (string subString in tbStrokeDashes.Text.Replace(" ", "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    double value;

                    if (double.TryParse(subString, out value)) doubleCollection.Add(value);
                }

                return doubleCollection;
            }

            set
            {
                StringBuilder result = new StringBuilder();

                foreach (double doubleValue in value) result.Append(doubleValue.ToString());

                tbStrokeDashes.Text = result.ToString();

            }
        }

        public VertexViewDataWindow()
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

            e.CanExecute = tbFillBrushColorA != null &&
                           tbFillBrushColorR != null &&
                           tbFillBrushColorG != null &&
                           tbFillBrushColorB != null &&
                           tbStrokeBrushColorA != null &&
                           tbStrokeBrushColorR != null &&
                           tbStrokeBrushColorG != null &&
                           tbStrokeBrushColorB != null &&
                           tbStrokeThickness != null &&
                           byte.TryParse(tbFillBrushColorA.Text, out ignoreMeByte) &&
                           byte.TryParse(tbFillBrushColorR.Text, out ignoreMeByte) &&
                           byte.TryParse(tbFillBrushColorG.Text, out ignoreMeByte) &&
                           byte.TryParse(tbFillBrushColorB.Text, out ignoreMeByte) &&
                           byte.TryParse(tbStrokeBrushColorA.Text, out ignoreMeByte) &&
                           byte.TryParse(tbStrokeBrushColorR.Text, out ignoreMeByte) &&
                           byte.TryParse(tbStrokeBrushColorG.Text, out ignoreMeByte) &&
                           byte.TryParse(tbStrokeBrushColorB.Text, out ignoreMeByte) &&
                           double.TryParse(tbStrokeThickness.Text, out ignoreMeDouble);
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            e.Handled = true;
        }
    }
}
