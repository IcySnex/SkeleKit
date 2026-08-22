# SkeleKit documentation

The Markdown documentation website for SkeleKit, built with [shadcn-docs-nuxt](https://github.com/ZTL-UwU/shadcn-docs-nuxt).

## Setup

```bash
npm ci
```

## Development

```bash
npm run dev
```

The development and static-build commands copy the current SkeleKit logo and
favicon from `../Assets/icon` into `public` before starting.

## Production

Build the static site:

```bash
npm run generate
```

Build it with the GitHub Pages repository base path:

```bash
NUXT_APP_BASE_URL=/SkeleKit/ npm run generate:pages
```

The repository workflow builds pull requests and deploys `main` to
`https://icysnex.github.io/SkeleKit/`. In the repository settings, select
**GitHub Actions** as the Pages source.

Markdown pages live in `content`. Number prefixes control navigation order and
are removed from public URLs. All Markdown routes are discovered automatically
during static generation.
