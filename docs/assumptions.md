# Assumptions

The XAML generation functionality of the toolkit is based on the following assumptions.

- The code in the project compiles.
- Each ViewModel class is in a separate file.
- Wiring up the ViewModel to the View is handled by the developer, not functionality in the toolkit.
- **C#** code always includes opening and closing curly braces and they are on their own lines.

Where code does not meet these expectations the result of using any  functionality cannot be guaranteed.
