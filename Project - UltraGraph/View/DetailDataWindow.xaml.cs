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
    /// Interaction logic for DetailDataWindow.xaml
    /// </summary>
    public partial class DetailDataWindow : MetroWindow
    {
        private List<DetailDataBaseField> fields;
        /// <summary>
        /// Get - gets the values of all fields and puts it into a new <see cref="DetailData"/>.<para/>
        /// Set - adds all fields to the <see cref="DetailDataWindow.FieldGrid"/> and sets their values. The field objects are attached as tag. Clears all children of the <see cref="DetailDataWindow.FieldGrid"/> beforehand.
        /// </summary>
        public DetailData DetailData
        {
            get
            {
                if (fields == null) return null;

                DetailData result = new DetailData();

                foreach (DetailDataBaseField field in fields)
                {
                    if (field as DetailDataField<string> != null)
                    {
                        DetailDataField<string> textField = field as DetailDataField<string>;
                        textField.Value = FieldGrid.Children.OfType<TextBox>().Single(x => x.Tag == textField).Text;
                        result.StringFields.Add(textField);
                    }

                    else if (field as DetailDataField<long> != null)
                    {
                        DetailDataField<long> longField = field as DetailDataField<long>;
                        longField.Value = long.Parse(FieldGrid.Children.OfType<TextBox>().Single(x => x.Tag == longField).Text);
                        result.LongFields.Add(longField);
                    }

                    else if (field as DetailDataField<decimal> != null)
                    {
                        DetailDataField<decimal> decimalField = field as DetailDataField<decimal>;
                        decimalField.Value = decimal.Parse(FieldGrid.Children.OfType<TextBox>().Single(x => x.Tag == decimalField).Text);
                        result.DecimalFields.Add(decimalField);
                    }

                    else if (field as DetailDataField<bool> != null)
                    {
                        DetailDataField<bool> boolField = field as DetailDataField<bool>;
                        boolField.Value = FieldGrid.Children.OfType<CheckBox>().Single(x => x.Tag == boolField).IsChecked.Value;
                        result.BoolFields.Add(boolField);
                    }

                    else if (field as DetailDataField<DateTime> != null)
                    {
                        DetailDataField<DateTime> dateField = field as DetailDataField<DateTime>;
                        dateField.Value = DateTime.Parse(FieldGrid.Children.OfType<TextBox>().Single(x => x.Tag == dateField).Text);
                        result.DateFields.Add(dateField);
                    }
                }

                return result;
            }
            set
            {
                fields = new List<DetailDataBaseField>();

                fields.AddRange(value.StringFields);
                fields.AddRange(value.LongFields);
                fields.AddRange(value.DecimalFields);
                fields.AddRange(value.BoolFields);
                fields.AddRange(value.DateFields);

                fields = fields.OrderBy(x => x.DisplayIndex).ToList();

                FieldGrid.RowDefinitions.Clear();
                FieldGrid.Children.Clear();

                foreach (DetailDataBaseField field in fields)
                {
                    FieldGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                    int row = fields.IndexOf(field);

                    // Add a textblock with the fieldname and the field object attached as tag
                    TextBlock fieldName = new TextBlock() { Text = field.Name + ":", VerticalAlignment = System.Windows.VerticalAlignment.Center, Margin = new Thickness(0, 2, 5, 2) };
                    Grid.SetColumn(fieldName, 0);
                    Grid.SetRow(fieldName, row);
                    FieldGrid.Children.Add(fieldName);

                    // Add a Tags button
                    Button editTagsButton = new Button() { Content = "Tags", Margin = new Thickness(0, 2, 5, 2) };
                    editTagsButton.SetResourceReference(Control.StyleProperty, "SquareButtonStyle");
                    editTagsButton.Click += EditTags_Executed;
                    editTagsButton.Tag = field;
                    Grid.SetColumn(editTagsButton, 2);
                    Grid.SetRow(editTagsButton, row);
                    FieldGrid.Children.Add(editTagsButton);

                    // Add appropriate controls to grid for field value
                    if (field as DetailDataField<string> != null)
                    {
                        DetailDataField<string> textField = field as DetailDataField<string>;
                        TextBox tb = new TextBox() { Text = textField.Value, Tag = textField, Margin = new Thickness(0, 2, 5, 2) };
                        Grid.SetColumn(tb, 1);
                        Grid.SetRow(tb, row);
                        FieldGrid.Children.Add(tb);
                    }

                    else if (field as DetailDataField<long> != null)
                    {
                        DetailDataField<long> longField = field as DetailDataField<long>;
                        TextBox tb = new TextBox() { Text = longField.Value.ToString(), Tag = longField, Margin = new Thickness(0, 2, 5, 2) };
                        Grid.SetColumn(tb, 1);
                        Grid.SetRow(tb, row);
                        FieldGrid.Children.Add(tb);
                    }

                    else if (field as DetailDataField<decimal> != null)
                    {
                        DetailDataField<decimal> decimalField = field as DetailDataField<decimal>;
                        TextBox tb = new TextBox() { Text = decimalField.Value.ToString(), Tag = decimalField, Margin = new Thickness(0, 2, 5, 2) };
                        Grid.SetColumn(tb, 1);
                        Grid.SetRow(tb, row);
                        FieldGrid.Children.Add(tb);
                    }

                    else if (field as DetailDataField<bool> != null)
                    {
                        DetailDataField<bool> boolField = field as DetailDataField<bool>;
                        CheckBox cb = new CheckBox() { IsChecked = boolField.Value, Tag = boolField, Margin = new Thickness(0, 2, 5, 2) };
                        Grid.SetColumn(cb, 1);
                        Grid.SetRow(cb, row);
                        FieldGrid.Children.Add(cb);
                    }

                    else if (field as DetailDataField<DateTime> != null)
                    {
                        DetailDataField<DateTime> dateField = field as DetailDataField<DateTime>;
                        TextBox tb = new TextBox() { Text = dateField.Value.ToString(), Tag = dateField, Margin = new Thickness(0, 2, 5, 2) };
                        Grid.SetColumn(tb, 1);
                        Grid.SetRow(tb, row);
                        FieldGrid.Children.Add(tb);
                    }
                }
            }
        }

        public DetailDataWindow()
        {
            InitializeComponent();

            fields = new List<DetailDataBaseField>();
        }

        private void OkayButtonClick_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
            Close();

            e.Handled = true;
        }

        private void OkayButtonClick_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Check all field values on consistency with their type
            e.CanExecute = fields != null && FieldGrid != null;

            if (!e.CanExecute) return;

            long ignoreMeLong;
            decimal ignoreMeDecimal;
            DateTime ignoreMeDateTime;

            foreach (DetailDataBaseField field in fields)
            {
                if (!e.CanExecute) return;

                if (field as DetailDataField<long> != null) e.CanExecute = e.CanExecute && long.TryParse(FieldGrid.Children.OfType<TextBox>().Single(x => x.Tag == (DetailDataField<long>)field).Text, out ignoreMeLong);
                else if (field as DetailDataField<decimal> != null) e.CanExecute = e.CanExecute && decimal.TryParse(FieldGrid.Children.OfType<TextBox>().Single(x => x.Tag == (DetailDataField<decimal>)field).Text, out ignoreMeDecimal);
                else if (field as DetailDataField<bool> != null) e.CanExecute = e.CanExecute && FieldGrid.Children.OfType<CheckBox>().Single(x => x.Tag == (DetailDataField<bool>)field).IsChecked.HasValue;
                else if (field as DetailDataField<DateTime> != null) e.CanExecute = e.CanExecute && DateTime.TryParse(FieldGrid.Children.OfType<TextBox>().Single(x => x.Tag == (DetailDataField<DateTime>)field).Text, out ignoreMeDateTime);
            }

            e.Handled = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            e.Handled = true;
        }

        private void EditDetailDataTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DetailDataTemplatePropertiesWindow dialog = new DetailDataTemplatePropertiesWindow();
            dialog.Title = "Detail Data Fields";
            dialog.MainGrid.Children.Remove(dialog.tName);
            dialog.MainGrid.Children.Remove(dialog.tbName);
            dialog.StringFields = new List<DetailDataField<string>>(DetailData.StringFields);
            dialog.LongFields = new List<DetailDataField<long>>(DetailData.LongFields);
            dialog.DecimalFields = new List<DetailDataField<decimal>>(DetailData.DecimalFields);
            dialog.BoolFields = new List<DetailDataField<bool>>(DetailData.BoolFields);
            dialog.DateFields = new List<DetailDataField<DateTime>>(DetailData.DateFields);

            dialog.lFields.ItemsSource = dialog.DetailDataBaseFields;

            if (dialog.ShowDialog() != true) return;

            DetailData = new DetailData()
            {
                StringFields = dialog.StringFields,
                LongFields = dialog.LongFields,
                DecimalFields = dialog.DecimalFields,
                BoolFields = dialog.BoolFields,
                DateFields = dialog.DateFields
            };

            e.Handled = true;
        }

        private void EditDetailDataTemplate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            OkayButtonClick_CanExecute(sender, e);

            e.Handled = true;
        }

        private void EditTags_Executed(object sender, RoutedEventArgs e)
        {
            DetailDataBaseField field = (DetailDataBaseField)((Button)sender).Tag;

            TagsWindow dialog = new TagsWindow();
            dialog.Tags = new List<Tag>(field.Tags);

            if (dialog.ShowDialog() != true) return;

            field.Tags = dialog.Tags;

            e.Handled = true;
        }
    }
}
