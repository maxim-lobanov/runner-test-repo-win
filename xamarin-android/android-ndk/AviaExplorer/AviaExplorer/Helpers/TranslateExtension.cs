using AviaExplorer.Resources;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AviaExplorer.Helpers
{
    /// <summary>
    /// Extension, which helps with text translation
    /// </summary>
    [ContentProperty(nameof(Text))]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return null;

            return Text.Translate();
        }
    }

    public static class StringTranslationExtensions
    {
        static ResourceManager resourceManager;

        static StringTranslationExtensions()
        {
            var assembly = typeof(AppResources).GetTypeInfo().Assembly;
            var assemblyName = assembly.GetName();
            resourceManager = new ResourceManager($"{assemblyName.Name}.Resources.AppResources", assembly);
        }

        /// <summary>
        /// Translate the text automatically
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Translate(this string text)
        {
            if (text != null)
            {
                var current = CultureInfo.CurrentCulture;
                return resourceManager.GetString(text, current);
            }

            return "";
        }
    }
}