# Profiles

A profile is a set of rules and settings that apply to XAML Generation.

## Mappings

Mappings are the encapsulation of the logic for turning the properties in C# or VB code into XAML.
Each profile will contain multiple mappings.
Each mapping is the specific rule for the generation of XAML for a specific property. A profile includes a configurable list of mappings for different combinations of type, name, and accessibility.

A mapping can match multiple property types. To specify multiple types, separate them with the pipe (|) character. To match any type specify `T`.

A mapping can be made to only apply to properties that are read-only. This is useful if you want different output for properties that can be edited.

A mapping can be made to apply to properties based on the property name. If name filters are provided, mapping will only be used if the name of the property contains the value specified. To specify multiple types, separate them with the pipe (|) character. Note that this check is case insensitive.

Mappings are matched in the following order:

1. Property Type.
2. If the property is Read-Only.
3. Property Name.

If a property matches multiple mappings, there is no guarantee on which will be used.

### Special mappings

In addition to the mappings for properties, there are also three special mappings that must be configured.

- Fallback
- Sub-Property
- Enum Members

#### Fallback mapping

The mapping used when a property does not match any other mapping.

#### Sub-Property mapping

The mapping used for properties included from the **$subprops$** placeholder.
Only **$name$**, **$incint$**, and **$repint$** placeholders are valid in the sub-property output.

#### Enum Members mapping

The mapping used for properties included from the **$members$** placeholder.
Only the **$element$**, **$elementwithspace$** and **$enumname$** placeholders are valid in the enum member output.

## Placeholders

Profile settings and mappings can include placeholders. A placeholder is something that will be replaced in the generated code. The following placeholders are defined.

- **$name$** Property name.
- **$safename** Property name formatted for use as an `x:Name` within XAML.
- **$namewithspaces$** Property name with spaces inserted between words if the name is camelCase or PascalCase.
- **$type$** Property type. If a generic type this will be the inner type.
- **$incint$** Incrementing integer. A number (starting at zero) that will increase with each property that is matched in a class.
- **$repint$** Repeating integer. The same number that was last used, repeated without increment. Useful when you want output with multiple items in the same row.
- **$subprops$** Sub-properties. Is replaced with output from the sub-property mapping for each property of the matched type. Useful when outputting collections of items.
- **$members$** Enum members. Is replaced with output from the enum member mapping for each property of the matched type.
- **$element$** Enum element. Is replaced with the name of an individual enum element.
- **$elementwithspaces$** Enum element. Is replaced with the name of an individual enum element and spaces are inserted between words if the name is camelCase or PascalCase.
- **$enumname$** Enum property name. Is replaced with the name of the enum property.
- **$nooutput$** No Output. Nothing will be included in the generated XAML when this is in the mapping output.
- **$xname$** A generated value based on the property name and the XAML element this is used within.
- **$repxname$** Repeat the last generated $xname$ value. If no $xname$ value has been generated, the attribute this is used within will be omitted from the output.

### Attribute based placeholders

It is also possible to generate XAML based on the attributes attached to a property. This can be useful if the property name isn't what you want displayed but you have an attribute attached that does hold a preferred value.
You may also use attributes to store additional information related to a property that can be useful to have in the XAML as well. (e.g. max field length.)

Attribute based placeholders take the form `$att:<attribute-name>:<output-if-attribute-on-property>[::<fallback-value>]$`

#### &lt;attribute-name&gt;

This is the name of the attribute to look for. In the following code snippet this woudl be `Display`. (Use of `DisplayAttribute` also works.)

```csharp
    [Display(Name = ShortName)]
    public string UserName { get; set; }
```

#### &lt;output-if-attribute-on-property&gt;

These can be regular strings to treat as XAML. It can also include values in square brackets which have special meaning with regard the properties of the attribute.

- **[PropertName]** can be used to access the values of named items passed to the attribute constructor. e.g. `Name` in the above example.
- **[1]** can be used to access values passed to the attribute constructor in numeric order. Order starts with '1'. (`[1]` and `[Name]` produce the same output in the above example.)

#### &lt;fallback-value&gt;

This is the value to output if the attribute has not been applied to the property.
This is optional. If no fallback is provided and the attribute is not applied to the property nothing is added to the output.
This may contain above listed placeholders but with at-signs ('@') instead of dollar-signs ('\$') at the start and end of the placeholder.

##### Example

Consider the following: `$att:Display:[Name]::@namewithspaces@$`

- `$att:` indicates the start of an embedded attribute.
- `Display` name of the attribute.
- `:` indicates the end of the name.
- `[Name]` the content to display in the output
- `::` (optional) indicator of fallback if attribute isn't applied to the property
- `@namewithspaces@` the content to output instead. '@' signs are replaced with '\$' and then any embedded placeholders are also evaluated.
- `$` end of the embedded attribute placeholder.

If a mapping output is defined as:

```xml
<TextBlock Text="$att:Display:[Name]::@namewithspaces@$" />
```

these properties:

```csharp
[Display(Name = ShortName)]
public string UserName { get; set; }

public string FullName { get; set; }
```

will produce:

```xml
<TextBlock Text="Short Name" />
<TextBlock Text="Full Name" />
```
