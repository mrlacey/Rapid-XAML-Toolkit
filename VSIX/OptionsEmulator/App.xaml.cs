// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace OptionsEmulator
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Any())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(e.Args[0]))
                    {
                        var culture = new CultureInfo(e.Args[0]);

                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;
                    }
                }
                catch (System.Exception exc)
                {
                    Debug.WriteLine(exc);
                }
            }
        }
    }
}
