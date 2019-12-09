// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using RapidXamlToolkit.Resources;

namespace RapidXamlToolkit.Options
{
    public class ProfileSummary : CanNotifyPropertyChanged
    {
        private bool isActive;

        private bool isFallBack;

        public int Index { get; set; }

        public string Name { get; set; }

        public string ProjectType { get; set; }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                this.isActive = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.DisplayName));
            }
        }

        public bool IsFallBack
        {
            get
            {
                return this.isFallBack;
            }

            set
            {
                this.isFallBack = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.DisplayName));
            }
        }

        public string DisplayName
        {
            get
            {
                if (this.IsActive)
                {
                    if (this.IsFallBack)
                    {
                        return StringRes.UI_FallBackActiveProfileName.WithParams(this.Name);
                    }
                    else
                    {
                        return StringRes.UI_ActiveProfileName.WithParams(this.Name);
                    }
                }
                else
                {
                    if (this.IsFallBack)
                    {
                        return StringRes.UI_FallBackProfileName.WithParams(this.Name);
                    }
                    else
                    {
                        return this.Name;
                    }
                }
            }
        }
    }
}
