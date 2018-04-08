// <copyright file="ProfileSummary.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace RapidXamlToolkit
{
    public class ProfileSummary : CanNotifyPropertyChanged
    {
        private bool isActive;

        public int Index { get; set; }

        public string Name { get; set; }

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

        public string DisplayName
        {
            get
            {
                if (this.IsActive)
                {
                    return $"{this.Name}    [*ACTIVE*]";
                }
                else
                {
                    return this.Name;
                }
            }
        }
    }
}
