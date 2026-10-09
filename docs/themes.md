# Themes in LiquidVictor

This guide explains how a deck's theme reaches its generated presentation, how to select and create themes, and how to keep the result reproducible. It describes the implementation currently in this repository, not every feature available in upstream RevealJS.

## Contents

* [Shared concepts and deck metadata](#shared-concepts-and-deck-metadata)
* [RevealJS output](#revealjs-output)
  * [The generation pipeline](#the-generation-pipeline)
  * [Template structure and replacement tokens](#template-structure-and-replacement-tokens)
  * [Selecting an existing theme](#selecting-an-existing-theme)
  * [Creating a theme from existing CSS](#creating-a-theme-from-existing-css)
  * [Creating a theme with Sass](#creating-a-theme-with-sass)
  * [Styling generated layouts](#styling-generated-layouts)
  * [Backgrounds, fonts, code, and math](#backgrounds-fonts-code-and-math)
  * [Previewing, printing, and distributing](#previewing-printing-and-distributing)
  * [Troubleshooting RevealJS themes](#troubleshooting-revealjs-themes)
* [PowerPoint output](#powerpoint-output)
* [Other outputs](#other-outputs)
* [Implementation references and upstream documentation](#implementation-references-and-upstream-documentation)

## Shared concepts and deck metadata

A theme is a presentation's visual styling: colors, typography, spacing, and related decoration. LiquidVictor does not have a central theme registry, theme entity, or cross-format theme package. It stores a string, `SlideDeck.ThemeName`, and each output implementation decides how to use it.

Keep these settings distinct:

| Setting | Responsibility |
| --- | --- |
| `ThemeName` | Deck-level name. For RevealJS, selects a CSS file within the chosen template. |
| `TemplatePath` | Generator configuration, not deck metadata. For RevealJS, selects the whole directory containing the HTML shell, runtime, plugins, stylesheets, and supporting assets. |
| Slide `Layout` | Determines generated content structure, such as image/text columns or vertical slides. A theme styles that structure; it does not choose the layout. |
| `AspectRatio` | Deck-level presentation dimensions. For RevealJS, `Widescreen` produces 1920 x 1080 and `Standard` produces 1024 x 768; RevealJS scales that canvas to the viewport. |
| `BackgroundContent` | A deck or slide Content Item used as an image background by RevealJS. It is separate from the theme's default background color. |
| `Transition` / `BackgroundTransition` | Animation settings, separate from styling. Slides can override incoming and outgoing transitions. |

### Persisting a selection

In a YAML repository, edit the selected deck file under `SlideDecks`:

```yaml
ThemeName: "moon"
```

The YAML reader passes the value to the domain deck, and the writer saves it again. This does not copy or embed the stylesheet in the YAML repository.

For code that constructs a deck, set the same value explicitly:

```csharp
var deck = new LiquidVictor.Builders.SlideDeckBuilder()
    .Title("Theme demonstration")
    .SubTitle("Visual styling without changing slide content")
    .Presenter("Example Presenter")
    .ThemeName("acme-night")
    .AspectRatio(LiquidVictor.Enumerations.AspectRatio.Widescreen)
    .Build();
```

The parameterless domain deck starts with `Black`, but this is not a fallback for a blank value loaded from YAML or assigned later. Always specify a nonempty theme. A missing YAML `ThemeName` becomes an empty string.

The PostgreSQL model also has a `themename` column (maximum 50 characters). However, its current deck-reading implementation is not implemented; use the working YAML repository for the CLI examples below, rather than treating the schema as evidence of a complete PostgreSQL theme workflow.

## RevealJS output

### The generation pipeline

The bundled runtime identifies itself as **RevealJS 5.1.0** in [dist/reveal.js](../Templates/RevealJS/dist/reveal.js). Use the assets under [Templates/RevealJS](../Templates/RevealJS), not the historical generated presentation under [Sample/Output](../Sample/Output), as the starting point for new templates.

The end-to-end flow is:

1. The CLI loads a deck and its slides/content from the selected repository.
2. The RevealJS generator creates the optional title slide and renders each slide using its layout strategy. Text Content Items are converted to HTML through Markdig with advanced extensions.
3. The generator reads `index.html` from `TemplatePath`.
4. It replaces the supported tokens, including `{ThemeName}`, with deck metadata and rendered HTML.
5. It recursively copies the entire template directory into `PresentationPath`, overwriting matching files.
6. It writes referenced Content Item images into `img` and writes the generated `index.html` over the copied template file.
7. The browser loads the generated HTML, RevealJS, the selected theme CSS, plugins, and other assets.

**There is no CSS compilation or theme existence check in this pipeline.** A successful generation does not prove that the theme loads in the browser. `CompilePresentation` (used by `--SkipOutput`) builds the HTML but does not copy assets or test CSS, fonts, or browser rendering.

Use different template and output directories. Do not generate into the template directory itself: the generated HTML would replace the token-bearing source. Existing output files not present in the template are not removed, so a fresh output directory is the clearest way to detect missing assets.

### Template structure and replacement tokens

The important parts of the current template bundle are:

```text
Templates\RevealJS\
  index.html
  dist\
    reset.css
    reveal.css
    reveal.js
    theme\
      black.css
      moon.css
      bsstahl.css
      ...
      fonts\
  plugin\
    highlight\
      highlight.js
      monokai.css
      zenburn.css
    markdown\
    math\
    notes\
    search\
    zoom\
  css\
    theme\
      source\       Sass theme sources
      template\     Shared Sass settings, mixins, and theme rules
```

The HTML shell loads these stylesheets, in this order:

```html
<link rel="stylesheet" href="dist/reset.css">
<link rel="stylesheet" href="dist/reveal.css">
<link rel="stylesheet" href="dist/theme/{ThemeName}.css">
<link rel="stylesheet" href="plugin/highlight/monokai.css">
```

`dist/reveal.css` supplies the framework's layout and print rules. The theme supplies the visual styling. The highlight stylesheet supplies a separate code color scheme.

Only `index.html` is processed for these tokens; this is literal string replacement, not a general-purpose template language:

| Token | Replacement |
| --- | --- |
| `{SlideSections}` | Generated slide sections, including the optional title slide. |
| `{PresentationTitle}` | Deck title, used in the document title and left footer. |
| `{Presenter}` | Deck presenter, used in the right footer. |
| `{ThemeName}` | Lowercased deck theme name. |
| `{Transition}` | Deck transition's RevealJS base name. |
| `{BackgroundTransition}` | Deck background transition's RevealJS base name. |
| `{Width}` / `{Height}` | Numeric canvas dimensions derived from `AspectRatio`. |

Token names are case-sensitive. There are no tokens for subtitle, logo, footer visibility, or arbitrary theme options. The subtitle appears through the generated title-slide content. CSS and JavaScript files copied from the template are not token-expanded.

The current shell uses `hash: true`, the generated dimensions/transitions, and these plugins: `RevealHighlight`, `RevealMarkdown`, `RevealSearch`, `RevealNotes`, `RevealMath`, and `RevealZoom`. Normal Content Item Markdown is already rendered by Markdig; do not replace `{SlideSections}` with an upstream `data-markdown` example and expect the same LiquidVictor pipeline.

For a branded shell, copy the bundle to a separate directory, edit its `index.html`, and select that directory using `-TemplatePath`. Preserve the `.reveal > .slides` structure, `{SlideSections}`, the required runtime/plugin files, and the dimension/transition tokens unless you intentionally want to override those deck settings.

### Selecting an existing theme

For the default shell, `ThemeName: "Moon"` generates the URL `dist/theme/moon.css`. The implementation lowercases using the current culture, so use simple lowercase ASCII names such as `acme-night`, with no path separators and **no `.css` extension**. That also avoids filename-case surprises on case-sensitive web hosts.

The stylesheets currently supplied in `dist\theme` are:

* `beige`, `black`, `black-contrast`, `blood`, `dracula`
* `league`, `moon`, `night`, `serif`, `simple`, `sky`, `solarized`
* `white`, `white-contrast`, `white_contrast_compact_verbatim_headers`
* `bsstahl`, `carvana`, `confoo`

Discover the actual choices in your selected bundle rather than assuming every upstream theme is installed:

```powershell
Get-ChildItem .\Templates\RevealJS\dist\theme\*.css |
    Select-Object -ExpandProperty BaseName
```

`bsstahl` and `confoo` are customized derivatives of `moon`; `carvana` also contains custom layout/alignment styling. These three and `white_contrast_compact_verbatim_headers` have no corresponding source file in the bundled `css\theme\source` directory. They are maintained as CSS here. Several compiled stylesheets also contain footer additions not represented in the shared Sass template, so recompiling Sass is not guaranteed to reproduce the checked-in CSS exactly.

There is no `-ThemeName` CLI argument. Change the deck metadata, or assign `ThemeName` in code; `-TemplatePath` alone does not change the selected name.

### Creating a theme from existing CSS

This is the simplest path: retain a complete existing stylesheet and add overrides. No Node.js or Sass build is needed.

The following example works from the LiquidVictor repository root in PowerShell. It copies the YAML test repository so the original fixture is not edited, and copies the template so the bundled files are not changed. Use a new `ThemeDemo` directory; do not reuse an output directory as a template.

```powershell
New-Item -ItemType Directory -Path .\ThemeDemo
Copy-Item .\Templates\RevealJS -Destination .\ThemeDemo\Template -Recurse
Copy-Item .\tst\LiquidVictor.Data.YamlFile.Test\TestRepo `
    -Destination .\ThemeDemo\Repository -Recurse
Copy-Item .\ThemeDemo\Template\dist\theme\black.css `
    -Destination .\ThemeDemo\Template\dist\theme\acme-night.css
```

Keep the copied stylesheet's attribution/license comments and append the following rules to `ThemeDemo\Template\dist\theme\acme-night.css`. This example changes colors and heading capitalization, adds predictable footer styling, and supports the alignment classes emitted by `MultiSlide`:

```css
:root {
  --r-background-color: #101827;
  --r-main-color: #f3f4f6;
  --r-heading-color: #ffffff;
  --r-heading-text-transform: none;
  --r-link-color: #7dd3fc;
  --r-link-color-hover: #bae6fd;
  --r-link-color-dark: #38bdf8;
  --r-selection-background-color: #075985;
  --r-selection-color: #ffffff;
}

.reveal .slides section.content-left { text-align: left; }
.reveal .slides section.content-center { text-align: center; }
.reveal .slides section.content-right { text-align: right; }

.reveal .footer-left,
.reveal .footer-right {
  position: fixed;
  bottom: 10px;
  z-index: 10;
  font-family: var(--r-main-font);
  font-size: 14px;
  color: var(--r-main-color);
}

.reveal .footer-left { left: 20px; }
.reveal .footer-right { right: 20px; }

@media print {
  .reveal .footer-left,
  .reveal .footer-right {
    display: none;
  }
}
```

Because the copied CSS supplies the complete base theme, these overrides are sufficient. A file containing only custom properties is not a complete replacement theme unless it also imports a base stylesheet. An alternative is to load a separate override stylesheet after the base theme in your copied HTML shell, but that override then applies to every deck using that shell.

Edit the copied `ThemeDemo\Repository\SlideDecks\Test Deck 1.yaml` and change only:

```yaml
ThemeName: "acme-night"
```

Build with the .NET 10 SDK:

```powershell
dotnet run --project .\src\LV\LV.csproj -- Build `
    -SlideDeckId:64b458e1-be36-4f42-86b0-63f7a9ca1503 `
    -SourceRepoType:YamlFile `
    "-SourceRepoPath:.\ThemeDemo\Repository" `
    -OutputEngineType:RevealJS `
    "-TemplatePath:.\ThemeDemo\Template" `
    "-PresentationPath:.\ThemeDemo\Output"
```

The deck ID belongs to the copied test deck; for your own deck, replace it with the `Id` from its YAML file. Supply all three paths explicitly: historical CLI defaults are working-directory-sensitive, and the default repository type is not one of the currently supported registrations.

The generated `ThemeDemo\Output\index.html` should link to `dist/theme/acme-night.css`. The output directory should also contain the copied CSS, fonts, runtime, and plugins. Open the generated HTML in a browser, not the unprocessed template HTML.

To make this theme available in the standard bundle later, copy your final CSS and any supporting assets into the corresponding locations under `Templates\RevealJS`, and use that bundle as `TemplatePath`. No C# registration, enum addition, or rebuild of the generator is required merely to add a stylesheet.

### Creating a theme with Sass

Use Sass when maintaining variables and shared theme rules is preferable to editing compiled CSS. **LiquidVictor does not run Sass.** Compile before generating the deck, and include the resulting CSS in the selected template bundle.

The bundled source follows this order:

1. Import the shared mixins.
2. Import the default settings.
3. Override theme variables and any background/text-color mixins.
4. Import the shared theme template, which exposes `--r-*` properties and emits the common selectors.
5. Add LiquidVictor-specific selectors, such as footer and alignment rules.

For an existing-source starting point, copy `css\theme\source\black.scss` to a new filename in the same directory and edit its overrides. For a small system-font theme instead, create `ThemeDemo\Template\css\theme\source\acme-night.scss` with:

```scss
@use "sass:color";
@import "../template/mixins";
@import "../template/settings";

$backgroundColor: #101827;
$mainColor: #f3f4f6;
$headingColor: #ffffff;
$mainFont: Arial, Helvetica, sans-serif;
$headingFont: Arial, Helvetica, sans-serif;
$mainFontSize: 42px;
$headingTextTransform: none;
$headingFontWeight: 600;
$heading1Size: 2.5em;
$heading2Size: 1.6em;
$heading3Size: 1.3em;
$heading4Size: 1em;
$linkColor: #7dd3fc;
$linkColorHover: color.scale($linkColor, $lightness: 15%);
$selectionBackgroundColor: #075985;
$selectionColor: #ffffff;

@include light-bg-text-color(#222);
@import "../template/theme";
```

Append the footer/alignment selectors and print rule from the CSS example to this source after the final import if you want the same behavior. Otherwise, the shared template does not style the shell's LiquidVictor footer elements. Treat the Sass source as authoritative once you adopt this route; later compilation will overwrite hand-edited output CSS.

With Node.js/npm installed, compile using Dart Sass 1.x:

```powershell
npx --yes --package sass@1 sass --no-source-map `
    ".\ThemeDemo\Template\css\theme\source\acme-night.scss" `
    ".\ThemeDemo\Template\dist\theme\acme-night.css"
```

This command downloads/runs the Sass CLI if needed; it does not install a LiquidVictor dependency. The legacy `@import` syntax and some shared Sass functions may produce deprecation warnings. The command stays on Sass 1.x because the bundled sources have not been migrated to the newer module-only workflow. For a maintained build pipeline, pin and record the exact compiler version that you verify.

Useful settings include `$mainFont`, `$headingFont`, `$mainFontSize`, `$mainColor`, `$headingColor`, `$backgroundColor`, `$linkColor`, `$headingTextTransform`, `$heading1Size` through `$heading4Size`, and `$blockMargin`. See [settings.scss](../Templates/RevealJS/css/theme/template/settings.scss) for the complete set and [exposer.scss](../Templates/RevealJS/css/theme/template/exposer.scss) for their CSS custom-property names.

The [bundled theme README](../Templates/RevealJS/css/theme/README.md) also describes upstream's full development setup and `npm run build -- css-themes`. That command requires a full RevealJS checkout with its package manifest and Gulp tooling. The LiquidVictor template directory does **not** contain that build environment; do not run it there expecting automatic compilation. Use the direct Sass command above, or an explicitly maintained upstream development checkout, then copy the resulting theme and assets into your LiquidVictor template.

### Styling generated layouts

Inspect the generated HTML before choosing selectors. Layout names appear in HTML comments, not automatic CSS classes such as `.ImageRight` or `.FullPage`.

| Layout | Generated structure relevant to a theme |
| --- | --- |
| `Title` | Deck title heading, subtitle, presentation URL, presenter, and optional print link. The generated title slide inherits the deck background. |
| `FullPage` | Title block, followed by Markdown HTML and/or image elements. Eligible solo images can become backgrounds when `--MakeSoloImagesFullScreen` is enabled. |
| `FullPageFragments` | A full-width table of text Content Items with fragment markup. |
| `ImageLeft` | Table with an image cell followed by a text cell. |
| `ImageRight` | Table with a text cell followed by image cells. |
| `ImageWithCaption` | First image and first text caption; the caption is wrapped in an `h2`. |
| `MultiColumn` | Table with one top-aligned cell per Content Item. |
| `MultiSlide` | Nested sections (vertical slides), one per Content Item; the first item title uses `h1`, subsequent titles use `h3`. The parent slide title is ignored. |

`MultiSlide` uses Content Item `Alignment` to emit a class such as `content-left`, `content-center`, or `content-right`. This is not a global alignment mechanism for all layouts, and a stylesheet must define the relevant classes. Some image/text layouts also contain inline alignment styles.

Be careful with global table rules: they affect both Markdown data tables and layout tables. Similarly, changing `h1` changes ordinary slide titles as well as title-page headings. The default theme font sizes are canvas-relative and then scaled by RevealJS; test long titles, dense tables, and both aspect ratios instead of assuming a CSS pixel size is the final projected size.

The shell's `.footer-left` and `.footer-right` elements are siblings of `.slides`, not children of an individual slide. They are also outside the scaled slide content. Keep their font sizes and positioning deliberate.

Some bundled themes contain a selector resembling:

```css
.reveal .slides > section:first-child ~ .footer-left {
  display: none;
}
```

Despite its accompanying comment, this does **not** hide the current shell's footer on the title slide: the footer is not a sibling of that `section`. Copying that selector will not create slide-dependent visibility. For that behavior, add explicit state handling in your copied shell using RevealJS's `ready` and `slidechanged` events, accounting for decks built with `--NoTitle`. CSS based solely on the first section's existence cannot determine whether it is currently visible.

Also inspect custom themes for deck-specific rules before reusing them. For example, `bsstahl.css` and `confoo.css` contain a title-link rule targeting a particular background Content Item GUID. Such a selector is not a general title-slide hook.

### Backgrounds, fonts, code, and math

#### Theme backgrounds versus Content Item backgrounds

The theme's `--r-background-color` is the default canvas background. A slide's `BackgroundContent` takes precedence over the deck's `BackgroundContent`; if neither is supplied, the theme background remains. The generated title slide receives the deck background.

LiquidVictor writes the effective image background as a section attribute pointing to `img/<content-item-id><original-extension>`, and copies its bytes to that output path. The filename is based on the Content Item ID, not the original image filename.

RevealJS background images default to `cover` and centered positioning, so cropping can be expected. The current LiquidVictor layout helper does not expose background size, position, opacity, video, or iframe settings as deck theme options. Upstream supports these attributes, but using them requires a renderer change or explicit customization of generated sections in a custom shell. They are not configured by `ThemeName`, and setting an attribute on the shell's outer container is not an equivalent per-slide feature.

With `--MakeSoloImagesFullScreen`, the `FullPage` renderer can promote a sole image to the slide background when the title is blank and `NeverFullScreen` is false. That is layout behavior, not a CSS theme feature.

#### Fonts and static branding assets

Keep CSS-relative asset paths correct. For a stylesheet in `dist\theme`, `url("./fonts/...")` resolves within `dist\theme\fonts`; it is not relative to `index.html`. For example, the copied `black.css` imports the locally bundled Source Sans Pro stylesheet. A logo referenced by your copied shell is instead relative to the shell's output location.

Put static logos, fonts, and other reusable theme assets somewhere inside the selected template tree; the generator copies the entire tree. Do not rely on a file that exists only on your development machine or on a path outside the template.

`bsstahl`, `confoo`, `carvana`, and some upstream-derived themes import Google-hosted fonts. Those imports need network access. Use packaged fonts with appropriate redistribution rights, or system fonts, when offline viewing matters. Preserve the original attribution and license information when copying themes or font assets.

#### Syntax highlighting

The default shell always loads `plugin/highlight/monokai.css`, independently of `ThemeName`. Changing the presentation theme does not automatically change syntax colors.

For a different bundled code theme, change this line in your copied `index.html`:

```html
<link rel="stylesheet" href="plugin/highlight/zenburn.css">
```

Keep the `RevealHighlight` script and plugin registration. For another highlight stylesheet, include that file in the template too. Scope code-color changes carefully because the highlight CSS loads after the main theme and can override its code styling.

#### Math

Markdig generates math markup, and the shell's `RevealMath` plugin renders it. MathJax is fetched from a CDN by default, not fully bundled for offline use. Preserve the plugin and configure locally served MathJax if offline math is required. See [LaTeX support](./latex-support.md) for syntax and configuration details.

### Previewing, printing, and distributing

For a basic visual check after the example build:

```powershell
Start-Process .\ThemeDemo\Output\index.html
```

For realistic testing, serve the output directory through your existing local static HTTP server. This avoids browser restrictions on local files and better represents deployment, especially for speaker-view/plugin behavior and external assets. LiquidVictor does not start a web server.

Check the browser developer tools' Network and Console panels, not only the CLI's success message:

* Confirm the requested theme URL is `dist/theme/acme-night.css` and it loads successfully.
* Confirm computed `--r-background-color`, body text, headings, and link colors match the overrides.
* Check the title page, all layouts used by your deck, fragments, vertical slides, notes, and image backgrounds.
* Check the smallest and largest practical viewports and both `Widescreen` and `Standard` decks.
* Check readable contrast on light/dark backgrounds and visibility of links, controls, and code.
* Test without network access if you promise offline delivery; inspect font and math requests.

#### PDF export of RevealJS

PDF printing remains **RevealJS output**, not the PowerPoint output engine.

1. Open the generated presentation in Chrome/Chromium with `?print-pdf` before its hash, for example `index.html?print-pdf#/`.
2. Wait for fonts, images, and math to load.
3. Open the print dialog and choose Save as PDF, Landscape, no margins, and Background graphics enabled.
4. Inspect the preview and saved PDF for cropping, blank pages, contrast, and notes.

The current shell sets `showNotes` to `true` when the URL contains `print-pdf`, so speaker notes are included by default. If that is undesirable, edit the copied shell's configuration. RevealJS also supports `showNotes: 'separate-page'` for notes on separate pages.

Fragments print as separate incremental pages by default. To show all fragments on one page, add `pdfSeparateFragments: false` to the copied shell's `Reveal.initialize` configuration. Oversized slides can span multiple pages; `pdfMaxPagesPerSlide: 1` limits this but is not a fix for content that does not fit.

The current 5.1.0 runtime handles print mode through the framework styles and print controller. Do not copy the old `css/print/pdf.css` injection from `Sample\Output\index.html` into the current shell just because those historical source files also exist in the template tree.

The example theme hides the global footer elements in print mode. Screen footers are not a supported per-page PDF header/footer mechanism; check the print output explicitly before choosing a different policy.

#### Reproducible delivery

Distribute the **entire generated output directory**, not just `index.html` or the selected CSS. Preserve relative paths when uploading to a static host.

Keep the deck metadata, custom HTML shell, final CSS, fonts/logos, and (if applicable) Sass source/build command under version control in the appropriate repository. Record the RevealJS/compiler versions you tested. Changing `ThemeName` or editing source templates affects future generations; already generated copies do not update automatically.

### Troubleshooting RevealJS themes

| Symptom | Check or action |
| --- | --- |
| Presentation is unstyled or uses only basic framework layout | Inspect the actual CSS request. Verify a nonempty name and a matching file in the selected bundle's `dist\theme`. There is no automatic fallback. |
| Request ends in `.css.css` or points to an unexpected subdirectory | Use a bare theme name without an extension or path. |
| Works locally but not on the host | Match lowercase filenames exactly, preserve folder structure, and check CSS-relative font/image URLs. |
| Sass edits have no effect | Compile to `dist\theme\<name>.css`, then regenerate from that template. LiquidVictor does not compile source files. |
| A rebuild removes custom footers or other styling | Checked-in CSS may have additions absent from Sass. Move those rules into your maintained source before recompiling. |
| Customizations disappear after generating again | Edit the source template/theme, not only the generated output; matching output files are overwritten. |
| Missing assets are unnoticed during generation | A reused output folder can retain stale files. Generate into a fresh directory and inspect browser requests. |
| Footer does not hide on the title page | Check the actual DOM relationship; the bundled sibling selector does not match the shell. Add explicit current-slide state if needed. |
| Theme changes but code colors do not | Change the independent highlight stylesheet in the copied HTML shell. |
| Background artwork is cropped | RevealJS defaults to `cover`. Changing theme colors does not change generated background attributes. |
| Fonts or math fail offline | Replace remote imports/CDN dependencies with locally served assets, or use system fonts where appropriate. |
| Preview resembles the old sample rather than the current generator | Use `Templates\RevealJS`, which loads `dist\...`, not `Sample\Output`, which uses historical `css\...` and `lib\...` paths. |
| `-OutputEngineType:Powerpoint` fails | That output engine is not registered in the current CLI; see the separate section below. |

## PowerPoint output

PowerPoint's visual system is separate from RevealJS. A PowerPoint theme involves Office/Open XML theme parts, fonts/color schemes, slide masters, and slide layouts. A RevealJS CSS file cannot be used as a PowerPoint theme.

### Current support

The [PowerPoint generator](../src/LiquidVictor.Output.Powerpoint.Generator/Engine.cs) exists, but it is a prototype rather than a complete theme-capable output path:

* The active `CreatePresentation` implementation creates a new presentation; it does not load a user `.pptx` or `.potx` template.
* It does not read `SlideDeck.ThemeName` and accepts no template path.
* It hardcodes a 4:3 slide size instead of using the deck's aspect ratio.
* The larger theme/master construction example in that file is commented out and is not executable theme support.
* `CompilePresentation` throws `NotImplementedException`.
* The current CLI only registers `Reveal` / `RevealJS` as presentation builders. Merely having a PowerPoint project in the repository does not make `-OutputEngineType:Powerpoint` a supported command.

Consequently, there is **no supported end-to-end LiquidVictor recipe for adding or selecting a PowerPoint output theme today**. Do not add a CSS file, change `ThemeName`, or point `-TemplatePath` at a `.potx` expecting Office styling.

### Authoring an Office theme separately

If you need a PowerPoint template now, author the colors/fonts and slide-master/layout styling in PowerPoint itself and save an Office template for your manual PowerPoint workflow. This does not register it with LiquidVictor.

Implementing LiquidVictor support would require an explicit PowerPoint template-loading mechanism, correct master/layout/theme relationships, rendering that uses those layouts, and CLI registration with tests. The selection contract would need to be documented independently of RevealJS; the shared string alone does not provide that behavior.

PowerPoint **import** is a different feature: importing content through `LVImport` does not provide an output theme loader or preserve an Office theme as a RevealJS stylesheet.

## Other outputs

* **Hardcoded output:** [the hardcoded builder](../src/LiquidVictor.Output.Hardcoded/Engine.cs) retains the supplied domain deck during compilation. File generation is not implemented, and there is no theme rendering or selection workflow.
* **Table of contents:** the TOC strategy produces Markdown content, not a themed presentation shell. If that content is rendered as part of a RevealJS slide deck, the presentation stylesheet applies there. The current CLI's TOC file-writing branch is not implemented.
* **YAML/exported data:** persisting `ThemeName` preserves a selection string, not a portable rendering theme or its assets. Rendering still requires the corresponding output implementation and template bundle.

For a future output target, document its own asset format, selection semantics, template setup, and validation separately. Do not assume it consumes RevealJS CSS or implements every deck styling property.

## Implementation references and upstream documentation

### LiquidVictor source of truth

* [Domain SlideDeck](../src/LiquidVictor/Entities/SlideDeck.cs) and [SlideDeckBuilder](../src/LiquidVictor/Builders/SlideDeckBuilder.cs): shared metadata and explicit theme assignment.
* [YAML reader](../src/LiquidVictor.Data.YamlFile/SlideDeckReadRepository.cs) and [writer](../src/LiquidVictor.Data.YamlFile/SlideDeckWriteRepository.cs): theme persistence.
* [CLI arguments](../src/LV/ArgumentExtensions.cs) and [builder registrations](../src/LV/ServiceCollectionExtensions.cs): supported options/targets.
* [RevealJS generator](../src/LiquidVictor.Output.RevealJs.Generator/Engine.cs): token replacement and output orchestration.
* [Template copying](../src/LiquidVictor.Output.RevealJs.Generator/Extensions/StringExtensions.cs) and [image writing](../src/LiquidVictor.Output.RevealJs.Generator/Extensions/ContentItemExtensions.cs): asset paths and overwrite behavior.
* [HTML shell](../Templates/RevealJS/index.html), [theme directory](../Templates/RevealJS/dist/theme), and [Sass sources](../Templates/RevealJS/css/theme/source): actual bundled resources.
* [Slide background helper](../src/LiquidVictor.Output.RevealJs/Extensions/SlideExtensions.cs) and [deck size/title helpers](../src/LiquidVictor.Output.RevealJs/Extensions/SlideDeckExtensions.cs): background precedence and dimensions.
* [MultiSlide layout](../src/LiquidVictor.Output.RevealJs.Layout.MultiSlide/Engine.cs) and [FullPage layout](../src/LiquidVictor.Output.RevealJs.Layout.FullPage/Engine.cs): alignment hooks and fullscreen image behavior.
* [Quick reference](./QUICK_REFERENCE.md) and [LaTeX support](./latex-support.md): related workflows.

### RevealJS references

These upstream pages explain framework features. They can describe newer defaults or paths than the bundled 5.1.0 assets; the local shell and generator determine what LiquidVictor actually uses.

* [Themes and CSS custom properties](https://revealjs.com/themes/)
* [Installation and full development setup](https://revealjs.com/installation/)
* [Presentation sizing and scaling](https://revealjs.com/presentation-size/)
* [Slide backgrounds](https://revealjs.com/backgrounds/)
* [Code highlighting and its separate theme](https://revealjs.com/code/)
* [Ready and slide-change events](https://revealjs.com/events/)
* [PDF export, notes, and fragments](https://revealjs.com/pdf-export/)
