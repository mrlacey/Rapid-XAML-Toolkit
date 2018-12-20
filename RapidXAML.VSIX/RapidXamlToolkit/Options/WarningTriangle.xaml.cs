// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Windows.Controls;

namespace RapidXamlToolkit.Options
{
    public partial class WarningTriangle : UserControl
    {
        public WarningTriangle()
        {
            this.InitializeComponent();

            // Loading embedded image resource in this way as the simplest way of doing this I could find.
            var bmp = RapidXamlToolkit.Resources.ImageResources.WarningIcon;

            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                this.WarningIconImage.Source = bitmapImage;
            }
        }
    }
}
