using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkusRezai.Project.UltraGraph.ViewModel
{
    public static class FieldPickerCommands
    {
        public static readonly RoutedUICommand AddField = new RoutedUICommand
           (
               "Add Field",
               "AddField",
               typeof(Commands),
               new InputGestureCollection()
               {
               }
           );

        public static readonly RoutedUICommand EditField = new RoutedUICommand
           (
               "Edit Field",
               "EditField",
               typeof(Commands),
               new InputGestureCollection()
               {
               }
           );

        public static readonly RoutedUICommand DeleteField = new RoutedUICommand
          (
              "Delete Field",
              "DeleteField",
              typeof(Commands),
              new InputGestureCollection()
              {
              }
          );

    }
}
