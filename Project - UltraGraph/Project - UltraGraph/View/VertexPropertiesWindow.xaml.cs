using MahApps.Metro.Controls;
using MarkusRezai.Project.UltraGraph.ViewModel;
using MarkusRezai.Project.UltraGraph.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for NewGraphWindow.xaml
    /// </summary>
    public partial class VertexPropertiesWindow : MetroWindow
    {
        public string VertexName { get { return tbName.Text; } set { tbName.Text = value; } }
        public double Radius { get { return Math.Round(double.Parse(tbRadius.Text), 0); } set { tbRadius.Text = Math.Round(value, 0).ToString(); } }
        public VertexViewData ViewData { get { return (VertexViewData)cVertexViewData.SelectedItem; } set { cVertexViewData.SelectedItem = value; } }
        public DetailData DetailData
        {
            get
            {
                if (cDetailDataTemplate.SelectedItem != null) return ((DetailDataTemplate)cDetailDataTemplate.SelectedItem).DetailData;
                else return null;
            }
        }

        public List<Tag> Tags { get { return (List<Tag>)lTags.ItemsSource; } set { lTags.ItemsSource = value; } }

        public VertexPropertiesWindow()
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
            double ignoreMe;

            e.CanExecute = tbRadius != null &&
                           cVertexViewData != null &&
                           double.TryParse(tbRadius.Text, out ignoreMe) &&
                           cVertexViewData.SelectedItem != null;

            e.Handled = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            e.Handled = true;
        }

        private void AddVertexViewData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            VertexViewData result = new VertexViewData() { Name = "New Style" };

            VertexViewDataWindow dialog = new VertexViewDataWindow();
            dialog.ViewDataName = result.Name;

            dialog.FillBrushColorA = result.FillBrush.Color.A;
            dialog.FillBrushColorR = result.FillBrush.Color.R;
            dialog.FillBrushColorG = result.FillBrush.Color.G;
            dialog.FillBrushColorB = result.FillBrush.Color.B;

            dialog.StrokeBrushColorA = result.StrokeBrush.Color.A;
            dialog.StrokeBrushColorR = result.StrokeBrush.Color.R;
            dialog.StrokeBrushColorG = result.StrokeBrush.Color.G;
            dialog.StrokeBrushColorB = result.StrokeBrush.Color.B;

            dialog.StrokeThickness = result.StrokeThickness;
            dialog.StrokeDashes = result.StrokeDashes;

            if (dialog.ShowDialog() != true) return;

            string key = dialog.ViewDataName;

            result.Name = dialog.ViewDataName;
            result.FillBrush = new SolidColorBrush(Color.FromArgb(dialog.FillBrushColorA, dialog.FillBrushColorR, dialog.FillBrushColorG, dialog.FillBrushColorB));
            result.StrokeBrush = new SolidColorBrush(Color.FromArgb(dialog.StrokeBrushColorA, dialog.StrokeBrushColorR, dialog.StrokeBrushColorG, dialog.StrokeBrushColorB));
            result.StrokeThickness = dialog.StrokeThickness;
            result.StrokeDashes = dialog.StrokeDashes;

            ViewDataManager.VertexViewDataDictionary.Add(ViewDataManager.VertexViewDataDictionary.Keys.Max() + 1, result);

            //Have to do that since there is no observable dictionary and I'm too lazy to write one, also Items.Refresh doesnt work for some reason...
            cVertexViewData.ItemsSource = ViewDataManager.VertexViewDataList;
        }

        private void AddVertexViewData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void EditVertexViewData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            VertexViewData result = (VertexViewData)cVertexViewData.SelectedItem;

            VertexViewDataWindow dialog = new VertexViewDataWindow();
            dialog.ViewDataName = result.Name;

            dialog.FillBrushColorA = result.FillBrush.Color.A;
            dialog.FillBrushColorR = result.FillBrush.Color.R;
            dialog.FillBrushColorG = result.FillBrush.Color.G;
            dialog.FillBrushColorB = result.FillBrush.Color.B;

            dialog.StrokeBrushColorA = result.StrokeBrush.Color.A;
            dialog.StrokeBrushColorR = result.StrokeBrush.Color.R;
            dialog.StrokeBrushColorG = result.StrokeBrush.Color.G;
            dialog.StrokeBrushColorB = result.StrokeBrush.Color.B;

            dialog.StrokeThickness = result.StrokeThickness;
            dialog.StrokeDashes = result.StrokeDashes;

            if (dialog.ShowDialog() != true) return;

            result.Name = dialog.ViewDataName;
            result.FillBrush = new SolidColorBrush(Color.FromArgb(dialog.FillBrushColorA, dialog.FillBrushColorR, dialog.FillBrushColorG, dialog.FillBrushColorB));
            result.StrokeBrush = new SolidColorBrush(Color.FromArgb(dialog.StrokeBrushColorA, dialog.StrokeBrushColorR, dialog.StrokeBrushColorG, dialog.StrokeBrushColorB));
            result.StrokeThickness = dialog.StrokeThickness;
            result.StrokeDashes = dialog.StrokeDashes;

            //Have to do that since there is no observable dictionary and I'm too lazy to write one, also Items.Refresh doesnt work for some reason...
            cVertexViewData.ItemsSource = ViewDataManager.VertexViewDataList;
        }

        private void EditVertexViewData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cVertexViewData != null && cVertexViewData.SelectedItem != null && cVertexViewData.SelectedItem.GetType() == typeof(VertexViewData);
        }

        private void DeleteVertexViewData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("This will irrevertably delete the selected item. Do you want to proceed?", "Delete Vertex View Data", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK &&
                ViewDataManager.VertexViewDataDictionary.ContainsValue((VertexViewData)cVertexViewData.SelectedItem))
            {
                ViewDataManager.VertexViewDataDictionary.Remove(ViewDataManager.VertexViewDataDictionary.Single(x => x.Value == cVertexViewData.SelectedItem).Key);
                cVertexViewData.ItemsSource = ViewDataManager.VertexViewDataList;
            }
        }

        private void DeleteVertexViewData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cVertexViewData != null && cVertexViewData.SelectedItem != null && cVertexViewData.SelectedItem.GetType() == typeof(VertexViewData);
        }

        private void AddDetailDataTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DetailDataTemplate result = new DetailDataTemplate();

            DetailDataTemplatePropertiesWindow dialog = new DetailDataTemplatePropertiesWindow();
            dialog.TemplateName = result.Name;
            dialog.StringFields = new List<DetailDataField<string>>(result.DetailData.StringFields);
            dialog.LongFields = new List<DetailDataField<long>>(result.DetailData.LongFields);
            dialog.DecimalFields = new List<DetailDataField<decimal>>(result.DetailData.DecimalFields);
            dialog.BoolFields = new List<DetailDataField<bool>>(result.DetailData.BoolFields);
            dialog.DateFields = new List<DetailDataField<DateTime>>(result.DetailData.DateFields);

            if (dialog.ShowDialog() != true) return;

            result.Name = dialog.TemplateName;
            result.DetailData.StringFields = dialog.StringFields;
            result.DetailData.LongFields = dialog.LongFields;
            result.DetailData.DecimalFields = dialog.DecimalFields;
            result.DetailData.BoolFields = dialog.BoolFields;
            result.DetailData.DateFields = dialog.DateFields;

            int id = 0;
            if (DetailDataTemplateManager.DetailDataTemplateDictionary.Count > 0) id = DetailDataTemplateManager.DetailDataTemplateDictionary.Keys.Max() + 1;
            DetailDataTemplateManager.DetailDataTemplateDictionary.Add(id, result);

            cDetailDataTemplate.ItemsSource = DetailDataTemplateManager.DetailDataTemplateList;
            cDetailDataTemplate.SelectedItem = result;

            e.Handled = true;
        }

        private void AddDetailDataTemplate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void EditDetailDataTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DetailDataTemplate target = (DetailDataTemplate)cDetailDataTemplate.SelectedItem;

            DetailDataTemplatePropertiesWindow dialog = new DetailDataTemplatePropertiesWindow();
            dialog.TemplateName = target.Name;
            dialog.StringFields = new List<DetailDataField<string>>(target.DetailData.StringFields);
            dialog.LongFields = new List<DetailDataField<long>>(target.DetailData.LongFields);
            dialog.DecimalFields = new List<DetailDataField<decimal>>(target.DetailData.DecimalFields);
            dialog.BoolFields = new List<DetailDataField<bool>>(target.DetailData.BoolFields);
            dialog.DateFields = new List<DetailDataField<DateTime>>(target.DetailData.DateFields);

            dialog.lFields.ItemsSource = dialog.DetailDataBaseFields;

            if (dialog.ShowDialog() != true) return;

            target.Name = dialog.TemplateName;
            target.DetailData.StringFields = dialog.StringFields;
            target.DetailData.LongFields = dialog.LongFields;
            target.DetailData.DecimalFields = dialog.DecimalFields;
            target.DetailData.BoolFields = dialog.BoolFields;
            target.DetailData.DateFields = dialog.DateFields;

            cDetailDataTemplate.ItemsSource = DetailDataTemplateManager.DetailDataTemplateList;

            e.Handled = true;
        }

        private void EditDetailDataTemplate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cDetailDataTemplate != null && cDetailDataTemplate.SelectedItem != null;

            e.Handled = true;
        }

        private void DeleteDetailDataTemplate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DetailDataTemplateManager.DetailDataTemplateDictionary.Remove(DetailDataTemplateManager.DetailDataTemplateDictionary.Single(x => x.Value == (DetailDataTemplate)cDetailDataTemplate.SelectedItem).Key);

            cDetailDataTemplate.ItemsSource = DetailDataTemplateManager.DetailDataTemplateList;

            e.Handled = true;
        }

        private void DeleteDetailDataTemplate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cDetailDataTemplate != null && cDetailDataTemplate.SelectedItem != null;

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
