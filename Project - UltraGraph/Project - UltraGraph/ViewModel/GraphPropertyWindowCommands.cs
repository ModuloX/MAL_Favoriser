using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkusRezai.Project.UltraGraph.ViewModel
{
    public static class GraphPropertyWindowCommands
    {
        public static readonly RoutedUICommand AddVertexViewData = new RoutedUICommand
            (
                "Add ViewData",
                "AddViewData",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );

        public static readonly RoutedUICommand EditVertexViewData = new RoutedUICommand
            (
                "Edit ViewData",
                "EditViewData",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );

        public static readonly RoutedUICommand DeleteVertexViewData = new RoutedUICommand
            (
                "Delete ViewData",
                "DeleteViewData",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );

        public static readonly RoutedUICommand AddEdgeViewData = new RoutedUICommand
            (
                "Add ViewData",
                "AddViewData",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );

        public static readonly RoutedUICommand EditEdgeViewData = new RoutedUICommand
            (
                "Edit ViewData",
                "EditViewData",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );

        public static readonly RoutedUICommand DeleteEdgeViewData = new RoutedUICommand
            (
                "Delete ViewData",
                "DeleteViewData",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );

        public static readonly RoutedUICommand AddDetailDataTemplate = new RoutedUICommand
            (
                "Add DetailDataTemplate",
                "AddDetailDataTemplate",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );

        public static readonly RoutedUICommand EditDetailDataTemplate = new RoutedUICommand
            (
                "Edit DetailDataTemplate",
                "EditDetailDataTemplate",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );

        public static readonly RoutedUICommand DeleteDetailDataTemplate = new RoutedUICommand
            (
                "Delete DetailDataTemplate",
                "DeleteDetailDataTemplate",
                typeof(Commands),
                new InputGestureCollection()
                {
                }
            );
    }
}
