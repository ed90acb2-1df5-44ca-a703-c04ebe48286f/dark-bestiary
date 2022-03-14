using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary
{
    public struct I18NString
    {
        public List<I18NStringVariable> Variables { get; }

        public string Key => this.data.Key;

        private readonly I18NStringData data;

        public I18NString(I18NStringData data)
        {
            this.data = data;

            Variables = data.Variables
                .Select(variable => new I18NStringVariable(variable))
                .ToList();
        }

        public static implicit operator string(I18NString i18NString)
        {
            return i18NString.ToString();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.data.Key))
            {
                return "";
            }

            return I18N.Instance.Translate(this.data.Key);
        }

        public string ToString(GameObject entity)
        {
            return ToString(new StringVariableContext(entity));
        }

        public string ToString(StringVariableContext context)
        {
            return Variables.Count > 0
                ? ToString(Variables.Select(variable => variable.GetValue(context)))
                : ToString();
        }

        public string ToString(object context)
        {
            return string.Format(ToString(), new[] {context});
        }

        public string ToString(IEnumerable<object> context)
        {
            var format = ToString();
            return string.IsNullOrEmpty(format) ? "" : string.Format(format, context.ToArray());
        }

        public bool IsNullOrEmpty()
        {
            return this.data == null || string.IsNullOrEmpty(ToString());
        }

        public bool LikeIgnoreCase(string text)
        {
            return StringExtensions.LikeIgnoreCase(this, text);
        }

        public bool Like(string text)
        {
            return StringExtensions.Like(this, text);
        }
    }
}