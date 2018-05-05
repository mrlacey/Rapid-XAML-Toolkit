// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RapidXamlToolkit
{
    public class Settings : CanNotifyPropertyChanged
    {
        public string ActiveProfileName { get; set; }

        public List<Profile> Profiles { get; set; }

        public bool ExtendedOutputEnabled { get; set; }

        public ObservableCollection<ProfileSummary> ProfilesList
        {
            get
            {
                var list = new ObservableCollection<ProfileSummary>();

                // If multile profiles have the same name as the active profile, use the first one in the list with matcing name
                bool activeIndicated = false;

                for (var index = 0; index < this.Profiles.Count; index++)
                {
                    var profile = this.Profiles[index];

                    var summary = new ProfileSummary
                    {
                        Index = index,
                        Name = profile.Name,
                        IsActive = !activeIndicated && profile.Name == this.ActiveProfileName,
                    };

                    if (summary.IsActive)
                    {
                        activeIndicated = true;
                    }

                    list.Add(summary);
                }

                return list;
            }
        }

        public bool IsActiveProfileSet
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.ActiveProfileName);
            }
        }

        public Profile GetActiveProfile()
        {
            Profile result = null;

            if (!string.IsNullOrEmpty(this.ActiveProfileName))
            {
                result = this.Profiles.FirstOrDefault(p => p.Name == this.ActiveProfileName);
            }

            if (result == null)
            {
                result = this.Profiles.FirstOrDefault();
            }

            return result;
        }

        public List<string> GetAllProfileNames()
        {
            return this.Profiles.Select(p => p.Name).ToList();
        }

        public void RefreshProfilesList()
        {
            this.OnPropertyChanged(nameof(this.ProfilesList));
        }
    }
}
