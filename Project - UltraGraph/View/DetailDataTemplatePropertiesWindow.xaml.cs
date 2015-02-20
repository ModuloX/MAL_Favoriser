using MahApps.Metro.Controls;
using MarkusRezai.Project.UltraGraph.Model;
using MarkusRezai.Project.UltraGraph.ViewModel;
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
    /// Interaction logic for DetailDataTemplatePropertiesWindow.xaml
    /// </summary>
    public partial class DetailDataTemplatePropertiesWindow : MetroWindow
    {
        public string TemplateName { get { return tbName.Text; } set { tbName.Text = value; } }

        public List<DetailDataField<string>> StringFields { get; set; }
        public List<DetailDataField<long>> LongFields { get; set; }
        public List<DetailDataField<decimal>> DecimalFields { get; set; }
        public List<DetailDataField<bool>> BoolFields { get; set; }
        public List<DetailDataField<DateTime>> DateFields { get; set; }

        public List<DetailDataBaseField> DetailDataBaseFields
        {
            get
            {
                List<DetailDataBaseField> result = new List<DetailDataBaseField>();
                result.AddRange(StringFields);
                result.AddRange(LongFields);
                result.AddRange(DecimalFields);
                result.AddRange(BoolFields);
                result.AddRange(DateFields);
                return result.OrderBy(x => x.DisplayIndex).ToList();
            }
        }

        public DetailDataTemplatePropertiesWindow()
        {
            InitializeComponent();
        }

        private void OkayButtonClick_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
            Close();

            e.Handled = true;
        }

        private void OkayButtonClick_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            e.Handled = true;
        }

        private void AddDetailDataField_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DetailDataFieldPropertiesWindow dialog = new DetailDataFieldPropertiesWindow();
            dialog.FieldName = "New Field";
            dialog.ValueType = "Text";
            dialog.DefaultValue = "Default Value";
            dialog.DisplayIndex = 1;
            dialog.Tags = new List<Tag>();

            if (dialog.ShowDialog() != true) return;

            if (dialog.ValueType == "Text") StringFields.Add(new DetailDataField<string>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, dialog.DefaultValue));
            if (dialog.ValueType == "Integer") LongFields.Add(new DetailDataField<long>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, long.Parse(dialog.DefaultValue)));
            if (dialog.ValueType == "Decimal") DecimalFields.Add(new DetailDataField<decimal>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, decimal.Parse(dialog.DefaultValue)));
            if (dialog.ValueType == "Boolean") BoolFields.Add(new DetailDataField<bool>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, bool.Parse(dialog.DefaultValue)));
            if (dialog.ValueType == "Date and Time") DateFields.Add(new DetailDataField<DateTime>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, DateTime.Parse(dialog.DefaultValue)));

            lFields.ItemsSource = DetailDataBaseFields;
        }

        private void AddDetailDataField_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void EditDetailDataField_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: field muss removed und nachher neu hinzugefügt werden, weil sich der value type ändern könnte
            DetailDataBaseField targetBase = (DetailDataBaseField)((ListBox)lFields).SelectedItem;

            DetailDataFieldPropertiesWindow dialog = new DetailDataFieldPropertiesWindow();
            dialog.FieldName = targetBase.Name;
            dialog.DisplayIndex = targetBase.DisplayIndex;
            dialog.Tags = new List<Tag>(targetBase.Tags);

            if (StringFields.Contains(targetBase))
            {
                DetailDataField<string> target = targetBase as DetailDataField<string>;

                dialog.ValueType = "Text";
                dialog.DefaultValue = target.Value;

                StringFields.Remove(target);
            }

            if (LongFields.Contains(targetBase))
            {
                DetailDataField<long> target = targetBase as DetailDataField<long>;

                dialog.ValueType = "Integer";
                dialog.DefaultValue = target.Value.ToString();

                LongFields.Remove(target);
            }

            if (DecimalFields.Contains(targetBase))
            {
                DetailDataField<decimal> target = targetBase as DetailDataField<decimal>;

                dialog.ValueType = "Decimal";
                dialog.DefaultValue = target.Value.ToString();

                DecimalFields.Remove(target);
            }

            if (BoolFields.Contains(targetBase))
            {
                DetailDataField<bool> target = targetBase as DetailDataField<bool>;

                dialog.ValueType = "Boolean";
                dialog.DefaultValue = target.Value.ToString();

                BoolFields.Remove(target);
            }

            if (DateFields.Contains(targetBase))
            {
                DetailDataField<DateTime> target = targetBase as DetailDataField<DateTime>;

                dialog.ValueType = "Date and Time";
                dialog.DefaultValue = target.Value.ToString();

                DateFields.Remove(target);
            }

            if (dialog.ShowDialog() != true) return;

            targetBase.Tags = dialog.Tags;

            if (dialog.ValueType == "Text") StringFields.Add(new DetailDataField<string>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, dialog.DefaultValue));
            if (dialog.ValueType == "Integer") LongFields.Add(new DetailDataField<long>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, long.Parse(dialog.DefaultValue)));
            if (dialog.ValueType == "Decimal") DecimalFields.Add(new DetailDataField<decimal>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, decimal.Parse(dialog.DefaultValue)));
            if (dialog.ValueType == "Boolean") BoolFields.Add(new DetailDataField<bool>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, bool.Parse(dialog.DefaultValue)));
            if (dialog.ValueType == "Date and Time") DateFields.Add(new DetailDataField<DateTime>(dialog.FieldName, dialog.Tags, dialog.DisplayIndex, DateTime.Parse(dialog.DefaultValue)));

            lFields.ItemsSource = DetailDataBaseFields;
        }

        private void EditDetailDataField_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lFields != null && ((ListBox)lFields).SelectedItem != null;

            e.Handled = true;
        }

        private void DeleteDetailDataField_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DetailDataBaseField targetBase = (DetailDataBaseField)((ListBox)lFields).SelectedItem;

            if (StringFields.Contains(targetBase)) StringFields.Remove(targetBase as DetailDataField<string>);
            if (LongFields.Contains(targetBase)) LongFields.Remove(targetBase as DetailDataField<long>);
            if (DecimalFields.Contains(targetBase)) DecimalFields.Remove(targetBase as DetailDataField<decimal>);
            if (BoolFields.Contains(targetBase)) BoolFields.Remove(targetBase as DetailDataField<bool>);
            if (DateFields.Contains(targetBase)) DateFields.Remove(targetBase as DetailDataField<DateTime>);

            lFields.ItemsSource = DetailDataBaseFields;
        }

        private void DeleteDetailDataField_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lFields != null && ((ListBox)lFields).SelectedItem != null;

            e.Handled = true;
        }
    }
}
