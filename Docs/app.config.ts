const baseURL = process.env.NUXT_APP_BASE_URL || '/';
const siteOrigin = (process.env.NUXT_SITE_URL || 'https://icysnex.github.io').replace(/\/$/, '');

export default defineAppConfig({
  shadcnDocs: {
    site: {
      name: 'SkeleKit',
      description: 'Native, code-first UI for .NET for iOS. UIKit underneath, clean C# on top.',
      ogImage: `${siteOrigin}${baseURL}favicon.png`,
      umami: {
        enable: false,
      },
    },
    theme: {
      customizable: true,
      color: 'violet',
      radius: 0.625,
    },
    header: {
      title: 'SkeleKit',
      showTitle: true,
      border: false,
      darkModeToggle: true,
      languageSwitcher: {
        enable: false,
        triggerType: 'icon',
        dropdownType: 'select',
      },
      logo: {
        light: '/logo.svg',
        dark: '/logo-dark.svg',
      },
      nav: [
        {
          title: 'Guides',
          to: '/guides/application',
          target: '_self',
          showLinkIcon: false,
        },
        {
          title: 'API Reference',
          to: '/reference',
          target: '_self',
          showLinkIcon: false,
        },
      ],
      links: [
        {
          icon: 'lucide:box',
          to: 'https://www.nuget.org/packages/SkeleKit.iOS',
          target: '_blank',
        },
        {
          icon: 'lucide:github',
          to: 'https://github.com/IcySnex/SkeleKit',
          target: '_blank',
        },
      ],
    },
    aside: {
      useLevel: true,
      levelStyle: 'aside',
      collapse: false,
    },
    main: {
      padded: true,
      breadCrumb: true,
      showTitle: true,
      editLink: {
        enable: true,
        pattern: 'https://github.com/IcySnex/SkeleKit/edit/main/Docs/content/:path',
        text: 'Edit this page',
        icon: 'lucide:square-pen',
        placement: ['docsFooter'],
      },
      backToTop: true,
    },
    footer: {
      border: true,
      credits: 'Copyright © 2026 SkeleKit contributors',
      links: [
        {
          icon: 'lucide:box',
          title: 'NuGet',
          to: 'https://www.nuget.org/packages/SkeleKit.iOS',
          target: '_blank',
        },
        {
          icon: 'lucide:github',
          title: 'GitHub',
          to: 'https://github.com/IcySnex/SkeleKit',
          target: '_blank',
        },
      ],
    },
    toc: {
      enable: true,
      enableInMobile: true,
      progressBar: true,
      links: [
        {
          title: 'View source',
          icon: 'lucide:github',
          to: 'https://github.com/IcySnex/SkeleKit',
          target: '_blank',
        },
        {
          title: 'Report an issue',
          icon: 'lucide:circle-dot',
          to: 'https://github.com/IcySnex/SkeleKit/issues',
          target: '_blank',
        },
      ],
    },
    search: {
      enable: true,
      inAside: false,
      style: 'input',
    },
  },
});
