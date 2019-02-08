using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Suggestions;

namespace RapidXamlToolkit.Tagging
{
    public class XamlDocument
    {
        private DataEntry[] attachedDatas;
        private int count;

        public XamlDocument()
        {
            this.Span = default(Span);
            this.SuggestionTags = new List<IRapidXamlTag>();
        }

        public Span Span { get; set; }

        public List<IRapidXamlTag> SuggestionTags { get; set; }

        public void SetData(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.attachedDatas == null)
            {
                this.attachedDatas = new DataEntry[1];
            }
            else
            {
                for (int index = 0; index < this.count; ++index)
                {
                    if (this.attachedDatas[index].Key == key)
                    {
                        this.attachedDatas[index].Value = value;
                        return;
                    }
                }

                if (this.count == this.attachedDatas.Length)
                {
                    DataEntry[] dataEntryArray = new DataEntry[this.attachedDatas.Length + 1];
                    Array.Copy((Array)this.attachedDatas, 0, (Array)dataEntryArray, 0, this.count);
                    this.attachedDatas = dataEntryArray;
                }
            }

            this.attachedDatas[this.count] = new DataEntry(key, value);
            ++this.count;
        }

        /// <summary>
        /// Determines whether this instance contains the specified key data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if a data with the key is stored</returns>
        /// <exception cref="T:System.ArgumentNullException">if key is null</exception>
        public bool ContainsData(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.attachedDatas == null)
            {
                return false;
            }

            for (int index = 0; index < this.count; ++index)
            {
                if (this.attachedDatas[index].Key == key)
                {
                    return true;
                }
            }

            return false;
        }

        public object GetData(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this.attachedDatas == null)
            {
                return (object)null;
            }

            for (int index = 0; index < this.count; ++index)
            {
                if (this.attachedDatas[index].Key == key)
                {
                    return this.attachedDatas[index].Value;
                }
            }

            return (object)null;
        }
    }
}
