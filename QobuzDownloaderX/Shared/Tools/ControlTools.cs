using System.Reflection;
using System.Windows.Forms;

namespace QobuzDownloaderX.Shared.Tools
{
    internal static class ControlTools
    {
        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        public static void RemoveControls(Control userControl)
        {
            while (userControl.Controls.Count > 0)
            {
                Control control = userControl.Controls[0];
                if (control.HasChildren)
                    RemoveControls(control); // Recursively remove and dispose all children
                userControl.Controls.Remove(control);
                control.Dispose(); // Remove Control to clear from memory
            }
        }
    }
}