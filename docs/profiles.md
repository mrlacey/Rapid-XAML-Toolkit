# Profiles

A profile is a set of rules and settings that apply to the way the tool works.

## Mappings

A mapping is the specific rule for the generation of XAML for a specific property. A profile includes a configurable list of mappings for different combinations of type, name, and accessibility.

### Special mappings

In addition to the mappings for properties, there are also two special mappings that must be configured.

- Fallback
- Sub-Property

#### Fallback mapping

The mapping used when a property does not match any other mapping.

#### Sub-Property mapping

The mapping used for properties included from the **$subprops$** placeholder.
Only **$name$**, **$incint$**, and **$repint$** placeholders are valid in the sub-property output.

## Placeholders

Profile settings and mappings can include placeholders. A placeholder is something that will be replaced in the generated code. The following placeholders are defined.

### File Generation placeholders

- **$viewproject$** The name of the project that the view will be created in.
- **$viewns$** The namespace the view will be created in. This is based on project and file structure.
- **$viewmodelns$** The namespace the view model will be created in. This is based on project and file structure.
- **$viewclass$** The name of the view class that will be created.
- **$viewmodelclass$** The name of the view model class that will be created.
- **$genxaml$** Where the generated XAML will be included in the created file.

### XAML generation placeholders

- **$name$** Property name.
- **$type$** Property type.
- **$incint$** Incrementing integer. A number (starting at zero) that will increase with each property that is matched in a class.
- **$repint$** Repeating integer. The same number that was last used, repeated without increment. Useful when you want output with multiple items in the same row.
- **$subprops$** Sub-properties. Is replaced with output from the sub-property mapping for each property of the matched type. Useful when outputting collections of items.
- **$nooutput$** No Output. Nothing will be included in the generated XAML when this is in the mapping output.
