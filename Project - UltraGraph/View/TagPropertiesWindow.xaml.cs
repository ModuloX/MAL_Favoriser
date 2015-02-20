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
    /// Interaction logic for TagPropertiesWindow.xaml
    /// </summary>
    public partial class TagPropertiesWindow : MetroWindow
    {
        public string DisplayText { get { return tbDisplayText.Text.Trim(); } set { tbDisplayText.Text = value; } }
        public List<String> TagValues
        {
            get { return tbTagValues.Text.Replace(" ", "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
            set
            {
                StringBuilder result = new StringBuilder();

                foreach (string tagValue in value) result.Append(tagValue).Append(" ; ");

                tbTagValues.Text = result.ToString();
            }
        }

        public TagPropertiesWindow()
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
            e.CanExecute = tbDisplayText != null && tbDisplayText.Text != string.Empty;

            e.Handled = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            e.Handled = true;
        }
    }
}
