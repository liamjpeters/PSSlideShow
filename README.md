# PSSlideShow site

This orphan branch contains the Hugo and Tailwind CSS source for the PSSlideShow documentation
site. It follows the layout and system/light/dark theme of
[Liam Peters' website](https://liampeters.co.uk), without the blog features.

## Content

- **Home** introduces the module and shows how to get started.
- **Cmdlets** is generated at build time from the validated PlatyPS Markdown on `main`. Do not
  edit or commit the generated pages in `content/cmdlets/`.
- **Widgets** contains authored examples in `content/widgets/`. A page can optionally set
  `image: "img/widgets/name.png"` and `image_alt: "Description"` in its front matter; store the
  screenshot in `static/img/widgets/`. No image is rendered when those fields are absent.

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

The [Pages workflow](.github/workflows/pages.yml) must exist on **both** `main` and `gh-pages`:
GitHub loads push-triggered workflows from the branch that changed. On a push to either branch,
it checks out the latest `main` for the module and `gh-pages` for the site, validates the command
help, builds with Hugo and Tailwind, and deploys a Pages artifact. Set the repository's GitHub
Pages source to **GitHub Actions**; the generated HTML does not live in this branch.
