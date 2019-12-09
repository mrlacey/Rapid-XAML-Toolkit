// Copyright (c) Matt Lacey Ltd.. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RapidXamlToolkit.Options
{
    public class CanNotifyPropertyChangedAndDataErrorInfo : CanNotifyPropertyChanged, IDataErrorInfo
    {
        public string Error
        {
            get
            {
                return string.Join(Environment.NewLine, this.Errors);
            }
        }

        protected Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();

        public string this[string columnName]
        {
            get
            {
                if (this.Errors.ContainsKey(columnName))
                {
                    return this.Errors[columnName];
                }

                return string.Empty;
            }
        }
    }
}
