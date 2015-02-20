using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkusRezai.Project.UltraGraph.ViewModel
{
    public static class TagPickerCommands
    {
        public static readonly RoutedUICommand AddTag = new RoutedUICommand
           (
               "Add Tag",
               "AddTag",
               typeof(Commands),
               new InputGestureCollection()
               {
               }
           );

        public static readonly RoutedUICommand EditTag = new RoutedUICommand
           (
               "Edit Tag",
               "EditTag",
               typeof(Commands),
               new InputGestureCollection()
               {
               }
           );

        public static readonly RoutedUICommand DeleteTag = new RoutedUICommand
          (
              "Delete Tag",
              "DeleteTag",
              typeof(Commands),
              new InputGestureCollection()
              {
              }
          );

    }
}
