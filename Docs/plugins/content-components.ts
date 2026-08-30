import LandingFeatures from '~/components/LandingFeatures.vue';
import LandingHero from '~/components/LandingHero.vue';
import GalleryShowcase from '~/components/GalleryShowcase.vue';

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.component('LandingFeatures', LandingFeatures);
  nuxtApp.vueApp.component('LandingHero', LandingHero);
  nuxtApp.vueApp.component('GalleryShowcase', GalleryShowcase);
});
