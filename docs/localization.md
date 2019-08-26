# Localization

The following is important information to know for anyone working with this codebase. It explains how to add new localized resources.

There are 5 places where localized resources are defined in the solution. (There are also localized versions of each of these files for all locales into which  Visual Studio is translated.)

- Resources/StringRes.resx (all strings)
- Resources/ImageResources.resx (all images used in the UI)
- VSPackage.en-US.resx (strings used in registering the packages with Visual Studio and visible in the Help>About dialog)
- RapidXamlPackage.en-US.vsct (defines commands and menus--including displayed text)
- xx-XX/Extension.vsixlangpack (localized versions of extension name and description)

`Resources/StringRes.resx` is the only place you are likely to need to add resources.

## Important points to note

- Avoid modifying existing string resources.
- Avoid reusing string resources in multiple locations.
- Please follow the existing naming conventions.

Make working with embedded string resources easier by installing the [String Resource Visualizer Extension](https://marketplace.visualstudio.com/items?itemName=MattLaceyLtd.StringResourceVisualizer).

![See the default resource translations inline](./Assets/string-res-viz.png)

## Adding new string resources used in code

1. Add new strings to the file `StringRes.resx` following the naming convention that already exists.
2. After adding the new entries, run LocalizationHelper.exe and select option 2.
3. Reference the newly created resource in code.

## LocalizationHelper.exe

This is a console app, in the Tools folder, that provides functionality to help working with localized files.

When started it lists 4 options.

1. Copy values from StringRes to other loc files
2. Copy from StringRes to StringRes.en-US
3. Extract new entries for localization
4. Merge translated files

### 1. Copy values from StringRes to other loc files

To simplify the initial process of localizing all the text content in the different files, StringRes.resx also contains a copy of the text from the other files. This option copies the translated strings from the localized versions of StringRes.resx to the other resx and vsct files.

### 2. Copy from StringRes to StringRes.en-US

This is the option you will use most. It copies `StringRes.resx` to `StringRes.en-US.resx`. The Visual Studio designer requires a neutral language file but the extension needs a locale specific version for the default locale (en-US).

### 3. Extract new entries for localization

Run this option when preparing to send new strings for localization. It will identify any strings in the default resource file that are not in the localized versions. It will then create resx files containing the strings that require translation. The output will include a list of these files.

```ps
The following files need translation:
- ../../../RapidXamlToolkit/Resources/StringRes.cs-CZ.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.de-DE.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.es-ES.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.fr-FR.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.it-IT.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.ja-JP.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.ko-KR.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.pl-PL.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.pt-BR.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.ru-RU.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.tr-TR.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.zh-CN.translation-needed.resx
- ../../../RapidXamlToolkit/Resources/StringRes.zh-TW.translation-needed.resx
Once translated, rename to 'xxx.translation-done.resx' and run option 4.
```

### 4. Merge translated files

Run this option once the new strings have been translated and the files renamed from `StringRes.xx-XX.translation-needed.resx` to `StringRes.xx-XX.translation-done.resx`. The new translations will then be merged into the main resource files.
