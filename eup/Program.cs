using System;
using System.Windows.Forms;
using System.Configuration; // Make sure to include this
// using your namespaces...
using eup;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Pass the configuration (using ConfigurationManager) to Form1
        Application.Run(new Form1(ConfigurationManager.ConnectionStrings["BlogDb"].ConnectionString));
    }
}
