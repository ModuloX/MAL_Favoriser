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
    /// Interaction logic for NewGraphWindow.xaml
    /// </summary>
    public partial class GraphPropertiesWindow : MetroWindow
    {
        public Size CanvasSize
        {
            get
            {
                return new Size(Math.Round(double.Parse(tbCanvasWidth.Text), 0), Math.Round(double.Parse(tbCanvasHeight.Text), 0));
            }

            set
            {
                tbCanvasWidth.Text = Math.Round(value.Width, 0).ToString();
                tbCanvasHeight.Text = Math.Round(value.Height, 0).ToString();
            }
        }
        public GraphPropertiesWindow()
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
            e.CanExecute = tbCanvasWidth != null && tbCanvasHeight != null && double.TryParse(tbCanvasWidth.Text, out ignoreMe) && double.TryParse(tbCanvasHeight.Text, out ignoreMe);

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

        private void AddEdgeViewData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EdgeViewData result = new EdgeViewData() { Name = "New Style" };

            EdgeViewDataWindow dialog = new EdgeViewDataWindow();
            dialog.ViewDataName = result.Name;

            dialog.StrokeBrushColorA = result.StrokeBrush.Color.A;
            dialog.StrokeBrushColorR = result.StrokeBrush.Color.R;
            dialog.StrokeBrushColorG = result.StrokeBrush.Color.G;
            dialog.StrokeBrushColorB = result.StrokeBrush.Color.B;

            dialog.StrokeThickness = result.StrokeThickness;
            dialog.ArrowAngle = result.ArrowAngle;
            dialog.ArrowLength = result.ArrowLength;
            dialog.IsArrowHeadClosed = result.IsArrowHeadClosed;

            if (dialog.ShowDialog() != true) return;

            string key = dialog.ViewDataName;

            result.Name = dialog.ViewDataName;
            result.StrokeBrush = new SolidColorBrush(Color.FromArgb(dialog.StrokeBrushColorA, dialog.StrokeBrushColorR, dialog.StrokeBrushColorG, dialog.StrokeBrushColorB));
            result.StrokeThickness = dialog.StrokeThickness;
            result.ArrowAngle = dialog.ArrowAngle;
            result.ArrowLength = dialog.ArrowLength;
            result.IsArrowHeadClosed = dialog.IsArrowHeadClosed;

            ViewDataManager.EdgeViewDataDictionary.Add(ViewDataManager.EdgeViewDataDictionary.Keys.Max() + 1, result);

            //Have to do that since there is no observable dictionary and I'm too lazy to write one, also Items.Refresh doesnt work for some reason...
            cEdgeViewData.ItemsSource = ViewDataManager.EdgeViewDataList;
        }

        private void AddEdgeViewData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void EditEdgeViewData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EdgeViewData result = (EdgeViewData)cEdgeViewData.SelectedItem;

            EdgeViewDataWindow dialog = new EdgeViewDataWindow();
            dialog.ViewDataName = result.Name;

            dialog.StrokeBrushColorA = result.StrokeBrush.Color.A;
            dialog.StrokeBrushColorR = result.StrokeBrush.Color.R;
            dialog.StrokeBrushColorG = result.StrokeBrush.Color.G;
            dialog.StrokeBrushColorB = result.StrokeBrush.Color.B;

            dialog.StrokeThickness = result.StrokeThickness;
            dialog.ArrowAngle = result.ArrowAngle;
            dialog.ArrowLength = result.ArrowLength;
            dialog.IsArrowHeadClosed = result.IsArrowHeadClosed;

            if (dialog.ShowDialog() != true) return;

            result.Name = dialog.ViewDataName;
            result.StrokeBrush = new SolidColorBrush(Color.FromArgb(dialog.StrokeBrushColorA, dialog.StrokeBrushColorR, dialog.StrokeBrushColorG, dialog.StrokeBrushColorB));
            result.StrokeThickness = dialog.StrokeThickness;
            result.ArrowAngle = dialog.ArrowAngle;
            result.ArrowLength = dialog.ArrowLength;
            result.IsArrowHeadClosed = dialog.IsArrowHeadClosed;

            //Have to do that since there is no observable dictionary and I'm too lazy to write one, also Items.Refresh doesnt work for some reason...
            cEdgeViewData.ItemsSource = ViewDataManager.EdgeViewDataList;
        }

        private void EditEdgeViewData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cEdgeViewData != null && cEdgeViewData.SelectedItem != null && cEdgeViewData.SelectedItem.GetType() == typeof(EdgeViewData);
        }

        private void DeleteEdgeViewData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("This will irrevertably delete the selected item. Do you want to proceed?", "Delete Edge View Data", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK &&
                ViewDataManager.EdgeViewDataDictionary.ContainsValue((EdgeViewData)cEdgeViewData.SelectedItem))
            {
                ViewDataManager.EdgeViewDataDictionary.Remove(ViewDataManager.EdgeViewDataDictionary.Single(x => x.Value == cEdgeViewData.SelectedItem).Key);
                cEdgeViewData.ItemsSource = ViewDataManager.EdgeViewDataList;
            }
        }

        private void DeleteEdgeViewData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cEdgeViewData != null && cEdgeViewData.SelectedItem != null && cEdgeViewData.SelectedItem.GetType() == typeof(EdgeViewData);
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
    }
}
