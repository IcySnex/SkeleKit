import { copyFileSync, readFileSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const docsDirectory = join(dirname(fileURLToPath(import.meta.url)), '..');
const iconDirectory = join(docsDirectory, '..', 'Assets', 'icon');
const publicDirectory = join(docsDirectory, 'public');

function copySvg(source, destination) {
  const svg = readFileSync(source, 'utf8').replace(/[\t ]+$/gm, '');
  writeFileSync(destination, svg);
}

copySvg(join(iconDirectory, 'bare_light.svg'), join(publicDirectory, 'logo.svg'));
copySvg(join(iconDirectory, 'bare_dark.svg'), join(publicDirectory, 'logo-dark.svg'));
copyFileSync(join(iconDirectory, 'transparent.png'), join(publicDirectory, 'logo-transparent.png'));
copyFileSync(join(iconDirectory, 'transparent.png'), join(publicDirectory, 'favicon.png'));
