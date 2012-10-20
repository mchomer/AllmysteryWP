using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Shell;

namespace Allmystery.ViewModel.Extensions
{
    public static class ApplicationBarExtension
    {
            public static void Enable(this IApplicationBar appBar)
            {
                appBar.IsMenuEnabled = true;

                foreach (var obj in appBar.Buttons)
                {
                    var button = obj as ApplicationBarIconButton;
                    if (button != null)
                        button.IsEnabled = true;
                }
            }

            public static void Disable(this IApplicationBar appBar)
            {
                appBar.IsMenuEnabled = false;

                foreach (var obj in appBar.Buttons)
                {
                    var button = obj as ApplicationBarIconButton;
                    if (button != null)
                        button.IsEnabled = false;
                }
            }
    }
}
