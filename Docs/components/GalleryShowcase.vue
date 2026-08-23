<template>
  <section class="gallery-showcase not-prose" aria-label="Gallery app screenshots">
    <div class="gallery-phones">
      <ScreenshotSlot
        v-for="screenshot in screenshots.slice(0, 3)"
        :key="screenshot.lightSrc"
        :screenshot="screenshot"
      />
    </div>

    <ScreenshotSlot :screenshot="screenshots[3]" />
  </section>
</template>

<script setup lang="ts">
type Screenshot = {
  lightSrc: string;
  darkSrc: string;
  alt: string;
  title: string;
  description: string;
  format: 'ipad' | 'iphone';
};

const screenshots: Screenshot[] = [
  {
    lightSrc: 'images/gallery/framework-iphone-light.png',
    darkSrc: 'images/gallery/framework-iphone-dark.png',
    alt: 'Framework section of the SkeleKit Gallery on iPhone',
    title: 'Gallery overview',
    description: 'The main sections of the sample application.',
    format: 'iphone',
  },
  {
    lightSrc: 'images/gallery/map-iphone-light.png',
    darkSrc: 'images/gallery/map-iphone-dark.png',
    alt: 'Collection interactions in the SkeleKit Gallery on iPhone',
    title: 'Native controls',
    description: 'UIKit controls configured through SkeleKit.',
    format: 'iphone',
  },
  {
    lightSrc: 'images/gallery/interactions-iphone-light.png',
    darkSrc: 'images/gallery/interactions-iphone-dark.png',
    alt: 'Map control in the SkeleKit Gallery on iPhone',
    title: 'Layouts',
    description: 'Alignment, spacing, scrolling, and composition.',
    format: 'iphone',
  },
  {
    lightSrc: 'images/gallery/tabs-pad-light.png',
    darkSrc: 'images/gallery/tabs-pad-dark.png',
    alt: 'Adaptive tabs and sidebar in the SkeleKit Gallery on iPad',
    title: 'Gallery on iPad',
    description: 'Adaptive navigation and layouts on a larger screen.',
    format: 'ipad',
  },
];
</script>

<style scoped>
.gallery-showcase {
  display: grid;
  gap: 1rem;
  margin: 2rem 0 2.5rem;
}

.gallery-phones {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 1rem;
}

@media (max-width: 720px) {
  .gallery-phones {
    grid-template-columns: 1fr;
  }
}
</style>
