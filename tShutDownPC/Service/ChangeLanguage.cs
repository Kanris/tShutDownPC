﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tShutDownPC.Service.Enums;

namespace tShutDownPC.Service
{
    public static class ChangeLanguage
    {
        public static void ChangeLanguageTo(LanguageSettings languageSettings)
        {
            switch (languageSettings)
            {
                case LanguageSettings.EN:
                    TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    break;

                case LanguageSettings.RU:
                    TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
                    break;
            }
        }
    }
}
