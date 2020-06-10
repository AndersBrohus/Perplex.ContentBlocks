# Release Notes

Summary of changes in each release. For a full changelog see [the commit history](https://github.com/PerplexDigital/Perplex.ContentBlocks/commits/master).

## v1.4.1

-   NuGet package is split into two:
    -   `Perplex.ContentBlocks.Core`: Assembly only
    -   `Perplex.ContentBlocks`: Full package, except it only contains `App_Plugins` itself and depends on `Perplex.ContentBlocks.Core` for the assembly.
-   Minor bugfix: handling of dataTypeId in JavaScript aligned with C#.
    -   JavaScript would skip a zero as it's falsy whereas in C# we check for `null`. Logic in JS has been adjusted to behave the same as in C# now.

## v1.4.0

-   Layout descriptions are now shown in the layout picker
    -   Thanks to @jveer for the issue to get this started
-   Close all other block settings when opening settings on another block
-   Added `<content-blocks-icon>` component to help render icons
-   Fixes preview not working in Edge Legacy

## v1.3.0

-   Added option "Hide property group container".
    -   This is the default behavior of our editor but could never be turned off. This hides the containing property group (= "tab" in v7) of this editor. However, when adding other property editors to the same property group this no longer looks good. Turning this option off will render our editor inside the group like other editors.
-   Editor initialization is more smooth.
    -   We will not wait with all rendering anymore until data is loaded, some elements can always be rendered.
    -   Turning off "Hide property group container" will contribute a lot too as hiding the group during intialization is very noticeable on page load (it will appear then hide when this editor is loaded).
-   Cleaned up NuGet dependencies. Only `UmbracoCms.Web` remains.
-   Restored transition effect on closing/opening settings button.
-   Removes date from "Last update:", only showing time now.

## v1.2.1

-   Show server validation messages properly if property is culture variant.
-   Fixes "jumpy" layout sliders by not showing them until fully initialized.

## v1.2.0

-   Initial public release
