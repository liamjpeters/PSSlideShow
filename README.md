# PSSlideShow site

This orphan branch contains the Hugo and Tailwind CSS source for the PSSlideShow documentation
site. It uses a responsive, card-based layout, sticky navigation, and a system/light/dark theme
without blog features.

## Content

- **Home** introduces the module and shows how to get started.
- **Cmdlets** is generated at build time from the validated PlatyPS Markdown on `main`. Do not
  edit or commit the generated pages in `content/cmdlets/`.
- **Widgets** contains authored examples in `content/widgets/`. A page can optionally set
  `image: "img/widgets/name.png"` and `image_alt: "Description"` in its front matter; store the
  screenshot in `static/img/widgets/`. For separate light- and dark-theme screenshots, set both
  `image_light` and `image_dark` instead of `image`; the page switches between them with the site
  theme. `image_alt` is shared by both. For example, add these fields to `content/widgets/alert.md`:

  ```yaml
  image_light: "img/widgets/alert-light.png"
  image_dark: "img/widgets/alert-dark.png"
  image_alt: "Alert widget example"
  ```

  The paths are relative to `static/`. For an image inside the article body, use the
  `theme-image` shortcode. Pass either one `src` or both `light` and `dark`, plus required `alt`
  text:

  ```markdown
  {{</* theme-image light="img/widgets/alert-light.png" dark="img/widgets/alert-dark.png" alt="Terminal slide showing alert examples." */>}}

  {{</* theme-image src="img/widgets/alert.png" alt="Terminal slide showing alert examples." */>}}
  ```

  Shortcode images switch with the site theme; a single `src` image is used in every theme. No
  hero image is rendered when none of the front-matter image fields is set.

## Build locally

Install Hugo Extended 0.148.1+, the standalone Tailwind CSS v4 CLI, PowerShell 7.6+, the .NET 10
SDK, and Microsoft.PowerShell.PlatyPS v1. With this branch checked out, use a separate checkout
of `main` for the module source:

```powershell
git worktree add ..\PSSlideShow-main main
pwsh -NoProfile -File ..\PSSlideShow-main\build.ps1 -Configuration Debug -SkipTests -Docs
pwsh -NoProfile -File .\scripts\Sync-CmdletHelp.ps1 -ModuleRoot ..\PSSlideShow-main
hugo server
```

If `main` is already checked out elsewhere, pass that directory to `-ModuleRoot` instead of
creating another worktree. The generated cmdlet pages, Hugo build output, and build statistics
are ignored by Git.

## Deploy

The [Pages workflow](.github/workflows/pages.yml) builds when site source changes on `gh-pages`.
A small workflow on `main` dispatches it when module source, help, or build inputs change, so
deployments always run from the branch allowed by the `github-pages` environment. README-only and
`.gitignore` changes do not redeploy the site. Each build checks out the latest `main` for the
module and `gh-pages` for the site, validates the command help, builds with Hugo and Tailwind, and
deploys a Pages artifact. Set the repository's GitHub Pages source to **GitHub Actions**; the
generated HTML does not live in this branch.
