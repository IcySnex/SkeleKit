import { readdirSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const baseURL = process.env.NUXT_APP_BASE_URL || '/';
const siteOrigin = (process.env.NUXT_SITE_URL || 'https://icysnex.github.io').replace(/\/$/, '');
const contentDirectory = join(dirname(fileURLToPath(import.meta.url)), 'content');

function getContentRoutes(directory: string, segments: string[] = []): string[] {
  return readdirSync(directory, { withFileTypes: true }).flatMap((entry) => {
    const name = entry.name.replace(/^\d+\./, '');

    if (entry.isDirectory()) {
      return getContentRoutes(join(directory, entry.name), [...segments, name]);
    }

    if (!entry.isFile() || !entry.name.endsWith('.md') || entry.name.startsWith('_')) {
      return [];
    }

    const page = name.replace(/\.md$/, '');
    const routeSegments = page === 'index' ? segments : [...segments, page];
    return [`/${routeSegments.join('/')}`];
  });
}

export default defineNuxtConfig({
  devtools: { enabled: false },
  extends: ['shadcn-docs-nuxt'],
  app: {
    baseURL,
    head: {
      link: [{ rel: 'icon', type: 'image/png', href: `${baseURL}favicon.png` }],
      meta: [
        { name: 'theme-color', content: '#09090b', media: '(prefers-color-scheme: dark)' },
        { name: 'theme-color', content: '#ffffff', media: '(prefers-color-scheme: light)' },
      ],
    },
  },
  colorMode: {
    preference: 'dark',
    fallback: 'dark',
    storageKey: 'skelekit-color-mode',
  },
  site: {
    url: siteOrigin,
  },
  i18n: {
    baseUrl: siteOrigin,
    defaultLocale: 'en',
    locales: [
      {
        code: 'en',
        name: 'English',
        language: 'en-US',
      },
    ],
  },
  icon: {
    clientBundle: {
      icons: [
        'lucide:box',
        'lucide:circle-dot',
        'lucide:clipboard',
        'lucide:square-pen',
        'lucide:app-window-mac',
        'lucide:book-open',
        'lucide:braces',
        'lucide:boxes',
        'lucide:github',
        'lucide:layout-grid',
        'lucide:rocket',
        'lucide:shield-check',
        'lucide:sparkles',
      ],
    },
  },
  content: {
    highlight: {
      preload: ['csharp'],
    },
  },
  ogImage: {
    enabled: false,
  },
  nitro: {
    prerender: {
      routes: getContentRoutes(contentDirectory),
      crawlLinks: false,
      failOnError: true,
    },
  },
  compatibilityDate: '2025-05-13',
});
