// <copyright file="Settings.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RapidXamlToolkit
{
    public class Settings : CanNotifyPropertyChanged
    {
        public string ActiveProfileName { get; set; }

        public List<Profile> Profiles { get; set; }

        public ObservableCollection<ProfileSummary> ProfilesList
        {
            get
            {
                var list = new ObservableCollection<ProfileSummary>();

                for (var index = 0; index < this.Profiles.Count; index++)
                {
                    var profile = this.Profiles[index];

                    list.Add(new ProfileSummary
                    {
                        Index = index,
                        Name = profile.Name,
                        IsActive = profile.Name == this.ActiveProfileName,
                    });
                }

                return list;
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
