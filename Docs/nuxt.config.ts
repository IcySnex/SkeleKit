import { readdirSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const baseURL = process.env.NUXT_APP_BASE_URL || '/';
const siteOrigin = (process.env.NUXT_SITE_URL || 'https://icysnex.github.io').replace(/\/$/, '');
const docsDirectory = dirname(fileURLToPath(import.meta.url));
const contentDirectory = join(docsDirectory, 'content');
const proseOverrides = new Map([
  ['ProseH1', join(docsDirectory, 'internal/components/DocsProseH1.vue')],
  ['ProseH2', join(docsDirectory, 'internal/components/DocsProseH2.vue')],
  ['ProseP', join(docsDirectory, 'internal/components/DocsProseP.vue')],
]);
const disabledOgImageComposable = join(docsDirectory, 'internal/defineOgImage.ts');

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
  hooks: {
    'components:extend'(components) {
      for (const component of components) {
        const override = proseOverrides.get(component.pascalName);
        if (override) {
          component.filePath = override;
          component.declarationPath = override;
        }
      }
    },
    'imports:extend'(imports) {
      imports.push({
        name: 'defineOgImage',
        from: disabledOgImageComposable,
        priority: 100,
      });
    },
  },
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
        'lucide:accessibility',
        'lucide:activity',
        'lucide:app-window',
        'lucide:arrow-right',
        'lucide:arrow-up-right',
        'lucide:blocks',
        'lucide:box',
        'lucide:boxes',
        'lucide:calendar-days',
        'lucide:chart-no-axes-column-increasing',
        'lucide:circle-dot',
        'lucide:circle-help',
        'lucide:circle-plus',
        'lucide:clipboard',
        'lucide:code-xml',
        'lucide:component',
        'lucide:cpu',
        'lucide:book-open',
        'lucide:download',
        'lucide:ellipsis',
        'lucide:external-link',
        'lucide:file-stack',
        'lucide:files',
        'lucide:folder-open',
        'lucide:gauge',
        'lucide:github',
        'lucide:globe',
        'lucide:grid-3x3',
        'lucide:image',
        'lucide:images',
        'lucide:layers',
        'lucide:layers-3',
        'lucide:layout-grid',
        'lucide:lightbulb',
        'lucide:layout-template',
        'lucide:library',
        'lucide:list',
        'lucide:list-checks',
        'lucide:list-restart',
        'lucide:loader-circle',
        'lucide:lock-keyhole',
        'lucide:map',
        'lucide:message-square-warning',
        'lucide:minus',
        'lucide:moon',
        'lucide:mouse-pointer-click',
        'lucide:move-vertical',
        'lucide:notebook-pen',
        'lucide:palette',
        'lucide:panel-left',
        'lucide:panel-top',
        'lucide:panels-top-left',
        'lucide:refresh-cw',
        'lucide:rocket',
        'lucide:route',
        'lucide:rows-3',
        'lucide:ruler',
        'lucide:share-2',
        'lucide:sliders-horizontal',
        'lucide:smartphone',
        'lucide:square-pen',
        'lucide:sun',
        'lucide:sun-moon',
        'lucide:swatch-book',
        'lucide:text',
        'lucide:text-cursor-input',
        'lucide:toggle-left',
        'lucide:type',
        'lucide:vibrate',
        'lucide:wrench',
      ],
    },
  },
  content: {
    highlight: {
      preload: ['csharp', 'xml'],
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
