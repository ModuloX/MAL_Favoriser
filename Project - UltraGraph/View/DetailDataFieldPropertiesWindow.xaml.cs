using MahApps.Metro.Controls;
using MarkusRezai.Project.UltraGraph.Model;
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
    /// Interaction logic for DetailDataFieldPropertiesWindow.xaml
    /// </summary>
    public partial class DetailDataFieldPropertiesWindow : MetroWindow
    {
        public string FieldName { get { return tbName.Text; } set { tbName.Text = value; } }
        public string ValueType
        {
            get
            {
                if (spValueTypes.Children.OfType<RadioButton>().Where(x => x.GroupName == "ValueTypes").Any(x => x.IsChecked == true))
                    return spValueTypes.Children.OfType<RadioButton>().Where(x => x.GroupName == "ValueTypes").Single(x => x.IsChecked == true).Content.ToString();
                else
                    return "";
            }

            set
            {
                if (spValueTypes.Children.OfType<RadioButton>().Where(x => x.GroupName == "ValueTypes").Any(x => x.Content.ToString() == value))
                    spValueTypes.Children.OfType<RadioButton>().Where(x => x.GroupName == "ValueTypes").Single(x => x.Content.ToString() == value).IsChecked = true;
            }
        }
        public string DefaultValue { get { return tbDefaultValue.Text; } set { tbDefaultValue.Text = value; } }
        public int DisplayIndex { get { return int.Parse(tbDisplayIndex.Text); } set { tbDisplayIndex.Text = value.ToString(); } }
        public List<Tag> Tags { get { return (List<Tag>)lTags.ItemsSource; } set { lTags.ItemsSource = value; } }

        public DetailDataFieldPropertiesWindow()
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
            int ignoreMeInt;
            decimal ignoreMeDecimal;
            bool ignoreMeBool;
            DateTime ignoreMeDateTime;

            e.CanExecute = tbName != null &&
                           spValueTypes != null &&
                           tbDefaultValue != null &&
                           tbDisplayIndex != null &&
                           tbName.Text != string.Empty &&
                           spValueTypes.Children.OfType<RadioButton>().Where(x => x.GroupName == "ValueTypes").Any(x => x.IsChecked == true) &&
                           int.TryParse(tbDisplayIndex.Text, out ignoreMeInt);

            // Default value consistency check
            if (!e.CanExecute) return;
            if (ValueType == "Integer") e.CanExecute = e.CanExecute && int.TryParse(DefaultValue, out ignoreMeInt);
            if (ValueType == "Decimal") e.CanExecute = e.CanExecute && decimal.TryParse(DefaultValue, out ignoreMeDecimal);
            if (ValueType == "Boolean") e.CanExecute = e.CanExecute && bool.TryParse(DefaultValue, out ignoreMeBool);
            if (ValueType == "Date and Time") e.CanExecute = e.CanExecute && DateTime.TryParse(DefaultValue, out ignoreMeDateTime);

            e.Handled = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            e.Handled = true;
        }

        private void AddTag_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Tag result = new Tag();

            TagPropertiesWindow dialog = new TagPropertiesWindow();
            dialog.DisplayText = result.DisplayText;

            if (dialog.ShowDialog() != true) return;

            result.DisplayText = dialog.DisplayText;
            result.TagValues = dialog.TagValues;

            Tags.Add(result);

            lTags.Items.Refresh();
        }

        private void AddTag_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void EditTag_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Tag result = (Tag)lTags.SelectedItem;

            TagPropertiesWindow dialog = new TagPropertiesWindow();
            dialog.DisplayText = result.DisplayText;
            dialog.TagValues = result.TagValues;

            if (dialog.ShowDialog() != true) return;

            result.DisplayText = dialog.DisplayText;
            result.TagValues = dialog.TagValues;

            lTags.Items.Refresh();
        }

        private void EditTag_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lTags != null && (Tag)((ListBox)lTags).SelectedItem != null;

            e.Handled = true;
        }

        private void DeleteTag_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Tags.Remove((Tag)lTags.SelectedItem);

            lTags.Items.Refresh();
        }

        private void DeleteTag_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lTags != null && (Tag)((ListBox)lTags).SelectedItem != null;

            e.Handled = true;
        }
    }
}
