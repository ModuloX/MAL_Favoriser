using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkusRezai.Project.UltraGraph.ViewModel
{
    public static class Commands
    {
        public static readonly RoutedUICommand NewGraph = new RoutedUICommand
            (
                "New Graph",
                "NewGraph",
                typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.N, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand OpenGraph = new RoutedUICommand
        (
            "Open Graph",
            "OpenGraph",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand SaveGraph = new RoutedUICommand
        (
            "Save Graph",
            "SaveGraph",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand ShowHelp = new RoutedUICommand
        (
            "Show Help",
            "ShowHelp",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.H, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand ExitProgram = new RoutedUICommand
        (
            "Exit",
            "ExitProgram",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                }
        );

        public static readonly RoutedUICommand ShowGraphProperties = new RoutedUICommand
        (
            "Show Graph Properties",
            "ShowGraphProperties",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.D0, ModifierKeys.Control),
                    new KeyGesture(Key.NumPad0, ModifierKeys.Control),
                    new KeyGesture(Key.G, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand NewVertexToolMode = new RoutedUICommand
        (
            "New Vertex Tool Mode",
            "NewVertex",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.D1, ModifierKeys.Control),
                    new KeyGesture(Key.NumPad1, ModifierKeys.Control),
                    new KeyGesture(Key.V, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand ConnectToolMode = new RoutedUICommand
        (
            "Connect Tool Mode",
            "Connect",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.D2, ModifierKeys.Control),
                    new KeyGesture(Key.NumPad2, ModifierKeys.Control),
                    new KeyGesture(Key.C, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand ReconnectToolMode = new RoutedUICommand
        (
            "Reconnect Tool Mode",
            "Reconnect",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.D3, ModifierKeys.Control),
                    new KeyGesture(Key.NumPad3, ModifierKeys.Control),
                    new KeyGesture(Key.R, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand EditToolMode = new RoutedUICommand
        (
            "Edit Tool Mode",
            "EditToolMode",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.D4, ModifierKeys.Control),
                    new KeyGesture(Key.NumPad4, ModifierKeys.Control),
                    new KeyGesture(Key.E, ModifierKeys.Control)
                }
        );

        public static readonly RoutedUICommand DeleteToolMode = new RoutedUICommand
        (
            "Delete Tool Mode",
            "DeleteMode",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.Delete)
                }
        );

        public static readonly RoutedUICommand UnselectTool = new RoutedUICommand
        (
            "Unselect Tool",
            "UnselectTool",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.Escape)
                }
        );

        public static readonly RoutedUICommand OkayButtonClick = new RoutedUICommand
        (
            "Okay",
            "Okay",
            typeof(Commands),
            new InputGestureCollection()
                {
                    new KeyGesture(Key.Enter)
                }
        );

        public static readonly RoutedUICommand EditTags = new RoutedUICommand
        (
            "Edit Tags",
            "EditTags",
            typeof(Commands),
            new InputGestureCollection()
                {
                }
        );
    }
}
